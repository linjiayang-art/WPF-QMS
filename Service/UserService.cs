using NLog;
using Prism.Mvvm;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Service
{
    public class UserService:BindableBase
    {

        public class UserInfoItem
        {
            public string UserName { get; set; }
            public string UserNo { get; set; }
            public bool UserStatus { get; set; }
        }

        public static ObservableCollection<UserInfoItem> GetUserInfos()
        {
            var results = new ObservableCollection<UserInfoItem>();
            using (var context=new SicoreQMSEntities1())
            {
                var userList = context.UserInfo.ToList();
                foreach (var item in userList)
                {
                    var userInfo = new UserInfoItem()
                    {
                        UserName = item.UserName,
                        UserNo = item.UserNo,
                        UserStatus = (bool)item.IsDeleted ? true : false
                    };
                    results.Add(userInfo);
                }
            }

            return results;
        }
      
    }
}
