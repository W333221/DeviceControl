using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Command.Modbus
{
    public interface IModbusHelper
    {
        public Task ConnectAsync();
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync();

        /// <summary>
        /// 读多个线圈（FC1）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public Task<bool[]> ReadCoilsAsync(byte slaveId, ushort startingAddress, ushort quantity);

        /// <summary>
        /// 读离散输入（FC2）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public Task<bool[]> ReadDiscreteInputsAsync(byte slaveId, ushort startingAddress, ushort quantity);

        /// <summary>
        /// 读保持寄存器（FC3）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort startingAddress, ushort quantity);

        /// <summary>
        /// 读输入寄存器（FC4）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public Task<ushort[]> ReadInputRegistersAsync(byte slaveId, ushort startingAddress, ushort quantity);

        /// <summary>
        /// 写单个线圈（FC5）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> WriteSingleCoilAsync(byte slaveId, ushort startingAddress, bool value);

        /// <summary>
        ///  写单个寄存器（FC6）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> WriteSingleRegisterAsync(byte slaveId, ushort startingAddress, ushort value);


        /// <summary>
        /// 写多个线圈（FC15）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Task<bool> WriteMultipleCoilsAsync(byte slaveId, ushort startingAddress, bool[] values);

        /// <summary>
        /// 写多个寄存器（FC16）
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startingAddress"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Task<bool> WriteMultipleRegistersAsync(byte slaveId, ushort startingAddress, ushort[] values);
    }
}
