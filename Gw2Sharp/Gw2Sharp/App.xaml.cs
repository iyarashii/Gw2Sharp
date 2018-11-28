using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gw2Sharp.Pages;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Gw2Sharp
{
    public partial class App : Application
    {      
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
