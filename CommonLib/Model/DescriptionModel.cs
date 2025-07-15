using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class DescriptionModel
    {
        public DescriptionModel()
        {
            PublicDescriptionModel = new PublicDescriptionModel();
        }

        public string LineDescription { get; set; }
        public PublicDescriptionModel PublicDescriptionModel { get; set; }
    }

    public class PublicDescriptionModel
    {
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
    }
}
