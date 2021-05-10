using System;
using System.ComponentModel;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamBasePacket.Effects;

[assembly: ExportEffect(typeof(GradientBackgroundPlatformEffect), "GradientBackgroundPlatformEffect")]
namespace XamBasePacket.Effects
{
    public class GradientBackgroundPlatformEffect : PlatformEffect
    {
        private const string GradientLayerTag = "gradientBg";
        private const string ColorAnimationTag = "colorchange";

        private bool _clipsToBounds;
        private UIView CurrentView => Control ?? Container;
        private VisualElement MeasuredView => Element as VisualElement;


        private readonly Type[] _hasBorderTypes = {
            typeof(Entry),
            typeof(DatePicker),
            typeof(TimePicker),
            typeof(Picker),
        };

        protected override void OnAttached()
        {
            _clipsToBounds = CurrentView.ClipsToBounds;
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
            if (CurrentView != null)
                CurrentView.ClipsToBounds = _clipsToBounds;
            if (_hasBorderTypes.Any(x => x == Element.GetType()) && CurrentView is UITextField textField)
            {
                textField.BorderStyle = UITextBorderStyle.RoundedRect;
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
            if (args.PropertyName == GradientBackground.FromColorProperty.PropertyName
                || args.PropertyName == GradientBackground.ToColorProperty.PropertyName
                || args.PropertyName == GradientBackground.CornerRadiusProperty.PropertyName
                || args.PropertyName == GradientBackground.GradientOrientationProperty.PropertyName
                || args.PropertyName == GradientBackground.OnProperty.PropertyName
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
            SetBorder(gradientLayer);
            if (layer == null)
                CurrentView?.Layer?.InsertSublayer(gradientLayer, 0);
        }
        private void SetBorder(CALayer layer)
        {
            if (layer == null)
                return;
            var borderType = GradientBackground.GetBorderType(Element);
            var borderColor = GradientBackground.GetBorderColor(Element);
            var borderWidth = GradientBackground.GetBorderWidth(Element);
            switch (borderType)
            {
                case GradientBackground.BorderType.Solid:
                    LayerBorder(layer, borderWidth, borderColor);
                    break;
                case GradientBackground.BorderType.Dotted:
                    LayerBorder(layer, borderWidth, borderColor);
                    break;
                case GradientBackground.BorderType.None:
                default:
                    break;
            }
        }

        private void LayerBorder(CALayer layer, double width, Color color)
        {
            if (_hasBorderTypes.Any(x => x == Element.GetType()) && CurrentView is UITextField textField)
            {
                textField.BorderStyle = UITextBorderStyle.None;
            }
            layer.BorderWidth = (nfloat)width;
            layer.BorderColor = color.ToCGColor();
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
                case GradientBackground.Orientation.TlBr:
                    return (new CGPoint(0, 0), new CGPoint(1, 1));
                case GradientBackground.Orientation.TrBl:
                    return (new CGPoint(1, 0), new CGPoint(0, 1));
                case GradientBackground.Orientation.BlTr:
                    return (new CGPoint(0, 1), new CGPoint(1, 0));
                case GradientBackground.Orientation.BrTl:
                    return (new CGPoint(1, 1), new CGPoint(0, 0));
                default:
                    return (new CGPoint(), new CGPoint());
            }
        }

    }

}
