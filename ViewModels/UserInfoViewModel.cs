using NLog;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SicoreQMS.Service.UserService;

namespace SicoreQMS.ViewModels
{
    public class UserInfoViewModel:BindableBase
    {
		#region
		private string _userName;
        private string _userNo;


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

        public string UserId { get; set; }
        public ObservableCollection<UserInfoItem> UserInfos { get; set; }


        public DelegateCommand<string> ExecuetCommand { get; private set; }
        public IDialogService DialogService { get; }
        public IEventAggregator EventAggregator { get; }

        #endregion
        public UserInfoViewModel(IDialogService dialogService, IEventAggregator  eventAggregator )
        {
				UserInfos = Service.UserService.GetUserInfos();
            
            ExecuetCommand =new DelegateCommand<string>(Execuet);
            DialogService = dialogService;
            EventAggregator = eventAggregator;
        }

        private void getUserInfo()
        {
            UserInfos.Clear();
            UserInfos = Service.UserService.GetUserInfos();
        }
        private void Execuet(string obj)
        {
            switch (obj)
            {
                case "add":
                    OpenUserMaintainDialog();
                    break;

                case "query":
                    getUserInfo();
                    break;
             
            }
        }
        
        private void OpenUserMaintainDialog()
        {
            var parameters = new DialogParameters()
            {
                   { "UserId",UserId }
            };
            DialogService.ShowDialog("UserMaintainView", parameters, r =>
            {
               
            });
        }

      
    }
}
