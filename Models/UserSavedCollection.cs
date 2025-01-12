using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace MAUIRecipeApp.Models
{
    [FirestoreData]
    public class UserSavedCollection
    {
        public string UserSavedCollectionId { get; set; }

        [FirestoreProperty]

        public string? AlternateName { get; set; }

        [FirestoreProperty]
        public DocumentReference UserSavedId { get; set; }
        [FirestoreProperty]
        public DocumentReference FCID { get; set; }
        public string? UIDString
        {
            get => UserSavedId?.Id;
            set => UserSavedId = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("User").Document(value);
        }
        public string? FCIDString
        {
            get => FCID?.Id;
            set => FCID = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("FoodCollection").Document(value);
        }


    }
}
