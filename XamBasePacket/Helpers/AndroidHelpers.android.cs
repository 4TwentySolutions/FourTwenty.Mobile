using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using Google.Android.Material.BottomNavigation;
using Java.Lang;
using Xamarin.Forms.Platform.Android;
using Exception = System.Exception;

namespace XamBasePacket.Helpers
{
    public static class AndroidHelpers
    {
        public static void SetShiftMode(this BottomNavigationView bottomNavigationView, bool enableShiftMode, bool enableItemShiftMode)
        {
            try
            {
                var menuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
                if (menuView == null)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to find BottomNavigationMenuView");
                    return;
                }

                for (int i = 0; i < menuView.ChildCount; i++)
                {
                    var item = menuView.GetChildAt(i) as BottomNavigationItemView;
                    if (item == null)
                        continue;

                    item.SetShifting(enableItemShiftMode);
                    item.SetChecked(item.ItemData.IsChecked);

                }

                menuView.UpdateMenuView();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to set shift mode: {ex}");
            }
        }

        /// <summary>
        /// Device Independent Pixels to Actual Pixles conversion
        /// </summary>
        /// <param name="context"></param>
        /// <param name="valueInDp"></param>
        /// <returns></returns>
        public static float DpToPixels(Android.Content.Context context, float valueInDp)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }

        /// <summary>
        /// Set backgroundTint to view across all targeting platform level.
        /// </summary>
        /// <param name="view">View to tint</param>
        /// <param name="color">Color used to tint</param>
        public static void TintView(View view, int color)
        {
            Drawable d = view.Background;
            Drawable nd = d.GetConstantState().NewDrawable().Mutate();
            nd.SetColorFilter(AppCompatDrawableManager.GetPorterDuffColorFilter(
                color, PorterDuff.Mode.SrcIn));
            view.SetBackground(nd);
        }


        public static void SetCursorDrawableColor(EditText editText, Color color)
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                {
                    var gradientDrawable = new GradientDrawable(GradientDrawable.Orientation.BottomTop, new[] { (int)color, color });
                    gradientDrawable.SetSize(SpToPx(2, editText.Context), (int)editText.TextSize);
                    editText.TextCursorDrawable = gradientDrawable;
                    return;
                }

                var fCursorDrawableRes =
                    Class.FromType(typeof(TextView)).GetDeclaredField("mCursorDrawableRes");
                fCursorDrawableRes.Accessible = true;
                int mCursorDrawableRes = fCursorDrawableRes.GetInt(editText);
                var fEditor = Class.FromType(typeof(TextView)).GetDeclaredField("mEditor");
                fEditor.Accessible = true;
                Java.Lang.Object editor = fEditor.Get(editText);
                Class clazz = editor.Class;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                {
                    //TODO This solution no longer works in Android P because of reflection
                    // Get the drawable and set a color filter
                    Drawable drawable = ContextCompat.GetDrawable(editText.Context, mCursorDrawableRes);
                    drawable.SetColorFilter(color, PorterDuff.Mode.SrcIn);
                    var fCursorDrawable = clazz.GetDeclaredField("mDrawableForCursor");
                    fCursorDrawable.Accessible = true;
                    fCursorDrawable.Set(editor, drawable);
                }
                else
                {
                    Drawable[] drawables = new Drawable[2];
                    drawables[0] = ContextCompat.GetDrawable(editText.Context, mCursorDrawableRes).Mutate();
                    drawables[1] = ContextCompat.GetDrawable(editText.Context, mCursorDrawableRes).Mutate();
                    drawables[0].SetColorFilter(color, PorterDuff.Mode.SrcIn);
                    drawables[1].SetColorFilter(color, PorterDuff.Mode.SrcIn);

                    var fCursorDrawable = clazz.GetDeclaredField("mCursorDrawable");
                    fCursorDrawable.Accessible = true;
                    fCursorDrawable.Set(editor, drawables);
                }
            }
            catch (ReflectiveOperationException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to set CursorDrawableColor: {ex}");
            }
        }
        public static int SpToPx(float sp, Context context)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, sp, context.Resources.DisplayMetrics);
        }
        public static void SetHandlesColor(EditText editText, Color color)
        {

            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                {
                    var size = SpToPx(22, editText.Context);
                    var corner = size / 2f;
                    var inset = SpToPx(10, editText.Context);

                    //left drawable
                    var drLeft = new GradientDrawable(GradientDrawable.Orientation.BottomTop, new[] { (int)color, color });
                    drLeft.SetSize(size, size);
                    drLeft.SetCornerRadii(new[] { corner, corner, 0f, 0f, corner, corner, corner, corner });
                    editText.TextSelectHandleLeft = new InsetDrawable(drLeft, inset, 0, inset, inset);

                    //right drawable
                    var drRight = new GradientDrawable(GradientDrawable.Orientation.BottomTop, new[] { (int)color, color });
                    drRight.SetSize(size, size);
                    drRight.SetCornerRadii(new[] { 0f, 0f, corner, corner, corner, corner, corner, corner });
                    editText.TextSelectHandleRight = new InsetDrawable(drRight, inset, 0, inset, inset);

                    //middle drawable
                    var drMiddle = new GradientDrawable(GradientDrawable.Orientation.BottomTop, new[] { (int)color, color });
                    drMiddle.SetSize(size, size);
                    drMiddle.SetCornerRadii(new[] { 0f, 0f, corner, corner, corner, corner, corner, corner });
                    var mInset = (int)(System.Math.Sqrt(2f) * corner - corner);
                    var insetDrawable = new InsetDrawable(drMiddle, mInset, mInset, mInset, mInset);
                    var rotateDrawable = new RotateDrawable();
                    rotateDrawable.Drawable = insetDrawable;
                    rotateDrawable.ToDegrees = 45f;
                    rotateDrawable.SetLevel(10000);
                    editText.TextSelectHandle = rotateDrawable;
                    return;
                }

                var editorField = Class.FromType(typeof(TextView)).GetDeclaredField("mEditor");
                if (!editorField.Accessible)
                    editorField.Accessible = true;

                var editor = editorField.Get(editText);
                var editorClass = editor.Class;
                string[] handleNames = { "mSelectHandleLeft", "mSelectHandleRight", "mSelectHandleCenter" };
                string[] resNames = { "mTextSelectHandleLeftRes", "mTextSelectHandleRightRes", "mTextSelectHandleRes" };

                for (int i = 0; i < handleNames.Length; i++)
                {
                    var handleField = editorClass.GetDeclaredField(handleNames[i]);
                    if (!handleField.Accessible)
                    {
                        handleField.Accessible = true;
                    }
                    Drawable handleDrawable = (Drawable)handleField.Get(editor);

                    if (handleDrawable == null)
                    {
                        var resField = Class.FromType(typeof(TextView)).GetDeclaredField(resNames[i]);
                        if (!resField.Accessible)
                        {
                            resField.Accessible = true;

                        }
                        int resId = resField.GetInt(editText);
                        handleDrawable = ContextCompat.GetDrawable(editText.Context, resId);

                    }

                    if (handleDrawable != null)
                    {
                        Drawable drawable = handleDrawable.Mutate();
                        drawable.SetColorFilter(color, PorterDuff.Mode.SrcIn);
                        handleField.Set(editor, drawable);
                    }
                }
            }
            catch (ReflectiveOperationException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to set HandlesColor: {ex}");
            }
        }
    }
}