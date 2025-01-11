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
	public class FoodRecipeService
	{
		private static FoodRecipeService _instance;
		private static readonly object _lock = new object();
		private readonly FirestoreDb _db;

		// Private constructor
		private FoodRecipeService()
		{
			_db = FirestoreService.Instance.Db;
			if (_db == null)
			{
				Debug.WriteLine("Firestore DB is null");
			}
		}

		// Singleton instance property
		public static FoodRecipeService Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new FoodRecipeService();
						}
					}
				}
				return _instance;
			}
		}

		// Retrieve all recipes with IsDeleted = false
		public async Task<ObservableCollection<FoodRecipe>> GetAllFoodRecipes()
		{
			try
			{
				var recipesRef = _db.Collection("FoodRecipes");
				var query = recipesRef.WhereEqualTo("IsDeleted", false);
				var snapshot = await query.GetSnapshotAsync();

				var recipes = new ObservableCollection<FoodRecipe>();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var recipe = document.ConvertTo<FoodRecipe>();
						recipe.Frid = document.Id; // Assign FRID from document ID
						recipes.Add(recipe);
					}
				}

				return recipes;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving recipes: {ex.Message}");
				return new ObservableCollection<FoodRecipe>();
			}
		}

		// Get recipes by UserId with IsDeleted = false
		public async Task<ObservableCollection<FoodRecipe>> GetFoodRecipesByUserId(int userId)
		{
			try
			{
				var recipesRef = _db.Collection("FoodRecipes");
				var query = recipesRef
					.WhereEqualTo("UploaderUid", userId)
					.WhereEqualTo("IsDeleted", false);
				var snapshot = await query.GetSnapshotAsync();

				var recipes = new ObservableCollection<FoodRecipe>();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var recipe = document.ConvertTo<FoodRecipe>();
						recipe.Frid = document.Id; // Assign FRID from document ID
						recipes.Add(recipe);
					}
				}

				return recipes;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving recipes for user {userId}: {ex.Message}");
				return new ObservableCollection<FoodRecipe>();
			}
		}

		// Retrieve a single recipe by ID
		public async Task<FoodRecipe> GetFoodRecipeById(string recipeId)
		{
			try
			{
				var recipeDoc = _db.Collection("FoodRecipes").Document(recipeId);
				var snapshot = await recipeDoc.GetSnapshotAsync();

				if (snapshot.Exists)
				{
					var recipe = snapshot.ConvertTo<FoodRecipe>();
					recipe.Frid = snapshot.Id; // Assign FRID from document ID
					return recipe;
				}

				Debug.WriteLine("Recipe not found");
				return null;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving recipe: {ex.Message}");
				return null;
			}
		}

		// Add a new recipe
		public async Task<bool> AddFoodRecipe(FoodRecipe recipe)
		{
			try
			{
				var recipeCollection = _db.Collection("FoodRecipes");

				// Generate custom ID if needed
				string recipeId = $"FR{DateTime.UtcNow:yyyyMMddHHmmss}";

				recipe.Frid = recipeId; // Assign ID to the recipe
				await recipeCollection.Document(recipeId).SetAsync(recipe);

				Debug.WriteLine("Recipe added successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding recipe: {ex.Message}");
				return false;
			}
		}

		// Get IngredientswithName by Recipe ID
		public async Task<ObservableCollection<IngredientInfos>> GetIngredientsWithNameByRecipeId(string recipeId)
		{
			try
			{
				var ingredients = await IngredientService.Instance.GetIngredientsByRecipeId(recipeId);
				var recipeIngredients = await RecipeIngredientService.Instance.GetRecipeIngredientsByRecipeId(recipeId);

				var IngredientInfos = new ObservableCollection<IngredientInfos>();

				foreach (var ingredient in ingredients)
				{
					var IngredientInfo = new IngredientInfos
					{
						Frid = recipeId,
						Iid = ingredient.Iid,
						Name = ingredient.IngredientName,
						Quantity = recipeIngredients.FirstOrDefault(ri => ri.Iid == ingredient.Iid)?.Quantity ?? 0,
					};

					IngredientInfos.Add(IngredientInfo);
				}

				return IngredientInfos;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading ingredients: {ex.Message}");
				return new ObservableCollection<IngredientInfos>();
			}
		}

		// Soft delete a recipe
		public async Task<bool> DeleteFoodRecipe(string recipeId)
		{
			try
			{
				var recipeDoc = _db.Collection("FoodRecipes").Document(recipeId);

				// Update the IsDeleted field to true
				await recipeDoc.UpdateAsync("IsDeleted", true);

				Debug.WriteLine("Recipe deleted successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error deleting recipe: {ex.Message}");
				return false;
			}
		}

		// Update an existing recipe
		public async Task<bool> UpdateFoodRecipe(string recipeId, FoodRecipe updatedRecipe)
		{
			try
			{
				var recipeDoc = _db.Collection("FoodRecipes").Document(recipeId);
				await recipeDoc.SetAsync(updatedRecipe, SetOptions.Overwrite);

				Debug.WriteLine("Recipe updated successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error updating recipe: {ex.Message}");
				return false;
			}
		}

		/// Add a new FoodRecipeType to a Recipe using Mapping
		public async Task<bool> AddFoodRecipeTypeToRecipeMappingAsync(string recipeId, string typeId)
		{
			try
			{
				var mappingCollection = _db.Collection("FoodRecipeTypeMappings");

				// Create a new mapping object
				var mapping = new FoodRecipeTypeMapping
				{
					Frid = recipeId,
					Tofid = typeId,
					IsDeleted = false
				};

				// Generate a unique mapping ID
				string mappingId = $"{recipeId}_{typeId}";

				// Save the mapping to Firestore
				await mappingCollection.Document(mappingId).SetAsync(mapping);

				Debug.WriteLine("FoodRecipeType added to recipe mapping successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding FoodRecipeType to recipe mapping: {ex.Message}");
				return false;
			}
		}
	}
}
