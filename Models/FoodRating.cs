using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

[FirestoreData]
public partial class FoodRating
{
    public string Rid { get; set; }

    [FirestoreProperty]
    public DocumentReference Uid { get; set; } // User ID

    [FirestoreProperty] 
    public DocumentReference Frid { get; set; } // Recipe ID

    [FirestoreProperty]
    public int Rating { get; set; }

    [FirestoreProperty]
    public string Review { get; set; }

    [FirestoreProperty]
    public DateTime DateRated { get; set; }

    [FirestoreProperty]
    public bool IsDeleted { get; set; }

    public string UidString
	{
		get => Uid?.Id;
		set => Uid = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("User").Document(value);
	}

    public string FridString
	{
		get => Frid?.Id;
		set => Frid = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("FoodRecipe").Document(value);
	}
}
