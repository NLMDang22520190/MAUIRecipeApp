using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using MAUIRecipeApp.View.UserView;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class UserInfoPageViewModel : ObservableObject
    {
        [ObservableProperty] private string userName;

        [ObservableProperty] private string email;

        [ObservableProperty] private string healthCondition;

        [ObservableProperty] private string allergies;

        [ObservableProperty] private float height;

        [ObservableProperty] private float weight;

        [ObservableProperty] private bool canUpdate = true;

        [ObservableProperty] private bool canLogout = true;

        [ObservableProperty] private string updateMsg = string.Empty;

        [ObservableProperty] private string currentPassword = string.Empty;

        [ObservableProperty] private string newPassword;

        [ObservableProperty] private string confirmPassword;

        [ObservableProperty] private string updatePasswordMsg = string.Empty;

        private readonly FirestoreDb db = FirestoreService.Instance.Db;

        public UserInfoPageViewModel()
        {
            LoadUserData();
        }

        [RelayCommand]
        public async void Logout()
        {
           await AuthService.Instance.LogOut();
        }

        [RelayCommand]
        public async void Update()
        {
            canUpdate = false;
            canLogout = false;
            var result = await FirestoreService.Instance.UpdateDocumentAsync("User",
                UserService.Instance.CurrentUser.Uid,
                new Dictionary<string, object>
                {
                    { "Allergies", Allergies },
                    { "HealthCondition", HealthCondition },
                    { "Username", UserName },
                    { "Height", Height },
                    { "Weight", Weight }
                });
            if (result)
            {
                UpdateMsg = "Update successfully!";
            }
            else
            {
                UpdateMsg = "Update failed!";
            }

            var updatedUser = new User
            {
                Uid = UserService.Instance.CurrentUser.Uid,
                Username = UserName,
                Email = Email,
                HealthCondition = HealthCondition,
                Allergies = Allergies,
                Height = Height,
                Weight = Weight

            };
            UserService.Instance.SetCurrentUser(updatedUser);
            canUpdate = true;
            canLogout = true;
        }

        [RelayCommand]
        public async void UpdatePassword()
        {
            var loginResult = AuthService.Instance.Login(UserService.Instance.CurrentUser.Email, CurrentPassword);
            if (loginResult == null)
            {
                UpdatePasswordMsg = "Invalid current password!";
                return;
            }
            
            var updatePassResult = await AuthService.Instance.UpdateUserPassword(UserService.Instance.CurrentUser.Email, NewPassword);

            if (updatePassResult)
            {
                UpdatePasswordMsg = "Update password successfully!";
            }
            else
            {
                UpdatePasswordMsg = "Update password failed!";
            }
        }


        private void LoadUserData()
        {
            var currentUser = UserService.Instance.CurrentUser;
            if (currentUser == null)
            {
                return;
            }
            UserName = currentUser.Username;
            Email = currentUser.Email;
            HealthCondition = currentUser.HealthCondition;
            Allergies = currentUser.Allergies;
            Height = currentUser.Height != null? (float)currentUser.Height : 0;
            Weight = currentUser.Weight != null ? (float)currentUser.Weight : 0;
        }
    }
}
