using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

[FirestoreData] // Đánh dấu lớp này là dữ liệu Firestore
public partial class FoodRecipeType
{
    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'Tofid' trong Firestore
    public string Tofid { get; set; }

    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'FoodTypeName' trong Firestore
    public string FoodTypeName { get; set; } = null!;

    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'IsDeleted' trong Firestore
    public bool? IsDeleted { get; set; }
}