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

        public string UploadName { get; set; }

        public string ImgUrl { get; set; } = string.Empty;

        public string CollectionName { get; set; }

        public string UIDString { get; set; }

        public string FCIDString { get; set; }
    }
}
