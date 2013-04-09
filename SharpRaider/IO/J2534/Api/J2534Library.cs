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

using Sharpen;

namespace RomRaider.IO.J2534.Api
{
	/// <summary>
	/// Each class instance holds the Vendor and Library details for a J2534
	/// installed device.
	/// </summary>
	/// <remarks>
	/// Each class instance holds the Vendor and Library details for a J2534
	/// installed device.  These are discovered on the local computer from
	/// keys and value settings in the Windows registry.
	/// </remarks>
	/// <seealso cref="J2534DllLocator"></seealso>
	public class J2534Library
	{
		private string vendor;

		private string library;

		/// <summary>Create a new instance of a J2534 library detail.</summary>
		/// <remarks>Create a new instance of a J2534 library detail.</remarks>
		/// <param name="vendor">- J2534 vendor string</param>
		/// <param name="library">- J2534 library path</param>
		public J2534Library(string vendor, string library)
		{
			this.vendor = vendor;
			this.library = library;
		}

		/// <summary>Get the vendor of this library detail instance.</summary>
		/// <remarks>Get the vendor of this library detail instance.</remarks>
		/// <returns>the vendor</returns>
		public virtual string GetVendor()
		{
			return vendor;
		}

		/// <summary>Get the fully qualified library path of this library detail instance.</summary>
		/// <remarks>Get the fully qualified library path of this library detail instance.</remarks>
		/// <returns>the library</returns>
		public virtual string GetLibrary()
		{
			return library;
		}
	}
}
