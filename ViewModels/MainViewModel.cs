using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Extensions;
using System.Collections.ObjectModel;

namespace SicoreQMS.ViewModels
{
    public class MainViewModel : BindableBase
    {

        public MainViewModel(IRegionManager regionManager)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            CreateMenuBar();
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
            MenuBars.Add(new MenuBar() { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "生产流程卡模板维护", NameSpace = "ProdModelMaintainView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "生产流程卡编制", NameSpace = "ProdProcessCreateView" });
            
        }
    }
}
