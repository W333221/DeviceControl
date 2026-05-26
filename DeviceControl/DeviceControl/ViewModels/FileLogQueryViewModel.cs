using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;

namespace DeviceControl.ViewModels
{
    /// <summary>
    /// 单行日志模型（用于 ListBox 虚拟化绑定）
    /// </summary>
    public class LogLineModel : BindableBase
    {
        private string _text;
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get => _isHighlighted;
            set => SetProperty(ref _isHighlighted, value);
        }
    }

    public class FileLogQueryViewModel : BindableBase
    {
        private readonly Dispatcher _uiDispatcher;

        public FileLogQueryViewModel()
        {
            _uiDispatcher = Dispatcher.CurrentDispatcher;
            OpenFileCommand = new DelegateCommand(OnOpenFile);
            ClearFilterCommand = new DelegateCommand(OnClearFilter);
        }

        #region 属性

        private string _filterText = "";
        public string FilterText
        {
            get => _filterText;
            set => SetProperty(ref _filterText, value, OnFilterTextChanged);
        }

        private string _highlightText = "";
        public string HighlightText
        {
            get => _highlightText;
            set => SetProperty(ref _highlightText, value, OnFilterTextChanged);
        }

        private bool _useRegex = false;
        public bool UseRegex
        {
            get => _useRegex;
            set => SetProperty(ref _useRegex, value, OnFilterTextChanged);
        }

        private string _selectedColumn = "全文本";
        public string SelectedColumn
        {
            get => _selectedColumn;
            set => SetProperty(ref _selectedColumn, value, OnFilterTextChanged);
        }

        public List<string> ColumnOptions { get; } = new List<string>
        {
            "全文本", "VIN"
        };

        private string _statusText = "就绪";
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        // 绑定到 ListBox 的集合（虚拟化友好）
        public ObservableCollection<LogLineModel> DisplayLines { get; } = new ObservableCollection<LogLineModel>();

        #endregion

        #region 命令

        public DelegateCommand OpenFileCommand { get; }
        public DelegateCommand ClearFilterCommand { get; }

        #endregion

        #region 私有字段

        private List<string> _rawLines = new List<string>();   // 原始行（内存中）
        private CancellationTokenSource _filterCts;            // 用于取消旧的过滤任务

        #endregion

        #region 命令实现

        private async void OnOpenFile()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "日志文件|*.txt|所有文件|*.*",
                    Multiselect = true
                };
                if (dialog.ShowDialog() == true)
                {
                    await LoadFilesAsync(dialog.FileNames);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "加载文件时发生错误");
            }
        }

        private void OnClearFilter()
        {
            FilterText = "";
            HighlightText = "";
            SelectedColumn = "全文本";
            UseRegex = false;
        }

        #endregion

        #region 异步加载文件

        private async Task LoadFilesAsync(string[] paths)
        {
            StatusText = "正在加载文件...";
            DisplayLines.Clear();
            _rawLines.Clear();

            // 取消正在进行的过滤任务
            _filterCts?.Cancel();
            _filterCts = new CancellationTokenSource();

            await Task.Run(() =>
            {
                var allLines = new List<string>();
                foreach (var path in paths)
                {
                    var lines = File.ReadLines(path, Encoding.UTF8);
                    allLines.AddRange(lines);
                }
                // 批量赋值到原始行
                _uiDispatcher.Invoke(() =>
                {
                    _rawLines = allLines;
                    StatusText = $"已加载 {_rawLines.Count} 行，来自 {paths.Length} 个文件";
                    // 加载完成后立即执行过滤（此时可能无过滤条件，直接显示全部）
                    ApplyFilterAsync();
                });
            });
        }

        #endregion

        #region 异步过滤与高亮

        private void OnFilterTextChanged()
        {
            ApplyFilterAsync();
        }

        private async void ApplyFilterAsync()
        {
            if (_rawLines.Count == 0) return;

            // 取消旧的过滤任务
            _filterCts?.Cancel();
            _filterCts = new CancellationTokenSource();
            var token = _filterCts.Token;

            // 捕获当前过滤条件（避免过滤过程中条件被修改）
            string filter = FilterText;
            string highlight = HighlightText;
            string column = SelectedColumn;
            bool useRegex = UseRegex;

            StatusText = "正在过滤...";

            try
            {
                // 在后台线程执行过滤和高亮判断
                var filteredModels = await Task.Run(() =>
                {
                    var result = new List<LogLineModel>();

                    // 预编译正则（如果需要）
                    Regex filterRegex = null;
                    if (useRegex && !string.IsNullOrEmpty(filter))
                    {
                        try { filterRegex = new Regex(filter, RegexOptions.IgnoreCase | RegexOptions.Compiled); }
                        catch { useRegex = false; } // 正则无效，回退到普通包含
                    }

                    foreach (var line in _rawLines)
                    {
                        if (token.IsCancellationRequested) return null;

                        // 过滤判断
                        bool passFilter = string.IsNullOrEmpty(filter);
                        if (!passFilter)
                        {
                            string fieldValue = GetFieldValue(line, column);
                            if (useRegex && filterRegex != null)
                                passFilter = filterRegex.IsMatch(fieldValue);
                            else
                                passFilter = fieldValue.Contains(filter);
                        }

                        if (!passFilter) continue;

                        // 高亮判断（只要行包含高亮词就标记）
                        bool isHighlight = false;
                        if (!string.IsNullOrEmpty(highlight))
                        {
                            string fieldForHighlight = GetFieldValue(line, column);
                            if (useRegex)
                            {
                                try { isHighlight = Regex.IsMatch(fieldForHighlight, highlight, RegexOptions.IgnoreCase); }
                                catch { isHighlight = fieldForHighlight.Contains(highlight); }
                            }
                            else
                            {
                                isHighlight = fieldForHighlight.Contains(highlight);
                            }
                        }

                        result.Add(new LogLineModel { Text = line, IsHighlighted = isHighlight });
                    }

                    return result;
                }, token);

                if (token.IsCancellationRequested) return;

                // 回到 UI 线程，批量更新 ObservableCollection（一次性清空并添加，避免逐条刷新）
                _uiDispatcher.Invoke(() =>
                {
                    DisplayLines.Clear();
                    if (filteredModels != null)
                    {
                        foreach (var model in filteredModels)
                            DisplayLines.Add(model);
                    }
                    StatusText = $"显示 {DisplayLines.Count} / {_rawLines.Count} 行";
                });
            }
            catch (OperationCanceledException)
            {
                // 任务被取消，忽略
            }
            catch (Exception ex)
            {
                _uiDispatcher.Invoke(() => StatusText = $"过滤出错: {ex.Message}");
            }
        }

        #endregion

        #region 辅助方法

        private string GetFieldValue(string line, string column)
        {
            switch (column)
            {
                case "VIN":
                    return ExtractByPattern(line, @"vin[\""']?\s*:\s*[\""']([A-Z0-9]+)");
                case "消息ID":
                    return ExtractByPattern(line, @"消息ID[=:]\s*([A-Za-z0-9_\-]+)");
                case "位置编码":
                    return ExtractByPattern(line, @"positionCode[\""']?\s*:\s*[\""']([^\""]+)");
                case "日志级别":
                    return ExtractLogLevel(line);
                default:
                    return line;
            }
        }

        private string ExtractByPattern(string line, string pattern)
        {
            var match = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : "";
        }

        private string ExtractLogLevel(string line)
        {
            if (line.Contains("[INF]")) return "INF";
            if (line.Contains("[WRN]")) return "WRN";
            if (line.Contains("[ERR]")) return "ERR";
            return "";
        }

        #endregion
    }
}