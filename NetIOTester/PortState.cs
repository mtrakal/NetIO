using System;

namespace NetIoTester
{
	public enum PortState
	{
		[StringValue("1")] On,
		[StringValue("0")] Off,
		[StringValue("manual")] Manual,
		[StringValue("int")] Interrupt
	}
}

