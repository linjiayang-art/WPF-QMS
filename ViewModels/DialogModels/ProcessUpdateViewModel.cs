﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class ProcessUpdateViewModel : BindableBase, IDialogAware
    {
        public ProcessUpdateViewModel()
        {
            Title = "生产流程卡进度更新";
            BtnStart = new DelegateCommand(ProcessStart, CanStartExecute);
            BtnEnd = new DelegateCommand(ProcessEnd, CanEndExecute);
            IsStartEnabled = false;
            IsEndEnabled = false;
        }

        private bool CanStartExecute()
        {
            return IsStartEnabled;

        }
        private bool CanEndExecute()
        {
            return IsEndEnabled;

        }


        public DelegateCommand BtnStart { get; set; }
        public DelegateCommand BtnEnd { get; set; }

        #region  属性




        private bool _isStartEnabled;
        public bool IsStartEnabled
        {
            get { return _isStartEnabled; }
            set
            {
                SetProperty(ref _isStartEnabled, value);
                RaisePropertyChanged();
            }
        }



        private bool _isEndEnabled;
        public bool IsEndEnabled
        {
            get { return _isEndEnabled; }
            set
            {
                SetProperty(ref _isEndEnabled, value);
                RaisePropertyChanged();
            }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _prodProcessCard;
        public string ProdProcessCard
        {
            get { return _prodProcessCard; }
            set { SetProperty(ref _prodProcessCard, value); }
        }

        private string _processType;
        public string ProcessType
        {
            get { return _processType; }
            set { SetProperty(ref _processType, value); }
        }

        private int _outQty;
        public int OutQty
        {
            get { return _outQty; }
            set { SetProperty(ref _outQty, value); }
        }

        private int _inputQty;
        public int InputQty
        {
            get { return _inputQty; }
            set { SetProperty(ref _inputQty, value); }
        }
        private string _prodStandard;

        public string ProdStandard
        {
            get { return _prodStandard; }
            set { _prodStandard = value; }
        }
        #endregion


        private void ProcessEnd()
        {


            using (var context = new SicoreQMSEntities1())
            {
                var prodProcessItem = context.Prod_ProcessItem.SingleOrDefault(b => b.Id == Id);
                prodProcessItem.OutQty = OutQty;
                prodProcessItem.ItemStatus = 2;//状态变更为已完结
                prodProcessItem.IsComplete = true;
               context.SaveChanges();
            }

        }
        /// <summary>
        /// 产品进度更新
        /// </summary>
        private void ProcessStart()  
        {
            using (var context = new SicoreQMSEntities1())
            {
                var prodProcessItem = context.Prod_ProcessItem.SingleOrDefault(b => b.Id == Id);
                prodProcessItem.InputQty = InputQty;
                prodProcessItem.ItemStatus = 1;//状态变更为正在进行
                context.SaveChanges();
            }

        }

        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

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

            using (var context = new SicoreQMSEntities1())
            {
                var prodProcessItem = context.Prod_ProcessItem.SingleOrDefault(b => b.Id == Id);
                if (prodProcessItem != null)
                {
                    ProdProcessCard = prodProcessItem.ProdProcessCard;
                    ProcessType = prodProcessItem.ProcessType;
                    OutQty = prodProcessItem.OutQty ?? 0;
                    InputQty = prodProcessItem.InputQty ?? 0;
                    ProdStandard = prodProcessItem.ProdStandard;

                }
                else
                {
                    MessageBox.Show("未查询到该产品");
                }
                if (prodProcessItem.ItemStatus==0)
                {
                    IsStartEnabled = true;
                    IsEndEnabled = false;
                    return;
                }
                if (prodProcessItem.ItemStatus == 1)
                {
                    IsStartEnabled = false;
                    IsEndEnabled = true;
                    return;
                }

            }
        }




    }
}
