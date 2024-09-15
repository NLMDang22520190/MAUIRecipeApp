using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class RecipeVideo
{
    public int Vid { get; set; }

    public int? Frid { get; set; }

    public string VideoUrl { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public virtual FoodRecipe? Fr { get; set; }
}
