using MAUIRecipeApp.ViewModel.UserView;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace MAUIRecipeApp.View.UserView
{
	public partial class SubmitNewRecipeView : ContentPage
	{

		public SubmitNewRecipeView(SubmitNewRecipeViewModel vm)
		{
			InitializeComponent();
			BindingContext = vm;
		}

		// Handle DifficultySlider Value Changed
		private void DifficultySlider_ValueChanged(object sender, ValueChangedEventArgs e)
		{
			if (sender is Slider slider)
			{
				slider.Value = Math.Round(e.NewValue);
			}
		}
	}
}
