using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class FoodRecipeTypeMapping
{
    public int Frid { get; set; }

    public int Tofid { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual FoodRecipe Fr { get; set; } = null!;

    public virtual FoodRecipeType Tof { get; set; } = null!;
}
