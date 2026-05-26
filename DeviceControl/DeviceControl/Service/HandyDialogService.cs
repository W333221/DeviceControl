using DeviceControl.Core.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeviceControl.Service
{
    public class HandyDialogService : IShowDialogService
    {
        public async Task<MessageBoxResult> ShowDialogAsync(string title, string msg, MessageBoxButton btn, MessageBoxImage messageBoxImage)
        {
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return HandyControl.Controls.MessageBox.Show(
                    msg,
                    title,
                    btn,
                    messageBoxImage
                );
            });
        }
    }

}
