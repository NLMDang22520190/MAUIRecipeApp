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

				// Let Firestore generate a new document ID for the saved recipe
				var savedRecipeDocRef = savedRecipesCollection.Document(); // Automatically generates a new ID

				// Save the UserSavedRecipe document with the generated ID
				await savedRecipeDocRef.SetAsync(savedRecipe);

				Debug.WriteLine("User saved recipe added successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding saved recipe: {ex.Message}");
				return false;
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

						// Convert string document ID to an integer
						if (int.TryParse(document.Id, out int frid))
						{
							savedRecipe.Frid = frid;
						}
						else
						{
							// Handle invalid ID case if necessary
							Debug.WriteLine($"Invalid Frid for document ID {document.Id}");
						}

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

		public async Task<UserSavedRecipe> GetUserSavedRecipeById(string savedRecipeId)
		{
			try
			{
				var savedRecipeDoc = _db.Collection("UserSavedRecipes").Document(savedRecipeId);
				var snapshot = await savedRecipeDoc.GetSnapshotAsync();

				if (snapshot.Exists)
				{
					var savedRecipe = snapshot.ConvertTo<UserSavedRecipe>();

					// Convert string document ID to an integer
					if (int.TryParse(snapshot.Id, out int frid))
					{
						savedRecipe.Frid = frid;
					}
					else
					{
						// Handle invalid ID case if necessary
						Debug.WriteLine($"Invalid Frid for document ID {snapshot.Id}");
					}

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
