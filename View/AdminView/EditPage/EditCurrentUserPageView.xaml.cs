using MAUIRecipeApp.ViewModel.AdminViewModel.EditPage;

namespace MAUIRecipeApp.View.AdminView.EditPage;

public partial class EditCurrentUserPageView : ContentPage
{
	public EditCurrentUserPageView(EditCurrentUserPageViewModal vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is EditCurrentUserPageViewModal viewModel)
        {
            viewModel.OnAppearing();
        }
    }
}