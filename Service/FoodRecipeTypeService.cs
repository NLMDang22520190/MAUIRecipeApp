using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Service
{
	public class FoodRecipeTypeService
	{
		private static FoodRecipeTypeService _instance;
		private static readonly object _lock = new object();
		private readonly FirestoreDb _db;

		// Private constructor to enforce singleton
		private FoodRecipeTypeService()
		{
			_db = FirestoreService.Instance.Db;
			if (_db == null)
			{
				Debug.WriteLine("Firestore DB instance is null");
			}
		}

		// Singleton instance property
		public static FoodRecipeTypeService Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new FoodRecipeTypeService();
						}
					}
				}
				return _instance;
			}
		}

		// Add a new FoodRecipeType
		public async Task<bool> AddFoodRecipeTypeAsync(FoodRecipeType recipeType)
		{
			if (recipeType == null)
			{
				Debug.WriteLine("Invalid FoodRecipeType: null value provided");
				return false;
			}

			try
			{
				var typeCollection = _db.Collection("FoodRecipeTypes");

				// Generate custom ID
				string typeId = $"FT{DateTime.UtcNow:yyyyMMddHHmmss}";
				recipeType.Tofid = typeId;

				await typeCollection.Document(typeId).SetAsync(recipeType);

				Debug.WriteLine($"FoodRecipeType {typeId} added successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding FoodRecipeType: {ex.Message}");
				return false;
			}
		}

		// Get all FoodRecipeTypes
		public async Task<List<FoodRecipeType>> GetAllFoodRecipeTypesAsync()
		{
			try
			{
				var typeCollection = _db.Collection("FoodRecipeTypes");
				var snapshot = await typeCollection.GetSnapshotAsync();

				if (snapshot.Documents.Count == 0)
				{
					Debug.WriteLine("No FoodRecipeTypes found");
					return new List<FoodRecipeType>();
				}

				var foodRecipeTypes = snapshot.Documents
					.Where(doc => doc.Exists)
					.Select(doc => doc.ConvertTo<FoodRecipeType>())
					.Where(type => type != null && (type.IsDeleted == false || type.IsDeleted == null))
					.ToList();

				Debug.WriteLine($"Retrieved {foodRecipeTypes.Count} FoodRecipeTypes");
				return foodRecipeTypes;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving FoodRecipeTypes: {ex.Message}");
				return new List<FoodRecipeType>();
			}
		}

		// Get a FoodRecipeType by ID
		public async Task<FoodRecipeType> GetFoodRecipeTypeByIdAsync(string typeId)
		{
			if (string.IsNullOrEmpty(typeId))
			{
				Debug.WriteLine("Invalid typeId: null or empty value provided");
				return null;
			}

			try
			{
				var typeDoc = _db.Collection("FoodRecipeTypes").Document(typeId);
				var snapshot = await typeDoc.GetSnapshotAsync();

				if (snapshot.Exists)
				{
					Debug.WriteLine($"FoodRecipeType {typeId} retrieved successfully");
					return snapshot.ConvertTo<FoodRecipeType>();
				}

				Debug.WriteLine($"FoodRecipeType {typeId} not found");
				return null;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving FoodRecipeType by ID: {ex.Message}");
				return null;
			}
		}

		// Update a FoodRecipeType
		public async Task<bool> UpdateFoodRecipeTypeAsync(string typeId, FoodRecipeType updatedType)
		{
			if (string.IsNullOrEmpty(typeId) || updatedType == null)
			{
				Debug.WriteLine("Invalid input: typeId or updatedType is null/empty");
				return false;
			}

			try
			{
				var typeDoc = _db.Collection("FoodRecipeTypes").Document(typeId);
				await typeDoc.SetAsync(updatedType, SetOptions.Overwrite);

				Debug.WriteLine($"FoodRecipeType {typeId} updated successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error updating FoodRecipeType: {ex.Message}");
				return false;
			}
		}

		// Soft delete a FoodRecipeType
		public async Task<bool> DeleteFoodRecipeTypeAsync(string typeId)
		{
			if (string.IsNullOrEmpty(typeId))
			{
				Debug.WriteLine("Invalid typeId: null or empty value provided");
				return false;
			}

			try
			{
				var typeDoc = _db.Collection("FoodRecipeTypes").Document(typeId);

				// Update the IsDeleted field to true
				await typeDoc.UpdateAsync("IsDeleted", true);

				Debug.WriteLine($"FoodRecipeType {typeId} deleted successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error deleting FoodRecipeType: {ex.Message}");
				return false;
			}
		}
	}
}
