using System;

namespace NetIoTester
{
	/***
	 * MessageBox.Show(StringEnum.GetStringValue(CarType.Estate));
     * The line above shows 'Estate / Wagon'

    * MessageBox.Show(StringEnum.Parse(typeof(CarType), 
                     "estate / wagon", true).ToString());
    *The line above converts back to an enum 
    *value from String Value (case insensitive)
    *and shows 'Estate'

    * int enumValue = (int)StringEnum.Parse(typeof(CarType), 
                                     "estate / wagon", true);
    * MessageBox.Show(enumValue.ToString());
    * The line above does the same again but this time shows 
    * the numeric value of the enum (6) 
	 */
	public enum ReturnValue
	{
		 [StringValue("100 HELLO")] HELLO,
		 [StringValue("120 Rebooting...")] REBOOTING,
		 [StringValue("130 CONNECTION TIMEOUT")] CONNECTION_TIMEOUT,
		 [StringValue("250 OK")] OK,
		 [StringValue("500 INVALID VALUE")] INVALID_VALUE,
		 [StringValue("501 INVALID PARAMETR")] INVALID_PARAMETER,
		 [StringValue("502 UNKNOWN COMMAND")] UNKNOWN_COMMAND,
		 [StringValue("503 INVALID LOGIN")] INVALID_LOGIN,
		 [StringValue("504 ALREADY LOGGED IN")] ALREADY_LOGGED_IN,
		 [StringValue("505 FORBIDDEN")] FORBIDDEN,
		 [StringValue("506 INPUT LINE TOO LONG")] INPUT_LINE_TOO_LONG,
		 [StringValue("507 TOO MANY CONNECTIONS")] TOO_MANY_CONNECTIONS
	}
}

