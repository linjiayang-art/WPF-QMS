using DocumentFormat.OpenXml.Office2010.Excel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels.DialogModels
{
    internal class TestEditViewModel : BindableBase, IDialogAware
    {

        #region 属性
        public string  TestId { get; set; }

        public DelegateCommand<string> BtnCommand { get; private set; }

        private IEventAggregator eventAggregator;

        private string prodProcessCard;

        public string ProdProcessCard
        {
            get { return prodProcessCard; }
            set { prodProcessCard = value;RaisePropertyChanged(); }
        }


        private string processType;

        public string ProcessType
        {
            get { return processType; }
            set { processType = value; RaisePropertyChanged(); }
        }

        private DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; RaisePropertyChanged(); }
        }


        private DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value;RaisePropertyChanged(); }
        }





        #endregion
        public TestEditViewModel(IEventAggregator aggregator)
        {
            BtnCommand = new DelegateCommand<string>(BtnCommandExecute);
            this.eventAggregator = aggregator;
        }

        private void BtnCommandExecute(string obj)
        {
            switch (obj)
            {
                case "Commit":
                    HandelBtnCommand(obj);
                    break;
                case "Cancel":
                    RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(ButtonResult.Cancel));
                    break;
                default:
                    break;
            }
        }

        public void HandelBtnCommand(string obj)
        {

            using (var context = new SicoreQMSEntities1())
            {
                var testItem = context.TestProcessItem.Find(TestId);
                testItem.ExperimentSatrtTime= this.StartDate;
                testItem.ExperimentEndTime = this.EndDate;


                context.SaveChanges();

            }



            ButtonResult btnResult = ButtonResult.OK;
            var messsage = new DialogParameters { { "key", "修改成功" } };
            var dialogResult = new Prism.Services.Dialogs.DialogResult(btnResult, messsage);

            RaiseRequestClose(dialogResult);
            this.eventAggregator.SendMessage("修改成功!");
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {

            RequestClose.Invoke(dialogResult);
        }

        public string Title { get; set; } = "试验流程卡修改";

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
            TestId = parameters.GetValue<string>("Id");
            using (var context = new SicoreQMSEntities1())
            {
                var testItem = context.TestProcessItem.Find(TestId);
                this.ProdProcessCard= testItem.ExperimentNo;
                this.ProcessType = testItem.ExperimentType;
                if (testItem.ExperimentSatrtTime!=null)
                {
                    this.StartDate = testItem.ExperimentSatrtTime.Value;
                }
                if (testItem.ExperimentEndTime!=null)
                {
                    this.EndDate = testItem.ExperimentEndTime.Value;
                }
            
           


              
            }


        }
    }
}
