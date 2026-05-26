using DeviceControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeviceControl.Views
{
    /// <summary>
    /// FileLogQuery.xaml 的交互逻辑
    /// </summary>
    public partial class FileLogQuery : UserControl
    {

        public FileLogQuery()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                // 让 ListBox 支持 Ctrl+C 复制选中项
                var listBox = this.FindName("LogListBox") as ListBox; // 需要给 ListBox 加 x:Name="LogListBox"
                if (listBox != null)
                {
                    var cmd = new RoutedCommand();
                    cmd.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
                    CommandBinding binding = new CommandBinding(cmd, (sender, args) =>
                    {
                        if (listBox.SelectedItem is LogLineModel selected)
                        {
                            Clipboard.SetText(selected.Text);
                        }
                    });
                    this.CommandBindings.Add(binding);
                }
            };
        }


        private void CopyLine_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var contextMenu = menuItem?.Parent as ContextMenu;
            var listBoxItem = contextMenu?.PlacementTarget as ListBoxItem;
            if (listBoxItem?.DataContext is LogLineModel line)
            {
                Clipboard.SetText(line.Text);
            }
        }

        private void CopySelectedText_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前焦点所在的 TextBox（用户可能选中了部分文本）
            var focusedElement = Keyboard.FocusedElement;
            if (focusedElement is TextBox textBox && textBox.SelectedText != "")
            {
                Clipboard.SetText(textBox.SelectedText);
            }
            else
            {
                // 如果没有选中文本，则尝试复制整行
                CopyLine_Click(sender, e);
            }
        }

    }
}
