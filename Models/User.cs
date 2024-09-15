using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class User
{
    public int Uid { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Provider { get; set; }

    public string? UserType { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<FoodRating> FoodRatings { get; set; } = new List<FoodRating>();

    public virtual ICollection<FoodRecipe> FoodRecipes { get; set; } = new List<FoodRecipe>();

    public virtual ICollection<UserDetail> UserDetails { get; set; } = new List<UserDetail>();

    public virtual ICollection<UserSavedRecipe> UserSavedRecipes { get; set; } = new List<UserSavedRecipe>();
}
