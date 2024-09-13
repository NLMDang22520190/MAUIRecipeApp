using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.Auth
{
    public partial class SignUpPageViewModel: ObservableObject
    {
        [RelayCommand]
        public async Task SignUp()
        {
            await Shell.Current.GoToAsync("//verifycode");
        }
    }
}
