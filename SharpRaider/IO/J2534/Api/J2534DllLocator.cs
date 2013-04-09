/*
 * This code is derived from the Java version of RomRaider
 *
 * RomRaider Open-Source Tuning, Logging and Reflashing
 * Copyright (C) 2006-2012 RomRaider.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Collections.Generic;
using Com.Sun.Jna;
using Com.Sun.Jna.Platform.Win32;
using Com.Sun.Jna.Ptr;
using Org.Apache.Log4j;
using RomRaider.IO.J2534.Api;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.J2534.Api
{
	/// <summary>
	/// Discover all the J2534 device installations on the local computer from
	/// keys and value settings in the Windows registry.
	/// </summary>
	/// <remarks>
	/// Discover all the J2534 device installations on the local computer from
	/// keys and value settings in the Windows registry.  Return a List of
	/// J2534Library instances.
	/// </remarks>
	/// <seealso cref="J2534Library">J2534Library</seealso>
	public class J2534DllLocator
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(J2534DllLocator));

		private static readonly string FUNCTIONLIBRARY = "FunctionLibrary";

		private const int KEY_READ = unchecked((int)(0x20019));

		private const int ERROR_NO_MORE_ITEMS = unchecked((int)(0x103));

		private static Advapi32 advapi32 = Advapi32.INSTANCE;

		/// <exception cref="System.Exception"></exception>
		public static ICollection<J2534Library> ListLibraries(string protocol)
		{
			ICollection<J2534Library> libraries = new HashSet<J2534Library>();
			WinReg.HKEY hklm = WinReg.HKEY_LOCAL_MACHINE;
			string passThru = "SOFTWARE\\PassThruSupport.04.04";
			WinReg.HKEYByReference passThruHandle = GetHandle(hklm, passThru);
			IList<string> vendors = GetKeys(passThruHandle.GetValue());
			foreach (string vendor in vendors)
			{
				WinReg.HKEYByReference vendorKey = GetHandle(passThruHandle.GetValue(), vendor);
				int supported = GetDWord(vendorKey.GetValue(), protocol);
				if (supported == 0)
				{
					continue;
				}
				string library = GetSZ(vendorKey.GetValue(), FUNCTIONLIBRARY);
				LOGGER.Debug(string.Format("Found J2534 Vendor:%s | Library:%s", vendor, library)
					);
				if (ParamChecker.IsNullOrEmpty(library))
				{
					continue;
				}
				libraries.AddItem(new J2534Library(vendor, library));
				advapi32.RegCloseKey(vendorKey.GetValue());
			}
			advapi32.RegCloseKey(passThruHandle.GetValue());
			return libraries;
		}

		/// <exception cref="System.Exception"></exception>
		private static WinReg.HKEYByReference GetHandle(WinReg.HKEY hKey, string lpSubKey
			)
		{
			WinReg.HKEYByReference phkResult = new WinReg.HKEYByReference();
			int ret = advapi32.RegOpenKeyEx(hKey, lpSubKey, 0, KEY_READ, phkResult);
			if (ret != WinError.ERROR_SUCCESS)
			{
				HandleError("RegOpenKeyEx", ret);
			}
			return phkResult;
		}

		private static int Reverse(byte[] bytes, int size)
		{
			ByteBuffer b = ByteBuffer.Wrap(bytes, 0, size);
			return b.Order(ByteOrder.LITTLE_ENDIAN).GetInt();
		}

		/// <exception cref="System.Exception"></exception>
		private static void HandleError(string operation, int status)
		{
			string errString = string.Format("%s error [%d]%n", operation, status);
			throw new Exception(errString);
		}

		/// <exception cref="System.Exception"></exception>
		private static IList<string> GetKeys(WinReg.HKEY hkey)
		{
			int dwIndex = 0;
			IList<string> vendors = new AList<string>();
			int ret = 0;
			do
			{
				char[] lpName = new char[255];
				IntByReference lpcName = new IntByReference(-1);
				IntByReference lpReserved = null;
				char[] lpClass = new char[255];
				IntByReference lpcClass = new IntByReference(-1);
				WinBase.FILETIME lpftLastWriteTime = new WinBase.FILETIME();
				ret = advapi32.RegEnumKeyEx(hkey, dwIndex, lpName, lpcName, lpReserved, lpClass, 
					lpcClass, lpftLastWriteTime);
				switch (ret)
				{
					case WinError.ERROR_SUCCESS:
					{
						dwIndex++;
						vendors.AddItem(Native.ToString(lpName));
						break;
					}

					case ERROR_NO_MORE_ITEMS:
					{
						break;
					}

					default:
					{
						HandleError("RegEnumKeyEx", ret);
						break;
					}
				}
			}
			while (ret == WinError.ERROR_SUCCESS);
			return vendors;
		}

		/// <exception cref="System.Exception"></exception>
		private static int GetDWord(WinReg.HKEY hkey, string valueName)
		{
			IntByReference lpType = new IntByReference(-1);
			byte[] lpData = new byte[16];
			IntByReference lpcbData = new IntByReference(-1);
			int ret = advapi32.RegQueryValueEx(hkey, valueName, 0, lpType, lpData, lpcbData);
			int dword = -1;
			switch (ret)
			{
				case WinError.ERROR_SUCCESS:
				{
					dword = Reverse(lpData, lpcbData.GetValue());
					break;
				}

				case WinError.ERROR_FILE_NOT_FOUND:
				{
					dword = 0;
					break;
				}

				default:
				{
					string errString = string.Format("DWORD_RegQueryValueEx(%s)", valueName);
					HandleError(errString, ret);
					break;
				}
			}
			return dword;
		}

		/// <exception cref="System.Exception"></exception>
		private static string GetSZ(WinReg.HKEY hkey, string valueName)
		{
			IntByReference lpType = new IntByReference(-1);
			char[] lpData = new char[1024];
			IntByReference lpcbData = new IntByReference(-1);
			int ret = advapi32.RegQueryValueEx(hkey, valueName, 0, lpType, lpData, lpcbData);
			string szValue = null;
			switch (ret)
			{
				case WinError.ERROR_SUCCESS:
				{
					szValue = Native.ToString(lpData);
					break;
				}

				case WinError.ERROR_FILE_NOT_FOUND:
				{
					break;
				}

				default:
				{
					string errString = string.Format("SZ_RegQueryValueEx(%s)", valueName);
					HandleError(errString, ret);
					break;
				}
			}
			return szValue;
		}
	}
}
