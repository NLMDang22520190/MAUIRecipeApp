using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Material.Controls;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditUserPageView : UraniumContentPage
{
	public EditUserPageView(EditUserPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EditUserPageViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }

    private void TextField_OnCompleted(object? sender, EventArgs e)
    {
        if (sender is TextField textField)
        {
            string searchText = textField.Text;
            var viewModel = BindingContext as EditUserPageViewModel;
            viewModel?.SearchUser(searchText);
        }
    }
}