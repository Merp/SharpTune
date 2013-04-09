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
using Java.Awt.Datatransfer;
using Java.Awt.Event;
using Javax.Swing;
using RomRaider;
using RomRaider.Editor.Ecu;
using RomRaider.Maps;
using RomRaider.Swing;
using RomRaider.Util;
using RomRaider.Xml;
using Sharpen;

namespace RomRaider.Maps
{
	[System.Serializable]
	public abstract class Table : JPanel
	{
		private const long serialVersionUID = 6559256489995552645L;

		protected internal static readonly string BLANK = string.Empty;

		public const int ENDIAN_LITTLE = 1;

		public const int ENDIAN_BIG = 2;

		public const int TABLE_1D = 1;

		public const int TABLE_2D = 2;

		public const int TABLE_3D = 3;

		public const int TABLE_X_AXIS = 4;

		public const int TABLE_Y_AXIS = 5;

		public const int TABLE_SWITCH = 6;

		public const int COMPARE_TYPE_ORIGINAL = 0;

		public const int COMPARE_TYPE_BIN = 1;

		public const int COMPARE_DISPLAY_OFF = 0;

		public const int COMPARE_DISPLAY_PERCENT = 1;

		public const int COMPARE_DISPLAY_ABSOLUTE = 2;

		public const int STORAGE_TYPE_FLOAT = 99;

		public const bool STORAGE_DATA_SIGNED = false;

		protected internal static readonly Color UNCHANGED_VALUE_COLOR = new Color(160, 160
			, 160);

		protected internal static readonly string NEW_LINE = Runtime.GetProperty("line.separator"
			);

		protected internal static readonly string TAB = "\t";

		protected internal string name;

		protected internal int type;

		protected internal string category = "Other";

		protected internal string description = BLANK;

		protected internal Vector<Scale> scales = new Vector<Scale>();

		protected internal int scaleIndex = 0;

		protected internal int storageAddress;

		protected internal int storageType;

		protected internal bool signed;

		protected internal int endian;

		protected internal bool flip;

		protected internal DataCell[] data = new DataCell[0];

		protected internal bool isStatic = false;

		protected internal bool beforeRam = false;

		protected internal int ramOffset = 0;

		protected internal BorderLayout borderLayout = new BorderLayout();

		protected internal GridLayout centerLayout = new GridLayout(1, 1, 0, 0);

		protected internal JPanel centerPanel;

		protected internal TableFrame frame;

		protected internal int verticalOverhead = 103;

		protected internal int horizontalOverhead = 2;

		protected internal int cellHeight = 18;

		protected internal int cellWidth = 42;

		protected internal int minHeight = 100;

		protected internal int minWidthNoOverlay = 465;

		protected internal int minWidthOverlay = 700;

		protected internal Rom container;

		protected internal int highlightX;

		protected internal int highlightY;

		protected internal bool highlight = false;

		protected internal RomRaider.Maps.Table axisParent;

		protected internal Color maxColor;

		protected internal Color minColor;

		protected internal bool isAxis = false;

		protected internal int userLevel = 0;

		protected internal ECUEditor editor;

		protected internal bool locked = false;

		protected internal int compareType = COMPARE_TYPE_ORIGINAL;

		protected internal int compareDisplay = COMPARE_DISPLAY_OFF;

		protected internal RomRaider.Maps.Table compareTable = null;

		protected internal IList<RomRaider.Maps.Table> comparedToTables = new AList<RomRaider.Maps.Table
			>();

		protected internal string logParam = BLANK;

		protected internal string liveValue = BLANK;

		protected internal bool overlayLog = false;

		protected internal CopyTableWorker copyTableWorker;

		protected internal CopySelectionWorker copySelectionWorker;

		public Table(ECUEditor editor)
		{
			centerPanel = new JPanel(centerLayout);
			// index of selected scale
			this.editor = editor;
			this.SetLayout(borderLayout);
			this.Add(centerPanel, BorderLayout.CENTER);
			centerPanel.SetVisible(true);
			// key binding actions
			Action rightAction = new _AbstractAction_149(this);
			Action leftAction = new _AbstractAction_157(this);
			Action downAction = new _AbstractAction_165(this);
			Action upAction = new _AbstractAction_173(this);
			Action incCoarseAction = new _AbstractAction_181(this);
			Action decCoarseAction = new _AbstractAction_189(this);
			Action incFineAction = new _AbstractAction_197(this);
			Action decFineAction = new _AbstractAction_205(this);
			Action num0Action = new _AbstractAction_213(this);
			Action num1Action = new _AbstractAction_221(this);
			Action num2Action = new _AbstractAction_229(this);
			Action num3Action = new _AbstractAction_237(this);
			Action num4Action = new _AbstractAction_245(this);
			Action num5Action = new _AbstractAction_253(this);
			Action num6Action = new _AbstractAction_261(this);
			Action num7Action = new _AbstractAction_269(this);
			Action num8Action = new _AbstractAction_277(this);
			Action num9Action = new _AbstractAction_285(this);
			Action numPointAction = new _AbstractAction_293(this);
			Action copyAction = new _AbstractAction_301(this);
			Action pasteAction = new _AbstractAction_309(this);
			Action multiplyAction = new _AbstractAction_317(this);
			Action numNegAction = new _AbstractAction_325(this);
			// set input mapping
			InputMap im = GetInputMap(WHEN_IN_FOCUSED_WINDOW);
			KeyStroke right = KeyStroke.GetKeyStroke(KeyEvent.VK_RIGHT, 0);
			KeyStroke left = KeyStroke.GetKeyStroke(KeyEvent.VK_LEFT, 0);
			KeyStroke up = KeyStroke.GetKeyStroke(KeyEvent.VK_UP, 0);
			KeyStroke down = KeyStroke.GetKeyStroke(KeyEvent.VK_DOWN, 0);
			KeyStroke decrement = KeyStroke.GetKeyStroke('-');
			KeyStroke increment = KeyStroke.GetKeyStroke('+');
			KeyStroke decrement2 = KeyStroke.GetKeyStroke("control DOWN");
			KeyStroke increment2 = KeyStroke.GetKeyStroke("control UP");
			KeyStroke decrement3 = KeyStroke.GetKeyStroke(KeyEvent.VK_MINUS, KeyEvent.CTRL_DOWN_MASK
				);
			KeyStroke increment3 = KeyStroke.GetKeyStroke(KeyEvent.VK_PLUS, KeyEvent.CTRL_DOWN_MASK
				);
			KeyStroke decrement4 = KeyStroke.GetKeyStroke("control shift DOWN");
			KeyStroke increment4 = KeyStroke.GetKeyStroke("control shift UP");
			KeyStroke num0 = KeyStroke.GetKeyStroke('0');
			KeyStroke num1 = KeyStroke.GetKeyStroke('1');
			KeyStroke num2 = KeyStroke.GetKeyStroke('2');
			KeyStroke num3 = KeyStroke.GetKeyStroke('3');
			KeyStroke num4 = KeyStroke.GetKeyStroke('4');
			KeyStroke num5 = KeyStroke.GetKeyStroke('5');
			KeyStroke num6 = KeyStroke.GetKeyStroke('6');
			KeyStroke num7 = KeyStroke.GetKeyStroke('7');
			KeyStroke num8 = KeyStroke.GetKeyStroke('8');
			KeyStroke num9 = KeyStroke.GetKeyStroke('9');
			KeyStroke mulKey = KeyStroke.GetKeyStroke('*');
			KeyStroke mulKeys = KeyStroke.GetKeyStroke(KeyEvent.VK_ENTER, KeyEvent.CTRL_DOWN_MASK
				);
			KeyStroke numPoint = KeyStroke.GetKeyStroke('.');
			KeyStroke copy = KeyStroke.GetKeyStroke("control C");
			KeyStroke paste = KeyStroke.GetKeyStroke("control V");
			KeyStroke numNeg = KeyStroke.GetKeyStroke('-');
			im.Put(right, "right");
			im.Put(left, "left");
			im.Put(up, "up");
			im.Put(down, "down");
			im.Put(increment, "incCoarseAction");
			im.Put(decrement, "decCoarseAction");
			im.Put(increment2, "incCoarseAction");
			im.Put(decrement2, "decCoarseAction");
			im.Put(increment3, "incFineAction");
			im.Put(decrement3, "decFineAction");
			im.Put(increment4, "incFineAction");
			im.Put(decrement4, "decFineAction");
			im.Put(num0, "num0Action");
			im.Put(num1, "num1Action");
			im.Put(num2, "num2Action");
			im.Put(num3, "num3Action");
			im.Put(num4, "num4Action");
			im.Put(num5, "num5Action");
			im.Put(num6, "num6Action");
			im.Put(num7, "num7Action");
			im.Put(num8, "num8Action");
			im.Put(num9, "num9Action");
			im.Put(numPoint, "numPointAction");
			im.Put(copy, "copyAction");
			im.Put(paste, "pasteAction");
			im.Put(mulKey, "mulAction");
			im.Put(mulKeys, "mulAction");
			im.Put(numNeg, "numNeg");
			GetActionMap().Put(im.Get(right), rightAction);
			GetActionMap().Put(im.Get(left), leftAction);
			GetActionMap().Put(im.Get(up), upAction);
			GetActionMap().Put(im.Get(down), downAction);
			GetActionMap().Put(im.Get(increment), incCoarseAction);
			GetActionMap().Put(im.Get(decrement), decCoarseAction);
			GetActionMap().Put(im.Get(increment2), incCoarseAction);
			GetActionMap().Put(im.Get(decrement2), decCoarseAction);
			GetActionMap().Put(im.Get(increment3), incFineAction);
			GetActionMap().Put(im.Get(decrement3), decFineAction);
			GetActionMap().Put(im.Get(increment4), incFineAction);
			GetActionMap().Put(im.Get(decrement4), decFineAction);
			GetActionMap().Put(im.Get(num0), num0Action);
			GetActionMap().Put(im.Get(num1), num1Action);
			GetActionMap().Put(im.Get(num2), num2Action);
			GetActionMap().Put(im.Get(num3), num3Action);
			GetActionMap().Put(im.Get(num4), num4Action);
			GetActionMap().Put(im.Get(num5), num5Action);
			GetActionMap().Put(im.Get(num6), num6Action);
			GetActionMap().Put(im.Get(num7), num7Action);
			GetActionMap().Put(im.Get(num8), num8Action);
			GetActionMap().Put(im.Get(num9), num9Action);
			GetActionMap().Put(im.Get(numPoint), numPointAction);
			GetActionMap().Put(im.Get(mulKey), multiplyAction);
			GetActionMap().Put(im.Get(mulKeys), multiplyAction);
			GetActionMap().Put(im.Get(copy), copyAction);
			GetActionMap().Put(im.Get(paste), pasteAction);
			GetActionMap().Put(im.Get(numNeg), numNegAction);
			this.SetInputMap(WHEN_FOCUSED, im);
		}

		private sealed class _AbstractAction_149 : AbstractAction
		{
			public _AbstractAction_149(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 1042884198300385041L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.CursorRight();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_157 : AbstractAction
		{
			public _AbstractAction_157(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -4970441255677214171L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.CursorLeft();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_165 : AbstractAction
		{
			public _AbstractAction_165(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -7898502951121825984L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.CursorDown();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_173 : AbstractAction
		{
			public _AbstractAction_173(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 6937621541727666631L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.CursorUp();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_181 : AbstractAction
		{
			public _AbstractAction_181(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -8308522736529183148L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.frame.GetToolBar().IncrementCoarse();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_189 : AbstractAction
		{
			public _AbstractAction_189(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -7407628920997400915L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.frame.GetToolBar().DecrementCoarse();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_197 : AbstractAction
		{
			public _AbstractAction_197(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 7261463425941761433L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.frame.GetToolBar().IncrementFine();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_205 : AbstractAction
		{
			public _AbstractAction_205(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 8929400237520608035L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.frame.GetToolBar().DecrementFine();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_213 : AbstractAction
		{
			public _AbstractAction_213(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -6310984176739090034L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('0');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_221 : AbstractAction
		{
			public _AbstractAction_221(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -6187220355403883499L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('1');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_229 : AbstractAction
		{
			public _AbstractAction_229(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -8745505977907325720L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('2');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_237 : AbstractAction
		{
			public _AbstractAction_237(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 4694872385823448942L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('3');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_245 : AbstractAction
		{
			public _AbstractAction_245(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 4005741329254221678L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('4');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_253 : AbstractAction
		{
			public _AbstractAction_253(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -5846094949106279884L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('5');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_261 : AbstractAction
		{
			public _AbstractAction_261(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -5338656374925334150L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('6');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_269 : AbstractAction
		{
			public _AbstractAction_269(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 1959983381590509303L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('7');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_277 : AbstractAction
		{
			public _AbstractAction_277(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 7442763278699460648L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('8');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_285 : AbstractAction
		{
			public _AbstractAction_285(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 7475171864584215094L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('9');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_293 : AbstractAction
		{
			public _AbstractAction_293(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -4729135055857591830L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('.');
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_301 : AbstractAction
		{
			public _AbstractAction_301(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -6978981449261938672L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.CopySelection();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_309 : AbstractAction
		{
			public _AbstractAction_309(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = 2026817603236490899L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.Paste();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_317 : AbstractAction
		{
			public _AbstractAction_317(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -2350912575392447149L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().Multiply();
			}

			private readonly Table _enclosing;
		}

		private sealed class _AbstractAction_325 : AbstractAction
		{
			public _AbstractAction_325(Table _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -6346750245035640773L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetFrame().GetToolBar().FocusSetValue('-');
			}

			private readonly Table _enclosing;
		}

		public virtual DataCell[] GetData()
		{
			return data;
		}

		public virtual void SetData(DataCell[] data)
		{
			this.data = data;
		}

		/// <exception cref="System.IndexOutOfRangeException"></exception>
		public virtual void PopulateTable(byte[] input)
		{
			if (scales.IsEmpty())
			{
				scales.AddItem(new Scale());
			}
			// temporarily remove lock
			bool tempLock = locked;
			locked = false;
			if (!isStatic)
			{
				if (!beforeRam)
				{
					ramOffset = container.GetRomID().GetRamOffset();
				}
				for (int i = 0; i < data.Length; i++)
				{
					if (data[i] == null)
					{
						data[i] = new DataCell(scales[scaleIndex], GetEditor().GetSettings().GetCellSize(
							));
						data[i].SetTable(this);
						// populate data cells
						if (storageType == STORAGE_TYPE_FLOAT)
						{
							//float storage type
							byte[] byteValue = new byte[4];
							byteValue[0] = input[storageAddress + i * 4 - ramOffset];
							byteValue[1] = input[storageAddress + i * 4 - ramOffset + 1];
							byteValue[2] = input[storageAddress + i * 4 - ramOffset + 2];
							byteValue[3] = input[storageAddress + i * 4 - ramOffset + 3];
							data[i].SetBinValue(RomAttributeParser.ByteToFloat(byteValue, endian));
						}
						else
						{
							// integer storage type
							data[i].SetBinValue(RomAttributeParser.ParseByteValue(input, endian, storageAddress
								 + i * storageType - ramOffset, storageType, signed));
						}
						data[i].SetPreferredSize(new Dimension(cellWidth, cellHeight));
						centerPanel.Add(data[i]);
						data[i].SetYCoord(i);
						data[i].SetOriginalValue(data[i].GetBinValue());
						// show locked cell
						if (tempLock)
						{
							data[i].SetForeground(Color.GRAY);
						}
					}
				}
			}
			// reset locked status
			locked = tempLock;
		}

		public virtual int GetType()
		{
			return type;
		}

		public virtual DataCell GetDataCell(int location)
		{
			return data[location];
		}

		public virtual void SetType(int type)
		{
			this.type = type;
		}

		public override string GetName()
		{
			return name;
		}

		public override void SetName(string name)
		{
			this.name = name;
		}

		public virtual string GetCategory()
		{
			return category;
		}

		public virtual void SetCategory(string category)
		{
			this.category = category;
		}

		public virtual string GetDescription()
		{
			return description;
		}

		public virtual void SetDescription(string description)
		{
			this.description = description;
		}

		public virtual Scale GetScale()
		{
			return scales[scaleIndex];
		}

		public virtual Vector<Scale> GetScales()
		{
			return scales;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual Scale GetScaleByName(string inputName)
		{
			// look for scale, else throw exception
			foreach (Scale scale in scales)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(scale.GetName(), inputName))
				{
					return scale;
				}
			}
			throw new Exception();
		}

		public virtual void SetScale(Scale scale)
		{
			// look for scale, replace or add new
			for (int i = 0; i < scales.Count; i++)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(scales[i].GetName(), scale.GetName()))
				{
					scales.Remove(i);
					break;
				}
			}
			scales.AddItem(scale);
		}

		public virtual int GetStorageAddress()
		{
			return storageAddress;
		}

		public virtual void SetStorageAddress(int storageAddress)
		{
			this.storageAddress = storageAddress;
		}

		public virtual int GetStorageType()
		{
			return storageType;
		}

		public virtual void SetStorageType(int storageType)
		{
			this.storageType = storageType;
		}

		public virtual bool IsSignedData()
		{
			return signed;
		}

		public virtual void SetSignedData(bool signed)
		{
			this.signed = signed;
		}

		public virtual int GetEndian()
		{
			return endian;
		}

		public virtual void SetEndian(int endian)
		{
			this.endian = endian;
		}

		public virtual void SetDataSize(int size)
		{
			data = new DataCell[size];
		}

		public virtual int GetDataSize()
		{
			return data.Length;
		}

		public virtual bool GetFlip()
		{
			return flip;
		}

		public virtual void SetFlip(bool flip)
		{
			this.flip = flip;
		}

		public virtual void SetLogParam(string logParam)
		{
			this.logParam = logParam;
		}

		public virtual string GetLogParam()
		{
			return logParam;
		}

		public override string ToString()
		{
			return name;
		}

		public virtual bool IsStatic()
		{
			return isStatic;
		}

		public virtual void SetIsStatic(bool isStatic)
		{
			this.isStatic = isStatic;
		}

		public virtual void AddStaticDataCell(DataCell input)
		{
			if (isStatic)
			{
				for (int i = 0; i < data.Length; i++)
				{
					if (data[i] == null)
					{
						data[i] = input;
						break;
					}
				}
			}
		}

		public virtual void Colorize()
		{
			if (compareDisplay == COMPARE_DISPLAY_OFF)
			{
				if (!isStatic && !isAxis)
				{
					double high = double.MinValue;
					double low = double.MaxValue;
					if (GetScale().GetMax() != 0 || GetScale().GetMin() != 0)
					{
						// set min and max values if they are set in scale
						high = GetScale().GetMax();
						low = GetScale().GetMin();
					}
					else
					{
						for (int i = 0; i < GetDataSize(); i++)
						{
							double value = data[i].GetValue();
							if (value > high)
							{
								high = value;
							}
							if (value < low)
							{
								low = value;
							}
						}
					}
					for (int i_1 = 0; i_1 < GetDataSize(); i_1++)
					{
						double value = data[i_1].GetValue();
						if (value > high || value < low)
						{
							// value exceeds limit
							data[i_1].SetColor(GetEditor().GetSettings().GetWarningColor());
						}
						else
						{
							// limits not set, scale based on table values
							double scale;
							if (high - low == 0)
							{
								// if all values are the same, color will be middle value
								scale = .5;
							}
							else
							{
								scale = (value - low) / (high - low);
							}
							data[i_1].SetColor(ColorScaler.GetScaledColor(scale, GetEditor().GetSettings()));
						}
					}
				}
				else
				{
					// is static/axis
					for (int i = 0; i < GetDataSize(); i++)
					{
						data[i].SetColor(GetEditor().GetSettings().GetAxisColor());
						data[i].SetOpaque(true);
						data[i].SetBorder(BorderFactory.CreateLineBorder(Color.BLACK, 1));
						data[i].SetHorizontalAlignment(DataCell.CENTER);
					}
				}
			}
			else
			{
				// comparing is on
				if (!isStatic)
				{
					double high = double.MinValue;
					// determine ratios
					for (int i = 0; i < GetDataSize(); i++)
					{
						if (Math.Abs(data[i].GetBinValue() - data[i].GetCompareValue()) > high)
						{
							high = Math.Abs(data[i].GetBinValue() - data[i].GetCompareValue());
						}
					}
					// colorize
					for (int i_1 = 0; i_1 < GetDataSize(); i_1++)
					{
						double cellDifference = Math.Abs(data[i_1].GetBinValue() - data[i_1].GetCompareValue
							());
						double scale;
						if (high == 0)
						{
							scale = 0;
						}
						else
						{
							scale = cellDifference / high;
						}
						if (scale == 0)
						{
							data[i_1].SetColor(UNCHANGED_VALUE_COLOR);
						}
						else
						{
							data[i_1].SetColor(ColorScaler.GetScaledColor(scale, GetEditor().GetSettings()));
						}
						// set border
						if (data[i_1].GetBinValue() > data[i_1].GetCompareValue())
						{
							data[i_1].SetBorder(BorderFactory.CreateLineBorder(GetEditor().GetSettings().GetIncreaseBorder
								()));
						}
						else
						{
							if (data[i_1].GetBinValue() < data[i_1].GetCompareValue())
							{
								data[i_1].SetBorder(BorderFactory.CreateLineBorder(GetEditor().GetSettings().GetDecreaseBorder
									()));
							}
							else
							{
								data[i_1].SetBorder(BorderFactory.CreateLineBorder(Color.BLACK, 1));
							}
						}
					}
				}
			}
			// colorize border
			for (int i_2 = 0; i_2 < GetDataSize(); i_2++)
			{
				double checkValue;
				if (compareDisplay == COMPARE_DISPLAY_OFF)
				{
					checkValue = data[i_2].GetOriginalValue();
				}
				else
				{
					checkValue = data[i_2].GetCompareValue();
				}
				if (checkValue > data[i_2].GetBinValue())
				{
					data[i_2].SetBorder(BorderFactory.CreateLineBorder(GetEditor().GetSettings().GetIncreaseBorder
						()));
				}
				else
				{
					if (checkValue < data[i_2].GetBinValue())
					{
						data[i_2].SetBorder(BorderFactory.CreateLineBorder(GetEditor().GetSettings().GetDecreaseBorder
							()));
					}
					else
					{
						data[i_2].SetBorder(BorderFactory.CreateLineBorder(Color.BLACK, 1));
					}
				}
			}
		}

		public virtual void SetFrame(TableFrame frame)
		{
			this.frame = frame;
			//frame.setSize(getFrameSize());
			frame.Pack();
		}

		public virtual Dimension GetFrameSize()
		{
			int height = verticalOverhead + cellHeight;
			int width = horizontalOverhead + data.Length * cellWidth;
			if (height < minHeight)
			{
				height = minHeight;
			}
			int minWidth = IsLiveDataSupported() ? minWidthOverlay : minWidthNoOverlay;
			if (width < minWidth)
			{
				width = minWidth;
			}
			return new Dimension(width, height);
		}

		public virtual TableFrame GetFrame()
		{
			return frame;
		}

		public virtual void Increment(double increment)
		{
			if (!isStatic && !locked && !(userLevel > GetEditor().GetSettings().GetUserLevel(
				)))
			{
				foreach (DataCell cell in data)
				{
					if (cell.IsSelected())
					{
						cell.Increment(increment);
					}
				}
				Colorize();
			}
			else
			{
				if (userLevel > GetEditor().GetSettings().GetUserLevel())
				{
					JOptionPane.ShowMessageDialog(this, "This table can only be modified by users with a userlevel of \n"
						 + userLevel + " or greater. Click View->User Level to change your userlevel.", 
						"Table cannot be modified", JOptionPane.INFORMATION_MESSAGE);
				}
			}
		}

		public virtual void Multiply(double factor)
		{
			if (!isStatic && !locked && !(userLevel > GetEditor().GetSettings().GetUserLevel(
				)))
			{
				foreach (DataCell cell in data)
				{
					if (cell.IsSelected())
					{
						cell.Multiply(factor);
					}
				}
			}
			else
			{
				if (userLevel > GetEditor().GetSettings().GetUserLevel())
				{
					JOptionPane.ShowMessageDialog(this, "This table can only be modified by users with a userlevel of \n"
						 + userLevel + " or greater. Click View->User Level to change your userlevel.", 
						"Table cannot be modified", JOptionPane.INFORMATION_MESSAGE);
				}
			}
			Colorize();
		}

		public virtual void SetRealValue(string realValue)
		{
			if (!isStatic && !locked && !(userLevel > GetEditor().GetSettings().GetUserLevel(
				)))
			{
				foreach (DataCell cell in data)
				{
					if (cell.IsSelected())
					{
						cell.SetRealValue(realValue);
					}
				}
			}
			else
			{
				if (userLevel > GetEditor().GetSettings().GetUserLevel())
				{
					JOptionPane.ShowMessageDialog(this, "This table can only be modified by users with a userlevel of \n"
						 + userLevel + " or greater. Click View->User Level to change your userlevel.", 
						"Table cannot be modified", JOptionPane.INFORMATION_MESSAGE);
				}
			}
			Colorize();
		}

		public virtual Rom GetRom()
		{
			return container;
		}

		public virtual void SetRom(Rom container)
		{
			this.container = container;
		}

		public virtual void ClearSelection()
		{
			foreach (DataCell cell in data)
			{
				cell.SetSelected(false);
			}
		}

		public virtual void StartHighlight(int x, int y)
		{
			this.highlightY = y;
			this.highlightX = x;
			highlight = true;
			Highlight(x, y);
		}

		public virtual void Highlight(int x, int y)
		{
			if (highlight)
			{
				for (int i = 0; i < data.Length; i++)
				{
					if ((i >= highlightY && i <= y) || (i <= highlightY && i >= y))
					{
						data[i].SetHighlighted(true);
					}
					else
					{
						data[i].SetHighlighted(false);
					}
				}
			}
		}

		public virtual void StopHighlight()
		{
			highlight = false;
			// loop through, selected and un-highlight
			foreach (DataCell cell in data)
			{
				if (cell.IsHighlighted())
				{
					cell.SetSelected(true);
					cell.SetHighlighted(false);
				}
			}
		}

		public abstract void CursorUp();

		public abstract void CursorDown();

		public abstract void CursorLeft();

		public abstract void CursorRight();

		public virtual RomRaider.Maps.Table GetAxisParent()
		{
			return axisParent;
		}

		public virtual void SetAxisParent(RomRaider.Maps.Table axisParent)
		{
			this.axisParent = axisParent;
		}

		public virtual void SetRevertPoint()
		{
			if (!isStatic)
			{
				foreach (DataCell cell in data)
				{
					cell.SetOriginalValue(cell.GetBinValue());
				}
			}
			Colorize();
		}

		public virtual void UndoAll()
		{
			ClearLiveDataTrace();
			if (!isStatic)
			{
				foreach (DataCell cell in data)
				{
					if (cell.GetBinValue() != cell.GetOriginalValue())
					{
						cell.SetBinValue(cell.GetOriginalValue());
					}
				}
			}
			Colorize();
		}

		public virtual void UndoSelected()
		{
			ClearLiveDataTrace();
			if (!isStatic)
			{
				foreach (DataCell cell in data)
				{
					// reset current value to original value
					if (cell.IsSelected())
					{
						if (cell.GetBinValue() != cell.GetOriginalValue())
						{
							cell.SetBinValue(cell.GetOriginalValue());
						}
					}
				}
			}
			Colorize();
		}

		public virtual byte[] SaveFile(byte[] binData)
		{
			if (!isStatic && userLevel <= GetEditor().GetSettings().GetUserLevel() && (userLevel
				 < 5 || GetEditor().GetSettings().IsSaveDebugTables()))
			{
				// save if table is not static
				// and user level is great enough
				// and table is not in debug mode, unless saveDebugTables is true
				for (int i = 0; i < data.Length; i++)
				{
					// determine output byte values
					byte[] output;
					if (storageType != STORAGE_TYPE_FLOAT)
					{
						// convert byte values
						output = RomAttributeParser.ParseIntegerValue((int)data[i].GetBinValue(), endian, 
							storageType);
						for (int z = 0; z < storageType; z++)
						{
							// insert into file
							binData[i * storageType + z + storageAddress - ramOffset] = output[z];
						}
					}
					else
					{
						// float
						// convert byte values
						output = RomAttributeParser.FloatToByte((float)data[i].GetBinValue(), endian);
						for (int z = 0; z < 4; z++)
						{
							// insert in to file
							binData[i * 4 + z + storageAddress - ramOffset] = output[z];
						}
					}
				}
			}
			return binData;
		}

		public virtual bool IsBeforeRam()
		{
			return beforeRam;
		}

		public virtual void SetBeforeRam(bool beforeRam)
		{
			this.beforeRam = beforeRam;
		}

		public override void AddKeyListener(KeyListener listener)
		{
			base.AddKeyListener(listener);
			foreach (DataCell cell in data)
			{
				for (int z = 0; z < storageType; z++)
				{
					cell.AddKeyListener(listener);
				}
			}
		}

		public virtual void SelectCellAt(int y)
		{
			if (y >= 0 && y < data.Length)
			{
				if (type == TABLE_X_AXIS || type == TABLE_Y_AXIS)
				{
					axisParent.ClearSelection();
				}
				else
				{
					ClearSelection();
				}
				data[y].SetSelected(true);
				highlightY = y;
			}
		}

		public virtual void CopySelection()
		{
			Window ancestorWindow = SwingUtilities.GetWindowAncestor(this);
			if (null != ancestorWindow)
			{
				ancestorWindow.SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			}
			GetEditor().SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			copySelectionWorker = new CopySelectionWorker(this);
			copySelectionWorker.Execute();
		}

		public virtual StringBuilder GetTableAsString()
		{
			//make a string of the selection
			StringBuilder output = new StringBuilder(BLANK);
			for (int i = 0; i < data.Length; i++)
			{
				output.Append(data[i].GetText());
				if (i < data.Length - 1)
				{
					output.Append(TAB);
				}
			}
			return output;
		}

		public virtual void CopyTable()
		{
			Window ancestorWindow = SwingUtilities.GetWindowAncestor(this);
			if (null != ancestorWindow)
			{
				ancestorWindow.SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			}
			GetEditor().SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			copyTableWorker = new CopyTableWorker(GetEditor().GetSettings(), this);
			copyTableWorker.Execute();
		}

		public virtual string GetCellAsString(int index)
		{
			return data[index].GetText();
		}

		public virtual void PasteValues(string[] input)
		{
			//set real values
			for (int i = 0; i < input.Length; i++)
			{
				try
				{
					double.ParseDouble(input[i]);
					data[i].SetRealValue(input[i]);
				}
				catch (FormatException)
				{
				}
			}
		}

		public virtual void Paste()
		{
			StringTokenizer st = new StringTokenizer(BLANK);
			try
			{
				string input = (string)Toolkit.GetDefaultToolkit().GetSystemClipboard().GetContents
					(null).GetTransferData(DataFlavor.stringFlavor);
				st = new StringTokenizer(input);
			}
			catch (UnsupportedFlavorException)
			{
			}
			catch (IOException)
			{
			}
			string pasteType = st.NextToken();
			if (Sharpen.Runtime.EqualsIgnoreCase("[Table1D]", pasteType))
			{
				// copied entire table
				int i = 0;
				while (st.HasMoreTokens())
				{
					string currentToken = st.NextToken();
					try
					{
						if (!Sharpen.Runtime.EqualsIgnoreCase(data[i].GetText(), currentToken))
						{
							data[i].SetRealValue(currentToken);
						}
					}
					catch (IndexOutOfRangeException)
					{
					}
					i++;
				}
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase("[Selection1D]", pasteType))
				{
					// copied selection
					if (data[highlightY].IsSelected())
					{
						int i = 0;
						while (st.HasMoreTokens())
						{
							try
							{
								data[highlightY + i].SetRealValue(st.NextToken());
							}
							catch (IndexOutOfRangeException)
							{
							}
							i++;
						}
					}
				}
			}
			Colorize();
		}

		public virtual void ApplyColorSettings()
		{
			if (this.GetType() != TABLE_SWITCH)
			{
				// apply settings to cells
				for (int i = 0; i < GetDataSize(); i++)
				{
					this.SetMaxColor(editor.GetSettings().GetMaxColor());
					this.SetMinColor(editor.GetSettings().GetMinColor());
					data[i].SetHighlightColor(editor.GetSettings().GetHighlightColor());
					data[i].SetIncreaseBorder(editor.GetSettings().GetIncreaseBorder());
					data[i].SetDecreaseBorder(editor.GetSettings().GetDecreaseBorder());
					data[i].SetFont(editor.GetSettings().GetTableFont());
					data[i].Repaint();
				}
				cellHeight = (int)editor.GetSettings().GetCellSize().GetHeight();
				cellWidth = (int)editor.GetSettings().GetCellSize().GetWidth();
				Colorize();
				ValidateScaling();
			}
		}

		public virtual void Resize()
		{
			//frame.setSize(getFrameSize());
			frame.Pack();
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

		public abstract void SetAxisColor(Color color);

		public virtual void ValidateScaling()
		{
			if (type != RomRaider.Maps.Table.TABLE_SWITCH && !isStatic)
			{
				// make sure a scale is present
				if (scales.IsEmpty())
				{
					scales.AddItem(new Scale());
				}
				double startValue = 5;
				double toReal = JEPUtil.Evaluate(scales[scaleIndex].GetExpression(), startValue);
				// convert real world value of "5"
				double endValue = JEPUtil.Evaluate(scales[scaleIndex].GetByteExpression(), toReal
					);
				// if real to byte doesn't equal 5, report conflict
				if (Math.Abs(endValue - startValue) > .001)
				{
					JPanel panel = new JPanel();
					panel.SetLayout(new GridLayout(4, 1));
					panel.Add(new JLabel("The real value and byte value conversion expressions for table "
						 + name + " are invalid."));
					panel.Add(new JLabel("To real value: " + scales[scaleIndex].GetExpression()));
					panel.Add(new JLabel("To byte: " + scales[scaleIndex].GetByteExpression()));
					JCheckBox check = new JCheckBox("Always display this message", true);
					check.SetHorizontalAlignment(JCheckBox.RIGHT);
					panel.Add(check);
					check.AddActionListener(new _ActionListener_1142(this));
					JOptionPane.ShowMessageDialog(editor, panel, "Warning", JOptionPane.ERROR_MESSAGE
						);
				}
			}
		}

		private sealed class _ActionListener_1142 : ActionListener
		{
			public _ActionListener_1142(Table _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent e)
			{
				this._enclosing.GetEditor().GetSettings().SetCalcConflictWarning(((JCheckBox)e.GetSource
					()).IsSelected());
			}

			private readonly Table _enclosing;
		}

		public virtual bool FillCompareValues()
		{
			if (null == compareTable)
			{
				return false;
			}
			DataCell[] compareData = compareTable.GetData();
			if (data.Length != compareData.Length)
			{
				return false;
			}
			ClearLiveDataTrace();
			int i = 0;
			foreach (DataCell cell in data)
			{
				if (compareType == COMPARE_TYPE_BIN)
				{
					cell.SetCompareValue(compareData[i].GetBinValue());
				}
				else
				{
					cell.SetCompareValue(compareData[i].GetOriginalValue());
				}
				i++;
			}
			return true;
		}

		public virtual void SetCompareDisplay(int compareDisplay)
		{
			this.compareDisplay = compareDisplay;
		}

		public virtual int GetCompareDisplay()
		{
			return this.compareDisplay;
		}

		public virtual void RefreshCellDisplay()
		{
			foreach (DataCell cell in data)
			{
				cell.SetCompareDisplay(compareDisplay);
				cell.UpdateDisplayValue();
			}
			Colorize();
		}

		public virtual int GetUserLevel()
		{
			return userLevel;
		}

		public virtual void SetUserLevel(int userLevel)
		{
			this.userLevel = userLevel;
			if (userLevel > 5)
			{
				userLevel = 5;
			}
			else
			{
				if (userLevel < 1)
				{
					userLevel = 1;
				}
			}
		}

		public virtual int GetScaleIndex()
		{
			return scaleIndex;
		}

		public virtual void SetScaleIndex(int scaleIndex)
		{
			this.scaleIndex = scaleIndex;
			RefreshValues();
		}

		public virtual void SetScaleByName(string scaleName)
		{
			for (int i = 0; i < scales.Count; i++)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(scales[i].GetName(), scaleName))
				{
					SetScaleIndex(i);
				}
			}
		}

		public virtual void RefreshValues()
		{
			if (!isStatic)
			{
				for (int i = 0; i < GetDataSize(); i++)
				{
					data[i].RefreshValue();
				}
			}
		}

		public virtual void SetEditor(ECUEditor editor)
		{
			this.editor = editor;
		}

		public virtual ECUEditor GetEditor()
		{
			return this.editor;
		}

		public virtual bool IsLocked()
		{
			return locked;
		}

		public virtual void SetLocked(bool locked)
		{
			this.locked = locked;
		}

		public virtual void SetOverlayLog(bool overlayLog)
		{
			this.overlayLog = overlayLog;
			if (overlayLog)
			{
				ClearLiveDataTrace();
			}
		}

		public virtual bool GetOverlayLog()
		{
			return this.overlayLog;
		}

		public virtual void SetLiveValue(string liveValue)
		{
			this.liveValue = liveValue;
		}

		public virtual double GetLiveValue()
		{
			try
			{
				return double.ParseDouble(liveValue);
			}
			catch (FormatException)
			{
				return 0.0;
			}
		}

		public abstract bool IsLiveDataSupported();

		public abstract bool IsButtonSelected();

		protected internal virtual void HighlightLiveData()
		{
		}

		public virtual void ClearLiveDataTrace()
		{
			liveValue = BLANK;
		}

		public virtual double GetMin()
		{
			if (GetScale().GetMin() == 0 && GetScale().GetMax() == 0)
			{
				double low = double.MaxValue;
				for (int i = 0; i < GetDataSize(); i++)
				{
					double value = data[i].GetValue();
					if (value < low)
					{
						low = value;
					}
				}
				return low;
			}
			else
			{
				return GetScale().GetMin();
			}
		}

		public virtual double GetMax()
		{
			if (GetScale().GetMin() == 0 && GetScale().GetMax() == 0)
			{
				double high = double.MinValue;
				for (int i = 0; i < GetDataSize(); i++)
				{
					double value = data[i].GetValue();
					if (value > high)
					{
						high = value;
					}
				}
				return high;
			}
			else
			{
				return GetScale().GetMax();
			}
		}

		public virtual bool GetIsStatic()
		{
			return this.isStatic;
		}

		public virtual bool GetIsAxis()
		{
			return this.isAxis;
		}

		public virtual int GetCompareType()
		{
			return this.compareType;
		}

		public virtual void SetCompareType(int compareType)
		{
			this.compareType = compareType;
		}

		public virtual void SetCompareTable(RomRaider.Maps.Table compareTable)
		{
			this.compareTable = compareTable;
		}

		public virtual IList<RomRaider.Maps.Table> GetComparedToTables()
		{
			return this.comparedToTables;
		}

		public virtual void AddComparedToTable(RomRaider.Maps.Table table)
		{
			if (!table.Equals(this) && !this.GetComparedToTables().Contains(table))
			{
				comparedToTables.AddItem(table);
			}
		}

		public virtual void RefreshCompares()
		{
			if (null == comparedToTables || comparedToTables.Count < 1)
			{
				return;
			}
			foreach (RomRaider.Maps.Table table in comparedToTables)
			{
				if (null != table)
				{
					if (table.FillCompareValues())
					{
						table.RefreshCellDisplay();
					}
				}
			}
		}
	}

	internal class CopySelectionWorker : SwingWorker<Void, Void>
	{
		internal Table table;

		public CopySelectionWorker(Table table)
		{
			this.table = table;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			// find bounds of selection
			// coords[0] = x min, y min, x max, y max
			string newline = Runtime.GetProperty("line.separator");
			string output = "[Selection1D]" + newline;
			bool copy = false;
			int[] coords = new int[2];
			coords[0] = table.GetDataSize();
			for (int i = 0; i < table.GetDataSize(); i++)
			{
				if (table.GetData()[i].IsSelected())
				{
					if (i < coords[0])
					{
						coords[0] = i;
						copy = true;
					}
					if (i > coords[1])
					{
						coords[1] = i;
						copy = true;
					}
				}
			}
			//make a string of the selection
			for (int i_1 = coords[0]; i_1 <= coords[1]; i_1++)
			{
				if (table.GetData()[i_1].IsSelected())
				{
					output = output + table.GetData()[i_1].GetText();
				}
				else
				{
					output = output + "x";
				}
				// x represents non-selected cell
				if (i_1 < coords[1])
				{
					output = output + "\t";
				}
			}
			//copy to clipboard
			if (copy)
			{
				Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
					output), null);
			}
			return null;
		}

		protected override void Done()
		{
			Window ancestorWindow = SwingUtilities.GetWindowAncestor(table);
			if (null != ancestorWindow)
			{
				ancestorWindow.SetCursor(null);
			}
			table.SetCursor(null);
			table.GetEditor().SetCursor(null);
		}
	}

	internal class CopyTableWorker : SwingWorker<Void, Void>
	{
		internal Settings settings;

		internal Table table;

		public CopyTableWorker(Settings settings, Table table)
		{
			this.settings = settings;
			this.table = table;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			string tableHeader = settings.GetTableHeader();
			StringBuilder output = new StringBuilder(tableHeader);
			for (int i = 0; i < table.GetDataSize(); i++)
			{
				output.Append(table.GetData()[i].GetText());
				if (i < table.GetDataSize() - 1)
				{
					output.Append(Table.TAB);
				}
			}
			Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
				output.ToString()), null);
			return null;
		}

		protected override void Done()
		{
			Window ancestorWindow = SwingUtilities.GetWindowAncestor(table);
			if (null != ancestorWindow)
			{
				ancestorWindow.SetCursor(null);
			}
			table.SetCursor(null);
			table.GetEditor().SetCursor(null);
		}
	}
}
