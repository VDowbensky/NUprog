using System;

namespace HID_SIMPLE.HID
{
	internal static class DESIREDACCESS
	{
		public const uint GENERIC_READ = 2147483648;

		public const uint GENERIC_WRITE = 1073741824;

		public const uint GENERIC_EXECUTE = 536870912;

		public const uint GENERIC_ALL = 268435456;
	}
}