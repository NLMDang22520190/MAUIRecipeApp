using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

[FirestoreData] // Đánh dấu lớp này là một thực thể dữ liệu của Firestore
public partial class RecipeIngredient
{
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore
    public string Frid { get; set; }

    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Iid' trong Firestore
    public string Iid { get; set; }

    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Quantity' trong Firestore
    public double? Quantity { get; set; }

    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'IsDeleted' trong Firestore
    public bool? IsDeleted { get; set; }
}