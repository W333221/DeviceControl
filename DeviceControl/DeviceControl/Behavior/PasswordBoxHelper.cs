using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DeviceControl.Behavior
{
    public static class PasswordBoxHelper
    {
        // 定义附加属性 PasswordProperty
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnPasswordPropertyChanged
                )
            );

        public static string GetPassword(DependencyObject obj) =>
            (string)obj.GetValue(PasswordProperty);
        public static void SetPassword(DependencyObject obj, string value) =>
            obj.SetValue(PasswordProperty, value);

        // 监听属性变化，更新 PasswordBox
        private static void OnPasswordPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d is PasswordBox passwordBox)
            {
                // 避免循环更新
                passwordBox.PasswordChanged -= OnPasswordChanged;

                // 更新 PasswordBox 的密码值
                if (passwordBox.Password != (string)e.NewValue)
                    passwordBox.Password = (string)e.NewValue;

                passwordBox.PasswordChanged += OnPasswordChanged;
            }

        }

        // 监听 PasswordBox 变化，更新附加属性
        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            SetPassword(passwordBox, passwordBox.Password);
        }
    }
}
