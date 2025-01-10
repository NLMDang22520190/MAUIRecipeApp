using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using OpenAI.Assistants;

namespace MAUIRecipeApp.ViewModel.AdminViewModel.EditPage
{
    [QueryProperty(nameof(SelectedUserId), "UUID")]
    public partial class EditCurrentUserPageViewModal: ObservableObject
    {
        [ObservableProperty]
        private string selectedUserId;

        [ObservableProperty] private User selectedUser = new User();

        [ObservableProperty] private string userName;

        [ObservableProperty] private string email;

        [ObservableProperty] private string healthCondition;

        [ObservableProperty] private string allergies;

        [ObservableProperty] private float height;

        [ObservableProperty] private float weight;

        [ObservableProperty] private bool isAdmin;

        [ObservableProperty] private bool isDeactivated;

        [ObservableProperty] private bool canUpdate = true;

        [ObservableProperty] private string updateMsg = string.Empty;


        private readonly FirestoreService _firestoreService;
        private FirestoreDb db;

        public EditCurrentUserPageViewModal()
        {
            
        }

        [RelayCommand]
        public async Task Back()
        {

            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        public async void Update()
        {
            canUpdate = false;
            var result = await FirestoreService.Instance.UpdateDocumentAsync("User",
                SelectedUserId,
                new Dictionary<string, object>
                {
                    { "Allergies", Allergies },
                    { "HealthCondition", HealthCondition },
                    { "Username", UserName },
                    { "Height", Height },
                    { "Weight", Weight },
                    { "isAdmin", IsAdmin },
                    { "isDeactivated", IsDeactivated }
                });
            if (result)
            {
                UpdateMsg = "Update successfully!";
            }
            else
            {
                UpdateMsg = "Update failed!";
            }

           
            canUpdate = true;
        }

        public void OnAppearing()
        {
            
            db = FirestoreService.Instance.Db;
            if (db == null)
            {
                Debug.WriteLine("Firestore DB is null");
                return;
            }
            LoadSelectedUser();
        }

        private void LoadSelectedUser()
        {
            try
            {
                CollectionReference usersRef = db.Collection("User");
                DocumentReference docRef = usersRef.Document(selectedUserId);
                DocumentSnapshot snapshot = docRef.GetSnapshotAsync().Result;
                if (snapshot.Exists)
                {
                    SelectedUser = snapshot.ConvertTo<User>();
                }
                else
                {
                    SelectedUser = null;
                }

                if (SelectedUser != null)
                {
                    UserName = SelectedUser.Username;
                    Email = SelectedUser.Email;
                    HealthCondition = SelectedUser.HealthCondition;
                    Allergies = SelectedUser.Allergies;
                    Height = SelectedUser.Height != null ? (float)SelectedUser.Height : 0;
                    Weight = SelectedUser.Weight != null ? (float)SelectedUser.Weight : 0;
                    IsAdmin = SelectedUser.isAdmin != null ? (bool)SelectedUser.isAdmin : false;
                    IsDeactivated = SelectedUser.isDeactivated != null ? (bool)SelectedUser.isDeactivated : false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e.Message);
            }
            

        }

    }
}
