﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gw2Sharp.ViewModels;
using System;

namespace Gw2Sharp.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage : ContentPage
    {
        private readonly ConfigurationViewModel ViewModel;

        public Func<bool> CustomBackButtonAction { get; set; }

        // page constructor
        public ConfigurationPage()
		{
            InitializeComponent();
            ViewModel = new ConfigurationViewModel(this);
            BindingContext = ViewModel;
            CustomBackButtonAction = () => OnBackButtonPressed();
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