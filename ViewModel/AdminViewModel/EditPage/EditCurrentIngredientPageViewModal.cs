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

namespace MAUIRecipeApp.ViewModel.AdminViewModel.EditPage
{
    [QueryProperty(nameof(SelectedIngredientId), "IID")]
    public partial class EditCurrentIngredientPageViewModal: ObservableObject
    {
        [ObservableProperty] private string selectedIngredientId;

        [ObservableProperty] private Ingredient selectedIngredient = new Ingredient();

        [ObservableProperty] private string ingredientName;

        [ObservableProperty] private string measurementUnit;

        [ObservableProperty] private string updateMsg;

        [ObservableProperty] private bool canUpdate = true;

        private FirestoreDb db;

        public EditCurrentIngredientPageViewModal()
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
            var result = await FirestoreService.Instance.UpdateDocumentAsync("Ingredients",
                selectedIngredientId,
                new Dictionary<string, object>
                {
                    { "IngredientName", IngredientName },
                    { "MeasurementUnit", MeasurementUnit }
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

        private async Task LoadItem()
        {
            LoadSelectedIngredient();
        }

        private async Task LoadSelectedIngredient()
        {
            try
            {
                CollectionReference usersRef = db.Collection("Ingredients");
                DocumentReference docRef = usersRef.Document(selectedIngredientId);
                DocumentSnapshot snapshot = docRef.GetSnapshotAsync().Result;
                if (snapshot.Exists)
                {
                    SelectedIngredient = snapshot.ConvertTo<Ingredient>();
                }
                else
                {
                    SelectedIngredient = null;
                }

                if (SelectedIngredient != null)
                {
                    IngredientName = SelectedIngredient.IngredientName;
                    MeasurementUnit = SelectedIngredient.MeasurementUnit;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e.Message);
            }
        }
    }
}
