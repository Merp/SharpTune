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
using Com.Centerkey.Utils;
using Java.Awt;
using Java.Awt.Event;
using Java.Beans;
using Javax.Swing;
using Javax.Swing.Table;
using Org.Apache.Log4j;
using RomRaider;
using RomRaider.Definition;
using RomRaider.Editor.Ecu;
using RomRaider.IO.Serial.Port;
using RomRaider.Logger.Ecu.Comms.Controller;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Comms.Reset;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Logger.Ecu.Profile;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Logger.Ecu.UI.Handler.Dyno;
using RomRaider.Logger.Ecu.UI.Handler.File;
using RomRaider.Logger.Ecu.UI.Handler.Graph;
using RomRaider.Logger.Ecu.UI.Handler.Injector;
using RomRaider.Logger.Ecu.UI.Handler.Livedata;
using RomRaider.Logger.Ecu.UI.Handler.Maf;
using RomRaider.Logger.Ecu.UI.Handler.Table;
using RomRaider.Logger.Ecu.UI.Paramlist;
using RomRaider.Logger.Ecu.UI.Playback;
using RomRaider.Logger.Ecu.UI.Swing.Layout;
using RomRaider.Logger.Ecu.UI.Swing.Menubar;
using RomRaider.Logger.Ecu.UI.Swing.Menubar.Action;
using RomRaider.Logger.Ecu.UI.Swing.Menubar.Util;
using RomRaider.Logger.Ecu.UI.Swing.Vertical;
using RomRaider.Logger.Ecu.UI.Tab.Dyno;
using RomRaider.Logger.Ecu.UI.Tab.Injector;
using RomRaider.Logger.Ecu.UI.Tab.Maf;
using RomRaider.Logger.External.Core;
using RomRaider.Swing;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu
{
	[System.Serializable]
	public sealed class EcuLogger : AbstractFrame, MessageListener
	{
		private const long serialVersionUID = 7145423251696282784L;

		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(RomRaider.Logger.Ecu.EcuLogger));

		private static readonly string ECU_LOGGER_TITLE = Version.PRODUCT_NAME + " v" + Version
			.VERSION + " | ECU/TCU Logger";

		private static readonly string LOGGER_FULLSCREEN_ARG = "-logger.fullscreen";

		private static readonly Uri ICON_PATH = typeof(Settings).GetType().GetResource("/graphics/romraider-ico.gif"
			);

		private static readonly string HEADING_PARAMETERS = "Parameters";

		private static readonly string HEADING_SWITCHES = "Switches";

		private static readonly string HEADING_EXTERNAL = "External";

		private static readonly string CAL_ID_LABEL = "CAL ID";

		private static readonly string FILE_NAME_EXTENTION = "Right-click to select or type text to add to the saved logfile name.";

		private static readonly string ECU_TEXT = "Engine Control Unit Polling. Uncheck both ECU & TCU for Externals logging only";

		private static readonly string TCU_TEXT = "Transmission Control Unit Polling. Un-check both ECU & TCU for Externals logging only";

		private static readonly string[] LOG_FILE_TEXT = new string[] { "1st PT", "2nd PT"
			, "3rd PT", "4th PT", "5th PT", "6th PT", "1st WOT", "2nd WOT", "3rd WOT", "4th WOT"
			, "5th WOT", "6th WOT", "cruising" };

		private static readonly string TOGGLE_LIST_TT_TEXT = "Hides the parameter list and saves the state on exit (F11)";

		private static readonly string UNSELECT_ALL_TT_TEXT = "Un-select all selected parameters/switches on all tabs! (F9)";

		private const byte ECU_ID = unchecked((byte)unchecked((int)(0x10)));

		private const byte TCU_ID = unchecked((byte)unchecked((int)(0x18)));

		private static readonly string LOG_TO_FILE_FK = "F1";

		private static readonly string LOG_TO_FILE_ICON = "/graphics/logger_log_to_file.png";

		private static readonly string LOG_TO_FILE_START = "Start file log";

		private static readonly string LOG_TO_FILE_STOP = "Stop file log";

		private static readonly string LOG_TO_FILE_TT_TEXT = "Start/stop file logging (F1)";

		private static string target = "ECU";

		private static string loadResult = string.Empty;

		private string defVersion;

		private ECUEditor ecuEditor;

		private Settings settings = ECUExec.settings;

		private LoggerController controller;

		private ResetManager resetManager;

		private JLabel messageLabel;

		private JLabel calIdLabel;

		private JLabel ecuIdLabel;

		private JLabel statsLabel;

		private JTabbedPane tabbedPane;

		private SerialPortComboBox portsComboBox;

		private DataUpdateHandlerManager dataHandlerManager;

		private DataRegistrationBroker dataTabBroker;

		private ParameterListTableModel dataTabParamListTableModel;

		private ParameterListTableModel dataTabSwitchListTableModel;

		private ParameterListTableModel dataTabExternalListTableModel;

		private DataUpdateHandlerManager graphHandlerManager;

		private DataRegistrationBroker graphTabBroker;

		private ParameterListTableModel graphTabParamListTableModel;

		private ParameterListTableModel graphTabSwitchListTableModel;

		private ParameterListTableModel graphTabExternalListTableModel;

		private DataUpdateHandlerManager dashboardHandlerManager;

		private DataRegistrationBroker dashboardTabBroker;

		private ParameterListTableModel dashboardTabParamListTableModel;

		private ParameterListTableModel dashboardTabSwitchListTableModel;

		private ParameterListTableModel dashboardTabExternalListTableModel;

		private FileUpdateHandlerImpl fileUpdateHandler;

		private LiveDataTableModel dataTableModel;

		private LiveDataUpdateHandler liveDataUpdateHandler;

		private JSplitPane splitPane;

		private JPanel graphPanel;

		private GraphUpdateHandler graphUpdateHandler;

		private JPanel dashboardPanel;

		private DashboardUpdateHandler dashboardUpdateHandler;

		private MafTab mafTab;

		private MafUpdateHandler mafUpdateHandler;

		private DataUpdateHandlerManager mafHandlerManager;

		private DataRegistrationBroker mafTabBroker;

		private InjectorTab injectorTab;

		private InjectorUpdateHandler injectorUpdateHandler;

		private DataUpdateHandlerManager injectorHandlerManager;

		private DataRegistrationBroker injectorTabBroker;

		private DynoTab dynoTab;

		private DynoUpdateHandler dynoUpdateHandler;

		private DataUpdateHandlerManager dynoHandlerManager;

		private DataRegistrationBroker dynoTabBroker;

		private EcuInit ecuInit;

		private JToggleButton logToFileButton;

		private IList<ExternalDataSource> externalDataSources;

		private IList<EcuParameter> ecuParams;

		private SerialPortRefresher refresher;

		private JWindow startStatus;

		private readonly JLabel startText = new JLabel(" Initializing Logger...");

		private readonly string HOME = Runtime.GetProperty("user.home");

		private StatusIndicator statusIndicator;

		private JProgressBar progressBar;

		private DefinitionRepoManager definitionRepoManager;

		public EcuLogger(Settings settings) : base(ECU_LOGGER_TITLE)
		{
			progressBar = Startbar();
			// PT = Part Throttle
			//TODO handle settings.
			Construct(settings);
		}

		public EcuLogger(ECUEditor ecuEditor) : base(ECU_LOGGER_TITLE)
		{
			progressBar = Startbar();
			this.ecuEditor = ecuEditor;
			Construct(ecuEditor.GetSettings());
		}

		private void Construct(Settings settings)
		{
			// 64-bit won't work with the native libs (e.g. serial rxtx) but won't
			// fail until we actually try to use them since the logger requires
			// these libraries, this is a hard error here
			if (!JREChecker.Is32bit())
			{
				JOptionPane.ShowMessageDialog(null, "Incompatible JRE detected.\n" + Version.PRODUCT_NAME
					 + " ECU Logger requires a 32-bit JRE.\nLogger will now exit.", "JRE Incompatibility Error"
					, JOptionPane.ERROR_MESSAGE);
				// this will generate a NullPointerException because we never got
				// things started
				WindowEvent e = new WindowEvent(this, WindowEvent.WINDOW_CLOSED);
				WindowClosing(e);
			}
			ParamChecker.CheckNotNull(settings);
			this.settings = settings;
			Org.Apache.Log4j.Logger.GetRootLogger().SetLevel(Level.ToLevel(settings.GetLoggerDebuggingLevel
				()));
			LOGGER.Info("Logger locale: " + Runtime.GetProperty("user.language") + "_" + Runtime
				.GetProperty("user.country"));
			if (ecuEditor == null)
			{
				Bootstrap();
				progressBar.SetValue(20);
				startText.SetText(" Loading ECU Defs...");
				LoadEcuDefs();
				progressBar.SetValue(40);
				startText.SetText(" Loading Plugins...");
				progressBar.SetIndeterminate(true);
				LoadLoggerPlugins();
				progressBar.SetIndeterminate(false);
				progressBar.SetValue(60);
				startText.SetText(" Loading ECU Parameters...");
				LoadLoggerParams();
				progressBar.SetValue(80);
				startText.SetText(" Starting Logger...");
				InitControllerListeners();
				InitUserInterface();
				progressBar.SetValue(100);
				InitDataUpdateHandlers();
				StartPortRefresherThread();
				if (!IsLogging())
				{
					StartLogging();
				}
				startStatus.Dispose();
			}
			else
			{
				Bootstrap();
				ecuEditor.statusPanel.Update("Loading ECU Defs...", 20);
				LoadEcuDefs();
				ecuEditor.statusPanel.Update("Loading Plugins...", 40);
				LoadLoggerPlugins();
				ecuEditor.statusPanel.Update("Loading ECU Parameters...", 60);
				LoadLoggerParams();
				ecuEditor.statusPanel.Update("Starting Logger...", 80);
				InitControllerListeners();
				InitUserInterface();
				ecuEditor.statusPanel.Update("Complete...", 100);
				InitDataUpdateHandlers();
				StartPortRefresherThread();
				if (!IsLogging())
				{
					StartLogging();
				}
				ecuEditor.statusPanel.Update("Ready...", 0);
			}
		}

		private void Bootstrap()
		{
			EcuInitCallback ecuInitCallback = new _EcuInitCallback_359(this);
			fileUpdateHandler = new FileUpdateHandlerImpl(settings, this);
			dataTableModel = new LiveDataTableModel();
			liveDataUpdateHandler = new LiveDataUpdateHandler(dataTableModel);
			graphPanel = new JPanel(new BorderLayout(2, 2));
			graphUpdateHandler = new GraphUpdateHandler(graphPanel);
			dashboardPanel = new JPanel(new BetterFlowLayout(FlowLayout.CENTER, 3, 3));
			dashboardUpdateHandler = new DashboardUpdateHandler(dashboardPanel);
			mafUpdateHandler = new MafUpdateHandler();
			injectorUpdateHandler = new InjectorUpdateHandler();
			dynoUpdateHandler = new DynoUpdateHandler();
			controller = new LoggerControllerImpl(settings, ecuInitCallback, this, liveDataUpdateHandler
				, graphUpdateHandler, dashboardUpdateHandler, mafUpdateHandler, injectorUpdateHandler
				, dynoUpdateHandler, fileUpdateHandler, TableUpdateHandler.GetInstance());
			mafHandlerManager = new DataUpdateHandlerManagerImpl();
			mafTabBroker = new DataRegistrationBrokerImpl(controller, mafHandlerManager);
			mafTab = new MafTabImpl(mafTabBroker, ecuEditor);
			mafUpdateHandler.SetMafTab(mafTab);
			injectorHandlerManager = new DataUpdateHandlerManagerImpl();
			injectorTabBroker = new DataRegistrationBrokerImpl(controller, injectorHandlerManager
				);
			injectorTab = new InjectorTabImpl(injectorTabBroker, ecuEditor);
			injectorUpdateHandler.SetInjectorTab(injectorTab);
			dynoHandlerManager = new DataUpdateHandlerManagerImpl();
			dynoTabBroker = new DataRegistrationBrokerImpl(controller, dynoHandlerManager);
			dynoTab = new DynoTabImpl(dynoTabBroker, ecuEditor);
			dynoUpdateHandler.SetDynoTab(dynoTab);
			resetManager = new ResetManagerImpl(settings, this);
			messageLabel = new JLabel(ECU_LOGGER_TITLE);
			calIdLabel = new JLabel(BuildEcuInfoLabelText(CAL_ID_LABEL, null));
			ecuIdLabel = new JLabel(BuildEcuInfoLabelText(target + " ID", null));
			statsLabel = BuildStatsLabel();
			tabbedPane = new JTabbedPane(SwingConstants.BOTTOM);
			portsComboBox = new SerialPortComboBox(settings);
			dataHandlerManager = new DataUpdateHandlerManagerImpl();
			dataTabBroker = new DataRegistrationBrokerImpl(controller, dataHandlerManager);
			dataTabParamListTableModel = new ParameterListTableModel(dataTabBroker, HEADING_PARAMETERS
				);
			dataTabSwitchListTableModel = new ParameterListTableModel(dataTabBroker, HEADING_SWITCHES
				);
			dataTabExternalListTableModel = new ParameterListTableModel(dataTabBroker, HEADING_EXTERNAL
				);
			graphHandlerManager = new DataUpdateHandlerManagerImpl();
			graphTabBroker = new DataRegistrationBrokerImpl(controller, graphHandlerManager);
			graphTabParamListTableModel = new ParameterListTableModel(graphTabBroker, HEADING_PARAMETERS
				);
			graphTabSwitchListTableModel = new ParameterListTableModel(graphTabBroker, HEADING_SWITCHES
				);
			graphTabExternalListTableModel = new ParameterListTableModel(graphTabBroker, HEADING_EXTERNAL
				);
			dashboardHandlerManager = new DataUpdateHandlerManagerImpl();
			dashboardTabBroker = new DataRegistrationBrokerImpl(controller, dashboardHandlerManager
				);
			dashboardTabParamListTableModel = new ParameterListTableModel(dashboardTabBroker, 
				HEADING_PARAMETERS);
			dashboardTabSwitchListTableModel = new ParameterListTableModel(dashboardTabBroker
				, HEADING_SWITCHES);
			dashboardTabExternalListTableModel = new ParameterListTableModel(dashboardTabBroker
				, HEADING_EXTERNAL);
		}

		private sealed class _EcuInitCallback_359 : EcuInitCallback
		{
			public _EcuInitCallback_359(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Callback(EcuInit newEcuInit)
			{
				string ecuId = newEcuInit.GetEcuId();
				RomRaider.Logger.Ecu.EcuLogger.LOGGER.Info(RomRaider.Logger.Ecu.EcuLogger.target 
					+ " ID = " + ecuId);
				if (this._enclosing.ecuInit == null || !this._enclosing.ecuInit.GetEcuId().Equals
					(ecuId))
				{
					this._enclosing.ecuInit = newEcuInit;
					SwingUtilities.InvokeLater(new _Runnable_366(this, ecuId));
				}
			}

			private sealed class _Runnable_366 : Runnable
			{
				public _Runnable_366(_EcuInitCallback_359 _enclosing, string ecuId)
				{
					this._enclosing = _enclosing;
					this.ecuId = ecuId;
				}

				public void Run()
				{
					string calId = this.GetCalId(ecuId);
					string carString = this.GetCarString(ecuId);
					RomRaider.Logger.Ecu.EcuLogger.LOGGER.Info("CAL ID: " + calId + ", Car: " + carString
						);
					this._enclosing._enclosing.calIdLabel.SetText(this._enclosing._enclosing.BuildEcuInfoLabelText
						(RomRaider.Logger.Ecu.EcuLogger.CAL_ID_LABEL, calId));
					this._enclosing._enclosing.ecuIdLabel.SetText(this._enclosing._enclosing.BuildEcuInfoLabelText
						(RomRaider.Logger.Ecu.EcuLogger.target + " ID", ecuId));
					RomRaider.Logger.Ecu.EcuLogger.loadResult = string.Format("Loading logger config for new %s ID: %s, "
						, RomRaider.Logger.Ecu.EcuLogger.target, ecuId);
					this._enclosing._enclosing.LoadLoggerParams();
					this._enclosing._enclosing.LoadUserProfile(Settings.GetLoggerProfileFilePath());
				}

				private string GetCalId(string ecuId)
				{
					IDictionary<string, EcuDefinition> ecuDefinitionMap = this._enclosing._enclosing.
						settings.GetLoggerEcuDefinitionMap();
					if (ecuDefinitionMap == null)
					{
						return null;
					}
					EcuDefinition def = ecuDefinitionMap.Get(ecuId);
					return def == null ? null : def.GetCalId();
				}

				private string GetCarString(string ecuId)
				{
					IDictionary<string, EcuDefinition> ecuDefinitionMap = this._enclosing._enclosing.
						settings.GetLoggerEcuDefinitionMap();
					if (ecuDefinitionMap == null)
					{
						return null;
					}
					EcuDefinition def = ecuDefinitionMap.Get(ecuId);
					return def == null ? null : def.GetCarString();
				}

				private readonly _EcuInitCallback_359 _enclosing;

				private readonly string ecuId;
			}

			private readonly EcuLogger _enclosing;
		}

		public void LoadLoggerParams()
		{
			LoadLoggerConfig();
			LoadFromExternalDataSources();
		}

		private void InitControllerListeners()
		{
			controller.AddListener(dataTabBroker);
			controller.AddListener(graphTabBroker);
			controller.AddListener(dashboardTabBroker);
		}

		private void StartPortRefresherThread()
		{
			refresher = new SerialPortRefresher(portsComboBox, settings.GetLoggerPortDefault(
				));
			ThreadUtil.RunAsDaemon(refresher);
			// wait until port refresher fully started before continuing
			WaitForSerialPortRefresher(refresher);
		}

		private void WaitForSerialPortRefresher(SerialPortRefresher refresher)
		{
			try
			{
				DoWait(refresher);
			}
			catch (PortNotFoundException)
			{
				LOGGER.Warn("Timeout while waiting for serial port refresher - continuing anyway..."
					);
			}
		}

		private void DoWait(SerialPortRefresher refresher)
		{
			long start = Runtime.CurrentTimeMillis();
			while (!refresher.IsStarted())
			{
				CheckSerialPortRefresherTimeout(start);
				ThreadUtil.Sleep(100);
			}
		}

		private void CheckSerialPortRefresherTimeout(long start)
		{
			if (Runtime.CurrentTimeMillis() - start > 2000)
			{
				throw new PortNotFoundException("Timeout while finding serial ports");
			}
		}

		private void InitUserInterface()
		{
			// add menubar to frame
			SetJMenuBar(BuildMenubar());
			// setup main panel
			JPanel mainPanel = new JPanel(new BorderLayout());
			mainPanel.Add(BuildControlToolbar(), BorderLayout.NORTH);
			mainPanel.Add(BuildTabbedPane(), BorderLayout.CENTER);
			mainPanel.Add(BuildStatusBar(), BorderLayout.SOUTH);
			// add to container
			GetContentPane().Add(mainPanel);
		}

		private void LoadEcuDefs()
		{
			try
			{
				IDictionary<string, EcuDefinition> ecuDefinitionMap = new Dictionary<string, EcuDefinition
					>();
				Vector<FilePath> ecuDefFiles = settings.GetEcuDefinitionFiles();
				if (!ecuDefFiles.IsEmpty())
				{
					EcuDataLoader dataLoader = new EcuDataLoaderImpl();
					foreach (FilePath ecuDefFile in ecuDefFiles)
					{
						dataLoader.LoadEcuDefsFromXml(ecuDefFile);
						ecuDefinitionMap.PutAll(dataLoader.GetEcuDefinitionMap());
					}
				}
				settings.SetLoggerEcuDefinitionMap(ecuDefinitionMap);
				LOGGER.Info(string.Format("%d ECU definitions loaded from %d files", ecuDefinitionMap
					.Count, ecuDefFiles.Count));
			}
			catch (System.Exception e)
			{
				ReportError(e);
			}
		}

		private void LoadLoggerConfig()
		{
			string loggerConfigFilePath = settings.GetLoggerDefFilePath();
			if (ParamChecker.IsNullOrEmpty(loggerConfigFilePath))
			{
				ShowMissingConfigDialog();
			}
			else
			{
				try
				{
					EcuDataLoader dataLoader = new EcuDataLoaderImpl();
					dataLoader.LoadConfigFromXml(loggerConfigFilePath, Settings.GetLoggerProtocol(), 
						settings.GetFileLoggingControllerSwitchId(), ecuInit);
					IList<EcuParameter> ecuParams = dataLoader.GetEcuParameters();
					AddConvertorUpdateListeners(ecuParams);
					LoadEcuParams(ecuParams);
					LoadEcuSwitches(dataLoader.GetEcuSwitches());
					if (target.Equals("ECU"))
					{
						InitFileLoggingController(dataLoader.GetFileLoggingControllerSwitch());
					}
					settings.SetLoggerConnectionProperties(dataLoader.GetConnectionProperties());
					if (dataLoader.GetDefVersion() == null)
					{
						defVersion = "na";
					}
					else
					{
						defVersion = dataLoader.GetDefVersion();
					}
					loadResult = string.Format("%sloaded %s: %d parameters, %d switches from def version %s."
						, loadResult, Settings.GetLoggerProtocol(), ecuParams.Count, dataLoader.GetEcuSwitches
						().Count, defVersion);
					LOGGER.Info(loadResult);
				}
				catch (ConfigurationException cfe)
				{
					ReportError(cfe);
					ShowMissingConfigDialog();
				}
				catch (System.Exception e)
				{
					ReportError(e);
				}
			}
		}

		private void ShowMissingConfigDialog()
		{
			object[] options = new object[] { "Yes", "No" };
			int answer = JOptionPane.ShowOptionDialog(this, "Logger definition not configured.\nGo online to download the latest definition file?"
				, "Configuration", JOptionPane.DEFAULT_OPTION, JOptionPane.WARNING_MESSAGE, null
				, options, options[0]);
			if (answer == 0)
			{
				BareBonesBrowserLaunch.OpenURL(Version.LOGGER_DEFS_URL);
			}
			else
			{
				JOptionPane.ShowMessageDialog(this, "The Logger definition file needs to be configured before connecting to the ECU.\nMenu: Settings > Logger Definition Location..."
					, "Configuration", JOptionPane.INFORMATION_MESSAGE);
				ReportError("Logger definition file not found");
			}
		}

		private void LoadLoggerPlugins()
		{
			try
			{
				ExternalDataSourceLoader dataSourceLoader = new ExternalDataSourceLoaderImpl();
				dataSourceLoader.LoadExternalDataSources(settings.GetLoggerPluginPorts());
				externalDataSources = dataSourceLoader.GetExternalDataSources();
			}
			catch (System.Exception e)
			{
				ReportError(e);
			}
		}

		private void LoadFromExternalDataSources()
		{
			try
			{
				IList<ExternalData> externalDatas = GetExternalData(externalDataSources);
				LoadExternalDatas(externalDatas);
				AddExternalConvertorUpdateListeners(externalDatas);
			}
			catch (System.Exception e)
			{
				ReportError(e);
			}
		}

		public void LoadUserProfile(string profileFilePath)
		{
			try
			{
				UserProfileLoader profileLoader = new UserProfileLoaderImpl();
				string path = ParamChecker.IsNullOrEmpty(profileFilePath) ? (HOME + UserProfileLoader
					.BACKUP_PROFILE) : profileFilePath;
				UserProfile profile = profileLoader.LoadProfile(path);
				ApplyUserProfile(profile);
				FilePath profileFile = new FilePath(path);
				if (profileFile.Exists())
				{
					ReportMessageInTitleBar("Profile: " + FormatFilename.GetShortName(profileFile));
				}
			}
			catch (System.Exception e)
			{
				ReportError(e);
			}
		}

		private void InitFileLoggingController(EcuSwitch fileLoggingControllerSwitch)
		{
			controller.SetFileLoggerSwitchMonitor(new FileLoggerControllerSwitchMonitorImpl(fileLoggingControllerSwitch
				, new _FileLoggerControllerSwitchHandler_611(this)));
		}

		private sealed class _FileLoggerControllerSwitchHandler_611 : FileLoggerControllerSwitchHandler
		{
			public _FileLoggerControllerSwitchHandler_611(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
				this.oldDefogStatus = false;
			}

			internal bool oldDefogStatus;

			public void HandleSwitch(double switchValue)
			{
				bool logToFile = (int)switchValue == 1;
				if (this._enclosing.settings.IsFileLoggingControllerSwitchActive() && logToFile !=
					 this.oldDefogStatus)
				{
					this._enclosing.logToFileButton.SetSelected(logToFile);
					if (logToFile)
					{
						this._enclosing.fileUpdateHandler.Start();
					}
					else
					{
						this._enclosing.fileUpdateHandler.Stop();
					}
				}
				this.oldDefogStatus = logToFile;
			}

			private readonly EcuLogger _enclosing;
		}

		private void ApplyUserProfile(UserProfile profile)
		{
			if (profile != null)
			{
				ApplyUserProfileToLiveDataTabParameters(dataTabParamListTableModel, profile);
				ApplyUserProfileToLiveDataTabParameters(dataTabSwitchListTableModel, profile);
				ApplyUserProfileToLiveDataTabParameters(dataTabExternalListTableModel, profile);
				ApplyUserProfileToGraphTabParameters(graphTabParamListTableModel, profile);
				ApplyUserProfileToGraphTabParameters(graphTabSwitchListTableModel, profile);
				ApplyUserProfileToGraphTabParameters(graphTabExternalListTableModel, profile);
				ApplyUserProfileToDashTabParameters(dashboardTabParamListTableModel, profile);
				ApplyUserProfileToDashTabParameters(dashboardTabSwitchListTableModel, profile);
				ApplyUserProfileToDashTabParameters(dashboardTabExternalListTableModel, profile);
			}
		}

		private void ApplyUserProfileToLiveDataTabParameters(ParameterListTableModel paramListTableModel
			, UserProfile profile)
		{
			IList<ParameterRow> rows = paramListTableModel.GetParameterRows();
			foreach (ParameterRow row in rows)
			{
				LoggerData loggerData = row.GetLoggerData();
				SetDefaultUnits(profile, loggerData);
				paramListTableModel.SelectParam(loggerData, IsSelectedOnLiveDataTab(profile, loggerData
					));
			}
		}

		private void ApplyUserProfileToGraphTabParameters(ParameterListTableModel paramListTableModel
			, UserProfile profile)
		{
			IList<ParameterRow> rows = paramListTableModel.GetParameterRows();
			foreach (ParameterRow row in rows)
			{
				LoggerData loggerData = row.GetLoggerData();
				SetDefaultUnits(profile, loggerData);
				paramListTableModel.SelectParam(loggerData, IsSelectedOnGraphTab(profile, loggerData
					));
			}
		}

		private void ApplyUserProfileToDashTabParameters(ParameterListTableModel paramListTableModel
			, UserProfile profile)
		{
			IList<ParameterRow> rows = paramListTableModel.GetParameterRows();
			foreach (ParameterRow row in rows)
			{
				LoggerData loggerData = row.GetLoggerData();
				SetDefaultUnits(profile, loggerData);
				paramListTableModel.SelectParam(loggerData, IsSelectedOnDashTab(profile, loggerData
					));
			}
		}

		private void AddConvertorUpdateListeners(IList<EcuParameter> ecuParams)
		{
			foreach (EcuParameter ecuParam in ecuParams)
			{
				ecuParam.AddConvertorUpdateListener(fileUpdateHandler);
				ecuParam.AddConvertorUpdateListener(liveDataUpdateHandler);
				ecuParam.AddConvertorUpdateListener(graphUpdateHandler);
				ecuParam.AddConvertorUpdateListener(dashboardUpdateHandler);
			}
		}

		private void AddExternalConvertorUpdateListeners(IList<ExternalData> externalDatas
			)
		{
			foreach (ExternalData externalData in externalDatas)
			{
				externalData.AddConvertorUpdateListener(fileUpdateHandler);
				externalData.AddConvertorUpdateListener(liveDataUpdateHandler);
				externalData.AddConvertorUpdateListener(graphUpdateHandler);
				externalData.AddConvertorUpdateListener(dashboardUpdateHandler);
			}
		}

		private void ClearParamTableModels()
		{
			dataTabParamListTableModel.Clear();
			graphTabParamListTableModel.Clear();
			dashboardTabParamListTableModel.Clear();
		}

		private void ClearSwitchTableModels()
		{
			dataTabSwitchListTableModel.Clear();
			graphTabSwitchListTableModel.Clear();
			dashboardTabSwitchListTableModel.Clear();
		}

		private void ClearExternalTableModels()
		{
			dataTabExternalListTableModel.Clear();
			graphTabExternalListTableModel.Clear();
			dashboardTabExternalListTableModel.Clear();
		}

		private void LoadEcuParams(IList<EcuParameter> ecuParams)
		{
			ClearParamTableModels();
			Sort(ecuParams, new EcuDataComparator());
			foreach (EcuParameter ecuParam in ecuParams)
			{
				dataTabParamListTableModel.AddParam(ecuParam, false);
				graphTabParamListTableModel.AddParam(ecuParam, false);
				dashboardTabParamListTableModel.AddParam(ecuParam, false);
			}
			mafTab.SetEcuParams(ecuParams);
			injectorTab.SetEcuParams(ecuParams);
			dynoTab.SetEcuParams(ecuParams);
			this.ecuParams = new AList<EcuParameter>(ecuParams);
		}

		private void LoadEcuSwitches(IList<EcuSwitch> ecuSwitches)
		{
			ClearSwitchTableModels();
			Sort(ecuSwitches, new EcuDataComparator());
			foreach (EcuSwitch ecuSwitch in ecuSwitches)
			{
				dataTabSwitchListTableModel.AddParam(ecuSwitch, false);
				graphTabSwitchListTableModel.AddParam(ecuSwitch, false);
				dashboardTabSwitchListTableModel.AddParam(ecuSwitch, false);
			}
			mafTab.SetEcuSwitches(ecuSwitches);
			injectorTab.SetEcuSwitches(ecuSwitches);
			dynoTab.SetEcuSwitches(ecuSwitches);
		}

		private IList<ExternalData> GetExternalData(IList<ExternalDataSource> externalDataSources
			)
		{
			IList<ExternalData> externalDatas = new AList<ExternalData>();
			foreach (ExternalDataSource dataSource in externalDataSources)
			{
				try
				{
					IList<ExternalDataItem> dataItems = dataSource.GetDataItems();
					foreach (ExternalDataItem item in dataItems)
					{
						externalDatas.AddItem(new ExternalDataImpl(item, dataSource));
					}
				}
				catch (System.Exception e)
				{
					ReportError("Error loading plugin: " + dataSource.GetName() + " v" + dataSource.GetVersion
						(), e);
				}
			}
			return externalDatas;
		}

		private void LoadExternalDatas(IList<ExternalData> externalDatas)
		{
			ClearExternalTableModels();
			Sort(externalDatas, new EcuDataComparator());
			foreach (ExternalData externalData in externalDatas)
			{
				dataTabExternalListTableModel.AddParam(externalData, false);
				graphTabExternalListTableModel.AddParam(externalData, false);
				dashboardTabExternalListTableModel.AddParam(externalData, false);
			}
			mafTab.SetExternalDatas(externalDatas);
			injectorTab.SetExternalDatas(externalDatas);
			dynoTab.SetExternalDatas(externalDatas);
		}

		private void SetDefaultUnits(UserProfile profile, LoggerData loggerData)
		{
			if (profile != null)
			{
				try
				{
					loggerData.SelectConvertor(profile.GetSelectedConvertor(loggerData));
				}
				catch (System.Exception e)
				{
					ReportError(e);
				}
			}
		}

		private bool IsSelectedOnLiveDataTab(UserProfile profile, LoggerData loggerData)
		{
			return profile != null && profile.IsSelectedOnLiveDataTab(loggerData);
		}

		private bool IsSelectedOnGraphTab(UserProfile profile, LoggerData loggerData)
		{
			return profile != null && profile.IsSelectedOnGraphTab(loggerData);
		}

		private bool IsSelectedOnDashTab(UserProfile profile, LoggerData loggerData)
		{
			return profile != null && profile.IsSelectedOnDashTab(loggerData);
		}

		public UserProfile GetCurrentProfile()
		{
			IDictionary<string, UserProfileItem> paramProfileItems = GetProfileItems(dataTabParamListTableModel
				.GetParameterRows(), graphTabParamListTableModel.GetParameterRows(), dashboardTabParamListTableModel
				.GetParameterRows());
			IDictionary<string, UserProfileItem> switchProfileItems = GetProfileItems(dataTabSwitchListTableModel
				.GetParameterRows(), graphTabSwitchListTableModel.GetParameterRows(), dashboardTabSwitchListTableModel
				.GetParameterRows());
			IDictionary<string, UserProfileItem> externalProfileItems = GetProfileItems(dataTabExternalListTableModel
				.GetParameterRows(), graphTabExternalListTableModel.GetParameterRows(), dashboardTabExternalListTableModel
				.GetParameterRows());
			return new UserProfileImpl(paramProfileItems, switchProfileItems, externalProfileItems
				);
		}

		private IDictionary<string, string> GetPluginPorts(IList<ExternalDataSource> externalDataSources
			)
		{
			IDictionary<string, string> plugins = new Dictionary<string, string>();
			foreach (ExternalDataSource dataSource in externalDataSources)
			{
				string id = dataSource.GetId();
				string port = dataSource.GetPort();
				if (port != null && port.Trim().Length > 0)
				{
					plugins.Put(id, port.Trim());
				}
			}
			return plugins;
		}

		private IDictionary<string, UserProfileItem> GetProfileItems(IList<ParameterRow> 
			dataTabRows, IList<ParameterRow> graphTabRows, IList<ParameterRow> dashTabRows)
		{
			IDictionary<string, UserProfileItem> profileItems = new Dictionary<string, UserProfileItem
				>();
			foreach (ParameterRow dataTabRow in dataTabRows)
			{
				string id = dataTabRow.GetLoggerData().GetId();
				string units = dataTabRow.GetLoggerData().GetSelectedConvertor().GetUnits();
				bool dataTabSelected = dataTabRow.IsSelected();
				bool graphTabSelected = IsEcuDataSelected(id, graphTabRows);
				bool dashTabSelected = IsEcuDataSelected(id, dashTabRows);
				profileItems.Put(id, new UserProfileItemImpl(units, dataTabSelected, graphTabSelected
					, dashTabSelected));
			}
			return profileItems;
		}

		private bool IsEcuDataSelected(string id, IList<ParameterRow> parameterRows)
		{
			foreach (ParameterRow row in parameterRows)
			{
				if (id.Equals(row.GetLoggerData().GetId()))
				{
					return row.IsSelected();
				}
			}
			return false;
		}

		private void InitDataUpdateHandlers()
		{
			dataHandlerManager.AddHandler(liveDataUpdateHandler);
			dataHandlerManager.AddHandler(fileUpdateHandler);
			dataHandlerManager.AddHandler(TableUpdateHandler.GetInstance());
			graphHandlerManager.AddHandler(graphUpdateHandler);
			graphHandlerManager.AddHandler(fileUpdateHandler);
			graphHandlerManager.AddHandler(TableUpdateHandler.GetInstance());
			dashboardHandlerManager.AddHandler(dashboardUpdateHandler);
			dashboardHandlerManager.AddHandler(fileUpdateHandler);
			dashboardHandlerManager.AddHandler(TableUpdateHandler.GetInstance());
		}

		private JComponent BuildTabbedPane()
		{
			AddSplitPaneTab("Data", BuildSplitPane(BuildParamListPane(dataTabParamListTableModel
				, dataTabSwitchListTableModel, dataTabExternalListTableModel), BuildDataTab()), 
				BuildUnselectAllButton());
			AddSplitPaneTab("Graph", BuildSplitPane(BuildParamListPane(graphTabParamListTableModel
				, graphTabSwitchListTableModel, graphTabExternalListTableModel), BuildGraphTab()
				), BuildUnselectAllButton());
			AddSplitPaneTab("Dashboard", BuildSplitPane(BuildParamListPane(dashboardTabParamListTableModel
				, dashboardTabSwitchListTableModel, dashboardTabExternalListTableModel), BuildDashboardTab
				()), BuildUnselectAllButton(), BuildToggleGaugeStyleButton());
			tabbedPane.Add("MAF", mafTab.GetPanel());
			tabbedPane.Add("Injector", injectorTab.GetPanel());
			tabbedPane.Add("Dyno", dynoTab.GetPanel());
			return tabbedPane;
		}

		private JButton BuildToggleGaugeStyleButton()
		{
			JButton button = new JButton();
			VerticalTextIcon textIcon = new VerticalTextIcon(button, "Gauge Style", VerticalTextIcon
				.ROTATE_LEFT);
			button.SetIcon(textIcon);
			button.SetPreferredSize(new Dimension(25, 90));
			button.GetInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).Put(KeyStroke.GetKeyStroke(
				"F12"), "toggleGaugeStyle");
			button.GetActionMap().Put("toggleGaugeStyle", new _AbstractAction_854(button));
			button.AddActionListener(new _AbstractAction_862(this));
			return button;
		}

		private sealed class _AbstractAction_854 : AbstractAction
		{
			public _AbstractAction_854(JButton button)
			{
				this.button = button;
				this.serialVersionUID = 6913964758354638587L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				button.DoClick();
			}

			private readonly JButton button;
		}

		private sealed class _AbstractAction_862 : AbstractAction
		{
			public _AbstractAction_862(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 123232894767995264L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.dashboardUpdateHandler.ToggleGaugeStyle();
			}

			private readonly EcuLogger _enclosing;
		}

		private void ClearAllSelectedParameters(ParameterListTableModel paramListTableModel
			)
		{
			IList<ParameterRow> rows = paramListTableModel.GetParameterRows();
			foreach (ParameterRow row in rows)
			{
				if (row.IsSelected())
				{
					row.GetLoggerData().SetSelected(false);
					row.SetSelected(false);
					paramListTableModel.SelectParam(row.GetLoggerData(), false);
				}
			}
			paramListTableModel.FireTableDataChanged();
		}

		private JButton BuildUnselectAllButton()
		{
			JButton button = new JButton();
			button.SetBackground(Color.YELLOW);
			VerticalTextIcon textIcon = new VerticalTextIcon(button, "Un-select ALL", VerticalTextIcon
				.ROTATE_LEFT);
			button.SetToolTipText(UNSELECT_ALL_TT_TEXT);
			button.SetIcon(textIcon);
			button.SetPreferredSize(new Dimension(25, 90));
			button.GetInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).Put(KeyStroke.GetKeyStroke(
				"F9"), "un-selectAll");
			button.GetActionMap().Put("un-selectAll", new _AbstractAction_893(button));
			button.AddActionListener(new _AbstractAction_901(this));
			return button;
		}

		private sealed class _AbstractAction_893 : AbstractAction
		{
			public _AbstractAction_893(JButton button)
			{
				this.button = button;
				this.serialVersionUID = 4913964758354638588L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				button.DoClick();
			}

			private readonly JButton button;
		}

		private sealed class _AbstractAction_901 : AbstractAction
		{
			public _AbstractAction_901(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 723232894767995265L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				try
				{
					this._enclosing.ClearAllSelectedParameters(this._enclosing.dataTabParamListTableModel
						);
					this._enclosing.ClearAllSelectedParameters(this._enclosing.dataTabSwitchListTableModel
						);
					this._enclosing.ClearAllSelectedParameters(this._enclosing.dataTabExternalListTableModel
						);
					this._enclosing.ClearAllSelectedParameters(this._enclosing.graphTabParamListTableModel
						);
					this._enclosing.ClearAllSelectedParameters(this._enclosing.graphTabSwitchListTableModel
						);
					this._enclosing.ClearAllSelectedParameters(this._enclosing.graphTabExternalListTableModel
						);
					this._enclosing.ClearAllSelectedParameters(this._enclosing.dashboardTabParamListTableModel
						);
					this._enclosing.ClearAllSelectedParameters(this._enclosing.dashboardTabSwitchListTableModel
						);
					this._enclosing.ClearAllSelectedParameters(this._enclosing.dashboardTabExternalListTableModel
						);
				}
				catch (System.Exception cae)
				{
					RomRaider.Logger.Ecu.EcuLogger.LOGGER.Error("Un-select ALL error: " + cae);
				}
				finally
				{
					RomRaider.Logger.Ecu.EcuLogger.LOGGER.Info("Un-select all parameters by user action"
						);
				}
			}

			private readonly EcuLogger _enclosing;
		}

		private void AddSplitPaneTab(string name, JSplitPane splitPane, params JComponent
			[] extraControls)
		{
			JToggleButton toggleListButton = new JToggleButton();
			toggleListButton.SetToolTipText(TOGGLE_LIST_TT_TEXT);
			toggleListButton.SetSelected(true);
			VerticalTextIcon textIcon = new VerticalTextIcon(toggleListButton, "Parameter List"
				, VerticalTextIcon.ROTATE_LEFT);
			toggleListButton.SetIcon(textIcon);
			toggleListButton.SetPreferredSize(new Dimension(25, 90));
			toggleListButton.GetInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).Put(KeyStroke.GetKeyStroke
				("F11"), "toggleHideParams");
			toggleListButton.GetActionMap().Put("toggleHideParams", new _AbstractAction_936(toggleListButton
				));
			toggleListButton.AddActionListener(new _AbstractAction_944(this, splitPane, toggleListButton
				));
			if (!settings.GetLoggerParameterListState())
			{
				toggleListButton.DoClick();
			}
			JPanel tabControlPanel = new JPanel(new BetterFlowLayout(FlowLayout.CENTER, 1, 1)
				);
			tabControlPanel.SetPreferredSize(new Dimension(25, 25));
			tabControlPanel.Add(toggleListButton);
			foreach (JComponent control in extraControls)
			{
				tabControlPanel.Add(control);
			}
			JPanel panel = new JPanel(new BorderLayout(0, 0));
			panel.Add(tabControlPanel, BorderLayout.WEST);
			panel.Add(splitPane, BorderLayout.CENTER);
			tabbedPane.Add(name, panel);
		}

		private sealed class _AbstractAction_936 : AbstractAction
		{
			public _AbstractAction_936(JToggleButton toggleListButton)
			{
				this.toggleListButton = toggleListButton;
				this.serialVersionUID = -276854997788647306L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				toggleListButton.DoClick();
			}

			private readonly JToggleButton toggleListButton;
		}

		private sealed class _AbstractAction_944 : AbstractAction
		{
			public _AbstractAction_944(EcuLogger _enclosing, JSplitPane splitPane, JToggleButton
				 toggleListButton)
			{
				this._enclosing = _enclosing;
				this.splitPane = splitPane;
				this.toggleListButton = toggleListButton;
				this.serialVersionUID = -1595098685575657317L;
				this.min = 1;
				this.size = splitPane.GetDividerLocation();
			}

			private const long serialVersionUID;

			private readonly int min;

			public int size;

			public override void ActionPerformed(ActionEvent e)
			{
				int current = splitPane.GetDividerLocation();
				if (toggleListButton.IsSelected())
				{
					splitPane.SetDividerLocation(this.size);
					this._enclosing.settings.SetLoggerParameterListState(true);
				}
				else
				{
					splitPane.SetDividerLocation(this.min);
					this.size = current;
					this._enclosing.settings.SetLoggerParameterListState(false);
				}
			}

			private readonly EcuLogger _enclosing;

			private readonly JSplitPane splitPane;

			private readonly JToggleButton toggleListButton;
		}

		private JComponent BuildParamListPane(ParameterListTableModel paramListTableModel
			, ParameterListTableModel switchListTableModel, ParameterListTableModel externalListTableModel
			)
		{
			JScrollPane paramList = new JScrollPane(BuildParamListTable(paramListTableModel), 
				ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED
				);
			JScrollPane externalList = new JScrollPane(BuildParamListTable(externalListTableModel
				), ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED
				);
			JScrollPane switchList = new JScrollPane(BuildParamListTable(switchListTableModel
				), ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED
				);
			JTabbedPane tabs = new JTabbedPane(JTabbedPane.TOP, JTabbedPane.SCROLL_TAB_LAYOUT
				);
			tabs.AddTab(HEADING_PARAMETERS, paramList);
			tabs.AddTab(HEADING_SWITCHES, switchList);
			tabs.AddTab("External Sensors", externalList);
			return tabs;
		}

		private JTable BuildParamListTable(ParameterListTableModel tableModel)
		{
			JTable paramListTable = new ParameterListTable(tableModel);
			ChangeColumnWidth(paramListTable, 0, 20, 55, 55);
			ChangeColumnWidth(paramListTable, 2, 50, 250, 130);
			return paramListTable;
		}

		private void ChangeColumnWidth(JTable paramListTable, int colIndex, int minWidth, 
			int maxWidth, int preferredWidth)
		{
			TableColumn column = paramListTable.GetColumnModel().GetColumn(colIndex);
			column.SetMinWidth(minWidth);
			column.SetMaxWidth(maxWidth);
			column.SetPreferredWidth(preferredWidth);
		}

		private JComponent BuildStatusBar()
		{
			GridBagLayout gridBagLayout = new GridBagLayout();
			JPanel statusBar = new JPanel(gridBagLayout);
			GridBagConstraints constraints = new GridBagConstraints();
			constraints.anchor = GridBagConstraints.WEST;
			constraints.fill = GridBagConstraints.BOTH;
			JPanel messagePanel = new JPanel(new BorderLayout());
			messagePanel.SetBorder(BorderFactory.CreateLoweredBevelBorder());
			messagePanel.Add(messageLabel, BorderLayout.WEST);
			constraints.gridx = 0;
			constraints.gridy = 0;
			constraints.gridwidth = 2;
			constraints.gridheight = 1;
			constraints.weightx = 10;
			constraints.weighty = 1;
			gridBagLayout.SetConstraints(messagePanel, constraints);
			statusBar.Add(messagePanel);
			JPanel ecuIdPanel = new JPanel(new FlowLayout());
			ecuIdPanel.SetBorder(BorderFactory.CreateLoweredBevelBorder());
			ecuIdPanel.Add(calIdLabel);
			ecuIdPanel.Add(ecuIdLabel);
			constraints.gridx = 2;
			constraints.gridy = 0;
			constraints.gridwidth = 1;
			constraints.gridheight = 1;
			constraints.weightx = 1;
			gridBagLayout.SetConstraints(ecuIdPanel, constraints);
			statusBar.Add(ecuIdPanel);
			JPanel statsPanel = new JPanel(new FlowLayout());
			statsPanel.SetBorder(BorderFactory.CreateLoweredBevelBorder());
			statsPanel.Add(statsLabel);
			constraints.gridx = 3;
			constraints.gridy = 0;
			constraints.gridwidth = 1;
			constraints.gridheight = 1;
			constraints.weightx = 1;
			gridBagLayout.SetConstraints(statsPanel, constraints);
			statusBar.Add(statsPanel);
			return statusBar;
		}

		private string BuildEcuInfoLabelText(string label, string value)
		{
			return label + ": " + (ParamChecker.IsNullOrEmpty(value) ? " Unknown " : value);
		}

		private JSplitPane BuildSplitPane(JComponent leftComponent, JComponent rightComponent
			)
		{
			splitPane = new JSplitPane(JSplitPane.HORIZONTAL_SPLIT, leftComponent, rightComponent
				);
			splitPane.SetDividerSize(5);
			splitPane.SetDividerLocation((int)settings.GetDividerLocation());
			splitPane.AddPropertyChangeListener(this);
			return splitPane;
		}

		private JMenuBar BuildMenubar()
		{
			return new EcuLoggerMenuBar(this, externalDataSources);
		}

		private JPanel BuildControlToolbar()
		{
			JPanel controlPanel = new JPanel(new BorderLayout());
			controlPanel.Add(BuildPortsComboBox(), BorderLayout.WEST);
			//TODO: Finish log playback stuff...
			//        controlPanel.add(buildPlaybackControls(), CENTER);
			controlPanel.Add(BuildStatusIndicator(), BorderLayout.EAST);
			return controlPanel;
		}

		private Component BuildPlaybackControls()
		{
			JButton playButton = new JButton("Play");
			playButton.AddActionListener(new _ActionListener_1078(this));
			JPanel panel = new JPanel();
			panel.Add(playButton);
			return panel;
		}

		private sealed class _ActionListener_1078 : ActionListener
		{
			public _ActionListener_1078(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				ThreadUtil.RunAsDaemon(new _Runnable_1081(this));
			}

			private sealed class _Runnable_1081 : Runnable
			{
				public _Runnable_1081(_ActionListener_1078 _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void Run()
				{
					PlaybackManagerImpl playbackManager = new PlaybackManagerImpl(this._enclosing._enclosing
						.ecuParams, this._enclosing._enclosing.liveDataUpdateHandler, this._enclosing._enclosing
						.graphUpdateHandler, this._enclosing._enclosing.dashboardUpdateHandler, this._enclosing
						._enclosing.mafUpdateHandler, this._enclosing._enclosing.dynoUpdateHandler, TableUpdateHandler
						.GetInstance());
					playbackManager.Load(new FilePath("foo.csv"));
					playbackManager.Play();
				}

				private readonly _ActionListener_1078 _enclosing;
			}

			private readonly EcuLogger _enclosing;
		}

		private Component BuildFileNameExtention()
		{
			JLabel fileNameLabel = new JLabel("Logfile Text");
			JTextField fileNameExtention = new JTextField(string.Empty, 8);
			fileNameExtention.SetToolTipText(FILE_NAME_EXTENTION);
			fileNameExtention.AddFocusListener(new _FocusListener_1101(this, fileNameExtention
				));
			JPopupMenu fileNamePopup = new JPopupMenu();
			JMenuItem ecuIdItem = new JMenuItem("Use Current " + target + " ID");
			ecuIdItem.AddActionListener(new _ActionListener_1114(this, fileNameExtention));
			fileNamePopup.Add(ecuIdItem);
			foreach (string item in LOG_FILE_TEXT)
			{
				ecuIdItem = new JMenuItem(item);
				if (item.EndsWith("PT"))
				{
					ecuIdItem.SetToolTipText("Part Throttle");
				}
				if (item.EndsWith("WOT"))
				{
					ecuIdItem.SetToolTipText("Wide Open Throttle");
				}
				ecuIdItem.AddActionListener(new _ActionListener_1126(this, fileNameExtention, item
					));
				fileNamePopup.Add(ecuIdItem);
			}
			ecuIdItem = new JMenuItem("Clear Logfile Text");
			ecuIdItem.AddActionListener(new _ActionListener_1136(this, fileNameExtention));
			fileNamePopup.Add(ecuIdItem);
			fileNameExtention.AddMouseListener(new LogFileNameFieldAction(fileNamePopup));
			JPanel panel = new JPanel();
			panel.Add(fileNameLabel);
			panel.Add(fileNameExtention);
			return panel;
		}

		private sealed class _FocusListener_1101 : FocusListener
		{
			public _FocusListener_1101(EcuLogger _enclosing, JTextField fileNameExtention)
			{
				this._enclosing = _enclosing;
				this.fileNameExtention = fileNameExtention;
			}

			public void FocusGained(FocusEvent arg0)
			{
			}

			public void FocusLost(FocusEvent arg0)
			{
				this._enclosing.settings.SetLogfileNameText(fileNameExtention.GetText());
			}

			private readonly EcuLogger _enclosing;

			private readonly JTextField fileNameExtention;
		}

		private sealed class _ActionListener_1114 : ActionListener
		{
			public _ActionListener_1114(EcuLogger _enclosing, JTextField fileNameExtention)
			{
				this._enclosing = _enclosing;
				this.fileNameExtention = fileNameExtention;
			}

			public void ActionPerformed(ActionEvent e)
			{
				fileNameExtention.SetText(this._enclosing.ecuInit.GetEcuId());
				this._enclosing.settings.SetLogfileNameText(fileNameExtention.GetText());
			}

			private readonly EcuLogger _enclosing;

			private readonly JTextField fileNameExtention;
		}

		private sealed class _ActionListener_1126 : ActionListener
		{
			public _ActionListener_1126(EcuLogger _enclosing, JTextField fileNameExtention, string
				 item)
			{
				this._enclosing = _enclosing;
				this.fileNameExtention = fileNameExtention;
				this.item = item;
			}

			public void ActionPerformed(ActionEvent e)
			{
				fileNameExtention.SetText(item.ReplaceAll(" ", "_"));
				this._enclosing.settings.SetLogfileNameText(fileNameExtention.GetText());
			}

			private readonly EcuLogger _enclosing;

			private readonly JTextField fileNameExtention;

			private readonly string item;
		}

		private sealed class _ActionListener_1136 : ActionListener
		{
			public _ActionListener_1136(EcuLogger _enclosing, JTextField fileNameExtention)
			{
				this._enclosing = _enclosing;
				this.fileNameExtention = fileNameExtention;
			}

			public void ActionPerformed(ActionEvent e)
			{
				fileNameExtention.SetText(string.Empty);
				this._enclosing.settings.SetLogfileNameText(fileNameExtention.GetText());
			}

			private readonly EcuLogger _enclosing;

			private readonly JTextField fileNameExtention;
		}

		private Component BuildLogToFileButton()
		{
			logToFileButton = new JToggleButton(LOG_TO_FILE_START, new ImageIcon(GetType().GetResource
				(LOG_TO_FILE_ICON)));
			logToFileButton.SetToolTipText(LOG_TO_FILE_TT_TEXT);
			logToFileButton.SetBackground(Color.GREEN);
			logToFileButton.SetOpaque(true);
			logToFileButton.AddActionListener(new _ActionListener_1160(this));
			logToFileButton.GetInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).Put(KeyStroke.GetKeyStroke
				(LOG_TO_FILE_FK), "toggleFileLogging");
			logToFileButton.GetActionMap().Put("toggleFileLogging", new ToggleButtonAction(this
				, logToFileButton));
			return logToFileButton;
		}

		private sealed class _ActionListener_1160 : ActionListener
		{
			public _ActionListener_1160(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				if (this._enclosing.logToFileButton.IsSelected() && this._enclosing.controller.IsStarted
					())
				{
					this._enclosing.fileUpdateHandler.Start();
					this._enclosing.logToFileButton.SetBackground(Color.RED);
					this._enclosing.logToFileButton.SetText(RomRaider.Logger.Ecu.EcuLogger.LOG_TO_FILE_STOP
						);
				}
				else
				{
					this._enclosing.fileUpdateHandler.Stop();
					if (!this._enclosing.controller.IsStarted())
					{
						this._enclosing.statusIndicator.Stopped();
					}
					this._enclosing.logToFileButton.SetBackground(Color.GREEN);
					this._enclosing.logToFileButton.SetSelected(false);
					this._enclosing.logToFileButton.SetText(RomRaider.Logger.Ecu.EcuLogger.LOG_TO_FILE_START
						);
				}
			}

			private readonly EcuLogger _enclosing;
		}

		private JPanel BuildPortsComboBox()
		{
			portsComboBox.AddActionListener(new _ActionListener_1187(this));
			// this is a hack...
			JPanel comboBoxPanel = new JPanel(new FlowLayout());
			comboBoxPanel.Add(new JLabel("COM Port:"));
			comboBoxPanel.Add(portsComboBox);
			JCheckBox ecuCheckBox = new JCheckBox("ECU");
			JCheckBox tcuCheckBox = new JCheckBox("TCU");
			ecuCheckBox.SetToolTipText(ECU_TEXT);
			tcuCheckBox.SetToolTipText(TCU_TEXT);
			ecuCheckBox.AddActionListener(new _ActionListener_1205(this, tcuCheckBox, ecuCheckBox
				));
			tcuCheckBox.AddActionListener(new _ActionListener_1220(this, ecuCheckBox, tcuCheckBox
				));
			if (Settings.GetDestinationId() == unchecked((int)(0x10)))
			{
				ecuCheckBox.SetSelected(true);
				tcuCheckBox.SetSelected(false);
				SetTargetEcu();
			}
			else
			{
				tcuCheckBox.SetSelected(true);
				ecuCheckBox.SetSelected(false);
				SetTargetTcu();
			}
			comboBoxPanel.Add(ecuCheckBox);
			comboBoxPanel.Add(tcuCheckBox);
			JButton reconnectButton = new JButton(new ImageIcon(GetType().GetResource("/graphics/logger_restart.png"
				)));
			reconnectButton.SetPreferredSize(new Dimension(25, 25));
			reconnectButton.SetToolTipText("Reconnect to " + target);
			reconnectButton.AddActionListener(new _ActionListener_1252(this));
			comboBoxPanel.Add(reconnectButton);
			JButton disconnectButton = new JButton(new ImageIcon(GetType().GetResource("/graphics/logger_stop.png"
				)));
			disconnectButton.SetPreferredSize(new Dimension(25, 25));
			disconnectButton.SetToolTipText("Disconnect from " + target);
			disconnectButton.AddActionListener(new _ActionListener_1266(this));
			comboBoxPanel.Add(reconnectButton);
			comboBoxPanel.Add(disconnectButton);
			comboBoxPanel.Add(new JSeparator(SwingConstants.VERTICAL));
			comboBoxPanel.Add(BuildLogToFileButton());
			comboBoxPanel.Add(BuildFileNameExtention());
			return comboBoxPanel;
		}

		private sealed class _ActionListener_1187 : ActionListener
		{
			public _ActionListener_1187(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.settings.SetLoggerPort((string)this._enclosing.portsComboBox.GetSelectedItem
					());
				if (!actionEvent.ParamString().EndsWith("modifiers="))
				{
					this._enclosing.RestartLogging();
				}
			}

			private readonly EcuLogger _enclosing;
		}

		private sealed class _ActionListener_1205 : ActionListener
		{
			public _ActionListener_1205(EcuLogger _enclosing, JCheckBox tcuCheckBox, JCheckBox
				 ecuCheckBox)
			{
				this._enclosing = _enclosing;
				this.tcuCheckBox = tcuCheckBox;
				this.ecuCheckBox = ecuCheckBox;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.StopLogging();
				tcuCheckBox.SetSelected(false);
				if (ecuCheckBox.IsSelected())
				{
					this._enclosing.settings.SetLogExternalsOnly(false);
					this._enclosing.SetTargetEcu();
				}
				else
				{
					this._enclosing.settings.SetLogExternalsOnly(true);
				}
				this._enclosing.StartLogging();
			}

			private readonly EcuLogger _enclosing;

			private readonly JCheckBox tcuCheckBox;

			private readonly JCheckBox ecuCheckBox;
		}

		private sealed class _ActionListener_1220 : ActionListener
		{
			public _ActionListener_1220(EcuLogger _enclosing, JCheckBox ecuCheckBox, JCheckBox
				 tcuCheckBox)
			{
				this._enclosing = _enclosing;
				this.ecuCheckBox = ecuCheckBox;
				this.tcuCheckBox = tcuCheckBox;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.StopLogging();
				ecuCheckBox.SetSelected(false);
				if (tcuCheckBox.IsSelected())
				{
					this._enclosing.settings.SetLogExternalsOnly(false);
					this._enclosing.SetTargetTcu();
				}
				else
				{
					this._enclosing.settings.SetLogExternalsOnly(true);
				}
				this._enclosing.StartLogging();
			}

			private readonly EcuLogger _enclosing;

			private readonly JCheckBox ecuCheckBox;

			private readonly JCheckBox tcuCheckBox;
		}

		private sealed class _ActionListener_1252 : ActionListener
		{
			public _ActionListener_1252(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				try
				{
					this._enclosing.RestartLogging();
				}
				catch (System.Exception e)
				{
					this._enclosing.ReportError(e);
				}
			}

			private readonly EcuLogger _enclosing;
		}

		private sealed class _ActionListener_1266 : ActionListener
		{
			public _ActionListener_1266(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				try
				{
					this._enclosing.StopLogging();
				}
				catch (System.Exception e)
				{
					this._enclosing.ReportError(e);
				}
			}

			private readonly EcuLogger _enclosing;
		}

		private void SetTargetEcu()
		{
			Settings.SetDestinationId(ECU_ID);
			target = "ECU";
		}

		private void SetTargetTcu()
		{
			Settings.SetDestinationId(TCU_ID);
			target = "TCU";
		}

		public string GetTarget()
		{
			return target;
		}

		public string GetDefVersion()
		{
			return defVersion;
		}

		public void RestartLogging()
		{
			StopLogging();
			StartLogging();
		}

		private StatusIndicator BuildStatusIndicator()
		{
			statusIndicator = new StatusIndicator();
			controller.AddListener(statusIndicator);
			fileUpdateHandler.AddListener(statusIndicator);
			return statusIndicator;
		}

		private JComponent BuildDataTab()
		{
			JPanel panel = new JPanel(new BorderLayout());
			JButton resetButton = new JButton("Reset Data");
			resetButton.AddActionListener(new _ActionListener_1317(this));
			panel.Add(resetButton, BorderLayout.NORTH);
			JScrollPane sp = new JScrollPane(new JTable(dataTableModel), ScrollPaneConstants.
				VERTICAL_SCROLLBAR_AS_NEEDED, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
			sp.GetVerticalScrollBar().SetUnitIncrement(40);
			panel.Add(sp, BorderLayout.CENTER);
			return panel;
		}

		private sealed class _ActionListener_1317 : ActionListener
		{
			public _ActionListener_1317(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.liveDataUpdateHandler.Reset();
			}

			private readonly EcuLogger _enclosing;
		}

		private JComponent BuildGraphTab()
		{
			JPanel panel = new JPanel(new BorderLayout());
			JButton resetButton = new JButton("Reset Data");
			resetButton.AddActionListener(new _ActionListener_1333(this));
			panel.Add(resetButton, BorderLayout.NORTH);
			JScrollPane sp = new JScrollPane(graphPanel, ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED
				, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED);
			sp.GetVerticalScrollBar().SetUnitIncrement(40);
			panel.Add(sp, BorderLayout.CENTER);
			return panel;
		}

		private sealed class _ActionListener_1333 : ActionListener
		{
			public _ActionListener_1333(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.graphUpdateHandler.Reset();
			}

			private readonly EcuLogger _enclosing;
		}

		private JComponent BuildDashboardTab()
		{
			JPanel panel = new JPanel(new BorderLayout());
			JButton resetButton = new JButton("Reset Data");
			resetButton.AddActionListener(new _ActionListener_1349(this));
			panel.Add(resetButton, BorderLayout.NORTH);
			JScrollPane sp = new JScrollPane(dashboardPanel, ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED
				, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
			sp.GetVerticalScrollBar().SetUnitIncrement(40);
			panel.Add(sp, BorderLayout.CENTER);
			return panel;
		}

		private sealed class _ActionListener_1349 : ActionListener
		{
			public _ActionListener_1349(EcuLogger _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.dashboardUpdateHandler.Reset();
			}

			private readonly EcuLogger _enclosing;
		}

		private void SelectTab(int tabIndex)
		{
			int count = tabbedPane.GetComponentCount();
			if (tabIndex >= 0 && tabIndex < count)
			{
				tabbedPane.SetSelectedIndex(tabIndex);
			}
		}

		public override void WindowOpened(WindowEvent windowEvent)
		{
		}

		public override void WindowClosing(WindowEvent windowEvent)
		{
			HandleExit();
		}

		public override void WindowClosed(WindowEvent windowEvent)
		{
		}

		public override void WindowIconified(WindowEvent windowEvent)
		{
		}

		public override void WindowDeiconified(WindowEvent windowEvent)
		{
		}

		public override void WindowActivated(WindowEvent windowEvent)
		{
		}

		public override void WindowDeactivated(WindowEvent windowEvent)
		{
		}

		public override void PropertyChange(PropertyChangeEvent propertyChangeEvent)
		{
		}

		public bool IsLogging()
		{
			return controller.IsStarted();
		}

		public void StartLogging()
		{
			controller.Start();
		}

		public void StopLogging()
		{
			controller.Stop();
		}

		private void StopPlugins()
		{
			foreach (ExternalDataSource dataSource in externalDataSources)
			{
				try
				{
					dataSource.Disconnect();
				}
				catch (System.Exception e)
				{
					LOGGER.Warn("Error stopping datasource: " + dataSource.GetName(), e);
				}
			}
		}

		public bool ResetEcu()
		{
			return resetManager.ResetEcu();
		}

		public void HandleExit()
		{
			try
			{
				try
				{
					try
					{
						StopLogging();
					}
					finally
					{
						StopPlugins();
					}
				}
				finally
				{
					CleanUpUpdateHandlers();
				}
			}
			catch (System.Exception e)
			{
				LOGGER.Warn("Error stopping logger:", e);
			}
			finally
			{
				SaveSettings();
				BackupCurrentProfile();
				LOGGER.Info("Logger shutdown successful");
			}
		}

		private void SaveSettings()
		{
			settings.SetLoggerPortDefault((string)portsComboBox.GetSelectedItem());
			settings.SetLoggerWindowMaximized(GetExtendedState() == MAXIMIZED_BOTH);
			settings.SetLoggerWindowSize(GetSize());
			settings.SetLoggerWindowLocation(GetLocation());
			if (settings.GetLoggerParameterListState())
			{
				Component c = tabbedPane.GetSelectedComponent();
				if (c is JSplitPane)
				{
					// Only save the divider location if there is one
					JSplitPane sp = (JSplitPane)c.GetComponentAt(100, 100);
					settings.SetLoggerDividerLocation(sp.GetDividerLocation());
				}
			}
			settings.SetLoggerSelectedTabIndex(tabbedPane.GetSelectedIndex());
			settings.SetLoggerPluginPorts(GetPluginPorts(externalDataSources));
			try
			{
				new SettingsManagerImpl().Save(settings);
				LOGGER.Debug("Logger settings saved");
			}
			catch (System.Exception e)
			{
				LOGGER.Warn("Error saving logger settings:", e);
			}
		}

		private void BackupCurrentProfile()
		{
			try
			{
				FileHelper.SaveProfileToFile(GetCurrentProfile(), new FilePath(HOME + UserProfileLoader
					.BACKUP_PROFILE));
				LOGGER.Debug("Backup profile saved");
			}
			catch (System.Exception e)
			{
				LOGGER.Warn("Error backing up profile", e);
			}
		}

		private void CleanUpUpdateHandlers()
		{
			fileUpdateHandler.CleanUp();
			dataHandlerManager.CleanUp();
			graphHandlerManager.CleanUp();
			dashboardHandlerManager.CleanUp();
		}

		public Settings GetSettings()
		{
			return settings;
		}

		public void ReportMessage(string message)
		{
			if (message != null)
			{
				SwingUtilities.InvokeLater(new _Runnable_1493(this, message));
			}
		}

		private sealed class _Runnable_1493 : Runnable
		{
			public _Runnable_1493(EcuLogger _enclosing, string message)
			{
				this._enclosing = _enclosing;
				this.message = message;
			}

			public void Run()
			{
				this._enclosing.messageLabel.SetText(message);
				this._enclosing.messageLabel.SetForeground(Color.BLACK);
			}

			private readonly EcuLogger _enclosing;

			private readonly string message;
		}

		public void ReportMessageInTitleBar(string message)
		{
			if (!ParamChecker.IsNullOrEmpty(message))
			{
				SetTitle(message);
			}
		}

		public void ReportStats(string message)
		{
			if (!ParamChecker.IsNullOrEmpty(message))
			{
				SwingUtilities.InvokeLater(new _Runnable_1511(this, message));
			}
		}

		private sealed class _Runnable_1511 : Runnable
		{
			public _Runnable_1511(EcuLogger _enclosing, string message)
			{
				this._enclosing = _enclosing;
				this.message = message;
			}

			public void Run()
			{
				this._enclosing.statsLabel.SetText(message);
			}

			private readonly EcuLogger _enclosing;

			private readonly string message;
		}

		private JLabel BuildStatsLabel()
		{
			JLabel label = new JLabel(" ");
			label.SetForeground(Color.BLACK);
			label.SetHorizontalTextPosition(SwingConstants.RIGHT);
			return label;
		}

		public void ReportError(string error)
		{
			if (!ParamChecker.IsNullOrEmpty(error))
			{
				SwingUtilities.InvokeLater(new _Runnable_1530(this, error));
			}
		}

		private sealed class _Runnable_1530 : Runnable
		{
			public _Runnable_1530(EcuLogger _enclosing, string error)
			{
				this._enclosing = _enclosing;
				this.error = error;
			}

			public void Run()
			{
				this._enclosing.messageLabel.SetText("Error: " + error);
				this._enclosing.messageLabel.SetForeground(Color.RED);
			}

			private readonly EcuLogger _enclosing;

			private readonly string error;
		}

		public void ReportError(System.Exception e)
		{
			if (e != null)
			{
				LOGGER.Error("Error occurred", e);
				string error = e.Message;
				if (!ParamChecker.IsNullOrEmpty(error))
				{
					ReportError(error);
				}
				else
				{
					ReportError(e.ToString());
				}
			}
		}

		public void ReportError(string error, System.Exception e)
		{
			if (e != null)
			{
				LOGGER.Error(error, e);
			}
			ReportError(error);
		}

		public override void SetTitle(string title)
		{
			if (title != null)
			{
				if (!title.StartsWith(ECU_LOGGER_TITLE))
				{
					title = ECU_LOGGER_TITLE + (title.Length == 0 ? string.Empty : " - " + title);
				}
				base.SetTitle(title);
			}
		}

		public void SetRefreshMode(bool refreshMode)
		{
			settings.SetRefreshMode(refreshMode);
			refresher.SetRefreshMode(refreshMode);
		}

		private JProgressBar Startbar()
		{
			startStatus = new JWindow();
			startStatus.SetAlwaysOnTop(true);
			startStatus.SetLocation((int)(settings.GetLoggerWindowSize().GetWidth() / 2 + settings
				.GetLoggerWindowLocation().GetX() - 150), (int)(settings.GetLoggerWindowSize().GetHeight
				() / 2 + settings.GetLoggerWindowLocation().GetY() - 36));
			JProgressBar progressBar = new JProgressBar(0, 100);
			progressBar.SetValue(0);
			progressBar.SetIndeterminate(false);
			progressBar.SetOpaque(true);
			startText.SetOpaque(true);
			JPanel panel = new JPanel();
			panel.SetLayout(new BorderLayout());
			panel.SetBorder(BorderFactory.CreateEtchedBorder());
			panel.Add(progressBar, BorderLayout.CENTER);
			panel.Add(startText, BorderLayout.SOUTH);
			startStatus.GetContentPane().Add(panel);
			startStatus.Pack();
			startStatus.SetVisible(true);
			return progressBar;
		}

		//**********************************************************************
		public static void StartLogger(int defaultCloseOperation, ECUEditor ecuEditor)
		{
			RomRaider.Logger.Ecu.EcuLogger ecuLogger = new RomRaider.Logger.Ecu.EcuLogger(ecuEditor
				);
			CreateAndShowGui(defaultCloseOperation, ecuLogger, false);
		}

		public static void StartLogger(int defaultCloseOperation, Settings settings, params 
			string[] args)
		{
			RomRaider.Logger.Ecu.EcuLogger ecuLogger = new RomRaider.Logger.Ecu.EcuLogger(settings
				);
			bool fullscreen = ContainsFullScreenArg(args);
			CreateAndShowGui(defaultCloseOperation, ecuLogger, fullscreen);
		}

		private static bool ContainsFullScreenArg(params string[] args)
		{
			foreach (string arg in args)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(LOGGER_FULLSCREEN_ARG, arg))
				{
					return true;
				}
			}
			return false;
		}

		private static void CreateAndShowGui(int defaultCloseOperation, RomRaider.Logger.Ecu.EcuLogger
			 ecuLogger, bool fullscreen)
		{
			SwingUtilities.InvokeLater(new _Runnable_1615(defaultCloseOperation, ecuLogger, fullscreen
				));
		}

		private sealed class _Runnable_1615 : Runnable
		{
			public _Runnable_1615(int defaultCloseOperation, RomRaider.Logger.Ecu.EcuLogger ecuLogger
				, bool fullscreen)
			{
				this.defaultCloseOperation = defaultCloseOperation;
				this.ecuLogger = ecuLogger;
				this.fullscreen = fullscreen;
			}

			public void Run()
			{
				RomRaider.Logger.Ecu.EcuLogger.DoCreateAndShowGui(defaultCloseOperation, ecuLogger
					, fullscreen);
			}

			private readonly int defaultCloseOperation;

			private readonly RomRaider.Logger.Ecu.EcuLogger ecuLogger;

			private readonly bool fullscreen;
		}

		private static void DoCreateAndShowGui(int defaultCloseOperation, RomRaider.Logger.Ecu.EcuLogger
			 ecuLogger, bool fullscreen)
		{
			Settings settings = ecuLogger.GetSettings();
			// set window properties
			ecuLogger.Pack();
			ecuLogger.SelectTab(settings.GetLoggerSelectedTabIndex());
			ecuLogger.SetRefreshMode(settings.GetRefreshMode());
			if (fullscreen)
			{
				// display full screen
				GraphicsEnvironment env = GraphicsEnvironment.GetLocalGraphicsEnvironment();
				GraphicsDevice device = env.GetDefaultScreenDevice();
				JFrame frame = new JFrame(ecuLogger.GetTitle());
				frame.SetIconImage(new ImageIcon(ICON_PATH).GetImage());
				frame.SetContentPane(ecuLogger.GetContentPane());
				frame.AddWindowListener(ecuLogger);
				frame.SetDefaultCloseOperation(defaultCloseOperation);
				frame.SetUndecorated(true);
				frame.SetResizable(false);
				device.SetFullScreenWindow(frame);
			}
			else
			{
				// display in window
				ecuLogger.AddWindowListener(ecuLogger);
				ecuLogger.SetIconImage(new ImageIcon(ICON_PATH).GetImage());
				ecuLogger.SetSize(settings.GetLoggerWindowSize());
				ecuLogger.SetLocation(settings.GetLoggerWindowLocation());
				if (settings.IsLoggerWindowMaximized())
				{
					ecuLogger.SetExtendedState(MAXIMIZED_BOTH);
				}
				ecuLogger.SetDefaultCloseOperation(defaultCloseOperation);
				ecuLogger.SetVisible(true);
			}
		}
	}
}
