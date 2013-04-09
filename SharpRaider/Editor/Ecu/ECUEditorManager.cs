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
using Javax.Swing;
using RomRaider.Editor.Ecu;
using Sharpen;

namespace RomRaider.Editor.Ecu
{
	public class ECUEditorManager
	{
		private static ECUEditor editor = null;

		public ECUEditorManager()
		{
			throw new NotSupportedException();
		}

		public static ECUEditor GetECUEditor()
		{
			if (editor == null)
			{
				try
				{
					SwingUtilities.InvokeAndWait(new _Runnable_35());
				}
				catch (Exception e)
				{
					throw new RuntimeException(e);
				}
			}
			return editor;
		}

		private sealed class _Runnable_35 : Runnable
		{
			public _Runnable_35()
			{
			}

			public void Run()
			{
				RomRaider.Editor.Ecu.ECUEditorManager.editor = new ECUEditor();
			}
		}
	}
}
