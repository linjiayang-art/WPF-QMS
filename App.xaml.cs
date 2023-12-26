using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using SicoreQMS.Common;
using SicoreQMS.Common.Models.Interface;
using SicoreQMS.Common.Server;
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

            //var dialog = Container.Resolve<IDialogService>();

            //dialog.ShowDialog("LoginView", callback =>
            //{
            //    if (callback.Result != ButtonResult.OK)
            //    {
            //        Application.Current.Shutdown();
            //        return;
            //    }

            //    var service = App.Current.MainWindow.DataContext as IConfigureService;
            //    if (service != null)
            //        service.Configure();
            //    base.OnInitialized();


            //});

            var service = App.Current.MainWindow.DataContext as IConfigureService;
            if (service != null)
            {
                service.Configure();
            }
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.RegisterDialog<TestItemAddView, TestItemAddViewModel>();

            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();

            containerRegistry.RegisterDialog<TestProcessItemUpdateView, TestProcessItemUpdateViewModel>();

            containerRegistry.RegisterDialog<LotSplitView, LotSplitViewModel>();
            containerRegistry.RegisterDialog<ProdProcessPrintView, ProdProcessPrintViewModel>();

            containerRegistry.RegisterDialog<ProcessUpdateView,ProcessUpdateViewModel>();
            //containerRegistry.RegisterForNavigation<ProcessUpdateView, ProcessUpdateViewModel>();

            //containerRegistry.Register<IDialogHostService, DialogHostService>();

            containerRegistry.RegisterForNavigation<ProdProcessPrintView, ProdProcessPrintViewModel>();
            containerRegistry.RegisterForNavigation<TestProcessUpdateView, TestProcessUpdateViewModel>();

            containerRegistry.RegisterForNavigation<TestProcessUpdateView, TestProcessUpdateViewModel>();

            containerRegistry.RegisterForNavigation<TestCreateView, TestCreateViewModel>();

            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();

            containerRegistry.RegisterForNavigation<ProdProcessUpdateView, ProdProcessUpdateViewModel>();
            containerRegistry.RegisterForNavigation<ProdProcessCreateView, ProdProcessCreateViewModel>();
            containerRegistry.RegisterForNavigation<ProdModelMaintainView, ProdModelMaintainViewModel>();
            containerRegistry.RegisterForNavigation<TestRequestView, TestRequestViewModel>();
        }

      
    }
}
