using System.Collections.Generic;
using Android.Support.V4.App;
using DK.Ostebaronen.Droid.ViewPagerIndicator.Interfaces;
using String = Java.Lang.String;
using AlwaysOn.Objects;

namespace AlwaysOn_Droid
{
    public class TabFragmentAdapter : FragmentPagerAdapter, IIconPageAdapter
    {
        private List<List<VoucherPackage>> Content;

        private static readonly int[] Icons =
        {
            0,0,0,0
        };

        public TabFragmentAdapter(FragmentManager p0) : base(p0)
        {
        }

        public TabFragmentAdapter(FragmentManager p0, List<List<VoucherPackage>> items) : base(p0)
        {
            Content = items;
        }

        public override int Count
        {
            get { return Content.Count; }
        }

        public override Fragment GetItem(int position)
        {
            return TabFragment.NewInstance(Content[position]);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            var packageType = Content[position][0].packageType;
            var Title = "";
            switch (packageType)
            {
                case "DT": { Title = "DATA";  break; }
                case "T": { Title = "TIME"; break; }
                case "S": { Title = "PLAN"; break; }
            }
            return new String(Title);
        }
        
        public int GetIconResId(int index)
        {
            return Icons[index % Icons.Length];
        }
    }
}