﻿using UIKit;

namespace AlwaysOn_iOS.Objects
{
    public static class UIColorExtensions
    {
        public static UIColor FromHex(this UIColor color, int hexValue)
        {
            return UIColor.FromRGB(((hexValue & 0xFF0000) >> 16) / 255.0f, ((hexValue & 0xFF00) >> 8) / 255.0f, (hexValue & 0xFF) / 255.0f);
        }
    }
}

