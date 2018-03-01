using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Content.PM;
using AlwaysOn;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Graphics;
using Android.Views.Animations;
using XamSvg;
using System.Threading.Tasks;
using AlwaysOn.Objects;


namespace AlwaysOn_Droid
{

    public class WidthAnimation : Animation
    {
        private int _newValue;
        private int _initialValue;
        private string _property;
        private object _target;
        private View _view;

        public WidthAnimation(View view, string property, int newValue)
        {
            _view = view;
            _property = property;
            _newValue = newValue;
            _target = view.LayoutParameters;

            // View measure is generally an enum of wrap_content / fill_parent
            // we need to make the measure explicit
            if (property == "Width" || property == "Height")
            {
                view.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);
                var unmeasuredValue = (int)view.GetType().GetProperty(property).GetValue(view);
                _initialValue = unmeasuredValue < 1 ? (int)view.GetType().GetProperty("Measured" + _property).GetValue(view) : unmeasuredValue;

                view.LayoutParameters.GetType().GetProperty(property).SetValue(view.LayoutParameters, _initialValue);
            }
            else
            {
                _initialValue = (int)_target.GetType().GetProperty(property).GetValue(_target);
            }
        }
    }
}

