using MAUIRecipeApp.DTO;
using MAUIRecipeApp.ViewModel.AdminViewModel;
using System.Diagnostics;
using UraniumUI.Dialogs.Mopups;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditIngredientsPageView : UraniumContentPage
{
	public EditIngredientsPageView(EditIngredientsPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EditIngredientsPageViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }

    private void TextField_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (BindingContext is EditIngredientsPageViewModel viewModel)
        {
            viewModel.SearchIngredient(e.NewTextValue);
        }
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        var ingredientViewModel = new AddNewIngredientDTO();

        var result = await this.DisplayFormViewAsync<AddNewIngredientDTO>(
            "Add New Ingredient",   // Title of the form
            ingredientViewModel,    // ViewModel for the form
            "OK",                   // Submit button text
            "Cancel"                // Cancel button text
        );

        // Xử lý kết quả
        if (result != null)
        {
            if (BindingContext is EditIngredientsPageViewModel viewModel)
            {
                var AddResult = await viewModel.AddIngredient(result);
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
        else
        {
            Debug.WriteLine("User cancelled the form.");
        }

    }
}