using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Service
{
	public class FoodRatingService
	{
		private static FoodRatingService _instance;
		private static readonly object _lock = new object();
		private readonly FirestoreDb _db;

		// Private constructor
		private FoodRatingService()
		{
			_db = FirestoreService.Instance.Db;
			if (_db == null)
			{
				Debug.WriteLine("Firestore DB is null");
			}
		}

		// Singleton instance property
		public static FoodRatingService Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new FoodRatingService();
						}
					}
				}
				return _instance;
			}
		}

		// Retrieve all ratings for a specific recipe
		public async Task<ObservableCollection<FoodRating>> GetRatingsByRecipeId(int recipeId)
		{
			try
			{
				var ratingsRef = _db.Collection("FoodRatings");
				var query = ratingsRef
					.WhereEqualTo("Frid", recipeId)
					.WhereEqualTo("IsDeleted", false);
				var snapshot = await query.GetSnapshotAsync();

				var ratings = new ObservableCollection<FoodRating>();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var rating = document.ConvertTo<FoodRating>();
						rating.Rid = int.Parse(document.Id); // Assign RID from document ID
						ratings.Add(rating);
					}
				}

				return ratings;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving ratings: {ex.Message}");
				return new ObservableCollection<FoodRating>();
			}
		}

		// Add a new rating
		public async Task<bool> AddRating(FoodRating rating)
		{
			try
			{
				var ratingsCollection = _db.Collection("FoodRatings");

				// Generate a custom ID if needed
				string ratingId = DateTime.UtcNow.Ticks.ToString();

				rating.Rid = int.Parse(ratingId); // Assign ID to the rating
				await ratingsCollection.Document(ratingId).SetAsync(rating);

				Debug.WriteLine("Rating added successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding rating: {ex.Message}");
				return false;
			}
		}

		// Update an existing rating
		public async Task<bool> UpdateRating(int ratingId, FoodRating updatedRating)
		{
			try
			{
				var ratingDoc = _db.Collection("FoodRatings").Document(ratingId.ToString());
				await ratingDoc.SetAsync(updatedRating, SetOptions.Overwrite);

				Debug.WriteLine("Rating updated successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error updating rating: {ex.Message}");
				return false;
			}
		}

		// Soft delete a rating
		public async Task<bool> DeleteRating(int ratingId)
		{
			try
			{
				var ratingDoc = _db.Collection("FoodRatings").Document(ratingId.ToString());

				// Update the IsDeleted field to true
				await ratingDoc.UpdateAsync("IsDeleted", true);

				Debug.WriteLine("Rating deleted successfully");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error deleting rating: {ex.Message}");
				return false;
			}
		}

		// Retrieve all ratings for a specific user
		public async Task<ObservableCollection<FoodRating>> GetRatingsByUserId(int userId)
		{
			try
			{
				var ratingsRef = _db.Collection("FoodRatings");
				var query = ratingsRef
					.WhereEqualTo("Uid", userId)
					.WhereEqualTo("IsDeleted", false);
				var snapshot = await query.GetSnapshotAsync();

				var ratings = new ObservableCollection<FoodRating>();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var rating = document.ConvertTo<FoodRating>();
						rating.Rid = int.Parse(document.Id); // Assign RID from document ID
						ratings.Add(rating);
					}
				}

				return ratings;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error retrieving user ratings: {ex.Message}");
				return new ObservableCollection<FoodRating>();
			}
		}
	}
}
