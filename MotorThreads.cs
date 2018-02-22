using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Cellevate{
    public class MotorThread{
        private static NamedPipeClientStream pipeClient;
        public MotorThread(){

            createPipe();
            startServer();
            Task.Delay(1000).Wait();
            pipeClient = new NamedPipeClientStream(Path.GetTempPath() + "Pipethread");
            Console.WriteLine("Connecting to server...\n");
            pipeClient.Connect();

            Thread rt = new Thread(read);
            Thread wt = new Thread(write);
            rt.Start();
            wt.Start();



        }

        public static void read(){
            StreamReader sr = new StreamReader(pipeClient);
            while(true){
                Console.WriteLine(sr.ReadLine() +"\n");


            }

        }
        public static void write(){
            StreamWriter sw = new StreamWriter(pipeClient);
                Console.WriteLine("PlanetCNC: ");
            while(true){
                string input = Console.ReadLine();
                if (String.IsNullOrEmpty(input)){
                     break;
                }
                sw.WriteLine(input);
                sw.Flush();

            }
        }

        public static void createPipe(){
            if (!File.Exists(Path.GetTempPath() + "Pipethread")){
                Console.WriteLine("pipe finns inte");
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/usr/bin/mkfifo", Arguments = Path.GetTempPath() + "Pipetest", };
                Process proc = new Process() { StartInfo = startInfo, };
                proc.Start();
                proc.WaitForExit();
            }else{
                Console.WriteLine("pipe finns redan");
            }
        }

        public static void startServer(){
            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream(Path.GetTempPath() + "Pipethread");
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                StreamWriter writer = new StreamWriter(server);
                while (true)
                {
                    var line = reader.ReadLine();
                    writer.WriteLine(String.Join("1", line));
                    writer.Flush();
                }
            });
        }
    }

    public class ExecuteMotor{
         static void Main(String[] args){
            MotorThread mt = new MotorThread();
            Console.WriteLine("klar");
        }
    }


    
}