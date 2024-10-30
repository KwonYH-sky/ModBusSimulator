namespace ModBusSimSlave
{
    public class VirtualDevice
    {
        public int SlaveID { get; set; }

        public bool[] Coils { get; set; }
        public bool[] DiscreteInputs { get; set; }

        public ushort[] HoldingRegisters { get; set; }
        public ushort[] InputRegisters { get; set; }

        public DateTime LastComunication { get; set; }


        public VirtualDevice(int slaveID, int coilCnt, int registerCnt)
        {
            SlaveID = slaveID;

            Coils = new bool[coilCnt];
            DiscreteInputs = new bool[coilCnt];

            HoldingRegisters = new ushort[registerCnt];
            InputRegisters = new ushort[registerCnt];

            LastComunication = DateTime.Now;
        }

        public bool ReadCoil(int address)
        {
            return Coils[address];
        }

        public bool ReadDiscreteInput(int address)
        {
            return DiscreteInputs[address];
        }

        public ushort ReadHoldingRegister(int address)
        {
            return HoldingRegisters[address];
        }

        public ushort ReadInputRegister(int address)
        {
            return InputRegisters[address];
        }

        public void WriteCoil(int address, bool value)
        {
            Coils[address] = value;
        }

        public void WriteHoldingRegister(int address, ushort value)
        {
            HoldingRegisters[address] = value;
        }

        public void UpdateComunication()
        {
            LastComunication = DateTime.Now;
        }
    }
}
