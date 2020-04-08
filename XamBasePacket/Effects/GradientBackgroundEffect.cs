using System;
using System.Linq;
using Xamarin.Forms;

namespace XamBasePacket.Effects
{
    public static class GradientBackground
    {
        public enum Orientation
        {
            TopBottom,
            BottomTop,
            LeftRight,
            RightLeft
        }

        public static readonly BindableProperty OnProperty =
            BindableProperty.CreateAttached(
                propertyName: "On",
                returnType: typeof(bool),
                declaringType: typeof(GradientBackground),
                defaultValue: false,
                propertyChanged: OnOffChanged
            );

        public static void SetOn(BindableObject view, bool value)
        {
            view.SetValue(OnProperty, value);
        }

        public static bool GetOn(BindableObject view)
        {
            return (bool)view.GetValue(OnProperty);
        }

        private static void OnOffChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is Element view))
                return;

            if ((bool)newValue)
            {
                view.Effects.Add(new GradientBackgroundEffect());
            }
            else
            {
                var toRemove = view.Effects.FirstOrDefault(e => e is GradientBackgroundEffect);
                if (toRemove != null)
                    view.Effects.Remove(toRemove);
            }
        }


        public static readonly BindableProperty FromColorProperty =
            BindableProperty.CreateAttached(
                "FromColor",
                typeof(Color),
                typeof(GradientBackground),
                Color.Default
            );

        public static void SetFromColor(BindableObject view, Color value)
        {
            view.SetValue(FromColorProperty, value);
        }

        public static Color GetFromColor(BindableObject view)
        {
            return (Color)view.GetValue(FromColorProperty);
        }

        public static readonly BindableProperty ToColorProperty =
            BindableProperty.CreateAttached(
                "ToColor",
                typeof(Color),
                typeof(GradientBackground),
                Color.Default
            );

        public static void SetToColor(BindableObject view, Color value)
        {
            view.SetValue(ToColorProperty, value);
        }

        public static Color GetToColor(BindableObject view)
        {
            return (Color)view.GetValue(ToColorProperty);
        }

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.CreateAttached(
                "CornerRadius",
                typeof(int),
                typeof(GradientBackground),
                -1
            );

        public static void SetCornerRadius(BindableObject view, int value)
        {
            view.SetValue(CornerRadiusProperty, value);
        }

        public static int GetCornerRadius(BindableObject view)
        {
            return (int)view.GetValue(CornerRadiusProperty);
        }

        public static readonly BindableProperty GradientOrientationProperty =
            BindableProperty.CreateAttached(
                "GradientOrientation",
                typeof(Orientation),
                typeof(GradientBackground),
                default(Orientation)
            );

        public static void SetGradientOrientation(BindableObject view, Orientation value)
        {
            view.SetValue(GradientOrientationProperty, value);
        }

        public static Orientation GetGradientOrientation(BindableObject view)
        {
            return (Orientation)view.GetValue(GradientOrientationProperty);
        }

        public static readonly BindableProperty AnimationDurationProperty =
            BindableProperty.CreateAttached(
                "AnimationDuration",
                typeof(TimeSpan?),
                typeof(GradientBackground),
                null
            );

        public static void SetAnimationDuration(BindableObject view, TimeSpan? value)
        {
            view.SetValue(AnimationDurationProperty, value);
        }

        public static TimeSpan? GetAnimationDuration(BindableObject view)
        {
            return (TimeSpan?)view.GetValue(AnimationDurationProperty);
        }
    }

    class GradientBackgroundEffect : RoutingEffect
    {
        public GradientBackgroundEffect() : base("XamBasePacket.GradientBackgroundPlatformEffect")
        {
        }
    }
}
