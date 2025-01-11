using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.DTO
{
    public class UserSavedCollectionDTO
    {
        public string UserSavedCollectionId { get; set; }

        public string uploadName { get; set; }

        public string imgUrl { get; set; } = string.Empty;

        public string collectionName { get; set; }
    }
}
