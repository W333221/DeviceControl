using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DeviceControl.Behavior
{
    public static class ListBoxAutoScrollBehavior
    {
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToEnd",
                typeof(bool),
                typeof(ListBoxAutoScrollBehavior),
                new PropertyMetadata(false, OnAutoScrollToEndChanged));

        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        private static void OnAutoScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox) return;

            if ((bool)e.NewValue)
            {
                listBox.Loaded += ListBox_Loaded;
            }
            else
            {
                listBox.Loaded -= ListBox_Loaded;
            }
        }

        private static void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not ListBox listBox) return;

            if (listBox.ItemsSource is INotifyCollectionChanged notifyCollection)
            {
                notifyCollection.CollectionChanged += (_, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        listBox.Dispatcher.InvokeAsync(() =>
                        {
                            if (listBox.Items.Count > 0)
                            {
                                listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
                            }
                        });
                    }
                };
            }
        }
    }
}
