using NLog;
using Prism.Commands;
using Prism.Mvvm;
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
        public ObservableCollection<UserInfoItem> UserInfos { get; set; }


        public DelegateCommand<string> ExecuetCommand { get; private set; }

        #endregion
        public UserInfoViewModel()
        {
				UserInfos = Service.UserService.GetUserInfos();
            
            ExecuetCommand =new DelegateCommand<string>(Execuet);


        }

        private void Execuet(string obj)
        {
            throw new Exception("Test exception for logging");
            throw new NotImplementedException();
        }
        private static Logger logger = LogManager.GetCurrentClassLogger();

      
    }
}
