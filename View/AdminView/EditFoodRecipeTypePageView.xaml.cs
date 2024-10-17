using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditFoodRecipeTypePageView : ContentPage
{
	public EditFoodRecipeTypePageView(EditFoodRecipeTypePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}