﻿using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class FoodRating
{
    public int Rid { get; set; }

    public int? Uid { get; set; }

    public int? Frid { get; set; }

    public int? Rating { get; set; }

    public string? Review { get; set; }

    public DateTime? DateRated { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual FoodRecipe? Fr { get; set; }

    public virtual User? UidNavigation { get; set; }
}
