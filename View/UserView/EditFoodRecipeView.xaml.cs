using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class EditFoodRecipeView : ContentPage
{
	public EditFoodRecipeView(EditFoodRecipeViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}