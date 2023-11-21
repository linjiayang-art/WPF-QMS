﻿using Prism.Mvvm;
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
using System.Windows.Input;
using System.Runtime.Remoting.Contexts;
using System.Data.Entity;

namespace SicoreQMS.ViewModels

{
    public class ProdProcessCreateViewModel : BindableBase
    {
        private ObservableCollection<SelectBasci> _productNameBasic;
        private ObservableCollection<Prod_ProcessModel> _processModel;


        private string _prodLot;
        private string _prodName;
        private string _qualitylevel;

        private string _prodType;

        private string PropId { get;  set; }
        public string QualityLevel
        {
            get { return _qualitylevel; }
            set { SetProperty(ref _qualitylevel, value); }
        }


        public string ProdLot
        {
            get { return _prodLot ; }
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


        public DelegateCommand CommitBtnCommand { get; private set; }


        public DelegateCommand<SelectBasci> HandelSelect
        {
            get;
           private  set;
        }

        public ProdProcessCreateViewModel()
        {
            ProductNameBasic = new ObservableCollection<SelectBasci>();
            CreateProductSelection(ProductNameBasic);
            ProcessModel = new ObservableCollection<Prod_ProcessModel>();

            HandelSelect = new DelegateCommand<SelectBasci>(GetInfo);

            CommitBtnCommand = new DelegateCommand(CommitBtn);
            QualityLevel = "军品";

            var dbConnt = new SicoreQMSEntities1();
            var allModel = dbConnt.Prod_ProcessModel.OrderBy(x=>x.ModelSort).ToList();
            foreach (var item in allModel)
            {
                ProcessModel.Add(item);

            }

        }

        private void GetInfo(SelectBasci parameter)
        {
            if (parameter is null)
            {
                return;
            }
            PropId = parameter.Value;
            using (var context = new SicoreQMSEntities1())
            {
                var productInfo = context.ProdInfo.SingleOrDefault(b => b.Id == PropId);
                if (productInfo != null)
                {
                    this.ProdName = productInfo.ProdName;
                    this.ProdLot = productInfo.ProdLot;
                    this.QualityLevel = productInfo.QualityLevel;
                }
                else
                {
                    MessageBox.Show("未查询到改产品");
                }
             
            }
        }
        public void CommitBtn()
        {

            using (var context = new SicoreQMSEntities1())
            {
                Prod_Process newProcessInfo = new Prod_Process
                {
                    Id = Guid.NewGuid().ToString(),
                    ProdId = this.PropId,
                    ProdName = this.ProdName,
                    ModelName="军工",
                };

                // 将新的 ProdInfo 对象添加到数据库
                context.Prod_Process.Add(newProcessInfo);
                context.SaveChanges();
                foreach (var item in ProcessModel)
                {

                    Prod_ProcessItem newProcessItem = new Prod_ProcessItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProdId = this.PropId,
                        ProdProcessId= newProcessInfo.Id,
                        ProdName = this.ProdName,
                        Lot=this.ProdLot,
                        QualityLevel = this.QualityLevel,
                        ModelName = "军工",
                    };
                    newProcessItem.CopyModelData(item);
                    context.Prod_ProcessItem.Add(newProcessItem);
                }
                context.SaveChanges();
                MessageBox.Show("新增成功");


            }

         

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
            using (var context = new SicoreQMSEntities1())
            {
                var productItem = context.ProdInfo
                    .Where(b => b.ProdStatus == 0).ToList().OrderBy(x => x.CreateDate);
                foreach (var item in productItem)
                {
                    selectBascis.Add(item.ProductSelect());
                }
            }
                
        }
    }




}