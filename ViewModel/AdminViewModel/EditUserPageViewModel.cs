using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.Models;
using Google.Cloud.Firestore;
using System.Diagnostics;
using MAUIRecipeApp.Service;
using MAUIRecipeApp.DTO;
using CommunityToolkit.Mvvm.Input;

namespace MAUIRecipeApp.ViewModel.AdminViewModel
{
    public partial class EditUserPageViewModel: ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<User> users = new ObservableCollection<User>();

        private FirestoreDb _db;

        public EditUserPageViewModel()
        {
            
        }

        [RelayCommand]
        public async Task EditUser(string uuid)
        {
            await Shell.Current.GoToAsync($"editcurrentuser?UUID={uuid}");
        }


        public void SearchUser(string searchKey)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                Users.Clear();
                LoadItem();
                return;
            }

            var tempUsers = new List<User>(Users);
            tempUsers = Users.ToList();
            Users.Clear();
            foreach (var user in tempUsers)
            {
                if (user.Username.ToLower().Contains(searchKey.ToLower()))
                {
                    Users.Add(user);
                }
            }
        }

        public void OnAppearing()
        {
            _db = FirestoreService.Instance.Db;
            if (_db == null)
            {
                Debug.WriteLine("Firestore DB is null");
                return;
            }
            Users.Clear();
            LoadItem();
        }

        private void LoadItem()
        {
            LoadUsers();
        }

        private async void LoadUsers()
        {
            try
            {
                CollectionReference usersRef = _db.Collection("User");
                QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        User user = document.ConvertTo<User>();
                        user.Uid = document.Id;
                        Users.Add(user); // Thêm vào ObservableCollection
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }
        

    }
}
