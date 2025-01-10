using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Dialogs.Mopups;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditFoodRecipeTypePageView : UraniumContentPage
{
	public EditFoodRecipeTypePageView(EditFoodRecipeTypePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EditFoodRecipeTypePageViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }


    private void TextField_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (BindingContext is EditFoodRecipeTypePageViewModel viewModel)
        {
            viewModel.SearchFoodType(e.NewTextValue);
        }
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        var result = await this.DisplayTextPromptAsync("Add New Food Type", "Food Type Name", placeholder: "Enter food type name...");

        if (BindingContext is EditFoodRecipeTypePageViewModel viewModel)
        {
            var AddResult = await viewModel.AddFoodType(result);
            if (AddResult)
            {
                await DisplayAlert("Result:", "Add successfully!", "OK");
            }
            else
            {
                await DisplayAlert("Result:", "Add failed!", "OK");
            }
        }
   
    }
}