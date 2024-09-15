using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class FoodRecipeType
{
    public int Tofid { get; set; }

    public string FoodTypeName { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public virtual ICollection<FoodRecipeTypeMapping> FoodRecipeTypeMappings { get; set; } = new List<FoodRecipeTypeMapping>();
}
