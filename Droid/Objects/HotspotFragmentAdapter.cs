using Android.Support.V4.App;
using DK.Ostebaronen.Droid.ViewPagerIndicator.Interfaces;
using String = Java.Lang.String;

namespace AlwaysOn_Droid
{
    public class HotspotFragmentAdapter : FragmentPagerAdapter, IIconPageAdapter
    {
        private readonly string[] Titles =
        {
            "All WiFi hotspots","Super WiFi only"
        };

        private static readonly int[] Icons =
        {
            0,0,0,0
        };

        public HotspotFragmentAdapter(FragmentManager p0) : base(p0)
        {
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return HotspotsFragment.NewInstance();
                case 1:
                    return SuperHotspotFragment.NewInstance();
                default:
                    return null;
            }
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(Titles[position]);
        }

        public int GetIconResId(int index)
        {
            return Icons[index % Icons.Length];
        }
    }
}