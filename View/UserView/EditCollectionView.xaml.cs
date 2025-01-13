using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class EditCollectionView : ContentPage
{
	public EditCollectionView(EditCollectionViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}