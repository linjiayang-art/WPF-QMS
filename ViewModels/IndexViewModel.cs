using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace SicoreQMS.ViewModels
{
    public class IndexViewModel:BindableBase
    {
        private readonly IDialogService dialog;

        public IndexViewModel(IDialogService dialog)
        {
            ExcuteCommand = new DelegateCommand<string>(Execute);
            this.dialog = dialog;
        }

        private void Execute(string obj)
        {
           if (obj == "生产流程卡进度更新") 
            {
                UpdateProcess();
            }
           else { return; }
        }

        void UpdateProcess()
        {
            dialog.ShowDialog("ProdProcessPrintView");
        }

        public DelegateCommand<string> ExcuteCommand { get; set; }

      

    }
}
