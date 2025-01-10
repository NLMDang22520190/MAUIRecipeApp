using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

[FirestoreData]
public class FoodRecipe
{
    [FirestoreProperty]
    public string Frid { get; set; }

    [FirestoreProperty]
    public string RecipeName { get; set; } = null!;

    [FirestoreProperty]
    public int? Calories { get; set; }

    [FirestoreProperty]
    public string? DifficultyLevel { get; set; }

    [FirestoreProperty]
    public string? HealthBenefits { get; set; }

    [FirestoreProperty]
    public int? CookingTime { get; set; }

    [FirestoreProperty]
    public int? Portion { get; set; }

    [FirestoreProperty]
    public int? UploaderUid { get; set; }

    [FirestoreProperty]
    public string? ImgUrl { get; set; }

    [FirestoreProperty]
    public string? VideoUrl { get; set; }

    [FirestoreProperty]
    public bool? IsDeleted { get; set; } = false;

    [FirestoreProperty] public bool? IsApproved { get; set; } = false;
}