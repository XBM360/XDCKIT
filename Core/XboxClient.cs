﻿//Do Not Delete This Comment... 
//Made By TeddyHammer on 08/20/16
//Any Code Copied Must Source This Project (its the law (:P)) Please.. i work hard on it 3 years and counting...
//Thank You for looking love you guys...

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace XDevkit
{

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class XboxClient
    {
        #region Property's
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TcpClient XboxName { get; set; }
        public static bool Connected = false;
        private static Xbox xboxConsole { get; set; } = new Xbox();
        public static int Port 
        {
            get => 730;
            set
            {
                Port = value;
            }
        }
        public static string IPAddress { get; set; } = "000.000.000.000";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="console"></param>
        /// <param name="XConsole"></param>
        /// <param name="ConsoleNameOrIP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        public static bool Connect(this Xbox console, out Xbox XConsole, string ConsoleNameOrIP = "default", int Port = 730)
        {
            XConsole = xboxConsole;//sets Class For Client

            // If user specifies to find their console IP address
            if (ConsoleNameOrIP.Equals("default")| ConsoleNameOrIP.Equals(string.Empty) | ConsoleNameOrIP.ToCharArray().Any(char.IsLetter))
            {
                if (FindConsole())//if true then continue
                {
                    XboxName = new TcpClient(IPAddress, 730);
                    Reader = new StreamReader(XboxName.GetStream());
                    IPAddress = ConsoleNameOrIP;
                    Connected = true;
                }
            }
            // If User Supply's IP To US.
            else if (ConsoleNameOrIP.ToCharArray().Any(char.IsDigit))
            {
                try
                {
                    IPAddress = ConsoleNameOrIP;
                    XboxName = new TcpClient(ConsoleNameOrIP, Port);
                    Reader = new StreamReader(XboxName.GetStream());
                    Connected = true;
                }
                catch (SocketException)
                {
                  
                }
            }

            return Connected;
        }
        public static bool Connect(string ConsoleNameOrIP = "default", int Port = 730)
        {
            // If user specifies to find their console IP address
            if (ConsoleNameOrIP.Equals("default") | ConsoleNameOrIP.Equals(string.Empty) | ConsoleNameOrIP.ToCharArray().Any(char.IsLetter))
            {
                if (FindConsole())//if true then continue
                {
                    XboxName = new TcpClient(IPAddress, 730);
                    Reader = new StreamReader(XboxName.GetStream());
                    IPAddress = ConsoleNameOrIP;
                    Connected = true;
                }
            }
            // If User Supply's IP To US.
            else if (ConsoleNameOrIP.ToCharArray().Any(char.IsDigit))
            {
                try
                {
                    IPAddress = ConsoleNameOrIP;
                    XboxName = new TcpClient(ConsoleNameOrIP, Port);
                    Reader = new StreamReader(XboxName.GetStream());
                    Connected = true;
                }
                catch (SocketException)
                {

                }
            }

            return Connected;
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static DoWorkEventHandler BackgroundSlave()
        {
            string ips = "192.168.0.";

            for (int i = 0; i <= 255; i += 1)
            {
                XboxName = new TcpClient();

                try
                {
                    if (XboxName.ConnectAsync(ips + i, 730).Wait(10))//keep calm just code..
                    {
                        IPAddress = ips + i;
                        Connected = true;
                        return null;
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

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void CloseConnection(uint Connection)
        {
            Disconnect();
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
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
            }

            BackgroundSlave();

            if (Connected == true)
            {
                return true;
            }
            else
            {
                int noOfRetries = 0;

                if (noOfRetries < 3)
                {
                    BackgroundSlave();
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
                    Xbox.SendTextCommand("bye");
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
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool FindConsole(int retryAttepts)
        {
            return DoWithRetry(FindConsole(), TimeSpan.FromSeconds(5), 3);
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool DoWithRetry(bool action, TimeSpan sleepPeriod, int tryCount = 3)
        {
            if (tryCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(tryCount));

            while (action == false)
            {
                try
                {
                    if (action)
                    {
                        return true;
                    }
                    break; // success!
                    //else retrun false fixes issue

                }
                catch
                {
                    if (--tryCount == 0)
                        throw;
                    Thread.Sleep(sleepPeriod);

                }

            }
            return false;
        }


        internal static void Reconnect()
        {
            Disconnect();
            if (Connect(IPAddress, Port))
            {
                return;
            }
        }
        #endregion
    }
}
