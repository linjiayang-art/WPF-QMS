﻿using Prism.Common;
using Prism.Mvvm;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Service
{
    public  class MenuService : BindableBase
    {

        public static ObservableCollection<MenuBar> GetMenu()
        {
            var menuBars = new ObservableCollection<MenuBar>();
            using (var context = new SicoreQMSEntities1())
            {
                var menu = context.Menus.Where(p=>p.IsDeleted==false).OrderBy(p=>p.sort) . ToList();

                foreach (var item in menu)
                {
                    var menuBar = new MenuBar()
                    {
                        Icon = item.Icon,
                        NameSpace = item.NameSpace,
                        Title = item.Title
                    };
                    menuBars.Add(menuBar);
                }
                
            }
            if (AppSession.UserNo== "1000145")
            {
                var a = new MenuBar()
                {
                    Icon = "Account",
                    NameSpace = "UserInfoView",
                    Title = "用户管理"
                };
                menuBars.Add(a);
            }
          
            return menuBars;
        }

    }
}
