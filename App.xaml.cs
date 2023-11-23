using Prism.DryIoc;
using Prism.Ioc;
using SicoreQMS.Common;
using SicoreQMS.ViewModels;
using SicoreQMS.ViewModels.DialogModels;
using SicoreQMS.ViewModels.PrintModels;
using SicoreQMS.Views;
using SicoreQMS.Views.Dialogs;
using SicoreQMS.Views.Prints;
using System;
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

        protected override void OnInitialized()
        {
            var service= App.Current.MainWindow.DataContext as IConfigureService;
            if (service !=null)
            {
                service.Configure();
            }
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ProdProcessPrintView, ProdProcessPrintViewModel>();
            containerRegistry.RegisterDialog<ProdProcessPrintView, ProdProcessPrintViewModel>();
            containerRegistry.RegisterDialog<ProcessUpdateView,ProcessUpdateViewModel>();
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<ToDoView, ToDoViewModel>();
            
            containerRegistry.RegisterForNavigation<ProdProcessUpdateView, ProdProcessUpdateViewModel>();
            containerRegistry.RegisterForNavigation<ProdProcessCreateView, ProdProcessCreateViewModel>();
            containerRegistry.RegisterForNavigation<ProdModelMaintainView, ProdModelMaintainViewModel>();
            containerRegistry.RegisterForNavigation<TestRequestView, TestRequestViewModel>();
        }

      
    }
}
