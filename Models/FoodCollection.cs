using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Models
{
    [FirestoreData] // Đánh dấu lớp này là dữ liệu Firestore
    public class FoodCollection
    {
        public string FoodCollectionId { get; set; }
        [FirestoreProperty]
        public string CollectionName { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public bool IsDeleted { get; set; }

        [FirestoreProperty]
        public DocumentReference UploaderId { get; set; }

        public string? UUIDString // UUID dưới dạng string để xử lý trong code
        {
            get => UploaderId?.Id; // Lấy Document ID từ DocumentReference
            set => UploaderId = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("User").Document(value);
        }
    }
}
