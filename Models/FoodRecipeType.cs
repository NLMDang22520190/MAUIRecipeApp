using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class FoodRecipeType
{
    public string Tofid { get; set; }

    public string FoodTypeName { get; set; } = null!;

    public bool? IsDeleted { get; set; }

}
