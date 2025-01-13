using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

[FirestoreData] // Đánh dấu lớp này là dữ liệu Firestore

public partial class UserSavedRecipe
{
    //[FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore
    public string UserSavedRecipeId { get; set; } // ID của UserSavedRecipe
    //public int Uid { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public DocumentReference FRID { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public DocumentReference UUID { get; set; }
    //[FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore


    //public int Frid { get; set; }
    //[FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    //public bool? IsDeleted { get; set; }
    //[FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    //public virtual FoodRecipe Fr { get; set; } = null!;
    //[FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    //public virtual User UidNavigation { get; set; } = null!;

    public string? UUIDString // UUID dưới dạng string để xử lý trong code
    {
        get => UUID?.Id; // Lấy Document ID từ DocumentReference
        set => UUID = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("User").Document(value);
    }

    public string? FRIDString // UUID dưới dạng string để xử lý trong code
    {
        get => FRID?.Id; // Lấy Document ID từ DocumentReference
        set => FRID = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("FoodRecipes").Document(value);
    }
}
