using MauiIcons.Core;
using MAUIRecipeApp.ViewModel.Auth;

namespace MAUIRecipeApp.View.Auth;

public partial class SignUpPageView : ContentPage
{
	public SignUpPageView(SignUpPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
    }
}