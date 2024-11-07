namespace ModBusSimSlave.Data
{
    public class LogData
    {
        public string Data { get; private set; }
        public DateTime Time { get; }

        public LogData(string data)
        {
            this.Data = data;
            this.Time = DateTime.Now;
        }
    }
}
