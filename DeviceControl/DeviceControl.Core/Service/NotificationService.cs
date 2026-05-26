using DeviceControl.Core.Message.UI;
using DeviceControl.Core.ServiceContract;
using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Data.Configs;
using DeviceControl.Data.Enum;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service
{
    /// <summary>
    /// 消息推送类
    /// 每个方法实现各个等级应该需要处理的动作
    /// </summary>
    public class NotificationService : INotificationService
    {
        private IEventAggregator _eventAggregator;

        #region 页面属性
        private IColorLampDevice _colorLampDevice => ColorLampDevice.Value;
        private Lazy<IColorLampDevice> ColorLampDevice { get; set; }
        private IConfiguration _configuration;

        private AllDeviceConfig _allDeviceConfig;

        #endregion

        public NotificationService(Lazy<IColorLampDevice> colorLampDevice, IEventAggregator eventAggregator, IConfiguration configuration)
        {
            ColorLampDevice = colorLampDevice;
            _eventAggregator = eventAggregator;
            _configuration = configuration;

            _allDeviceConfig = _configuration.GetSection("AppSettings:DeviceConfig").Get<AllDeviceConfig>();

        }

        /// <summary>
        /// 普通通知
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        public void CommonNotice(NotificationLevel Level, string message)
        {
            if (Level == NotificationLevel.Normal)//日志
            {
                //日志
                SendAndLog(false, "", true, message);
            }
            else if (Level == NotificationLevel.Urgent) //通知+日志
            {
                //日志
                SendAndLog(true, "", true, message);
            }
            else if (Level == NotificationLevel.Perilous)//熄灯
            {
                //日志
                SendAndLog(true, "", true, message);
                _colorLampDevice?.ReSet();
            }
            InsertLog(Level, NotificationType.CommonNotice, message);
        }

        /// <summary>
        /// 普通通知
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task CommonNoticeAsync(NotificationLevel Level, string message)
        {
            if (Level == NotificationLevel.Normal)//日志
            {
                //日志
                SendAndLog(false, "", true, message);
            }
            else if (Level == NotificationLevel.Urgent) //通知+日志
            {
                //日志
                SendAndLog(true, "", true, message);
            }
            else if (Level == NotificationLevel.Perilous)//熄灯
            {
                //日志
                SendAndLog(true, "", true, message);
                _colorLampDevice?.ReSet();
            }
            await InsertLogAsync(Level, NotificationType.CommonNotice, message);
        }

        /// <summary>
        /// 报警通知
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        public void ErrNotice(NotificationLevel Level, string message, Exception ex = null)
        {
            if (Level == NotificationLevel.Normal) //日志+推送
            {
                //日志
                SendAndLog(true, "NG", true, message);
            }
            else if (Level == NotificationLevel.Urgent)//闪烁蜂鸣+日志+推送
            {
                //日志
                SendAndLog(true, "NG", true, message);
                Lamp(Level, NotificationType.ErrNotice);

            }
            else if (Level == NotificationLevel.Perilous)//长亮蜂鸣+日志+推送+上传服务器
            {
                //日志
                SendAndLog(true, "NG", true, message);
                Lamp(Level, NotificationType.ErrNotice);
                //日志
                Log.Error(message + $"EX：{ex?.ToString()}");


                //其他
                //RFIDTagService.Instance.LogAlarm
                //    (Dns.GetHostName(), HardWareTightenConfig.Instance.Station.StationID,
                //    HardWareTightenConfig.Instance.Station.StationName,
                //    "CatchErr",
                //    message, ex.Message + ",StackTrace:" + ex.StackTrace,
                //    HardWareTightenConfig.Instance.UserCode,
                //    HardWareTightenConfig.Instance.UserName);
            }
            InsertLog(Level, NotificationType.ErrNotice, message);
        }

        /// <summary>
        /// 报警通知
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        public async Task ErrNoticeAsync(NotificationLevel Level, string message, Exception ex = null)
        {
            if (Level == NotificationLevel.Normal) //日志+推送
            {
                //日志
                SendAndLog(true, "NG", true, message);
            }
            else if (Level == NotificationLevel.Urgent)//闪烁蜂鸣+日志+推送
            {
                //日志
                SendAndLog(true, "NG", true, message);
                Lamp(Level, NotificationType.ErrNotice);

            }
            else if (Level == NotificationLevel.Perilous)//长亮蜂鸣+日志+推送+上传服务器
            {
                //日志
                SendAndLog(true, "NG", true, message);
                Lamp(Level, NotificationType.ErrNotice);
                //日志
                Log.Error(message + $"EX：{ex?.ToString()}");
            }
            await InsertLogAsync(Level, NotificationType.ErrNotice, message);
        }

        /// <summary>
        /// 进站准备通知
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        public void ReadyNotice(NotificationLevel Level, string message)
        {
            if (Level == NotificationLevel.Normal) //三色灯+通知+日志
            {
                //日志
                SendAndLog(true, "", true, message);
                Lamp(Level, NotificationType.ReadyNotice);
            }
            else if (Level == NotificationLevel.Urgent)//触发过岗停线  //三色灯+通知+日志
            {
                //日志
                SendAndLog(true, "", true, message);
                Lamp(Level, NotificationType.ReadyNotice);
            }
            else if (Level == NotificationLevel.Perilous)
            {
                //日志
                SendAndLog(true, "", true, message);
            }
            InsertLog(Level, NotificationType.ReadyNotice, message);
        }

        /// <summary>
        /// 进站准备通知
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        public async Task ReadyNoticeAsync(NotificationLevel Level, string message)
        {
            if (Level == NotificationLevel.Normal) //三色灯+通知+日志
            {
                //日志
                SendAndLog(true, "", true, message);
                Lamp(Level, NotificationType.ReadyNotice);
            }
            else if (Level == NotificationLevel.Urgent)//触发过岗停线  //三色灯+通知+日志
            {
                //日志
                SendAndLog(true, "", true, message);
                Lamp(Level, NotificationType.ReadyNotice);
            }
            else if (Level == NotificationLevel.Perilous)
            {
                //日志
                SendAndLog(true, "", true, message);
            }
            await InsertLogAsync(Level, NotificationType.ReadyNotice, message);
        }

        /// <summary>
        /// 成功通知
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        public void SuccseNotice(NotificationLevel Level, string message)
        {
            if (Level == NotificationLevel.Normal) //拧紧中
            {
                //日志
                SendAndLog(true, "OK", true, message);
                Lamp(Level, NotificationType.SuccseNotice);
            }
            else if (Level == NotificationLevel.Urgent) //拧紧完毕
            {
                //日志
                SendAndLog(true, "OK", true, message);
                Lamp(Level, NotificationType.SuccseNotice);
            }
            else if (Level == NotificationLevel.Perilous)
            {
                //日志
                SendAndLog(true, "OK", true, message);
                Lamp(Level, NotificationType.SuccseNotice);
            }
            InsertLog(Level, NotificationType.SuccseNotice, message);
        }

        /// <summary>
        /// 成功通知
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="message"></param>
        public async Task SuccseNoticeAsync(NotificationLevel Level, string message)
        {
            if (Level == NotificationLevel.Normal) //拧紧中
            {
                //日志
                SendAndLog(true, "OK", true, message);
                Lamp(Level, NotificationType.SuccseNotice);
            }
            else if (Level == NotificationLevel.Urgent) //拧紧完毕
            {
                //日志
                SendAndLog(true, "OK", true, message);
                Lamp(Level, NotificationType.SuccseNotice);
            }
            else if (Level == NotificationLevel.Perilous)
            {
                //日志
                SendAndLog(true, "OK", true, message);
                Lamp(Level, NotificationType.SuccseNotice);
            }
            await InsertLogAsync(Level, NotificationType.SuccseNotice, message);
        }

        /// <summary>
        /// 按钮操作
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool LogButton(string type, string message, string userID, string userName)
        {
            try
            {
                SendAndLog(true, "", true, message);
                InsertLog(NotificationLevel.Perilous, NotificationType.LogButton, message);

                //return RFIDTagService.Instance.LogAlarm(hard.FTP.URL, hard.LineClass + hard.StationNo, hard.StationName, type, message, userID, userName, hard.LineClass);
                return true;
            }
            catch (Exception ex)
            {
                //Log.Error("将报警信息写入服务端时异常", ex);
                Log.Error(ex, "将报警信息写入服务端时异常，{msg}", ex.Message + ",StackTrace:" + ex.StackTrace);
                return false;
            }

        }

        /// <summary>
        /// 按钮操作
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> LogButtonAsync(string type, string message, string userID, string userName)
        {
            try
            {
                SendAndLog(true, "", true, message);
                await InsertLogAsync(NotificationLevel.Perilous, NotificationType.LogButton, message);
                return true;
            }
            catch (Exception ex)
            {
                //Log.Error("将报警信息写入服务端时异常", ex);
                Log.Error(ex, "将报警信息写入服务端时异常，{msg}", ex.Message + ",StackTrace:" + ex.StackTrace);
                return false;
            }

        }


        /// <summary>
        /// 接口日志记录
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="message">只能是报文</param>
        /// <param name="content">输出的备注</param>
        /// <param name="isloginformation">是否写入Information</param>
        public void LogAPI(string type, string message, string content, bool isloginformation = false)
        {
            Log.Verbose("【type】：{type}【message】：{message},【content】：{content}", type, message, content);
            if (isloginformation)
            {
                try
                {
                    Log.Information("【message】：{message},【content】：{content}", message, content);
                    //_logInfoService.Insert(new LogInfo
                    //{
                    //    LogLevel = NotificationLevel.Perilous.ToString(),
                    //    LogMessage = "【type】：" + type + " 【content】：" + message,
                    //    LogType = NotificationType.LogAPI.ToString(),
                    //    CreateTime = DateTime.Now,
                    //    Bak = type
                    //});
                }
                catch (Exception ex)
                {
                    Log.Error($"{type} 接口日志  写入数据库失败！ex：{ex.ToString()}", ex);
                }

            }
        }
        /// <summary>
        /// 接口日志记录
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="message">只能是报文</param>
        /// <param name="content">输出的备注</param>
        /// <param name="isloginformation">是否写入Information</param>
        public async Task LogAPIAsync(string type, string message, string content, bool isloginformation = false)
        {
            Log.Verbose("【type】：{type}【message】：{message},【content】：{content}", type, message, content);
            if (isloginformation)
            {
                try
                {
                    Log.Information("【message】：{message},【content】：{content}", message, content);
                    //await _logInfoService.InsertAsync(new LogInfo
                    //{
                    //    LogLevel = NotificationLevel.Perilous.ToString(),
                    //    LogMessage = "【type】：" + type + " 【content】：" + message,
                    //    LogType = NotificationType.LogAPI.ToString(),
                    //    CreateTime = DateTime.Now,
                    //    Bak = type
                    //});
                }
                catch (Exception ex)
                {
                    Log.Error($"{type} 接口日志  写入数据库失败！ex：{ex.ToString()}", ex);
                }

            }
        }

        private void InsertLog(NotificationLevel Level, NotificationType notifiType, string message)
        {
            try
            {
                //_logInfoService.Insert(new LogInfo
                //{
                //    LogLevel = Level.ToString(),
                //    LogMessage = message,
                //    LogType = notifiType.ToString(),
                //    CreateTime = DateTime.Now
                //});
            }
            catch (Exception ex)
            {
                Log.Error($"日志  写入数据库失败！ex：{ex.ToString()}", ex);
            }
        }

        private async Task InsertLogAsync(NotificationLevel Level, NotificationType notifiType, string message)
        {
            try
            {
                //await _logInfoService.InsertAsync(new LogInfo
                //{
                //    LogLevel = Level.ToString(),
                //    LogMessage = message,
                //    LogType = notifiType.ToString(),
                //    CreateTime = DateTime.Now
                //});
            }
            catch (Exception ex)
            {
                Log.Error($"日志  写入数据库失败！ex：{ex.ToString()}", ex);
            }
        }

        /// <summary>
        /// 发送消息和记录日志
        /// </summary>
        /// <param name="isSend">是否发送</param>
        /// <param name="SendColor">发送消息的颜色</param>
        /// <param name="isLogInfo">是否记录日志</param>
        /// <param name="message">消息</param>
        private void SendAndLog(bool isSend, string SendColor, bool isLogInfo, string message)
        {
            if (isSend)
            {
                TipMsgArg msg = new TipMsgArg(message, SendColor);
                _eventAggregator.GetEvent<TipMsgEvent>().Publish(msg);
            }
            //日志
            if (isLogInfo)
            {
                Log.Information(message);
            }
        }

        /// <summary>
        /// 等级灯光提示
        /// </summary>
        /// <param name="notificationLevel"></param>
        /// <param name="type"></param>
        private void Lamp(NotificationLevel notificationLevel, NotificationType type)
        {
            try
            {
                switch (type)
                {
                    case NotificationType.ReadyNotice:
                        switch (notificationLevel)
                        {
                            case NotificationLevel.Normal:
                                if (_colorLampDevice?.DeviceType == "QLight_LampDevice")
                                {
                                    _colorLampDevice?.LampWrite(0, 1, 0, 0); //黄长亮
                                }
                                else//四色灯
                                {
                                    if (_allDeviceConfig.ColorLampConfig.LightHouseNumber == 4)
                                    {
                                        _colorLampDevice?.LampWrite(0, 0, 0, 0, 1);//蓝长亮
                                    }
                                    else {
                                        _colorLampDevice?.LampWrite(0, 1, 0, 0); //黄长亮
                                    }

                                }
                                break;
                            case NotificationLevel.Urgent:
                                if (_colorLampDevice?.DeviceType == "QLight_LampDevice")//三色灯
                                {
                                    _colorLampDevice?.LampWrite(0, 2, 0, 0); //黄闪烁
                                }
                                else//四色灯
                                {
                                    if (_allDeviceConfig.ColorLampConfig.LightHouseNumber == 4)
                                    {
                                        _colorLampDevice?.LampWrite(0, 0, 0, 0, 2);//蓝闪烁
                                    }
                                    else
                                    {
                                        _colorLampDevice?.LampWrite(0, 2, 0, 0); //黄闪烁
                                    }
                                }
                                break;
                            case NotificationLevel.Perilous:
                                break;
                            default:
                                break;
                        }
                        break;
                    case NotificationType.SuccseNotice:
                        switch (notificationLevel)
                        {
                            case NotificationLevel.Normal:
                                break;
                            case NotificationLevel.Urgent:
                                if (_colorLampDevice?.DeviceType == "QLight_LampDevice")//三色灯
                                {
                                    _colorLampDevice?.LampWrite(0, 0, 2, 0); //绿闪烁
                                }
                                else//四色灯
                                {
                                    if (_allDeviceConfig.ColorLampConfig.LightHouseNumber == 4)
                                    {
                                        _colorLampDevice?.LampWrite(0, 0, 2, 0, 0);//绿闪烁 蓝长亮
                                    }
                                    else
                                    {
                                        _colorLampDevice?.LampWrite(0, 0, 2, 0); //绿闪烁
                                    }
                                }
                                break;
                            case NotificationLevel.Perilous:
                                if (_colorLampDevice?.DeviceType == "QLight_LampDevice")//三色灯
                                {
                                    _colorLampDevice?.LampWrite(0, 0, 1, 0); //绿长亮
                                }
                                else//四色灯
                                {
                                    if (_allDeviceConfig.ColorLampConfig.LightHouseNumber == 4)
                                    {
                                        _colorLampDevice?.LampWrite(0, 0, 1, 0, 0);//绿长亮
                                    }
                                    else
                                    {
                                        _colorLampDevice?.LampWrite(0, 0, 1, 0); //绿长亮
                                    }
                                }
                                break;
                        }
                        break;
                    case NotificationType.ErrNotice:
                        switch (notificationLevel)
                        {
                            case NotificationLevel.Normal:
                                break;
                            case NotificationLevel.Urgent:
                                if (_colorLampDevice?.DeviceType == "QLight_LampDevice")//三色灯
                                {
                                    _colorLampDevice?.LampWrite(2, 0, 0, 5); //闪烁蜂鸣
                                }
                                else//四色灯
                                {
                                    if (_allDeviceConfig.ColorLampConfig.LightHouseNumber == 4)
                                    {
                                        _colorLampDevice?.LampWrite(2, 0, 0, 5, 0);//闪烁蜂鸣
                                    }
                                    else
                                    {
                                        _colorLampDevice?.LampWrite(2, 0, 0, 5); //闪烁蜂鸣
                                    }

                                }
                                break;
                            case NotificationLevel.Perilous:
                                if (_colorLampDevice?.DeviceType == "QLight_LampDevice")//三色灯
                                {
                                    _colorLampDevice?.LampWrite(1, 2, 0, 5); //长亮蜂鸣
                                }
                                else//四色灯
                                {                                   
                                    if (_allDeviceConfig.ColorLampConfig.LightHouseNumber == 4)
                                    {
                                        _colorLampDevice?.LampWrite(1, 2, 0, 5, 0);//长亮蜂鸣
                                    }
                                    else
                                    {
                                        _colorLampDevice?.LampWrite(1, 0, 0, 5); //长亮蜂鸣
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Lamp异常");
            }

        }
    }
}
