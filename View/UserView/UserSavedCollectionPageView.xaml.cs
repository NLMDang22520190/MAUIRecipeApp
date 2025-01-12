using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class UserSavedCollectionPageView : ContentPage
{
	public UserSavedCollectionPageView(UserSavedCollectionPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        if (BindingContext is UserSavedCollectionPageViewModel vm)
        {
            var id =  (sender as Button).CommandParameter.ToString();
            if (id != null)
            {
                var result = await DisplayAlert("Delete Confirmation",
                    "Are you sure want to remove this colletion from your saved collection?", "Yes", "No");

                if(result)
                {
                    await vm.DeleteCollection(id);
                }
            }
           

        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is UserSavedCollectionPageViewModel vm)
        {
            vm.OnAppearing();
        }
    }
}