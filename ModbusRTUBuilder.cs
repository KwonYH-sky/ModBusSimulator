namespace ModBusMaster
{
    internal class ModbusRTUBuilder
    {
        private byte _slaveAddr;
        private byte _functionCode;
        private byte[] _data;

        public ModbusRTUBuilder SetSlaveAddr(byte slaveAddr)
        {
            _slaveAddr = slaveAddr;
            return this;
        }

        public ModbusRTUBuilder SetFunctionCode(byte functionCode)
        {
            _functionCode = functionCode;
            return this;
        }

        public ModbusRTUBuilder SetData(byte[] data)
        {
            _data = data;
            return this;
        }

        public ModbusRTU Build()
        {
            return new ModbusRTU(_slaveAddr, _functionCode, _data);
        }
    }
}
