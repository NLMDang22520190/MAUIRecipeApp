using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditFoodRecipePageView : ContentPage
{
	public EditFoodRecipePageView(EditFoodRecipePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}