using Prism.Commands;
using Prism.Common;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Interface;
using SicoreQMS.Common.Models.Operation;
using Syncfusion.UI.Xaml.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels
{
    public class TestProcessUpdateViewModel : BindableBase
    {

        #region 属性

        public DelegateCommand<TestProcessItem> BtnCommand { get; set; }

        private readonly IDialogService dialog;
        private ObservableCollection<TestProcessItem> _testItems;

        public ObservableCollection<TestProcessItem> TestItems
        {
            get { return _testItems; }
            set { _testItems = value; RaisePropertyChanged(); }
        }

        public string Id { get; set; }

        private ObservableCollection<SelectBasic> _allTestProcess;

        public ObservableCollection<SelectBasic> AllTestProcess
        {
            get { return _allTestProcess; }
            set { _allTestProcess = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<string> HandleSelect { get; set; }

        private TestProcess _testProcessInfo;

        public TestProcess TestProcessInfo
        {
            get { return _testProcessInfo; }
            set { SetProperty(ref _testProcessInfo, value); }
        }




        #endregion

        public TestProcessUpdateViewModel(IDialogService dialog)
        {
            AllTestProcess = new ObservableCollection<SelectBasic>();
            HandleSelect = new DelegateCommand<string>(GetTestItem);
            TestItems = new ObservableCollection<TestProcessItem>();

            BtnCommand = new DelegateCommand<TestProcessItem>(ShowUpdateDiaLog);
            CreateData();
            this.dialog = dialog;
        }

        private void ShowUpdateDiaLog(TestProcessItem obj)
        {
            if (obj is null)
            {
                return;
            }
            DialogParameters dialogParameters = new DialogParameters
            {
                { "Id",obj.Id},
            };

            dialog.ShowDialog("TestProcessItemUpdateView", dialogParameters, result =>
            {  GetTestItem(Id);}) ;



        }

        private void GetTestItem(string obj)
        {
            Id = obj;
            if (obj == null)
            {
                return;
            }


            TestItems.Clear();
            using (var context = new SicoreQMSEntities1())
            {

                TestProcessInfo = context.TestProcess.Find(Id);

                var result = context.TestProcessItem.Where(p => p.TestProcessId == Id).ToList();
                if (result.Count == 0)
                {
                    return;
                }
                foreach (var item in result)
                {
                    TestItems.Add(item);
                }
            }

        }

        private void CreateData()
        {
            using (var context = new SicoreQMSEntities1())
            {
                var result = context.TestProcess.Where(p => p.Isdeletd == false && p.CompleteStatus == false).ToList();
                foreach (var item in result)
                {
                    AllTestProcess.Add(item.GetSelection());

                }
            }
        }


    }
}
