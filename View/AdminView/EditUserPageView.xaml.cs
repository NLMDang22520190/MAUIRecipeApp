using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditUserPageView : UraniumContentPage
{
	public EditUserPageView(EditUserPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}