using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class UserRecipesView : ContentPage
{
	public UserRecipesView(UserRecipesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}