using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SicoreQMS.Common.Models.Operation;
using System.Diagnostics;
using System.Data.Entity.Validation;

namespace SicoreQMS.ViewModels
{
    public class TestRequestViewModel:BindableBase
    {

       public DelegateCommand BtnSumbit { get; set; }

  
        public TestRequestViewModel()
        {
            BtnSumbit = new DelegateCommand(OnSumbit);
        }

        private void OnSumbit()
        {
            
            if (string.IsNullOrEmpty(ProdName)|| string.IsNullOrEmpty(ProdType)|| string.IsNullOrEmpty(ProdLot))
            {
                MessageBox.Show("请完善数据后再提交");
                return;
            }
            string lastChar = _prodType.Substring(_prodType.Length - 1, 1).ToUpper();

            if (lastChar == "J")
            {
                QualityLevel = "军品";
            }
            else
            {
                QualityLevel = "民品";
            }

            using (var dbContext = new SicoreQMSEntities1())
            {
                // 创建一个新的 ProdInfo 对象
                ProdInfo newProdInfo = new ProdInfo
                {
                    Id = Guid.NewGuid().ToString(),

                    ProdName =this.ProdName,  // 替换成实际的值
                    ProdType = this.ProdType,  // 替换成实际的值
                    ProdStatus = 0,                 // 替换成实际的值
                    ProdLot = this.ProdLot,        // 替换成实际的值
                   // CheckLot = "YourCheckLot",      // 替换成实际的值
                    QualityLevel = this.QualityLevel,  // 替换成实际的值
                   // CreateDate = DateTime.Now.ToString(), // 使用当前时间作为字符串
                  //  CreateUser = "YourCreateUser",  // 替换成实际的值
                    //IsDeleted = false               // 替换成实际的值
                };

                // 将新的 ProdInfo 对象添加到数据库
                dbContext.ProdInfo.Add(newProdInfo);
                dbContext.SaveChanges();

                MessageBox.Show("新增成功!");

                ProdName = "";
                ProdType = "";
                ProdLot = "";
                // 保存更改

            }

        

        }

        private string _prodLot;

        private string _qualitylevel;
        private string _prodType;
        private string _prodName;

        public string ProdName
        {
            get { return _prodName; }
            set { SetProperty(ref _prodName, value); }
        }

        public string ProdType
        {
            get { return _prodType; }
            set { SetProperty(ref _prodType, value); }
        }

        public string ProdLot
        {
            get { return _prodLot; }
            set { SetProperty(ref _prodLot, value); }
        }




        public string QualityLevel
        {
            get { return _qualitylevel; }
            set { SetProperty(ref _qualitylevel, value); }
        }


  



    }
}
