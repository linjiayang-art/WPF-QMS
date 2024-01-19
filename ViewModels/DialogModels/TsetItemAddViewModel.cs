using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SicoreQMS.ViewModels.DialogModels
{
    internal class TestItemAddViewModel : BindableBase, IDialogAware
    {
        private TestProcessItem _model;
        public TestProcessItem Model{ get { return _model; } set { _model = value;RaisePropertyChanged(); } }
        public DelegateCommand SaveCommand { get; set; }

        public TestItemAddViewModel()
        {
            CancelCommand = new DelegateCommand(Cancel);
            SaveCommand = new DelegateCommand(Save);
            _model=new TestProcessItem();
        }

        private void Cancel()
        {
            ButtonResult btnResult = ButtonResult.None;


            RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(btnResult));
        }

        public DelegateCommand CancelCommand { get;set; }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {

            RequestClose.Invoke(dialogResult);
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(Model.ExperimentItemNo))
            {
                MessageBox.Show("请填写实验编号");
                return;
            }

            if (string.IsNullOrEmpty(Model.ExperimentName))
            {
                MessageBox.Show("请填写实验项目");
                return;
            }
            if  (string.IsNullOrEmpty(Model.ExperimentConditions))
            {
                MessageBox.Show("请填写试验条件");
                return;
            }

            if (string.IsNullOrEmpty(Model.ExperimentNo))
            {
                MessageBox.Show("请填写试验编号");
                return;
            }

            var result= TestProcessService.ADDItem(Model);
            if (result.ResultStatus==true)
            {

                ButtonResult btnResult = ButtonResult.OK;

                RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(btnResult));
            }
            else
            {
                
            }
         
        }

        public string Title { get; set; } = "新增测试项";

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

            var prodId = parameters.GetValue<string>("ProdId");
            var testProcessId= parameters.GetValue<string>("TestProcessId");
            Model.Id = Guid.NewGuid().ToString();
            Model.ProdId= prodId;
            Model.TestProcessId = testProcessId;
            Model.ModelId = "CE282394-1E58-4A38-BA00-C87D512617D7";
            
        }
    }
}
