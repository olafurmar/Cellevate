using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Diagnostics;


namespace Cellevate{

    public class MotorTest{
        public MotorTest(){
            //vet inte riktigt vad pipen ska heta "\\\\.\\pipe\\PlanetCNC"
            //ska den vara client eller server?
            //verkar som streamwriter är bättre än streamprotocol

            createPipe();

            StartServerUsingStreamProtocol();
            Task.Delay(1000).Wait();
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(Path.GetTempPath() + "Pipetest");
            Console.WriteLine("Connecting to server...\n");
            pipeClient.Connect();
            
            communicatePipeUsingStreamWriter(new StreamReader(pipeClient), new StreamWriter(pipeClient));
            //StreamProtocol sp = new StreamProtocol(pipeClient);
            //communicatePipeUsingStreamProtocol(sp);

        }
        
        public void communicatePipeUsingStreamProtocol(StreamProtocol sp){
            while(true){
                Console.WriteLine("PlanetCNC: ");
                string input = Console.ReadLine();
                if (String.IsNullOrEmpty(input)){
                     break;
                }
                sp.WriteString(input);
                Console.WriteLine(sp.ReadString());
            }
        }

        public void communicatePipeUsingStreamWriter(StreamReader reader, StreamWriter writer){
             while (true)
            {
                Console.WriteLine("PlanetCNC: ");
                string input = Console.ReadLine();
                if (String.IsNullOrEmpty(input)){
                     break;
                }
                writer.WriteLine(input);
                writer.Flush();
                Console.WriteLine(reader.ReadLine() +"\n");
            }

        }

        public static void StartServerUsingStreamWriter(){
            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream(Path.GetTempPath() + "Pipetest");
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                StreamWriter writer = new StreamWriter(server);
                while (true)
                {
                    var line = reader.ReadLine();
                    writer.WriteLine(String.Join("", line.Reverse()));
                    writer.Flush();
                }
            });
        }
        public static void StartServerUsingStreamProtocol(){
            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream(Path.GetTempPath() + "Pipetest");
                server.WaitForConnection();
                StreamProtocol sp = new StreamProtocol(server);
                while (true)
                {
                    var line = sp.ReadString();
                    sp.WriteString(String.Join("", line.Reverse()));
                    
                }
            });
        }

        public static void createPipe(){
            if (!File.Exists(Path.GetTempPath() + "Pipetest")){
                Console.WriteLine("pipe finns inte");
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/usr/bin/mkfifo", Arguments = Path.GetTempPath() + "Pipetest", };
                Process proc = new Process() { StartInfo = startInfo, };
                proc.Start();
                proc.WaitForExit();
            }else{
                Console.WriteLine("pipe finns");
            }
        }



        public String finished(){
            return "klar";
        }
    }
    
}