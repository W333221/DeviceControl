using DeviceControl.Core.Command.Modbus;
using Microsoft.Extensions.Configuration;
using NModbus;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DeviceControl.ViewModels
{
    public class ModbusPageViewModel : BindableBase
    {
        private IModbusHelper _modbusHelper;

        public ModbusPageViewModel(IConfiguration configuration)
        {
            ClearLogCommand = new DelegateCommand(() => CommunicationLogs.Clear());
        }

        #region 连接配置属性
        private string _ipAddress = "127.0.0.1";
        public string IpAddress { get => _ipAddress; set => SetProperty(ref _ipAddress, value); }

        private int _port = 502;
        public int Port { get => _port; set => SetProperty(ref _port, value); }

        private byte _slaveId = 1;
        public byte SlaveId { get => _slaveId; set => SetProperty(ref _slaveId, value); }

        private bool _isConnected;
        public bool IsConnected { get => _isConnected; set { SetProperty(ref _isConnected, value); ConnectButtonText = value ? "断开" : "连接"; } }

        private string _connectButtonText = "连接";
        public string ConnectButtonText { get => _connectButtonText; set => SetProperty(ref _connectButtonText, value); }

        private SolidColorBrush _statusColor = new SolidColorBrush(Colors.Red);
        public SolidColorBrush StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }
        #endregion

        #region 读取属性
        private int _readFunctionCode = 3;
        public int ReadFunctionCode { get => _readFunctionCode; set => SetProperty(ref _readFunctionCode, value); }

        private ushort _readStartAddress;
        public ushort ReadStartAddress { get => _readStartAddress; set => SetProperty(ref _readStartAddress, value); }

        private ushort _readQuantity = 10;
        public ushort ReadQuantity { get => _readQuantity; set => SetProperty(ref _readQuantity, value); }

        public ObservableCollection<ModbusDataItem> ReadResults { get; } = new();
        #endregion

        #region 写入属性
        private int _writeFunctionCode = 6;
        public int WriteFunctionCode { get => _writeFunctionCode; set => SetProperty(ref _writeFunctionCode, value); }

        private ushort _writeStartAddress;
        public ushort WriteStartAddress { get => _writeStartAddress; set => SetProperty(ref _writeStartAddress, value); }

        private string _writeValue = "";
        public string WriteValue { get => _writeValue; set => SetProperty(ref _writeValue, value); }
        #endregion

        #region 日志
        public ObservableCollection<string> CommunicationLogs { get; } = new();
        #endregion

        #region Loading
        private bool _isLoading;
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
        #endregion

        #region 命令
        public DelegateCommand ClearLogCommand { get; }

        public DelegateCommand ConnectCommand => new DelegateCommand(async () => await ConnectAsync());
        public DelegateCommand ReadCommand => new DelegateCommand(async () => await ReadAsync());
        public DelegateCommand WriteCommand => new DelegateCommand(async () => await WriteAsync());
        #endregion

        private void AddLog(string msg)
        {
            var log = $"[{DateTime.Now:HH:mm:ss.fff}] {msg}";
            Application.Current?.Dispatcher.Invoke(() => CommunicationLogs.Add(log));
        }

        private async Task ConnectAsync()
        {
            if (IsConnected)
            {
                try
                {
                    await _modbusHelper?.CloseAsync();
                    IsConnected = false;
                    StatusColor = new SolidColorBrush(Colors.Red);
                    AddLog("已断开连接");
                }
                catch (Exception ex) { AddLog($"断开失败: {ex.Message}"); }
                return;
            }

            try
            {
                IsLoading = true;
                _modbusHelper = new TouchSocketModbusHelper(IpAddress, Port);
                await _modbusHelper.ConnectAsync();
                IsConnected = true;
                StatusColor = new SolidColorBrush(Colors.LimeGreen);
                AddLog($"已连接到 {IpAddress}:{Port}");
            }
            catch (Exception ex)
            {
                IsConnected = false;
                StatusColor = new SolidColorBrush(Colors.Red);
                AddLog($"连接失败: {ex.Message}");
            }
            finally { IsLoading = false; }
        }

        private async Task ReadAsync()
        {
            if (!IsConnected || _modbusHelper == null) { AddLog("请先连接设备"); return; }
            try
            {
                IsLoading = true;
                ReadResults.Clear();
                AddLog($"读取: FC={ReadFunctionCode}, 地址={ReadStartAddress}, 数量={ReadQuantity}");

                switch (ReadFunctionCode)
                {
                    case 1:
                        var coils = await _modbusHelper.ReadCoilsAsync(SlaveId, ReadStartAddress, ReadQuantity);
                        for (int i = 0; i < coils.Length; i++)
                            ReadResults.Add(new ModbusDataItem { Address = ReadStartAddress + i, Value = coils[i] ? "ON" : "OFF" });
                        break;
                    case 2:
                        var inputs = await _modbusHelper.ReadDiscreteInputsAsync(SlaveId, ReadStartAddress, ReadQuantity);
                        for (int i = 0; i < inputs.Length; i++)
                            ReadResults.Add(new ModbusDataItem { Address = ReadStartAddress + i, Value = inputs[i] ? "ON" : "OFF" });
                        break;
                    case 3:
                        var hr = await _modbusHelper.ReadHoldingRegistersAsync(SlaveId, ReadStartAddress, ReadQuantity);
                        for (int i = 0; i < hr.Length; i++)
                            ReadResults.Add(new ModbusDataItem { Address = ReadStartAddress + i, Value = $"0x{hr[i]:X4} ({hr[i]})" });
                        break;
                    case 4:
                        var ir = await _modbusHelper.ReadInputRegistersAsync(SlaveId, ReadStartAddress, ReadQuantity);
                        for (int i = 0; i < ir.Length; i++)
                            ReadResults.Add(new ModbusDataItem { Address = ReadStartAddress + i, Value = $"0x{ir[i]:X4} ({ir[i]})" });
                        break;
                }
                AddLog($"读取完成, 共 {ReadResults.Count} 条");
            }
            catch (Exception ex) { AddLog($"读取失败: {ex.Message}"); }
            finally { IsLoading = false; }
        }

        private async Task WriteAsync()
        {
            if (!IsConnected || _modbusHelper == null) { AddLog("请先连接设备"); return; }
            try
            {
                IsLoading = true;
                switch (WriteFunctionCode)
                {
                    case 5:
                        bool coilVal = WriteValue.Trim().ToUpper() == "ON" || WriteValue.Trim() == "1" || WriteValue.Trim().ToUpper() == "TRUE";
                        await _modbusHelper.WriteSingleCoilAsync(SlaveId, WriteStartAddress, coilVal);
                        AddLog($"写入线圈: 地址={WriteStartAddress}, 值={coilVal}");
                        break;
                    case 6:
                        ushort regVal = Convert.ToUInt16(WriteValue.Trim());
                        await _modbusHelper.WriteSingleRegisterAsync(SlaveId, WriteStartAddress, regVal);
                        AddLog($"写入寄存器: 地址={WriteStartAddress}, 值=0x{regVal:X4} ({regVal})");
                        break;
                    case 15:
                        var coilVals = WriteValue.Split(',').Select(v => v.Trim().ToUpper() == "ON" || v.Trim() == "1").ToArray();
                        await _modbusHelper.WriteMultipleCoilsAsync(SlaveId, WriteStartAddress, coilVals);
                        AddLog($"批量写线圈: 地址={WriteStartAddress}, 数量={coilVals.Length}");
                        break;
                    case 16:
                        var regVals = WriteValue.Split(',').Select(v => Convert.ToUInt16(v.Trim())).ToArray();
                        await _modbusHelper.WriteMultipleRegistersAsync(SlaveId, WriteStartAddress, regVals);
                        AddLog($"批量写寄存器: 地址={WriteStartAddress}, 数量={regVals.Length}");
                        break;
                }
                AddLog("写入成功");
            }
            catch (Exception ex) { AddLog($"写入失败: {ex.Message}"); }
            finally { IsLoading = false; }
        }
    }

    public class ModbusDataItem
    {
        public int Address { get; set; }
        public string Value { get; set; } = "";
    }
}
