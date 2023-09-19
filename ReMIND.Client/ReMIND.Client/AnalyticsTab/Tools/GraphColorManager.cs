using System;
using System.Collections.Generic;

using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ReMIND.Client.AnalyticsTab.Tools
{
    public static class GraphColorManager
    {
        private static List<SolidColorPaint> _available { get; set; } = new();
        private static List<SolidColorPaint> _selected { get; set; } = new();

        public static SolidColorPaint takeNext()
        {
            Random r = new Random();
            int randIndex = r.Next(0, _available.Count);

            SolidColorPaint paintToTake = _available[randIndex];
            _selected.Add(paintToTake);
            _available.RemoveAt(randIndex);

            return paintToTake;
        }
        public static bool release(SolidColorPaint color)
        {
            if (_selected.Contains(color))
            {
                _available.Add(color);
                _selected.Remove(color);
                return true;
            }
            return false;
        }

        public static void loadColors()
        {
            string opacity = "A0";//hex code for 50%, for opacity
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}D37979")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}318FB7")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}698626")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}D0A90E")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}7E5D09")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}B53AB2")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}DA8C29")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}16A774")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}002B4C")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}9086E8")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}EA5617")));
            _available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}19ABE9")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}9ED322")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}FDD535")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}E420E0")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}FF8F00")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}21DD9B")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}0078D3")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}000000")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}4533E5")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}266986")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}862626")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}C2E082")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}263086")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}862671")));
            //_available.Add(new SolidColorPaint(SKColor.Parse($"#{opacity}666666")));
        }
    }
}
