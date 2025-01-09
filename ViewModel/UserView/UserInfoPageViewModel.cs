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

        private readonly FirestoreDb db = FirestoreService.Instance.Db;

        public UserInfoPageViewModel()
        {
            LoadUserData();
        }

        [RelayCommand]
        public async void Logout()
        {
            UserService.Instance.ClearCurrentUser();
            await Shell.Current.GoToAsync("//login");
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
            Height = (float)currentUser.Height;
            Weight = (float)currentUser.Weight;
        }


    }
}
