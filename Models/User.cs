using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

[FirestoreData] // Đánh dấu lớp này là dữ liệu Firestore

public partial class User
{
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public string Uid { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore


    public string Username { get; set; } = null!;

    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public string Password { get; set; } = null!;
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public string Email { get; set; } = null!;
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public string? Provider { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public bool? UserType { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public bool? isAdmin { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public bool? IsDeleted { get; set; }

    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public float? Height { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public float? Weight { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public string? HealthCondition { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public string? Allergies { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore



    public virtual ICollection<FoodRating> FoodRatings { get; set; } = new List<FoodRating>();

    public virtual ICollection<FoodRecipe> FoodRecipes { get; set; } = new List<FoodRecipe>();

    public virtual ICollection<UserDetail> UserDetails { get; set; } = new List<UserDetail>();

    public virtual ICollection<UserSavedRecipe> UserSavedRecipes { get; set; } = new List<UserSavedRecipe>();
}
