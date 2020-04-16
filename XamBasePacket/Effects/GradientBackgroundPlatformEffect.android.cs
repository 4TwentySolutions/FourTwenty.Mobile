using System;
using System.ComponentModel;
using Android.Graphics.Drawables;
using Android.OS;
using AndroidX.Core.View;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamBasePacket.Effects;
using XamBasePacket.Helpers;

[assembly: ExportEffect(typeof(GradientBackgroundPlatformEffect), "GradientBackgroundPlatformEffect")]
namespace XamBasePacket.Effects
{
    public class GradientBackgroundPlatformEffect : BaseEffect
    {
        private Android.Views.View View => Control ?? Container;
        private Drawable _oldDrawable;

        protected override void OnAttached()
        {
            _oldDrawable = View.Background;
            SetUpBackground();

        }

        protected override void OnDetached()
        {
            if (_oldDrawable != null)
                ViewCompat.SetBackground(View, _oldDrawable);
        }


        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
            if (args.PropertyName == GradientBackground.FromColorProperty.PropertyName
                || args.PropertyName == GradientBackground.ToColorProperty.PropertyName
                || args.PropertyName == GradientBackground.CornerRadiusProperty.PropertyName
                || args.PropertyName == GradientBackground.GradientOrientationProperty.PropertyName
                || args.PropertyName == GradientBackground.OnProperty.PropertyName
                || args.PropertyName == GradientBackground.AnimationDurationProperty.PropertyName
                || args.PropertyName == GradientBackground.BorderColorProperty.PropertyName
                || args.PropertyName == GradientBackground.BorderTypeProperty.PropertyName
                || args.PropertyName == GradientBackground.BorderWidthProperty.PropertyName)
                SetUpBackground();
        }


        private void SetUpBackground()
        {
            Color fromColor = GradientBackground.GetFromColor(Element);
            Color toColor = GradientBackground.GetToColor(Element);
            var formsBack = (Element as VisualElement)?.BackgroundColor;
            if (fromColor == Color.Default
                && toColor == Color.Default
                && formsBack.HasValue
                && formsBack != Color.Default)
                fromColor = toColor = formsBack.Value;

            GradientBackground.Orientation orientation = GradientBackground.GetGradientOrientation(Element);
            GradientDrawable gd = new GradientDrawable(ToOrientation(orientation), new int[] { fromColor.ToAndroid(), toColor.ToAndroid() });
            SetBorder(gd);
            var radius = AndroidHelpers.DpToPixels(View.Context,
                Convert.ToSingle(GradientBackground.GetCornerRadius(Element)));

            gd.SetCornerRadius(radius);
            var animationDuration = GradientBackground.GetAnimationDuration(Element);
            if (animationDuration.HasValue && View.Background != null)
            {
                var transitionDrawable = new TransitionDrawable(new[] { View.Background, gd });
                ViewCompat.SetBackground(View, transitionDrawable);
                transitionDrawable.StartTransition((int)animationDuration.Value.TotalMilliseconds);
            }
            else
            {
                ViewCompat.SetBackground(View, gd);
            }
        }

        private void SetBorder(GradientDrawable drawable)
        {
            var borderType = GradientBackground.GetBorderType(Element);
            var borderColor = GradientBackground.GetBorderColor(Element);
            var borderWidth = AndroidHelpers.DpToPixels(View.Context,
                Convert.ToSingle(GradientBackground.GetBorderWidth(Element)));
            switch (borderType)
            {
                case GradientBackground.BorderType.Solid:
                    drawable.SetStroke((int)borderWidth, borderColor.ToAndroid());
                    View.SetPadding((int)borderWidth, (int)borderWidth, (int)borderWidth, (int)borderWidth);
                    if (Build.VERSION.SdkInt > BuildVersionCodes.Lollipop)
                        View.ClipToOutline = true; //not to overflow children
                    break;
                case GradientBackground.BorderType.Dotted:
                    drawable.SetStroke((int)borderWidth, borderColor.ToAndroid(), 10, 10);
                    View.SetPadding((int)borderWidth, (int)borderWidth, (int)borderWidth, (int)borderWidth);
                    if (Build.VERSION.SdkInt > BuildVersionCodes.Lollipop)
                        View.ClipToOutline = true; //not to overflow children
                    break;
                case GradientBackground.BorderType.None:
                default:
                    break;
            }
        }


        private GradientDrawable.Orientation ToOrientation(GradientBackground.Orientation orientation)
        {
            switch (orientation)
            {
                case GradientBackground.Orientation.BottomTop:
                    return GradientDrawable.Orientation.BottomTop;
                case GradientBackground.Orientation.TopBottom:
                    return GradientDrawable.Orientation.TopBottom;
                case GradientBackground.Orientation.LeftRight:
                    return GradientDrawable.Orientation.LeftRight;
                case GradientBackground.Orientation.RightLeft:
                    return GradientDrawable.Orientation.RightLeft;
                default:
                    return GradientDrawable.Orientation.TopBottom;
            }
        }


    }
}