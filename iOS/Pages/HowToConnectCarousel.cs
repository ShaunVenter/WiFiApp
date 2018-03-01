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
    public partial class HowToConnectCarousel : UIViewController
    {
        UISvgImageView _SlideImage;
        UILabel _MainHeader;
        UILabel _SubHeader;
        UISvgImageView _LeftArrow;
        UISvgImageView _slide1;
        UISvgImageView _slide2;
        UISvgImageView _slide3;
        UISvgImageView _slide4;
        UISvgImageView _RightArrow;
        MDButton _RightArrowBtn;
        MDButton _LeftArrowBtn;
        LoadingOverlay loadingOverlay;

        public HowToConnectCarousel() : base("HowToConnectCarousel", null)
        {
            UtilProvider.MenuIsOpen = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            AnalyticsProvider_iOS.PageViewGA(PageName.HowToConnect.ToString());

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            UIScrollView carousel = new UIScrollView(new CGRect(0, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                ShowsHorizontalScrollIndicator = false,
                ContentSize = new CGSize(UtilProvider.ScreenWidth * 4, UtilProvider.ScreenHeight),
                ContentOffset = new CGPoint(0, 0)
            };

            carousel.Add(SlideOne());
            carousel.Add(SlideTwo());
            carousel.Add(SlideThree());
            carousel.Add(SlideFour());

            carousel.PagingEnabled = true;
            carousel.Scrolled += Carousel_Scrolled;

            View.Add(carousel);
            View.Add(CarouselNavigation(carousel));
            View.Add(UtilProvider.HeaderStrip(true, true, this));
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
                _RightArrow.Alpha = 1;
                _RightArrowBtn.Alpha = 1;
                _slide1.Alpha = 0.5f;
                _slide2.Alpha = 0.5f;
                _slide4.Alpha = 0.5f;
                _slide3.Alpha = 1;
            }
            else if (carousel.ContentOffset.X == (UtilProvider.ScreenWidth * 3))
            {
                _LeftArrow.Alpha = 1;
                _LeftArrowBtn.Alpha = 1;
                _RightArrow.Alpha = 0;
                _slide1.Alpha = 0.5f;
                _slide2.Alpha = 0.5f;
                _slide3.Alpha = 0.5f;
                _slide4.Alpha = 1;
            }
        }

        private UIView SlideOne()
        {
            var SlideWrapper = new UIView(new CGRect(0, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight));

            _SlideImage = new UISvgImageView("svg/settingsGrid.svg")
            {
                Frame = new CGRect((UtilProvider.ScreenWidth / 2) - 120, (UtilProvider.ScreenHeight / 2) - 120, 240, 250),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            _MainHeader = new UILabel(new CGRect(30, (UtilProvider.ScreenWidth / 7) + 15, UtilProvider.ScreenWidth - 30, 30));
            _MainHeader.TextColor = UIColor.White;
            //_MainHeader.MinimumFontSize =  100;
            _MainHeader.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14);
            _MainHeader.TextAlignment = UITextAlignment.Left;
            _MainHeader.Lines = 0;
            _MainHeader.Text = "How to connect to AlwaysOn";

            _SubHeader = new UILabel(new CGRect(30, (_MainHeader.Bounds.Y + 30) + (UtilProvider.ScreenWidth / 7), UtilProvider.ScreenWidth - 30, 50));
            _SubHeader.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 20);
            _SubHeader.TextAlignment = UITextAlignment.Left;
            _SubHeader.TextColor = UIColor.White;
            _SubHeader.Lines = 0;
            _SubHeader.Text = "1. Go to your settings app";

            SlideWrapper.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorSlideOrange);

            SlideWrapper.Add(_MainHeader);
            SlideWrapper.Add(_SubHeader);
            SlideWrapper.Add(_SlideImage);

            return SlideWrapper;
        }

        private UIView SlideTwo()
        {
            var SlideWrapper = new UIView(new CGRect(UtilProvider.ScreenWidth, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight));
            
            _MainHeader = new UILabel(new CGRect(30, (UtilProvider.ScreenWidth / 7) + 15, UtilProvider.ScreenWidth - 30, 30));
            _MainHeader.TextColor = UIColor.White;
            //_MainHeader.MinimumFontSize =  100;
            _MainHeader.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14);
            _MainHeader.TextAlignment = UITextAlignment.Left;
            _MainHeader.Lines = 0;
            _MainHeader.Text = "How to connect to AlwaysOn";
            
            _SubHeader = new UILabel(new CGRect(30, (_MainHeader.Bounds.Y + 30) + (UtilProvider.ScreenWidth / 7), UtilProvider.ScreenWidth - 60, 50));
            _SubHeader.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 20);
            _SubHeader.TextAlignment = UITextAlignment.Left;
            _SubHeader.TextColor = UIColor.White;
            _SubHeader.Lines = 0;
            _SubHeader.Text = "2. Turn on your WiFi ";


            UILabel wifiLabel = new UILabel(new CGRect(40, 10, 60, 30))
            {
                Text = "Wi-Fi",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 16),
                TextColor = UIColor.Black
            };

            UIView WhiteStripWrapper = new UIView(new CGRect(0, UtilProvider.ScreenHeight / 3, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(0xf8f8f8)
            };

            _SlideImage = new UISvgImageView("svg/switchOff.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth - 90, (WhiteStripWrapper.Bounds.Height / 2) - 12.5, 65, 25),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            WhiteStripWrapper.Add(wifiLabel);
            WhiteStripWrapper.Add(_SlideImage);

            SlideWrapper.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorTan);
            
            SlideWrapper.Add(WhiteStripWrapper);
            SlideWrapper.Add(_MainHeader);
            SlideWrapper.Add(_SubHeader);
            
            return SlideWrapper;
        }

        private UIView SlideThree()
        {
            var SlideWrapper = new UIView(new CGRect(UtilProvider.ScreenWidth * 2, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight));
            
            _MainHeader = new UILabel(new CGRect(30, (UtilProvider.ScreenWidth / 7), UtilProvider.ScreenWidth - 60, 60));
            _MainHeader.TextColor = UIColor.White;
            //_MainHeader.MinimumFontSize =  100;
            _MainHeader.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14);
            _MainHeader.TextAlignment = UITextAlignment.Left;
            _MainHeader.Lines = 0;
            _MainHeader.Text = "How to connect to AlwaysOn";

            _SubHeader = new UILabel(new CGRect(30, (_MainHeader.Bounds.Y + 30) + (UtilProvider.ScreenWidth / 7), UtilProvider.ScreenWidth - 60, 100));
            _SubHeader.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 20);
            _SubHeader.TextAlignment = UITextAlignment.Left;
            _SubHeader.TextColor = UIColor.White;
            _SubHeader.Lines = 0;
            _SubHeader.Text = "3. Your phone should find the hotspots that are in range within a few seconds. ";

            UILabel wifiLabel = new UILabel(new CGRect(40, 10, 60, 30))
            {
                Text = "Wi-Fi",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 16),
                TextColor = UIColor.Black
            };

            UIView WhiteStripWrapper = new UIView(new CGRect(0, UtilProvider.ScreenHeight / 3, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(0xf8f8f8)
            };

            _SlideImage = new UISvgImageView("svg/switchOn.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth - 90, (WhiteStripWrapper.Bounds.Height / 2) - 12.5, 65, 25),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            WhiteStripWrapper.Add(wifiLabel);
            WhiteStripWrapper.Add(_SlideImage);

            UILabel wifiSearch = new UILabel(new CGRect(40, (UtilProvider.ScreenHeight / 3) + 50, 150, 30))
            {
                Text = "CHOOSE A NETWORK",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 15),
                TextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorGrey)
            };
            
            UIView WhiteStripWrapperOne = new UIView(new CGRect(0, (UtilProvider.ScreenHeight / 3) + 80, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(0xf8f8f8)
            };
            
            UILabel AlwaysOnLabel = new UILabel(new CGRect(40, 10, 150, 30))
            {
                Text = "AlwaysOn",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 16),
                TextColor = UIColor.Black
            };

            WhiteStripWrapperOne.Add(AlwaysOnLabel);
            WhiteStripWrapperOne.Add(UtilProvider.WifiSettingsImages(WhiteStripWrapperOne));
            WhiteStripWrapperOne.Add(UtilProvider.WifiDivider(WhiteStripWrapperOne));

            UIView WhiteStripWrapperTwo = new UIView(new CGRect(0, (UtilProvider.ScreenHeight / 3) + 127, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(0xf8f8f8)
            };
            
            UILabel WifiOne = new UILabel(new CGRect(40, 10, 150, 30))
            {
                Text = "Other Network 1",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 16),
                TextColor = UIColor.Black
            };

            WhiteStripWrapperTwo.Add(WifiOne);
            WhiteStripWrapperTwo.Add(UtilProvider.WifiSettingsImages(WhiteStripWrapperTwo));
            WhiteStripWrapperTwo.Add(UtilProvider.WifiDivider(WhiteStripWrapperOne));
            
            UIView WhiteStripWrapperThree = new UIView(new CGRect(0, (UtilProvider.ScreenHeight / 3) + 174, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(0xf8f8f8)
            };
            
            UILabel WifiTwo = new UILabel(new CGRect(40, 10, 150, 30))
            {
                Text = "Other Network 2",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 16),
                TextColor = UIColor.Black
            };

            WhiteStripWrapperThree.Add(WifiTwo);
            WhiteStripWrapperThree.Add(UtilProvider.WifiSettingsImages(WhiteStripWrapperThree));

            SlideWrapper.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);

            SlideWrapper.Add(_MainHeader);
            SlideWrapper.Add(_SubHeader);
            SlideWrapper.Add(WhiteStripWrapper);
            SlideWrapper.Add(wifiSearch);
            SlideWrapper.Add(WhiteStripWrapperOne);
            SlideWrapper.Add(WhiteStripWrapperTwo);
            SlideWrapper.Add(WhiteStripWrapperThree);

            return SlideWrapper;
        }

        private UIView SlideFour()
        {
            var SlideWrapper = new UIView(new CGRect(UtilProvider.ScreenWidth * 3, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight));
            
            _MainHeader = new UILabel(new CGRect(30, (UtilProvider.ScreenWidth / 7), UtilProvider.ScreenWidth - 60, 60));
            _MainHeader.TextColor = UIColor.White;
            //_MainHeader.MinimumFontSize =  100;
            _MainHeader.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14);
            _MainHeader.TextAlignment = UITextAlignment.Left;
            _MainHeader.Lines = 0;
            _MainHeader.Text = "How to connect to AlwaysOn";

            _SubHeader = new UILabel(new CGRect(30, (_MainHeader.Bounds.Y + 15) + (UtilProvider.ScreenWidth / 7), UtilProvider.ScreenWidth - 60, 100));
            _SubHeader.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 20);
            _SubHeader.TextAlignment = UITextAlignment.Left;
            _SubHeader.TextColor = UIColor.White;
            _SubHeader.Lines = 0;
            _SubHeader.Text = "4. Tap the AlwaysOn hotspot that’s in range.";

            UILabel extraInfo = new UILabel(new CGRect(30, (_MainHeader.Bounds.Y + 65) + (UtilProvider.ScreenWidth / 7), UtilProvider.ScreenWidth - 60, 100))
            {
                Text = "If you’re unsure of the Wi-Fi hotspot’s name, check the ",
                TextAlignment = UITextAlignment.Left,
                Lines = 2,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TextColor = UIColor.White

            };

            var hotspotFinder = new UIButton(new CGRect(0, _MainHeader.Bounds.Y + 164, 150, 50))
            {
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14)
            };
            hotspotFinder.SetTitle("Hotspot Finder", UIControlState.Normal);
            hotspotFinder.TouchUpInside += (object sender, EventArgs e) =>
            {
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                View.Add(loadingOverlay);

                //Navigate to the listing page
                NavigationController.PushViewController(new HotspotTabs(), true);
            };

            UIView hotspotFinderUnderline = new UIView(new CGRect(33, _MainHeader.Bounds.Y + 195, 85, 2))
            {
                BackgroundColor = UIColor.White
            };
            
            UILabel wifiLabel = new UILabel(new CGRect(40, 10, 60, 30))
            {
                Text = "Wi-Fi",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 16),
                TextColor = UIColor.Black
            };

            UIView WhiteStripWrapper = new UIView(new CGRect(0, (UtilProvider.ScreenHeight / 3) + 10, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(0xf8f8f8)
            };

            _SlideImage = new UISvgImageView("svg/switchOn.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth - 90, (WhiteStripWrapper.Bounds.Height / 2) - 12.5, 65, 25),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            WhiteStripWrapper.Add(wifiLabel);
            WhiteStripWrapper.Add(_SlideImage);
            WhiteStripWrapper.Add(UtilProvider.WifiDivider(WhiteStripWrapper));

            UIView WhiteStripWrapperOne = new UIView(new CGRect(0, (UtilProvider.ScreenHeight / 3) + 57, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(0xf8f8f8)
            };

            UILabel AlwaysOnLabel = new UILabel(new CGRect(40, 10, 150, 30))
            {
                Text = "AlwaysOn",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 16),
                TextColor = UIColor.Black
            };

            WhiteStripWrapperOne.Add(AlwaysOnLabel);
            WhiteStripWrapperOne.Add(UtilProvider.WifiSettingsImages(WhiteStripWrapperOne));


            var GotIt = new MDButton(new CGRect(40, (UtilProvider.ScreenHeight / 3) + 140, UtilProvider.ScreenWidth / 2 - 45, 40), MDButtonType.Flat, UIColor.White);
            GotIt.SetTitle("YES, GOT IT", UIControlState.Normal);
            GotIt.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            GotIt.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);

            GotIt.TouchUpInside += async (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HowToConnectGotIt.ToString());

                await UtilProvider.LoadDashBoard(this);
            };

            var GetHelp = new MDButton(new CGRect((UtilProvider.ScreenWidth - 40) - (UtilProvider.ScreenWidth / 2 - 45), (UtilProvider.ScreenHeight / 3) + 140, UtilProvider.ScreenWidth / 2 - 45, 40), MDButtonType.Flat, UIColor.White);
            GetHelp.SetTitle("NO, GET HELP", UIControlState.Normal);
            GetHelp.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            GetHelp.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);

            GetHelp.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HowToConnectGetHelp.ToString());

                NavigationController.PushViewController(new TechnicalSupportScreen(), true);
            };

            SlideWrapper.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);

            SlideWrapper.Add(_MainHeader);
            SlideWrapper.Add(_SubHeader);
            SlideWrapper.Add(extraInfo);
            SlideWrapper.Add(hotspotFinder);
            SlideWrapper.Add(hotspotFinderUnderline);
            SlideWrapper.Add(WhiteStripWrapper);
            SlideWrapper.Add(WhiteStripWrapperOne);
            SlideWrapper.Add(GotIt);
            SlideWrapper.Add(GetHelp);
            
            return SlideWrapper;
        }

        public UIView CarouselNavigation(UIScrollView carousel)
        {
            var NavigationWrapper = new UIView(new CGRect(0, UtilProvider.ScreenHeight - 65 - UtilProvider.SafeBottom, UtilProvider.ScreenWidth, 50));
            
            _LeftArrowBtn = new MDButton(new CGRect(UtilProvider.ScreenWidth / 4, -2, 18, 18), MDButtonType.Flat, UIColor.White);
            _LeftArrowBtn.BackgroundColor = UIColor.Clear;
            
            _LeftArrow = new UISvgImageView("svg/prev_btn.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth / 4, -2, 18, 18),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Alpha = 0
            };

            _RightArrowBtn = new MDButton(new CGRect(UtilProvider.ScreenWidth - 90, -2, 18, 18), MDButtonType.Flat, UIColor.White);
            _RightArrowBtn.BackgroundColor = UIColor.Clear;

            _RightArrow = new UISvgImageView("svg/next_btn.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth - 90, -2, 18, 18),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            var pagerWrapper = new UIView(new CGRect((UtilProvider.ScreenWidth / 2) - 55, 3, 120, 10));
            
            _slide1 = new UISvgImageView("svg/circle.svg")
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(15, 0, 10, 10)
            };

            _slide2 = new UISvgImageView("svg/circle.svg")
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(40, 0, 10, 10),
                Alpha = 0.5f
            };

            _slide3 = new UISvgImageView("svg/circle.svg")
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(65, 0, 10, 10),
                Alpha = 0.5f
            };

            _slide4 = new UISvgImageView("svg/circle.svg")
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(87, 0, 10, 10),
                Alpha = 0.5f
            };
            
            UITapGestureRecognizer slideOneTap = new UITapGestureRecognizer(() =>
            {

                carousel.SetContentOffset(new CGPoint(0, 0), true);
                _LeftArrowBtn.Alpha = 0;
                _LeftArrow.Alpha = 0;
                _RightArrow.Alpha = 1;
                _RightArrowBtn.Alpha = 1;
                _slide1.Alpha = 1;
                _slide2.Alpha = 0.5f;
                _slide3.Alpha = 0.5f;
            });

            _slide1.UserInteractionEnabled = true;
            _slide1.AddGestureRecognizer(slideOneTap);
            
            UITapGestureRecognizer slideTwoTap = new UITapGestureRecognizer(() =>
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
            });

            _slide2.UserInteractionEnabled = true;
            _slide2.AddGestureRecognizer(slideTwoTap);
            
            UITapGestureRecognizer slideThreeTap = new UITapGestureRecognizer(
                () =>
                {
                    carousel.SetContentOffset(new CGPoint(UtilProvider.ScreenWidth * 2, 0), true);
                    //ScreenWidth
                    _LeftArrowBtn.Alpha = 1;
                    _LeftArrow.Alpha = 1;
                    _RightArrow.Alpha = 1;
                    _RightArrowBtn.Alpha = 1;
                    _slide1.Alpha = 0.5f;
                    _slide2.Alpha = 0.5f;
                    _slide3.Alpha = 1;
                    _slide4.Alpha = 0.5f;
                }
            );

            _slide3.UserInteractionEnabled = true;
            _slide3.AddGestureRecognizer(slideThreeTap);

            UITapGestureRecognizer slideFourTap = new UITapGestureRecognizer(
                () =>
                {
                    carousel.SetContentOffset(new CGPoint(UtilProvider.ScreenWidth * 3, 0), true);
                    _LeftArrow.Alpha = 1;
                    _LeftArrowBtn.Alpha = 1;
                    _RightArrow.Alpha = 0;
                    _slide1.Alpha = 0.5f;
                    _slide2.Alpha = 0.5f;
                    _slide3.Alpha = 0.5f;
                    _slide4.Alpha = 1;
                }
            );

            _slide4.UserInteractionEnabled = true;
            _slide4.AddGestureRecognizer(slideFourTap);
            
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
                else if (carousel.ContentOffset.X == (UtilProvider.ScreenWidth * 3))
                {
                    carousel.SetContentOffset(new CGPoint(UtilProvider.ScreenWidth * 2, 0), true);
                }
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
                else if (carousel.ContentOffset.X == UtilProvider.ScreenWidth * 2)
                {
                    carousel.SetContentOffset(new CGPoint(UtilProvider.ScreenWidth * 3, 0), true);
                }
            };

            pagerWrapper.Add(_slide1);
            pagerWrapper.Add(_slide2);
            pagerWrapper.Add(_slide3);
            pagerWrapper.Add(_slide4);

            NavigationWrapper.Add(_LeftArrow);
            NavigationWrapper.Add(_LeftArrowBtn);
            NavigationWrapper.Add(_RightArrow);
            NavigationWrapper.Add(_RightArrowBtn);

            NavigationWrapper.Add(pagerWrapper);
            //NavigationWrapper.Add (_utilProvider.ButtonBottomBorder(SkipButton));

            return NavigationWrapper;

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
    }
}