using System;

namespace HID_SIMPLE.HID
{
	internal static class FLAGSANDATTRIBUTES
	{
		public const uint FILE_FLAG_WRITE_THROUGH = 2147483648;

		public const uint FILE_FLAG_OVERLAPPED = 1073741824;

		public const uint FILE_FLAG_NO_BUFFERING = 536870912;

		public const uint FILE_FLAG_RANDOM_ACCESS = 268435456;

		public const uint FILE_FLAG_SEQUENTIAL_SCAN = 134217728;

		public const uint FILE_FLAG_DELETE_ON_CLOSE = 67108864;

		public const uint FILE_FLAG_BACKUP_SEMANTICS = 33554432;

		public const uint FILE_FLAG_POSIX_SEMANTICS = 16777216;

		public const uint FILE_FLAG_OPEN_REPARSE_POINT = 2097152;

		public const uint FILE_FLAG_OPEN_NO_RECALL = 1048576;

		public const uint FILE_FLAG_FIRST_PIPE_INSTANCE = 524288;
	}
}