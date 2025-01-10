using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Service
{
	public class UserSavedRecipesService
	{
		private static UserSavedRecipesService _instance;
		private static readonly object _lock = new object();
		private readonly FirestoreDb _db;

		// Private constructor
		private UserSavedRecipesService()
		{
			_db = FirestoreService.Instance.Db;
			if (_db == null)
			{
				Debug.WriteLine("Firestore DB is null");
			}
		}

		// Singleton instance property
		public static UserSavedRecipesService Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new UserSavedRecipesService();
						}
					}
				}
				return _instance;
			}
		}

		// Add a new saved recipe
		public async Task<bool> AddUserSavedRecipe(UserSavedRecipe savedRecipe)
		{
			try
			{
				var savedRecipesCollection = _db.Collection("UserSavedRecipes");

				// Generate custom ID if needed (or use UUID)
				string savedRecipeId = $"USR{DateTime.UtcNow:yyyyMMddHHmmss}";

				// Assign the DocumentReference to the UUID field and FRID (for FoodRecipe reference)
				savedRecipe.UUID = _db.Collection("User").Document(savedRecipeId);  // Correct reference for User document
				savedRecipe.FRID = _db.Collection("FoodRecipes").Document(savedRecipeId);  // Correct reference for FoodRecipe document

				await savedRecipesCollection.Document(savedRecipeId).SetAsync(savedRecipe);

				Debug.WriteLine("User saved recipe added successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding saved recipe: {ex.Message}");
				return false;
			}
		}

		// Retrieve all saved recipes by User ID
		public async Task<List<UserSavedRecipe>> GetUserSavedRecipesByUserId(int userId)
		{
			try
			{
				var userSavedRecipesRef = _db.Collection("UserSavedRecipes");
				var query = userSavedRecipesRef.WhereEqualTo("Uid", userId);
				var snapshot = await query.GetSnapshotAsync();

				var userSavedRecipes = new List<UserSavedRecipe>();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var savedRecipe = document.ConvertTo<UserSavedRecipe>();
						savedRecipe.Frid = document.Id; // Assign ID from Firestore document to Frid
						userSavedRecipes.Add(savedRecipe);
					}
				}

				return userSavedRecipes;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving saved recipes for user {userId}: {ex.Message}");
				return new List<UserSavedRecipe>();
			}
		}

		// Delete a saved recipe by User ID
		public async Task<bool> DeleteUserSavedRecipe(string savedRecipeId)
		{
			try
			{
				var savedRecipeDoc = _db.Collection("UserSavedRecipes").Document(savedRecipeId);

				// Soft delete (set IsDeleted flag to true)
				await savedRecipeDoc.UpdateAsync("IsDeleted", true);

				Debug.WriteLine("Saved recipe deleted successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error deleting saved recipe: {ex.Message}");
				return false;
			}
		}

		// Retrieve a single saved recipe by its ID
		public async Task<UserSavedRecipe> GetUserSavedRecipeById(string savedRecipeId)
		{
			try
			{
				var savedRecipeDoc = _db.Collection("UserSavedRecipes").Document(savedRecipeId);
				var snapshot = await savedRecipeDoc.GetSnapshotAsync();

				if (snapshot.Exists)
				{
					var savedRecipe = snapshot.ConvertTo<UserSavedRecipe>();
					savedRecipe.Frid = snapshot.Id; // Assign ID from Firestore document to Frid
					return savedRecipe;
				}

				Debug.WriteLine("Saved recipe not found");
				return null;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving saved recipe: {ex.Message}");
				return null;
			}
		}
	}
}
