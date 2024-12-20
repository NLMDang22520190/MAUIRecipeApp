using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditUserSavedRecipePageView : UraniumContentPage
{
	public EditUserSavedRecipePageView(EditUserSavedRecipePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}