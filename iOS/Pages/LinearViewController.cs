using System;
using UIKit;
using Alliance.Carousel;
using System.Collections.Generic;
using System.Linq;

namespace AlwaysOn_iOS
{
    public partial class LinearViewController : UIViewController
    {
        public List<int> items;

        public LinearViewController() : base("LinearViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


