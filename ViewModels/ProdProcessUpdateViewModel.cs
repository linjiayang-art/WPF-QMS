using Prism.Commands;
using Prism.Mvvm;
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

namespace SicoreQMS.ViewModels
{
    public class ProdProcessUpdateViewModel:BindableBase
    {
        
        public ProdProcessUpdateViewModel(IDialogService dialog)
        {
            ProductNameBasic = new ObservableCollection<SelectBasci>();
            UpdateProgressCommand=new DelegateCommand<Prod_ProcessItem>(UpdateProgress);
            CreateProductSelection(ProductNameBasic);

            ProcessItem = new ObservableCollection<Prod_ProcessItem>();

            HandelSelect = new DelegateCommand<SelectBasci>(GetInfo);
            QualityLevel = "军品";
            this.dialog = dialog;


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
        #region 属性
        private ObservableCollection<SelectBasci> _productNameBasic;
        private readonly IDialogService dialog;
        private ObservableCollection<Prod_ProcessItem> _processItem;

       public DelegateCommand<Prod_ProcessItem> UpdateProgressCommand { get; set; }

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


        private void GetInfo(SelectBasci parameter)
        {
            if (parameter is null)
            {
                return;
            }
            ProdProcessId = parameter.Value;
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
                    MessageBox.Show("未查询到该产品");
                }

            }
            GetProcessList();
        }
 

        public ObservableCollection<SelectBasci> ProductNameBasic
        {
            get { return _productNameBasic; }
            private set { _productNameBasic = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<Prod_ProcessItem> ProcessItem
        {
            get { return _processItem; }
            private set { _processItem = value; RaisePropertyChanged(); }

        }

        void CreateProductSelection(ObservableCollection<SelectBasci> selectBascis)
        {
            using (var context = new SicoreQMSEntities1())
            {
                var productItem = context.Prod_Process
                    .Where(b => b.ProdStatus == 0).ToList().OrderBy(x => x.CreateDate);
                foreach (var item in productItem)
                {
                    selectBascis.Add(item.ProductSelect());
                }
            }
         

        }


        void GetProcessList()
        {
            ProcessItem.Clear();
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


        public DelegateCommand<SelectBasci> HandelSelect
        {
            get;
            private set;
        }
    }
}
