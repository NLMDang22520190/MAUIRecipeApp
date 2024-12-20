using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

[FirestoreData] // Đánh dấu lớp này là dữ liệu Firestore

public partial class UserDetail
{
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public int Udid { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public int? Uid { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public DocumentReference? UUID { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public float? Height { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public float? Weight { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public string? HealthCondition { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public string? Allergies { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public bool? IsDeleted { get; set; }
    [FirestoreProperty] // Ánh xạ thuộc tính với trường 'Frid' trong Firestore

    public virtual User? UidNavigation { get; set; }
    public string? UUIDString // UUID dưới dạng string để xử lý trong code
    {
        get => UUID?.Id; // Lấy Document ID từ DocumentReference
        set => UUID = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("User").Document(value);
    }
}
