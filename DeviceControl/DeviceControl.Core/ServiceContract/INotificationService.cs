using DeviceControl.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract
{
    /// <summary>
    /// 通知服务接口
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// 准备通知
        /// Normal 普通进站 黄灯长亮
        /// Urgent 闪烁通知
        /// Perilous 待定
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        void ReadyNotice(NotificationLevel Level, string message);

        /// <summary>
        /// 准备通知
        /// Normal 普通进站 黄灯长亮
        /// Urgent 闪烁通知
        /// Perilous 待定
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        Task ReadyNoticeAsync(NotificationLevel Level, string message);

        /// <summary>
        /// 成功通知
        /// Normal 日志记录 UI交互
        /// Urgent 闪烁+日志记录 UI交互
        /// Perilous 长亮+日志记录 UI交互
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        void SuccseNotice(NotificationLevel Level, string message);


        /// <summary>
        /// 成功通知
        /// Normal 日志记录 UI交互
        /// Urgent 闪烁+日志记录 UI交互
        /// Perilous 长亮+日志记录 UI交互
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        Task SuccseNoticeAsync(NotificationLevel Level, string message);

        /// <summary>
        /// 报错通知
        /// Normal 日志记录 UI交互
        /// Urgent 闪烁+日志记录 UI交互
        /// Perilous 长亮+日志记录+上报服务器+UI交互
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        void ErrNotice(NotificationLevel Level, string message, Exception ex = null);

        /// <summary>
        /// 报错通知
        /// Normal 日志记录 UI交互
        /// Urgent 闪烁+日志记录 UI交互
        /// Perilous 长亮+日志记录+上报服务器+UI交互
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        Task ErrNoticeAsync(NotificationLevel Level, string message, Exception ex = null);

        /// <summary>
        /// 一般通知
        /// Normal 日志记录 
        /// Urgent 日志记录 UI交互
        /// Perilous 待定
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        void CommonNotice(NotificationLevel Level, string message);

        /// <summary>
        /// 一般通知
        /// Normal 日志记录 
        /// Urgent 日志记录 UI交互
        /// Perilous 待定
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        Task CommonNoticeAsync(NotificationLevel Level, string message);

        /// <summary>
        /// 向服务器通讯记录按钮动作
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool LogButton(string type, string message, string userID, string userName);

        /// <summary>
        /// 向服务器通讯记录按钮动作
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<bool> LogButtonAsync(string type, string message, string userID, string userName);

        /// <summary>
        /// 记录API动作
        /// </summary>
        /// <param name="type">api名</param>
        /// <param name="message"></param>
        void LogAPI(string type, string message, string content, bool isloginformation = false);

        /// <summary>
        /// 记录API动作
        /// </summary>
        /// <param name="type">api名</param>
        /// <param name="message"></param>
        Task LogAPIAsync(string type, string message, string content, bool isloginformation = false);
    }
}
