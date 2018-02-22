using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;

namespace Cellevate{
    public class PumpTest{
        static SerialPort _serialPort;
        static bool _communicateToPump = true;
        public PumpTest(){
            /*
            Dessa värden måste vara de samma som spec för pumpen.
            Vet inte om COM1 beror på i vilken port pumpen är kopplad */
            //_serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            _serialPort = new SerialPort();
            _serialPort.Handshake = Handshake.None;
            //_serialPort.RtsEnable = true; 
             
             //problem med att få read tråden att sluta köra
             //Thread readThread = new Thread(Read);
             //readThread.IsBackground = true;
             //readThread.Start();

             _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

             _serialPort.Open();
             sendStringToPump();
             _serialPort.Close();    
        }

        public static void sendStringToPump(){
            //behöver veta hur datan som skickas ser ut för att göra denna
            while(_communicateToPump){
                string _toSend = Console.ReadLine();
                if(_toSend.Equals("quit")){
                 
                   _communicateToPump = false;
                }else{
                    
                    _toSend = _toSend + "\n";
                    _serialPort.Write(_toSend);
                }
            }
        }

        public static void sendByteToPump(){
            while(_communicateToPump){
                string _stringToSend = Console.ReadLine();
                if(_stringToSend.Equals("quit")){
                   _communicateToPump = false;
                }else{
                    _stringToSend = _stringToSend + "\n";
                    byte[] _bytesToSend = System.Text.ASCIIEncoding.Default.GetBytes(_stringToSend);
                    _serialPort.Write(_bytesToSend, 0, _bytesToSend.Length);
                }
            }  
        }

        public static void Read(){
            while(_communicateToPump){
                try{
                    //behöver veta vilken data pumpen skickar tillbaka för att kuna läsa den rätt

                    byte[] _bytesFromPort = new byte[10];
                    int _lengthOfMessage = _serialPort.Read(_bytesFromPort, 0, 10);
                    byte[] _byteMessage = new byte[_lengthOfMessage];
                    for(int i = 0; i < _lengthOfMessage; i++){
                        _byteMessage[i] = _bytesFromPort[i];
                    }
                    string _messageAsString = System.Text.ASCIIEncoding.Default.GetString(_byteMessage);
                    Console.WriteLine(_messageAsString);
                }catch(Exception e){
                    Console.WriteLine("Read thread stopping");
                }
            }
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e){
            SerialPort sp = (SerialPort)sender;
            //behöver veta vilken data pumpen skickar tillbaka för att kuna läsa den rätt
            byte[] _bytesFromPort = new byte[10];
            int _lengthOfMessage = sp.Read(_bytesFromPort, 0, 10);
            byte[] _byteMessage = new byte[_lengthOfMessage];
            for(int i = 0; i < _lengthOfMessage; i++){
                _byteMessage[i] = _bytesFromPort[i];
            }
            string _messageAsString = System.Text.ASCIIEncoding.Default.GetString(_byteMessage);
            Console.WriteLine(_messageAsString);
                
        }

        public List<String> listAllPorts(){
            List<String> allPorts = new List<String>();
            foreach(String portName in System.IO.Ports.SerialPort.GetPortNames()){
                allPorts.Add(portName);
                Console.WriteLine(portName);
            }
            return allPorts;

        }

        public String finished(){
            return "klar";
        }
    }
}

/*
byte[] bytesToSend = System.Text.ASCIIEncoding.Default.GetBytes("Hello World\n);
_serialPort.Write(bytesToSend, 0, bytesToSend.Length);
 */

    
