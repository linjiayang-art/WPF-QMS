using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using SicoreQMS.Common;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Server;
using SicoreQMS.Extensions;
using SicoreQMS.Service;
using System.Collections.ObjectModel;

namespace SicoreQMS.ViewModels
{
    public class MainViewModel : BindableBase, IConfigureService
    {

        public MainViewModel(IRegionManager regionManager)
        {
         
            MenuBars = new ObservableCollection<MenuBar>();
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            GoBackCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoBack)
                {
                    journal.GoBack();
                }
            });
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoForward)
                {
                    journal.GoForward();
                }
            });
            this.regionManager = regionManager;
        }

        private IRegionNavigationJournal journal;


        private readonly IRegionManager regionManager;
        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }

        public DelegateCommand GoBackCommand { get; private set; }

        public DelegateCommand GoForwardCommand { get; private set; }

   


        private ObservableCollection<MenuBar> _menuBars;

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return _menuBars; }
            set { _menuBars = value; RaisePropertyChanged(); }
        }

        private void Navigate(MenuBar obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.NameSpace))
            {
                return;
            }
            regionManager.Regions[PrismManger.MainViewRegionName].RequestNavigate(obj.NameSpace, back =>
            {
                journal = back.Context.NavigationService.Journal;
            });

        }

        void CreateMenuBar()
        {
            MenuBars = MenuService.GetMenu();
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
        }
        /// <summary>
        /// 配置首页初始化参数
        /// </summary>

        public void Configure()
        {
            CreateMenuBar();
            regionManager.Regions[PrismManger.MainViewRegionName].RequestNavigate("ProdProcessUpdateView");
        }
    }
}
