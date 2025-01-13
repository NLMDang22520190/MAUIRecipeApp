using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.Service;

namespace MAUIRecipeApp.ViewModel.Auth.GoogleSignIn
{
    public partial class GoogleSignUpPageViewModel: ObservableObject
    {
        [ObservableProperty]
        private string email;

        [ObservableProperty] private string errorMSG;

       

        public GoogleSignUpPageViewModel()
        {
        }

        [RelayCommand]
        public async Task SignUp()
        {
            string password = string.Empty;
            string username = "GGUser" + DateTime.Now.ToString("yyyyMMddHHmmss"); // Sử dụng giờ hệ thống để tạo username

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
