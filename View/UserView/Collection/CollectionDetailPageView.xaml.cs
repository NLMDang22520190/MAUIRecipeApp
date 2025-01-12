using MAUIRecipeApp.ViewModel.UserView.Collection;

namespace MAUIRecipeApp.View.UserView.Collection;

public partial class CollectionDetailPageView : ContentPage
{
    public CollectionDetailPageView(CollectionDetailPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CollectionDetailPageViewModel vm)
        {
            vm.OnAppearing();
        }
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        if (BindingContext is CollectionDetailPageViewModel vm)
        {
            var result = await DisplayPromptAsync("New Collection Name", "Enter New Collection Name");

            if (!string.IsNullOrEmpty(result))
            {
                await vm.UpdateName(result);
            }

        }
    }
}