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

        private List<User> allUsers = new List<User>();

        private FirestoreDb _db;

        public EditUserPageViewModel()
        {
            
        }

        [RelayCommand]
        public async Task EditUser(string uuid)
        {
            await Shell.Current.GoToAsync($"editcurrentuser?UUID={uuid}");
        }

        [RelayCommand]
        public async Task ShowAllAdmin()
        {
            var tempUsers = new List<User>(allUsers);
            //tempUsers = Users.ToList();
            Users.Clear();
            foreach (var user in tempUsers)
            {
                if (user.isAdmin == true)
                {
                    Users.Add(user);
                }
            }
        }

        [RelayCommand]
        public async Task ShowAllDeactivated()
        {
            var tempUsers = new List<User>(allUsers);
            //tempUsers = Users.ToList();
            Users.Clear();
            foreach (var user in tempUsers)
            {
                if (user.isDeactivated == true)
                {
                    Users.Add(user);
                }
            }
        }

        [RelayCommand]
        public async Task ShowAll()
        {
            Users.Clear();
            Users = new ObservableCollection<User>(allUsers);
        }


        public void SearchUser(string searchKey)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                Users.Clear();
                LoadItem();
                return;
            }

            var tempUsers = new List<User>(allUsers);
            //tempUsers = allUsers.ToList();
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
            
            LoadItem();
        }

        private void LoadItem()
        {
            Users.Clear();
            allUsers.Clear();
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
                        allUsers.Add(user); // Thêm vào ObservableCollection
                    }
                }

                Users = new ObservableCollection<User>(allUsers);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }
        

    }
}
