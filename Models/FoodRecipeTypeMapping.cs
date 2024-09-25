using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

[FirestoreData] // Đánh dấu lớp này là dữ liệu Firestore
public partial class FoodRecipeTypeMapping
{
    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'Frid' trong Firestore
    public int Frid { get; set; }

    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'Tofid' trong Firestore
    public int Tofid { get; set; }

    [FirestoreProperty] // Ánh xạ thuộc tính này với trường 'IsDeleted' trong Firestore
    public bool? IsDeleted { get; set; }
}
