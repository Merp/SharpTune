// Range.cs: Simple helper struct.

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

namespace ModRom.Tables
{
    public struct Range : IEquatable<Range>
    {
        // 2 fields
        int pos;
        int size;

        /// <summary>
        /// Position of first byte.
        /// </summary>
        public int Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        /// <summary>
        /// Data size in bytes.
        /// </summary>
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Last used byte position.
        /// (= Pos + Size - 1)
        /// </summary>
        public int Last
        {
            get { return pos + size - 1; }
        }

        public Range(int start, int size)
        {
            this.pos = start;
            this.size = size;
        }

        public bool Intersects(Range other)
        {
            if (other.Last < this.pos || other.pos > this.Last)
                return false;
            else
                return true;
        }

        public override string ToString()
        {
            return string.Format("[Pos=0x{0:X}, Size={1}, Last=0x{2:X}]", pos, size.ToString(), Last);
        }

        public override int GetHashCode()
        {
            return this.pos ^ (this.size << 3);
        }


        #region IEquatable<Range> implementation

        public bool Equals(Range other)
        {
            return this.pos == other.pos && this.size == other.size;
        }

        #endregion
    }
}
