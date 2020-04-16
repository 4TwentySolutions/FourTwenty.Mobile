using System;
using System.ComponentModel;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
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
        private RippleDrawable _ripple;
        private bool _isClippedToBounds;

        protected override void OnAttached()
        {
            _oldDrawable = View.Background;
            _isClippedToBounds = View.ClipToOutline;
            SetUpBackground();

        }

        protected override void OnDetached()
        {
            if (_oldDrawable != null)
                ViewCompat.SetBackground(View, _oldDrawable);
            _ripple?.Dispose();
            _ripple = null;
            View.ClipToOutline = _isClippedToBounds;
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
                SetBackground(transitionDrawable, (int)radius);
                transitionDrawable.StartTransition((int)animationDuration.Value.TotalMilliseconds);
            }
            else
            {
                SetBackground(gd, (int)radius);
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


        private void SetBackground(Drawable drawable, int radius)
        {
            var rippleColor = GradientBackground.GetRippleColor(Element);
            if (rippleColor != Color.Default)
            {
                var rippleDrawable = CreateRipple(drawable, rippleColor.ToAndroid(), radius);
                drawable = new LayerDrawable(new[] { drawable, rippleDrawable });
            }
            ViewCompat.SetBackground(View, drawable);
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
                case GradientBackground.Orientation.TlBr:
                    return GradientDrawable.Orientation.TlBr;
                case GradientBackground.Orientation.TrBl:
                    return GradientDrawable.Orientation.TrBl;
                case GradientBackground.Orientation.BlTr:
                    return GradientDrawable.Orientation.BlTr;
                case GradientBackground.Orientation.BrTl:
                    return GradientDrawable.Orientation.BrTl;
                default:
                    return GradientDrawable.Orientation.TopBottom;
            }
        }



        #region Ripple

        private RippleDrawable CreateRipple(Drawable background, Android.Graphics.Color color, int radius)
        {
            if (Element is Layout)
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, GetRippleMask(Android.Graphics.Color.White, radius));

            var back = background;
            if (back == null)
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, GetRippleMask(Android.Graphics.Color.White, radius));


            if (back is RippleDrawable drawable)
            {
                _ripple = drawable;
                _ripple.SetColor(GetPressedColorSelector(color));
                _ripple.Radius = radius;
                return _ripple;
            }

            return _ripple = new RippleDrawable(GetPressedColorSelector(color), back, null);
        }

        static ColorStateList GetPressedColorSelector(Android.Graphics.Color pressedColor) => ColorStateList.ValueOf(pressedColor);


        private static Drawable GetRippleMask(Android.Graphics.Color color, int radius)
        {
            float[] outerRadii = new float[8];
            // 3 is radius of final ripple, 
            // instead of 3 you can give required final radius
            //Arrays.fill(outerRadii, 3);
            Array.Fill(outerRadii, radius);

            RoundRectShape r = new RoundRectShape(outerRadii, null, null);
            ShapeDrawable shapeDrawable = new ShapeDrawable(r);
            shapeDrawable.Paint.Color = color;
            return shapeDrawable;
        }

        #endregion

    }
}