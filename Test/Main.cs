using System;
using NetIoTester;
using System.Threading;

namespace Test
{
	class MainClass
	{
		public static void OK (ReturnValue rv, string popis)
		{
			if (rv != ReturnValue.OK) {
				Console.WriteLine ("FAIL: " + popis);
			}
		}
		
		public static void Main (string[] args)
		{
			string ipAddress = "192.168.24.120";
			int port = 1234;
			
			NetIO netio = new NetIO (ipAddress, port);
			Console.WriteLine ("Uptime: " + netio.GetUptime ());
			Console.WriteLine ("Login:" + netio.Login ("admin", "admin", true));
			Console.WriteLine ("Version: " + netio.GetVersion ());
			Console.WriteLine ("Alias: " + netio.GetAlias ());
			Console.WriteLine ("WebPort: " + netio.GetWebPort ());
			Console.WriteLine ("KShellPort: " + netio.GetKShellPort ());
			Console.WriteLine ("DNS: " + netio.GetDns ());
			Console.WriteLine ("Ethernet: " + netio.GetEthernet ());
			Console.WriteLine ("Email server: " + netio.GetEmailServer ());
			Console.WriteLine ("SWDelay: " + netio.GetSwDelay ());
			
//			netio.SetPortList(PortStateList.On,PortStateList.On,PortStateList.Off,PortStateList.Off);
//						
//			PortState state;
//			for (int i = 0; i < 30; i++) {
//				for (int j = 1; j < 5; j++) {
//					OK(netio.SetPortState (1, PortState.Interrupt),"Port state");
//					state = netio.GetPortState (j);
//					if (state != PortState.Interrupt) {
//						Console.WriteLine ("Port "+j+", iterace: "+i+" state:"+state);
//					}
//				}
//			}
			
//			OK (netio.SetDiscover (false), "SetDiscover false");
//			if (netio.GetDiscover () != false) {
//				Console.WriteLine ("GetDiscover should be false!");
//			}
//			OK (netio.SetDiscover (true), "SetDiscover true");
//			if (netio.GetDiscover () != true) {
//				Console.WriteLine ("GetDiscover should be true!");
//			}
//			
//			netio.SetPortList (PortStateList.Off, PortStateList.Off, PortStateList.Off, PortStateList.Off);
//			for (int i = 0; i < 2; i++) {
//				for (int j = 1; j < 5; j++) {
//					int k = 1;
//					netio.SetPortState (j, PortState.On);
//					if (netio.GetPortState (j) != PortState.On)
//						Console.WriteLine (i + ": port " + j + " ON: fail");
//					foreach (PortStateList item in netio.GetPortList()) {
//						if (k == j) {
//							if (item != PortStateList.On) {
//								Console.WriteLine (i + " GetPortList: failed on port" + k + " shoud be ON, but: " + item);
//							}		
//						} else {
//							if (item != PortStateList.Off) {
//								Console.WriteLine (i + " GetPortList: failed on port" + k + " shoud be OFF, but: " + item);
//							}		
//						}
//						k++;
//					}
//					netio.SetPortState (j, PortState.Off);
//					if (netio.GetPortState (j) != PortState.Off)
//						Console.WriteLine (i + ": port " + j + " ON: fail");	
//				}
//			}
//			string mailServer = netio.GetEmailServer ();
//			netio.SetEmailServer ("mail.mtrakal.cz");
//			if (netio.GetEmailServer () != "mail.mtrakal.cz") {
//				Console.WriteLine ("Mail server failed" + netio.GetEmailServer ());
//			}
//			netio.SetEmailServer (mailServer);
//			if (netio.GetEmailServer () != mailServer) {
//				Console.WriteLine ("Mail server failed" + netio.GetEmailServer ());
//			}
//			
//			netio.Reboot ();
//			
//			Thread.Sleep (10000);
//			netio.Connect (ipAddress, port);
//			Console.WriteLine ("Uptime: " + netio.GetUptime ());
			
			//Console.WriteLine (netio.SetPortList(PortStateList.On,PortStateList.On,PortStateList.Unchanged,PortStateList.Off));
			Console.ReadLine ();
			netio.Disconnect ();
		}
	}
}
