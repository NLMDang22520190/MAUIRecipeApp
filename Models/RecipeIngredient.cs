using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class RecipeIngredient
{
    public int Frid { get; set; }

    public int Iid { get; set; }

    public decimal? Quantity { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual FoodRecipe Fr { get; set; } = null!;

    public virtual Ingredient IidNavigation { get; set; } = null!;
}
