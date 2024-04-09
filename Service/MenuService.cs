using Prism.Common;
using Prism.Mvvm;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
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
                var menu = context.Menus.OrderBy(p=>p.sort) . ToList();

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
            return menuBars;
        }

    }
}
