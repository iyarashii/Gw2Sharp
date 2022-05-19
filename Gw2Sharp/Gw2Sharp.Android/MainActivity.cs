// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using System.Globalization;
using System.Threading;
using Gw2Sharp.Views.Pages;
using System.Linq;

namespace Gw2Sharp.Droid
{
    [Activity(Label = "Gw2Sharp", Icon = "@mipmap/icon", Theme = "@style/splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            // add actionbar to app window
            base.Window.RequestFeature(WindowFeatures.ActionBar);

            // changing theme from splash screen to main
            base.SetTheme(Resource.Style.MainTheme);

            // changing orientation to remove portrait mode lock
            RequestedOrientation = ScreenOrientation.Unspecified;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // needed for OnOptionsItemSelected to work correctly
            Android.Support.V7.Widget.Toolbar toolbar
                = this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        }

        // override used for blocking back navigation button while the database is being downloaded
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // check if the current item id 
            // is equal to the back button id
            if (item.ItemId == 16908332)
            {
                // retrieve the current xamarin forms page instance
                var currentPage = 
                Xamarin.Forms.Application.
                Current.MainPage.Navigation.
                NavigationStack.LastOrDefault() as ConfigurationPage;

                // check if the page has subscribed to 
                // the custom back button event
                if (currentPage?.CustomBackButtonAction != null)
                {
                    // check if GettingApiResponses is true
                    if (currentPage.CustomBackButtonAction())
                    {
                        // and disable the default back button action
                        return false;
                    }

                    // use default back button action if GettingApiResponses is not true
                    return base.OnOptionsItemSelected(item);
                }

                // if its not subscribed then go ahead 
                // with the default back button action
                return base.OnOptionsItemSelected(item);
            }
            else
            {
                // since its not the back button 
                //click, pass the event to the base
                return base.OnOptionsItemSelected(item);
            }
        }
    }
}