using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditIngredientsPageView : UraniumContentPage
{
	public EditIngredientsPageView(EditIngredientsPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}