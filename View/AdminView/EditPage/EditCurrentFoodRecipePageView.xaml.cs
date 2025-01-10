using MAUIRecipeApp.ViewModel.AdminViewModel.EditPage;

namespace MAUIRecipeApp.View.AdminView.EditPage;

public partial class EditCurrentFoodRecipePageView : ContentPage
{
	public EditCurrentFoodRecipePageView(EditCurrentFoodRecipePageViewModal vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EditCurrentFoodRecipePageViewModal viewModel)
        {
            viewModel.OnAppearing();
        }
    }
   
}