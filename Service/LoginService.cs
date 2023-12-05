using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Server;
using SicoreQMS.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Service
{
    public  class LoginService
    {





        public static void CreateUser(string userno,string userName, string password)
        {
            password = password.GetMD5();
            using (var context=new SicoreQMSEntities1())
            {
                UserInfo userInfo=new UserInfo()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserNo = userno,
                    UserName= userName,
                    PassWord = password
                };
                context.UserInfo.Add(userInfo);
                context.SaveChanges();

            }

        }


       public static ResultInfo Login( string userno,string password)
        {
            var resultInfo = new ResultInfo();

            password = password.GetMD5();

            using (var context = new SicoreQMSEntities1())
            {
                var user=context.UserInfo.Where(p=>p.PassWord == password&&p.UserNo==userno).FirstOrDefault();
                if (user ==null)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "账号密码错误,请检查!";
                    return resultInfo;
                }

                AppSession.UserID = user.Id;
                AppSession.UserNo = user.UserNo;
                AppSession.UserName = user.UserName;


            }
            resultInfo.ResultStatus = true;
            resultInfo.ResultMessage = "登录成功!";
            return resultInfo;

        }

        public static ResultInfo Resgiter(string userno, string password)
        {
            return new ResultInfo() { };
        }

    }
}
