using DocumentFormat.OpenXml.Wordprocessing;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class TestCardMaintainViewModel:BindableBase,IDialogAware
    {

        #region 属性
        private string _experimentNo;


        private string _experimentName;

        private string _experimentStandard;

        private string _experimentConditions;
    
        private int _experimentQty;
        private string _experimentNumber;

        private string _itemDesc;

        public string ModelId { get; set; }
        public string ItemDesc
        {
            get => _itemDesc;
            set => SetProperty(ref _itemDesc, value);
        }

        public string ExperimentNumber
        {
            get => _experimentNumber;
            set => SetProperty(ref _experimentNumber, value);

        }

        public int ExperimentQty
        {
            get => _experimentQty;
            set => SetProperty(ref _experimentQty, value);

        }


        public string ExperimentConditions
        {
            get => _experimentConditions;
            set => SetProperty(ref _experimentConditions, value);

        }

        public string ExperimentStandard
        {
            get => _experimentStandard;
            set => SetProperty(ref _experimentStandard, value);

        }


        public string ExperimentName
        {
            get => _experimentName;
            set => SetProperty(ref _experimentName, value);

        }

        public string ExperimentNo
        {
            get => _experimentNo;
            set => SetProperty(ref _experimentNo, value);

        }

        public DelegateCommand<string> BtnCommand { get;private set; }

        #endregion

        public TestCardMaintainViewModel()
        {
            BtnCommand = new DelegateCommand<string>(BtnExecute);


        }

        private void BtnExecute(string obj)
        {
            switch (obj)
            {
                case "Add":
                    AddModelItem();
                    break;
                case "cancel":
                    ButtonResult btnResult = ButtonResult.Cancel;
                    var messsage = new DialogParameters { { "key", "取消" } };
                    var dialogResult = new Prism.Services.Dialogs.DialogResult(btnResult, messsage);

                    RaiseRequestClose(dialogResult);
                    break;
                default:
                    break;
            }
        }

        private void AddModelItem()
        {
            if (string.IsNullOrEmpty(ExperimentNo))
            {
                MessageBox.Show("试验序号为必填项！");
                return;
            }

            using (var context=new SicoreQMSEntities1())
            {
                var testItem = new TestModelItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ExperimentItemNo= ExperimentNo,
                    ExperimentItemName= ExperimentName,
                    ExperimentItemStandard= ExperimentStandard,
                    ExperimentItemConditions= ExperimentConditions,
                    ExperimentItemQty= ExperimentQty,
                    ExperimentItemNumber= ExperimentNumber,
                    ItemDesc= ItemDesc,
                    ModelId= ModelId,
                    ExperimentItemRank= 0,
                    IsDeleted= false,

                };
                context.TestModelItem.Add(testItem);
                context.SaveChanges();
            }
            ButtonResult btnResult = ButtonResult.OK;
            var messsage = new DialogParameters { { "key", "新增成功" } };
            var dialogResult = new Prism.Services.Dialogs.DialogResult(btnResult, messsage);

            RaiseRequestClose(dialogResult);
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {

            RequestClose.Invoke(dialogResult);
        }

        public string Title { get;set; }="试验流程卡维护";

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
            ModelId=parameters.ContainsKey("ModelId")?parameters.GetValue<string>("ModelId"):"";

        }
    }
}
