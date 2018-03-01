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
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Text;

namespace AlwaysOn_Droid
{
    public class AndroidAlert
    {
        public static AlwaysOnAlertDialog ShowAlert(Context context, string title, string message, params string[] buttons)
        {
            AlertDialog alertDialog = null;
            
            if (!((Activity)context).IsFinishing)
            {
                try
                {
                    using (var alert = new AlertDialog.Builder(context))
                    {
                        using (var aoAlert = new AlwaysOnAlertDialog())
                        {
                            int CornerRadius = Utils.CalcDimension(20);

                            var Container = new LinearLayout(context)
                            {
                                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent),
                                Orientation = Orientation.Vertical
                            };
                            var pd = new PaintDrawable(Color.White);
                            pd.SetCornerRadius(CornerRadius);
                            Container.Background = pd;

                            var txtTitle = new CustomTextView(context)
                            {
                                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent),
                                TextSize = 20,
                                Gravity = GravityFlags.Center,
                                TextAlignment = TextAlignment.Center,
                                Text = title
                            };
                            txtTitle.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(50), Utils.CalcDimension(30), 0);
                            txtTitle.SetTextColor(Color.Black);
                            txtTitle.SetCustomFont(context.GetString(Resource.String.fontBold));
                            Container.AddView(txtTitle);

                            var txtMessage = new CustomTextView(context)
                            {
                                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
                                {
                                    TopMargin = 0,
                                    BottomMargin = Utils.CalcDimension(50)
                                },
                                TextSize = 15,
                                Gravity = GravityFlags.Center,
                                TextAlignment = TextAlignment.Center
                            };
                            txtMessage.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(30), Utils.CalcDimension(10));
                            txtMessage.SetTextColor(Color.Black);
                            txtMessage.SetCustomFont(context.GetString(Resource.String.fontRegular));
                            txtMessage.SetText(fromHtml(message), TextView.BufferType.Spannable);
                            Container.AddView(txtMessage);

                            var ButtonContainer = new LinearLayout(context)
                            {
                                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent),
                                Orientation = buttons.Length == 2 ? Orientation.Horizontal : Orientation.Vertical,
                                WeightSum = buttons.Length == 2 ? 2 : 0
                            };
                            ButtonContainer.SetBackgroundColor(Color.Black);

                            for (int i = 0; i < buttons.Length; i++)
                            {
                                var Button = new LinearLayout(context)
                                {
                                    LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
                                    {
                                        LeftMargin = 0,
                                        RightMargin = buttons.Length == 2 && i == 0 ? 1 : 0,
                                        TopMargin = 1, //Divide Buttons with line on top
                                        BottomMargin = 0,
                                        Weight = buttons.Length == 2 ? 1 : 0
                                    },
                                    TextAlignment = TextAlignment.Center,
                                    Tag = i
                                };
                                Button.SetBackgroundColor(Color.White);
                                Button.SetPadding(0, Utils.CalcDimension(25), 0, Utils.CalcDimension(25));

                                if (buttons.Length == 2)
                                {
                                    var roundedCorners = new PaintDrawable(Color.White);
                                    roundedCorners.SetCornerRadii((i == 0) ? new float[] { 0, 0, 0, 0, 0, 0, CornerRadius, CornerRadius } : new float[] { 0, 0, 0, 0, CornerRadius, CornerRadius, 0, 0 });
                                    Button.Background = roundedCorners;
                                }
                                else
                                {
                                    if (i + 1 == buttons.Length)
                                    {
                                        var roundedCorners = new PaintDrawable(Color.White);
                                        roundedCorners.SetCornerRadii(new float[] { 0, 0, 0, 0, CornerRadius, CornerRadius, CornerRadius, CornerRadius });
                                        Button.Background = roundedCorners;
                                    }
                                }

                                var ButtonText = new CustomTextView(context)
                                {
                                    LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent),
                                    TextSize = 18,
                                    Gravity = GravityFlags.Center,
                                    TextAlignment = TextAlignment.Center,
                                    Text = buttons[i]
                                };
                                ButtonText.SetTextColor(Color.DodgerBlue);
                                ButtonText.SetCustomFont(context.GetString(Resource.String.fontRegular));

                                Button.AddView(ButtonText);
                                Button.Click += (object sender, EventArgs e) =>
                                {
                                    aoAlert.OnButtonClicked(sender, new AlwaysOnAlertDialogArgs() { alertDialog = alertDialog, which = (int)((LinearLayout)sender).Tag });
                                };

                                ButtonContainer.AddView(Button);
                            }

                            Container.AddView(ButtonContainer);

                            alert.SetView(Container);
                            alertDialog = alert.Create();

                            alertDialog.KeyPress += (object sender, DialogKeyEventArgs e) =>
                            {
                                if(e.KeyCode == Keycode.Back)
                                {
                                    aoAlert.OnButtonClicked(null, new AlwaysOnAlertDialogArgs() { alertDialog = alertDialog, which = buttons.Length - 1 });
                                }
                            };

                            aoAlert.SetAlertDialog(alertDialog);

                            return aoAlert;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return null;
        }

        public static AlwaysOnAlertDialog ShowAlertWithTextField(Context context, string title, string message, bool passwordTextfield, params string[] buttons)
        {
            AlertDialog alertDialog = null;

            if (!((Activity)context).IsFinishing)
            {
                try
                {
                    using (var alert = new AlertDialog.Builder(context))
                    {
                        using (var aoAlert = new AlwaysOnAlertDialog())
                        {
                            int CornerRadius = Utils.CalcDimension(20);
                
                            var Container = new LinearLayout(context)
                            {
                                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent),
                                Orientation = Orientation.Vertical
                            };
                            var pd = new PaintDrawable(Color.White);
                            pd.SetCornerRadius(CornerRadius);
                            Container.Background = pd;

                            var txtTitle = new CustomTextView(context)
                            {
                                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent),
                                TextSize = 20,
                                Gravity = GravityFlags.Center,
                                TextAlignment = TextAlignment.Center,
                                Text = title
                            };
                            txtTitle.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(50), Utils.CalcDimension(30), 0);
                            txtTitle.SetTextColor(Color.Black);
                            txtTitle.SetCustomFont(context.GetString(Resource.String.fontBold));
                            Container.AddView(txtTitle);

                            var txtMessage = new CustomTextView(context)
                            {
                                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
                                {
                                    TopMargin = 0,
                                    BottomMargin = Utils.CalcDimension(50)
                                },
                                TextSize = 15,
                                Gravity = GravityFlags.Center,
                                TextAlignment = TextAlignment.Center
                            };
                            txtMessage.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(30), Utils.CalcDimension(10));
                            txtMessage.SetTextColor(Color.Black);
                            txtMessage.SetCustomFont(context.GetString(Resource.String.fontRegular));
                            txtMessage.SetText(fromHtml(message), TextView.BufferType.Spannable);
                            Container.AddView(txtMessage);

                            var textInput = new CustomEditText(context)
                            {
                                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
                                {
                                    LeftMargin = Utils.CalcDimension(40),
                                    TopMargin = 0,
                                    RightMargin = Utils.CalcDimension(40),
                                    BottomMargin = Utils.CalcDimension(50)
                                },
                                InputType = passwordTextfield ? InputTypes.ClassText | InputTypes.TextVariationPassword : InputTypes.TextVariationNormal,
                                Hint = passwordTextfield ? "Password" : "",
                                ShowSoftInputOnFocus = true
                            };
                            textInput.SetTextColor(Color.Black);
                            textInput.SetCustomFont(context.GetString(Resource.String.fontRegular));
                            textInput.SetHintTextColor(Color.LightGray);
                            var textBoxDrawable = new PaintDrawable(Color.Black);
                            textBoxDrawable.Paint.StrokeWidth = 1;
                            textBoxDrawable.Paint.SetStyle(Paint.Style.Stroke);
                            textBoxDrawable.SetCornerRadius(14);
                            textInput.Background = textBoxDrawable;
                            //textInput.SetHintTextColor(Color.Yellow);
                            Container.AddView(textInput);

                            var ButtonContainer = new LinearLayout(context)
                            {
                                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent),
                                Orientation = buttons.Length == 2 ? Orientation.Horizontal : Orientation.Vertical,
                                WeightSum = buttons.Length == 2 ? 2 : 0
                            };
                            ButtonContainer.SetBackgroundColor(Color.Black);

                            for (int i = 0; i < buttons.Length; i++)
                            {
                                var Button = new LinearLayout(context)
                                {
                                    LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
                                    {
                                        LeftMargin = 0,
                                        RightMargin = buttons.Length == 2 && i == 0 ? 1 : 0,
                                        TopMargin = 1, //Divide Buttons with line on top
                                        BottomMargin = 0,
                                        Weight = buttons.Length == 2 ? 1 : 0
                                    },
                                    TextAlignment = TextAlignment.Center,
                                    Tag = i
                                };
                                Button.SetBackgroundColor(Color.White);
                                Button.SetPadding(0, Utils.CalcDimension(25), 0, Utils.CalcDimension(25));

                                if (buttons.Length == 2)
                                {
                                    var roundedCorners = new PaintDrawable(Color.White);
                                    roundedCorners.SetCornerRadii((i == 0) ? new float[] { 0, 0, 0, 0, 0, 0, CornerRadius, CornerRadius } : new float[] { 0, 0, 0, 0, CornerRadius, CornerRadius, 0, 0 });
                                    Button.Background = roundedCorners;
                                }
                                else
                                {
                                    if (i + 1 == buttons.Length)
                                    {
                                        var roundedCorners = new PaintDrawable(Color.White);
                                        roundedCorners.SetCornerRadii(new float[] { 0, 0, 0, 0, CornerRadius, CornerRadius, CornerRadius, CornerRadius });
                                        Button.Background = roundedCorners;
                                    }
                                }

                                var ButtonText = new CustomTextView(context)
                                {
                                    LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent),
                                    TextSize = 18,
                                    Gravity = GravityFlags.Center,
                                    TextAlignment = TextAlignment.Center,
                                    Text = buttons[i]
                                };
                                ButtonText.SetTextColor(Color.DodgerBlue);
                                ButtonText.SetCustomFont(context.GetString(Resource.String.fontRegular));

                                Button.AddView(ButtonText);
                                Button.Click += (object sender, EventArgs e) =>
                                {
                                    aoAlert.OnButtonClicked(sender, new AlwaysOnAlertDialogArgs() { alertDialog = alertDialog, which = (int)((LinearLayout)sender).Tag, TextFieldText = textInput.Text.ToString().Trim() });
                                };

                                ButtonContainer.AddView(Button);
                            }

                            Container.AddView(ButtonContainer);

                            alert.SetView(Container);
                            alertDialog = alert.Create();

                            aoAlert.SetAlertDialog(alertDialog);

                            return aoAlert;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return null;
        }

        public static ISpanned fromHtml(String source)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return Html.FromHtml(source, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                return Html.FromHtml(source);
            }
        }
    }

    public class AlwaysOnAlertDialog : IDisposable
    {
        public event EventHandler<AlwaysOnAlertDialogArgs> ButtonClicked;
        public void OnButtonClicked(object sender, AlwaysOnAlertDialogArgs e)
        {
            Hide();

            if (ButtonClicked != null)
                ButtonClicked(sender, e);
        }

        private AlertDialog alertDialog { get; set; }

        public void SetAlertDialog(AlertDialog AlertDialog)
        {
            alertDialog = AlertDialog;
        }
        
        public void Show()
        {
            alertDialog.Show();
        }

        private void Hide()
        {
            try
            {
                alertDialog.Dismiss();
                alertDialog.Dispose();
            } catch (Exception ex)
            {

            }
            finally
            {
                alertDialog = null;
            }
        }

        public void Dispose()
        {
        }
    }

    public class AlwaysOnAlertDialogArgs
    {
        public AlertDialog alertDialog { get; set; }
        public int which { get; set; }
        public string TextFieldText { get; set; } = null;
    }
}