using MAUIRecipeApp.ViewModel.Auth;

namespace MAUIRecipeApp.View.Auth;

public partial class NewPasswordPageView : ContentPage
{
	public NewPasswordPageView(NewPasswordPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}