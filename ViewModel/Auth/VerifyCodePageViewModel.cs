using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.Auth
{
    public partial class VerifyCodePageViewModel: ObservableObject
    {
        [RelayCommand]
        public async Task VerifyCode()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
