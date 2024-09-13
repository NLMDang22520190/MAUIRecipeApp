using MAUIRecipeApp.ViewModel.Auth;

namespace MAUIRecipeApp.View.Auth;

public partial class PasswordVerifyCodePageView : ContentPage
{
	public PasswordVerifyCodePageView(PasswordVerifyCodePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}