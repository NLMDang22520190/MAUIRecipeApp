using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Service
{
	public class FoodRecipeTypeMappingService
	{
		private static FoodRecipeTypeMappingService _instance;
		private static readonly object _lock = new object();
		private readonly FirestoreDb _db;

		// Private constructor
		private FoodRecipeTypeMappingService()
		{
			_db = FirestoreService.Instance.Db;
			if (_db == null)
			{
				Debug.WriteLine("Firestore DB is null");
			}
		}

		// Singleton instance property
		public static FoodRecipeTypeMappingService Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new FoodRecipeTypeMappingService();
						}
					}
				}
				return _instance;
			}
		}

		// Retrieve all mappings
		public async Task<ObservableCollection<FoodRecipeTypeMapping>> GetAllMappings()
		{
			try
			{
				var mappingRef = _db.Collection("FoodRecipeTypeMappings");
				var snapshot = await mappingRef.WhereEqualTo("IsDeleted", false).GetSnapshotAsync();

				var mappings = new ObservableCollection<FoodRecipeTypeMapping>();
				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var mapping = document.ConvertTo<FoodRecipeTypeMapping>();
						mappings.Add(mapping);
					}
				}

				return mappings;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving mappings: {ex.Message}");
				return new ObservableCollection<FoodRecipeTypeMapping>();
			}
		}

		// Get mappings by RecipeId
		public async Task<ObservableCollection<FoodRecipeTypeMapping>> GetMappingsByRecipeId(string recipeId)
		{
			try
			{
				var mappingRef = _db.Collection("FoodRecipeTypeMappings");
				var query = mappingRef.WhereEqualTo("Frid", recipeId).WhereEqualTo("IsDeleted", false);
				var snapshot = await query.GetSnapshotAsync();

				var mappings = new ObservableCollection<FoodRecipeTypeMapping>();
				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var mapping = document.ConvertTo<FoodRecipeTypeMapping>();
						mappings.Add(mapping);
					}
				}

				return mappings;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving mappings for recipe {recipeId}: {ex.Message}");
				return new ObservableCollection<FoodRecipeTypeMapping>();
			}
		}

		// Add a new mapping
		public async Task<bool> AddMapping(string recipeId, string typeId)
		{
			try
			{
				var mappingCollection = _db.Collection("FoodRecipeTypeMappings");

				var mapping = new FoodRecipeTypeMapping
				{
					Frid = recipeId,
					Tofid = typeId,
					IsDeleted = false
				};

				string mappingId = $"{recipeId}_{typeId}";
				await mappingCollection.Document(mappingId).SetAsync(mapping);

				Debug.WriteLine("Mapping added successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding mapping: {ex.Message}");
				return false;
			}
		}

		// Soft delete a mapping
		public async Task<bool> DeleteMapping(string mappingId)
		{
			try
			{
				var mappingDoc = _db.Collection("FoodRecipeTypeMappings").Document(mappingId);
				await mappingDoc.UpdateAsync("IsDeleted", true);

				Debug.WriteLine("Mapping deleted successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error deleting mapping: {ex.Message}");
				return false;
			} 
		}

		// Update an existing mapping
		public async Task<bool> UpdateMapping(string mappingId, FoodRecipeTypeMapping updatedMapping)
		{
			try
			{
				var mappingDoc = _db.Collection("FoodRecipeTypeMappings").Document(mappingId);
				await mappingDoc.SetAsync(updatedMapping, SetOptions.Overwrite);

				Debug.WriteLine("Mapping updated successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error updating mapping: {ex.Message}");
				return false;
			}
		}
	}
}
