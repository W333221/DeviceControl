using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using TouchSocket.Core;
using TouchSocket.Modbus;
using TouchSocket.Sockets;

namespace DeviceControl.Core.Command.Modbus
{
    public class TouchSocketModbusHelper : IModbusHelper
    {
        private string _ip;
        private int _port;
        private ModbusTcpMaster modbusTcpMaster;
        public TouchSocketModbusHelper(string ip, int port)
        {
            _ip = ip;
            _port = port;
            modbusTcpMaster = new ModbusTcpMaster();
        }

        public async Task ConnectAsync()
        {
            var config = new TouchSocketConfig();
            config.SetRemoteIPHost($"{_ip}:{_port}");
            config.ConfigurePlugins(a =>
            {
                a.UseReconnection<ModbusTcpMaster>(options =>
                {
                    options.PollingInterval = TimeSpan.FromSeconds(1);
                });
            });

            await modbusTcpMaster.SetupAsync(config);
            await modbusTcpMaster.ConnectAsync();
        }

        /// <summary>
        /// 读多个线圈（FC1）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<bool[]> ReadCoilsAsync(byte slaveId, ushort startingAddress, ushort quantity)
        {
            var values = await modbusTcpMaster.ReadCoilsAsync(slaveId, startingAddress, quantity);
            return values.ToArray();
        }

        /// <summary>
        /// 读离散输入（FC2）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<bool[]> ReadDiscreteInputsAsync(byte slaveId, ushort startingAddress, ushort quantity)
        {
            var values = await modbusTcpMaster.ReadDiscreteInputsAsync(slaveId, startingAddress, quantity);
            return values.ToArray();
        }

        /// <summary>
        /// 读保持寄存器（FC3）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort startingAddress, ushort quantity)
        {
            var response = await modbusTcpMaster.ReadHoldingRegistersAsync(slaveId, startingAddress, quantity);
            if (response.IsSuccess)
            {
                List<ushort> values = new List<ushort>();
                ReadOnlySpan<byte> span = response.Data.Span;
                for (int i = 0; i < quantity; i++)
                {
                    values.Add(span.ReadValue<ushort>(EndianType.Big));
                }
                return values.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读输入寄存器（FC4）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<ushort[]> ReadInputRegistersAsync(byte slaveId, ushort startingAddress, ushort quantity)
        {
            var response = await modbusTcpMaster.ReadInputRegistersAsync(slaveId, startingAddress, quantity);
            if (response.IsSuccess)
            {
                List<ushort> values = new List<ushort>();
                ReadOnlySpan<byte> span = response.Data.Span;
                for (int i = 0; i < quantity; i++)
                {
                    values.Add(span.ReadValue<ushort>(EndianType.Big));
                }
                return values.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 写单个线圈（FC5）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> WriteSingleCoilAsync(byte slaveId, ushort startingAddress, bool value)
        {
            var response = await modbusTcpMaster.WriteSingleCoilAsync(slaveId, startingAddress, value);
            return response.IsSuccess;
        }


        /// <summary>
        ///  写单个寄存器（FC6）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> WriteSingleRegisterAsync(byte slaveId, ushort startingAddress, ushort value)
        {
            var response = await modbusTcpMaster.WriteSingleRegisterAsync(slaveId, startingAddress, (short)value);
            return response.IsSuccess;
        }


        /// <summary>
        /// 写多个线圈（FC15）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<bool> WriteMultipleCoilsAsync(byte slaveId, ushort startingAddress, bool[] values)
        {
            IModbusResponse modbusResponse = await modbusTcpMaster.WriteMultipleCoilsAsync(slaveId, startingAddress, values);
            return modbusResponse.IsSuccess;
        }


        /// <summary>
        /// 写多个寄存器（FC16）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<bool> WriteMultipleRegistersAsync(byte slaveId, ushort startingAddress, ushort[] values)
        {
            Span<byte> span = new Span<byte>();
            foreach (var item in values)
            {
                span.WriteValue(item);
            }
            IModbusResponse modbusResponse = await modbusTcpMaster.WriteMultipleRegistersAsync(slaveId, startingAddress, span.ToArray());
            return modbusResponse.IsSuccess;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            await modbusTcpMaster.CloseAsync();
        }
    }
}
