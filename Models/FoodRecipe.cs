using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class FoodRecipe
{
    public int Frid { get; set; }

    public string RecipeName { get; set; } = null!;

    public int? Calories { get; set; }

    public string? DifficultyLevel { get; set; }

    public string? HealthBenefits { get; set; }

    public int? CookingTime { get; set; }

    public int? Portion { get; set; }

    public int? UploaderUid { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<FoodRating> FoodRatings { get; set; } = new List<FoodRating>();

    public virtual ICollection<FoodRecipeTypeMapping> FoodRecipeTypeMappings { get; set; } = new List<FoodRecipeTypeMapping>();

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    public virtual ICollection<RecipeVideo> RecipeVideos { get; set; } = new List<RecipeVideo>();

    public virtual User? UploaderU { get; set; }

    public virtual ICollection<UserSavedRecipe> UserSavedRecipes { get; set; } = new List<UserSavedRecipe>();
}
