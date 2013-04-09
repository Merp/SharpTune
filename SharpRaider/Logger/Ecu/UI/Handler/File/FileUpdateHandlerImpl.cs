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

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using RomRaider;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Handler.File;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.File
{
	public sealed class FileUpdateHandlerImpl : FileUpdateHandler, ConvertorUpdateListener
	{
		private readonly IDictionary<LoggerData, int> loggerDatas = Sharpen.Collections.SynchronizedMap
			(new LinkedHashMap<LoggerData, int>());

		private readonly IList<StatusChangeListener> listeners = Sharpen.Collections.SynchronizedList
			(new AList<StatusChangeListener>());

		private readonly FileLogger fileLogger;

		private FileUpdateHandlerImpl.Line currentLine;

		public FileUpdateHandlerImpl(Settings settings, MessageListener messageListener)
		{
			currentLine = new FileUpdateHandlerImpl.Line(this, loggerDatas.Keys);
			fileLogger = new FileLoggerImpl(settings, messageListener);
		}

		public void AddListener(StatusChangeListener listener)
		{
			lock (this)
			{
				ParamChecker.CheckNotNull(listener, "listener");
				listeners.AddItem(listener);
			}
		}

		public void RegisterData(LoggerData loggerData)
		{
			lock (this)
			{
				if (loggerDatas.Keys.Contains(loggerData))
				{
					loggerDatas.Put(loggerData, loggerDatas.Get(loggerData) + 1);
				}
				else
				{
					loggerDatas.Put(loggerData, 1);
					ResetLine();
					WriteHeaders();
				}
			}
		}

		public void HandleDataUpdate(Response response)
		{
			lock (this)
			{
				if (fileLogger.IsStarted())
				{
					foreach (LoggerData loggerData in response.GetData())
					{
						currentLine.UpdateParamValue(loggerData, loggerData.GetSelectedConvertor().Format
							(response.GetDataValue(loggerData)));
					}
					if (currentLine.IsFull())
					{
						fileLogger.WriteLine(currentLine.Values(), response.GetTimestamp());
						ResetLine();
					}
				}
			}
		}

		public void DeregisterData(LoggerData loggerData)
		{
			lock (this)
			{
				if (loggerDatas.Keys.Contains(loggerData) && loggerDatas.Get(loggerData) > 1)
				{
					loggerDatas.Put(loggerData, loggerDatas.Get(loggerData) - 1);
				}
				else
				{
					Sharpen.Collections.Remove(loggerDatas, loggerData);
					ResetLine();
					WriteHeaders();
				}
			}
		}

		public void CleanUp()
		{
			lock (this)
			{
				if (fileLogger.IsStarted())
				{
					fileLogger.Stop();
				}
			}
		}

		public void Reset()
		{
			lock (this)
			{
			}
		}

		public void NotifyConvertorUpdate(LoggerData updatedLoggerData)
		{
			lock (this)
			{
				ResetLine();
				WriteHeaders();
			}
		}

		public void Start()
		{
			lock (this)
			{
				if (!fileLogger.IsStarted())
				{
					fileLogger.Start();
					NotifyListeners(true);
					WriteHeaders();
				}
			}
		}

		public void Stop()
		{
			lock (this)
			{
				if (fileLogger.IsStarted())
				{
					fileLogger.Stop();
					NotifyListeners(false);
				}
			}
		}

		private void ResetLine()
		{
			currentLine = new FileUpdateHandlerImpl.Line(this, loggerDatas.Keys);
		}

		private void WriteHeaders()
		{
			if (fileLogger.IsStarted())
			{
				fileLogger.WriteHeaders(currentLine.Headers());
			}
		}

		private void NotifyListeners(bool loggingToFile)
		{
			foreach (StatusChangeListener listener in listeners)
			{
				if (loggingToFile)
				{
					listener.LoggingData();
				}
				else
				{
					listener.ReadingData();
				}
			}
		}

		private sealed class Line
		{
			private readonly CultureInfo userLocale = CultureInfo.CurrentCulture;

			private const char COMMA = ',';

			private const char SEMICOLON = ';';

			private readonly ICollection<string> locales = new HashSet<string>(Arrays.AsList(
				new string[] { "be_BY", "bg_BG", "ca_ES", "cs_CZ", "da_DK", "de_AT", "de_CH", "de_DE"
				, "de_LU", "el_CY", "el_GR", "es_AR", "es_BO", "es_CL", "es_CO", "es_EC", "es_ES"
				, "es_PE", "es_PY", "es_UY", "es_VE", "et_EE", "fi_FI", "fr_BE", "fr_CA", "fr_CH"
				, "fr_FR", "fr_LU", "hr_HR", "hu_HU", "in_ID", "is_IS", "it_CH", "it_IT", "lt_LT"
				, "lv_LV", "mk_MK", "nl_BE", "nl_NL", "no_NO", "no_NO_NY", "pl_PL", "pt_BR", "pt_PT"
				, "ro_RO", "ru_RU", "sk_SK", "sl_SI", "sq_AL", "sr_BA", "sr_CS", "sr_ME", "sr_RS"
				, "sv_SE", "tr_TR", "uk_UA", "vi_VN" }));

			private readonly IDictionary<LoggerData, string> loggerDataValues;

			private readonly char delimiter;

			public Line(FileUpdateHandlerImpl _enclosing, ICollection<LoggerData> loggerDatas
				)
			{
				this._enclosing = _enclosing;
				{
					if (this.locales.Contains(this.userLocale.ToString()))
					{
						this.delimiter = FileUpdateHandlerImpl.Line.SEMICOLON;
					}
					else
					{
						this.delimiter = FileUpdateHandlerImpl.Line.COMMA;
					}
				}
				this.loggerDataValues = new LinkedHashMap<LoggerData, string>();
				foreach (LoggerData loggerData in loggerDatas)
				{
					this.loggerDataValues.Put(loggerData, null);
				}
			}

			public void UpdateParamValue(LoggerData loggerData, string value)
			{
				lock (this)
				{
					if (this.loggerDataValues.ContainsKey(loggerData))
					{
						this.loggerDataValues.Put(loggerData, value);
					}
				}
			}

			public bool IsFull()
			{
				lock (this)
				{
					foreach (LoggerData loggerData in this.loggerDataValues.Keys)
					{
						if (this.loggerDataValues.Get(loggerData) == null)
						{
							return false;
						}
					}
					return true;
				}
			}

			public string Headers()
			{
				lock (this)
				{
					StringBuilder buffer = new StringBuilder();
					foreach (LoggerData loggerData in this.loggerDataValues.Keys)
					{
						buffer.Append(this.delimiter).Append(loggerData.GetName()).Append(" (").Append(loggerData
							.GetSelectedConvertor().GetUnits()).Append(')');
					}
					return buffer.ToString();
				}
			}

			public string Values()
			{
				lock (this)
				{
					StringBuilder buffer = new StringBuilder();
					foreach (LoggerData loggerData in this.loggerDataValues.Keys)
					{
						string value = this.loggerDataValues.Get(loggerData);
						buffer.Append(this.delimiter).Append(value);
					}
					return buffer.ToString();
				}
			}

			private readonly FileUpdateHandlerImpl _enclosing;
		}
	}
}
