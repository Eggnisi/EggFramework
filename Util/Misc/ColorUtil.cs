#region

//文件创建者：Egg
//创建时间：09-08 01:32

#endregion

using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace EggFramework.Util
{
    public static class ColorUtil
    {
        public static readonly List<string> ColorEnum = new()
        {
            "White", "Red", "Yellow", "Blue", "Cyan", "Magenta", "Green", "Black", "Grey", "Gray", ""
        };

        public static Color SetTransparent(this Color self)
        {
            return self.SetAlpha(0);
        }

        public static Color SetAlpha(this Color self, float alpha)
        {
            alpha = Mathf.Clamp01(alpha);
            return new Color(self.r, self.g, self.b, alpha);
        }

        public static Color ResetTransparent(this Color self)
        {
            return self.SetAlpha(1);
        }

        public static Color ParseHexColor(string hex)
        {
            if (hex.Length != 7 || !hex.StartsWith("#"))
            {
                Debug.LogError("颜色格式解析错误");
                return Color.clear;
            }

            var red   = int.Parse(hex[1..3], NumberStyles.HexNumber);
            var green = int.Parse(hex[3..5], NumberStyles.HexNumber);
            var blue  = int.Parse(hex[5..7], NumberStyles.HexNumber);
            return new Color(red / 255f, green / 255f, blue / 255f);
        }

        public static Color ParseColor(string key)
        {
            return key switch
            {
                "Red"     => Color.red,
                "Blue"    => Color.blue,
                "Green"   => Color.green,
                "Magenta" => Color.magenta,
                "Cyan"    => Color.cyan,
                "Yellow"  => Color.yellow,
                "White"   => Color.white,
                "Black"   => Color.black,
                "Grey"    => Color.grey,
                "Gray"    => Color.gray,
                _         => Color.clear
            };
        }

        public static string Color2Hex(Color color)
        {
            return
                $"#{Mathf.RoundToInt(color.r * 255f):X2}{Mathf.RoundToInt(color.g * 255f):X2}{Mathf.RoundToInt(color.b * 255f):X2}";
        }
    }
}