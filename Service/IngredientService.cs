using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Service
{
	public class IngredientService
	{
		private static IngredientService _instance;
		private static readonly object _lock = new object();
		private readonly FirestoreDb _db;

		// Private constructor to enforce singleton
		private IngredientService()
		{
			_db = FirestoreService.Instance.Db;
			if (_db == null)
			{
				Debug.WriteLine("Firestore DB is null");
			}
		}

		// Singleton instance property
		public static IngredientService Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new IngredientService();
						}
					}
				}
				return _instance;
			}
		}

		// Add a new Ingredient
		public async Task<bool> AddIngredient(Ingredient ingredient)
		{
			try
			{
				var ingredientCollection = _db.Collection("Ingredients");

				// Generate custom ID
				string ingredientId = $"ING{DateTime.UtcNow:yyyyMMddHHmmss}";
				ingredient.Iid = ingredientId;

				await ingredientCollection.Document(ingredientId).SetAsync(ingredient);

				Debug.WriteLine("Ingredient added successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding ingredient: {ex.Message}");
				return false;
			}
		}

		// Get all Ingredients
		public async Task<ObservableCollection<Ingredient>> LoadAllIngredients()
		{
			var Ingredients = new ObservableCollection<Ingredient>();
			try
			{
				var ingredientCollection = FirestoreService.Instance.Db.Collection("Ingredients");
				var snapshot = await ingredientCollection.GetSnapshotAsync();

				// Clear the existing Ingredients list
				Ingredients.Clear();

				foreach (var doc in snapshot.Documents)
				{
					var ingredient = doc.ConvertTo<Ingredient>();
					if (ingredient.IsDeleted != true)
					{
						Ingredients.Add(ingredient); // Add ingredient object
					}
				}

				Debug.WriteLine("Ingredients loaded successfully");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading ingredients: {ex.Message}");
				// Return empty collection if there is an error
			}
			return Ingredients;
		}

		// Get an Ingredient by ID
		public async Task<Ingredient> GetIngredientById(string ingredientId)
		{
			try
			{
				var ingredientDoc = _db.Collection("Ingredients").Document(ingredientId);
				var snapshot = await ingredientDoc.GetSnapshotAsync();

				if (snapshot.Exists)
				{
					return snapshot.ConvertTo<Ingredient>();
				}

				Debug.WriteLine("Ingredient not found");
				return null;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving ingredient: {ex.Message}");
				return null;
			}
		}

		// Get All Ingredients by Recipe ID (Frid)
		public async Task<ObservableCollection<Ingredient>> GetIngredientsByRecipeId(string recipeId)
		{
			var ingredients = new ObservableCollection<Ingredient>();
			try
			{
				var ingredientCollection = FirestoreService.Instance.Db.Collection("Ingredients");
				var query = ingredientCollection.WhereEqualTo("Frid", recipeId);
				var snapshot = await query.GetSnapshotAsync();

				foreach (var doc in snapshot.Documents)
				{
					var ingredient = doc.ConvertTo<Ingredient>();
					if (ingredient.IsDeleted != true)
					{
						ingredients.Add(ingredient);
					}
				}

				Debug.WriteLine("Ingredients retrieved successfully");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving Ingredients: {ex.Message}");
			}
			return ingredients;
		}

		// Update an Ingredient
		public async Task<bool> UpdateIngredient(string ingredientId, Ingredient updatedIngredient)
		{
			try
			{
				var ingredientDoc = _db.Collection("Ingredients").Document(ingredientId);
				await ingredientDoc.SetAsync(updatedIngredient, SetOptions.Overwrite);

				Debug.WriteLine("Ingredient updated successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error updating ingredient: {ex.Message}");
				return false;
			}
		}

		// Soft delete an Ingredient
		public async Task<bool> DeleteIngredient(string ingredientId)
		{
			try
			{
				var ingredientDoc = _db.Collection("Ingredients").Document(ingredientId);

				// Update the IsDeleted field to true
				await ingredientDoc.UpdateAsync("IsDeleted", true);

				Debug.WriteLine("Ingredient deleted successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error deleting ingredient: {ex.Message}");
				return false;
			}
		}
	}
}
