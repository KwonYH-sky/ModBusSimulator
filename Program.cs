// See https://aka.ms/new-console-template for more information

using ModBusSlave;

SerialPortConnector serialPortConnector = new SerialPortConnector();
serialPortConnector.GetPortNames();
string portName = Console.ReadLine();
try
{
    serialPortConnector.Open(portName);
}
catch (Exception e)
{
    Console.WriteLine("잘못된 포트");
    Console.Error.WriteLine(e.Message);
    return;
}

Console.WriteLine("연결되었습니다.");









