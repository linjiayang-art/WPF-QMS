using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Extensions;
using SicoreQMS.Service;
using Syncfusion.Windows.Controls.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "仕芯质量管理系统";

        public event Action<IDialogResult> RequestClose;

        private IEventAggregator aggregator;

        public LoginViewModel(IEventAggregator aggregator)
        {
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.aggregator = aggregator;
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "Login":Login();break;
                case "LoginOut": LoginOut(); break;
            }
        }

       void Login()
        {
            //LoginService.CreateUser(userno:UserNo,userName:"林佳阳",password:PassWord );

            var isLog= LoginService.Login(userno: UserNo,password: PassWord);

            if (isLog.ResultStatus)
            {
                aggregator.SendMessage(isLog.ResultMessage);

                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            else
            {
                MessageBox.Show(isLog.ResultMessage);
                //aggregator.SendMessage(isLog.ResultMessage);
                return;
            }
          //登录失败


        }

        void LoginOut()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.No));

        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            LoginOut();
           
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }


        public DelegateCommand<string> ExecuteCommand { get; private set; }


        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _userNo;

        public string UserNo
        {
            get { return _userNo; }
            set { SetProperty(ref _userNo, value); }
        }

        private string _passWord;

        public string PassWord
        {
            get { return _passWord; }
            set { _passWord = value; RaisePropertyChanged(); }
        }


    }
}
