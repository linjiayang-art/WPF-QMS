using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class UserMaintainViewModel : BindableBase, IDialogAware
    {
        #region
        public string UserId { get; set; }

        private string _userNo;

        private string _userName;

        private string _password;

        public string PassWord
        {
            get => _password;
            set => SetProperty(ref _password, value);

        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);

        }


        public string UserNo
        {
            get => _userNo;
            set => SetProperty(ref _userNo, value);

        }

        public DelegateCommand<string> BtnCommand { get; private set; }

        #endregion

        public UserMaintainViewModel()
        {
            BtnCommand = new DelegateCommand<string>(Execuet);


        }

        private void Execuet(string obj)
        {
            switch (obj)
            {
                case "Add":
                    AddUser();
                    break;
                case "Cancel":
                    RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
                    break;
                default:
                    break;
            }
        }

        private void AddUser()
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {

            }
            Service.LoginService.CreateUser(userName:UserName,userno:UserNo,password:PassWord);
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        public string Title { get; set; } = "维护用户信息";

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
            var userId=parameters.GetValue<string>("UserId");
            UserId = userId;
            if (string.IsNullOrEmpty(UserId))
            {
                using(var context=new SicoreQMSEntities1())
                {
                    var userInfo=context.UserInfo.Where(u=>u.Id==UserId).SingleOrDefault();
                    if (userInfo is null)
                    {
                        return;
                    }
                }
            
            }
        }
    }
}
