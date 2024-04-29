using Prism.Mvvm;
using SicoreQMS.Common.Interface;
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
    public class NavigationMenuService : BindableBase, INavigationMenuService
    {
        private ObservableCollection<NavigationItem> items;

        public ObservableCollection<NavigationItem> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged(); }
        }

        public NavigationMenuService()
        {

            Items = new ObservableCollection<NavigationItem>();
            InitMenus();
        }

        public void InitMenus()
        {
            Items.Clear();

            Items.Add(new NavigationItem("Home", "首页", "IndexView",new ObservableCollection<NavigationItem>
            {   new NavigationItem("PencilBoxMultiple", "试验流程卡模板维护", "TestModelMaintenanceView" ,new ObservableCollection<NavigationItem>
            {
                       new NavigationItem("PencilBoxMultiple", "生产流程卡进度更新", "ProdProcessUpdateView"),
                new NavigationItem("PencilBoxMultiple", "生产流程卡编制", "ProdProcessCreateView"),
            })
              
            }));

            var item = new NavigationItem("WrenchCheck", "实验需求申请", "TestRequestView", new ObservableCollection<NavigationItem>
            {
                new NavigationItem("PencilBoxMultiple", "试验流程卡模板维护", "TestModelMaintenanceView"),
                new NavigationItem("PencilBoxMultiple", "试验流程卡审核编制", "TestCreateView"),
                new NavigationItem("PencilBoxMultiple", "试验流程卡进度更新", "TestProcessUpdateView"),
            });
            var item2 = new NavigationItem("WrenchCheck", "生产流程卡", "TestRequestView", new ObservableCollection<NavigationItem>
            {
                new NavigationItem("PencilBoxMultiple", "生产流程卡进度更新", "ProdProcessUpdateView"),
                new NavigationItem("PencilBoxMultiple", "生产流程卡编制", "ProdProcessCreateView"),

          
            });
            var item3 = new NavigationItem("WrenchCheck", "设备", "TestRequestView", new ObservableCollection<NavigationItem>
            {
                 // MenuBars.Add(new MenuBar() { Icon = "PencilBoxMultiple", Title = "试验流程卡进度更新", NameSpace = "TestProcessUpdateView" });
            // MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设备运行统计", NameSpace = "EquipemntUsageView" });
            //MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "生产流程卡编制", NameSpace = "ProdProcessCreateView" });
                 new NavigationItem("PencilBoxMultiple", "设备运行统计", "EquipemntUsageView"),
                new NavigationItem("PencilBoxMultiple", "生产流程卡编制", "ProdProcessCreateView"),


            });
            Items.Add(item);
            Items.Add(item2);
            Items.Add(item3);

            //MenuBars.Add(new MenuBar() { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
            // MenuBars.Add(new MenuBar() { Icon = "WrenchCheck", Title = "实验需求申请", NameSpace = "TestRequestView" });
            // MenuBars.Add(new MenuBar() { Icon = "PencilBoxMultiple", Title = "生产流程卡进度更新", NameSpace = "ProdProcessUpdateView" });
            // MenuBars.Add(new MenuBar() { Icon = "PencilBoxMultiple", Title = "试验流程卡模板维护", NameSpace = "TestModelMaintenanceView" });
            // MenuBars.Add(new MenuBar() { Icon = "PencilBoxMultiple", Title = "试验流程卡审核编制", NameSpace = "TestCreateView" });
            // MenuBars.Add(new MenuBar() { Icon = "PencilBoxMultiple", Title = "试验流程卡进度更新", NameSpace = "TestProcessUpdateView" });
            // MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设备运行统计", NameSpace = "EquipemntUsageView" });
            //MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "生产流程卡编制", NameSpace = "ProdProcessCreateView" });
            //MenuBars.Add(new MenuBar() { Icon = "PrinterPosOutline", Title = "生产流程卡打印", NameSpace = "ProdProcessPrintView" });
            //MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "生产流程卡模板维护", NameSpace = "ProdModelMaintainView" });
            //MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });



            //using (var context = new SicoreQMSEntities1())
            //{
            //    var menu = context.Menus.OrderBy(p => p.sort).ToList();

            //    foreach (var item in menu)
            //    {
            //        var menuBar = new NavigationItem(item.Icon, item.Title,item.NameSpace);

            //        Items.Add(menuBar);
            //    }

            //}

        }
    }
}
