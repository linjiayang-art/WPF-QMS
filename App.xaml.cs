using Prism.DryIoc;
using Prism.Ioc;
using SicoreQMS.ViewModels;
using SicoreQMS.ViewModels.DialogModels;
using SicoreQMS.Views;
using SicoreQMS.Views.Dialogs;
using System.Windows;

namespace SicoreQMS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<ProcessUpdateView,ProcessUpdateViewModel>();
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<ToDoView, ToDoViewModel>();
            containerRegistry.RegisterForNavigation<ProdProcessCreateView, ProdProcessCreateViewModel>();
            containerRegistry.RegisterForNavigation<ProdModelMaintainView, ProdModelMaintainViewModel>();
            containerRegistry.RegisterForNavigation<TestRequestView, TestRequestViewModel>();
        }
    }
}
