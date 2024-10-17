using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditIngredientsPageView : ContentPage
{
	public EditIngredientsPageView(EditIngredientsPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}