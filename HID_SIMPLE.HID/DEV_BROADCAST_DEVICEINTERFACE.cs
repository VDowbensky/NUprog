using System;

namespace HID_SIMPLE.HID
{
	public struct DEV_BROADCAST_DEVICEINTERFACE
	{
		public int dbcc_size;

		public int dbcc_devicetype;

		public int dbcc_reserved;

		public Guid dbcc_classguid;

		public string dbcc_name;
	}
}