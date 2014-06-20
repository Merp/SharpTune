// Table.cs: Table base class, provides common features.

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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Util;
using Extensions;

namespace Subaru.Tables
{
	/// <summary>
	/// Common functionality for 2D and 3D Table types.
	/// </summary>
	public abstract class Table
	{
		// Axis item count restrictions. Count < 2 or high does not make sense.
		// This restriction helps avoiding false positives.
		public const int CountMin = 2;
		public const int CountMax = 255;

		// All 2D- and 3D-Table structs contain pointers. If a pointer points to a rather odd position,
		// the suggested table struct will be discarded. This restriction helps avoiding false positives.
		static protected int posMax = (1024 + 512) * 1024;
		static protected int posMin = 8 * 1024;

		/// <summary>
		/// Gets or sets the pointer maximum, improving table detection.
		/// Set to maximum expected pointer position or file size if in doubt.
		/// </summary>
		/// <value>
		/// The position max.
		/// </value>
		public static int PosMax {
			get {
				return posMax;
			}
			set {
				posMax = value;
			}
		}

		/// <summary>
		/// Gets or sets the pointer minimum, improving table detection.
		/// Set to minimum expected position or 0 if in doubt.
		/// </summary>
		/// <value>
		/// The position minimum.
		/// </value>
		public static int PosMin {
			get {
				return posMin;
			}
			set {
				posMin = value;
			}
		}

		public const int FloatSize = 4;

		// floats like x.xxxxxxxxE-40 etc. suggest these are invalid, not to be included
		public const float FloatMin = (float)1E-12;
		public const float FloatMax = (float)1E+12;
		public static string endian = "big";
		#region Fields

		protected int countX;
		protected TableType tableType;
		protected bool typeUncertain = false;
		protected float multiplier, offset;

		// additional fields
		protected int location;
		protected Range rangeX, rangeY;
		protected bool hasMAC = false;

		// axis are always floats
		protected float[] valuesX;

		// metadata
		protected string title, category, description, nameX, unitX, unitY;

		#endregion Fields

		/// <summary>
		/// Struct position in file, not part of ROM struct content
		/// </summary>
		public int Location {
			get { return location; }
			set { location = value; }
		}

		public int CountX {
			get { return countX; }
			set { countX = value; }
		}

		public float[] ValuesX {
			get { return this.valuesX; }
		}

		// valid object has increasing axis values
		public float Xmin {
			get { return valuesX != null ? valuesX [0] : float.NaN; }
		}

		// valid object has increasing axis values
		public float Xmax {
			get { return valuesX != null ? valuesX [this.valuesX.Length - 1] : float.NaN; }
		}

		/// <summary>
		/// Might be wrong!
		/// </summary>
		public TableType TableType {
			get { return tableType; }
			set { tableType = value; }
		}

		public bool TypeUncertain {
			get { return this.typeUncertain; }
		}

		/// <summary>
		/// If Multiplier and Offset floats are available (valid).
		/// </summary>
		public bool HasMAC {
			get { return this.hasMAC; }
		}

		// these two floats are optional, usually for type non-float but not always
		public float Multiplier {
			get { return multiplier; }
			set { multiplier = value; }
		}

		public float Offset {
			get { return offset; }
			set { offset = value; }
		}

		public Range RangeX {
			get { return this.rangeX; }
			set { rangeX = value; }
		}

		public Range RangeY {
			get { return this.rangeY; }
			set { rangeY = value; }
		}

		public string NameX {
			get { return this.nameX; }
			set { nameX = value; }
		}

		public string Title {
			get { return this.title; }
			set { title = value; }
		}

		public string Category {
			get { return this.category; }
			set { category = value; }
		}

		public string Description {
			get { return this.description; }
			set { description = value; }
		}

		public string UnitX {
			get { return this.unitX; }
			set { unitX = value; }
		}

		public string UnitY {
			get { return this.unitY; }
			set { unitY = value; }
		}

		public virtual void Reset ()
		{
			hasMAC = false;
			typeUncertain = false;
			multiplier = float.NaN;
			offset = float.NaN;
		}

		public abstract bool IsRecordValid ()

;
		public abstract bool ReadValidateValues (System.IO.Stream stream)

;
		public abstract void ChangeTypeToAndReload (TableType newType, System.IO.Stream stream)

;
		public abstract XElement RRXml ();

		public bool HasMetadata {
			get { return !string.IsNullOrEmpty (title) || !string.IsNullOrEmpty (category) || !string.IsNullOrEmpty (description) || !string.IsNullOrEmpty (nameX) || !string.IsNullOrEmpty (unitX); }
		}

		protected static void ThrowInvalidTableType (TableType tableType)
		{
			throw new ArgumentOutOfRangeException ("Invalid TableType: " + tableType.ToString ());
		}

		#region ReadValues

		public static Array ReadValues (System.IO.Stream stream, Range range, TableType tableType)
		{
			switch (tableType) {
			case TableType.Float:
				return ReadValuesFloat (stream, range);
			case TableType.UInt8:
				return ReadValuesUInt8 (stream, range);
			case TableType.UInt16:
				return ReadValuesUInt16 (stream, range);
			case TableType.Int8:
				return ReadValuesInt8 (stream, range);
			case TableType.Int16:
				return ReadValuesInt16 (stream, range);
			default:
				ThrowInvalidTableType (tableType);
				return null;
			}
		}

		// Used for reading axis values (always float) and values (in case of type float)
		public static float[] ReadValuesFloat (System.IO.Stream stream, Range range)
		{
			stream.Seek (range.Pos, System.IO.SeekOrigin.Begin);
			int count = range.Size / FloatSize;
			float[] array = new float[count];

			for (int i = 0; i < array.Length; i++) {
				array [i] = stream.ReadSingleBigEndian ();
			}
			return array;
		}

		public static byte[] ReadValuesUInt8 (System.IO.Stream stream, Range range)
		{
			stream.Seek (range.Pos, System.IO.SeekOrigin.Begin);
			int count = range.Size;
			byte[] buf = new byte[count];
			stream.Read (buf, 0, count);
			return buf;
		}

		public static sbyte[] ReadValuesInt8 (System.IO.Stream stream, Range range)
		{
			stream.Seek (range.Pos, System.IO.SeekOrigin.Begin);
			int count = range.Size;
			byte[] buf = new byte[count];
			stream.Read (buf, 0, count);

			// Array.Copy won't work with different Array types like byte[] and sbyte[]
			sbyte[] array = new sbyte[buf.Length];
			for (int i = 0; i < array.Length; i++) {
				array [i] = (sbyte)buf [i];
			}
			return array;
		}

		public static ushort[] ReadValuesUInt16 (System.IO.Stream stream, Range range)
		{
			stream.Seek (range.Pos, System.IO.SeekOrigin.Begin);
			int count = range.Size / 2;
			ushort[] array = new ushort[count];

			for (int i = 0; i < array.Length; i++) {
				array [i] = (ushort)stream.ReadInt16BigEndian ();
			}
			return array;
		}

		public static short[] ReadValuesInt16 (System.IO.Stream stream, Range range)
		{
			stream.Seek (range.Pos, System.IO.SeekOrigin.Begin);
			int count = range.Size / 2;
			short[] array = new short[count];

			for (int i = 0; i < array.Length; i++) {
				array [i] = stream.ReadInt16BigEndian ();
			}
			return array;
		}

		#endregion ReadValues

		public static bool IsFloatValid (float value)
		{
			if (float.IsNaN (value))
				return false;
			if (value == 0f)
				return true;
			if (value < 0f)
				value = Math.Abs (value);
			return (value >= FloatMin) && (value <= FloatMax);
		}

		/// <summary>
		/// Values must increase steadily (required for ROM interpolation sub to work).
		/// </summary>
		/// <param name="floats">
		/// A <see cref="System.Single[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool CheckAxisArray (float[] floats)
		{
			for (int i = 1; i < floats.Length; i++) {
				// not all valid axis have strictly increasing values!!!
				// Ex: MAF-Sensor has a duplicate point (X[32] == X[33] incl. corresponding Y-values)
				// had to relaxe original condition: if (floats[i - 1] >= floats[i])
				if (floats [i - 1] > floats [i])
					return false;
			}
			return true;
		}

		public static bool CheckFloatArray (float[] floats)
		{
			for (int i = 0; i < floats.Length; i++) {
				if (!IsFloatValid (floats [i]))
					return false;
			}
			return true;
		}

		#region Values as float[]

		protected float[] ValuesAsFloats (Array array)
		{
			switch (tableType) {
			case TableType.Float:
				return ValuesFromTypeFloat (array);
			case TableType.UInt8:
				return ValuesFromTypeUInt8 (array);
			case TableType.UInt16:
				return ValuesFromTypeUInt16 (array);
			case TableType.Int8:
				return ValuesFromTypeInt8 (array);
			case TableType.Int16:
				return ValuesFromTypeInt16 (array);
			default:
				ThrowInvalidTableType (tableType);
				return null;
			}
		}

		protected float[] ValuesFromTypeFloat (Array array)
		{
			float[] srcFloat = (float[])array;
			var floats = new float[srcFloat.Length];
			if (hasMAC) {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = srcFloat [i] * multiplier + offset;
				}
			} else {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = srcFloat [i];
				}
			}
			return floats;
		}

		protected float[] ValuesFromTypeUInt8 (Array array)
		{
			byte[] srcUInt8 = (byte[])array;
			var floats = new float[srcUInt8.Length];
			if (hasMAC) {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = srcUInt8 [i] * multiplier + offset;
				}
			} else {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = srcUInt8 [i];
				}
			}
			return floats;
		}

		protected float[] ValuesFromTypeUInt16 (Array array)
		{
			ushort[] src = (ushort[])array;
			var floats = new float[src.Length];
			if (hasMAC) {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = src [i] * multiplier + offset;
				}
			} else {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = src [i];
				}
			}
			return floats;
		}

		protected float[] ValuesFromTypeInt8 (Array array)
		{
			sbyte[] src = (sbyte[])array;
			var floats = new float[src.Length];
			if (hasMAC) {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = src [i] * multiplier + offset;
				}
			} else {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = src [i];
				}
			}
			return floats;
		}

		protected float[] ValuesFromTypeInt16 (Array array)
		{
			short[] src = (short[])array;
			var floats = new float[src.Length];
			if (hasMAC) {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = src [i] * multiplier + offset;
				}
			} else {
				for (int i = 0; i < floats.Length; i++) {
					floats [i] = src [i];
				}
			}
			return floats;
		}

		#endregion Values as float[]

		public static string HexAddress (int value)
		{
			return "0x" + value.ToString ("X");
		}

		public string Expression {
			get {
				if (!hasMAC || (multiplier == 1f && offset == 0f))
					return "x";
				StringBuilder sb = new StringBuilder ();
				sb.Append ('x');
				if (multiplier != 1f) {
					sb.Append ('*');
					sb.Append (multiplier.ToString (CultureInfo.InvariantCulture));
				}
				if (offset != 0f) {
					if (offset > 0f)
						sb.Append ('+');
					sb.Append (offset.ToString (CultureInfo.InvariantCulture));
				}
				return sb.ToString ();
			}
		}

		public string ExpressionBack {
			// tested: 0.09999999f to double yields 0.0999999940395355
			get {
				if (!hasMAC || (multiplier == 1f && offset == 0f))
					return "x";
				bool needParantheses = multiplier != 1f && offset != 0f;

				StringBuilder sb = new StringBuilder ();
				if (needParantheses)
					sb.Append ('(');

				sb.Append ('x');
				if (offset != 0f) {
					if (offset < 0f)
						sb.Append ('+');
					sb.Append ((-offset).ToString (CultureInfo.InvariantCulture));
				}
				if (needParantheses)
					sb.Append (')');

				if (multiplier != 1f) {
					sb.Append ('/');
					sb.Append (multiplier.ToString (CultureInfo.InvariantCulture));
				}
				return sb.ToString ();
			}
		}

		public static string ValuesStats (float[] values)
		{
			return string.Format (CultureInfo.InvariantCulture, " {0} to {1} ", values.Min (), values.Max ());
		}

		public static string ValuesStats (float min, float max, float avg)
		{
			return string.Format (CultureInfo.InvariantCulture, " min: {0}  max: {1}  average: {2} ", min.ToString (), max.ToString (), avg.ToString ());
		}

		public static XElement RRXmlScaling (string units, string expr, string to_byte, string format, float fineincrement, float coarseincrement)
		{
			return new XElement ("scaling", new XAttribute ("units", units), new XAttribute ("expression", expr), new XAttribute ("to_byte", to_byte), new XAttribute ("format", format), new XAttribute ("fineincrement", fineincrement), new XAttribute ("coarseincrement", coarseincrement));
		}

		public XElement RRXmlAxis (string axisType, string name, string unit, TableType tableType, Range range, float[] axis)
		{
			return new XElement ("table", new XAttribute ("type", axisType), new XAttribute ("name", name), new XAttribute ("storagetype", "float"), new XAttribute ("storageaddress", HexAddress (range.Pos)), new XComment (ValuesStats (axis)), RRXmlScaling (unit, "x", "x", "0.00", 1f, 5f));
		}
	}
}
