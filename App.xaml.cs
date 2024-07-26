using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using SicoreQMS.Common;
using SicoreQMS.Common.Interface;
using SicoreQMS.Common.Models.Interface;
using SicoreQMS.Common.Server;
using SicoreQMS.Service;
using SicoreQMS.ViewModels;
using SicoreQMS.ViewModels.DialogModels;
using SicoreQMS.ViewModels.PrintModels;
using SicoreQMS.Views;
using SicoreQMS.Views.Dialogs;
using SicoreQMS.Views.Prints;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using NLog;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
namespace SicoreQMS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        static App()
        {
            LogManager.Setup().LoadConfigurationFromFile("NLog.config");
            // 加载 NLog 配置文件
           //LogManager.LoadConfiguration("NLog.config");
        }

        protected override Window CreateShell()
        {
            //return Container.Resolve<MainView>();
            return Container.Resolve<MainView>();
        }
        //protected override Window CreateShell() => null;

        //protected override void OnInitialized()
        //{

        //    var container = ContainerLocator.Current;
        //    var shell=container.Resolve<object>("NewMainView");
        //    if (shell is Window view)
        //    {
        //        //更新Prism注册区域信息
        //        var reginManager = container.Resolve<IRegionManager>();
        //        RegionManager.SetRegionManager(view, reginManager);
        //        RegionManager.UpdateRegions();
        //        //调用首页的导航接口，做一个初始化操作
        //        if (view.DataContext is INavigationAware navigationAware)
        //        {
        //            navigationAware.OnNavigatedTo(null);
        //            //呈现首页
        //            App.Current.MainWindow = view;
        //        }
        //    }
        // base.OnInitialized();
        //}



        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error(e.Exception, "Unhandled exception caught in App_DispatcherUnhandledException");
            e.Handled = true; // Prevent application from crashing
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                logger.Fatal(ex, "Unhandled exception caught in CurrentDomain_UnhandledException");
            }
            else
            {
                logger.Fatal("Unhandled exception caught in CurrentDomain_UnhandledException: {0}", e.ExceptionObject);
            }
        }

        protected override void OnInitialized()
        {
            //从容器中获取对话框服务
            var dialog = Container.Resolve<IDialogService>();
            //显示登录对话框
            dialog.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Application.Current.Shutdown();
                    return;
                }

                var service = App.Current.MainWindow.DataContext as IConfigureService;
                if (service != null)
                    service.Configure();
                base.OnInitialized();
            });

            //var service = App.Current.MainWindow.DataContext as IConfigureService;
            //if (service != null)
            //{
            //    service.Configure();
            //}
            //base.OnInitialized();
            //logger.Info("Application started");
        }
        protected override void RegisterTypes(IContainerRegistry servicees)
        {
            servicees.RegisterForNavigation<NewMainView, NewMainViewModel>();
            servicees.RegisterSingleton<INavigationMenuService, NavigationMenuService>();

            servicees.RegisterDialog<ProdProcessEditView, ProdProcessEditViewModel>();
            servicees.RegisterDialog<TestStatusEditView, TestStatusEditViewModel>();

            servicees.RegisterDialog<TestItemAddView, TestItemAddViewModel>();
            servicees.RegisterDialog<TestCardMaintainView, TestCardMaintainViewModel>();
            servicees.RegisterDialog<LoginView, LoginViewModel>();
            servicees.RegisterDialog<UserMaintainView, UserMaintainViewModel>();
            servicees.RegisterDialog<TestProcessItemUpdateView, TestProcessItemUpdateViewModel>();
            servicees.RegisterDialog<EquipmentManagementView, EquipmentManagementViewModel>();

            servicees.RegisterDialog<LotSplitView, LotSplitViewModel>();
            //containerRegistry.RegisterDialog<ProdProcessPrintView, ProdProcessPrintViewModel>();
            servicees.RegisterDialog<TestEditView, TestEditViewModel>();
            servicees.RegisterDialog<ProcessUpdateView, ProcessUpdateViewModel>();
            servicees.RegisterDialog<ProcessEditView, ProcessEditViewModel>();
            //containerRegistry.RegisterForNavigation<ProcessUpdateView, ProcessUpdateViewModel>();


            //containerRegistry.Register<IDialogHostService, DialogHostService>();
            servicees.RegisterDialog<EquipmentUsageDetailView, EquipmentUsageDetailViewModel>();
            servicees.RegisterForNavigation<UserInfoView, UserInfoViewModel>();
            servicees.RegisterForNavigation<EquipmentReportView, EquipmentReportViewModel>();
            servicees.RegisterForNavigation<TestModelMaintenanceView, TestModelMaintenanceViewModel>();
            servicees.RegisterForNavigation<EquipemntUsageView, EquipemntUsageViewModel>();

            servicees.RegisterForNavigation<ProdProcessPrintView, ProdProcessPrintViewModel>();
            //containerRegistry.RegisterForNavigation<TestProcessUpdateView, TestProcessUpdateViewModel>();

            servicees.RegisterForNavigation<TestProcessUpdateView, TestProcessUpdateViewModel>();

            servicees.RegisterForNavigation<TestCreateView, TestCreateViewModel>();

            servicees.RegisterForNavigation<IndexView, IndexViewModel>();

            servicees.RegisterForNavigation<ProdProcessUpdateView, ProdProcessUpdateViewModel>();
            servicees.RegisterForNavigation<ProdProcessCreateView, ProdProcessCreateViewModel>();
            servicees.RegisterForNavigation<ProdModelMaintainView, ProdModelMaintainViewModel>();
            servicees.RegisterForNavigation<TestRequestView, TestRequestViewModel>();
            // Register NLog
            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    loggingBuilder.AddNLog();
                })
                .BuildServiceProvider();

            servicees.RegisterInstance(serviceProvider.GetService<ILoggerFactory>());
            servicees.RegisterInstance(serviceProvider.GetService<ILogger<App>>());
        }




        //protected override void RegisterTypes(IContainerRegistry containerRegistry)
        //{



        //    //containerRegistry.RegisterDialog<TestItemAddView, TestItemAddViewModel>();

        //    //containerRegistry.RegisterDialog<LoginView, LoginViewModel>();

        //    //containerRegistry.RegisterDialog<TestProcessItemUpdateView, TestProcessItemUpdateViewModel>();
        //    //containerRegistry.RegisterDialog<EquipmentManagementView, EquipmentManagementViewModel>();

        //    //containerRegistry.RegisterDialog<LotSplitView, LotSplitViewModel>();
        //    ////containerRegistry.RegisterDialog<ProdProcessPrintView, ProdProcessPrintViewModel>();
        //    //containerRegistry.RegisterDialog<TestEditView, TestEditViewModel>();
        //    //containerRegistry.RegisterDialog<ProcessUpdateView,ProcessUpdateViewModel>();
        //    //containerRegistry.RegisterDialog<ProcessEditView, ProcessEditViewModel>();
        //    ////containerRegistry.RegisterForNavigation<ProcessUpdateView, ProcessUpdateViewModel>();

        //    ////containerRegistry.Register<IDialogHostService, DialogHostService>();

        //    //containerRegistry.RegisterForNavigation<TestModelMaintenanceView, TestModelMaintenanceViewModel>();
        //    //containerRegistry.RegisterForNavigation< EquipemntUsageView,EquipemntUsageViewModel>();

        //    //containerRegistry.RegisterForNavigation<ProdProcessPrintView, ProdProcessPrintViewModel>();
        //    ////containerRegistry.RegisterForNavigation<TestProcessUpdateView, TestProcessUpdateViewModel>();

        //    //containerRegistry.RegisterForNavigation<TestProcessUpdateView, TestProcessUpdateViewModel>();

        //    //containerRegistry.RegisterForNavigation<TestCreateView, TestCreateViewModel>();

        //    //containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();

        //    //containerRegistry.RegisterForNavigation<ProdProcessUpdateView, ProdProcessUpdateViewModel>();
        //    //containerRegistry.RegisterForNavigation<ProdProcessCreateView, ProdProcessCreateViewModel>();
        //    //containerRegistry.RegisterForNavigation<ProdModelMaintainView, ProdModelMaintainViewModel>();
        //    //containerRegistry.RegisterForNavigation<TestRequestView, TestRequestViewModel>();
        //}


    }
}
