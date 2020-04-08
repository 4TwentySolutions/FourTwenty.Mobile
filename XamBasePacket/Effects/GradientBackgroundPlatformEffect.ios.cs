using System;
using System.ComponentModel;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Energihjem.Mobile.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamBasePacket.Effects;

[assembly: ExportEffect(typeof(GradientBackgroundPlatformEffect), "GradientBackgroundPlatformEffect")]
namespace Energihjem.Mobile.iOS.Effects
{
    public class GradientBackgroundPlatformEffect : PlatformEffect
    {
        private const string GradientLayerTag = "gradientBg";
        private const string ColorAnimationTag = "colorchange";

        private UIView CurrentView => Control ?? Container;
        private VisualElement MeasuredView => Element as VisualElement;

        protected override void OnAttached()
        {
            SetUpBackground();
            if (MeasuredView != null)
                MeasuredView.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            SetUpBackground();
        }

        protected override void OnDetached()
        {
            if (MeasuredView != null)
                MeasuredView.SizeChanged -= OnSizeChanged;
            var layer = CurrentView?.Layer?.Sublayers?.FirstOrDefault(x => x.Name == GradientLayerTag);
            layer?.RemoveFromSuperLayer();
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
            if (args.PropertyName == GradientBackground.FromColorProperty.PropertyName
                || args.PropertyName == GradientBackground.ToColorProperty.PropertyName
                || args.PropertyName == GradientBackground.CornerRadiusProperty.PropertyName
                || args.PropertyName == GradientBackground.GradientOrientationProperty.PropertyName
                || args.PropertyName == GradientBackground.OnProperty.PropertyName)
                SetUpBackground();
        }

        private void SetUpBackground()
        {
            Color fromColor = GradientBackground.GetFromColor(Element);
            Color toColor = GradientBackground.GetToColor(Element);
            var radius = GradientBackground.GetCornerRadius(Element);
            var height = MeasuredView?.Height;
            var width = MeasuredView?.Width;
            GradientBackground.Orientation orientation = GradientBackground.GetGradientOrientation(Element);
            var (start, end) = ToOrientation(orientation);

            var layer = CurrentView?.Layer?.Sublayers?.FirstOrDefault(x => x.Name == GradientLayerTag);
            //layer?.RemoveFromSuperLayer();


            var gradientLayer = layer as CAGradientLayer ?? new CAGradientLayer();

            gradientLayer.Name = GradientLayerTag;
            gradientLayer.Frame = new CGRect(0, 0, width.GetValueOrDefault(), height.GetValueOrDefault());
            gradientLayer.Colors = new[] { fromColor.ToCGColor(), toColor.ToCGColor() };
            gradientLayer.CornerRadius = radius;
            gradientLayer.StartPoint = start;
            gradientLayer.EndPoint = end;

            var animationDuration = GradientBackground.GetAnimationDuration(Element);
            if (animationDuration.HasValue)
            {
                var oldAnimation = gradientLayer.AnimationForKey(ColorAnimationTag);
                if (oldAnimation != null)
                {
                    if (oldAnimation.Duration != animationDuration.Value.TotalSeconds)
                    {
                        var animation = CABasicAnimation.FromKeyPath(CAAnimation.TransitionFade);
                        animation.Duration = animationDuration.Value.TotalSeconds;
                        gradientLayer.AddAnimation(animation, ColorAnimationTag);
                    }
                }
                else
                {
                    var animation = CABasicAnimation.FromKeyPath(CAAnimation.TransitionFade);
                    animation.Duration = animationDuration.Value.TotalSeconds;
                    gradientLayer.AddAnimation(animation, ColorAnimationTag);
                }
            }
            else
            {
                gradientLayer.RemoveAnimation(ColorAnimationTag);
            }

            if (layer == null)
                CurrentView?.Layer?.InsertSublayer(gradientLayer, 0);
        }


        private (CGPoint start, CGPoint end) ToOrientation(GradientBackground.Orientation orientation)
        {
            switch (orientation)
            {
                case GradientBackground.Orientation.BottomTop:
                    return (new CGPoint(0.5, 1), new CGPoint(0.5, 0));
                case GradientBackground.Orientation.TopBottom:
                    return (new CGPoint(0.5, 0), new CGPoint(0.5, 1));
                case GradientBackground.Orientation.LeftRight:
                    return (new CGPoint(0, .5), new CGPoint(1, .5));
                case GradientBackground.Orientation.RightLeft:
                    return (new CGPoint(1, .5), new CGPoint(0, .5));
                default:
                    return (new CGPoint(), new CGPoint());
            }
        }

    }

}
