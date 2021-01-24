﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XDevkit;

namespace XDevkit
{
    public static class XboxClient
    {
        #region Property's
        public static TcpClient XboxName { get; set; } = new TcpClient();
        public static bool Connected { get; set; }
        private static Xbox xboxConsole { get; set; } = new Xbox();
        public static int Port { get; set; } = 730;
        public static string IPAddress { get; set; } = "000.000.000.000";
        [Browsable(false)]
        public static StreamReader Reader; 
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
            if (ConsoleNameOrIP == "default")
            {
                XboxName = new TcpClient();
                if (FindConsole())//if true then continue
                {
                    XboxName = new TcpClient(IPAddress, 730);
                    Reader = new StreamReader(XboxName.GetStream());
                    Console.WriteLine("/Connection - F01/....(" + IPAddress + ")");//debugging purposes..
                    // set class properties once connected 
                    xboxConsole.IPAddress = ConsoleNameOrIP;
                    IPAddress = ConsoleNameOrIP;
                    XboxClient.XboxName = XboxName;
                    return Connected = true;
                }
                else// if top fails
                {
                    return Connected = false;
                }
            }
            //User Enter's Nothing
            else if (ConsoleNameOrIP == "")
            {
                XboxName = new TcpClient();
                if (FindConsole())//if true then continue
                {
                    XboxName = new TcpClient(IPAddress, 730);//test...
                    Reader = new StreamReader(XboxName.GetStream());
                    Console.WriteLine("/Connection - F01/....(" + IPAddress + ")");//debugging purposes..
                    // set class properties once connected 
                    xboxConsole.IPAddress = ConsoleNameOrIP;
                    IPAddress = ConsoleNameOrIP;
                    XboxClient.XboxName = XboxName;
                    return Connected = true;
                }
                else// if top fails
                {
                    return Connected = false;
                }
            }
            // If User Supply's IP To US.
            else if (ConsoleNameOrIP.ToCharArray().Any(char.IsDigit))
            {
                IPAddress = ConsoleNameOrIP;
                xboxConsole.IPAddress = IPAddress;
                XboxName = new TcpClient(ConsoleNameOrIP, Port);
                Reader = new StreamReader(XboxName.GetStream());
                Console.WriteLine("/Connection - Degits/....(" + "Manual Connection Mode" + ")");
                return Connected = true;
            }
            //Get IP Via Name
            else if (ConsoleNameOrIP.ToCharArray().Any(char.IsLetter))//uses ip to find console makes user think it finds it via name 
            {
                Connected = FindConsole();
                xboxConsole.IPAddress = IPAddress;
                XboxName = new TcpClient(IPAddress, 730);//test...
                Reader = new StreamReader(XboxName.GetStream());
                Console.WriteLine("/Connection - F01/....(" + IPAddress + ")");//debugging purposes..
                return Connected;
            }
            else
            {
                return Connected;
            }
        }
        static DoWorkEventHandler BackgroundSlave()
        {
            string ips = "192.168.0.";
            while (true)
            {
                int i = 0;

                for (; ; )
                {
                    if (i < 255)
                    {
                        XboxName = new TcpClient();
                        if (XboxName.ConnectAsync(ips + i, 730).Wait(10))//keep calm just code..
                        {
                            xboxConsole.IPAddress = ips + i;
                            Connected = true;
                            return null;
                        }
                        else
                        {
                            i++;
                        }

                    }
                    else
                    {
                        Connected = false;
                        return null;
                    }
                }
            }
        }
        static readonly BackgroundWorker FindConsoleBc = new BackgroundWorker();
        static readonly BackgroundWorker FindConsolegc = new BackgroundWorker();
        public static void CloseConnection(uint Connection)
        {

            Disconnect();
        }
        public static void FindConsole(uint Retries, uint RetryDelay)
        {

        }

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
            int n = 0;
            switch (n)
            {
                case 0:
                    BackgroundSlave();
                    goto case 1;
                case 1:
                    if (Connected == true)
                    {
                        return true;
                    }
                    else
                    {
                        if (n < 3)
                        {
                            Console.WriteLine("Connection Fail Safe Activated....");
                            n++;
                            BackgroundSlave();
                        }
                        if (n < 6)
                        {
                            Console.WriteLine("Connection Must Not Be Available, Please Make Sure Your On the Same Network As Your Console Otherwise Please Try Again Later.....");
                            Console.WriteLine("Connection Fail Safe Terminated, Reason: FindConsole Has Failed User Not Connected To Network....");

                            return false;
                        }
                        n++;
                        goto case 1;
                    }
            }
            return false;
        }

        public static void Disconnect()
        {
            try
            {
                if (Connected)
                {
                    Xbox.SendTextCommand("bye");
                    XboxName.Client.Dispose();
                    IPAddress = "000.000.000.000";
                    xboxConsole.IPAddress = "000.000.000.000";
                    XboxName.Close();
                    Connected = false;
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        static bool FindConsole(int retryAttepts)
        {
            return DoWithRetry(FindConsole(), TimeSpan.FromSeconds(5), 3);
        }

        static bool DoWithRetry(bool action, TimeSpan sleepPeriod, int tryCount = 3)
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
        public static string TranslateError(int code)
        {
            string str;
            int num = code;
            switch (num)
            {
                case 200:
                    str = "200- OK (Standard response for successful execution of a command.)";
                    break;

                case 0xc9:
                    str = "201- connected (Initial response sent after a connection is established. The client does not need to send anything to solicit this response.)";
                    break;

                case 0xca:
                    str = "202- multiline response follows - The response line is followed by one or more additional lines of data terminated by a line containing only a . (period).The client must read all available lines before sending another command.)";
                    break;

                case 0xcb:
                    str = "203- binary response follows (The response line is followed by raw binary data, the length of which is indicated in some command-specific way.The client must read all available data before sending another command.)";
                    break;

                case 0xcc:
                    str = "204- send binary data (The command is expecting additional binary data from the client.After the client sends the required number of bytes, XBDM will send another response line with the final result of the command.)";
                    break;

                case 0xcd:
                    str = "205- connection dedicated (The connection has been moved to a dedicated handler thread).";
                    break;

                default:
                    switch (num)
                    {
                        case 400:
                            str = "400- unexpected error = An internal error occurred that could not be translated to a standard error code.The message is typically more descriptive, such as 'out of memory' or 'bad parameter'.";
                            break;

                        case 0x191:
                            str = "401- max number of connections exceeded = The connection could not be established because XBDM is already serving the maximum number of clients(4).";
                            break;

                        case 0x192:
                            str = "402- file not found = An operation was attempted on a file that does not exist.";
                            break;

                        case 0x193:
                            str = "403- no such module = An operation was attempted on a module that does not exist.";
                            break;

                        case 0x194:
                            str = "404- memory not mapped = An operation was attempted on a region of memory that is not mapped in the page table.";
                            break;

                        case 0x195:
                            str = "405- no such thread = An operation was attempted on a thread that does not exist.";
                            break;

                        case 0x196:
                            str = "406- = An attempt to set the system time with the setsystime command failed. This status code is undocumented.";
                            break;

                        case 0x197:
                            str = "407- unknown command = The command is not recognized.";
                            break;

                        case 0x198:
                            str = "408- not stopped = The target thread is not stopped.";
                            break;

                        case 0x199:
                            str = "409- file must be copied = A move operation was attempted on a file that can only be copied.";
                            break;

                        case 410:
                            str = "410- file already exists = A file could not be created or moved because one already exists with the same name.";
                            break;

                        case 0x19b:
                            str = "411- directory not empty = A directory could not be deleted because it still contains files and/or directories.";
                            break;

                        case 0x19c:
                            str = "412- filename is invalid = The specified file contains invalid characters or is too long.";
                            break;

                        case 0x19d:
                            str = "413- file cannot be created = The file cannot be created for some unspecified reason.";
                            break;

                        case 0x19e:
                            str = "414- access denied = The file cannot be accessed at the connection's current privilege level (see #Security).";
                            break;

                        case 0x19f:
                            str = "415- no room on device = The target device has run out of storage space.";
                            break;

                        case 0x1a0:
                            str = "416- not debuggable = The title is not debuggable.";
                            break;

                        case 0x1a1:
                            str = "417- type invalid = The performance counter type is invalid.";
                            break;

                        case 0x1a2:
                            str = "418- data not available = The performance counter data is not available.";
                            break;

                        case 420:
                            str = "420- box not locked = The command can only be executed when security is enabled (see #Security).";
                            break;

                        case 0x1a5:
                            str = "421- key exchange required = The client must perform a key exchange with the keyxchg command (see #Security).";
                            break;

                        case 0x1a6:
                            str = "422- dedicated connection required = The command can only be executed on a dedicated connection (see #Connection dedication).";
                            break;

                        default:
                            str = "Response code you entered is either invalid or there isn't any information for it.";
                            break;
                    }
                    break;
            }
            return str;
        }
        #endregion
    }
}