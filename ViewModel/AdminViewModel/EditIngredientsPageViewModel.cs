using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.DTO;

namespace MAUIRecipeApp.ViewModel.AdminViewModel
{
   public partial class EditIngredientsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Ingredient> ingredients = new ObservableCollection<Ingredient>();

        private FirestoreDb _db;

        public EditIngredientsPageViewModel()
        {
           
        }


        [RelayCommand]
        public async Task EditIngredient(string iid)
        {
            Debug.WriteLine(iid);
            await Shell.Current.GoToAsync($"editcurrentingredient?IID={iid}");
        }

        [RelayCommand]
        public async Task<bool> AddIngredient(AddNewIngredientDTO newIngre)
        {
            var result = await FirestoreService.Instance.AddDocumentAsync("Ingredients", new Ingredient
            {
                IngredientName = newIngre.IngredientName,
                MeasurementUnit = newIngre.Unit,
                IsDeleted = false
            });
            if (result)
            {
                LoadItem();
            }
            return result;
        }

        public void SearchIngredient(string searchKey)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                Ingredients.Clear();
                LoadItem();
                return;
            }

            var tempIngredients = new List<Ingredient>(Ingredients);
            tempIngredients = Ingredients.ToList();
            Ingredients.Clear();
            foreach (var ingre in tempIngredients)
            {
                if (ingre.IngredientName.ToLower().Contains(searchKey.ToLower()))
                {
                    Ingredients.Add(ingre);
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
            Ingredients.Clear();
            LoadIngredients();
        }

        private async void LoadIngredients()
        {
            // Truy vấn để lấy các mục có IsDeleted = false
            try
            {
                CollectionReference recipesRef = _db.Collection("Ingredients");
                //Query query = recipesRef.WhereEqualTo("IsDeleted", false);
                QuerySnapshot snapshot = await recipesRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        Ingredient recipe = document.ConvertTo<Ingredient>();
                        recipe.Iid = document.Id; // Lấy FRID từ Document ID
                        Ingredients.Add(recipe); // Thêm vào ObservableCollection
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
