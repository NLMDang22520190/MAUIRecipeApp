using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Models
{
	public class IngredientInfos
	{
		public string Frid { get; set; }
		public string Iid { get; set; }
		public string Name { get; set; }
		public double Quantity { get; set; }
		public string Unit { get; set; }
		public bool IsDeleted { get; set; }
	}
}
