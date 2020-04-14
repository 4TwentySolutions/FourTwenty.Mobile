using System;
using System.ComponentModel;
using Android.Graphics.Drawables;
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
                || args.PropertyName == GradientBackground.AnimationDurationProperty.PropertyName)
                SetUpBackground();
        }


        private void SetUpBackground()
        {
            Color fromColor = GradientBackground.GetFromColor(Element);
            Color toColor = GradientBackground.GetToColor(Element);
            var radius = AndroidHelpers.DpToPixels(View.Context,
                Convert.ToSingle(GradientBackground.GetCornerRadius(Element)));

            GradientBackground.Orientation orientation = GradientBackground.GetGradientOrientation(Element);
            GradientDrawable gd = new GradientDrawable(ToOrientation(orientation), new int[] { fromColor.ToAndroid(), toColor.ToAndroid() });
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