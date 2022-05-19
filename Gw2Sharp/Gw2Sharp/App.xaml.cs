// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gw2Sharp.Views.Pages;
using Gw2Sharp.Models;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Gw2Sharp
{
    public partial class App : Application
    {
        // local Database field
        static Database database;

        // Database property that creates a new Database instance as a singleton
        public static Database Database
        {
            get
            {
                if(database == null)
                {
                    database = new Database(Constants.ItemDBPath);
                }
                return database;
            }
        }

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
