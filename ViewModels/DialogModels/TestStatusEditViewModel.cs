using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Interface;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels.DialogModels
{
    internal class TestStatusEditViewModel : BindableBase, IDialogAware
    {

        public TestStatusEditViewModel()
        {
            BtnCommit = new DelegateCommand<string>(OnBtnCommit);
            StatusDesc = "0";
       
        }

        private void OnBtnCommit(string obj)
        {
            switch (obj)
            {
                case "Edit":
                    EditProces();
                    break;

                default:
                    break;
            }
        }

        private void EditProces()
        {
            var a=int.TryParse(StatusDesc, out int status);
            if (a)
            {
                using (var db = new SicoreQMSEntities1())
                {
                    var model = db.TestProcess.FirstOrDefault(x => x.Id == TestProcess.Id);
                    if (model != null)
                    {
                        model.Remark = Remark;
                        model.StatusDesc = status;
                        db.SaveChanges();
                    }
                }
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            else
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            }
            
        }

        private TestProcess testProcess;

        private string _statusDesc;

        public DelegateCommand<string> BtnCommit { get; private set; }
        
        public string StatusDesc
        {
            get => _statusDesc;
            set => SetProperty(ref _statusDesc, value);

        }

        public TestProcess TestProcess
        {
            get => testProcess;
            set => SetProperty(ref testProcess, value);

        }

        private string _remark;

        public string Remark
        {
            get => _remark;
            set => SetProperty(ref _remark, value);

        }



        private ObservableCollection<SelectBasic> _statusItem;

        public ObservableCollection<SelectBasic> StatusItem
        {
            get => _statusItem;
            set => SetProperty(ref _statusItem, value);

        }



        public string Title { get; set; } = "试验流程卡状态修改";

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
            var id = parameters.GetValue<string>("Id");
            using (var db = new SicoreQMSEntities1())
            {
                var model = db.TestProcess.FirstOrDefault(x => x.Id == id);
                if (model != null)
                {
                    TestProcess = model;
                }


            }
            StatusDesc= TestProcess.StatusDesc.ToString();
            Remark = TestProcess.Remark;
       StatusItem = new ObservableCollection<SelectBasic>
            {
                 new SelectBasic() { Label = "未开始", Value = "0" },
                 new SelectBasic() { Label = "进行中", Value = "1" },
                 new SelectBasic() { Label = "已完成", Value = "2" },
                 new SelectBasic() { Label = "终止", Value = "3" },
               
            };   


        }
    }
}
