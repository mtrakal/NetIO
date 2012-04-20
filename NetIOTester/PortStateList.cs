using System;

namespace NetIoTester
{
	public enum PortStateList
	{
		[StringValue("1")] On,
		[StringValue("0")] Off,
		[StringValue("u")] Unchanged,
		[StringValue("i")] Interrupt
	}
}
