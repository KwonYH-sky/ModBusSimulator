using ModBusSimSlave.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace ModBusSimSlave
{
    public class SerialPortConnector
    {
        private readonly SerialPort seriallPort = new();
        private readonly Dictionary<int, VirtualDevice> vitualDeviceManagement;
        private readonly List<LogData> logDatas;
        private readonly Service service;

        private readonly Object packBufferLock = new();
        private readonly List<byte> packetBuffer = [];

        public SerialPortConnector(Dictionary<int, VirtualDevice> management, List<LogData> logs)
        {
            seriallPort.BaudRate = 115200;
            seriallPort.Parity = Parity.None;
            seriallPort.DataBits = 8;
            seriallPort.StopBits = StopBits.One;
            seriallPort.Handshake = Handshake.None;
            seriallPort.ReadTimeout = 500;
            seriallPort.WriteTimeout = 500;
            seriallPort.DataReceived += DataReceivedHandler;

            vitualDeviceManagement = management;
            logDatas = logs;

            VirtualDevice device = new(1, 10, 10);
            vitualDeviceManagement.Add(1, device);

            service = new Service(vitualDeviceManagement);
        }

        private async void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            await Task.Delay(100); // 100ms 지연 추가

            SerialPort sp = (SerialPort)sender;
            byte[] buffer = new byte[sp.BytesToRead];
            sp.Read(buffer, 0, buffer.Length);

            Debug.WriteLine($"버퍼 {buffer}");
            buffer.ToList().ForEach(e => Debug.Write($"{e:X2} "));
            Debug.WriteLine("");

            ProcessPacketBuffer(buffer);
        }

        private void ProcessPacketBuffer(byte[] bytes)
        {
            lock (packBufferLock)
            {
                packetBuffer.AddRange(bytes);
                while (packetBuffer.Count >= 8)
                {
                    int expectedLength = PacketHelpers.GetExpectedRequestPKLength([.. packetBuffer]);

                    if (packetBuffer.Count < expectedLength) break;

                    byte[] packetBytes = [.. packetBuffer.GetRange(0, expectedLength)];

                    if (!PacketHelpers.CheckCRC(packetBytes))
                    {
                        Console.Error.WriteLine("CRC 불일치");
                        packetBuffer.RemoveAt(0);
                        return;
                    }

                    StringBuilder sb = new();

                    var packet = new RequestPacket(packetBytes);

                    Debug.WriteLine("수신 데이터");
                    Debug.WriteLine($"SlaveAddr: {packet.SlaveAddr} FunctioanCode: {packet.FunctionCode}");
                    packet.Data.ToList().ForEach(e => Debug.Write($"{e:X2} "));
                    Debug.WriteLine("");

                    sb.Append("# 수신 데이터\n");
                    packetBytes.ToList().ForEach(e => sb.Append($"{e:X2} "));
                    sb.Append("\n");

                    ResponsePacket response = service.Response(packet);
                    byte[] frame = response.Frame;

                    Debug.WriteLine("응답 데이터");
                    frame.ToList().ForEach(e => Debug.Write($"{e:X2} "));
                    Debug.WriteLine("");

                    sb.Append("# 응답 데이터\n");
                    frame.ToList().ForEach(e => sb.Append($"{e:X2} "));
                    sb.Append("\n");

                    seriallPort.Write(frame, 0, frame.Length);
                    packetBuffer.RemoveRange(0, expectedLength);

                    logDatas.Add(new LogData(sb.ToString()));

                }
            }
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
            return ports;
        }

    }
}
