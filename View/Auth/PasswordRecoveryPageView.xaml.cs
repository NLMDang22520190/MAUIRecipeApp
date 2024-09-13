using MAUIRecipeApp.ViewModel.Auth;

namespace MAUIRecipeApp.View.Auth;

public partial class PasswordRecoveryPageView : ContentPage
{
	public PasswordRecoveryPageView(PasswordRecoveryPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}