using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Interface;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SicoreQMS.Service;
using System.Windows.Forms.VisualStyles;

namespace SicoreQMS.ViewModels
{
    public class TestCreateViewModel : BindableBase
    {
        public TestCreateViewModel()
        {

            TestTypes = new ObservableCollection<CheckBasic>();
            ModelType = new ObservableCollection<SelectBasic>();

            HandelSelect = new DelegateCommand<string>(GetInfo);

            CheckCommand = new DelegateCommand<string>(ExcuteCheckCommand);
            TestModelItem = new ObservableCollection<TestProcessItem>();
            OnSumbit = new DelegateCommand<string>(CreateTestProcess);

            ProductNameBasic = ProdBasicService.CreateProductSelection();
            CreateData();


            ProdName = "放大器";
            ProdLot = "cksfd";
            ProdType = "siv0056-j";



        }

        private void GetInfo(string parameter)
        {
            if (parameter == null)
            {
                return;
            }

            using (var context = new SicoreQMSEntities1())
            {
                var productInfo = context.ProdInfo.SingleOrDefault(b => b.Id == parameter);
                if (productInfo != null)
                {
                    this.ProdName = productInfo.ProdName;
                    this.ProdLot = productInfo.ProdLot;
                    this.ProdType = productInfo.ProdType;
                    this.ProdNumber = productInfo.ProdNumber;
                    this.Prodstandard = productInfo.Prodstandard;
                    this.TestLot = productInfo.TestLot;
                    GetTemplate(parameter);
                }
                else
                {
                    System.Windows.MessageBox.Show("未查询到该产品");
                }

            }

        }

        private void ExcuteCheckCommand(string obj)
        {
            for (int i = 0; i < TestTypes.Count; i++)
            {
                if (TestTypes[i].Label != obj)
                {
                    TestTypes[i].IsCheck = false;
                }
            }
            return;
        }

        private void CreateTestProcess(string modelId)
        {

            using (var context = new SicoreQMSEntities1())
            {
              

                context.SaveChanges();

            }
            return;

        }

        private void GetTemplate(string obj)
        {
            if (obj == null)
            {
                return;
            }
            TestModelItem.Clear();
            using (var contxt = new SicoreQMSEntities1())
            {
                var items = contxt.TestProcessItem.Where(p => p.ProdId== obj).OrderByDescending(p => p.ExperimentItemNo).ToList();

                if (items.Count == 0)
                {
                    return;
                }

                foreach (var item in items)
                {
                    TestModelItem.Add(item);

                }

            }
        }
        #region 属性

        public DelegateCommand<string> CheckCommand { get; private set; }

        public DelegateCommand<string> OnSumbit { get; private set; }

        private ObservableCollection<SelectBasic> _productNameBasic;

        public ObservableCollection<SelectBasic> ProductNameBasic
        {
            get { return _productNameBasic; }
            set { _productNameBasic = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<string> HandelSelect { get; set; }

        private ObservableCollection<SelectBasic> _modelType;

        private ObservableCollection<TestProcessItem> _testModelItem;

        private string _prodName;

        public string ProdName
        {
            get { return _prodName; }
            set { SetProperty(ref _prodName, value); }
        }

        private string _prodType;

        public string ProdType
        {
            get { return _prodType; }
            set { SetProperty(ref _prodType, value); }
        }

        private string _prodlot;

        public string ProdLot
        {
            get { return _prodlot; }
            set { SetProperty(ref _prodlot, value); }
        }

        private string _Prodstandard;

        public string Prodstandard
        {
            get { return _Prodstandard; }
            set { SetProperty(ref _Prodstandard, value); }
        }

        private string _prodNmuber;

        public string ProdNumber
        {
            get { return _prodNmuber; }
            set { SetProperty(ref _prodNmuber, value); }

        }

        private string _testLot;

        public string TestLot
        {
            get { return _testLot; }
            set { SetProperty(ref _testLot, value); }
        }



        public ObservableCollection<TestProcessItem> TestModelItem
        {
            get { return _testModelItem; }
            set { _testModelItem = value; RaisePropertyChanged(); }
        }


        public ObservableCollection<SelectBasic> ModelType
        {
            get { return _modelType; }
            set { _modelType = value; }
        }

        private ObservableCollection<CheckBasic> _testTypes;

        public ObservableCollection<CheckBasic> TestTypes
        {
            get { return _testTypes; }
            set { _testTypes = value; RaisePropertyChanged(); }
        }
        #endregion

        void CreateData()
        {
            TestTypes.Add(new CheckBasic() { Label = "筛选", IsCheck = false });
            TestTypes.Add(new CheckBasic() { Label = "鉴定", IsCheck = false });
            TestTypes.Add(new CheckBasic() { Label = "质量一致性", IsCheck = false });
            TestTypes.Add(new CheckBasic() { Label = "研发验证", IsCheck = false });
            TestTypes.Add(new CheckBasic() { Label = "其它", IsCheck = false });

            using (var context = new SicoreQMSEntities1())
            {
                var selectItem = context.TestModelBasic.ToList();

                foreach (var item in selectItem)
                {
                    ModelType.Add(item.GetSelection());
                }

            }


        }
    }
}
