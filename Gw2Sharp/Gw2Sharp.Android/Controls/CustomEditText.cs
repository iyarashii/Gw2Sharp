using Android.Content;
using Xamarin.Forms.Platform.Android;

namespace Gw2Sharp.Droid.Controls
{
    // edit text custom renderer for Android
    public class CustomEditText : FormsEditText
    {
        public CustomEditText(Context context) : base(context)
        {
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
        }
        
    }

}