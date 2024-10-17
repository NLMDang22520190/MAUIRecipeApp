using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditFoodRecipeMappingPageView : ContentPage
{
	public EditFoodRecipeMappingPageView(EditFoodRecipeMappingPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}