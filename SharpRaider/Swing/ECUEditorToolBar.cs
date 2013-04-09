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
using Java.Awt;
using Java.Awt.Event;
using Javax.Swing;
using RomRaider.Editor.Ecu;
using RomRaider.Maps;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class ECUEditorToolBar : JToolBar, ActionListener
	{
		private const long serialVersionUID = 7778170684606193919L;

		private readonly ECUEditor parent;

		private readonly JButton openImage = new JButton();

		private readonly JButton saveImage = new JButton();

		private readonly JButton refreshImage = new JButton();

		private readonly JButton closeImage = new JButton();

		public ECUEditorToolBar(ECUEditor parent, string name) : base(name)
		{
			this.parent = parent;
			this.SetFloatable(true);
			this.SetRollover(true);
			FlowLayout toolBarLayout = new FlowLayout(FlowLayout.LEFT, 0, 0);
			this.SetLayout(toolBarLayout);
			//this.setBorder(BorderFactory.createTitledBorder("Editor Tools"));
			this.UpdateIcons();
			this.Add(openImage);
			this.Add(saveImage);
			this.Add(closeImage);
			this.Add(refreshImage);
			openImage.SetMaximumSize(new Dimension(58, 50));
			openImage.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 0));
			saveImage.SetMaximumSize(new Dimension(50, 50));
			saveImage.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 0));
			closeImage.SetMaximumSize(new Dimension(50, 50));
			closeImage.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 0));
			refreshImage.SetMaximumSize(new Dimension(50, 50));
			refreshImage.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 0
				));
			this.UpdateButtons();
			openImage.AddActionListener(this);
			saveImage.AddActionListener(this);
			closeImage.AddActionListener(this);
			refreshImage.AddActionListener(this);
		}

		public virtual void UpdateIcons()
		{
			openImage.SetIcon(RescaleImageIcon(new ImageIcon(GetType().GetResource("/graphics/icon-open.png"
				)), parent.GetSettings().GetEditorIconScale()));
			saveImage.SetIcon(RescaleImageIcon(new ImageIcon(GetType().GetResource("/graphics/icon-save.png"
				)), parent.GetSettings().GetEditorIconScale()));
			refreshImage.SetIcon(RescaleImageIcon(new ImageIcon(GetType().GetResource("/graphics/icon-refresh.png"
				)), parent.GetSettings().GetEditorIconScale()));
			closeImage.SetIcon(RescaleImageIcon(new ImageIcon(GetType().GetResource("/graphics/icon-close.png"
				)), parent.GetSettings().GetEditorIconScale()));
		}

		private ImageIcon RescaleImageIcon(ImageIcon imageIcon, int percentOfOriginal)
		{
			int newHeight = (int)(imageIcon.GetImage().GetHeight(this) * (percentOfOriginal *
				 .01));
			int newWidth = (int)(imageIcon.GetImage().GetWidth(this) * (percentOfOriginal * .01
				));
			if (newHeight > 0 && newWidth > 0)
			{
				imageIcon.SetImage(imageIcon.GetImage().GetScaledInstance(newWidth, newHeight, Image
					.SCALE_SMOOTH));
			}
			return imageIcon;
		}

		public virtual void UpdateButtons()
		{
			string file = GetLastSelectedRomFileName();
			openImage.SetToolTipText("Open Image");
			saveImage.SetToolTipText("Save " + file + " As New Image...");
			refreshImage.SetToolTipText("Refresh " + file + " from saved copy");
			closeImage.SetToolTipText("Close " + file);
			if (string.Empty.Equals(file))
			{
				saveImage.SetEnabled(false);
				refreshImage.SetEnabled(false);
				closeImage.SetEnabled(false);
			}
			else
			{
				saveImage.SetEnabled(true);
				refreshImage.SetEnabled(true);
				closeImage.SetEnabled(true);
			}
		}

		public virtual void ActionPerformed(ActionEvent e)
		{
			if (e.GetSource() == openImage)
			{
				try
				{
					((ECUEditorMenuBar)parent.GetJMenuBar()).OpenImageDialog();
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
						((ECUEditorMenuBar)parent.GetJMenuBar()).SaveImage(parent.GetLastSelectedRom());
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
						((ECUEditorMenuBar)parent.GetJMenuBar()).CloseImage();
					}
					else
					{
						if (e.GetSource() == refreshImage)
						{
							try
							{
								((ECUEditorMenuBar)parent.GetJMenuBar()).RefreshImage();
							}
							catch (Exception ex)
							{
								JOptionPane.ShowMessageDialog(parent, new DebugPanel(ex, parent.GetSettings().GetSupportURL
									()), "Exception", JOptionPane.ERROR_MESSAGE);
							}
						}
					}
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
