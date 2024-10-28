using System.IO.Ports;

namespace ModBusSlave
{
    public class SerialPortConnector
    {
        SerialPort seriallPort = new SerialPort();

        public SerialPortConnector()
        {
            seriallPort.BaudRate = 115200;
            seriallPort.Parity = Parity.None;
            seriallPort.DataBits = 8;
            seriallPort.StopBits = StopBits.One;
            seriallPort.Handshake = Handshake.None;
            seriallPort.ReadTimeout = 500;
            seriallPort.WriteTimeout = 500;
        }

        public void Open(string portName)
        {
            seriallPort.PortName = portName;
            seriallPort.Open();
        }

        public void Close()
        {
            seriallPort.Close();
        }

        public void Write(byte[] data)
        {
            seriallPort.Write(data, 0, data.Length);
        }

        public byte[] Read()
        {
            byte[] buffer = new byte[seriallPort.BytesToRead];
            seriallPort.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public bool IsOpen()
        {
            return seriallPort.IsOpen;
        }

        public string[] GetPortNames()
        {
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("시리얼 포트 선택하기: ");
            ports.ToList().ForEach(port => Console.WriteLine($"{port} "));
            return ports;
        }

    }
}
