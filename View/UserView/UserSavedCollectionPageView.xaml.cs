using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class UserSavedCollectionPageView : ContentPage
{
	public UserSavedCollectionPageView(UserSavedCollectionPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}