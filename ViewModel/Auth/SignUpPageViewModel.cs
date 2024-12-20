using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.Service;
using System.Diagnostics;

namespace MAUIRecipeApp.ViewModel.Auth
{
    public partial class SignUpPageViewModel : ObservableObject
    {
        [ObservableProperty] private string email;

        [ObservableProperty] private string password;

        [ObservableProperty] private string username;

        [ObservableProperty] private string _errorMSG;

        [RelayCommand]
        public async Task SignUp()
        {
            var user = AuthService.Instance.SignUp(email, password, username);
            if (user != null)
            {
                UserService.Instance.SetCurrentSignUpUser(user);
                await Shell.Current.GoToAsync("//verifycode");
            }
            else
            {
                ErrorMSG = "Email already exists";
                Debug.WriteLine("Email already exists");
            }

        }

        [RelayCommand]
        public async Task Login()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}