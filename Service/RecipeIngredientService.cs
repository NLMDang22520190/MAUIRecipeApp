using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Service
{
	public class RecipeIngredientService
	{
		private static RecipeIngredientService _instance;
		private static readonly object _lock = new object();
		private readonly FirestoreDb _db;

		// Private constructor to enforce singleton
		private RecipeIngredientService()
		{
			_db = FirestoreService.Instance.Db;
			if (_db == null)
			{
				Debug.WriteLine("Firestore DB is null");
			}
		}

		// Singleton instance property
		public static RecipeIngredientService Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new RecipeIngredientService();
						}
					}
				}
				return _instance;
			}
		}

		// Add a new RecipeIngredient
		public async Task<bool> AddRecipeIngredient(RecipeIngredient recipeIngredient)
		{
			try
			{
				var recipeIngredientCollection = _db.Collection("RecipeIngredients");

				// Generate custom ID if not already set
				if (string.IsNullOrEmpty(recipeIngredient.Frid))
				{
					recipeIngredient.Frid = $"RI{DateTime.UtcNow:yyyyMMddHHmmss}";
				}

				await recipeIngredientCollection.Document(recipeIngredient.Frid).SetAsync(recipeIngredient);

				Debug.WriteLine("RecipeIngredient added successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding RecipeIngredient: {ex.Message}");
				return false;
			}
		}

		// Get all RecipeIngredients
		public async Task<ObservableCollection<RecipeIngredient>> LoadAllRecipeIngredients()
		{
			var recipeIngredients = new ObservableCollection<RecipeIngredient>();
			try
			{
				var recipeIngredientCollection = FirestoreService.Instance.Db.Collection("RecipeIngredients");
				var snapshot = await recipeIngredientCollection.GetSnapshotAsync();

				// Clear the existing recipeIngredients list
				recipeIngredients.Clear();

				foreach (var doc in snapshot.Documents)
				{
					var recipeIngredient = doc.ConvertTo<RecipeIngredient>();
					if (recipeIngredient.IsDeleted != true)
					{
						recipeIngredients.Add(recipeIngredient); // Add RecipeIngredient object
					}
				}

				Debug.WriteLine("RecipeIngredients loaded successfully");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading RecipeIngredients: {ex.Message}");
			}
			return recipeIngredients;
		}

		// Get RecipeIngredients by Recipe ID (Frid)
		public async Task<ObservableCollection<RecipeIngredient>> GetRecipeIngredientsByRecipeId(string recipeId)
		{
			var recipeIngredients = new ObservableCollection<RecipeIngredient>();
			try
			{
				var recipeIngredientCollection = FirestoreService.Instance.Db.Collection("RecipeIngredients");
				var query = recipeIngredientCollection.WhereEqualTo("Frid", recipeId);
				var snapshot = await query.GetSnapshotAsync();

				foreach (var doc in snapshot.Documents)
				{
					var recipeIngredient = doc.ConvertTo<RecipeIngredient>();
					if (recipeIngredient.IsDeleted != true)
					{
						recipeIngredients.Add(recipeIngredient);
					}
				}

				Debug.WriteLine("RecipeIngredients retrieved successfully");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving RecipeIngredients: {ex.Message}");
			}
			return recipeIngredients;
		}

		// Update a RecipeIngredient
		public async Task<bool> UpdateRecipeIngredient(string recipeIngredientId, RecipeIngredient updatedRecipeIngredient)
		{
			try
			{
				var recipeIngredientDoc = _db.Collection("RecipeIngredients").Document(recipeIngredientId);
				await recipeIngredientDoc.SetAsync(updatedRecipeIngredient, SetOptions.Overwrite);

				Debug.WriteLine("RecipeIngredient updated successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error updating RecipeIngredient: {ex.Message}");
				return false;
			}
		}

		// Soft delete a RecipeIngredient
		public async Task<bool> DeleteRecipeIngredient(string recipeIngredientId)
		{
			try
			{
				var recipeIngredientDoc = _db.Collection("RecipeIngredients").Document(recipeIngredientId);

				// Update the IsDeleted field to true
				await recipeIngredientDoc.UpdateAsync("IsDeleted", true);

				Debug.WriteLine("RecipeIngredient deleted successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error deleting RecipeIngredient: {ex.Message}");
				return false;
			}
		}
	}
}
