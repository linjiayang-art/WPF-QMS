using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Interface;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class TestProcessItemUpdateViewModel : BindableBase, IDialogAware
    {
   

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }




        public TestProcessItemUpdateViewModel()
        {
            DialogHostName = "Root";
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);

            BtnStart = new DelegateCommand(ProcessStart, CanStartExecute);
            BtnEnd = new DelegateCommand(ProcessEnd, CanEndExecute);

            Title = "试验流程卡进度更新";
        }

        void Save()
        {

        }
        void Cancel()
        {
            

        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="dialogResult"></param>
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {

            RequestClose.Invoke(dialogResult);
        }


        private void ProcessEnd()
        {
            using (var context = new SicoreQMSEntities1())
            {
                var item = context.TestProcessItem.Find(Id);
                if (item.ExperimentQty < this.PassQty)
                {
                    MessageBox.Show("产出数大于投入数!");
                    return;
                }
                item.ExperimentItemPassQty = this.PassQty;
                item.ExperimentStatus = 2;
                item.Remark += "完成时备注:" + this.Remark;
                item.ExperimentEndTime = DateTime.Now;
                item.EstimatedCompletionTime = DateTime.Now;

                context.SaveChanges();
            }
            ButtonResult result = ButtonResult.None;

            RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(result));

        }
        private void ProcessStart()
        {
            using (var context = new SicoreQMSEntities1())
            {
                var item = context.TestProcessItem.Find(Id);
                if (item.ExperimentQty < this.PassQty)
                {
                    MessageBox.Show("产出数大于投入数!");
                    return;
                }
                item.ExperimentItemPassQty = this.PassQty;
                item.ExperimentStatus = 1;
                item.Remark += this.Remark;
                item.ExperimentSatrtTime = DateTime.Now;
                context.SaveChanges();
            }
            ButtonResult result = ButtonResult.None;

            RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(result));
        }





        #region 属性
        /// <summary>
        /// 按钮相关
        /// </summary>
        public DelegateCommand BtnStart { get; set; }
        public DelegateCommand BtnEnd { get; set; }

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

        private bool CanStartExecute()
        {
            return IsStartEnabled;

        }
        private bool CanEndExecute()
        {
            return IsEndEnabled;

        }
        /// <summary>
        /// 字段属性
        /// </summary>

        public string Id { get; set; }
        private string _remark;

        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }

        private int _passQty;

        public int PassQty
        {
            get { return _passQty; }
            set { SetProperty(ref _passQty, value); }
        }


        private TestProcessItem _testItem;

        public TestProcessItem TestItem
        {
            get { return _testItem; }
            set { SetProperty(ref _testItem, value); }

        }

        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

        #endregion


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
                TestItem = context.TestProcessItem.Find(Id);
            }
            if (TestItem.ExperimentStatus == 0)
            {
                IsStartEnabled = true;
                IsEndEnabled = false;
                return;
            }
            if (TestItem.ExperimentStatus == 1)
            {
                IsStartEnabled = false;
                IsEndEnabled = true;
                return;
            }


        }

        public void OnDialogOpend(IDialogParameters parameters)
        {

            Id = parameters.GetValue<string>("Id");

            using (var context = new SicoreQMSEntities1())
            {
                TestItem = context.TestProcessItem.Find(Id);
            }
            if (TestItem.ExperimentStatus == 0)
            {
                IsStartEnabled = true;
                IsEndEnabled = false;
                return;
            }
            if (TestItem.ExperimentStatus == 1)
            {
                IsStartEnabled = false;
                IsEndEnabled = true;
                return;
            }
        }
    }
}
