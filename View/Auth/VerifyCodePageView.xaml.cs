using MAUIRecipeApp.ViewModel.Auth;

namespace MAUIRecipeApp.View.Auth;

public partial class VerifyCodePageView : ContentPage
{
	public VerifyCodePageView(VerifyCodePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}