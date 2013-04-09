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
using System.IO;
using System.Text;
using RomRaider;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Handler.File;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.File
{
	public sealed class FileLoggerImpl : FileLogger
	{
		private static readonly string NEW_LINE = Runtime.GetProperty("line.separator");

		private readonly SimpleDateFormat dateFormat = new SimpleDateFormat("yyyyMMdd_HHmmss"
			);

		private readonly SimpleDateFormat timestampFormat = new SimpleDateFormat("HH:mm:ss.SSS"
			);

		private readonly Settings settings;

		private readonly MessageListener messageListener;

		private bool started;

		private OutputStream os;

		private long startTimestamp;

		private bool zero;

		public FileLoggerImpl(Settings settings, MessageListener messageListener)
		{
			ParamChecker.CheckNotNull(settings, messageListener);
			this.settings = settings;
			this.messageListener = messageListener;
		}

		public void Start()
		{
			if (!started)
			{
				Stop();
				try
				{
					string filePath = BuildFilePath();
					os = new BufferedOutputStream(new FileOutputStream(filePath));
					messageListener.ReportMessageInTitleBar("Started logging to file: " + FormatFilename
						.GetShortName(filePath));
					zero = true;
				}
				catch (Exception e)
				{
					Stop();
					throw new FileLoggerException(e);
				}
				started = true;
			}
		}

		public void Stop()
		{
			if (os != null)
			{
				try
				{
					os.Close();
					messageListener.ReportMessageInTitleBar("Stopped logging to file.");
				}
				catch (System.Exception e)
				{
					throw new FileLoggerException(e);
				}
			}
			started = false;
		}

		public bool IsStarted()
		{
			return started;
		}

		public void WriteHeaders(string headers)
		{
			string timeHeader = "Time";
			if (!settings.IsFileLoggingAbsoluteTimestamp())
			{
				timeHeader = timeHeader + " (msec)";
			}
			WriteText(timeHeader + headers);
		}

		public void WriteLine(string line, long timestamp)
		{
			WriteText(PrependTimestamp(line, timestamp));
		}

		private void WriteText(string text)
		{
			try
			{
				os.Write(Sharpen.Runtime.GetBytesForString(text));
				if (!text.EndsWith(NEW_LINE))
				{
					os.Write(Sharpen.Runtime.GetBytesForString(NEW_LINE));
				}
			}
			catch (System.Exception e)
			{
				Stop();
				throw new FileLoggerException(e);
			}
		}

		private string PrependTimestamp(string line, long timestamp)
		{
			string formattedTimestamp;
			if (settings.IsFileLoggingAbsoluteTimestamp())
			{
				formattedTimestamp = timestampFormat.Format(Sharpen.Extensions.CreateDate(timestamp
					));
			}
			else
			{
				if (zero)
				{
					formattedTimestamp = "0";
					startTimestamp = Runtime.CurrentTimeMillis();
					zero = false;
				}
				else
				{
					formattedTimestamp = (Runtime.CurrentTimeMillis() - startTimestamp).ToString();
				}
			}
			return new StringBuilder(formattedTimestamp).Append(line).ToString();
		}

		private string BuildFilePath()
		{
			string logDir = settings.GetLoggerOutputDirPath();
			if (!logDir.EndsWith(FilePath.separator))
			{
				logDir += FilePath.separator;
			}
			logDir += "romraiderlog_";
			if (settings.GetLogfileNameText() != null && !settings.GetLogfileNameText().IsEmpty
				())
			{
				logDir += settings.GetLogfileNameText() + "_";
			}
			logDir += dateFormat.Format(new DateTime()) + ".csv";
			return logDir;
		}
	}
}
