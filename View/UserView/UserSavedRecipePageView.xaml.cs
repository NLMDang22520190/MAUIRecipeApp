using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class UserSavedRecipePageView : ContentPage
{
	public UserSavedRecipePageView(UserSavedRecipePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as UserSavedRecipePageViewModel)?.OnAppearing();
    }
}