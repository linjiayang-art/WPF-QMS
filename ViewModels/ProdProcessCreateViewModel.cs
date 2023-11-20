using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SicoreQMS.Common.Models.Basic;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using SicoreQMS.Common.Models.Operation;
using Prism.Commands;
using System.Windows;
using Prism.Regions;

namespace SicoreQMS.ViewModels

{
    public class ProdProcessCreateViewModel : BindableBase
    {
        private ObservableCollection<SelectBasci> _productNameBasic;
        private ObservableCollection<Prod_ProcessModel> _processModel;

        private string _prodLot;

        public string ProdLot
        {
            get { return _prodLot ; }
            set { SetProperty(ref _prodLot, value); }
        }




        public DelegateCommand CommitBtnCommand { get; private set; }
        public ProdProcessCreateViewModel()
        {
            ProductNameBasic = new ObservableCollection<SelectBasci>();
            CreateProductSelection(ProductNameBasic);
            ProcessModel = new ObservableCollection<Prod_ProcessModel>();

            CommitBtnCommand = new DelegateCommand(CommitBtn);

            var dbConnt = new SicoreQMSEntities1();
            var allModel = dbConnt.Prod_ProcessModel.OrderBy(x=>x.ModelSort).ToList();
            foreach (var item in allModel)
            {
                ProcessModel.Add(item);

            }
        }

        public void CommitBtn()
        {
            if (string.IsNullOrEmpty(ProdLot))
            {
                return;
            }
            MessageBox.Show(ProdLot.ToString());
            //foreach (var item in ProcessModel)
            //{
            //    MessageBox.Show(item.SayContent());
            //}

        }


        public ObservableCollection<SelectBasci> ProductNameBasic
        {
            get { return _productNameBasic; }
            private set { _productNameBasic = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<Prod_ProcessModel> ProcessModel
        {
            get { return _processModel; }
            private set { _processModel = value; RaisePropertyChanged(); }

        }

        void CreateProductSelection(ObservableCollection<SelectBasci> selectBascis)
        {
            selectBascis.Add(new SelectBasci() { Label = "混频器", Value = "mixer" });
            selectBascis.Add(new SelectBasci() { Label = "混频器1", Value = "mixer1" });
            selectBascis.Add(new SelectBasci() { Label = "混频器2", Value = "mixer2" });
            selectBascis.Add(new SelectBasci() { Label = "混频器3", Value = "mixer3" });

        }

      


    }




}
