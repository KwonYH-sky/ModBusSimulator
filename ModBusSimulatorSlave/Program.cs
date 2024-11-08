// See https://aka.ms/new-console-template for more information

using ModBusSimSlave;
using ModBusSimSlave.Data;
using System.Text.RegularExpressions;

const string LOGO = """
                                  _ _                 __ _                 _       _             
          /\/\   ___   __| | |__  _   _ ___  / _(_)_ __ ___  _   _| | __ _| |_ ___  _ __ 
         /    \ / _ \ / _` | '_ \| | | / __| \ \| | '_ ` _ \| | | | |/ _` | __/ _ \| '__|
        / /\/\ \ (_) | (_| | |_) | |_| \__ \ _\ \ | | | | | | |_| | | (_| | || (_) | |   
        \/    \/\___/ \__,_|_.__/ \__,_|___/ \__/_|_| |_| |_|\__,_|_|\__,_|\__\___/|_|   

                               __ _                                                      
                              / _\ | __ ___   _____                                      
                              \ \| |/ _` \ \ / / _ \                                     
                              _\ \ | (_| |\ V /  __/                                     
                              \__/_|\__,_| \_/ \___|                                     
                                                                                         
        """;

Dictionary<int, VirtualDevice> vitualDeviceManagement = [];
List<LogData> logDatas = [];

Regex deviceComPattern = new(@"^\d+ \d+ \d+$");

Console.Title = "Modbus Simulator Slave";
Console.WriteLine(LOGO);

SerialPortConnector serialPortConnector = new(vitualDeviceManagement, logDatas);
string[] ports = SerialPortConnector.GetPortNames();

Console.WriteLine(new String('-', 75));
Console.WriteLine("사용 가능한 시리얼 포트 목록");
ports.ToList().ForEach(port => Console.Write($"{port} | "));

Console.WriteLine('\n' + new String('-', 75));
Console.Write("\n시리얼 포트 선택하기: ");
string portName = Console.ReadLine().ToUpper();

try
{
    serialPortConnector.Open(portName);
}
catch (Exception e)
{
    Console.WriteLine("잘못된 포트이거나 이미 사용 중인 포트입니다.");
    Console.Error.WriteLine(e.Message);
    return;
}

Console.WriteLine('\n'+ new string('-', 75));
Console.WriteLine("연결되었습니다.");
Console.WriteLine(new string('-', 75));
Console.WriteLine();

while (true)
{
    Console.WriteLine(new String('-', 75));
    Console.WriteLine("1. 가상 장치 추가");
    Console.WriteLine("2. 가상 장치 제거");
    Console.WriteLine("3. 가상 장치 보기");
    Console.WriteLine("4. 로그 보기");
    Console.WriteLine("5. 로그 데이터 비우기");
    Console.WriteLine("Clear: 화면 지우기");
    Console.WriteLine("종료하려면 'q' or 'Q'를 입력하세요.");
    Console.WriteLine(new String('-', 75));

    Console.Write("\n명령어를 입력하세요: ");

    string input = Console.ReadLine()?.ToLower();

    switch (input)
    {
        case "1":
            Console.WriteLine("추가할 가상 장치의 Slave ID, Coil 수, 레지스터 수를 공백 구분으로 입력하세요.");
            Console.WriteLine("EX) 2 10 10 (Slave ID: 2, Coil: 10, Register: 10)");
            string rawCommand = Console.ReadLine();
            if (!deviceComPattern.IsMatch(rawCommand))
            {
                Console.WriteLine("올바른 형식이 아닙니다.");
                break;
            }
            int[] command = rawCommand.Split(' ').Select(e => int.Parse(e)).ToArray();
            AddVirtualDevice(command[0], command[1], command[2]);
            break;

        case "2":
            Console.WriteLine("제거할 가상 장치의 Slave ID를 입력하세요.");
            string rawSlaveID = Console.ReadLine();
            if (!int.TryParse(rawSlaveID, out int slaveID))
            {
                Console.WriteLine("올바른 형식이 아닙니다.");
                break;
            }
            RemoveVirtualDevice(slaveID);
            break;

        case "3":
            ViewVirtualDevice();
            break;

        case "4":
            ViewLogData();
            break;

        case "5":
            ClearLogDatas();
            break;

        case "clear":
            Console.Clear();
            Console.WriteLine(LOGO);
            break;

        case "q":
            serialPortConnector.Close();
            Console.WriteLine("종료합니다.");
            return;

        default:
            Console.WriteLine("옳지 않은 명령어입니다.");
            break;

    }
}

void AddVirtualDevice(int slaveID, int coilCnt = 10, int registerCnt = 10)
{
    if (vitualDeviceManagement.ContainsKey(slaveID))
    {
        Console.WriteLine("이미 존재하는 슬레이브 ID입니다.\n");
        return;
    }

    if (coilCnt < 10 || registerCnt < 10)
    {
        Console.WriteLine("최소 10개 이상의 코일과 레지스터를 가질 수 있습니다.");
        coilCnt = 10;
        registerCnt = 10;
        Console.WriteLine("기본값으로 설정합니다.");
    }

    VirtualDevice virtualDevice = new(slaveID, coilCnt, registerCnt);
    vitualDeviceManagement.Add(slaveID, virtualDevice);
    Console.WriteLine("가상 장치가 추가되었습니다.\n");
}

void RemoveVirtualDevice(int slaveID)
{
    if (slaveID == 1)
    {
        Console.WriteLine("기본 슬레이브 ID는 제거할 수 없습니다.\n");
        return;
    }

    vitualDeviceManagement.Remove(slaveID);
    Console.WriteLine($"Slave ID: {slaveID} 가상 장치가 제거되었습니다.\n");
}

void ViewVirtualDevice()
{
    Console.WriteLine(new String('-', 75));
    Console.WriteLine("Slave ID\tCoil\tDiscrete\tHolding Register\tInput Register");
    Console.WriteLine(new string('-', 75));

    foreach (var device in vitualDeviceManagement)
    {
        Console.WriteLine($"{device.Key}\t\t{device.Value.Coils.Length}\t{device.Value.DiscreteInputs.Length}\t{device.Value.HoldingRegisters.Length}\t{device.Value.InputRegisters.Length}");
    }
    Console.WriteLine(new String('-', 75));
    Console.WriteLine();
}


void ViewLogData ()
{
    Console.WriteLine(new String('-', 75));

    if (logDatas.Count == 0)
    {
        Console.WriteLine("로그 데이터가 없습니다.");
    }

    logDatas.ForEach(log =>
    {
        Console.Write(log.Data);
        Console.WriteLine($"Log 시간 : {log.Time}\n");
    });
    
    Console.WriteLine(new String('-', 75));
    Console.WriteLine();
}

void ClearLogDatas()
{
    logDatas.Clear();
    Console.WriteLine("Log 데이터를 지웠습니다.");
}