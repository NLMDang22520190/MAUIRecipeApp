using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditUserSavedRecipePageView : ContentPage
{
	public EditUserSavedRecipePageView(EditUserSavedRecipePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}