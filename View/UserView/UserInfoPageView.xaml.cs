using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class UserInfoPageView : ContentPage
{
	public UserInfoPageView(UserInfoPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}