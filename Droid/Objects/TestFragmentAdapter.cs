using System.Collections.Generic;
using Android.Support.V4.App;
using DK.Ostebaronen.Droid.ViewPagerIndicator.Interfaces;

namespace AlwaysOn_Droid
{
    public class TestFragmentAdapter : FragmentPagerAdapter, IIconPageAdapter
    {
        private List<List<KeyValuePair<string,string>>> Content;

        private static readonly int[] Icons =
        {
            0,0,0,0
        }; 

        private int _count = 3;

        public TestFragmentAdapter(FragmentManager p0) : base(p0)
        {
        }

		public TestFragmentAdapter(FragmentManager p0, List<List<KeyValuePair<string, string>>> items) : base(p0)
        {
            Content = items;
        }

        public override int Count
        {
            get { return _count; }
        }

        public override Fragment GetItem(int position)
        {                                                                                                                                                                             
			return TestFragment.NewInstance(Content[position]);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int p0)
        {
            return new Java.Lang.String("");
        }

        public void SetCount(int count)
        {
            if (count <= 0 || count > 10) return;

            _count = count;
            NotifyDataSetChanged();
        }

        public int GetIconResId(int index)
        {
            return Icons[index % Icons.Length];
        }
    }
}