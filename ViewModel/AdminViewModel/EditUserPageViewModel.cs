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

namespace MAUIRecipeApp.ViewModel.AdminViewModel
{
    public partial class EditUserPageViewModel: ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<User> users = new ObservableCollection<User>();

        private readonly FirestoreDb _db;

        public EditUserPageViewModel()
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
