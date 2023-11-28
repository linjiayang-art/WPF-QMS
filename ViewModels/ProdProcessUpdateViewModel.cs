using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SicoreQMS.ViewModels
{
    public class ProdProcessUpdateViewModel:BindableBase, IRegionMemberLifetime
    {
        
        public ProdProcessUpdateViewModel(IDialogService dialog)
        {
            ProductNameBasic = new ObservableCollection<SelectBasci>();

            UpdateProgressCommand =new DelegateCommand<Prod_ProcessItem>(UpdateProgress);

            SplitLotCommand = new DelegateCommand<SelectBasci>(SpiltLot);

            CreateProductSelection();

            ProcessItem = new ObservableCollection<Prod_ProcessItem>();

            HandelSelect = new DelegateCommand<string>(GetInfo);
            QualityLevel = "军品";
            this.dialog = dialog;

        }

        private void SpiltLot(SelectBasci obj)
        {
            if (string.IsNullOrEmpty(obj.Value))
            {
                return;
            }
            DialogParameters dialogParameters = new DialogParameters
            {
                { "Id",obj.Value},

            };

            dialog.ShowDialog("LotSplitView", dialogParameters, result => {

              var newid = result.Parameters.GetValue<string>("NewId");
                if (!string.IsNullOrEmpty(newid))
                {
                    using (var context = new SicoreQMSEntities1())
                    {
                        var productItem = context.Prod_Process.SingleOrDefault(p => p.Id == newid);
                        ProductNameBasic.Add(productItem.ProductSelect());
                    }
                }

                GetProcessList();
            });
        }

        private void UpdateProgress(Prod_ProcessItem obj)
        {
        
            DialogParameters dialogParameters = new DialogParameters
            {
                { "Id",obj.Id},
   
            };
            dialog.ShowDialog("ProcessUpdateView", dialogParameters, result =>
            {
                GetProcessList();

            } );
           
        }

        public bool KeepAlive => false;
        #region 属性
        private ObservableCollection<SelectBasci> _productNameBasic;
        private readonly IDialogService dialog;
        private ObservableCollection<Prod_ProcessItem> _processItem;

       public DelegateCommand<Prod_ProcessItem> UpdateProgressCommand { get; set; }
        public DelegateCommand<SelectBasci> SplitLotCommand { get; set; }
        private string _prodLot;
        private string _prodName;
        private string _qualitylevel;

   


        private string _prodType;

        private string ProdProcessId { get; set; }

        

        public string QualityLevel
        {
            get { return _qualitylevel; }
            set { SetProperty(ref _qualitylevel, value); }
        }


        public string ProdLot
        {
            get { return _prodLot; }
            set { SetProperty(ref _prodLot, value); }
        }

        public string ProdType
        {
            get { return _prodType; }
            set { SetProperty(ref _prodType, value); }
        }

        public string ProdName
        {
            get { return _prodName; }
            set { SetProperty(ref _prodName, value); }
        }

        #endregion


        private void GetInfo(string parameter)
        {
            if (parameter == null)
            {
                return;
            }
         
                ProdProcessId = parameter;

                using (var context = new SicoreQMSEntities1())
                {
                    var productInfo = context.Prod_Process.SingleOrDefault(b => b.Id == ProdProcessId);
                    if (productInfo != null)
                    {
                        this.ProdName = productInfo.ProdName;
                        this.ProdLot = productInfo.ProdLot;
                        this.QualityLevel = productInfo.QualityLevel;
                    }
                    else
                    {
                    System.Windows.MessageBox.Show("未查询到该产品");
                    }

                }
                GetProcessList();
            
          

          
        }
 

        public ObservableCollection<SelectBasci> ProductNameBasic
        {
            get { return _productNameBasic; }
           set { _productNameBasic = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<Prod_ProcessItem> ProcessItem
        {
            get { return _processItem; }
            private set { _processItem = value; RaisePropertyChanged(); }

        }

        public void CreateProductSelection()
        {

            var newProductNameBasic = new ObservableCollection<SelectBasci>();
          
            using (var context = new SicoreQMSEntities1())
            {
                var productItem = context.Prod_Process
                    .Where(b => b.ProdStatus == 0|| b.ProdStatus==5).ToList().OrderBy(x => x.CreateDate);
                foreach (var item in productItem)
                {
                    ProductNameBasic.Add(item.ProductSelect());
                }
            }
            //ProductNameBasic = newProductNameBasic;
        }

        void GetProcessList()
        {
            using (var context = new SicoreQMSEntities1())
            {
                var allModel = context.Prod_ProcessItem.Where(b => b.ProdProcessId == ProdProcessId).ToList().OrderBy(x => x.ModelSort);
                foreach (var item in allModel)
                {
                    ProcessItem.Add(item);

                }
            }
        }

        public DelegateCommand CommitBtnCommand { get; private set; }


        public DelegateCommand<string> HandelSelect
        {
            get;
            private set;
        }

      
    }
}
