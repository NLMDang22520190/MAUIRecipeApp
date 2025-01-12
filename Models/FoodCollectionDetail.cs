using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace MAUIRecipeApp.Models
{
    [FirestoreData]
    public class FoodCollectionDetail
    {
        public string? FoodCollectionDetailId { get; set; }

        [FirestoreProperty]
        public DocumentReference FCID { get; set; }

        [FirestoreProperty]
        public DocumentReference FRID { get; set; }

        public string? FCIDString
        {
            get => FCID?.Id;
            set => FCID = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("FoodCollection").Document(value);
        }

        public string? FRIDString
        {
            get => FRID?.Id;
            set => FRID = string.IsNullOrEmpty(value) ? null : FirestoreDb.Create(null).Collection("FoodRecipe").Document(value);
        }

    }
}
