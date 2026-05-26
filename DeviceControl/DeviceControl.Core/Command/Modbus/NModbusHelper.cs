using NModbus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Command.Modbus
{
    public class NModbusHelper : IModbusHelper
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly int _maxRetryCount = 3;
        private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(2);
        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(5);

        private IModbusMaster _modbusMaster;
        private TcpClient _tcpClient;
        private readonly ModbusFactory _factory = new ModbusFactory();
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        private bool _isDisposed;
        private bool _isConnected = false;

        public bool IsConnected => _isConnected && _tcpClient?.Connected == true;

        public NModbusHelper(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        /// <summary>
        /// 连接Modbus设备
        /// </summary>
        public async Task ConnectAsync()
        {
            await _connectionLock.WaitAsync();
            try
            {
                if (_isConnected && _tcpClient?.Connected == true)
                    return;

                await InternalDisconnectAsync();

                _tcpClient = new TcpClient();
                _tcpClient.ReceiveTimeout = (int)_connectionTimeout.TotalMilliseconds;
                _tcpClient.SendTimeout = (int)_connectionTimeout.TotalMilliseconds;

                await _tcpClient.ConnectAsync(_ip, _port);

                _modbusMaster = _factory.CreateMaster(_tcpClient);
                _modbusMaster.Transport.ReadTimeout = (int)_connectionTimeout.TotalMilliseconds;
                _modbusMaster.Transport.WriteTimeout = (int)_connectionTimeout.TotalMilliseconds;
                _modbusMaster.Transport.Retries = 1;
                _modbusMaster.Transport.WaitToRetryMilliseconds = 100;

                _isConnected = true;
            }
            catch (Exception ex)
            {
                _isConnected = false;
                throw;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        private async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync();
            try
            {
                await InternalDisconnectAsync();
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        /// <summary>
        /// 内部断开连接方法
        /// </summary>
        private async Task InternalDisconnectAsync()
        {
            try
            {
                if (_modbusMaster != null)
                {
                    _modbusMaster.Dispose();
                    _modbusMaster = null;
                }

                if (_tcpClient != null)
                {
                    if (_tcpClient.Connected)
                    {
                        _tcpClient.Close();
                    }
                    _tcpClient.Dispose();
                    _tcpClient = null;
                }
            }
            finally
            {
                _isConnected = false;
            }
        }

        /// <summary>
        /// 重试策略 - 带指数退避
        /// </summary>
        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
        {
            int retryCount = 0;

            while (true)
            {
                try
                {
                    if (!IsConnected)
                    {
                        await ConnectAsync();
                    }

                    return await operation();
                }
                catch (Exception ex) when (IsConnectionException(ex) && retryCount < _maxRetryCount)
                {
                    retryCount++;

                    await InternalDisconnectAsync();

                    // 指数退避策略
                    var delay = TimeSpan.FromMilliseconds(_retryDelay.TotalMilliseconds * Math.Pow(2, retryCount - 1));
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 判断是否为连接异常
        /// </summary>
        private bool IsConnectionException(Exception ex)
        {
            return ex is IOException ||
                   ex is SocketException ||
                   ex is TimeoutException ||
                   (ex is InvalidOperationException && ex.Message.Contains("连接")) ||
                   ex is System.Net.Sockets.SocketException;
        }

        /// <summary>
        /// 读多个线圈（FC1）
        /// </summary>
        public async Task<bool[]> ReadCoilsAsync(byte slaveId, ushort startingAddress, ushort quantity)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                var values = await _modbusMaster.ReadCoilsAsync(slaveId, startingAddress, quantity);
                return values.ToArray();
            });
        }

        /// <summary>
        /// 读离散输入（FC2）
        /// </summary>
        public async Task<bool[]> ReadDiscreteInputsAsync(byte slaveId, ushort startingAddress, ushort quantity)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                var values = await _modbusMaster.ReadInputsAsync(slaveId, startingAddress, quantity);
                return values.ToArray();
            });
        }

        /// <summary>
        /// 读保持寄存器（FC3）
        /// </summary>
        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort startingAddress, ushort quantity)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                return await _modbusMaster.ReadHoldingRegistersAsync(slaveId, startingAddress, quantity);
            });
        }

        /// <summary>
        /// 读输入寄存器（FC4）
        /// </summary>
        public async Task<ushort[]> ReadInputRegistersAsync(byte slaveId, ushort startingAddress, ushort quantity)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                return await _modbusMaster.ReadInputRegistersAsync(slaveId, startingAddress, quantity);
            });
        }

        /// <summary>
        /// 写单个线圈（FC5）
        /// </summary>
        public async Task<bool> WriteSingleCoilAsync(byte slaveId, ushort startingAddress, bool value)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                await _modbusMaster.WriteSingleCoilAsync(slaveId, startingAddress, value);
                return true;
            });
        }

        /// <summary>
        /// 写单个寄存器（FC6）
        /// </summary>
        public async Task<bool> WriteSingleRegisterAsync(byte slaveId, ushort startingAddress, ushort value)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                await _modbusMaster.WriteSingleRegisterAsync(slaveId, startingAddress, value);
                return true;
            });
        }

        /// <summary>
        /// 写多个线圈（FC15）
        /// </summary>
        public async Task<bool> WriteMultipleCoilsAsync(byte slaveId, ushort startingAddress, bool[] values)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                await _modbusMaster.WriteMultipleCoilsAsync(slaveId, startingAddress, values);
                return true;
            });
        }

        /// <summary>
        /// 写多个寄存器（FC16）
        /// </summary>
        public async Task<bool> WriteMultipleRegistersAsync(byte slaveId, ushort startingAddress, ushort[] values)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                await _modbusMaster.WriteMultipleRegistersAsync(slaveId, startingAddress, values);
                return true;
            });
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            await DisconnectAsync();
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        public async Task<bool> TestConnectionAsync(byte slaveId = 1, ushort testAddress = 0, ushort testQuantity = 1)
        {
            try
            {
                await ExecuteWithRetryAsync(async () =>
                {
                    // 尝试读取一个寄存器来测试连接
                    await _modbusMaster.ReadHoldingRegistersAsync(slaveId, testAddress, testQuantity);
                    return true;
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 定期心跳检测
        /// </summary>
        public async Task<bool> HeartbeatAsync(byte slaveId = 1, ushort heartbeatAddress = 0)
        {
            return await TestConnectionAsync(slaveId, heartbeatAddress, 1);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            try
            {
                _connectionLock.Wait();
                InternalDisconnectAsync().GetAwaiter().GetResult();
            }
            finally
            {
                _connectionLock.Dispose();
            }
        }


    }
}
