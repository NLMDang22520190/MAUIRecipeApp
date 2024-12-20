using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.Service;

namespace MAUIRecipeApp.ViewModel.Auth
{
    public partial class PasswordRecoveryPageViewModel: ObservableObject
    {
        [ObservableProperty] private string email;

        [ObservableProperty] private string _errorMSG;


        [RelayCommand]
        private async Task SendCode()
        {
            if (!AuthService.Instance.CheckEmailExist(Email))
            {
                ErrorMSG = $"No account found associated with the email.  \n Please check your email address \n or sign up if you don't have an account.";
                return;
            }

            UserService.Instance.CurrentRecoveryEmail = Email;

            await Shell.Current.GoToAsync("//passwordcode");
        }

        [RelayCommand]
        public async Task Login()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
