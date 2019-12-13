using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HID_SIMPLE.HID
{
	public class Hid
	{
		private IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

		private const int MAX_USB_DEVICES = 64;

		private bool deviceOpened = false;

		private FileStream hidDevice = null;

		private IntPtr hHubDevice;

		private int outputReportLength;

		private int inputReportLength;

		public Hid.DelegateDataReceived DataReceived;

		public Hid.DelegateStatusConnected DeviceRemoved;

		public int InputReportLength
		{
			get
			{
				return this.inputReportLength;
			}
		}

		public int OutputReportLength
		{
			get
			{
				return this.outputReportLength;
			}
		}

		public Hid()
		{
		}

		private void BeginAsyncRead()
		{
			byte[] numArray = new byte[this.InputReportLength];
			this.hidDevice.BeginRead(numArray, 0, this.InputReportLength, new AsyncCallback(this.ReadCompleted), numArray);
		}

		public void CloseDevice()
		{
			if (this.deviceOpened)
			{
				this.deviceOpened = false;
				this.hidDevice.Close();
			}
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr CreateFile(string fileName, uint desiredAccess, uint shareMode, uint securityAttributes, uint creationDisposition, uint flagsAndAttributes, uint templateFile);

		public static void GetHidDeviceList(ref List<string> deviceList)
		{
			Guid empty = Guid.Empty;
			uint i = 0;
			deviceList.Clear();
			Hid.HidD_GetHidGuid(ref empty);
			IntPtr intPtr = Hid.SetupDiGetClassDevs(ref empty, 0, IntPtr.Zero, DIGCF.DIGCF_PRESENT | DIGCF.DIGCF_DEVICEINTERFACE);
			if (intPtr != IntPtr.Zero)
			{
				SP_DEVICE_INTERFACE_DATA sPDEVICEINTERFACEDATum = new SP_DEVICE_INTERFACE_DATA()
				{
					cbSize = Marshal.SizeOf(sPDEVICEINTERFACEDATum)
				};
				for (i = 0; i < 64; i++)
				{
					if (Hid.SetupDiEnumDeviceInterfaces(intPtr, IntPtr.Zero, ref empty, i, ref sPDEVICEINTERFACEDATum))
					{
						int num = 0;
						Hid.SetupDiGetDeviceInterfaceDetail(intPtr, ref sPDEVICEINTERFACEDATum, IntPtr.Zero, num, ref num, null);
						IntPtr intPtr1 = Marshal.AllocHGlobal(num);
						SP_DEVICE_INTERFACE_DETAIL_DATA sPDEVICEINTERFACEDETAILDATum = new SP_DEVICE_INTERFACE_DETAIL_DATA()
						{
							cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DETAIL_DATA))
						};
						Marshal.StructureToPtr(sPDEVICEINTERFACEDETAILDATum, intPtr1, false);
						if (Hid.SetupDiGetDeviceInterfaceDetail(intPtr, ref sPDEVICEINTERFACEDATum, intPtr1, num, ref num, null))
						{
							deviceList.Add(Marshal.PtrToStringAuto((IntPtr)((int)intPtr1 + 4)));
						}
						Marshal.FreeHGlobal(intPtr1);
					}
				}
			}
			Hid.SetupDiDestroyDeviceInfoList(intPtr);
		}

		[DllImport("hid.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool HidD_FreePreparsedData(IntPtr PreparsedData);

		[DllImport("hid.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool HidD_GetAttributes(IntPtr hidDeviceObject, out HIDD_ATTRIBUTES attributes);

		[DllImport("hid.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern void HidD_GetHidGuid(ref Guid HidGuid);

		[DllImport("hid.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool HidD_GetPreparsedData(IntPtr hidDeviceObject, out IntPtr PreparsedData);

		[DllImport("hid.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool HidD_GetSerialNumberString(IntPtr hidDeviceObject, IntPtr buffer, int bufferLength);

		[DllImport("hid.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern uint HidP_GetCaps(IntPtr PreparsedData, out HIDP_CAPS Capabilities);

		protected virtual void OnDataReceived(report e)
		{
			if (this.DataReceived != null)
			{
				this.DataReceived(this, e);
			}
		}

		protected virtual void OnDeviceRemoved(EventArgs e)
		{
			if (this.DeviceRemoved != null)
			{
				this.DeviceRemoved(this, e);
			}
		}

		public Hid.HID_RETURN OpenDevice(ushort vID, ushort pID, string serial)
		{
			Hid.HID_RETURN hIDRETURN;
			HIDD_ATTRIBUTES hIDDATTRIBUTE;
			IntPtr intPtr;
			HIDP_CAPS hIDPCAP;
			if (this.deviceOpened)
			{
				hIDRETURN = Hid.HID_RETURN.DEVICE_OPENED;
			}
			else
			{
				List<string> strs = new List<string>();
				Hid.GetHidDeviceList(ref strs);
				if (strs.Count != 0)
				{
					for (int i = 0; i < strs.Count; i++)
					{
						IntPtr intPtr1 = Hid.CreateFile(strs[i], -1073741824, 0, 0, 3, 1073741824, 0);
						if (intPtr1 != this.INVALID_HANDLE_VALUE)
						{
							IntPtr intPtr2 = Marshal.AllocHGlobal(512);
							Hid.HidD_GetAttributes(intPtr1, out hIDDATTRIBUTE);
							Hid.HidD_GetSerialNumberString(intPtr1, intPtr2, 512);
							string stringAuto = Marshal.PtrToStringAuto(intPtr2);
							Marshal.FreeHGlobal(intPtr2);
							if ((hIDDATTRIBUTE.VendorID != vID || hIDDATTRIBUTE.ProductID != pID ? false : stringAuto.Contains(serial)))
							{
								Hid.HidD_GetPreparsedData(intPtr1, out intPtr);
								Hid.HidP_GetCaps(intPtr, out hIDPCAP);
								Hid.HidD_FreePreparsedData(intPtr);
								this.outputReportLength = hIDPCAP.OutputReportByteLength;
								this.inputReportLength = hIDPCAP.InputReportByteLength;
								this.hidDevice = new FileStream(new SafeFileHandle(intPtr1, false), FileAccess.ReadWrite, this.inputReportLength, true);
								this.deviceOpened = true;
								this.BeginAsyncRead();
								this.hHubDevice = intPtr1;
								hIDRETURN = Hid.HID_RETURN.SUCCESS;
								return hIDRETURN;
							}
						}
					}
					hIDRETURN = Hid.HID_RETURN.DEVICE_NOT_FIND;
				}
				else
				{
					hIDRETURN = Hid.HID_RETURN.NO_DEVICE_CONECTED;
				}
			}
			return hIDRETURN;
		}

		private void ReadCompleted(IAsyncResult iResult)
		{
			byte[] asyncState = (byte[])iResult.AsyncState;
			try
			{
				this.hidDevice.EndRead(iResult);
				byte[] numArray = new byte[(int)asyncState.Length - 1];
				for (int i = 1; i < (int)asyncState.Length; i++)
				{
					numArray[i - 1] = asyncState[i];
				}
				this.OnDataReceived(new report(asyncState[0], numArray));
				if (this.deviceOpened)
				{
					this.BeginAsyncRead();
				}
			}
			catch
			{
				this.OnDeviceRemoved(new EventArgs());
				this.CloseDevice();
			}
		}

		[DllImport("Kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool ReadFile(IntPtr file, byte[] buffer, uint numberOfBytesToRead, out uint numberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

		[DllImport("setupapi.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

		[DllImport("setupapi.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

		[DllImport("setupapi.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, uint Enumerator, IntPtr HwndParent, DIGCF Flags);

		[DllImport("setupapi.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, SP_DEVINFO_DATA deviceInfoData);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool UnregisterDeviceNotification(IntPtr handle);

		public Hid.HID_RETURN Write(report r)
		{
			Hid.HID_RETURN hIDRETURN;
			if (!this.deviceOpened)
			{
				hIDRETURN = Hid.HID_RETURN.WRITE_FAILD;
			}
			else
			{
				try
				{
					byte[] numArray = new byte[this.outputReportLength];
					numArray[0] = r.reportID;
					int num = 0;
					num = ((int)r.reportBuff.Length >= this.outputReportLength - 1 ? this.outputReportLength - 1 : (int)r.reportBuff.Length);
					for (int i = 0; i < num; i++)
					{
						numArray[i + 1] = r.reportBuff[i];
					}
					this.hidDevice.Write(numArray, 0, this.OutputReportLength);
					hIDRETURN = Hid.HID_RETURN.SUCCESS;
				}
				catch
				{
					this.OnDeviceRemoved(new EventArgs());
					this.CloseDevice();
					hIDRETURN = Hid.HID_RETURN.NO_DEVICE_CONECTED;
				}
			}
			return hIDRETURN;
		}

		[DllImport("Kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool WriteFile(IntPtr file, byte[] buffer, uint numberOfBytesToWrite, out uint numberOfBytesWritten, IntPtr lpOverlapped);

		public delegate void DelegateDataReceived(object sender, report e);

		public delegate void DelegateStatusConnected(object sender, EventArgs e);

		public enum HID_RETURN
		{
			SUCCESS,
			NO_DEVICE_CONECTED,
			DEVICE_NOT_FIND,
			DEVICE_OPENED,
			WRITE_FAILD,
			READ_FAILD
		}
	}
}