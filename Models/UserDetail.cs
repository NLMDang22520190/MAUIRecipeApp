using System;
using System.Collections.Generic;

namespace MAUIRecipeApp.Models;

public partial class UserDetail
{
    public int Udid { get; set; }

    public int? Uid { get; set; }

    public decimal? Height { get; set; }

    public decimal? Weight { get; set; }

    public string? HealthCondition { get; set; }

    public string? Allergies { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual User? UidNavigation { get; set; }
}
