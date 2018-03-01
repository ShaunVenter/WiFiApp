using AlwaysOn;
using AlwaysOn.Objects;
using AlwaysOn_iOS.Objects;
using CoreGraphics;
using MaterialControls;
using System;
using UIKit;
using XamSvg;

namespace AlwaysOn_iOS
{
    public partial class IntroCarousel : UIViewController
    {
        UISvgImageView _LeftArrow;
        MDButton _LeftArrowBtn;

        UISvgImageView _RightArrow;
        MDButton _RightArrowBtn;

        UISvgImageView _slide1;
        UISvgImageView _slide2;
        UISvgImageView _slide3;
        
        public IntroCarousel() : base("IntroCarousel", null)
        {
            UtilProvider.MenuIsOpen = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AnalyticsProvider_iOS.PageViewGA(PageName.IntroCarousel.ToString());

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            UIScrollView carousel = new UIScrollView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                ShowsHorizontalScrollIndicator = false,
                ContentSize = new CGSize(UtilProvider.ScreenWidth * 3, UtilProvider.ScreenHeight),
                ContentOffset = new CGPoint(0, 0),
                PagingEnabled = true
            };
            carousel.Add(SlideOne());
            carousel.Add(SlideTwo());
            carousel.Add(SlideThree());

            carousel.Scrolled += Carousel_Scrolled;

            View.Add(carousel);
            View.Add(CarouselNavigation(carousel));
            View.Add(UtilProvider.HeaderStrip(false, false, this));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning(); // Release any cached data, images, etc that aren't in use.
        }

        void Carousel_Scrolled(object sender, EventArgs e)
        {
            var carousel = (UIScrollView)sender;

            // update pager dots

            if (carousel.ContentOffset.X == 0)
            {
                _LeftArrowBtn.Alpha = 0;
                _LeftArrow.Alpha = 0;
                _RightArrow.Alpha = 1;
                _RightArrowBtn.Alpha = 1;
                _slide1.Alpha = 1;
                _slide2.Alpha = 0.5f;
                _slide3.Alpha = 0.5f;
            }
            else if (carousel.ContentOffset.X == UtilProvider.ScreenWidth)
            {
                _LeftArrowBtn.Alpha = 1;
                _LeftArrow.Alpha = 1;
                _RightArrow.Alpha = 1;
                _RightArrowBtn.Alpha = 1;
                _slide1.Alpha = 0.5f;
                _slide2.Alpha = 1;
                _slide3.Alpha = 0.5f;
            }
            else if (carousel.ContentOffset.X == (UtilProvider.ScreenWidth * 2))
            {
                _LeftArrowBtn.Alpha = 1;
                _LeftArrow.Alpha = 1;
                _RightArrow.Alpha = 0;
                _RightArrowBtn.Alpha = 0;
                _slide1.Alpha = 0.5f;
                _slide2.Alpha = 0.5f;
                _slide3.Alpha = 1;
            }
        }

        public UIView CarouselNavigation(UIScrollView carousel)
        {
            var NavigationWrapper = new UIView(new CGRect(0, carousel.Bounds.Height - 65, UtilProvider.ScreenWidth, 50));

            var SkipButton = new UIButton();
            SkipButton.Frame = new CGRect(20, -20, 50, 50);
            SkipButton.SetTitle("SKIP", UIControlState.Normal);
            SkipButton.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12);
            SkipButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.IntroCarouselSkipIntro.ToString());

                NavigationController.PushViewController(new LoginScreen(false), true);
            };
            NavigationWrapper.Add(SkipButton);

            _LeftArrow = new UISvgImageView("svg/prev_btn.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth / 4, -2, 18, 18),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Alpha = 0
            };
            NavigationWrapper.Add(_LeftArrow);

            _LeftArrowBtn = new MDButton(new CGRect(UtilProvider.ScreenWidth / 4, -2, 18, 18), MDButtonType.Flat, UIColor.White)
            {
                BackgroundColor = UIColor.Clear
            };
            _LeftArrowBtn.TouchUpInside += (object sender, EventArgs e) =>
            {
                if (carousel.ContentOffset.X == 0)
                {
                    carousel.SetContentOffset(new CGPoint(0, 0), true);
                }
                else if (carousel.ContentOffset.X == UtilProvider.ScreenWidth)
                {
                    carousel.SetContentOffset(new CGPoint(0, 0), true);
                }
                else if (carousel.ContentOffset.X == (UtilProvider.ScreenWidth * 2))
                {
                    carousel.SetContentOffset(new CGPoint(UtilProvider.ScreenWidth, 0), true);
                }
            };
            NavigationWrapper.Add(_LeftArrowBtn);

            _RightArrow = new UISvgImageView("svg/next_btn.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth - 90, -2, 18, 18),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            NavigationWrapper.Add(_RightArrow);

            _RightArrowBtn = new MDButton(new CGRect(UtilProvider.ScreenWidth - 90, -2, 18, 18), MDButtonType.Flat, UIColor.White)
            {
                BackgroundColor = UIColor.Clear
            };
            _RightArrowBtn.TouchUpInside += (object sender, EventArgs e) =>
            {
                if (carousel.ContentOffset.X == 0)
                {
                    carousel.SetContentOffset(new CGPoint(UtilProvider.ScreenWidth, 0), true);
                }
                else if (carousel.ContentOffset.X == UtilProvider.ScreenWidth)
                {
                    carousel.SetContentOffset(new CGPoint(UtilProvider.ScreenWidth * 2, 0), true);
                }
            };
            NavigationWrapper.Add(_RightArrowBtn);

            var pagerWrapper = new UIView(new CGRect((UtilProvider.ScreenWidth / 2) - 45, 3, 90, 10));

            _slide1 = new UISvgImageView("svg/circle.svg")
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(15, 0, 10, 10),
                UserInteractionEnabled = true
            };
            _slide1.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                carousel.SetContentOffset(new CGPoint(0, 0), true);
                _LeftArrowBtn.Alpha = 0;
                _LeftArrow.Alpha = 0;
                _RightArrow.Alpha = 1;
                _RightArrowBtn.Alpha = 1;
                _slide1.Alpha = 1;
                _slide2.Alpha = 0.5f;
                _slide3.Alpha = 0.5f;
            }));
            pagerWrapper.Add(_slide1);
            
            _slide2 = new UISvgImageView("svg/circle.svg")
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(40, 0, 10, 10),
                Alpha = 0.5f,
                UserInteractionEnabled = true
            };
            _slide2.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                //carousel.ContentOffset = new CGPoint(ScreenWidth,0);
                carousel.SetContentOffset(new CGPoint(UtilProvider.ScreenWidth, 0), true);
                _LeftArrowBtn.Alpha = 1;
                _LeftArrow.Alpha = 1;
                _RightArrow.Alpha = 1;
                _RightArrowBtn.Alpha = 1;
                _slide1.Alpha = 0.5f;
                _slide2.Alpha = 1;
                _slide3.Alpha = 0.5f;
            }));
            pagerWrapper.Add(_slide2);
            
            _slide3 = new UISvgImageView("svg/circle.svg")
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(65, 0, 10, 10),
                Alpha = 0.5f,
                UserInteractionEnabled = true
            };
            _slide3.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                    carousel.SetContentOffset(new CGPoint(UtilProvider.ScreenWidth * 2, 0), true);
                    //ScreenWidth
                    _LeftArrowBtn.Alpha = 1;
                    _LeftArrow.Alpha = 1;
                    _RightArrow.Alpha = 0;
                    _RightArrowBtn.Alpha = 0;
                    _slide1.Alpha = 0.5f;
                    _slide2.Alpha = 0.5f;
                    _slide3.Alpha = 1;
            }));
            pagerWrapper.Add(_slide3);
            
            NavigationWrapper.Add(pagerWrapper);
            NavigationWrapper.Add(UtilProvider.ButtonBottomBorder(SkipButton));

            return NavigationWrapper;
        }

        private UIView SlideOne()
        {
            var SlideWrapper = new UIView(new CGRect(0, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple)
            };
            
            var _SlideImage = new UISvgImageView("svg/Intro-03.svg")
            {
                Frame = new CGRect((UtilProvider.ScreenWidth / 2) - 150, (UtilProvider.ScreenHeight / 2) - 140 - (UtilProvider.SafeTop + UtilProvider.ScreenHeight12th), 295, 280),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            SlideWrapper.Add(_SlideImage);

            return SlideWrapper;
        }

        private UIView SlideTwo()
        {
            var SlideWrapper = new UIView(new CGRect(UtilProvider.ScreenWidth, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple)
            };
            
            var _SlideImage = new UISvgImageView("svg/Intro-02.svg")
            {
                Frame = new CGRect((UtilProvider.ScreenWidth / 2) - 125, (UtilProvider.ScreenHeight / 2) - 112 - (UtilProvider.SafeTop + UtilProvider.ScreenHeight12th), 250, 225),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            SlideWrapper.Add(_SlideImage);

            return SlideWrapper;
        }

        private UIView SlideThree()
        {
            var SlideWrapper = new UIView(new CGRect(UtilProvider.ScreenWidth * 2, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple)
            };

            var _SlideImage = new UISvgImageView("svg/Intro-01.svg")
            {
                Frame = new CGRect((UtilProvider.ScreenWidth / 2) - 100, (UtilProvider.ScreenHeight / 2) - 125 - (UtilProvider.SafeTop + UtilProvider.ScreenHeight12th), 200, 250),
                Image = UIImage.FromBundle("slide3"),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            SlideWrapper.Add(_SlideImage);
            
            var getStarted = new MDButton(new CGRect((UtilProvider.ScreenWidth / 2) - 60, UtilProvider.ScreenHeight - 130 - (UtilProvider.SafeTop + UtilProvider.ScreenHeight12th), 120, 35), MDButtonType.Flat, UIColor.White)
            {
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14),
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan)
            };
            getStarted.SetTitle("GET STARTED", UIControlState.Normal);
            getStarted.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.IntroCarouselGetStarted.ToString());

                NavigationController.PushViewController(new LoginScreen(false), true);
            };
            SlideWrapper.Add(getStarted);

            return SlideWrapper;
        }
    }
}