﻿//Do Not Delete This Comment... 
//Made By Serenity on 08/20/16
//Any Code Copied Must Source This Project (its the law (:P)) Please.. i work hard on it since 2016.
//Thank You for looking love you guys...

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Tommy;

namespace XDCKIT
{

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class XboxClient
    {
        #region Property's
        public static string IP_Range = "";
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TcpClient XboxName;
        /// <summary>
        /// Checks For Connection, Defaults To False.
        /// </summary>
        public static bool Connected = false;
        private static int port = 730;
        public static string IPAddress { get; set; } = "192.000.000.000";
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static StreamReader Reader;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static readonly BackgroundWorker FindConsoleBc = new BackgroundWorker();
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static readonly BackgroundWorker FindConsolegc = new BackgroundWorker();
        #endregion

        #region Networking
        public static string DefaultConsole
        {
            get
            {
                if (Connected)
                {
                    return IPAddress;
                }
                else
                {
                    return "Error";
                }
            }
            set
            {
                DefaultConsole = value;
            }
        }

        public static int Port { get => port; set => port = value; }

        private static byte[] ConfigFile = {
    0x74, 0x69, 0x74, 0x6C, 0x65, 0x20, 0x3D, 0x20, 0x22, 0x58, 0x44, 0x43,
    0x4B, 0x49, 0x54, 0x20, 0x43, 0x6F, 0x6E, 0x66, 0x69, 0x67, 0x22, 0x0D,
    0x0A, 0x0D, 0x0A, 0x5B, 0x49, 0x50, 0x43, 0x6F, 0x6E, 0x66, 0x69, 0x67,
    0x5D, 0x0D, 0x0A, 0x49, 0x50, 0x5F, 0x52, 0x61, 0x6E, 0x67, 0x65, 0x20,
    0x3D, 0x20, 0x22, 0x31, 0x39, 0x32, 0x2E, 0x31, 0x36, 0x38, 0x2E, 0x30,
    0x2E, 0x22, 0x0D, 0x0A, 0x44, 0x65, 0x66, 0x61, 0x75, 0x6C, 0x74, 0x20,
    0x3D, 0x20, 0x22, 0x31, 0x39, 0x32, 0x2E, 0x31, 0x36, 0x38, 0x2E, 0x31,
    0x2E, 0x37, 0x31, 0x22, 0x0D, 0x0A, 0x50, 0x6F, 0x72, 0x74, 0x20, 0x3D,
    0x20, 0x22, 0x37, 0x33, 0x30, 0x22
};
        public static void CreateConfig()
        {
            Directory.CreateDirectory(@"XDCKIT");
            File.WriteAllBytes(@"XDCKIT/config.toml", ConfigFile);
            ReadConfig();
        }
        public static void ReadConfig()
        {
            // Parse into a node
            using (StreamReader reader = File.OpenText(@"XDCKIT/config.toml"))
            {
                // Parse the table
                TomlTable table = TOML.Parse(reader);
                IP_Range = table["IPConfig"]["IP_Range"];
                Port = int.Parse(table["IPConfig"]["Port"]);
                XboxConsole.DefaultConsole = table["IPConfig"]["Default"];
            }
        }
        private static bool ConfigExistance()
        {
            return !Directory.Exists(@"XDCKIT") | !File.Exists(@"XDCKIT/config.toml");
        }
        #region Connection Types
        public static bool Connect(this XboxConsole Source,out XboxConsole Console,string ConsoleNameOrIP = "jtag")
        {
            Source = Console = new XboxConsole();
            if(XboxConsole.DefaultConsole != string.Empty)
            {
                return Connect(XboxConsole.DefaultConsole);
            }
           else
            {
                return Connect(ConsoleNameOrIP);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Client"></param>
        /// <param name="ConsoleNameOrIP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        public static bool Connect(string ConsoleNameOrIP = "jtag")
        {
            if (string.IsNullOrEmpty(ConsoleNameOrIP))
            {
                ConsoleNameOrIP = "jtag";
            }
            if (ConfigExistance())
            {
                CreateConfig();
            }
            else
            {
                ReadConfig();
            }

            // If user specifies to find their console IP address
            if (ConsoleNameOrIP.Equals("jtag") | ConsoleNameOrIP.Equals(null) | ConsoleNameOrIP.ToCharArray().Any(char.IsLetter))
            {
                if (FindConsole())//if true then continue
                {
                    try
                    {
                        XboxName = new TcpClient(IPAddress, 730);
                        Reader = new StreamReader(XboxName.GetStream());
                        IPAddress = ConsoleNameOrIP;

                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
            // If User Supply's IP To US.
            else if (ConsoleNameOrIP.ToCharArray().Any(char.IsDigit))
            {
                try
                {
                    IPAddress = ConsoleNameOrIP;
                    XboxName = new TcpClient(ConsoleNameOrIP, Port);
                    XboxName.SendTimeout = 100;

                    Reader = new StreamReader(XboxName.GetStream());

                }
                catch (SocketException ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return XboxName.Connected;
        }
        #endregion
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static DoWorkEventHandler FindConsoleFunction()//TODO: Add a Second Search with a bool if connection is true then it will stop both for a faster search
        {
            for (int i = 1; i <= 100; i += 1)
            {
                XboxName = new TcpClient();

                try
                {
                    if (i == 100)
                    {
                        if (!XboxName.Connected)
                        {
                            Console.WriteLine("Slave1 Reached 100 With No IPS Found");
                            Console.WriteLine("Slave1 Is Console Even ON!");
                        }
                        else
                        {
                            return null;
                        }

                    }
                    else
                    {
                        //if connection is false then it will continue 
                        if (!Connected)
                        {
                            if (XboxName.ConnectAsync(IP_Range + i, 730).Wait(20))//keep calm just code..
                            {
                                //if Connection was A Success then it will set the found IP and it will signal the connection was true
                                IPAddress = IP_Range + i;
                                Connected = true;
                                Console.WriteLine("Connected");
                                return null;
                            }
                        }
                    }
                }
                catch
                {

                    return null;
                }
            }

            Connected = false;
            return null;
        }
        public static void FindConsole(uint Retries, uint RetryDelay)
        {

        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static bool FindConsole()
        {
            if (FindConsoleBc.IsBusy == true)
            {
                FindConsolegc.RunWorkerAsync();
            }
            else
            {

                FindConsoleBc.RunWorkerAsync();
                FindConsoleFunction();
            }
            if (XboxName.Connected)
            {
                return true;
            }
            else
            {
                int noOfRetries = 0;

                if (noOfRetries < 3)
                {
                    FindConsoleFunction();
                    noOfRetries++;
                }
                if (noOfRetries < 6)
                {
                    return false;
                }
            }
            return false;
        }


        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Disconnect()
        {
            try
            {
                if (Connected)
                {
                    XboxConsole.SendTextCommand("bye");
                    XboxName.Client.Dispose();
                    IPAddress = "000.000.000.000";
                    XboxName.Close();
                    Connected = false;
                }
            }
            catch
            {

            }
        }

        public static bool Delay(int millisecond)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool flag = false;
            while (!flag)
            {
                if (sw.ElapsedMilliseconds > millisecond)
                {
                    flag = true;
                }
            }
            sw.Stop();
            return true;

        }

        internal static void Reconnect()
        {
            Delay(1000);
            Disconnect();
            if (Connect(IPAddress))
            {
                return;
            }
        }
        #endregion
    }
}
