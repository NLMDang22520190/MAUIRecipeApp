using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class HomePageViewModel: ObservableObject
    {
        [RelayCommand]
        public async Task FoodDetail()
        {
            await Shell.Current.GoToAsync("//fooddetail");
        }
    }
}
