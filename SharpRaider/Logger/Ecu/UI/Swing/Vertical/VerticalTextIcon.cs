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
using Java.Beans;
using Javax.Swing;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Swing.Vertical
{
	/// <summary>VTextIcon is an Icon implementation which draws a short string vertically.
	/// 	</summary>
	/// <remarks>
	/// VTextIcon is an Icon implementation which draws a short string vertically.
	/// It's useful for JTabbedPanes with LEFT or RIGHT tabs but can be used in any
	/// component which supports Icons, such as JLabel or JButton
	/// You can provide a hint to indicate whether to rotate the string
	/// to the left or right, or not at all, and it checks to make sure
	/// that the rotation is legal for the given string
	/// (for example, Chinese/Japanese/Korean scripts have special rules when
	/// drawn vertically and should never be rotated)
	/// </remarks>
	public sealed class VerticalTextIcon : Icon, PropertyChangeListener
	{
		internal string fLabel;

		internal string[] fCharStrings;

		internal int[] fCharWidths;

		internal int[] fPosition;

		internal int fWidth;

		internal int fHeight;

		internal int fCharHeight;

		internal int fDescent;

		internal int fRotation;

		internal Component fComponent;

		internal const int POSITION_NORMAL = 0;

		internal const int POSITION_TOP_RIGHT = 1;

		internal const int POSITION_FAR_TOP_RIGHT = 2;

		public const int ROTATE_DEFAULT = unchecked((int)(0x00));

		public const int ROTATE_NONE = unchecked((int)(0x01));

		public const int ROTATE_LEFT = unchecked((int)(0x02));

		public const int ROTATE_RIGHT = unchecked((int)(0x04));

		/// <summary>
		/// Creates a <code>VTextIcon</code> for the specified <code>component</code>
		/// with the specified <code>label</code>.
		/// </summary>
		/// <remarks>
		/// Creates a <code>VTextIcon</code> for the specified <code>component</code>
		/// with the specified <code>label</code>.
		/// It sets the orientation to the default for the string
		/// </remarks>
		/// <seealso cref="VerifyRotation(string, int)">VerifyRotation(string, int)</seealso>
		public VerticalTextIcon(Component component, string label) : this(component, label
			, ROTATE_DEFAULT)
		{
		}

		/// <summary>
		/// Creates a <code>VTextIcon</code> for the specified <code>component</code>
		/// with the specified <code>label</code>.
		/// </summary>
		/// <remarks>
		/// Creates a <code>VTextIcon</code> for the specified <code>component</code>
		/// with the specified <code>label</code>.
		/// It sets the orientation to the provided value if it's legal for the string
		/// </remarks>
		/// <seealso cref="VerifyRotation(string, int)">VerifyRotation(string, int)</seealso>
		public VerticalTextIcon(Component component, string label, int rotateHint)
		{
			// for efficiency, break the fLabel into one-char strings to be passed to drawString
			// Roman characters should be centered when not rotated (Japanese fonts are monospaced)
			// Japanese half-height characters need to be shifted when drawn vertically
			// Cached for speed
			fComponent = component;
			fLabel = label;
			fRotation = VerifyRotation(label, rotateHint);
			CalcDimensions();
			fComponent.AddPropertyChangeListener(this);
		}

		/// <summary>
		/// sets the label to the given string, updating the orientation as needed
		/// and invalidating the layout if the size changes
		/// </summary>
		/// <seealso cref="VerifyRotation(string, int)">VerifyRotation(string, int)</seealso>
		public void SetLabel(string label)
		{
			fLabel = label;
			fRotation = VerifyRotation(label, fRotation);
			// Make sure the current rotation is still legal
			RecalcDimensions();
		}

		/// <summary>
		/// Checks for changes to the font on the fComponent
		/// so that it can invalidate the layout if the size changes
		/// </summary>
		public void PropertyChange(PropertyChangeEvent e)
		{
			string prop = e.GetPropertyName();
			if ("font".Equals(prop))
			{
				RecalcDimensions();
			}
		}

		/// <summary>Calculates the dimensions.</summary>
		/// <remarks>
		/// Calculates the dimensions.  If they've changed,
		/// invalidates the component
		/// </remarks>
		internal void RecalcDimensions()
		{
			int wOld = GetIconWidth();
			int hOld = GetIconHeight();
			CalcDimensions();
			if (wOld != GetIconWidth() || hOld != GetIconHeight())
			{
				fComponent.Invalidate();
			}
		}

		internal void CalcDimensions()
		{
			FontMetrics fm = fComponent.GetFontMetrics(fComponent.GetFont());
			fCharHeight = fm.GetAscent() + fm.GetDescent();
			fDescent = fm.GetDescent();
			if (fRotation == ROTATE_NONE)
			{
				int len = fLabel.Length;
				char[] data = new char[len];
				Sharpen.Runtime.GetCharsForString(fLabel, 0, len, data, 0);
				// if not rotated, width is that of the widest char in the string
				fWidth = 0;
				// we need an array of one-char strings for drawString
				fCharStrings = new string[len];
				fCharWidths = new int[len];
				fPosition = new int[len];
				char ch;
				for (int i = 0; i < len; i++)
				{
					ch = data[i];
					fCharWidths[i] = fm.CharWidth(ch);
					if (fCharWidths[i] > fWidth)
					{
						fWidth = fCharWidths[i];
					}
					fCharStrings[i] = new string(data, i, 1);
					// small kana and punctuation
					if (sDrawsInTopRight.IndexOf(ch) >= 0)
					{
						// if ch is in sDrawsInTopRight
						fPosition[i] = POSITION_TOP_RIGHT;
					}
					else
					{
						if (sDrawsInFarTopRight.IndexOf(ch) >= 0)
						{
							fPosition[i] = POSITION_FAR_TOP_RIGHT;
						}
						else
						{
							fPosition[i] = POSITION_NORMAL;
						}
					}
				}
				// and height is the font height * the char count, + one extra leading at the bottom
				fHeight = fCharHeight * len + fDescent;
			}
			else
			{
				// if rotated, width is the height of the string
				fWidth = fCharHeight;
				// and height is the width, plus some buffer space
				fHeight = fm.StringWidth(fLabel) + 2 * kBufferSpace;
			}
		}

		/// <summary>Draw the icon at the specified location.</summary>
		/// <remarks>
		/// Draw the icon at the specified location.  Icon implementations
		/// may use the Component argument to get properties useful for
		/// painting, e.g. the foreground or background color.
		/// </remarks>
		public void PaintIcon(Component c, Graphics g, int x, int y)
		{
			// We don't insist that it be on the same Component
			g.SetColor(c.GetForeground());
			g.SetFont(c.GetFont());
			if (fRotation == ROTATE_NONE)
			{
				int yPos = y + fCharHeight;
				for (int i = 0; i < fCharStrings.Length; i++)
				{
					// Special rules for Japanese - "half-height" characters (like ya, yu, yo in combinations)
					// should draw in the top-right quadrant when drawn vertically
					// - they draw in the bottom-left normally
					int tweak;
					switch (fPosition[i])
					{
						case POSITION_NORMAL:
						{
							// Roman fonts should be centered. Japanese fonts are always monospaced.
							g.DrawString(fCharStrings[i], x + ((fWidth - fCharWidths[i]) / 2), yPos);
							break;
						}

						case POSITION_TOP_RIGHT:
						{
							tweak = fCharHeight / 3;
							// Should be 2, but they aren't actually half-height
							g.DrawString(fCharStrings[i], x + (tweak / 2), yPos - tweak);
							break;
						}

						case POSITION_FAR_TOP_RIGHT:
						{
							tweak = fCharHeight - fCharHeight / 3;
							g.DrawString(fCharStrings[i], x + (tweak / 2), yPos - tweak);
							break;
						}
					}
					yPos += fCharHeight;
				}
			}
			else
			{
				if (fRotation == ROTATE_LEFT)
				{
					g.Translate(x + fWidth, y + fHeight);
					((Graphics2D)g).Rotate(-NINETY_DEGREES);
					g.DrawString(fLabel, kBufferSpace, -fDescent);
					((Graphics2D)g).Rotate(NINETY_DEGREES);
					g.Translate(-(x + fWidth), -(y + fHeight));
				}
				else
				{
					if (fRotation == ROTATE_RIGHT)
					{
						g.Translate(x, y);
						((Graphics2D)g).Rotate(NINETY_DEGREES);
						g.DrawString(fLabel, kBufferSpace, -fDescent);
						((Graphics2D)g).Rotate(-NINETY_DEGREES);
						g.Translate(-x, -y);
					}
				}
			}
		}

		/// <summary>Returns the icon's width.</summary>
		/// <remarks>Returns the icon's width.</remarks>
		/// <returns>an int specifying the fixed width of the icon.</returns>
		public int GetIconWidth()
		{
			return fWidth;
		}

		/// <summary>Returns the icon's height.</summary>
		/// <remarks>Returns the icon's height.</remarks>
		/// <returns>an int specifying the fixed height of the icon.</returns>
		public int GetIconHeight()
		{
			return fHeight;
		}

		/// <summary>
		/// verifyRotation
		/// <p/>
		/// returns the best rotation for the string (ROTATE_NONE, ROTATE_LEFT, ROTATE_RIGHT)
		/// <p/>
		/// This is public static so you can use it to test a string without creating a VTextIcon
		/// <p/>
		/// from http://www.unicode.org/unicode/reports/tr9/tr9-3.html
		/// When setting text using the Arabic script in vertical lines,
		/// it is more common to employ a horizontal baseline that
		/// is rotated by 90ï¿½ counterclockwise so that the characters
		/// are ordered from top to bottom.
		/// </summary>
		/// <remarks>
		/// verifyRotation
		/// <p/>
		/// returns the best rotation for the string (ROTATE_NONE, ROTATE_LEFT, ROTATE_RIGHT)
		/// <p/>
		/// This is public static so you can use it to test a string without creating a VTextIcon
		/// <p/>
		/// from http://www.unicode.org/unicode/reports/tr9/tr9-3.html
		/// When setting text using the Arabic script in vertical lines,
		/// it is more common to employ a horizontal baseline that
		/// is rotated by 90ï¿½ counterclockwise so that the characters
		/// are ordered from top to bottom. Latin text and numbers
		/// may be rotated 90ï¿½ clockwise so that the characters
		/// are also ordered from top to bottom.
		/// <p/>
		/// Rotation rules
		/// - Roman can rotate left, right, or none - default right (counterclockwise)
		/// - CJK can't rotate
		/// - Arabic must rotate - default left (clockwise)
		/// <p/>
		/// from the online edition of _The Unicode Standard, Version 3.0_, file ch10.pdf page 4
		/// Ideographs are found in three blocks of the Unicode Standard...
		/// U+4E00-U+9FFF, U+3400-U+4DFF, U+F900-U+FAFF
		/// <p/>
		/// Hiragana is U+3040-U+309F, katakana is U+30A0-U+30FF
		/// <p/>
		/// from http://www.unicode.org/unicode/faq/writingdirections.html
		/// East Asian scripts are frequently written in vertical lines
		/// which run from top-to-bottom and are arrange columns either
		/// from left-to-right (Mongolian) or right-to-left (other scripts).
		/// Most characters use the same shape and orientation when displayed
		/// horizontally or vertically, but many punctuation characters
		/// will change their shape when displayed vertically.
		/// <p/>
		/// Letters and words from other scripts are generally rotated through
		/// ninety degree angles so that they, too, will read from top to bottom.
		/// That is, letters from left-to-right scripts will be rotated clockwise
		/// and letters from right-to-left scripts counterclockwise, both
		/// through ninety degree angles.
		/// <p/>
		/// Unlike the bidirectional case, the choice of vertical layout
		/// is usually treated as a formatting style; therefore,
		/// the Unicode Standard does not define default rendering behavior
		/// for vertical text nor provide directionality controls designed to override such behavior
		/// </remarks>
		public static int VerifyRotation(string label, int rotateHint)
		{
			bool hasCJK = false;
			bool hasMustRotate = false;
			// Arabic, etc
			int len = label.Length;
			char[] data = new char[len];
			char ch;
			Sharpen.Runtime.GetCharsForString(label, 0, len, data, 0);
			for (int i = 0; i < len; i++)
			{
				ch = data[i];
				if ((ch >= '\u4E00' && ch <= '\u9FFF') || (ch >= '\u3400' && ch <= '\u4DFF') || (
					ch >= '\uF900' && ch <= '\uFAFF') || (ch >= '\u3040' && ch <= '\u309F') || (ch >=
					 '\u30A0' && ch <= '\u30FF'))
				{
					hasCJK = true;
				}
				if ((ch >= '\u0590' && ch <= '\u05FF') || (ch >= '\u0600' && ch <= '\u06FF') || (
					ch >= '\u0700' && ch <= '\u074F'))
				{
					// Hebrew
					// Arabic
					// Syriac
					hasMustRotate = true;
				}
			}
			// If you mix Arabic with Chinese, you're on your own
			if (hasCJK)
			{
				return DEFAULT_CJK;
			}
			int legal = hasMustRotate ? LEGAL_MUST_ROTATE : LEGAL_ROMAN;
			if ((rotateHint & legal) > 0)
			{
				return rotateHint;
			}
			// The hint wasn't legal, or it was zero
			return hasMustRotate ? DEFAULT_MUST_ROTATE : DEFAULT_ROMAN;
		}

		internal static readonly string sDrawsInTopRight = "\u3041\u3043\u3045\u3047\u3049\u3063\u3083\u3085\u3087\u308E"
			 + "\u30A1\u30A3\u30A5\u30A7\u30A9\u30C3\u30E3\u30E5\u30E7\u30EE\u30F5\u30F6";

		internal static readonly string sDrawsInFarTopRight = "\u3001\u3002";

		internal const int DEFAULT_CJK = ROTATE_NONE;

		internal const int LEGAL_ROMAN = ROTATE_NONE | ROTATE_LEFT | ROTATE_RIGHT;

		internal const int DEFAULT_ROMAN = ROTATE_RIGHT;

		internal const int LEGAL_MUST_ROTATE = ROTATE_LEFT | ROTATE_RIGHT;

		internal const int DEFAULT_MUST_ROTATE = ROTATE_LEFT;

		internal static readonly double NINETY_DEGREES = Math.ToRadians(90.0);

		internal const int kBufferSpace = 5;
		// The small kana characters and Japanese punctuation that draw in the top right quadrant:
		// small a, i, u, e, o, tsu, ya, yu, yo, wa  (katakana only) ka ke
		// hiragana
		// katakana
		// comma, full stop
	}
}
