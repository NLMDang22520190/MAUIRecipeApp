using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.DTO
{
    public class RecipeIngredientDetailDto
    {
        public string RecipeName { get; set; }

        public string IngredientName { get; set; }

        public double Quantity { get; set; }

        public bool? IsDeleted { get; set; }

    }
}
