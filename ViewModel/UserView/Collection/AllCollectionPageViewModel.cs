using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView.Collection
{
    public partial class AllCollectionPageViewModel: ObservableObject
    {
       
        
        private FirestoreDb db = FirestoreService.Instance.Db;

        public AllCollectionPageViewModel()
        {

        }

        public void OnAppearing()
        {
            //throw new NotImplementedException();
        }
    }
}
