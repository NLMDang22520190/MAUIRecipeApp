using MAUIRecipeApp.ViewModel.AdminViewModel.EditPage;

namespace MAUIRecipeApp.View.AdminView.EditPage;

public partial class EditCurrentFoodTypePageView : ContentPage
{
	public EditCurrentFoodTypePageView(EditCurrentFoodTypePageViewModal vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EditCurrentFoodTypePageViewModal viewModel)
        {
            viewModel.OnAppearing();
        }
    }
}