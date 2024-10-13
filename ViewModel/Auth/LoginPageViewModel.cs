using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;


namespace MAUIRecipeApp.ViewModel.Auth
{
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly IConfiguration _configuration;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;


        public LoginPageViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [RelayCommand]
        private async Task SignUp()
        {
            await Shell.Current.GoToAsync("//signup");
        }

        [RelayCommand]
        private async Task ForgotPassword()
        {
            await Shell.Current.GoToAsync("//forgotpass");
        }

        [RelayCommand]
        private async Task Login()
        {
            if (email == "admin" && password == "admin")
            {
                await Shell.Current.GoToAsync("//adminhome");
                return;
            }
            await Shell.Current.GoToAsync("//home");
        }




        #region GoogleSignIn


     
        #endregion
    }
}
