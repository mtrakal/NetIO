using System;
using System.Diagnostics;

namespace NetIoTester
{
	public static class Debug
	{
		public static void WriteLine (Exception ex)
		{
			Debug.WriteLine(ex.GetType().ToString()+"::"+ex.Message);
		}
		public static void WriteLine (string s)
		{
			Debug.WriteLine(s);
		}
	}
}

