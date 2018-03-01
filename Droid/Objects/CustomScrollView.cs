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
using Android.Util;

namespace AlwaysOn_Droid
{
    public class CustomScrollView : ScrollView
    {
        public bool ScrollEnabled { get; set; } = true;

        public CustomScrollView(Context context) : base(context)
        {
        }

        public CustomScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CustomScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public CustomScrollView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            return !ScrollEnabled ? false : base.OnTouchEvent(ev);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return !ScrollEnabled ? false : base.OnInterceptTouchEvent(ev);
        }
    }
}