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

        [ObservableProperty]
        private string _errorMSG;


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
            try
            {
                var user = AuthService.Instance.Login(email, password);
                if (user != null)
                {
                    UserService.Instance.SetCurrentUser(user);
                    if (user.isAdmin == true)
                    {
                        await Shell.Current.GoToAsync("//adminhome");
                        return;
                    }
                    else
                        await Shell.Current.GoToAsync("//home");
                }
                else
                {
                    ErrorMSG = "Incorrect email or password";
                    Debug.WriteLine("Incorrect email or password");
                }
            }
            catch (Exception ex)
            {
                ErrorMSG = ex.Message;
                Debug.WriteLine(ex.Message);
            }


        }




        #region GoogleSignIn


     
        #endregion
    }
}
