using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Operation
{
    public partial class TestModelBasic : ISelectItem
    {
        public SelectBasci GetSelection()
        {
            return new SelectBasci()
            {
                Label=ModelName,
                Value=ModelId
            };
        }
    }
}
