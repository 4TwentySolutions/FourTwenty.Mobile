using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xamarin.Forms.Platform.Android;

namespace XamBasePacket.Effects
{
    public abstract class BaseEffect : PlatformEffect
    {
        public static bool IsFastRenderers = true;

        IVisualElementRenderer _renderer;
        bool _isDisposed = false;
        protected bool IsDisposed
        {
            get
            {
                if (_isDisposed)
                {
                    return _isDisposed;
                }

                if (_renderer == null)
                {
                    _renderer = (Container ?? Control) as IVisualElementRenderer;
                }

                if (IsFastRendererButton)
                {
                    return CheckButtonIsDisposed();
                }

                return _isDisposed = _renderer?.Tracker == null; //disposed check
            }
        }

        // whether Element is FastRenderer.(Exept Button)
        //If Container is null, it regards this as FastRenderer Element.
        //But this judging may not become right in the future. 
        protected bool IsFastRenderer => IsFastRenderers && (Container == null && !(Element is Xamarin.Forms.Button));

        // whether Element is a Button of FastRenderer.
        protected bool IsFastRendererButton => (IsFastRenderers && (Element is Xamarin.Forms.Button));

        // whether Element can add ClickListener.
        protected bool IsClickable => !(IsFastRenderer || Element is Xamarin.Forms.Layout || Element is Xamarin.Forms.BoxView);

        static Func<object, object> _getDisposed; //cache

        // In case Button of FastRenderer, IVisualElementRenderer.Tracker don't become null.
        // So refered to the private field of "_disposed", judge whether being disposed. 
        bool CheckButtonIsDisposed()
        {
            if (_getDisposed == null)
            {
                _getDisposed = CreateGetField(typeof(VisualElementTracker));
            }
            _isDisposed = (bool)_getDisposed(_renderer.Tracker);

            return _isDisposed;
        }

        Func<object, object> CreateGetField(Type t)
        {
            var prop = t.GetRuntimeFields().FirstOrDefault(x => x.DeclaringType == t && x.Name == "_disposed");
            if (prop == null)
                return null;

            var target = Expression.Parameter(typeof(object), "target");

            var body = Expression.PropertyOrField(Expression.Convert(target, t), prop.Name);

            var lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(body, typeof(object)), target
            );

            return lambda.Compile();
        }
    }
}