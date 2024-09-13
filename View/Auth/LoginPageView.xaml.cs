using MauiIcons.Core;
using MAUIRecipeApp.ViewModel.Auth;

namespace MAUIRecipeApp.View.Auth;

public partial class LoginPageView : ContentPage
{
	public LoginPageView(LoginPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;

    }
}