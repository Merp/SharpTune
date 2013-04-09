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

namespace RomRaider.Maps
{
	[System.Serializable]
	public class RomID
	{
		private const long serialVersionUID = 7272741253665400643L;

		private string xmlid;

		private int internalIdAddress;

		private string internalIdString;

		private string caseId;

		private string ecuId;

		private string make;

		private string market;

		private string model;

		private string subModel;

		private string transmission;

		private string year = "Unknown";

		private string flashMethod;

		private string memModel;

		private string editStamp;

		private int fileSize;

		private int ramOffset;

		private bool obsolete;

		//ECU version definition
		//ID stored in XML
		//address of ECU version in image
		//ID stored in image
		//ECU hardware version
		//manufacturer
		//trim, ie WRX
		//flash method string used for ecuflash
		//model used for reflashing with ecuflash
		//YYYY-MM-DD and v, the save count for this ROM
		// whether a more recent revision exists
		public override string ToString()
		{
			return string.Format("%n   ---- RomID %s ----" + "%n   Internal ID Address: %s" +
				 "%n   Internal ID String: %s" + "%n   Case ID: %s" + "%n   ECU ID: %s" + "%n   Make: %s"
				 + "%n   Market: %s" + "%n   Model: %s" + "%n   Submodel: %s" + "%n   Transmission: %s"
				 + "%n   Year: %s" + "%n   Flash Method: %s" + "%n   Memory Model: %s" + "%n   ---- End RomID %s ----"
				, xmlid, internalIdAddress, internalIdString, caseId, ecuId, make, market, model
				, subModel, transmission, year, flashMethod, memModel, xmlid);
		}

		public RomID()
		{
			this.internalIdString = string.Empty;
			this.caseId = string.Empty;
		}

		public virtual string GetXmlid()
		{
			return xmlid;
		}

		public virtual void SetXmlid(string xmlid)
		{
			this.xmlid = xmlid;
		}

		public virtual int GetInternalIdAddress()
		{
			return internalIdAddress;
		}

		public virtual void SetInternalIdAddress(int internalIdAddress)
		{
			this.internalIdAddress = internalIdAddress;
		}

		public virtual string GetInternalIdString()
		{
			return internalIdString;
		}

		public virtual void SetInternalIdString(string internalIdString)
		{
			this.internalIdString = internalIdString;
		}

		public virtual string GetCaseId()
		{
			return caseId;
		}

		public virtual void SetCaseId(string caseId)
		{
			this.caseId = caseId;
		}

		public virtual string GetEcuId()
		{
			return ecuId;
		}

		public virtual void SetEcuId(string ecuId)
		{
			this.ecuId = ecuId;
		}

		public virtual string GetMake()
		{
			return make;
		}

		public virtual void SetMake(string make)
		{
			this.make = make;
		}

		public virtual string GetMarket()
		{
			return market;
		}

		public virtual void SetMarket(string market)
		{
			this.market = market;
		}

		public virtual string GetModel()
		{
			return model;
		}

		public virtual void SetModel(string model)
		{
			this.model = model;
		}

		public virtual string GetSubModel()
		{
			return subModel;
		}

		public virtual void SetSubModel(string subModel)
		{
			this.subModel = subModel;
		}

		public virtual string GetTransmission()
		{
			return transmission;
		}

		public virtual void SetTransmission(string transmission)
		{
			this.transmission = transmission;
		}

		public virtual string GetYear()
		{
			return year;
		}

		public virtual void SetYear(string year)
		{
			this.year = year;
		}

		public virtual string GetFlashMethod()
		{
			return flashMethod;
		}

		public virtual void SetFlashMethod(string flashMethod)
		{
			this.flashMethod = flashMethod;
		}

		public virtual string GetMemModel()
		{
			return memModel;
		}

		public virtual void SetMemModel(string memModel)
		{
			this.memModel = memModel;
		}

		public virtual int GetRamOffset()
		{
			return ramOffset;
		}

		public virtual void SetRamOffset(int ramOffset)
		{
			this.ramOffset = ramOffset;
		}

		public virtual int GetFileSize()
		{
			return fileSize;
		}

		public virtual void SetFileSize(int fileSize)
		{
			this.fileSize = fileSize;
		}

		public virtual bool IsObsolete()
		{
			return obsolete;
		}

		public virtual void SetObsolete(bool obsolete)
		{
			this.obsolete = obsolete;
		}

		public virtual string GetEditStamp()
		{
			return editStamp;
		}

		public virtual void SetEditStamp(string editStamp)
		{
			this.editStamp = editStamp;
		}
	}
}
