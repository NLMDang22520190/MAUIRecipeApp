using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Material.Controls;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditFoodRecipePageView : UraniumContentPage
{
	public EditFoodRecipePageView(EditFoodRecipePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is EditFoodRecipePageViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }

    private void TextField_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextField textField)
        {
            string searchText = textField.Text;
            var viewModel = BindingContext as EditFoodRecipePageViewModel;
            viewModel?.SearchFood(searchText);
        }
    }
}