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
using Com.Centerkey.Utils;
using Java.Awt.Event;
using Javax.Swing;
using RomRaider;
using RomRaider.Editor.Ecu;
using RomRaider.Maps;
using RomRaider.Ramtune.Test;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class ECUEditorMenuBar : JMenuBar, ActionListener
	{
		private const long serialVersionUID = -4777040428837855236L;

		private readonly JMenu fileMenu = new JMenu("File");

		private readonly JMenuItem openImage = new JMenuItem("Open Image...");

		private readonly JMenuItem openImages = new JMenuItem("Open Image(s)...");

		private readonly JMenuItem saveImage = new JMenuItem("Save Image As...");

		private readonly JMenuItem saveAsRepository = new JMenuItem("Save Image As Repository..."
			);

		private readonly JMenuItem refreshImage = new JMenuItem("Refresh Image");

		private readonly JMenuItem closeImage = new JMenuItem("Close Image");

		private readonly JMenuItem closeAll = new JMenuItem("Close All Images");

		private readonly JMenuItem exit = new JMenuItem("Exit");

		private readonly JMenu definitionMenu = new JMenu("ECU Definitions");

		private readonly JMenuItem defManager = new JMenuItem("ECU Definition Manager..."
			);

		private readonly JMenuItem updateDefinition = new JMenuItem("Get ECU Definitions..."
			);

		private readonly JMenu editMenu = new JMenu("Edit");

		private readonly JMenuItem settingsMenuItem = new JMenuItem(Version.PRODUCT_NAME 
			+ " Settings...");

		private readonly JMenuItem compareImages = new JMenuItem("Compare Images...");

		private readonly JMenu viewMenu = new JMenu("View");

		private readonly JMenuItem romProperties = new JMenuItem("ECU Image Properties");

		private readonly ButtonGroup levelGroup = new ButtonGroup();

		private readonly JMenu levelMenu = new JMenu("User Level");

		private readonly JRadioButtonMenuItem level1 = new JRadioButtonMenuItem("1 Beginner"
			);

		private readonly JRadioButtonMenuItem level2 = new JRadioButtonMenuItem("2 Intermediate"
			);

		private readonly JRadioButtonMenuItem level3 = new JRadioButtonMenuItem("3 Advanced"
			);

		private readonly JRadioButtonMenuItem level4 = new JRadioButtonMenuItem("4 Highest"
			);

		private readonly JRadioButtonMenuItem level5 = new JRadioButtonMenuItem("5 Debug Mode"
			);

		private readonly JMenu loggerMenu = new JMenu("Logger");

		private readonly JMenuItem openLogger = new JMenuItem("Launch Logger...");

		private readonly JMenu ramTuneMenu = new JMenu("SSM");

		private readonly JMenuItem launchRamTuneTestApp = new JMenuItem("Launch Test App..."
			);

		private readonly JMenu helpMenu = new JMenu("Help");

		private readonly JMenuItem about = new JMenuItem("About " + Version.PRODUCT_NAME);

		private readonly ECUEditor parent;

		public ECUEditorMenuBar(ECUEditor parent)
		{
			//    private JMenuItem editDefinition = new JMenuItem("Edit ECU Definitions...");
			this.parent = parent;
			// file menu items
			Add(fileMenu);
			fileMenu.SetMnemonic('F');
			openImage.SetMnemonic('O');
			openImage.SetMnemonic('I');
			saveImage.SetMnemonic('S');
			saveAsRepository.SetMnemonic('D');
			refreshImage.SetMnemonic('R');
			closeImage.SetMnemonic('C');
			//closeAll.setMnemonic('A');
			exit.SetMnemonic('X');
			fileMenu.Add(openImage);
			//fileMenu.add(openImages);
			fileMenu.Add(saveImage);
			fileMenu.Add(saveAsRepository);
			fileMenu.Add(refreshImage);
			fileMenu.Add(new JSeparator());
			fileMenu.Add(closeImage);
			//fileMenu.add(closeAll);
			fileMenu.Add(new JSeparator());
			fileMenu.Add(exit);
			openImage.AddActionListener(this);
			//openImages.addActionListener(this);
			saveImage.AddActionListener(this);
			saveAsRepository.AddActionListener(this);
			refreshImage.AddActionListener(this);
			closeImage.AddActionListener(this);
			//closeAll.addActionListener(this);
			exit.AddActionListener(this);
			// edit menu items
			Add(editMenu);
			editMenu.SetMnemonic('E');
			editMenu.Add(settingsMenuItem);
			settingsMenuItem.AddActionListener(this);
			editMenu.Add(compareImages);
			compareImages.AddActionListener(this);
			// ecu def menu items
			Add(definitionMenu);
			definitionMenu.SetMnemonic('D');
			defManager.SetMnemonic('D');
			//        editDefinition.setMnemonic('E');
			updateDefinition.SetMnemonic('U');
			settingsMenuItem.SetMnemonic('S');
			compareImages.SetMnemonic('C');
			definitionMenu.Add(defManager);
			//        definitionMenu.add(editDefinition);
			definitionMenu.Add(updateDefinition);
			defManager.AddActionListener(this);
			//        editDefinition.addActionListener(this);
			updateDefinition.AddActionListener(this);
			// view menu items
			Add(viewMenu);
			viewMenu.SetMnemonic('V');
			romProperties.SetMnemonic('P');
			levelMenu.SetMnemonic('U');
			level1.SetMnemonic('1');
			level2.SetMnemonic('2');
			level3.SetMnemonic('3');
			level4.SetMnemonic('4');
			level5.SetMnemonic('5');
			viewMenu.Add(romProperties);
			viewMenu.Add(levelMenu);
			levelMenu.Add(level1);
			levelMenu.Add(level2);
			levelMenu.Add(level3);
			levelMenu.Add(level4);
			levelMenu.Add(level5);
			romProperties.AddActionListener(this);
			level1.AddActionListener(this);
			level2.AddActionListener(this);
			level3.AddActionListener(this);
			level4.AddActionListener(this);
			level5.AddActionListener(this);
			levelGroup.Add(level1);
			levelGroup.Add(level2);
			levelGroup.Add(level3);
			levelGroup.Add(level4);
			levelGroup.Add(level5);
			// select correct userlevel button
			if (parent.GetSettings().GetUserLevel() == 1)
			{
				level1.SetSelected(true);
			}
			else
			{
				if (parent.GetSettings().GetUserLevel() == 2)
				{
					level2.SetSelected(true);
				}
				else
				{
					if (parent.GetSettings().GetUserLevel() == 3)
					{
						level3.SetSelected(true);
					}
					else
					{
						if (parent.GetSettings().GetUserLevel() == 4)
						{
							level4.SetSelected(true);
						}
						else
						{
							if (parent.GetSettings().GetUserLevel() == 5)
							{
								level5.SetSelected(true);
							}
						}
					}
				}
			}
			// logger menu stuff
			Add(loggerMenu);
			loggerMenu.SetMnemonic('L');
			openLogger.SetMnemonic('O');
			loggerMenu.Add(openLogger);
			openLogger.AddActionListener(this);
			// ramtune menu stuff
			Add(ramTuneMenu);
			ramTuneMenu.SetMnemonic('R');
			launchRamTuneTestApp.SetMnemonic('L');
			ramTuneMenu.Add(launchRamTuneTestApp);
			launchRamTuneTestApp.AddActionListener(this);
			// help menu stuff
			Add(helpMenu);
			helpMenu.SetMnemonic('H');
			about.SetMnemonic('A');
			helpMenu.Add(about);
			about.AddActionListener(this);
			// disable unused buttons! 0.3.1
			//        editDefinition.setEnabled(false);
			UpdateMenu();
		}

		public virtual void UpdateMenu()
		{
			string file = GetLastSelectedRomFileName();
			if (string.Empty.Equals(file))
			{
				saveImage.SetEnabled(false);
				saveAsRepository.SetEnabled(false);
				closeImage.SetEnabled(false);
				//closeAll.setEnabled(false);
				romProperties.SetEnabled(false);
				saveImage.SetText("Save As...");
				saveAsRepository.SetText("Save As Repository...");
				compareImages.SetEnabled(false);
			}
			else
			{
				saveImage.SetEnabled(true);
				saveAsRepository.SetEnabled(true);
				closeImage.SetEnabled(true);
				//closeAll.setEnabled(true);
				romProperties.SetEnabled(true);
				saveImage.SetText("Save " + file + " As...");
				saveAsRepository.SetText("Save " + file + " As Repository...");
				compareImages.SetEnabled(true);
			}
			refreshImage.SetText("Refresh " + file);
			closeImage.SetText("Close " + file);
			romProperties.SetText(file + "Properties");
		}

		public virtual void ActionPerformed(ActionEvent e)
		{
			if (e.GetSource() == openImage)
			{
				try
				{
					OpenImageDialog();
				}
				catch (Exception ex)
				{
					JOptionPane.ShowMessageDialog(parent, new DebugPanel(ex, parent.GetSettings().GetSupportURL
						()), "Exception", JOptionPane.ERROR_MESSAGE);
				}
			}
			else
			{
				if (e.GetSource() == openImages)
				{
					try
					{
						OpenImagesDialog();
					}
					catch (Exception ex)
					{
						JOptionPane.ShowMessageDialog(parent, new DebugPanel(ex, parent.GetSettings().GetSupportURL
							()), "Exception", JOptionPane.ERROR_MESSAGE);
					}
				}
				else
				{
					if (e.GetSource() == saveImage)
					{
						try
						{
							this.SaveImage(parent.GetLastSelectedRom());
						}
						catch (Exception ex)
						{
							JOptionPane.ShowMessageDialog(parent, new DebugPanel(ex, parent.GetSettings().GetSupportURL
								()), "Exception", JOptionPane.ERROR_MESSAGE);
						}
					}
					else
					{
						if (e.GetSource() == saveAsRepository)
						{
							try
							{
								this.SaveAsRepository(parent.GetLastSelectedRom(), parent.GetSettings().GetLastRepositoryDir
									());
							}
							catch (Exception ex)
							{
								JOptionPane.ShowMessageDialog(parent, new DebugPanel(ex, parent.GetSettings().GetSupportURL
									()), "Exception", JOptionPane.ERROR_MESSAGE);
							}
						}
						else
						{
							if (e.GetSource() == closeImage)
							{
								this.CloseImage();
							}
							else
							{
								if (e.GetSource() == closeAll)
								{
									this.CloseAllImages();
								}
								else
								{
									if (e.GetSource() == exit)
									{
										parent.HandleExit();
										System.Environment.Exit(0);
									}
									else
									{
										if (e.GetSource() == romProperties)
										{
											JOptionPane.ShowMessageDialog(parent, new RomPropertyPanel(parent.GetLastSelectedRom
												()), parent.GetLastSelectedRom().GetRomIDString() + " Properties", JOptionPane.INFORMATION_MESSAGE
												);
										}
										else
										{
											if (e.GetSource() == refreshImage)
											{
												try
												{
													RefreshImage();
												}
												catch (Exception ex)
												{
													JOptionPane.ShowMessageDialog(parent, new DebugPanel(ex, parent.GetSettings().GetSupportURL
														()), "Exception", JOptionPane.ERROR_MESSAGE);
												}
											}
											else
											{
												if (e.GetSource() == settingsMenuItem)
												{
													SettingsForm form = new SettingsForm(parent);
													form.SetLocationRelativeTo(parent);
													form.SetVisible(true);
												}
												else
												{
													if (e.GetSource() == compareImages)
													{
														CompareImagesForm form = new CompareImagesForm(parent.GetImages(), parent.GetIconImage
															());
														form.SetLocationRelativeTo(parent);
														form.SetVisible(true);
													}
													else
													{
														if (e.GetSource() == defManager)
														{
															DefinitionManager form = new DefinitionManager();
															form.SetLocationRelativeTo(parent);
															form.SetVisible(true);
														}
														else
														{
															if (e.GetSource() == level1)
															{
																parent.SetUserLevel(1);
															}
															else
															{
																if (e.GetSource() == level2)
																{
																	parent.SetUserLevel(2);
																}
																else
																{
																	if (e.GetSource() == level3)
																	{
																		parent.SetUserLevel(3);
																	}
																	else
																	{
																		if (e.GetSource() == level4)
																		{
																			parent.SetUserLevel(4);
																		}
																		else
																		{
																			if (e.GetSource() == level5)
																			{
																				parent.SetUserLevel(5);
																			}
																			else
																			{
																				if (e.GetSource() == openLogger)
																				{
																					parent.LaunchLogger();
																				}
																				else
																				{
																					if (e.GetSource() == updateDefinition)
																					{
																						BareBonesBrowserLaunch.OpenURL(Version.ECU_DEFS_URL);
																					}
																					else
																					{
																						if (e.GetSource() == launchRamTuneTestApp)
																						{
																							RamTuneTestApp.StartTestApp(WindowConstants.DISPOSE_ON_CLOSE);
																						}
																						else
																						{
																							if (e.GetSource() == about)
																							{
																								//TODO:  change this to use com.romraider.swing.menubar.action.AboutAction
																								string message = Version.PRODUCT_NAME + " - ECU Editor\n" + "Version: " + Version
																									.VERSION + "\n" + "Build #: " + Version.BUILDNUMBER + "\n" + Version.SUPPORT_URL;
																								string title = "About " + Version.PRODUCT_NAME;
																								JOptionPane.ShowMessageDialog(parent, message, title, JOptionPane.INFORMATION_MESSAGE
																									, Version.ABOUT_ICON);
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void RefreshImage()
		{
			if (parent.GetLastSelectedRom() != null)
			{
				FilePath file = parent.GetLastSelectedRom().GetFullFileName();
				parent.CloseImage();
				parent.OpenImage(file);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void OpenImageDialog()
		{
			JFileChooser fc = new JFileChooser(parent.GetSettings().GetLastImageDir());
			fc.SetFileFilter(new ECUImageFilter());
			fc.SetDialogTitle("Open Image");
			if (fc.ShowOpenDialog(parent) == JFileChooser.APPROVE_OPTION)
			{
				parent.OpenImage(fc.GetSelectedFile());
				parent.GetSettings().SetLastImageDir(fc.GetCurrentDirectory());
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void OpenImagesDialog()
		{
			JFileChooser fc = new JFileChooser(parent.GetSettings().GetLastImageDir());
			fc.SetFileFilter(new ECUImageFilter());
			fc.SetMultiSelectionEnabled(true);
			fc.SetDialogTitle("Open Image(s)");
			if (fc.ShowOpenDialog(parent) == JFileChooser.APPROVE_OPTION)
			{
				parent.OpenImages(fc.GetSelectedFiles());
				parent.GetSettings().SetLastImageDir(fc.GetCurrentDirectory());
			}
		}

		public virtual void CloseImage()
		{
			parent.CloseImage();
		}

		public virtual void CloseAllImages()
		{
			parent.CloseAllImages();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SaveImage(Rom input)
		{
			if (parent.GetLastSelectedRom() != null)
			{
				JFileChooser fc = new JFileChooser(parent.GetSettings().GetLastImageDir());
				fc.SetFileFilter(new ECUImageFilter());
				if (fc.ShowSaveDialog(parent) == JFileChooser.APPROVE_OPTION)
				{
					bool save = true;
					FilePath selectedFile = fc.GetSelectedFile();
					if (selectedFile.Exists())
					{
						int option = JOptionPane.ShowConfirmDialog(parent, selectedFile.GetName() + " already exists! Overwrite?"
							);
						// option: 0 = Cancel, 1 = No
						if (option == JOptionPane.CANCEL_OPTION || option == 1)
						{
							save = false;
						}
					}
					if (save)
					{
						byte[] output = parent.GetLastSelectedRom().SaveFile();
						FileOutputStream fos = new FileOutputStream(selectedFile);
						try
						{
							fos.Write(output);
						}
						finally
						{
							fos.Close();
						}
						parent.GetLastSelectedRom().SetFullFileName(selectedFile.GetAbsoluteFile());
						parent.SetLastSelectedRom(parent.GetLastSelectedRom());
						parent.GetSettings().SetLastImageDir(selectedFile.GetParentFile());
					}
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		private void SaveAsRepository(Rom image, FilePath lastRepositoryDir)
		{
			JFileChooser fc = new JFileChooser();
			fc.SetCurrentDirectory(lastRepositoryDir);
			fc.SetDialogTitle("Select Repository Directory");
			fc.SetFileSelectionMode(JFileChooser.DIRECTORIES_ONLY);
			// disable the "All files" option
			fc.SetAcceptAllFileFilterUsed(false);
			string separator = Runtime.GetProperty("file.separator");
			if (fc.ShowSaveDialog(parent) == JFileChooser.APPROVE_OPTION)
			{
				bool save = true;
				FilePath selectedDir = fc.GetSelectedFile();
				if (selectedDir.Exists())
				{
					int option = JOptionPane.ShowConfirmDialog(parent, selectedDir.GetName() + " already exists! Overwrite?"
						);
					// option: 0 = Cancel, 1 = No
					if (option == JOptionPane.CANCEL_OPTION || option == 1)
					{
						save = false;
					}
				}
				if (save)
				{
					Vector<Table> romTables = image.GetTables();
					for (int i = 0; i < romTables.Count; i++)
					{
						Table curTable = romTables[i];
						string category = curTable.GetCategory();
						string tableName = curTable.GetName();
						string tableDirString = selectedDir.GetAbsolutePath() + separator + category;
						FilePath tableDir = new FilePath(tableDirString.Replace('/', '-'));
						tableDir.Mkdirs();
						string tableFileString = tableDir.GetAbsolutePath() + separator + tableName + ".txt";
						FilePath tableFile = new FilePath(tableFileString.Replace('/', '-'));
						if (tableFile.Exists())
						{
							tableFile.Delete();
						}
						tableFile.CreateNewFile();
						StringBuilder tableData = curTable.GetTableAsString();
						BufferedWriter @out = new BufferedWriter(new FileWriter(tableFile));
						try
						{
							@out.Write(tableData.ToString());
						}
						finally
						{
							@out.Close();
						}
					}
					this.parent.GetSettings().SetLastRepositoryDir(selectedDir);
				}
			}
		}

		private string GetLastSelectedRomFileName()
		{
			Rom lastSelectedRom = parent.GetLastSelectedRom();
			return lastSelectedRom == null ? string.Empty : lastSelectedRom.GetFileName() + " ";
		}
	}
}
