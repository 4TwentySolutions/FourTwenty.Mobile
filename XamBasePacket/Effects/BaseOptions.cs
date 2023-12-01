using System.Linq;
using Xamarin.Forms;

[assembly: ResolutionGroupName("XamBasePacket.Effects")]
namespace XamBasePacket.Effects
{
    public static class BaseOptions<TEffect> where TEffect : RoutingEffect, new()
    {
        public static readonly BindableProperty OnProperty =
            BindableProperty.CreateAttached(
                propertyName: "On",
                returnType: typeof(bool),
                declaringType: typeof(BaseOptions<TEffect>),
                defaultValue: false,
                propertyChanged: OnOffChanged
            );


        private static void OnOffChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is VisualElement view))
                return;

            if ((bool)newValue)
            {
                view.Effects.Add(new TEffect());
            }
            else
            {
                var toRemove = view.Effects.FirstOrDefault(e => e is TEffect);
                if (toRemove != null)
                    view.Effects.Remove(toRemove);
            }
        }
    }
}
