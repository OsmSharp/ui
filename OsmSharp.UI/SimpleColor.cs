// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI
{
    /// <summary>
    /// A simple color implementation.
    /// </summary>
    public struct SimpleColor
    {
        /// <summary>
        /// The ARGB value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Alpha.
        /// </summary>
        public short A
        {
            get { return (short)(((uint)this.Value >> 24) % 256); }
        }

        /// <summary>
        /// Red.
        /// </summary>
        public short R
        {
            get
            {
                return (short)(((uint)this.Value >> 16) % 256);
            }
        }

        /// <summary>
        /// Green.
        /// </summary>
        public short G
        {
            get
            {
                return (short)(((uint)this.Value >> 8) % 256);
            }
        }

        /// <summary>
        /// Blue.
        /// </summary>
        public short B
        {
            get
            {
                return (short)((uint)this.Value % 256);
            }
        }

        /// <summary>
        /// Returns the dex description of this color.
        /// </summary>
        public string HexRgb
        {
            get
            {
                return string.Format("#{0}{1}{2}",
                                     this.R.ToString("X2"),
                                     this.G.ToString("X2"),
                                     this.B.ToString("X2"));
            }
        }

		/// <summary>
		/// Creates a new simplecolor from the given argb value.
		/// </summary>
		/// <returns>The ARGB.</returns>
		/// <param name="argb">ARGB.</param>
		public static SimpleColor FromArgb(int argb)
		{
			return new SimpleColor () { Value = argb };
		}

        /// <summary>
        /// Gets the 32-bit ARGB simple color from the specified 8-bit color values (red, green, and blue). The alpha value is implicitly 255 (fully opaque). Although this method allows a 32-bit value to be passed for each color component, the value of each component is limited to 8 bits.
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public static SimpleColor FromArgb(int red, int green, int blue)
        {
            return SimpleColor.FromArgb(255, red, green, blue);
        }

        /// <summary>
        /// Gets the 32-bit ARGB simple color from the four ARGB component (alpha, red, green, and blue) values. Although this method allows a 32-bit value to be passed for each component, the value of each component is limited to 8 bits.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public static SimpleColor FromArgb(int alpha, int red, int green, int blue)
        {
            if ((alpha > 255) || (alpha < 0))
            {
                // alpha out of range!
                throw new ArgumentOutOfRangeException("alpha", "Value has to be in the range 0-255!");
            }
            if ((red > 255) || (red < 0))
            {
                // red out of range!
                throw new ArgumentOutOfRangeException("red", "Value has to be in the range 0-255!");
            }
            if ((green > 255) || (green < 0))
            {
                // green out of range!
                throw new ArgumentOutOfRangeException("green", "Value has to be in the range 0-255!");
            }
            if ((blue > 255) || (blue < 0))
            {
                // red out of range!
                throw new ArgumentOutOfRangeException("blue", "Value has to be in the range 0-255!");
            }
            return new SimpleColor()
                       {
                           Value = (int) ((uint) alpha << 24) + (red << 16) + (green << 8) + blue
                       };
        }

        /// <summary>
        /// Gets the 32-bit ARGB simple color from the known color enumeration.
        /// </summary>
        /// <param name="kc"></param>
        /// <returns></returns>
        public static SimpleColor FromKnownColor(KnownColor kc)
        {
            var n = (short)kc;
            if ((n <= 0) || (n >= ArgbValues.Length))
            {
                return new SimpleColor();
            }
            else
            {
                return new SimpleColor()
                {
                    Value = (int)ArgbValues[n]
                };
            }
        }

        /// <summary>
        /// Gets the 32-bit ARGB simple color from the known color enumeration.
        /// </summary>
        /// <param name="kc"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static SimpleColor FromKnownColor(KnownColor kc, int alpha)
        {
            SimpleColor knownColor = SimpleColor.FromKnownColor(kc);
            return SimpleColor.FromArgb(alpha, knownColor.R, knownColor.G, knownColor.B);
        }

		/// <summary>
		/// Gets the 32-bit ARGB simple color from the given hex string.
		/// </summary>
		/// <returns>The hex.</returns>
		/// <param name="hex">Hex.</param>
		public static SimpleColor FromHex(string hex)
		{
			if (hex == null) {
				throw new ArgumentNullException ("hex");
			}
			if (hex.Length < 7) {
				throw new ArgumentOutOfRangeException ("hex", 
					string.Format("The given string can never be a color: {0} too short.", hex));
			}

            if (hex.Length == 7) {
                return SimpleColor.FromArgb(
                    Int32.Parse("FF" + hex.Replace("#", ""), System.Globalization.NumberStyles.HexNumber,
                                 System.Globalization.CultureInfo.InvariantCulture));
            } 
			else if (hex.Length == 9) {
				return SimpleColor.FromArgb (
					Int32.Parse (hex.Replace ("#", ""), System.Globalization.NumberStyles.HexNumber, 
					             System.Globalization.CultureInfo.InvariantCulture));
			} else if (hex.Length == 10) {
				return SimpleColor.FromArgb (
					Int32.Parse (hex.Replace ("0x", ""), System.Globalization.NumberStyles.HexNumber, 
					             System.Globalization.CultureInfo.InvariantCulture));
			} else {
				throw new ArgumentOutOfRangeException ("hex", 
				                                       string.Format("The given string can is not a hex-color: {0}.", hex));
			}
		}

        /// <summary>
        /// Contains know colors.
        /// </summary>
        static internal uint[] ArgbValues = new uint[] {
                    0x00000000,	/* 000 - Empty */
                    0xFFD4D0C8,	/* 001 - ActiveBorder */
                    0xFF0054E3,	/* 002 - ActiveCaption */
                    0xFFFFFFFF,	/* 003 - ActiveCaptionText */
                    0xFF808080,	/* 004 - AppWorkspace */
                    0xFFECE9D8,	/* 005 - Control */
                    0xFFACA899,	/* 006 - ControlDark */
                    0xFF716F64,	/* 007 - ControlDarkDark */
                    0xFFF1EFE2,	/* 008 - ControlLight */
                    0xFFFFFFFF,	/* 009 - ControlLightLight */
                    0xFF000000,	/* 010 - ControlText */
                    0xFF004E98,	/* 011 - Desktop */
                    0xFFACA899,	/* 012 - GrayText */
                    0xFF316AC5,	/* 013 - Highlight */
                    0xFFFFFFFF,	/* 014 - HighlightText */
                    0xFF000080,	/* 015 - HotTrack */
                    0xFFD4D0C8,	/* 016 - InactiveBorder */
                    0xFF7A96DF,	/* 017 - InactiveCaption */
                    0xFFD8E4F8,	/* 018 - InactiveCaptionText */
                    0xFFFFFFE1,	/* 019 - Info */
                    0xFF000000,	/* 020 - InfoText */
                    0xFFFFFFFF,	/* 021 - Menu */
                    0xFF000000,	/* 022 - MenuText */
                    0xFFD4D0C8,	/* 023 - ScrollBar */
                    0xFFFFFFFF,	/* 024 - Window */
                    0xFF000000,	/* 025 - WindowFrame */
                    0xFF000000,	/* 026 - WindowText */
                    0x00FFFFFF,	/* 027 - Transparent */
                    0xFFF0F8FF,	/* 028 - AliceBlue */
                    0xFFFAEBD7,	/* 029 - AntiqueWhite */
                    0xFF00FFFF,	/* 030 - Aqua */
                    0xFF7FFFD4,	/* 031 - Aquamarine */
                    0xFFF0FFFF,	/* 032 - Azure */
                    0xFFF5F5DC,	/* 033 - Beige */
                    0xFFFFE4C4,	/* 034 - Bisque */
                    0xFF000000,	/* 035 - Black */
                    0xFFFFEBCD,	/* 036 - BlanchedAlmond */
                    0xFF0000FF,	/* 037 - Blue */
                    0xFF8A2BE2,	/* 038 - BlueViolet */
                    0xFFA52A2A,	/* 039 - Brown */
                    0xFFDEB887,	/* 040 - BurlyWood */
                    0xFF5F9EA0,	/* 041 - CadetBlue */
                    0xFF7FFF00,	/* 042 - Chartreuse */
                    0xFFD2691E,	/* 043 - Chocolate */
                    0xFFFF7F50,	/* 044 - Coral */
                    0xFF6495ED,	/* 045 - CornflowerBlue */
                    0xFFFFF8DC,	/* 046 - Cornsilk */
                    0xFFDC143C,	/* 047 - Crimson */
                    0xFF00FFFF,	/* 048 - Cyan */
                    0xFF00008B,	/* 049 - DarkBlue */
                    0xFF008B8B,	/* 050 - DarkCyan */
                    0xFFB8860B,	/* 051 - DarkGoldenrod */
                    0xFFA9A9A9,	/* 052 - DarkGray */
                    0xFF006400,	/* 053 - DarkGreen */
                    0xFFBDB76B,	/* 054 - DarkKhaki */
                    0xFF8B008B,	/* 055 - DarkMagenta */
                    0xFF556B2F,	/* 056 - DarkOliveGreen */
                    0xFFFF8C00,	/* 057 - DarkOrange */
                    0xFF9932CC,	/* 058 - DarkOrchid */
                    0xFF8B0000,	/* 059 - DarkRed */
                    0xFFE9967A,	/* 060 - DarkSalmon */
                    0xFF8FBC8B,	/* 061 - DarkSeaGreen */
                    0xFF483D8B,	/* 062 - DarkSlateBlue */
                    0xFF2F4F4F,	/* 063 - DarkSlateGray */
                    0xFF00CED1,	/* 064 - DarkTurquoise */
                    0xFF9400D3,	/* 065 - DarkViolet */
                    0xFFFF1493,	/* 066 - DeepPink */
                    0xFF00BFFF,	/* 067 - DeepSkyBlue */
                    0xFF696969,	/* 068 - DimGray */
                    0xFF1E90FF,	/* 069 - DodgerBlue */
                    0xFFB22222,	/* 070 - Firebrick */
                    0xFFFFFAF0,	/* 071 - FloralWhite */
                    0xFF228B22,	/* 072 - ForestGreen */
                    0xFFFF00FF,	/* 073 - Fuchsia */
                    0xFFDCDCDC,	/* 074 - Gainsboro */
                    0xFFF8F8FF,	/* 075 - GhostWhite */
                    0xFFFFD700,	/* 076 - Gold */
                    0xFFDAA520,	/* 077 - Goldenrod */
                    0xFF808080,	/* 078 - Gray */
                    0xFF008000,	/* 079 - Green */
                    0xFFADFF2F,	/* 080 - GreenYellow */
                    0xFFF0FFF0,	/* 081 - Honeydew */
                    0xFFFF69B4,	/* 082 - HotPink */
                    0xFFCD5C5C,	/* 083 - IndianRed */
                    0xFF4B0082,	/* 084 - Indigo */
                    0xFFFFFFF0,	/* 085 - Ivory */
                    0xFFF0E68C,	/* 086 - Khaki */
                    0xFFE6E6FA,	/* 087 - Lavender */
                    0xFFFFF0F5,	/* 088 - LavenderBlush */
                    0xFF7CFC00,	/* 089 - LawnGreen */
                    0xFFFFFACD,	/* 090 - LemonChiffon */
                    0xFFADD8E6,	/* 091 - LightBlue */
                    0xFFF08080,	/* 092 - LightCoral */
                    0xFFE0FFFF,	/* 093 - LightCyan */
                    0xFFFAFAD2,	/* 094 - LightGoldenrodYellow */
                    0xFFD3D3D3,	/* 095 - LightGray */
                    0xFF90EE90,	/* 096 - LightGreen */
                    0xFFFFB6C1,	/* 097 - LightPink */
                    0xFFFFA07A,	/* 098 - LightSalmon */
                    0xFF20B2AA,	/* 099 - LightSeaGreen */
                    0xFF87CEFA,	/* 100 - LightSkyBlue */
                    0xFF778899,	/* 101 - LightSlateGray */
                    0xFFB0C4DE,	/* 102 - LightSteelBlue */
                    0xFFFFFFE0,	/* 103 - LightYellow */
                    0xFF00FF00,	/* 104 - Lime */
                    0xFF32CD32,	/* 105 - LimeGreen */
                    0xFFFAF0E6,	/* 106 - Linen */
                    0xFFFF00FF,	/* 107 - Magenta */
                    0xFF800000,	/* 108 - Maroon */
                    0xFF66CDAA,	/* 109 - MediumAquamarine */
                    0xFF0000CD,	/* 110 - MediumBlue */
                    0xFFBA55D3,	/* 111 - MediumOrchid */
                    0xFF9370DB,	/* 112 - MediumPurple */
                    0xFF3CB371,	/* 113 - MediumSeaGreen */
                    0xFF7B68EE,	/* 114 - MediumSlateBlue */
                    0xFF00FA9A,	/* 115 - MediumSpringGreen */
                    0xFF48D1CC,	/* 116 - MediumTurquoise */
                    0xFFC71585,	/* 117 - MediumVioletRed */
                    0xFF191970,	/* 118 - MidnightBlue */
                    0xFFF5FFFA,	/* 119 - MintCream */
                    0xFFFFE4E1,	/* 120 - MistyRose */
                    0xFFFFE4B5,	/* 121 - Moccasin */
                    0xFFFFDEAD,	/* 122 - NavajoWhite */
                    0xFF000080,	/* 123 - Navy */
                    0xFFFDF5E6,	/* 124 - OldLace */
                    0xFF808000,	/* 125 - Olive */
                    0xFF6B8E23,	/* 126 - OliveDrab */
                    0xFFFFA500,	/* 127 - Orange */
                    0xFFFF4500,	/* 128 - OrangeRed */
                    0xFFDA70D6,	/* 129 - Orchid */
                    0xFFEEE8AA,	/* 130 - PaleGoldenrod */
                    0xFF98FB98,	/* 131 - PaleGreen */
                    0xFFAFEEEE,	/* 132 - PaleTurquoise */
                    0xFFDB7093,	/* 133 - PaleVioletRed */
                    0xFFFFEFD5,	/* 134 - PapayaWhip */
                    0xFFFFDAB9,	/* 135 - PeachPuff */
                    0xFFCD853F,	/* 136 - Peru */
                    0xFFFFC0CB,	/* 137 - Pink */
                    0xFFDDA0DD,	/* 138 - Plum */
                    0xFFB0E0E6,	/* 139 - PowderBlue */
                    0xFF800080,	/* 140 - Purple */
                    0xFFFF0000,	/* 141 - Red */
                    0xFFBC8F8F,	/* 142 - RosyBrown */
                    0xFF4169E1,	/* 143 - RoyalBlue */
                    0xFF8B4513,	/* 144 - SaddleBrown */
                    0xFFFA8072,	/* 145 - Salmon */
                    0xFFF4A460,	/* 146 - SandyBrown */
                    0xFF2E8B57,	/* 147 - SeaGreen */
                    0xFFFFF5EE,	/* 148 - SeaShell */
                    0xFFA0522D,	/* 149 - Sienna */
                    0xFFC0C0C0,	/* 150 - Silver */
                    0xFF87CEEB,	/* 151 - SkyBlue */
                    0xFF6A5ACD,	/* 152 - SlateBlue */
                    0xFF708090,	/* 153 - SlateGray */
                    0xFFFFFAFA,	/* 154 - Snow */
                    0xFF00FF7F,	/* 155 - SpringGreen */
                    0xFF4682B4,	/* 156 - SteelBlue */
                    0xFFD2B48C,	/* 157 - Tan */
                    0xFF008080,	/* 158 - Teal */
                    0xFFD8BFD8,	/* 159 - Thistle */
                    0xFFFF6347,	/* 160 - Tomato */
                    0xFF40E0D0,	/* 161 - Turquoise */
                    0xFFEE82EE,	/* 162 - Violet */
                    0xFFF5DEB3,	/* 163 - Wheat */
                    0xFFFFFFFF,	/* 164 - White */
                    0xFFF5F5F5,	/* 165 - WhiteSmoke */
                    0xFFFFFF00,	/* 166 - Yellow */
                    0xFF9ACD32,	/* 167 - YellowGreen */
                };
    }
}