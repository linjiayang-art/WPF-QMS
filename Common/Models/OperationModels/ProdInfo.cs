﻿using SicoreQMS.Common.Models.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Operation
{
    public partial class ProdInfo
    {
        public ProdInfo()
        {
          
           // 设置默认值
           ProdStatus = 0;
           IsDeleted = false;
           CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        }

        public SelectBasci ProductSelect()
        {
            return new SelectBasci { Label= ProdType ,Value=Id  };
        }

    }
}
