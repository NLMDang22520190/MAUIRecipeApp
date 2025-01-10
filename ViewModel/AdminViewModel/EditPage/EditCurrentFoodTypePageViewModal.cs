using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using OpenAI.Assistants;

namespace MAUIRecipeApp.ViewModel.AdminViewModel.EditPage
{
    [QueryProperty(nameof(SelectedFoodTypeId), "TOFID")]
    public partial class EditCurrentFoodTypePageViewModal: ObservableObject
    {
        [ObservableProperty] private string selectedFoodTypeId;

        [ObservableProperty] private FoodRecipeType selectedFoodType = new FoodRecipeType();

        [ObservableProperty] private string foodTypeName;

        [ObservableProperty] private string updateMsg;

        [ObservableProperty] private bool canUpdate = true;

        private FirestoreDb db;

        public EditCurrentFoodTypePageViewModal()
        {
        }

        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        public async Task Update()
        {
            canUpdate = false;
            var result = await FirestoreService.Instance.UpdateDocumentAsync("FoodRecipeTypes",
                selectedFoodTypeId,
                new Dictionary<string, object>
                {
                    { "FoodTypeName", FoodTypeName },
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
            LoadItem();
        }

        private void LoadItem()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                CollectionReference usersRef = db.Collection("FoodRecipeTypes");
                DocumentReference docRef = usersRef.Document(selectedFoodTypeId);
                DocumentSnapshot snapshot = docRef.GetSnapshotAsync().Result;
                if (snapshot.Exists)
                {
                    SelectedFoodType = snapshot.ConvertTo<FoodRecipeType>();
                }
                else
                {
                    SelectedFoodType = null;
                }

                if (SelectedFoodType != null)
                {
                    FoodTypeName = SelectedFoodType.FoodTypeName;
                    
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e.Message);
            }
        }
    }
}
