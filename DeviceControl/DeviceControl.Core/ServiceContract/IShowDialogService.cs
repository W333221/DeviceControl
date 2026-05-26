using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeviceControl.Core.ServiceContract
{
    public interface IShowDialogService
    {
        Task<MessageBoxResult> ShowDialogAsync(string title, string msg, MessageBoxButton btn = MessageBoxButton.YesNo, MessageBoxImage messageBoxImage = MessageBoxImage.Question);
    }
}
