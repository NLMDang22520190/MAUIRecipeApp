using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel
{
    public partial class StartUpPageViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task Login()
        {
           var db = FirestoreService.Instance.Db;

            // Tạo danh sách các FoodRecipeType
          

            await Shell.Current.GoToAsync("//login");
        }

        [RelayCommand]

        private async Task SignUp()
        {
            await Shell.Current.GoToAsync("//signup");
        }
    }
}
