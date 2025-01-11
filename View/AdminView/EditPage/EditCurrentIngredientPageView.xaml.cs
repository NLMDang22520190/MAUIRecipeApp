using MAUIRecipeApp.ViewModel.AdminViewModel.EditPage;

namespace MAUIRecipeApp.View.AdminView.EditPage;

public partial class EditCurrentIngredientPageView : ContentPage
{
	public EditCurrentIngredientPageView(EditCurrentIngredientPageViewModal vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EditCurrentIngredientPageViewModal viewModel)
        {
            viewModel.OnAppearing();
        }
    }
}