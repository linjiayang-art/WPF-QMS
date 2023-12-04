using Prism.Events;
using SicoreQMS.Extensions;
using System.Windows;
using System.Windows.Input;

namespace SicoreQMS.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IEventAggregator aggregator)
        {
            aggregator.ResgiterMessage(arg =>
            {
                Snackbar.MessageQueue.Enqueue(arg);
            });

            InitializeComponent();
            menuBar.SelectionChanged += (s, e) =>
            {
                drawerHost.IsLeftDrawerOpen = false;
            };
            btnMin.Click += (s, e) =>
            {
                this.WindowState = WindowState.Minimized;
            };
            btnMax.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    return;
                }
                this.WindowState = WindowState.Maximized;

            };
            btnClose.Click += (s, e) =>
            {
                this.Close();
            };
            colorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            colorZone.MouseDoubleClick += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    return;
                }
                this.WindowState = WindowState.Maximized;

            };
        }






    }
}
