using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Timers;

namespace NetIoTester
{
	/// <summary>
	/// Library for controlling NetIO (http://www.koukaam.se/products.php?cat_id=19).
	/// </summary>
	/// <exception cref='Exception'>
	/// Represents errors that occur during application execution.
	/// </exception>
	public class NetIO
	{
		Timer timer = new Timer(25000);
		
		public static string NotLogged = "Not logged!";
		private static string OK = "250";
		private static string dateTimeFormat = "yyyy/MM/dd,HH:mm:ss";
		TelnetConnection tc = null;
		string recievedHash = "";
		string recievedData = "";
		string ipAddress = "";
		int port = 1234;
		public bool Logged {get; private set;}
		
		private ReturnValue returnReturnValue(string s) {
			return (ReturnValue) StringEnum.Parse(typeof(ReturnValue),s);	
		}
		private void timerElapsed(object sender, ElapsedEventArgs e) {
			Noop();
		}
		private void timerRegister(bool register) {
			if(register) {
				timer.Elapsed += new ElapsedEventHandler(timerElapsed);
			} else {
				// zkontrolovat, zda-li funguje
				timer.Elapsed -= new ElapsedEventHandler(timerElapsed);
			}
			timer.Enabled = register;
		}

		
		public NetIO () {}
		public NetIO (string ipAddress):this(ipAddress,1234) {}
		public NetIO (string ipAddress, int port):this() {this.Connect(ipAddress,port);}
		public bool Connect(string ipAddress, int port) {
			this.ipAddress = ipAddress;
			this.port = port;
			
			try
            {
                tc = new TelnetConnection(ipAddress, port);
				recievedData = tc.Read();
				// format: 100 HELLO 4AB59351 - KSHELL V1.3
				if(recievedData.Substring(0,StringEnum.GetStringValue(ReturnValue.HELLO).Length) == StringEnum.GetStringValue(ReturnValue.HELLO)) {
					recievedHash = recievedData.Substring(StringEnum.GetStringValue(ReturnValue.HELLO).Length+1,8);
					timerRegister(true);
					return true;
				} else {
					return false;
				}	
            }
            catch (Exception ex)
            {
				Debug.WriteLine(ex);
                return false;
            }
		}
		private static string byteArrayToString(byte[] ba)
		{
		  StringBuilder hex = new StringBuilder(ba.Length * 2);
		  foreach (byte b in ba)
		    hex.AppendFormat("{0:x2}", b);
		  return hex.ToString();
		}
		private string enableDisable(bool state) {
			return	(state?"enable":"disable");	
		}
		public bool Login(string username, string password, bool crypto) {
			string cryptoString = byteArrayToString(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(username+password+recievedHash)));
			string s = (crypto?"clogin":"login")
				+" "
				+username
				+" "
				+(crypto?cryptoString:password);
			tc.WriteLine(s);
			if((recievedData = tc.Read()) == StringEnum.GetStringValue(ReturnValue.OK)) {
				Logged = true;
				return true;
			} else {
				return false;
			}
		}
		public void Disconnect() {
			Logged = false;
			timerRegister(false);
			tc.WriteLine("quit");
		}
		public string GetVersion() {
			tc.WriteLine("version");
			return tc.Read().Substring(4);
		}
		public string GetAlias() {
			if(Logged) {
				tc.WriteLine("alias");	
				recievedData = tc.Read();
				if(recievedData == StringEnum.GetStringValue(ReturnValue.FORBIDDEN)) {
					return NotLogged;
				}
				return recievedData.Substring(4);
			}
			return NotLogged;
		}
		/// <summary>
		/// After reboot you must create new instance of connection.
		/// </summary>
		public bool Reboot(){
			if(Logged) {
				timerRegister(false);
				tc.WriteLine("system reboot");
				recievedData = tc.Read();
				if(recievedData == StringEnum.GetStringValue(ReturnValue.OK)) {
					Disconnect();
					return true;		
				}
				return false;
			}
			return false;
		}
		public void Noop() {
			tc.WriteLine("noop");
		}
		public string SendCommand(string command){
			tc.WriteLine(command);
			return tc.Read(); 
		}
		public string GetUptime() {
			tc.WriteLine("uptime");
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4);
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		public ReturnValue SetPortState(int portNumber, PortState state) {
			tc.WriteLine("port "+portNumber+" "+ StringEnum.GetStringValue(state));
			return returnReturnValue(tc.Read());
		}
		public PortState GetPortState(int port) {
			tc.WriteLine("port "+port);
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return (PortState)StringEnum.Parse(typeof(PortState),recievedData.Substring(4,1));
			}
			return PortState.Manual;
		}
		public ReturnValue SetPortList(PortStateList port1, PortStateList port2, PortStateList port3, PortStateList port4) {
			tc.WriteLine("port list "+StringEnum.GetStringValue(port1)+StringEnum.GetStringValue(port2)+StringEnum.GetStringValue(port3)+StringEnum.GetStringValue(port4));
			return returnReturnValue(tc.Read());
		}
		public List<PortState> GetPortList() {
			List<PortState> portList = new List<PortState>();
			tc.WriteLine("port list");
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				portList.Add((PortState)StringEnum.Parse(typeof(PortState),recievedData.Substring(4,1)));
				portList.Add((PortState)StringEnum.Parse(typeof(PortState),recievedData.Substring(5,1)));
				portList.Add((PortState)StringEnum.Parse(typeof(PortState),recievedData.Substring(6,1)));
				portList.Add((PortState)StringEnum.Parse(typeof(PortState),recievedData.Substring(7,1)));
			}
			return portList;
		}
		public ReturnValue SetPortSetup(int outputNumber, string outputName, bool mode, int interruptDelay, bool powerOnStatus) {
			tc.WriteLine("port setup "+outputNumber+" "+"'"+outputName+"'"+" "+(mode?"manual":"timer")+" "+interruptDelay+" "+powerOnStatus);
			return returnReturnValue(tc.Read());
		}
		public ReturnValue SetPortTimer(int outputNumer, string timeFormat, TimeMode mode, string startTime, string endTime, string weekSchedule) {
			tc.WriteLine("port timer "+outputNumer+" "+timeFormat+" "+mode.ToString().ToLower()+" "+startTime+" "+endTime+" "+weekSchedule);
			return returnReturnValue(tc.Read());
		}
		
		public string GetWatchDog(int portNumber) {
			tc.WriteLine("port wd "+portNumber);
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4);
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		public ReturnValue SetWatchDog(int portNumber, bool state) {
			tc.WriteLine("port wd "+portNumber+" "+enableDisable(state));
			return returnReturnValue(tc.Read());
		}
		public ReturnValue SetWatchDog(int portNumber, bool state, string wdIPaddress, int wdTimeout, int wdPowerOnDelay, int pingInterval, int maxRetry, bool maxRetryEnable, bool sendEmail) {
			tc.WriteLine("port wd "+portNumber+" "+enableDisable(state)+" "+wdIPaddress+" "+wdTimeout+" "+wdPowerOnDelay+" "+pingInterval+" "+maxRetry+" "+enableDisable(maxRetryEnable)+" "+enableDisable( sendEmail));
			return returnReturnValue(tc.Read());
		}
		public string GetEthernet() {
			tc.WriteLine("system eth");
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4);
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		/// <summary>
		/// After using this command you must create new instance of connection.
		/// </summary>
		/// <returns>
		/// The ethernet DHC.
		/// </returns>
		public ReturnValue SetEthernetDHCP(){
			tc.WriteLine("system eth dhcp");
			recievedData = tc.Read();
			if(Reboot()) {
				return returnReturnValue(recievedData);
			} else {
				return ReturnValue.FORBIDDEN;
			}
		}
		/// <summary>
		/// After using this command you must create new instance of connection.
		/// </summary>
		/// <returns>
		/// The ethernet.
		/// </returns>
		/// <param name='ipAddress'>
		/// Ip address.
		/// </param>
		/// <param name='netmask'>
		/// Netmask.
		/// </param>
		/// <param name='gateway'>
		/// Gateway.
		/// </param>
		public ReturnValue SetEthernet(string ipAddress,string netmask, string gateway){
			tc.WriteLine(string.Format("system eth manual {0} {1} [2}",ipAddress,netmask,gateway));
			recievedData = tc.Read();
			if(Reboot()) {
				return returnReturnValue(recievedData);
			} else {
				return ReturnValue.FORBIDDEN;
			}
		}
		public string GetEmailServer() {
			tc.WriteLine("email server");
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4).Trim();
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		public ReturnValue SetEmailServer(string server) {
			tc.WriteLine("email server "+server);
			return returnReturnValue(tc.Read());
		}
		public bool GetDiscover() {
			tc.WriteLine("system discover");
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				 return (recievedData.Substring(4).Trim()=="disable"?false:true);
			}
			throw new Exception("Error in recieved data.");
		}
		public ReturnValue SetDiscover(bool enable) {
			tc.WriteLine("system discover "+(enableDisable(enable)));
			return returnReturnValue(tc.Read());
		}
		public ReturnValue SetSwDelay(string tenthOfSeconds) {
			tc.WriteLine("system swdelay "+tenthOfSeconds);
			return returnReturnValue(tc.Read());
		}
		public string GetSwDelay() {
			tc.WriteLine("system swdelay");
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4);
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		/// <summary>
		/// After using this command you must create new instance of connection.
		/// </summary>
		/// <returns>
		/// The dns.
		/// </returns>
		/// <param name='ipAddress'>
		/// Ip address.
		/// </param>
		public ReturnValue SetDns(string ipAddress) {
			tc.WriteLine("system dns "+ipAddress);
			recievedData = tc.Read();
			Reboot();
			return returnReturnValue(recievedData);
		}
		public string GetDns() {
			tc.WriteLine("system dns");
				recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4);
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		public ReturnValue SetSntp(bool enable,string sntpServer) {
			tc.WriteLine("system sntp "+(enableDisable(enable))+" "+sntpServer);
			return returnReturnValue(tc.Read());
		}
		public string GetSntp() {
			tc.WriteLine("system sntp");
					recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4);
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		public ReturnValue SetTime(DateTime dateTime) {
			tc.WriteLine(String.Format("system time {0:"+dateTimeFormat+"}", dateTime));
			return returnReturnValue(tc.Read());
		}
		public DateTime GetTime() {
			tc.WriteLine("system time");
			DateTime dt;
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				dt = DateTime.ParseExact(recievedData.Substring(4),dateTimeFormat,null);
			}
			return dt;
		}
		public ReturnValue SetTimezone(int offset) {
			tc.WriteLine("system timezone" + offset);
			return returnReturnValue(tc.Read());
		}
		/// <summary>
		/// Gets the timezone.
		/// </summary>
		/// <returns>
		/// The timezone offset in seconds. If failed, return int.MinValue.
		/// </returns>
		public int GetTimezone() {
			tc.WriteLine("system timezone");
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return int.Parse(recievedData.Substring(4));
			} else {
				return int.MinValue;
			}
		}
		public void FactoryReset() {
			tc.WriteLine("system reset");
		}
		public ReturnValue SetWebPort(int port) {
			tc.WriteLine("system webport "+port);
			recievedData = tc.Read();//.Substring(4);
			//Reboot();
			return returnReturnValue(recievedData);
		}
		public string GetWebPort() {
			tc.WriteLine("system webport");
						recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4);
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		public ReturnValue SetKShellPort(int port) {
			this.port = port;
			tc.WriteLine("system kshport "+port);
			recievedData = tc.Read();
			Disconnect();
			tc = new TelnetConnection(ipAddress,this.port);
			return returnReturnValue(recievedData);
		}
		public string GetKShellPort() {
			tc.WriteLine("system kshport");
							recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4);
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		public ReturnValue SetDhcp(string hostname, bool enable) {
			tc.WriteLine("system dhcp "+hostname+" "+enableDisable(enable));
			recievedData = tc.Read();
			return returnReturnValue(recievedData);
		}
		public string GetDhcp() {
			tc.WriteLine("system dhcp");
			recievedData = tc.Read();
			if(recievedData.Substring(0,3) == OK) {
				return recievedData.Substring(4);
			} else { 
				return "FAILED: "+recievedData;
			}
		}
		public ReturnValue SetDhcpSntp(bool enable) {
			tc.WriteLine("system dhcp sntp "+enable);
			recievedData = tc.Read();
			return returnReturnValue(recievedData);
		}
		// TODO: uživatelé
	}
}
