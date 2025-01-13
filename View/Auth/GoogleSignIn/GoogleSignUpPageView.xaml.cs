using MAUIRecipeApp.ViewModel.Auth.GoogleSignIn;

namespace MAUIRecipeApp.View.Auth.GoogleSignIn;

public partial class GoogleSignUpPageView : ContentPage
{
	public GoogleSignUpPageView(GoogleSignUpPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}