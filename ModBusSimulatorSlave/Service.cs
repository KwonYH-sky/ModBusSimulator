using ModBusMaster.Data;

namespace ModBusSlave
{
    class Service
    {
        private VirtualDevice virtualDevice { get; set; }
        private RequestPacket requestPacket { get; set; }

        public Service(VirtualDevice device, RequestPacket packet)
        {
            virtualDevice = device;
            requestPacket = packet;
        }

        public ResponsePacket? Response()
        {
            if (requestPacket.SlaveAddr != virtualDevice.SlaveID) return null;

            Console.WriteLine("수신 데이터");
            Console.WriteLine($"SlaveAddr: {requestPacket.SlaveAddr} FunctioanCode: {requestPacket.FunctionCode}");
            requestPacket.Data.ToList().ForEach(e => Console.Write($"{e} "));
            Console.WriteLine("");

            return requestPacket.FunctionCode switch
            {
                0x01 => ReadCoils(),
                0x02 => ReadDiscreteInputs(),
                0x03 => ReadHoldingRegisters(),
                0x04 => ReadInputRegisters(),
                0x05 => WriteSingleCoil(),
                0x06 => WriteSingleRegister(),
                0x0F => WriteMultipleCoils(),
                0x10 => WriteMultipleRegisters(),
                _ => null,
            };
        }

        private ResponsePacket ReadCoils()
        {
            // TODO: ReadCoils
            return null;
        }

        private ResponsePacket ReadDiscreteInputs()
        {
            // TODO: ReadDiscreteInputs
            return null;
        }

        private ResponsePacket ReadHoldingRegisters()
        {
            ushort address = (ushort)((requestPacket.Data[0] << 8) | requestPacket.Data[1] & 0xFF);
            ushort quantity = (ushort)((requestPacket.Data[2] << 8) | requestPacket.Data[3] & 0xFF);

            // TODO: ReadHoldingRegisters
            return null;
        }

        private ResponsePacket ReadInputRegisters()
        {
            ushort address = (ushort)((requestPacket.Data[0] << 8) | requestPacket.Data[1] & 0xFF);
            ushort quantity = (ushort)((requestPacket.Data[2] << 8) | requestPacket.Data[3] & 0xFF);

            byte byteCount = (byte)(quantity * 2);
            byte[] data = new byte[byteCount];

            for (int i = 0; i < quantity; i++)
            {
                ushort value = virtualDevice.InputRegisters[address + i];
                data[i * 2] = (byte)(value >> 8);
                data[i * 2 + 1] = (byte)(value & 0xFF);
            }

            return new ResponsePacket.ResponsePacketBuilder()
                .SetSlaveAddr(requestPacket.SlaveAddr)
                .SetFunctionCode(requestPacket.FunctionCode)
                .SetDataLength(byteCount)
                .SetData(data)
                .Build();
        }

        private ResponsePacket WriteSingleCoil()
        {
            // TODO: WriteSingleCoil
            return null;
        }

        private ResponsePacket WriteSingleRegister()
        {
            // TODO: WriteSingleRegister
            return null;
        }

        private ResponsePacket WriteMultipleCoils()
        {
            // TODO: WriteMultipleCoils
            return null;
        }

        private ResponsePacket WriteMultipleRegisters()
        {
            // TODO: WriteMultipleRegisters
            return null;
        }

    }
}
