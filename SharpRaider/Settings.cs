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
using System.Globalization;
using Java.Awt;
using RomRaider;
using RomRaider.IO.Connection;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Util;
using Sharpen;

namespace RomRaider
{
	[System.Serializable]
	public class Settings
	{
		private const long serialVersionUID = 1026542922680475190L;

		public static readonly string NEW_LINE = Runtime.GetProperty("line.separator");

		public static readonly string TAB = "\t";

		public static readonly string TABLE_CLIPBOARD_FORMAT_ELEMENT = "table-clipboard-format";

		public static readonly string TABLE_CLIPBOARD_FORMAT_ATTRIBUTE = "format-string";

		public static readonly string TABLE_ELEMENT = "table";

		public static readonly string TABLE1D_ELEMENT = "table1D";

		public static readonly string TABLE2D_ELEMENT = "table2D";

		public static readonly string TABLE3D_ELEMENT = "table3D";

		public static readonly string TABLE_HEADER_ATTRIBUTE = "table-header";

		public static readonly string DEFAULT_CLIPBOARD_FORMAT = "Default";

		public static readonly string DEFAULT_TABLE_HEADER = "[Table1D]" + NEW_LINE;

		public static readonly string DEFAULT_TABLE1D_HEADER = string.Empty;

		public static readonly string DEFAULT_TABLE2D_HEADER = "[Table2D]" + NEW_LINE;

		public static readonly string DEFAULT_TABLE3D_HEADER = "[Table3D]" + NEW_LINE;

		public static readonly string AIRBOYS_CLIPBOARD_FORMAT = "Airboys";

		public static readonly string AIRBOYS_TABLE_HEADER = string.Empty;

		public static readonly string AIRBOYS_TABLE1D_HEADER = string.Empty;

		public static readonly string AIRBOYS_TABLE2D_HEADER = "[Table2D]" + NEW_LINE;

		public static readonly string AIRBOYS_TABLE3D_HEADER = "[Table3D]" + TAB;

		public static readonly string CUSTOM_CLIPBOARD_FORMAT = "Custom";

		public static readonly string REPOSITORY_ELEMENT_NAME = "repository-dir";

		public static readonly string REPOSITORY_ATTRIBUTE_NAME = "path";

		public static readonly string ICONS_ELEMENT_NAME = "icons";

		public static readonly string EDITOR_ICONS_ELEMENT_NAME = "editor-toolbar";

		public static readonly string EDITOR_ICONS_SCALE_ATTRIBUTE_NAME = "scale";

		public static readonly string TABLE_ICONS_ELEMENT_NAME = "table-toolbar";

		public static readonly string TABLE_ICONS_SCALE_ATTRIBUTE_NAME = "scale";

		public const int DEFAULT_EDITOR_ICON_SCALE = 50;

		public const int DEFAULT_TABLE_ICON_SCALE = 70;

		private static readonly string ISO15765 = "ISO15765";

		private static readonly string ISO9141 = "ISO9141";

		private static readonly string SYSTEM_NUMFORMAT = "system";

		private static readonly string USER_LANGUAGE = "user.language";

		private static readonly string USER_COUNTRY = "user.country";

		private static readonly string EN_US = "en_US";

		private readonly Dimension windowSize = new Dimension(800, 600);

		private readonly Point windowLocation = new Point();

		private int splitPaneLocation = 150;

		private bool windowMaximized;

		private string recentVersion = "x";

		private FilePath lastImageDir = new FilePath("images");

		private FilePath lastRepositoryDir = new FilePath("repositories");

		private bool obsoleteWarning = true;

		private bool calcConflictWarning = true;

		private bool debug;

		private int userLevel = 1;

		private bool saveDebugTables = true;

		private bool displayHighTables = true;

		private bool valueLimitWarning = true;

		private Font tableFont = new Font("Arial", Font.BOLD, 11);

		private Dimension cellSize = new Dimension(42, 18);

		private Color maxColor = new Color(255, 102, 102);

		private Color minColor = new Color(153, 153, 255);

		private Color highlightColor = new Color(204, 204, 204);

		private Color increaseBorder = new Color(255, 0, 0);

		private Color decreaseBorder = new Color(0, 0, 255);

		private Color axisColor = new Color(255, 255, 255);

		private Color warningColor = new Color(255, 0, 0);

		private int tableClickCount = 1;

		public static readonly string HOME = Runtime.GetProperty("user.home");

		public static readonly string defaultGitUrl = "http://github.com/RomRaider/SubaruDefs.git";

		public static readonly string defaultGitRemote = "romraider";

		public static readonly string gitDefsBaseDir = HOME + "/.RomRaider/SubaruDefs";

		private string gitCurrentRemote;

		private IDictionary<string, string> gitRemotes = new Dictionary<string, string>();

		private string gitCurrentBranch = "/refs/remotes/romraider/Alpha";

		public static readonly string RRECUDEFREPO = HOME + "/.RomRaider/SubaruDefs/RomRaider/ecu/";

		public static readonly string RR_LOGGER_REPO = HOME + "/.RomRaider/SubaruDefs/RomRaider/logger/";

		public static readonly string RR_CARS_REPO = HOME + "/.RomRaider/SubaruDefs/RomRaider/dyno/";

		public long definitionDirDate = 0;

		public static FilePath definitionDir = new FilePath(HOME + "/.RomRaider");

		private Vector<FilePath> ecuDefinitionFiles = new Vector<FilePath>();

		private string carsDefFilePath = RR_CARS_REPO + "cars_def.xml";

		private bool ecuDefExists = true;

		private bool loggerDefExists = true;

		private bool carsDefExists = true;

		private string loggerPort;

		private string loggerPortDefault;

		private static string loggerProtocol = "SSM";

		private string loggerDefFilePath;

		private IDictionary<string, FilePath> availableLoggerDefFiles = new Dictionary<string
			, FilePath>();

		private static string loggerProfileFilePath;

		private string fileLoggingControllerSwitchId = "S20";

		private string loggerOutputDirPath = Runtime.GetProperty("user.home");

		private bool fileLoggingControllerSwitchActive = true;

		private bool fileLoggingAbsoluteTimestamp;

		private string logfileNameText;

		private bool logExternalsOnly;

		private static string userLocale = SYSTEM_NUMFORMAT;

		private Dimension loggerWindowSize = new Dimension(1000, 600);

		private Point loggerWindowLocation = new Point();

		internal bool loggerWindowMaximized;

		private int loggerSelectedTabIndex;

		private bool loggerParameterListState = true;

		private ConnectionProperties loggerConnectionProperties;

		private IDictionary<string, EcuDefinition> loggerEcuDefinitionMap;

		private IDictionary<string, string> loggerPluginPorts;

		private bool loggerRefreshMode;

		private static byte loggerDestinationId = unchecked((int)(0x10));

		private bool fastPoll = true;

		private double loggerDividerLocation = 400;

		private string loggerDebuggingLevel = "info";

		private static string j2534Device;

		private static string transportProtocol = ISO9141;

		private string tableClipboardFormat = DEFAULT_CLIPBOARD_FORMAT;

		private string tableHeader = DEFAULT_TABLE_HEADER;

		private string table1DHeader = DEFAULT_TABLE1D_HEADER;

		private string table2DHeader = DEFAULT_TABLE2D_HEADER;

		private string table3DHeader = DEFAULT_TABLE3D_HEADER;

		private int editorIconScale = DEFAULT_EDITOR_ICON_SCALE;

		private int tableIconScale = DEFAULT_TABLE_ICON_SCALE;

		public Settings()
		{
			// number of clicks to open table
			//TODO make these persistent
			//TODO make this persistent
			//TODO Make persistent
			// defogger switch by default
			// Currently 2 options.  Default and Airboy. Custom is not hooked up.
			//center window by default
			Dimension screenSize = Toolkit.GetDefaultToolkit().GetScreenSize();
			windowLocation.Move(((int)(screenSize.GetWidth() - windowSize.GetWidth()) / 2), (
				(int)(screenSize.GetHeight() - windowSize.GetHeight()) / 2));
			gitCurrentRemote = defaultGitRemote;
			gitRemotes.Put(defaultGitRemote, defaultGitUrl);
		}

		public virtual Dimension GetWindowSize()
		{
			return windowSize;
		}

		public virtual Point GetWindowLocation()
		{
			return windowLocation;
		}

		public virtual void SetWindowSize(Dimension size)
		{
			windowSize.SetSize(size);
		}

		public virtual void SetWindowLocation(Point location)
		{
			windowLocation.SetLocation(location);
		}

		public virtual Vector<FilePath> GetEcuDefinitionFiles()
		{
			return ecuDefinitionFiles;
		}

		public virtual void AddEcuDefinitionFile(FilePath ecuDefinitionFile)
		{
			if (!ecuDefinitionFiles.Contains(ecuDefinitionFile))
			{
				ecuDefinitionFiles.AddItem(ecuDefinitionFile);
			}
		}

		public virtual void SetEcuDefinitionFiles(Vector<FilePath> ecuDefinitionFiles)
		{
			this.ecuDefinitionFiles = ecuDefinitionFiles;
		}

		public virtual FilePath GetLastImageDir()
		{
			return lastImageDir;
		}

		public virtual void SetLastImageDir(FilePath lastImageDir)
		{
			this.lastImageDir = lastImageDir;
		}

		public virtual FilePath GetLastRepositoryDir()
		{
			return lastRepositoryDir;
		}

		public virtual void SetLastRepositoryDir(FilePath lastRepositoryDir)
		{
			this.lastRepositoryDir = lastRepositoryDir;
		}

		public virtual int GetSplitPaneLocation()
		{
			return splitPaneLocation;
		}

		public virtual void SetSplitPaneLocation(int splitPaneLocation)
		{
			this.splitPaneLocation = splitPaneLocation;
		}

		public virtual bool IsWindowMaximized()
		{
			return windowMaximized;
		}

		public virtual void SetWindowMaximized(bool windowMaximized)
		{
			this.windowMaximized = windowMaximized;
		}

		public virtual string GetRomRevisionURL()
		{
			return Version.ROM_REVISION_URL;
		}

		public virtual string GetSupportURL()
		{
			return Version.SUPPORT_URL;
		}

		public virtual Font GetTableFont()
		{
			return tableFont;
		}

		public virtual void SetTableFont(Font tableFont)
		{
			this.tableFont = tableFont;
		}

		public virtual bool IsObsoleteWarning()
		{
			return obsoleteWarning;
		}

		public virtual void SetObsoleteWarning(bool obsoleteWarning)
		{
			this.obsoleteWarning = obsoleteWarning;
		}

		public virtual bool IsDebug()
		{
			return debug;
		}

		public virtual void SetDebug(bool debug)
		{
			this.debug = debug;
		}

		public virtual Dimension GetCellSize()
		{
			return cellSize;
		}

		public virtual void SetCellSize(Dimension cellSize)
		{
			this.cellSize = cellSize;
		}

		public virtual Color GetMaxColor()
		{
			return maxColor;
		}

		public virtual void SetMaxColor(Color maxColor)
		{
			this.maxColor = maxColor;
		}

		public virtual Color GetMinColor()
		{
			return minColor;
		}

		public virtual void SetMinColor(Color minColor)
		{
			this.minColor = minColor;
		}

		public virtual Color GetHighlightColor()
		{
			return highlightColor;
		}

		public virtual void SetHighlightColor(Color highlightColor)
		{
			this.highlightColor = highlightColor;
		}

		public virtual bool IsCalcConflictWarning()
		{
			return calcConflictWarning;
		}

		public virtual void SetCalcConflictWarning(bool calcConflictWarning)
		{
			this.calcConflictWarning = calcConflictWarning;
		}

		public virtual Color GetIncreaseBorder()
		{
			return increaseBorder;
		}

		public virtual void SetIncreaseBorder(Color increaseBorder)
		{
			this.increaseBorder = increaseBorder;
		}

		public virtual Color GetDecreaseBorder()
		{
			return decreaseBorder;
		}

		public virtual void SetDecreaseBorder(Color decreaseBorder)
		{
			this.decreaseBorder = decreaseBorder;
		}

		public virtual Color GetAxisColor()
		{
			return axisColor;
		}

		public virtual void SetAxisColor(Color axisColor)
		{
			this.axisColor = axisColor;
		}

		public virtual int GetUserLevel()
		{
			return userLevel;
		}

		public virtual void SetUserLevel(int userLevel)
		{
			if (userLevel > 5)
			{
				this.userLevel = 5;
			}
			else
			{
				if (userLevel < 1)
				{
					this.userLevel = 1;
				}
				else
				{
					this.userLevel = userLevel;
				}
			}
		}

		public virtual int GetTableClickCount()
		{
			return tableClickCount;
		}

		public virtual void SetTableClickCount(int tableClickCount)
		{
			this.tableClickCount = tableClickCount;
		}

		public virtual string GetRecentVersion()
		{
			return recentVersion;
		}

		public virtual void SetRecentVersion(string recentVersion)
		{
			this.recentVersion = recentVersion;
		}

		public virtual string GetReleaseNotes()
		{
			return Version.RELEASE_NOTES;
		}

		public virtual bool IsSaveDebugTables()
		{
			return saveDebugTables;
		}

		public virtual void SetSaveDebugTables(bool saveDebugTables)
		{
			this.saveDebugTables = saveDebugTables;
		}

		public virtual bool IsDisplayHighTables()
		{
			return displayHighTables;
		}

		public virtual void SetDisplayHighTables(bool displayHighTables)
		{
			this.displayHighTables = displayHighTables;
		}

		public virtual bool IsValueLimitWarning()
		{
			return valueLimitWarning;
		}

		public virtual void SetValueLimitWarning(bool valueLimitWarning)
		{
			this.valueLimitWarning = valueLimitWarning;
		}

		public virtual Color GetWarningColor()
		{
			return warningColor;
		}

		public virtual void SetWarningColor(Color warningColor)
		{
			this.warningColor = warningColor;
		}

		public virtual string GetLoggerPort()
		{
			return loggerPort;
		}

		public virtual void SetLoggerPort(string loggerPort)
		{
			this.loggerPort = loggerPort;
		}

		public virtual string GetLoggerPortDefault()
		{
			return loggerPortDefault;
		}

		public virtual void SetLoggerPortDefault(string loggerPortDefault)
		{
			this.loggerPortDefault = loggerPortDefault;
		}

		public static void SetLoggerProtocol(string protocol)
		{
			RomRaider.Settings.loggerProtocol = protocol;
		}

		public static string GetLoggerProtocol()
		{
			return loggerProtocol;
		}

		public virtual string GetLoggerOutputDirPath()
		{
			return loggerOutputDirPath;
		}

		public virtual Point GetLoggerWindowLocation()
		{
			return loggerWindowLocation;
		}

		public virtual void SetLoggerWindowLocation(Point loggerWindowLocation)
		{
			this.loggerWindowLocation = loggerWindowLocation;
		}

		public virtual bool IsLoggerWindowMaximized()
		{
			return loggerWindowMaximized;
		}

		public virtual void SetLoggerWindowMaximized(bool loggerWindowMaximized)
		{
			this.loggerWindowMaximized = loggerWindowMaximized;
		}

		public virtual Dimension GetLoggerWindowSize()
		{
			return loggerWindowSize;
		}

		public virtual void SetLoggerWindowSize(Dimension loggerWindowSize)
		{
			this.loggerWindowSize = loggerWindowSize;
		}

		public virtual double GetDividerLocation()
		{
			return loggerDividerLocation;
		}

		public virtual void SetLoggerDividerLocation(double dividerLocation)
		{
			this.loggerDividerLocation = dividerLocation;
		}

		public static string GetLoggerProfileFilePath()
		{
			return loggerProfileFilePath;
		}

		public virtual void SetLoggerProfileFilePath(string loggerProfileFilePath)
		{
			RomRaider.Settings.loggerProfileFilePath = loggerProfileFilePath;
		}

		public virtual void SetLoggerOutputDirPath(string loggerOutputDirPath)
		{
			this.loggerOutputDirPath = loggerOutputDirPath;
		}

		public virtual string GetFileLoggingControllerSwitchId()
		{
			return fileLoggingControllerSwitchId;
		}

		public virtual void SetFileLoggingControllerSwitchId(string fileLoggingControllerSwitchId
			)
		{
			ParamChecker.CheckNotNullOrEmpty(fileLoggingControllerSwitchId, "fileLoggingControllerSwitchId"
				);
			this.fileLoggingControllerSwitchId = fileLoggingControllerSwitchId;
		}

		public virtual bool IsFileLoggingControllerSwitchActive()
		{
			return fileLoggingControllerSwitchActive;
		}

		public virtual void SetFileLoggingControllerSwitchActive(bool fileLoggingControllerSwitchActive
			)
		{
			this.fileLoggingControllerSwitchActive = fileLoggingControllerSwitchActive;
		}

		public virtual bool IsFileLoggingAbsoluteTimestamp()
		{
			return fileLoggingAbsoluteTimestamp;
		}

		public virtual void SetFileLoggingAbsoluteTimestamp(bool fileLoggingAbsoluteTimestamp
			)
		{
			this.fileLoggingAbsoluteTimestamp = fileLoggingAbsoluteTimestamp;
		}

		public virtual ConnectionProperties GetLoggerConnectionProperties()
		{
			return loggerConnectionProperties;
		}

		public virtual void SetLoggerConnectionProperties(ConnectionProperties loggerConnectionProperties
			)
		{
			this.loggerConnectionProperties = loggerConnectionProperties;
		}

		public virtual IDictionary<string, EcuDefinition> GetLoggerEcuDefinitionMap()
		{
			return loggerEcuDefinitionMap;
		}

		public virtual void SetLoggerEcuDefinitionMap(IDictionary<string, EcuDefinition> 
			loggerEcuDefinitionMap)
		{
			this.loggerEcuDefinitionMap = loggerEcuDefinitionMap;
		}

		public virtual void SetLoggerSelectedTabIndex(int loggerSelectedTabIndex)
		{
			this.loggerSelectedTabIndex = loggerSelectedTabIndex;
		}

		public virtual int GetLoggerSelectedTabIndex()
		{
			return loggerSelectedTabIndex;
		}

		public virtual IDictionary<string, string> GetLoggerPluginPorts()
		{
			return loggerPluginPorts;
		}

		public virtual void SetLoggerPluginPorts(IDictionary<string, string> loggerPluginPorts
			)
		{
			this.loggerPluginPorts = loggerPluginPorts;
		}

		public virtual void SetLoggerParameterListState(bool ShowListState)
		{
			this.loggerParameterListState = ShowListState;
		}

		public virtual bool GetLoggerParameterListState()
		{
			return loggerParameterListState;
		}

		public virtual void SetRefreshMode(bool selected)
		{
			this.loggerRefreshMode = selected;
		}

		public virtual bool GetRefreshMode()
		{
			return loggerRefreshMode;
		}

		public static void SetDestinationId(byte id)
		{
			loggerDestinationId = id;
		}

		public static byte GetDestinationId()
		{
			return loggerDestinationId;
		}

		public virtual void SetFastPoll(bool state)
		{
			this.fastPoll = state;
		}

		public virtual bool IsFastPoll()
		{
			return fastPoll;
		}

		public virtual void SetLogfileNameText(string text)
		{
			this.logfileNameText = text;
		}

		public virtual string GetLogfileNameText()
		{
			return logfileNameText;
		}

		public virtual void SetLoggerDebuggingLevel(string level)
		{
			this.loggerDebuggingLevel = level;
		}

		public virtual string GetLoggerDebuggingLevel()
		{
			return loggerDebuggingLevel;
		}

		public static void SetJ2534Device(string j2534Device)
		{
			RomRaider.Settings.j2534Device = j2534Device;
		}

		public static string GetJ2534Device()
		{
			return j2534Device;
		}

		public static void SetTransportProtocol(string transport)
		{
			RomRaider.Settings.transportProtocol = transport;
		}

		public static string GetTransportProtocol()
		{
			return transportProtocol;
		}

		public virtual void SetTableClipboardFormat(string formatString)
		{
			this.tableClipboardFormat = formatString;
		}

		public virtual string GetTableClipboardFormat()
		{
			return this.tableClipboardFormat;
		}

		public virtual void SetTableHeader(string header)
		{
			this.tableHeader = header;
		}

		public virtual string GetTableHeader()
		{
			return this.tableHeader;
		}

		public virtual void SetTable1DHeader(string header)
		{
			this.table1DHeader = header;
		}

		public virtual string GetTable1DHeader()
		{
			return this.table1DHeader;
		}

		public virtual void SetTable2DHeader(string header)
		{
			this.table2DHeader = header;
		}

		public virtual string GetTable2DHeader()
		{
			return this.table2DHeader;
		}

		public virtual void SetTable3DHeader(string header)
		{
			this.table3DHeader = header;
		}

		public virtual string GetTable3DHeader()
		{
			return this.table3DHeader;
		}

		public virtual void SetDefaultFormat()
		{
			this.tableClipboardFormat = DEFAULT_CLIPBOARD_FORMAT;
			this.tableHeader = DEFAULT_TABLE_HEADER;
			this.table1DHeader = DEFAULT_TABLE1D_HEADER;
			this.table2DHeader = DEFAULT_TABLE2D_HEADER;
			this.table3DHeader = DEFAULT_TABLE3D_HEADER;
		}

		public virtual void SetAirboysFormat()
		{
			this.tableClipboardFormat = AIRBOYS_CLIPBOARD_FORMAT;
			this.tableHeader = AIRBOYS_TABLE_HEADER;
			this.table1DHeader = AIRBOYS_TABLE1D_HEADER;
			this.table2DHeader = AIRBOYS_TABLE2D_HEADER;
			this.table3DHeader = AIRBOYS_TABLE3D_HEADER;
		}

		public virtual int GetEditorIconScale()
		{
			return this.editorIconScale;
		}

		public virtual void SetEditorIconScale(int scale)
		{
			this.editorIconScale = scale;
		}

		public virtual int GetTableIconScale()
		{
			return this.tableIconScale;
		}

		public virtual void SetTableIconScale(int scale)
		{
			this.tableIconScale = scale;
		}

		public virtual void SetLogExternalsOnly(bool state)
		{
			this.logExternalsOnly = state;
		}

		public virtual bool IsLogExternalsOnly()
		{
			return logExternalsOnly;
		}

		public static bool IsCanBus()
		{
			if (transportProtocol.Equals(ISO15765))
			{
				return true;
			}
			return false;
		}

		public bool IsUsNumberFormat()
		{
			if (Sharpen.Runtime.EqualsIgnoreCase(userLocale, EN_US))
			{
				return true;
			}
			return false;
		}

		public string GetLocale()
		{
			return userLocale;
		}

		public void SetLocale(string locale)
		{
			userLocale = locale;
			if (!Sharpen.Runtime.EqualsIgnoreCase(locale, SYSTEM_NUMFORMAT))
			{
				string[] language = locale.Split("_");
				Runtime.SetProperty(USER_LANGUAGE, language[0]);
				Runtime.SetProperty(USER_COUNTRY, language[1]);
				CultureInfo lc = new CultureInfo(language[0], language[1]);
				System.Threading.Thread.CurrentThread.CurrentCulture = lc;
			}
		}

		public virtual void CheckDefs()
		{
			foreach (FilePath f in FilterCI(Walk(RRECUDEFREPO), ".xml"))
			{
				AddEcuDefinitionFile(f);
			}
			//TODO: verify settings defaults exist, set flags to open DefManager if not.
			if (ecuDefinitionFiles != null && !ecuDefinitionFiles.IsEmpty())
			{
				AList<FilePath> reml = new AList<FilePath>();
				foreach (FilePath s in ecuDefinitionFiles)
				{
					if (!s.Exists())
					{
						ecuDefExists = false;
						reml.AddItem(s);
					}
				}
				//TODO ABSTRACT THIS
				foreach (FilePath r in reml)
				{
					ecuDefinitionFiles.Remove(r);
				}
				if (ecuDefinitionFiles.IsEmpty())
				{
					ecuDefExists = false;
				}
			}
			else
			{
				ecuDefExists = false;
			}
			foreach (FilePath f_1 in FilterCI(Walk(RR_LOGGER_REPO), ".xml"))
			{
				AddAvailableLoggerDefFile(f_1, false);
			}
			if (!availableLoggerDefFiles.IsEmpty())
			{
				if (GetLoggerDefFilePath() == null || !new FilePath(GetLoggerDefFilePath()).Exists
					())
				{
					foreach (FilePath f_2 in availableLoggerDefFiles.Values)
					{
						if (f_2.GetName().ToUpper().Contains("STD") && f_2.GetName().ToUpper().Contains("EN"
							))
						{
							//TODO ABSTRACT THIS and add to settings
							SetLoggerDefFilePath(f_2.GetAbsolutePath());
						}
					}
				}
				if (GetLoggerDefFilePath() == null || !new FilePath(GetLoggerDefFilePath()).Exists
					())
				{
					try
					{
						SetLoggerDefFilePath(availableLoggerDefFiles.Values.Iterator().Next().GetAbsolutePath
							());
					}
					catch (Exception)
					{
						loggerDefExists = false;
					}
				}
			}
			else
			{
				loggerDefExists = false;
			}
			if (!new FilePath(carsDefFilePath).Exists())
			{
				carsDefExists = false;
			}
		}

		public virtual Vector<FilePath> Walk(string path)
		{
			FilePath root = new FilePath(path);
			FilePath[] list = root.ListFiles();
			Vector<FilePath> ret = new Vector<FilePath>();
			foreach (FilePath f in list)
			{
				if (f.IsDirectory())
				{
					Sharpen.Collections.AddAll(ret, Walk(f.GetAbsolutePath()));
				}
				else
				{
					ret.AddItem(f);
				}
			}
			return ret;
		}

		public virtual Vector<FilePath> FilterCI(Vector<FilePath> files, string filter)
		{
			Vector<FilePath> ret = new Vector<FilePath>();
			foreach (FilePath f in files)
			{
				if (f.GetName().ToUpper().Contains(filter.ToUpper()))
				{
					ret.AddItem(f);
				}
			}
			return ret;
		}

		public virtual void RemoveEcuDefinitionFile(FilePath s)
		{
			ecuDefinitionFiles.Remove(s);
		}

		public virtual void RemoveAvailableLoggerDef(FilePath f)
		{
			Sharpen.Collections.Remove(availableLoggerDefFiles, f);
		}

		public virtual bool IsEcuDefExists()
		{
			return ecuDefExists;
		}

		public virtual bool IsCarsDefExists()
		{
			return carsDefExists;
		}

		public virtual bool IsLoggerDefExists()
		{
			return loggerDefExists;
		}

		public virtual string GetCarsDefFilePath()
		{
			return carsDefFilePath;
		}

		public virtual IDictionary<string, FilePath> GetAvailableLoggerDefs()
		{
			return GetAvailableLoggerDefFiles();
		}

		public virtual IDictionary<string, FilePath> GetAvailableLoggerDefFiles()
		{
			return availableLoggerDefFiles;
		}

		public virtual void SetAvailableLoggerDefFiles(IDictionary<string, FilePath> availableLoggerDefs
			)
		{
			availableLoggerDefFiles = availableLoggerDefs;
		}

		public virtual void AddAvailableLoggerDefFile(FilePath f, bool useFullPath)
		{
			if (!GetAvailableLoggerDefFiles().ContainsKey(f))
			{
				if (availableLoggerDefFiles.ContainsValue(f.GetName()) || useFullPath)
				{
					availableLoggerDefFiles.Put(f.GetAbsolutePath(), f);
				}
				else
				{
					GetAvailableLoggerDefFiles().Put(f.GetName(), f);
				}
			}
		}

		public virtual string GetGitBranch()
		{
			return gitCurrentBranch;
		}

		public virtual void SetGitBranch(string gitBranch)
		{
			this.gitCurrentBranch = gitBranch;
		}

		public virtual string GetLoggerDefFilePath()
		{
			return loggerDefFilePath;
		}

		public virtual void SetLoggerDefFilePath(string loggerDefFilePath)
		{
			this.loggerDefFilePath = loggerDefFilePath;
		}

		public static string GetGitDefsBaseDir()
		{
			return gitDefsBaseDir;
		}

		public virtual string GetGitCurrentRemoteName()
		{
			return gitCurrentRemote;
		}

		public virtual string GetGitCurrentRemoteUrl()
		{
			return gitRemotes.Get(GetGitCurrentRemoteName());
		}

		public virtual IDictionary<string, string> GetGitRemotes()
		{
			return gitRemotes;
		}

		public virtual void SetGitRemotes(IDictionary<string, string> gitRemotes)
		{
			this.gitRemotes = gitRemotes;
		}

		public virtual void AddGitRemote(string url, string name)
		{
			this.gitRemotes.Put(name, url);
		}
	}
}
