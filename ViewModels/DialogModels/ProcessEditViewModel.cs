
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class ProcessEditViewModel : BindableBase, IDialogAware, INotifyPropertyChanged
    {
        public string Title { get; set; }="生产流程卡修改";

        public event Action<IDialogResult> RequestClose;


        public ProcessEditViewModel(IEventAggregator aggregator)
        {
            this.aggregator=aggregator;
            SumbitBtn=new DelegateCommand<string>(Sumbit);
        }

        private void Sumbit(string obj)
        {
            switch (obj)
            {
                case "Edit":
                    EditProcess(); break;
                default:
                    break;
            }
        }

        private void EditProcess()
        {
            var a= ProcessItem.EndRemark;

            using (var context=new SicoreQMSEntities1())
            {
                context.Entry(ProcessItem).State = EntityState.Modified;
                context.SaveChanges();
            }

            ButtonResult btnResult = ButtonResult.None;

            RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(btnResult));

            aggregator.SendMessage("修改成功!");
        }


        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {

            RequestClose.Invoke(dialogResult);
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
            using (var db = new SicoreQMSEntities1())
            {
                ProcessItem = db.Prod_ProcessItem.SingleOrDefault(p => p.Id == Id);
                if (ProcessItem == null) 
                { 
                    aggregator.SendMessage("未找到对应的生产流程卡");
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region 属性
        private string _id;
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public DelegateCommand<string> SumbitBtn { get;private set; }

        private readonly IEventAggregator aggregator;

        private Prod_ProcessItem prcoessItem;


        public Prod_ProcessItem ProcessItem
        {
            get { return prcoessItem; }
            set { prcoessItem = value; RaisePropertyChanged(); }
        }


        #endregion



    }
}
