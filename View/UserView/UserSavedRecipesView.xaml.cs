using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class UserSavedRecipesView : ContentPage
{
	public UserSavedRecipesView(UserSavedRecipesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}