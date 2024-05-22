using Microsoft.Xaml.Behaviors;
using Syncfusion.Windows.Controls.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SicoreQMS.Extensions
{
    public class PassWordExtensions
    {


        public static string GetPassWord(DependencyObject obj)
        {
            return (string)obj.GetValue(PassWordProperty);
        }

        public static void SetPassWord(DependencyObject obj, string value)
        {
            obj.SetValue(PassWordProperty, value);
        }

        // Using a DependencyProperty as the backing store for PassWord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PassWordProperty =
            DependencyProperty.RegisterAttached("PassWord", typeof(string), typeof(PassWordExtensions), new PropertyMetadata(string.Empty,OnPassWordPropertyChanged ));

        private static void OnPassWordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passWord = d as PasswordBox;
            string password=(string)e.NewValue;
            if (passWord!=null&&passWord.Password!=password)
            {
                passWord.Password = password;
            }


        }

    }

    //public class PasswordBehavior : Behavior<PasswordBox>
    //{
    //    protected override void OnAttached()
    //    {
    //        base.OnAttached();
    //        AssociatedObject.PasswordChanged+= AssociatedObject_PasswordChanged;
    //        AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
    //    }

    //    private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
    //    {
    //        PasswordBox passwordBox = sender as PasswordBox;
    //        string password = PassWordExtensions.GetPassWord(passwordBox);

    //        if (passwordBox != null && passwordBox.Password != password)
    //            PassWordExtensions.SetPassWord(passwordBox, passwordBox.Password);
    //    }

    //    protected override void OnDetaching()
    //    {
    //        base.OnDetaching();
    //        AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
    //        AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
    //    }

    //    private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
    //    {
    //        if (e.Key == Key.Enter)
    //        {
    //            // 执行绑定的命令
    //            if (Command != null && Command.CanExecute(null))
    //            {
    //                Command.Execute(null);
    //            }
    //        }
    //    }
    //    public ICommand Command
    //    {
    //        get { return (ICommand)GetValue(CommandProperty); }
    //        set { SetValue(CommandProperty, value); }
    //    }

    //    public static readonly DependencyProperty CommandProperty =
    //        DependencyProperty.Register("Command", typeof(ICommand), typeof(PasswordBehavior), new PropertyMetadata(null));



    //}
    public class PasswordBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }


        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            string password = PassWordExtensions.GetPassWord(passwordBox);

            if (passwordBox != null && passwordBox.Password != password)
                PassWordExtensions.SetPassWord(passwordBox, passwordBox.Password);
        }


        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Command != null && Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(PasswordBehavior), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(PasswordBehavior), new PropertyMetadata(null));
    }
}
