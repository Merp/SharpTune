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
using System.Text;
using Javax.Swing;
using Org.Apache.Log4j;
using RomRaider.Logger.Ecu.UI.Handler.Table;
using RomRaider.Maps;
using RomRaider.Swing;
using RomRaider.Util;
using RomRaider.Xml;
using Sharpen;

namespace RomRaider.Maps
{
	[System.Serializable]
	public class Rom
	{
		private const long serialVersionUID = 7865405179738828128L;

		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.Maps.Rom
			));

		private RomID romID = new RomID();

		private string fileName = string.Empty;

		private FilePath fullFileName = new FilePath(".");

		private readonly Vector<Table> tables = new Vector<Table>();

		private byte[] binData;

		private string parent = string.Empty;

		private bool isAbstract = false;

		public Rom()
		{
		}

		public virtual void AddTable(Table table)
		{
			bool found = false;
			for (int i = 0; i < tables.Count; i++)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(tables[i].GetName(), table.GetName()))
				{
					tables.Remove(i);
					tables.Add(i, table);
					found = true;
					break;
				}
			}
			if (!found)
			{
				tables.AddItem(table);
			}
		}

		/// <exception cref="RomRaider.Xml.TableNotFoundException"></exception>
		public virtual Table GetTable(string tableName)
		{
			foreach (Table table in tables)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(table.GetName(), tableName))
				{
					return table;
				}
			}
			throw new TableNotFoundException();
		}

		public virtual IList<Table> FindTables(string regex)
		{
			IList<Table> result = new AList<Table>();
			foreach (Table table in tables)
			{
				string name = table.GetName();
				if (name.Matches(regex))
				{
					result.AddItem(table);
				}
			}
			return result;
		}

		public virtual void RemoveTable(string tableName)
		{
			for (int i = 0; i < tables.Count; i++)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(tables[i].GetName(), tableName))
				{
					tables.Remove(i);
				}
			}
		}

		public virtual void PopulateTables(byte[] binData, JProgressPane progress)
		{
			this.binData = binData;
			for (int i = 0; i < GetTables().Count; i++)
			{
				// update progress
				int currProgress = (int)((double)i / (double)GetTables().Count * 40);
				progress.Update("Populating tables...", 50 + currProgress);
				Table table = tables[i];
				try
				{
					// if storageaddress has not been set (or is set to 0) omit table
					if (table.GetStorageAddress() != 0)
					{
						try
						{
							table.PopulateTable(binData);
							TableUpdateHandler.GetInstance().RegisterTable(table);
							if (Sharpen.Runtime.EqualsIgnoreCase(table.GetName(), "Checksum Fix"))
							{
								SetEditStamp(binData, table.GetStorageAddress());
							}
						}
						catch (IndexOutOfRangeException ex)
						{
							LOGGER.Error(table.GetName() + " type " + table.GetType() + " start " + table.GetStorageAddress
								() + " " + binData.Length + " filesize", ex);
							// table storage address extends beyond end of file
							JOptionPane.ShowMessageDialog(table.GetEditor(), "Storage address for table \"" +
								 table.GetName() + "\" is out of bounds.\nPlease check ECU definition file.", "ECU Definition Error"
								, JOptionPane.ERROR_MESSAGE);
							tables.RemoveElementAt(i);
							i--;
						}
					}
					else
					{
						tables.Remove(i);
						// decrement i because length of vector has changed
						i--;
					}
				}
				catch (ArgumentNullException)
				{
					JOptionPane.ShowMessageDialog(table.GetEditor(), "There was an error loading table "
						 + table.GetName(), "ECU Definition Error", JOptionPane.ERROR_MESSAGE);
					tables.RemoveElementAt(i);
				}
			}
		}

		private void SetEditStamp(byte[] binData, int address)
		{
			byte[] stampData = new byte[4];
			System.Array.Copy(binData, address + 204, stampData, 0, stampData.Length);
			string stamp = HexUtil.AsHex(stampData);
			if (Sharpen.Runtime.EqualsIgnoreCase(stamp, "FFFFFFFF"))
			{
				romID.SetEditStamp(string.Empty);
			}
			else
			{
				StringBuilder niceStamp = new StringBuilder(stamp);
				niceStamp.Replace(6, 9, (unchecked((int)(0xFF)) & stampData[3]).ToString());
				niceStamp.Insert(6, " v");
				niceStamp.Insert(4, "-");
				niceStamp.Insert(2, "-");
				niceStamp.Insert(0, "20");
				romID.SetEditStamp(niceStamp.ToString());
			}
		}

		public virtual void SetRomID(RomID romID)
		{
			this.romID = romID;
		}

		public virtual RomID GetRomID()
		{
			return romID;
		}

		public virtual string GetRomIDString()
		{
			return romID.GetXmlid();
		}

		public override string ToString()
		{
			string output = string.Empty;
			output = output + "\n---- Rom ----" + romID.ToString();
			for (int i = 0; i < GetTables().Count; i++)
			{
				output = output + GetTables()[i];
			}
			output = output + "\n---- End Rom ----";
			return output;
		}

		public virtual string GetFileName()
		{
			return fileName;
		}

		public virtual void SetFileName(string fileName)
		{
			this.fileName = fileName;
		}

		public virtual Vector<RomRaider.Maps.Table> GetTables()
		{
			return tables;
		}

		public virtual void ApplyTableColorSettings()
		{
			foreach (RomRaider.Maps.Table table in tables)
			{
				table.ApplyColorSettings();
				//tables.get(i).resize();
				table.GetFrame().Pack();
			}
		}

		public virtual byte[] SaveFile()
		{
			RomRaider.Maps.Table checksum = null;
			foreach (RomRaider.Maps.Table table in tables)
			{
				table.SaveFile(binData);
				if (Sharpen.Runtime.EqualsIgnoreCase(table.GetName(), "Checksum Fix"))
				{
					checksum = table;
				}
			}
			if (checksum != null && !checksum.IsLocked())
			{
				RomChecksum.CalculateRomChecksum(binData, checksum.GetStorageAddress(), checksum.
					GetDataSize());
			}
			else
			{
				if (checksum != null && checksum.IsLocked() && !checksum.IsButtonSelected())
				{
					object[] options = new object[] { "Yes", "No" };
					string message = string.Format("One or more ROM image Checksums is invalid.  " + 
						"Calculate new Checksums?%n" + "(NOTE: this will only fix the Checksums it will NOT repair a corrupt ROM image)"
						);
					int answer = JOptionPane.ShowOptionDialog(checksum.GetEditor(), message, "Checksum Fix"
						, JOptionPane.DEFAULT_OPTION, JOptionPane.QUESTION_MESSAGE, null, options, options
						[0]);
					if (answer == 0)
					{
						RomChecksum.CalculateRomChecksum(binData, checksum.GetStorageAddress(), checksum.
							GetDataSize());
					}
				}
			}
			if (checksum != null)
			{
				byte count = binData[checksum.GetStorageAddress() + 207];
				if (count == -1)
				{
					count = 1;
				}
				else
				{
					count++;
				}
				string currentDate = new SimpleDateFormat("yyMMdd").Format(new DateTime());
				string stamp = string.Format("%s%02x", currentDate, count);
				byte[] romStamp = HexUtil.AsBytes(stamp);
				System.Array.Copy(romStamp, 0, binData, checksum.GetStorageAddress() + 204, 4);
				SetEditStamp(binData, checksum.GetStorageAddress());
			}
			return binData;
		}

		public virtual int GetRealFileSize()
		{
			return binData.Length;
		}

		public virtual FilePath GetFullFileName()
		{
			return fullFileName;
		}

		public virtual void SetFullFileName(FilePath fullFileName)
		{
			this.fullFileName = fullFileName;
			this.SetFileName(fullFileName.GetName());
			foreach (RomRaider.Maps.Table table in tables)
			{
				table.GetFrame().UpdateFileName();
			}
		}

		public virtual string GetParent()
		{
			return parent;
		}

		public virtual void SetParent(string parent)
		{
			this.parent = parent;
		}

		public virtual bool IsAbstract()
		{
			return isAbstract;
		}

		public virtual void SetAbstract(bool isAbstract)
		{
			this.isAbstract = isAbstract;
		}
	}
}
