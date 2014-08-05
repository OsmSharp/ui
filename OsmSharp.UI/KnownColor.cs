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
    /// Enumeration of know colors.
    /// </summary>
    public enum KnownColor
    {
        /// <summary>
        /// ActiveBorder
        /// </summary>
        ActiveBorder = 1,
        /// <summary>
        /// ActiveCaption
        /// </summary>
        ActiveCaption = 2,
        /// <summary>
        /// ActiveCaptionText
        /// </summary>
        ActiveCaptionText = 3,
        /// <summary>
        /// AppWorkspace
        /// </summary>
        AppWorkspace = 4,
        /// <summary>
        /// Control
        /// </summary>
        Control = 5,
        /// <summary>
        /// ControlDark
        /// </summary>
        ControlDark = 6,
        /// <summary>
        /// ControlDarkDark
        /// </summary>
        ControlDarkDark = 7,
        /// <summary>
        /// ControlLight
        /// </summary>
        ControlLight = 8,
        /// <summary>
        /// ControlLightLight
        /// </summary>
        ControlLightLight = 9,
        /// <summary>
        /// ControlText
        /// </summary>
        ControlText = 10,
        /// <summary>
        /// Desktop
        /// </summary>
        Desktop = 11,
        /// <summary>
        /// GrayText
        /// </summary>
        GrayText = 12,
        /// <summary>
        /// Highlight
        /// </summary>
        Highlight = 13,
        /// <summary>
        /// HighlightText
        /// </summary>
        HighlightText = 14,
        /// <summary>
        /// HotTrack
        /// </summary>
        HotTrack = 15,
        /// <summary>
        /// InactiveBorder
        /// </summary>
        InactiveBorder = 16,
        /// <summary>
        /// InactiveCaption
        /// </summary>
        InactiveCaption = 17,
        /// <summary>
        /// InactiveCaptionText
        /// </summary>
        InactiveCaptionText = 18,
        /// <summary>
        /// Info
        /// </summary>
        Info = 19,
        /// <summary>
        /// InfoText
        /// </summary>
        InfoText = 20,
        /// <summary>
        /// Menu
        /// </summary>
        Menu = 21,
        /// <summary>
        /// MenuText
        /// </summary>
        MenuText = 22,
        /// <summary>
        /// ScrollBar
        /// </summary>
        ScrollBar = 23,
        /// <summary>
        /// Window
        /// </summary>
        Window = 24,
        /// <summary>
        /// WindowFrame
        /// </summary>
        WindowFrame = 25,
        /// <summary>
        /// WindowText
        /// </summary>
        WindowText = 26,
        /// <summary>
        /// Transparent
        /// </summary>
        Transparent = 27,
        /// <summary>
        /// AliceBlue
        /// </summary>
        AliceBlue = 28,
        /// <summary>
        /// AntiqueWhite
        /// </summary>
        AntiqueWhite = 29,
        /// <summary>
        /// Aqua
        /// </summary>
        Aqua = 30,
        /// <summary>
        /// Aquamarine
        /// </summary>
        Aquamarine = 31,
        /// <summary>
        /// Azure
        /// </summary>
        Azure = 32,
        /// <summary>
        /// Beige
        /// </summary>
        Beige = 33,
        /// <summary>
        /// Bisque
        /// </summary>
        Bisque = 34,
        /// <summary>
        /// Black
        /// </summary>
        Black = 35,
        /// <summary>
        /// BlanchedAlmond
        /// </summary>
        BlanchedAlmond = 36,
        /// <summary>
        /// Blue
        /// </summary>
        Blue = 37,
        /// <summary>
        /// BlueViolet
        /// </summary>
        BlueViolet = 38,
        /// <summary>
        /// Brown
        /// </summary>
        Brown = 39,
        /// <summary>
        /// BurlyWood
        /// </summary>
        BurlyWood = 40,
        /// <summary>
        /// CadetBlue
        /// </summary>
        CadetBlue = 41,
        /// <summary>
        /// Chartreuse
        /// </summary>
        Chartreuse = 42,
        /// <summary>
        /// Chocolate
        /// </summary>
        Chocolate = 43,
        /// <summary>
        /// Coral
        /// </summary>
        Coral = 44,
        /// <summary>
        /// CornflowerBlue
        /// </summary>
        CornflowerBlue = 45,
        /// <summary>
        /// Cornsilk
        /// </summary>
        Cornsilk = 46,
        /// <summary>
        /// Crimson
        /// </summary>
        Crimson = 47,
        /// <summary>
        /// Cyan
        /// </summary>
        Cyan = 48,
        /// <summary>
        /// DarkBlue
        /// </summary>
        DarkBlue = 49,
        /// <summary>
        /// DarkCyan
        /// </summary>
        DarkCyan = 50,
        /// <summary>
        /// DarkGoldenrod
        /// </summary>
        DarkGoldenrod = 51,
        /// <summary>
        /// DarkGray
        /// </summary>
        DarkGray = 52,
        /// <summary>
        /// DarkGreen
        /// </summary>
        DarkGreen = 53,
        /// <summary>
        /// DarkKhaki
        /// </summary>
        DarkKhaki = 54,
        /// <summary>
        /// DarkMagenta
        /// </summary>
        DarkMagenta = 55,
        /// <summary>
        /// DarkOliveGreen
        /// </summary>
        DarkOliveGreen = 56,
        /// <summary>
        /// DarkOrange
        /// </summary>
        DarkOrange = 57,
        /// <summary>
        /// DarkOrchid
        /// </summary>
        DarkOrchid = 58,
        /// <summary>
        /// DarkRed
        /// </summary>
        DarkRed = 59,
        /// <summary>
        /// DarkSalmon
        /// </summary>
        DarkSalmon = 60,
        /// <summary>
        /// DarkSeaGreen
        /// </summary>
        DarkSeaGreen = 61,
        /// <summary>
        /// DarkSlateBlue
        /// </summary>
        DarkSlateBlue = 62,
        /// <summary>
        /// DarkSlateGray
        /// </summary>
        DarkSlateGray = 63,
        /// <summary>
        /// DarkTurquoise
        /// </summary>
        DarkTurquoise = 64,
        /// <summary>
        /// DarkViolet
        /// </summary>
        DarkViolet = 65,
        /// <summary>
        /// DeepPink
        /// </summary>
        DeepPink = 66,
        /// <summary>
        /// DeepSkyBlue
        /// </summary>
        DeepSkyBlue = 67,
        /// <summary>
        /// DimGray
        /// </summary>
        DimGray = 68,
        /// <summary>
        /// DodgerBlue
        /// </summary>
        DodgerBlue = 69,
        /// <summary>
        /// Firebrick
        /// </summary>
        Firebrick = 70,
        /// <summary>
        /// FloralWhite
        /// </summary>
        FloralWhite = 71,
        /// <summary>
        /// ForestGreen
        /// </summary>
        ForestGreen = 72,
        /// <summary>
        /// Fuchsia
        /// </summary>
        Fuchsia = 73,
        /// <summary>
        /// Gainsboro
        /// </summary>
        Gainsboro = 74,
        /// <summary>
        /// GhostWhite
        /// </summary>
        GhostWhite = 75,
        /// <summary>
        /// Gold
        /// </summary>
        Gold = 76,
        /// <summary>
        /// Goldenrod
        /// </summary>
        Goldenrod = 77,
        /// <summary>
        /// Gray
        /// </summary>
        Gray = 78,
        /// <summary>
        /// Green
        /// </summary>
        Green = 79,
        /// <summary>
        /// GreenYellow
        /// </summary>
        GreenYellow = 80,
        /// <summary>
        /// Honeydew
        /// </summary>
        Honeydew = 81,
        /// <summary>
        /// HotPink
        /// </summary>
        HotPink = 82,
        /// <summary>
        /// IndianRed
        /// </summary>
        IndianRed = 83,
        /// <summary>
        /// Indigo
        /// </summary>
        Indigo = 84,
        /// <summary>
        /// Ivory
        /// </summary>
        Ivory = 85,
        /// <summary>
        /// Khaki
        /// </summary>
        Khaki = 86,
        /// <summary>
        /// Lavender
        /// </summary>
        Lavender = 87,
        /// <summary>
        /// LavenderBlush
        /// </summary>
        LavenderBlush = 88,
        /// <summary>
        /// LawnGreen
        /// </summary>
        LawnGreen = 89,
        /// <summary>
        /// LemonChiffon
        /// </summary>
        LemonChiffon = 90,
        /// <summary>
        /// LightBlue
        /// </summary>
        LightBlue = 91,
        /// <summary>
        /// LightCoral
        /// </summary>
        LightCoral = 92,
        /// <summary>
        /// LightCyan
        /// </summary>
        LightCyan = 93,
        /// <summary>
        /// LightGoldenrodYellow
        /// </summary>
        LightGoldenrodYellow = 94,
        /// <summary>
        /// LightGray
        /// </summary>
        LightGray = 95,
        /// <summary>
        /// LightGreen
        /// </summary>
        LightGreen = 96,
        /// <summary>
        /// LightPink
        /// </summary>
        LightPink = 97,
        /// <summary>
        /// LightSalmon
        /// </summary>
        LightSalmon = 98,
        /// <summary>
        /// LightSeaGreen
        /// </summary>
        LightSeaGreen = 99,
        /// <summary>
        /// LightSkyBlue
        /// </summary>
        LightSkyBlue = 100,
        /// <summary>
        /// LightSlateGray
        /// </summary>
        LightSlateGray = 101,
        /// <summary>
        /// LightSteelBlue
        /// </summary>
        LightSteelBlue = 102,
        /// <summary>
        /// LightYellow
        /// </summary>
        LightYellow = 103,
        /// <summary>
        /// Lime
        /// </summary>
        Lime = 104,
        /// <summary>
        /// LimeGreen
        /// </summary>
        LimeGreen = 105,
        /// <summary>
        /// Linen
        /// </summary>
        Linen = 106,
        /// <summary>
        /// Magenta
        /// </summary>
        Magenta = 107,
        /// <summary>
        /// Maroon
        /// </summary>
        Maroon = 108,
        /// <summary>
        /// MediumAquamarine
        /// </summary>
        MediumAquamarine = 109,
        /// <summary>
        /// MediumBlue
        /// </summary>
        MediumBlue = 110,
        /// <summary>
        /// MediumOrchid
        /// </summary>
        MediumOrchid = 111,
        /// <summary>
        /// MediumPurple
        /// </summary>
        MediumPurple = 112,
        /// <summary>
        /// MediumSeaGreen
        /// </summary>
        MediumSeaGreen = 113,
        /// <summary>
        /// MediumSlateBlue
        /// </summary>
        MediumSlateBlue = 114,
        /// <summary>
        /// MediumSpringGreen
        /// </summary>
        MediumSpringGreen = 115,
        /// <summary>
        /// MediumTurquoise
        /// </summary>
        MediumTurquoise = 116,
        /// <summary>
        /// MediumVioletRed
        /// </summary>
        MediumVioletRed = 117,
        /// <summary>
        /// MidnightBlue
        /// </summary>
        MidnightBlue = 118,
        /// <summary>
        /// MintCream
        /// </summary>
        MintCream = 119,
        /// <summary>
        /// MistyRose
        /// </summary>
        MistyRose = 120,
        /// <summary>
        /// Moccasin
        /// </summary>
        Moccasin = 121,
        /// <summary>
        /// NavajoWhite
        /// </summary>
        NavajoWhite = 122,
        /// <summary>
        /// Navy
        /// </summary>
        Navy = 123,
        /// <summary>
        /// OldLace
        /// </summary>
        OldLace = 124,
        /// <summary>
        /// Olive
        /// </summary>
        Olive = 125,
        /// <summary>
        /// OliveDrab
        /// </summary>
        OliveDrab = 126,
        /// <summary>
        /// Orange
        /// </summary>
        Orange = 127,
        /// <summary>
        /// OrangeRed
        /// </summary>
        OrangeRed = 128,
        /// <summary>
        /// Orchid
        /// </summary>
        Orchid = 129,
        /// <summary>
        /// PaleGoldenrod
        /// </summary>
        PaleGoldenrod = 130,
        /// <summary>
        /// PaleGreen
        /// </summary>
        PaleGreen = 131,
        /// <summary>
        /// PaleTurquoise
        /// </summary>
        PaleTurquoise = 132,
        /// <summary>
        /// PaleVioletRed
        /// </summary>
        PaleVioletRed = 133,
        /// <summary>
        /// PapayaWhip
        /// </summary>
        PapayaWhip = 134,
        /// <summary>
        /// PeachPuff
        /// </summary>
        PeachPuff = 135,
        /// <summary>
        /// Peru
        /// </summary>
        Peru = 136,
        /// <summary>
        /// Pink
        /// </summary>
        Pink = 137,
        /// <summary>
        /// Plum
        /// </summary>
        Plum = 138,
        /// <summary>
        /// PowderBlue
        /// </summary>
        PowderBlue = 139,
        /// <summary>
        /// Purple
        /// </summary>
        Purple = 140,
        /// <summary>
        /// Red
        /// </summary>
        Red = 141,
        /// <summary>
        /// RosyBrown
        /// </summary>
        RosyBrown = 142,
        /// <summary>
        /// RoyalBlue
        /// </summary>
        RoyalBlue = 143,
        /// <summary>
        /// SaddleBrown
        /// </summary>
        SaddleBrown = 144,
        /// <summary>
        /// Salmon
        /// </summary>
        Salmon = 145,
        /// <summary>
        /// SandyBrown
        /// </summary>
        SandyBrown = 146,
        /// <summary>
        /// SeaGreen
        /// </summary>
        SeaGreen = 147,
        /// <summary>
        /// SeaShell
        /// </summary>
        SeaShell = 148,
        /// <summary>
        /// Sienna
        /// </summary>
        Sienna = 149,
        /// <summary>
        /// Silver
        /// </summary>
        Silver = 150,
        /// <summary>
        /// SkyBlue
        /// </summary>
        SkyBlue = 151,
        /// <summary>
        /// SlateBlue
        /// </summary>
        SlateBlue = 152,
        /// <summary>
        /// SlateGray
        /// </summary>
        SlateGray = 153,
        /// <summary>
        /// Snow
        /// </summary>
        Snow = 154,
        /// <summary>
        /// SpringGreen
        /// </summary>
        SpringGreen = 155,
        /// <summary>
        /// SteelBlue
        /// </summary>
        SteelBlue = 156,
        /// <summary>
        /// Tan
        /// </summary>
        Tan = 157,
        /// <summary>
        /// Teal
        /// </summary>
        Teal = 158,
        /// <summary>
        /// Thistle
        /// </summary>
        Thistle = 159,
        /// <summary>
        /// Tomato
        /// </summary>
        Tomato = 160,
        /// <summary>
        /// Turquoise
        /// </summary>
        Turquoise = 161,
        /// <summary>
        /// Violet
        /// </summary>
        Violet = 162,
        /// <summary>
        /// Wheat
        /// </summary>
        Wheat = 163,
        /// <summary>
        /// White
        /// </summary>
        White = 164,
        /// <summary>
        /// WhiteSmoke
        /// </summary>
        WhiteSmoke = 165,
        /// <summary>
        /// Yellow
        /// </summary>
        Yellow = 166,
        /// <summary>
        /// YellowGreen
        /// </summary>
        YellowGreen = 167
    }
}