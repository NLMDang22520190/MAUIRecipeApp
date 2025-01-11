using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class UserInfoPageView : ContentPage
{
	public UserInfoPageView()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
       BindingContext = new UserInfoPageViewModel();
    }
}