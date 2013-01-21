using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ATCommandsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string p in ports)
            {


                var port = new SerialPort(p);
                port.BaudRate = 19200; // 115200;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Handshake = Handshake.RequestToSend;
                port.DtrEnable = true;
                port.RtsEnable = true;
                port.NewLine = System.Environment.NewLine;
                port.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);

                try
                {
                    port.Open();
                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();
                    var command = "AT";
                    while (!command.Equals("exit"))
                    {
                        port.WriteLine(command);
                        command = Console.ReadLine();
                    }
                    port.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Console.ReadKey();
        }

        private static void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort spL = (SerialPort)sender;
            const int bufSize = 12;
            Byte[] buf = new Byte[bufSize];
            Console.WriteLine("DATA RECEIVED!");
            Console.WriteLine(spL.ReadExisting());
            //Console.WriteLine(spL.Read(buf, 0, bufSize));
        }
    }
}
