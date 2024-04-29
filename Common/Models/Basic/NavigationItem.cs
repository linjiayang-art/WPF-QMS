using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Basic
{
    public class NavigationItem:BindableBase
    {

        public NavigationItem(string icon,string name,string pageName,ObservableCollection<NavigationItem> items=null )
        {
            Icon = icon;
            Name = name;
            PageName = pageName;
            Items = items;
        }

        private string naem;
        private string icon;
        private  ObservableCollection<NavigationItem> items; 
        private string pageName;
         
        public string Name
		{
			get { return naem; }
			set { naem = value;RaisePropertyChanged(); }
		}
        /// <summary>
        ///  图标
        /// </summary>

		public string Icon
		{
			get { return icon; }
			set { icon = value; RaisePropertyChanged(); }
		}
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName
        {
            get { return pageName; }
            set { pageName = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 菜单子项
        /// </summary>
        public ObservableCollection<NavigationItem> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged(); }
        }


    }
}
