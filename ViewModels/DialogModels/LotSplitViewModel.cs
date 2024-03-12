using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Diagnostics;
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

        public string NewId { get; set; }


        private ObservableCollection<SpiltModel> _spiltList;

        public ObservableCollection<SpiltModel> SpiltList
        {
            get { return _spiltList; }
            set { _spiltList = value; }
        }

        public IEventAggregator Aggregator { get; }

        public DelegateCommand BtnCommitSpilt { get; set; }


        private Prod_Process _processes;

        public Prod_Process Processes
        {
            get { return _processes; }
            set { SetProperty(ref _processes, value); }
        }

        private ProdInfo prodData;

        public ProdInfo ProdData
        {
            get { return prodData; }
            set { prodData = value; RaisePropertyChanged(); }
        }



        private string _id;
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }



        private int _splitQty;


        public int SplitQty
        {
            get { return _splitQty; }
            set { SetProperty(ref _splitQty, value); }
        }


        #endregion
        public LotSplitViewModel(IEventAggregator aggregator)
        {
            Title = "拆批";
            SplitQty = 0;
            BtnCommitSpilt = new DelegateCommand(SpiltLot);

            SpiltList = new ObservableCollection<SpiltModel>();
            Aggregator = aggregator;


        }



        private void GetSpiltList()
        {
            //SpiltList.Add(new SpiltModel() { LotNo = "测试1", Qty = 2 });
            //SpiltList.Add(new SpiltModel() { LotNo = "测试2", Qty = 5 });
            //SpiltList.Add(new SpiltModel() { LotNo = "测试3", Qty = 8 });
            using (var context = new SicoreQMSEntities1())
            {
                var parentLot = context.ProdInfo.Find(Processes.ProdId);

                List<string> targetIds = new List<string>();

                var childernList = context.LotRelation.Where(s => s.ParentId == parentLot.Id && s.RelationType == "spilt").ToList();
                if (childernList.Count == 0)
                {
                    return;
                }
                foreach (var item in childernList)
                {
                    targetIds.Add(item.ProdId);
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

            if (SplitQty > ProdData.Qty)
            {
                Aggregator.SendMessage("拆分数大于已有数量!");
               //MessageBox.Show("拆分数大于已有数量!");
                return;
            }
            using (var context = new SicoreQMSEntities1())
            {
                var postCount = context.LotRelation.Where(p => p.ParentId == Processes.ProdId && p.RelationType == "spilt").Count();
                string childernNumber = Processes.ProdLot;
                if (postCount == 0)
                {
                    childernNumber += ".A";

                }
                if (postCount > 0)
                {

                    childernNumber += "." + Factory.ProcessNumberToLetter(postCount);
                }
                CreateSpiltInfo(childernNumber, SplitQty);
            }

            ButtonResult result = ButtonResult.None;
            var parameters = new DialogParameters
            {
                { "NewId", this.NewId }
            };
            RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(result, parameters));

        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {

            RequestClose.Invoke(dialogResult);
        }

        public List<Prod_ProcessModel> ProcessModels;


        public void GetModels()
        {
            ProcessModels.Clear();
            using (var dbConnt = new SicoreQMSEntities1())
            {
                var allModel = dbConnt.Prod_ProcessModel.Where(p => p.ModelName == "军品").OrderBy(x => x.ModelSort).ToList();
                foreach (var item in allModel)
                {
                    ProcessModels.Add(item);

                }
            }

        }

        public void CreateSpiltInfo(string childNumber, int qty)
        {

            using (var context = new SicoreQMSEntities1())
            {
                //插入新批次
                ProdInfo newProdInfo = new ProdInfo()
                {

                    Id = Guid.NewGuid().ToString(),
                    
                    ParentId = Processes.Id,
                    ProdName = Processes.ProdName,
                    ProdType = Processes.ProdType,
                    ProdLot = childNumber,
                    QualityLevel = Processes.QualityLevel,
                    OriginQty = qty,
                    Qty = qty,
                };

                context.ProdInfo.Add(newProdInfo);
                context.SaveChanges();

                //更新批次数据
                ProdInfo parentProd = context.ProdInfo.Find(Processes.ProdId);

                int nowQty = (int)(parentProd.Qty - qty);

                parentProd.Qty = nowQty;
                parentProd.ProdStatus = 4;
                context.SaveChanges();
                context.Database.ExecuteSqlCommand(
                                                $" update dbo.Prod_ProcessItem set ItemStatus=5 where ProdProcessId='{Processes.Id}' ");
                context.Database.ExecuteSqlCommand(
                                                $" update dbo.Prod_Process set ProdStatus=5,Qty={nowQty} where Id='{Processes.Id}' ");
                context.Database.ExecuteSqlCommand(
                                               $" update dbo.ProdInfo set Qty={nowQty} where Id='{Processes.Id}' ");

                LotRelation relation = new LotRelation()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProdId = newProdInfo.Id,
                    ParentId = Processes.ProdId,
                    RelationType = "spilt"

                };
                context.LotRelation.Add(relation);
                context.SaveChanges();

                Prod_Process newProcessInfo = new Prod_Process
                {
                    

                    Id = Guid.NewGuid().ToString(),
                    ProdId = newProdInfo.Id,
                    ProdName = newProdInfo.ProdName,
                    ProdLot = childNumber,
                    Qty = qty,
                    OriginQty = qty,
                    QualityLevel = newProdInfo.QualityLevel,
                    ProdType = newProdInfo.ProdType,
                    ModelName = "军工",
                };

                // 将新的 ProdInfo 对象添加到数据库
                context.Prod_Process.Add(newProcessInfo);
                context.SaveChanges();

                var isCompleteCount = context.Prod_ProcessItem.Where(i=>i.IsComplete==true && i.ProdProcessId==Processes.Id).Count();

               

                foreach (var item in ProcessModels)
                {

                    Prod_ProcessItem newProcessItem = new Prod_ProcessItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProdId = newProdInfo.Id,
                        ProdProcessId = newProcessInfo.Id,
                        ProdName = newProdInfo.ProdName,
                        ProdType = newProdInfo.ProdType,
                        Lot = childNumber,
                        QualityLevel = newProdInfo.QualityLevel,
                        ModelName = "军工",
                    };
                    if (isCompleteCount>0)
                    {
                        newProcessItem.IsComplete = true;
                        newProcessItem.ItemStatus = 2;
                        newProcessItem.OutQty = qty;
                        newProcessItem.InputQty = qty;
                        isCompleteCount--;
                    }

                    newProcessItem.CopyModelData(item);
                    context.Prod_ProcessItem.Add(newProcessItem);
                }
                context.SaveChanges();
                MessageBox.Show("拆分成功");
                NewId = newProcessInfo.Id;

            }


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
                    var prodInfo = context.ProdInfo.Find(prodprocee.ProdId);
                    if (prodInfo != null)
                    {
                        ProdData = prodInfo;
                    }
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
            ProcessModels = new List<Prod_ProcessModel>();
            GetModels();
        }

    }
}
