using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class EditIngredientsView : ContentPage
{
	public EditIngredientsView(EditIngredientsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	// Correct signature for the OnSearchTextChanged event handler
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            // Optionally, you can call the ViewModel method to handle the search
            var viewModel = BindingContext as EditIngredientsViewModel;
            if (viewModel != null)
            {
                viewModel.SearchText = e.NewTextValue; // Bind the search text to ViewModel
            }
        }
}