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
    public partial class NewPasswordPageViewModel: ObservableObject
    {
        [ObservableProperty] private string _password;

        [ObservableProperty] private string _confirmPassword;

        [ObservableProperty] private string _errorMSG;

        [RelayCommand]
        private async Task ChangePassword()
        {
            if(_password != _confirmPassword)
            {
                ErrorMSG = "Password does not match";
                return;
            }
            
            bool status = await AuthService.Instance.UpdateUserPassword(UserService.Instance.CurrentRecoveryEmail, _password);
            if (status)
            {
                await Shell.Current.GoToAsync("//login");
            }
            else
            {
                ErrorMSG = "Error changing password";
            }

            await Shell.Current.GoToAsync("//login");
        }
    }
}
