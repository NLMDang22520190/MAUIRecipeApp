using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditFoodRecipeTypePageView : UraniumContentPage
{
	public EditFoodRecipeTypePageView(EditFoodRecipeTypePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}