using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

[FirestoreData] // Đánh dấu lớp này là dữ liệu Firestore
public partial class Ingredient
{
    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'Iid' trong Firestore
    public string Iid { get; set; }

    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'IngredientName' trong Firestore
    public string IngredientName { get; set; } = null!;

    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'MeasurementUnit' trong Firestore
    public string? MeasurementUnit { get; set; }

    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'IsDeleted' trong Firestore
    public bool? IsDeleted { get; set; }
}