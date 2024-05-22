using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Operation
{
    public partial class Equipment
    {
        public  Equipment(string n)
        {
            CreateDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Capacity = 0;
            AvailableCapacity = 0;
            IsDeleted = false;
           
        
        }
    }
}
