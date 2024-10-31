using ModBusSimSlave.Data;
using System.Diagnostics;
using System.IO.Ports;

namespace ModBusSimSlave
{
    public class SerialPortConnector
    {
        private SerialPort seriallPort = new();
        private VirtualDevice device = new(1, 10, 10);
        private Service service;
        private readonly List<byte> packetBuffer = [];

        public SerialPortConnector()
        {
            seriallPort.BaudRate = 115200;
            seriallPort.Parity = Parity.None;
            seriallPort.DataBits = 8;
            seriallPort.StopBits = StopBits.One;
            seriallPort.Handshake = Handshake.None;
            seriallPort.ReadTimeout = 500;
            seriallPort.WriteTimeout = 500;
            seriallPort.DataReceived += DataReceivedHandler;

            device.InputRegisters[0] = Convert.ToUInt16(DateTime.Now.Year);
            device.InputRegisters[1] = Convert.ToUInt16(DateTime.Now.Month);
            device.InputRegisters[2] = Convert.ToUInt16(DateTime.Now.Day);
            device.InputRegisters[3] = Convert.ToUInt16(DateTime.Now.Hour);
            device.InputRegisters[4] = Convert.ToUInt16(DateTime.Now.Minute);
            device.InputRegisters[5] = Convert.ToUInt16(DateTime.Now.Second);

            service = new Service(device);
        }

        private async void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            await Task.Delay(100); // 100ms 지연 추가

            SerialPort sp = (SerialPort)sender;
            byte[] buffer = new byte[sp.BytesToRead];
            sp.Read(buffer, 0, buffer.Length);

            Debug.WriteLine($"버퍼 {buffer}");
            buffer.ToList().ForEach(e => Debug.Write($"{e.ToString("X2")} "));
            Debug.WriteLine("");

            packetBuffer.AddRange(buffer);

            var packet = new RequestPacket(buffer);
            byte[] crc = new byte[2];
            Array.Copy(buffer, buffer.Length - 2, crc, 0, 2);

            if (crc[0] != packet.Crc[0] && crc[1] != packet.Crc[1])
            {
                Console.Error.WriteLine("CRC 불일치");
                return;
            }

            Debug.WriteLine("수신 데이터");
            Debug.WriteLine($"SlaveAddr: {packet.SlaveAddr} FunctioanCode: {packet.FunctionCode}");
            packet.Data.ToList().ForEach(e => Debug.Write($"{e.ToString("X2")} "));
            Debug.WriteLine("");

            ResponsePacket? response = service.Response(packet);

            if (response == null)
            {
                Debug.WriteLine("응답 데이터 없음");
                return;
            }

            byte[] frame = response.Frame;
            Debug.WriteLine("응답 데이터");
            frame.ToList().ForEach(e => Debug.Write($"{e.ToString("X2")} "));
            Debug.WriteLine("");
            seriallPort.Write(frame, 0, frame.Length);
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

        public bool IsOpen()
        {
            return seriallPort.IsOpen;
        }

        public static string[] GetPortNames()
        {
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("사용 가능한 시리얼 포트 목록");
            ports.ToList().ForEach(port => Console.WriteLine($"{port} "));
            Console.WriteLine("시리얼 포트 선택하기: ");
            return ports;
        }

    }
}
