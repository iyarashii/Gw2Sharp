using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gw2Sharp.ViewModels;

namespace Gw2Sharp.Views.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConfigurationPage : ContentPage
	{
        ConfigurationViewModel ViewModel = new ConfigurationViewModel();

        // page constructor
        public ConfigurationPage()
		{
            InitializeComponent();
            BindingContext = ViewModel;
		}

        // override that changes back button behavior depending on GettingApiResponses property value
        protected override bool OnBackButtonPressed()
        {
            if (ViewModel.GettingApiResponses)
            {
                ViewModel.ConfigurationStatusText = "Can't go back while getting data from api!";
                return true;
            }
            return false;
        }
    }
}