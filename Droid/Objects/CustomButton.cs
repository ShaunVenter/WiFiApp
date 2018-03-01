using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace AlwaysOn_Droid
{
    public class CustomButton : Button
    {
        public CustomButton(Context context) :
            base(context)
        {
            Initialize();
        }

        public CustomButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.CustomFonts);
            var customFont = a.GetString(Resource.Styleable.CustomFonts_customFont);
            SetCustomFont(customFont);
            a.Recycle();
        }

        public CustomButton(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
        }

        public void SetCustomFont(string asset)
        {
            Typeface tf;
            try
            {
                tf = Typeface.CreateFromAsset(Context.Assets, asset);
            }
            catch
            {
                //Log.Error(Tag, string.Format("Could not get Typeface: {0} Error: {1}", asset, e));
                return;
            }

            if (null == tf) return;

            var tfStyle = TypefaceStyle.Normal;
            if (null != Typeface) //Takes care of android:textStyle=""
                tfStyle = Typeface.Style;
            SetTypeface(tf, tfStyle);
        }
    }
}

