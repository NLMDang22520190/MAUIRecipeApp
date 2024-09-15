using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class Ingredient
{
    public int Iid { get; set; }

    public string IngredientName { get; set; } = null!;

    public string? MeasurementUnit { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
