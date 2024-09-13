using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.Auth
{
    public partial class LoginPageViewModel : ObservableObject
    {
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
            await Shell.Current.GoToAsync("//home");
        }
    }
}
