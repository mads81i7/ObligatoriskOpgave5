using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using ObligatoriskOpgave1;

namespace ObligatoriskOpgave5
{
    class Program
    {
        public static List<FootballPlayer> FootballPlayers = new List<FootballPlayer>
        {
            new FootballPlayer {Id = 1, Name = "Messi", Price = 100000000, ShirtNumber = 30},
            new FootballPlayer {Id = 2, Name = "Ronaldo", Price = 100000000, ShirtNumber = 7}
        };
        static void Main(string[] args)
        {
            Console.WriteLine("Opgave 5 server");
            TcpListener listener = new TcpListener(IPAddress.Any, 2121);
            listener.Start();

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Console.WriteLine("Ny klient");

                Task.Run((() => NewClient(socket)));
            }
        }

        private static void NewClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            writer.WriteLine("Mulige handlinger");
            writer.WriteLine("HentAlle: Henter alle FootballPlayer-objekter fra server");
            writer.WriteLine("Hent <<id>>: Hent and FootballPlayer med det specifikke objekt (eksempel: linje 1 = Hent | linje 2 = 1)");
            writer.WriteLine("Gem <<Objekt som Json>>: Gemmer et objekt (eksempel: linje 1 = Gem | linje 2 = {\"Id\"})");
            writer.WriteLine("Q for quit");

            writer.Flush();
            
            bool toContinue = true;
            while (toContinue)
            {
                string line1 = reader.ReadLine();
                string line2 = reader.ReadLine();

                switch (line1)
                {
                    case "HentAlle":
                        writer.WriteLine(JsonSerializer.Serialize(FootballPlayers));
                        writer.Flush();
                        break;
                    case "Hent":
                        try
                        {
                            int searchId = int.Parse(line2);
                            FootballPlayer playerHent = FootballPlayers.Find(p => p.Id == searchId);
                            writer.WriteLine(JsonSerializer.Serialize(playerHent));
                            writer.Flush();
                        }
                        catch (Exception e)
                        {
                            writer.WriteLine("EXCEPTION ENCOUNTERED: " + e);
                            writer.Flush();
                        }
                        break;
                    case "Gem":
                        try
                        {
                            FootballPlayer newPlayer = JsonSerializer.Deserialize<FootballPlayer>(line2);
                            FootballPlayers.Add(newPlayer);
                            writer.WriteLine("The FooballPlayer has been added");
                            writer.Flush();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("EXCEPTION ENCOUNTERED: " + e);
                        }
                        break;
                    case "Q":
                        toContinue = false;
                        writer.WriteLine("Forbindelsen til serveren er brudt");
                        writer.Flush();
                        break;
                    default:
                        break;
                }
            }
            socket.Close();
        }
    }
}
