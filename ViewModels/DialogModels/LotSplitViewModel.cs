using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class LotSplitViewModel : BindableBase, IDialogAware
    {


        #region 属性

  


        private ObservableCollection<SpiltModel> _spiltList;

        public ObservableCollection<SpiltModel> SpiltList
        {
            get { return _spiltList; }
            set { _spiltList = value; }
        }



        public DelegateCommand BtnCommitSpilt { get; set; }


        private Prod_Process _processes;

        public Prod_Process Processes
        {
            get { return _processes; }
            set { SetProperty(ref _processes, value); }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }



        private int _qty;


        public int Qty
        {
            get { return _qty; }
            set { SetProperty(ref _qty, value); }
        }


        #endregion
        public LotSplitViewModel()
        {
            Title = "拆批";
            Qty = 0;
            BtnCommitSpilt = new DelegateCommand(SpiltLot);
           
            SpiltList = new ObservableCollection<SpiltModel>();
        

        }



        private void GetSpiltList()
        {
            SpiltList.Add(new SpiltModel() { LotNo = "测试1", Qty = 2 });
            SpiltList.Add(new SpiltModel() { LotNo = "测试2", Qty = 5 });
            SpiltList.Add(new SpiltModel() { LotNo = "测试3", Qty = 8 });
           
            using (var context = new SicoreQMSEntities1())
            {
                var parentLot = context.ProdInfo.Find(Processes.ProdId);

                List<string> targetIds = new List<string>();

                var childernList = context.LotRelation.Where(s => s.ParentId == parentLot.Id).ToList();
                if (childernList.Count==0 )
                {
                    return;
                }

                foreach (var item in childernList)
                {
                    targetIds.Add(item.LotId);
                }
                var products = context.ProdInfo
                .Where(p => targetIds.Contains(p.Id))
                .ToList();
                if (products.Count == 0)
                {
                    return;
                }
                foreach (var product in products)
                {
                    SpiltList.Add(product.GetSpiltModel());
                }
            }

        }


        private void SpiltLot()
        {
            if (Qty > Processes.Qty)
            {
                MessageBox.Show("拆分数大于已有数量!");
                return;
            }
            using (var context=new SicoreQMSEntities1() )
         



        }

        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;



        public void GetProdDetail(string id)
        {
            using (var context = new SicoreQMSEntities1())
            {
                var prodprocee = context.Prod_Process.Find(id);
                if (prodprocee != null)
                {
                    Processes = prodprocee;
                    return;
                }
                MessageBox.Show("未获取到批次号");
            }

        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Id = parameters.GetValue<string>("Id");
            GetProdDetail(Id);
            GetSpiltList();
        }

    }
}
