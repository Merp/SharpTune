// IdcScript.cs: Helper class for generating IDC scripts.

/* Copyright (C) 2011 SubaruDieselCrew
 *
 * This file is part of ScoobyRom.
 *
 * ScoobyRom is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ScoobyRom is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with ScoobyRom.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.IO;

namespace IDA
{
	public sealed class IdcScript : IDisposable
	{
		const int IndentStep = 2;

		TextWriter tw;
		int indentCount = 0;
		string indent = string.Empty;

		public IdcScript (TextWriter writer)
		{
			this.tw = writer;

			IncHeader ();
			IncFunc_MakeDataMultiple ();
		}

		public IdcScript (string path) : this(new StreamWriter (path))
		{
		}

		public void Dispose ()
		{
			if (tw != null) {
				tw.Dispose ();
				tw = null;
			}
		}

		void IncHeader ()
		{
			tw.WriteLine ("// Generated IDC file for IDA");
			tw.WriteLine ("#include <idc.idc>");
		}

		public void IndentInc ()
		{
			indentCount += IndentStep;
			indent = new string (' ', indentCount);
		}

		public void IndentDec ()
		{
			indentCount -= IndentStep;
			indent = new string (' ', indentCount);
		}

		void WriteIndent ()
		{
			tw.Write (indent);
		}

		public void WriteLine ()
		{
			tw.WriteLine ();
		}

		public void WriteLine (string l)
		{
			WriteIndent ();
			tw.WriteLine (l);
		}

		public void AddCommentLine (string l)
		{
			WriteIndent ();
			tw.Write ("// ");
			tw.WriteLine (l);
		}

		public void AddFuncDef (string name, string parameters)
		{
			WriteLine (string.Format ("static {0}({1})", name, parameters));
		}

		public void OpeningBrace ()
		{
			WriteLine ("{");
			IndentInc ();
		}

		public void ClosingBrace ()
		{
			IndentDec ();
			WriteLine ("}");
		}

		public void MakeDataMultiple (int addr, int count, int increment, IdcType idcType)
		{
			WriteLine (string.Format ("MakeDataMultiple(0x{0}, {1}, {2}, {3});", addr.ToString ("X"), count.ToString (), increment.ToString (), idcType.ToString ().Replace (", ", " | ")));
		}

		public void IncFunc_MakeDataMultiple ()
		{
			AddCommentLine ("addr: start address");
			AddCommentLine ("count: number of entities (not bytes)");
			AddCommentLine ("increment: size of single data item in bytes");
			AddCommentLine ("type: e.g. FF_BYTE | FF_0NUMD");
			AddFuncDef ("MakeDataMultiple", "addr, count, increment, type");
			OpeningBrace ();

			WriteLine ("auto a;");
			WriteLine ("for (a = addr; a < addr + increment * count; a = a + increment)");
			OpeningBrace ();
			WriteLine ("MakeData(a, type, increment, 0);");
			ClosingBrace ();

			ClosingBrace ();
			WriteLine ();
		}
	}
}
