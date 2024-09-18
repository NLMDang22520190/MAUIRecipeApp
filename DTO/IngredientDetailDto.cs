using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.DTO
{
    public class IngredientDetailDto
    {
        public string IngredientName { get; set; }
        public decimal Quantity { get; set; }
        public string MeasurementUnit { get; set; }
    }
}
