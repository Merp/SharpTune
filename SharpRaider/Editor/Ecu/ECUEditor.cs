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
using Com.Sun.Org.Apache.Xerces.Internal.Parsers;
using Java.Awt;
using Java.Awt.Event;
using Java.Beans;
using Javax.Swing;
using Javax.Swing.Tree;
using Org.W3c.Dom;
using Org.Xml.Sax;
using RomRaider;
using RomRaider.Definition;
using RomRaider.Editor.Ecu;
using RomRaider.Logger.Ecu;
using RomRaider.Logger.Ecu.UI.Handler.Table;
using RomRaider.Maps;
using RomRaider.Net;
using RomRaider.Swing;
using RomRaider.Util;
using RomRaider.Xml;
using Sharpen;

namespace RomRaider.Editor.Ecu
{
	[System.Serializable]
	public class ECUEditor : AbstractFrame
	{
		private const long serialVersionUID = -7826850987392016292L;

		private readonly string titleText = Version.PRODUCT_NAME + " v" + Version.VERSION
			 + " | ECU Editor";

		private static readonly string NEW_LINE = Runtime.GetProperty("line.separator");

		private readonly SettingsManager settingsManager = new SettingsManagerImpl();

		private readonly RomTreeRootNode imageRoot = new RomTreeRootNode("Open Images");

		private readonly RomTree imageList;

		public MDIDesktopPane rightPanel;

		public JProgressPane statusPanel = new JProgressPane();

		private JSplitPane splitPane = new JSplitPane();

		private Rom lastSelectedRom = null;

		private ECUEditorToolBar toolBar;

		private readonly ECUEditorMenuBar menuBar;

		private Settings settings = ECUExec.settings;

		private readonly TableToolBar tableToolBar;

		private readonly JPanel toolBarPanel = new JPanel();

		private OpenImageWorker openImageWorker;

		private CloseImageWorker closeImageWorker;

		private SetUserLevelWorker setUserLevelWorker;

		private LaunchLoggerWorker launchLoggerWorker;

		private readonly ImageIcon editorIcon = new ImageIcon(GetType().GetResource("/graphics/romraider-ico.gif"
			), "RomRaider ECU Editor");

		private DefinitionManager definitionManager = new DefinitionManager();

		private DefinitionRepoManager definitionRepoManager = ECUExec.definitionRepoManager;

		public ECUEditor()
		{
			imageList = new RomTree(imageRoot);
			rightPanel = new MDIDesktopPane(this);
			if (!Sharpen.Runtime.EqualsIgnoreCase(settings.GetRecentVersion(), Version.VERSION
				))
			{
				ShowReleaseNotes();
			}
			SetSize(GetSettings().GetWindowSize());
			SetLocation(GetSettings().GetWindowLocation());
			if (GetSettings().IsWindowMaximized())
			{
				SetExtendedState(MAXIMIZED_BOTH);
			}
			JScrollPane rightScrollPane = new JScrollPane(rightPanel, ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED
				, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED);
			JScrollPane leftScrollPane = new JScrollPane(imageList, ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED
				, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED);
			splitPane = new JSplitPane(JSplitPane.HORIZONTAL_SPLIT, leftScrollPane, rightScrollPane
				);
			splitPane.SetDividerSize(3);
			splitPane.SetDividerLocation(GetSettings().GetSplitPaneLocation());
			splitPane.AddPropertyChangeListener(this);
			splitPane.SetContinuousLayout(true);
			GetContentPane().Add(splitPane);
			rightPanel.SetBackground(Color.BLACK);
			imageList.SetScrollsOnExpand(true);
			imageList.SetContainer(this);
			//create menubar
			menuBar = new ECUEditorMenuBar(this);
			this.SetJMenuBar(menuBar);
			this.Add(statusPanel, BorderLayout.SOUTH);
			// create toolbars
			toolBar = new ECUEditorToolBar(this, "Editor Tools");
			tableToolBar = new TableToolBar("Table Tools", this);
			tableToolBar.UpdateTableToolBar(null);
			CustomToolbarLayout toolBarLayout = new CustomToolbarLayout(FlowLayout.LEFT, 0, 0
				);
			toolBarPanel.SetLayout(toolBarLayout);
			toolBarPanel.Add(toolBar);
			toolBarPanel.Add(tableToolBar);
			toolBarPanel.SetVisible(true);
			this.Add(toolBarPanel, BorderLayout.NORTH);
			//set remaining window properties
			SetIconImage(editorIcon.GetImage());
			SetDefaultCloseOperation(EXIT_ON_CLOSE);
			AddWindowListener(this);
			SetTitle(titleText);
			SetVisible(true);
		}

		private void ShowReleaseNotes()
		{
			try
			{
				BufferedReader br = new BufferedReader(new FileReader(settings.GetReleaseNotes())
					);
				try
				{
					// new version being used, display release notes
					JTextArea releaseNotes = new JTextArea();
					releaseNotes.SetEditable(false);
					releaseNotes.SetWrapStyleWord(true);
					releaseNotes.SetLineWrap(true);
					releaseNotes.SetFont(new Font("Tahoma", Font.PLAIN, 12));
					StringBuilder sb = new StringBuilder();
					while (br.Ready())
					{
						sb.Append(br.ReadLine()).Append(NEW_LINE);
					}
					releaseNotes.SetText(sb.ToString());
					releaseNotes.SetCaretPosition(0);
					JScrollPane scroller = new JScrollPane(releaseNotes, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS
						, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
					scroller.SetPreferredSize(new Dimension(600, 500));
					JOptionPane.ShowMessageDialog(this, scroller, Version.PRODUCT_NAME + Version.VERSION
						 + " Release Notes", JOptionPane.INFORMATION_MESSAGE);
				}
				finally
				{
					br.Close();
				}
			}
			catch (Exception)
			{
			}
		}

		public virtual void HandleExit()
		{
			settings.SetSplitPaneLocation(splitPane.GetDividerLocation());
			settings.SetWindowMaximized(GetExtendedState() == MAXIMIZED_BOTH);
			settings.SetWindowSize(GetSize());
			settings.SetWindowLocation(GetLocation());
			// Save when exit to save file settings.
			settingsManager.Save(settings, statusPanel);
			statusPanel.Update("Ready...", 0);
			Repaint();
		}

		public override void WindowClosing(WindowEvent e)
		{
			HandleExit();
		}

		public override void WindowOpened(WindowEvent e)
		{
		}

		public override void WindowClosed(WindowEvent e)
		{
		}

		public override void WindowIconified(WindowEvent e)
		{
		}

		public override void WindowDeiconified(WindowEvent e)
		{
		}

		public override void WindowActivated(WindowEvent e)
		{
		}

		public override void WindowDeactivated(WindowEvent e)
		{
		}

		public virtual string GetVersion()
		{
			return Version.VERSION;
		}

		public virtual Settings GetSettings()
		{
			return settings;
		}

		public virtual void AddRom(Rom input)
		{
			// add to ecu image list pane
			RomTreeNode romNode = new RomTreeNode(input, GetSettings().GetUserLevel(), GetSettings
				().IsDisplayHighTables(), this);
			GetImageRoot().Add(romNode);
			GetImageList().SetVisible(true);
			GetImageList().ExpandPath(new TreePath(GetImageRoot()));
			GetImageList().ExpandPath(new TreePath(romNode.GetPath()));
			// uncomment collapsePath if you want ROM to open collapsed.
			// imageList.collapsePath(addedRomPath);
			GetImageList().SetRootVisible(false);
			GetImageList().Repaint();
			// Only set if no other rom has been selected.
			if (null == GetLastSelectedRom())
			{
				SetLastSelectedRom(input);
			}
			if (input.GetRomID().IsObsolete() && GetSettings().IsObsoleteWarning())
			{
				JPanel infoPanel = new JPanel();
				infoPanel.SetLayout(new GridLayout(3, 1));
				infoPanel.Add(new JLabel("A newer version of this ECU revision exists. " + "Please visit the following link to download the latest revision:"
					));
				infoPanel.Add(new URL(GetSettings().GetRomRevisionURL()));
				JCheckBox check = new JCheckBox("Always display this message", true);
				check.SetHorizontalAlignment(JCheckBox.RIGHT);
				check.AddActionListener(new _ActionListener_297(this));
				infoPanel.Add(check);
				JOptionPane.ShowMessageDialog(this, infoPanel, "ECU Revision is Obsolete", JOptionPane
					.INFORMATION_MESSAGE);
			}
			input.ApplyTableColorSettings();
		}

		private sealed class _ActionListener_297 : ActionListener
		{
			public _ActionListener_297(ECUEditor _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent e)
			{
				this._enclosing.settings.SetObsoleteWarning(((JCheckBox)e.GetSource()).IsSelected
					());
			}

			private readonly ECUEditor _enclosing;
		}

		public virtual void DisplayTable(TableFrame frame)
		{
			frame.SetVisible(true);
			try
			{
				rightPanel.Add(frame);
			}
			catch (ArgumentException)
			{
				// table is already open, so set focus
				frame.RequestFocus();
			}
			//frame.setSize(frame.getTable().getFrameSize());
			frame.Pack();
			rightPanel.Repaint();
		}

		public virtual void RemoveDisplayTable(TableFrame frame)
		{
			frame.SetVisible(false);
			UpdateTableToolBar(null);
			rightPanel.Remove(frame);
			rightPanel.Repaint();
		}

		public virtual void CloseImage()
		{
			SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			closeImageWorker = new CloseImageWorker(this);
			closeImageWorker.AddPropertyChangeListener(this);
			closeImageWorker.Execute();
		}

		public virtual void CloseAllImages()
		{
			while (imageRoot.GetChildCount() > 0)
			{
				CloseImage();
			}
		}

		public virtual Rom GetLastSelectedRom()
		{
			return lastSelectedRom;
		}

		public virtual void SetLastSelectedRom(Rom lastSelectedRom)
		{
			this.lastSelectedRom = lastSelectedRom;
			if (lastSelectedRom == null)
			{
				SetTitle(titleText);
			}
			else
			{
				SetTitle(titleText + " - " + lastSelectedRom.GetFileName());
			}
			// update filenames
			for (int i = 0; i < imageRoot.GetChildCount(); i++)
			{
				((RomTreeNode)imageRoot.GetChildAt(i)).UpdateFileName();
			}
		}

		public virtual ECUEditorToolBar GetToolBar()
		{
			return toolBar;
		}

		public virtual void SetToolBar(ECUEditorToolBar toolBar)
		{
			this.toolBar = toolBar;
		}

		public virtual ECUEditorMenuBar GetEditorMenuBar()
		{
			return menuBar;
		}

		public virtual TableToolBar GetTableToolBar()
		{
			return tableToolBar;
		}

		public virtual void UpdateTableToolBar(Table currentTable)
		{
			tableToolBar.UpdateTableToolBar(currentTable);
		}

		public virtual void SetSettings(Settings settings)
		{
			this.settings = settings;
			for (int i = 0; i < imageRoot.GetChildCount(); i++)
			{
				RomTreeNode rtn = (RomTreeNode)imageRoot.GetChildAt(i);
				rtn.GetRom().ApplyTableColorSettings();
			}
		}

		public virtual void SetUserLevel(int userLevel)
		{
			SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			setUserLevelWorker = new SetUserLevelWorker(this, userLevel);
			setUserLevelWorker.AddPropertyChangeListener(this);
			setUserLevelWorker.Execute();
		}

		public virtual Vector<Rom> GetImages()
		{
			Vector<Rom> images = new Vector<Rom>();
			for (int i = 0; i < imageRoot.GetChildCount(); i++)
			{
				RomTreeNode rtn = (RomTreeNode)imageRoot.GetChildAt(i);
				images.AddItem(rtn.GetRom());
			}
			return images;
		}

		public override void PropertyChange(PropertyChangeEvent evt)
		{
			imageList.UpdateUI();
			imageList.Repaint();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void OpenImage(FilePath inputFile)
		{
			SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			openImageWorker = new OpenImageWorker(this, inputFile);
			openImageWorker.AddPropertyChangeListener(statusPanel);
			openImageWorker.Execute();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void OpenImages(FilePath[] inputFiles)
		{
			if (inputFiles.Length < 1)
			{
				JOptionPane.ShowMessageDialog(this, "Image Not Found", "Error Loading Image(s)", 
					JOptionPane.ERROR_MESSAGE);
				return;
			}
			for (int j = 0; j < inputFiles.Length; j++)
			{
				OpenImage(inputFiles[j]);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual byte[] ReadFile(FilePath inputFile)
		{
			ByteArrayOutputStream baos = new ByteArrayOutputStream();
			FileInputStream fis = new FileInputStream(inputFile);
			try
			{
				byte[] buf = new byte[8192];
				int bytesRead;
				while ((bytesRead = fis.Read(buf)) != -1)
				{
					baos.Write(buf, 0, bytesRead);
				}
			}
			finally
			{
				fis.Close();
			}
			return baos.ToByteArray();
		}

		public virtual void LaunchLogger()
		{
			SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			launchLoggerWorker = new LaunchLoggerWorker(this);
			launchLoggerWorker.AddPropertyChangeListener(this);
			launchLoggerWorker.Execute();
		}

		public virtual SettingsManager GetSettingsManager()
		{
			return this.settingsManager;
		}

		public virtual RomTreeRootNode GetImageRoot()
		{
			return imageRoot;
		}

		public virtual RomTree GetImageList()
		{
			return imageList;
		}

		public virtual JProgressPane GetStatusPanel()
		{
			return this.statusPanel;
		}

		public virtual MDIDesktopPane GetRightPanel()
		{
			return this.rightPanel;
		}

		public virtual DefinitionRepoManager GetDefinitionRepoManager()
		{
			return definitionRepoManager;
		}

		public virtual ImageIcon GetEditorImageIcon()
		{
			return this.editorIcon;
		}

		public virtual void RefreshTableMenus()
		{
			Vector<Rom> roms = GetImages();
			foreach (Rom rom in roms)
			{
				Vector<Table> tables = rom.GetTables();
				foreach (Table table in tables)
				{
					table.GetFrame().GetTableMenuBar().RefreshTableMenuBar();
				}
			}
		}
	}

	internal class LaunchLoggerWorker : SwingWorker<Void, Void>
	{
		private readonly ECUEditor editor;

		public LaunchLoggerWorker(ECUEditor editor)
		{
			this.editor = editor;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			editor.GetStatusPanel().SetStatus("Launching Logger...");
			SetProgress(10);
			EcuLogger.StartLogger(WindowConstants.DISPOSE_ON_CLOSE, editor);
			return null;
		}

		protected override void Done()
		{
			editor.GetStatusPanel().SetStatus("Ready...");
			SetProgress(0);
			editor.GetToolBar().UpdateButtons();
			editor.GetEditorMenuBar().UpdateMenu();
			editor.SetCursor(null);
		}
	}

	internal class SetUserLevelWorker : SwingWorker<Void, Void>
	{
		private readonly ECUEditor editor;

		internal int userLevel;

		public SetUserLevelWorker(ECUEditor editor, int userLevel)
		{
			this.editor = editor;
			this.userLevel = userLevel;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			Settings settings = editor.GetSettings();
			RomTreeRootNode imageRoot = editor.GetImageRoot();
			settings.SetUserLevel(userLevel);
			imageRoot.SetUserLevel(userLevel, settings.IsDisplayHighTables());
			return null;
		}

		protected override void Done()
		{
			editor.GetStatusPanel().SetStatus("Ready...");
			SetProgress(0);
			editor.GetToolBar().UpdateButtons();
			editor.GetEditorMenuBar().UpdateMenu();
			editor.SetCursor(null);
		}
	}

	internal class CloseImageWorker : SwingWorker<Void, Void>
	{
		private readonly ECUEditor editor;

		public CloseImageWorker(ECUEditor editor)
		{
			this.editor = editor;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			RomTreeRootNode imageRoot = editor.GetImageRoot();
			RomTree imageList = editor.GetImageList();
			for (int i = 0; i < imageRoot.GetChildCount(); i++)
			{
				RomTreeNode romTreeNode = (RomTreeNode)imageRoot.GetChildAt(i);
				Rom rom = romTreeNode.GetRom();
				if (rom == editor.GetLastSelectedRom())
				{
					Vector<Table> romTables = rom.GetTables();
					foreach (Table t in romTables)
					{
						editor.GetRightPanel().Remove(t.GetFrame());
						TableUpdateHandler.GetInstance().DeregisterTable(t);
					}
					Vector<TreePath> path = new Vector<TreePath>();
					path.AddItem(new TreePath(romTreeNode.GetPath()));
					imageRoot.Remove(i);
					imageList.RemoveDescendantToggledPaths(path.GetEnumerator());
					break;
				}
			}
			if (imageRoot.GetChildCount() > 0)
			{
				editor.SetLastSelectedRom(((RomTreeNode)imageRoot.GetChildAt(0)).GetRom());
			}
			else
			{
				// no other images open
				editor.SetLastSelectedRom(null);
			}
			editor.GetRightPanel().Repaint();
			return null;
		}

		protected override void Done()
		{
			editor.GetStatusPanel().SetStatus("Ready...");
			SetProgress(0);
			editor.GetToolBar().UpdateButtons();
			editor.GetEditorMenuBar().UpdateMenu();
			editor.RefreshTableMenus();
			editor.SetCursor(null);
		}
	}

	internal class OpenImageWorker : SwingWorker<Void, Void>
	{
		private readonly ECUEditor editor;

		private readonly FilePath inputFile;

		public OpenImageWorker(ECUEditor editor, FilePath inputFile)
		{
			this.editor = editor;
			this.inputFile = inputFile;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			try
			{
				Settings settings = editor.GetSettings();
				editor.GetStatusPanel().SetStatus("Parsing ECU definitions...");
				SetProgress(0);
				byte[] input = editor.ReadFile(inputFile);
				DOMRomUnmarshaller domUms = new DOMRomUnmarshaller(settings, editor);
				DOMParser parser = new DOMParser();
				editor.GetStatusPanel().SetStatus("Finding ECU definition...");
				SetProgress(10);
				Rom rom;
				// parse ecu definition files until result found
				for (int i = 0; i < settings.GetEcuDefinitionFiles().Count; i++)
				{
					InputSource src = new InputSource(new FileInputStream(settings.GetEcuDefinitionFiles
						()[i]));
					parser.Parse(src);
					Document doc = parser.GetDocument();
					try
					{
						rom = domUms.UnmarshallXMLDefinition(doc.GetDocumentElement(), input, editor.GetStatusPanel
							());
						editor.GetStatusPanel().SetStatus("Populating tables...");
						SetProgress(50);
						rom.PopulateTables(input, editor.GetStatusPanel());
						rom.SetFileName(inputFile.GetName());
						editor.GetStatusPanel().SetStatus("Finalizing...");
						SetProgress(75);
						editor.AddRom(rom);
						rom.SetFullFileName(inputFile);
						editor.GetStatusPanel().SetStatus("Done loading image...");
						SetProgress(100);
						return null;
					}
					catch (RomNotFoundException)
					{
					}
				}
				// rom was not found in current file, skip to next
				// if code executes to this point, no ROM was found, report to user
				JOptionPane.ShowMessageDialog(editor, "ECU Definition Not Found", "Error Loading "
					 + inputFile.GetName(), JOptionPane.ERROR_MESSAGE);
			}
			catch (SAXParseException)
			{
				// catch general parsing exception - enough people don't unzip the defs that a better error message is in order
				JOptionPane.ShowMessageDialog(editor, "Unable to read XML definitions.  Please make sure the definition file is correct.  If it is in a ZIP archive, unzip the file and try again."
					, "Error Loading " + inputFile.GetName(), JOptionPane.ERROR_MESSAGE);
			}
			catch (StackOverflowError)
			{
				// handles looped inheritance, which will use up all available memory
				JOptionPane.ShowMessageDialog(editor, "Looped \"base\" attribute in XML definitions."
					, "Error Loading " + inputFile.GetName(), JOptionPane.ERROR_MESSAGE);
			}
			catch (OutOfMemoryException)
			{
				// handles Java heap space issues when loading multiple Roms.
				JOptionPane.ShowMessageDialog(editor, "Error loading Image. Out of memeory.", "Error Loading "
					 + inputFile.GetName(), JOptionPane.ERROR_MESSAGE);
			}
			return null;
		}

		protected override void Done()
		{
			editor.GetStatusPanel().SetStatus("Ready...");
			SetProgress(0);
			editor.GetToolBar().UpdateButtons();
			editor.GetEditorMenuBar().UpdateMenu();
			editor.RefreshTableMenus();
			editor.SetCursor(null);
		}
	}
}
