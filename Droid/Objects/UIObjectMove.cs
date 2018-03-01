using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AlwaysOn_Droid
{
    public class UIObjectMove
    {
        public int xPad { get; set; }
        public int yPad { get; set; }
        public int xDelta { get; set; }
        public int yDelta { get; set; }
        public bool Moved { get; set; }
    }
}