using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class ProdProcessEditViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "生产流程卡更改";

        public event Action<IDialogResult> RequestClose;

        public string Id { get; set; }

        #region
        private string _remark;
        private string _statusDesc;
        private string _prodName;

        public string ProdName
        {
            get => _prodName;
            set => SetProperty(ref _prodName, value);

        }


        public string StatusDesc
        {
            get => _statusDesc;
            set => SetProperty(ref _statusDesc, value);

        }

        public string Remark
        {
            get => _remark;
            set => SetProperty(ref _remark, value);

        }



        public DelegateCommand<string> BtnCommit { get; private set; }
        public ObservableCollection<SelectBasic> StatusItem { get; set; }
        #endregion
        public ProdProcessEditViewModel()
        {
            InitSelectItem();
            BtnCommit = new DelegateCommand<string>(Commit);
        }

        private void Commit(string obj)
        {
            switch (obj)
            {
                case "Edit":
                    EditProces();
                    break;
                case "Cancel":
                    RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
                    break;
                default:
                    RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
                    break;
            }
        }

        private void EditProces()
        {
            using (var db = new SicoreQMSEntities1())
            {
                var model = db.Prod_Process.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    model.ProdStatus = int.Parse(StatusDesc);
                    model.Remark = Remark;
                    db.SaveChanges();
                }
            }
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void InitSelectItem()
        {
            StatusItem = new ObservableCollection<SelectBasic>
            {
                 new SelectBasic() { Label = "未开始", Value = "0" },
                 new SelectBasic() { Label = "进行中", Value = "1" },
                 new SelectBasic() { Label = "已完成", Value = "2" },
                 new SelectBasic() { Label = "终止", Value = "3" },
                 //new SelectBasic() { Label = "终止", Value = "3" },
                    new SelectBasic() { Label = "拆分批次无法进行", Value = "5" },

            };
            // itemStatus 0未开始 1开始 2完成 3跳过 4暂定 5拆分批次无法进行
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
            if (parameters.ContainsKey("Id"))
            {
                Id = parameters.GetValue<string>("Id");
            }
            using (var db = new SicoreQMSEntities1())
            {
                var model = db.Prod_Process.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    ProdName = model.ProdName;
                    StatusDesc = model.ProdStatus.ToString();
                    Remark = model.Remark;
                }
            }

        }
    }
}
