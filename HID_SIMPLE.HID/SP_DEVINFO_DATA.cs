using System;
using System.Runtime.InteropServices;

namespace HID_SIMPLE.HID
{
	public class SP_DEVINFO_DATA
	{
		public int cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));

		public Guid classGuid = Guid.Empty;

		public int devInst = 0;

		public int reserved = 0;

		public SP_DEVINFO_DATA()
		{
		}
	}
}