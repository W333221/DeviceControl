using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using System.IO;
using Serilog;


namespace DeviceControl.Core.Command
{
    public class StartupManager
    {
        /// <summary>
        /// 添加程序到启动文件夹
        /// </summary>
        /// <param name="appName">快捷方式名称（不含扩展名）</param>
        /// <param name="exePath">程序完整路径</param>
        /// <param name="description">快捷方式描述（可选）</param>
        public static void AddToStartup(string appName, string exePath, string description = "")
        {
            try
            {
                // 获取启动文件夹路径
                string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                // 创建快捷方式文件路径
                string shortcutPath = Path.Combine(startupPath, $"{appName}.lnk");

                // 创建快捷方式对象
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                // 设置快捷方式属性
                shortcut.TargetPath = exePath;              // 目标程序路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(exePath); // 工作目录
                shortcut.Description = description;         // 描述
                shortcut.IconLocation = exePath + ",0";     // 图标（使用程序图标）
                shortcut.WindowStyle = 1;                   // 窗口样式：1=正常，3=最大化，7=最小化

                // 保存快捷方式
                shortcut.Save();

                Log.Information($"已添加自启动：{shortcutPath}");
            }
            catch (Exception ex)
            {
                Log.Error($"添加自启动失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 移除自启动
        /// </summary>
        /// <param name="appName">快捷方式名称</param>
        public static void RemoveFromStartup(string appName)
        {
            try
            {
                string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupPath, $"{appName}.lnk");

                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                    Log.Information($"已移除自启动：{shortcutPath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"移除自启动失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 检查是否已设置自启动
        /// </summary>
        public static bool IsInStartup(string appName)
        {
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startupPath, $"{appName}.lnk");
            return System.IO.File.Exists(shortcutPath);
        }
    }

}
