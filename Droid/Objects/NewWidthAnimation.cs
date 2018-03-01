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
using Android.Views.Animations;

namespace AlwaysOn_Droid
{
    public class NewWidthAnimation : Animation
    {
        private int startWidth;
        private int deltaWidth; // distance between start and end width
        private View view;

        public NewWidthAnimation(View v, int StartWidth, int EndWidth)
        {
            view = v;
            startWidth = StartWidth;
            deltaWidth = EndWidth - startWidth;
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            view.LayoutParameters.Width = (int)(startWidth + deltaWidth * interpolatedTime);
            view.RequestLayout();
        }

        public override bool WillChangeBounds()
        {
            return true;
        }
    }
}