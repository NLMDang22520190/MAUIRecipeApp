using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditRecipeIngredientsPageView : ContentPage
{
	public EditRecipeIngredientsPageView(EditRecipeIngredientsPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}