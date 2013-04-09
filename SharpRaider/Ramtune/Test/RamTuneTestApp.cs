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
using System.IO;
using System.Text;
using Java.Awt;
using Java.Awt.Event;
using Javax.Swing;
using Javax.Swing.Border;
using RomRaider;
using RomRaider.IO.Connection;
using RomRaider.IO.Protocol;
using RomRaider.IO.Serial.Port;
using RomRaider.Logger.Ecu.Comms.IO.Protocol;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Ramtune.Test.Command.Executor;
using RomRaider.Ramtune.Test.Command.Generator;
using RomRaider.Ramtune.Test.IO;
using RomRaider.Swing;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Ramtune.Test
{
	[System.Serializable]
	public sealed class RamTuneTestApp : AbstractFrame
	{
		private const long serialVersionUID = 7140513114169019846L;

		private static readonly string REGEX_VALID_ADDRESS_BYTES = "[0-9a-fA-F]{6}";

		private static readonly string REGEX_VALID_DATA_BYTES = "[0-9a-fA-F]{2,}";

		private static readonly PollingState pollMode = new PollingStateImpl();

		private static readonly string ISO9141 = "ISO9141";

		private static RomRaider.IO.Protocol.Protocol protocol;

		private readonly Settings settings;

		private readonly JTextField addressField = new JTextField(6);

		private readonly JTextField lengthField = new JTextField(4);

		private readonly JTextField sendTimeoutField = new JTextField(4);

		private readonly JTextArea dataField = new JTextArea(10, 80);

		private readonly JTextArea responseField = new JTextArea(15, 80);

		private readonly SerialPortComboBox portsComboBox;

		private readonly JComboBox commandComboBox;

		private static byte ecuId = unchecked((int)(0x10));

		private static string userTp;

		private static string userLibrary;

		public RamTuneTestApp(string title) : base(title)
		{
			SettingsManager manager = new SettingsManagerImpl();
			settings = manager.Load();
			portsComboBox = new SerialPortComboBox(settings);
			userTp = Settings.GetTransportProtocol();
			userLibrary = Settings.GetJ2534Device();
			Settings.SetTransportProtocol(ISO9141);
			// Read Address blocks only seems to work with ISO9141, it
			// may not be implemented in the ECU for ISO15765
			LoggerProtocol lp = ProtocolFactory.GetProtocol("SSM", ISO9141);
			protocol = lp.GetProtocol();
			commandComboBox = new JComboBox(new CommandGenerator[] { new EcuInitCommandGenerator
				(protocol), new ReadCommandGenerator(protocol), new WriteCommandGenerator(protocol
				) });
			InitUserInterface();
			StartPortRefresherThread();
		}

		private void StartPortRefresherThread()
		{
			SerialPortRefresher serialPortRefresher = new SerialPortRefresher(portsComboBox, 
				settings.GetLoggerPort());
			ThreadUtil.RunAsDaemon(serialPortRefresher);
			// wait until port refresher fully started before continuing
			while (!serialPortRefresher.IsStarted())
			{
				ThreadUtil.Sleep(100);
			}
		}

		private void InitUserInterface()
		{
			// setup main panel
			JPanel mainPanel = new JPanel(new BorderLayout());
			JPanel contentPanel = BuildContentPanel();
			mainPanel.Add(contentPanel, BorderLayout.CENTER);
			// add to container
			GetContentPane().Add(mainPanel);
		}

		private JPanel BuildContentPanel()
		{
			GridBagLayout gridBagLayout = new GridBagLayout();
			JPanel mainPanel = new JPanel(gridBagLayout);
			GridBagConstraints constraints = new GridBagConstraints();
			constraints.fill = GridBagConstraints.BOTH;
			constraints.insets = new Insets(3, 5, 3, 5);
			constraints.gridx = 0;
			constraints.gridy = 0;
			constraints.gridwidth = 2;
			constraints.gridheight = 1;
			constraints.weightx = 1;
			constraints.weighty = 0;
			mainPanel.Add(BuildComPortPanel(), constraints);
			constraints.gridx = 0;
			constraints.gridy = 1;
			constraints.gridwidth = 1;
			constraints.gridheight = 1;
			constraints.weightx = 1;
			constraints.weighty = 1;
			mainPanel.Add(BuildInputPanel(), constraints);
			constraints.gridx = 0;
			constraints.gridy = 2;
			constraints.gridwidth = 1;
			constraints.gridheight = 1;
			constraints.weightx = 1;
			constraints.weighty = 1;
			mainPanel.Add(BuildOutputPanel(), constraints);
			return mainPanel;
		}

		private Component BuildInputPanel()
		{
			GridBagLayout gridBagLayout = new GridBagLayout();
			JPanel inputPanel = new JPanel(gridBagLayout);
			inputPanel.SetBorder(new TitledBorder(new EtchedBorder(EtchedBorder.LOWERED), "Command"
				));
			GridBagConstraints constraints = new GridBagConstraints();
			constraints.fill = GridBagConstraints.BOTH;
			constraints.insets = new Insets(0, 5, 5, 5);
			constraints.gridx = 0;
			constraints.gridy = 0;
			constraints.gridwidth = 2;
			constraints.gridheight = 1;
			constraints.weightx = 1;
			constraints.weighty = 1;
			inputPanel.Add(commandComboBox, constraints);
			JPanel addressFieldPanel = new JPanel(new FlowLayout());
			addressFieldPanel.Add(new JLabel("Address (eg. 020000):"));
			addressFieldPanel.Add(addressField);
			JPanel lengthPanel = new JPanel(new FlowLayout());
			lengthPanel.Add(new JLabel("   Read Length:"));
			lengthField.SetText("1");
			lengthPanel.Add(lengthField);
			lengthPanel.Add(new JLabel("byte(s)"));
			JPanel addressPanel = new JPanel(new FlowLayout(FlowLayout.LEFT));
			addressPanel.Add(addressFieldPanel);
			addressPanel.Add(lengthPanel);
			constraints.gridx = 3;
			constraints.gridy = 0;
			constraints.gridwidth = 2;
			constraints.gridheight = 1;
			constraints.weightx = 1;
			constraints.weighty = 1;
			inputPanel.Add(addressPanel, constraints);
			dataField.SetFont(new Font("Monospaced", Font.PLAIN, 12));
			dataField.SetLineWrap(true);
			dataField.SetBorder(new BevelBorder(BevelBorder.LOWERED));
			constraints.gridx = 0;
			constraints.gridy = 1;
			constraints.gridwidth = 5;
			constraints.gridheight = 1;
			constraints.weightx = 1;
			constraints.weighty = 1;
			inputPanel.Add(new JScrollPane(dataField, ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED
				, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER), constraints);
			constraints.gridx = 0;
			constraints.gridy = 2;
			constraints.gridwidth = 5;
			constraints.gridheight = 1;
			constraints.weightx = 1;
			constraints.weighty = 1;
			inputPanel.Add(BuildSendButton(), constraints);
			return inputPanel;
		}

		private Component BuildOutputPanel()
		{
			responseField.SetFont(new Font("Monospaced", Font.PLAIN, 12));
			responseField.SetLineWrap(true);
			responseField.SetEditable(false);
			responseField.SetBorder(new BevelBorder(BevelBorder.LOWERED));
			JScrollPane responseScrollPane = new JScrollPane(responseField, ScrollPaneConstants
				.VERTICAL_SCROLLBAR_AS_NEEDED, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
			responseScrollPane.SetBorder(new TitledBorder(new EtchedBorder(EtchedBorder.LOWERED
				), "Trace"));
			return responseScrollPane;
		}

		private JButton BuildSendButton()
		{
			JButton button = new JButton("Send Command");
			button.AddActionListener(new _ActionListener_258(this, button));
			return button;
		}

		private sealed class _ActionListener_258 : ActionListener
		{
			public _ActionListener_258(RamTuneTestApp _enclosing, JButton button)
			{
				this._enclosing = _enclosing;
				this.button = button;
			}

			public void ActionPerformed(ActionEvent e)
			{
				ThreadUtil.RunAsDaemon(new _Runnable_260(this, button));
			}

			private sealed class _Runnable_260 : Runnable
			{
				public _Runnable_260(_ActionListener_258 _enclosing, JButton button)
				{
					this._enclosing = _enclosing;
					this.button = button;
				}

				public void Run()
				{
					button.SetEnabled(false);
					try
					{
						ConnectionProperties connectionProperties = new RamTuneTestAppConnectionProperties
							(RomRaider.Ramtune.Test.RamTuneTestApp.protocol.GetDefaultConnectionProperties()
							, this._enclosing._enclosing.GetSendTimeout());
						CommandExecutor commandExecutor = new CommandExecutorImpl(connectionProperties, (
							string)this._enclosing._enclosing.portsComboBox.GetSelectedItem());
						CommandGenerator commandGenerator = (CommandGenerator)this._enclosing._enclosing.
							commandComboBox.GetSelectedItem();
						if (this._enclosing._enclosing.ValidateInput(commandGenerator) && this._enclosing
							._enclosing.ConfirmCommandExecution(commandGenerator))
						{
							StringBuilder builder = new StringBuilder();
							IList<byte[]> commands = commandGenerator.CreateCommands(RomRaider.Ramtune.Test.RamTuneTestApp
								.ecuId, this._enclosing._enclosing.GetData(), this._enclosing._enclosing.GetAddress
								(), this._enclosing._enclosing.GetLength());
							foreach (byte[] command in commands)
							{
								this._enclosing._enclosing.AppendResponseLater("SND [" + commandGenerator + "]:\t"
									 + HexUtil.AsHex(command) + "\n");
								byte[] response = RomRaider.Ramtune.Test.RamTuneTestApp.protocol.PreprocessResponse
									(command, commandExecutor.ExecuteCommand(command), RomRaider.Ramtune.Test.RamTuneTestApp
									.pollMode);
								this._enclosing._enclosing.AppendResponseLater("RCV [" + commandGenerator + "]:\t"
									 + HexUtil.AsHex(response) + "\n");
								builder.Append(HexUtil.AsHex(RomRaider.Ramtune.Test.RamTuneTestApp.protocol.ParseResponseData
									(response)));
							}
							this._enclosing._enclosing.AppendResponseLater("DATA [Raw]:\t" + builder.ToString
								() + "\n\n");
						}
						commandExecutor.Close();
					}
					catch (Exception ex)
					{
						this._enclosing._enclosing.ReportError(ex);
					}
					finally
					{
						button.SetEnabled(true);
					}
				}

				private readonly _ActionListener_258 _enclosing;

				private readonly JButton button;
			}

			private readonly RamTuneTestApp _enclosing;

			private readonly JButton button;
		}

		private void AppendResponseLater(string text)
		{
			SwingUtilities.InvokeLater(new _Runnable_292(this, text));
		}

		private sealed class _Runnable_292 : Runnable
		{
			public _Runnable_292(RamTuneTestApp _enclosing, string text)
			{
				this._enclosing = _enclosing;
				this.text = text;
			}

			public void Run()
			{
				this._enclosing.responseField.Append(text);
			}

			private readonly RamTuneTestApp _enclosing;

			private readonly string text;
		}

		private byte[] GetAddress()
		{
			try
			{
				return HexUtil.AsBytes(addressField.GetText());
			}
			catch (Exception)
			{
				return null;
			}
		}

		private byte[] GetData()
		{
			try
			{
				return HexUtil.AsBytes(dataField.GetText());
			}
			catch (Exception)
			{
				return null;
			}
		}

		private int GetLength()
		{
			return GetIntFromField(lengthField);
		}

		private int GetSendTimeout()
		{
			return GetIntFromField(sendTimeoutField);
		}

		private int GetIntFromField(JTextField field)
		{
			try
			{
				return System.Convert.ToInt32(field.GetText().Trim());
			}
			catch (FormatException)
			{
				return -1;
			}
		}

		private bool ValidateInput(CommandGenerator commandGenerator)
		{
			bool isReadCommandGenerator = typeof(ReadCommandGenerator).IsAssignableFrom(commandGenerator
				.GetType());
			bool isWriteCommandGenerator = typeof(WriteCommandGenerator).IsAssignableFrom(commandGenerator
				.GetType());
			if (isReadCommandGenerator || isWriteCommandGenerator)
			{
				string address = addressField.GetText();
				if (address.Trim().Length != 6)
				{
					ShowErrorDialog("Invalid address - must be 3 bytes long.");
					return false;
				}
				else
				{
					if (!address.Matches(REGEX_VALID_ADDRESS_BYTES))
					{
						ShowErrorDialog("Invalid address - bad bytes.");
						return false;
					}
				}
			}
			if (isReadCommandGenerator)
			{
				try
				{
					int length = System.Convert.ToInt32(lengthField.GetText().Trim());
					if (length <= 0)
					{
						ShowErrorDialog("Invalid length - must be greater then zero.");
						return false;
					}
				}
				catch (FormatException)
				{
					ShowErrorDialog("Invalid length.");
					return false;
				}
			}
			if (isWriteCommandGenerator)
			{
				string data = dataField.GetText().Trim();
				int dataLength = data.Length;
				if (dataLength == 0)
				{
					ShowErrorDialog("No data specified.");
					return false;
				}
				else
				{
					if (dataLength % 2 != 0)
					{
						ShowErrorDialog("Invalid data - odd number of characters.");
						return false;
					}
					else
					{
						if (!data.Matches(REGEX_VALID_DATA_BYTES))
						{
							ShowErrorDialog("Invalid data - bad bytes.");
							return false;
						}
					}
				}
			}
			return true;
		}

		private bool ConfirmCommandExecution(CommandGenerator commandGenerator)
		{
			bool isWriteCommandGenerator = typeof(WriteCommandGenerator).IsAssignableFrom(commandGenerator
				.GetType());
			return !isWriteCommandGenerator || JOptionPane.ShowConfirmDialog(this, "Are you sure you want to write to ECU memory?"
				, "Confirm Write Command", JOptionPane.YES_NO_OPTION, JOptionPane.WARNING_MESSAGE
				) == JOptionPane.YES_OPTION;
		}

		private JPanel BuildComPortPanel()
		{
			JPanel panel = new JPanel(new FlowLayout(FlowLayout.LEFT));
			panel.Add(BuildComPorts());
			panel.Add(BuildSendTimeout());
			JCheckBox ecuCheckBox = new JCheckBox("ECU");
			JCheckBox tcuCheckBox = new JCheckBox("TCU");
			ecuCheckBox.AddActionListener(new _ActionListener_385(tcuCheckBox));
			tcuCheckBox.AddActionListener(new _ActionListener_391(ecuCheckBox));
			ecuCheckBox.SetSelected(true);
			panel.Add(ecuCheckBox);
			panel.Add(tcuCheckBox);
			return panel;
		}

		private sealed class _ActionListener_385 : ActionListener
		{
			public _ActionListener_385(JCheckBox tcuCheckBox)
			{
				this.tcuCheckBox = tcuCheckBox;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				tcuCheckBox.SetSelected(false);
				RomRaider.Ramtune.Test.RamTuneTestApp.ecuId = unchecked((int)(0x10));
			}

			private readonly JCheckBox tcuCheckBox;
		}

		private sealed class _ActionListener_391 : ActionListener
		{
			public _ActionListener_391(JCheckBox ecuCheckBox)
			{
				this.ecuCheckBox = ecuCheckBox;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				ecuCheckBox.SetSelected(false);
				RomRaider.Ramtune.Test.RamTuneTestApp.ecuId = unchecked((int)(0x18));
			}

			private readonly JCheckBox ecuCheckBox;
		}

		private Component BuildSendTimeout()
		{
			sendTimeoutField.SetText("55");
			JPanel panel = new JPanel(new FlowLayout());
			panel.Add(new JLabel("Send Timeout:"));
			panel.Add(sendTimeoutField);
			panel.Add(new JLabel("ms"));
			return panel;
		}

		private JPanel BuildComPorts()
		{
			JPanel panel = new JPanel(new FlowLayout());
			panel.Add(new JLabel("COM Port:"));
			panel.Add(portsComboBox);
			return panel;
		}

		private void ReportError(Exception e)
		{
			TextWriter writer = new StringWriter();
			PrintWriter printWriter = new PrintWriter(writer);
			Sharpen.Runtime.PrintStackTrace(e, printWriter);
			responseField.Append("\n**************************************************************************\n"
				);
			responseField.Append("ERROR: ");
			responseField.Append(writer.ToString());
			responseField.Append("\n**************************************************************************\n\n"
				);
		}

		//showErrorDialog("An error occurred:\n\n" + writer.toString());
		private void ShowErrorDialog(string message)
		{
			JOptionPane.ShowMessageDialog(this, message, "Error", JOptionPane.ERROR_MESSAGE);
		}

		//**********************************************************************
		public static void Main(string[] args)
		{
			LogManager.InitDebugLogging();
			LookAndFeelManager.InitLookAndFeel();
			StartTestApp(EXIT_ON_CLOSE);
		}

		public static void StartTestApp(int defaultCloseOperation)
		{
			SwingUtilities.InvokeLater(new _Runnable_445(defaultCloseOperation));
		}

		private sealed class _Runnable_445 : Runnable
		{
			public _Runnable_445(int defaultCloseOperation)
			{
				this.defaultCloseOperation = defaultCloseOperation;
			}

			public void Run()
			{
				RomRaider.Ramtune.Test.RamTuneTestApp ramTuneTestApp = new RomRaider.Ramtune.Test.RamTuneTestApp
					("SSM Read/Write");
				ramTuneTestApp.SetIconImage(new ImageIcon(this.GetType().GetResource("/graphics/romraider-ico.gif"
					)).GetImage());
				ramTuneTestApp.SetDefaultCloseOperation(defaultCloseOperation);
				ramTuneTestApp.AddWindowListener(ramTuneTestApp);
				ramTuneTestApp.SetLocation(100, 50);
				ramTuneTestApp.Pack();
				ramTuneTestApp.SetVisible(true);
			}

			private readonly int defaultCloseOperation;
		}

		public override void WindowClosing(WindowEvent e)
		{
			Settings.SetTransportProtocol(userTp);
			Settings.SetJ2534Device(userLibrary);
		}
	}
}
