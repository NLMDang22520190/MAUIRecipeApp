using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class UserSavedRecipe
{
    public int Uid { get; set; }

    public int Frid { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual FoodRecipe Fr { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}
