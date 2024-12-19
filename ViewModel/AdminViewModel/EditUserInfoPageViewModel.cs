
using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.Models;
using System.Diagnostics;
using MAUIRecipeApp.Service;

namespace MAUIRecipeApp.ViewModel.AdminViewModel
{
   public partial class EditUserInfoPageViewModel: ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<UserDetail> userDetails = new ObservableCollection<UserDetail>();

        private readonly FirestoreDb _db;

        public EditUserInfoPageViewModel()
        {
            _db = FirestoreService.Instance.Db;
            if (_db == null)
            {
                Debug.WriteLine("Firestore DB is null");
                return;
            }
            LoadItem();
        }

        private void LoadItem()
        {
            LoadUserDetails();
        }

        private async void LoadUserDetails()
        {
            try
            {
                // Tham chiếu tới collection "UserDetails"
                CollectionReference userDetailsRef = _db.Collection("UserDetails");
                QuerySnapshot snapshot = await userDetailsRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        // Chuyển đổi document thành đối tượng UserDetail
                        UserDetail userDetail = document.ConvertTo<UserDetail>();
                Debug.WriteLine(userDetail.HealthCondition);

                // Thêm UserDetail vào ObservableCollection
                        UserDetails.Add(userDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }


    }
}
