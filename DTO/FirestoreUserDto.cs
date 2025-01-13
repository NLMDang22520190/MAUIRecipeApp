using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.DTO
{
    [FirestoreData]
    public class FirestoreUserDto
    {
        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public string Password { get; set; }

        [FirestoreProperty]
        public bool isAdmin { get; set; }

        [FirestoreProperty]
        public bool isDeactivated { get; set; }

        // Constructor mặc định
        public FirestoreUserDto() { }
    }
}
