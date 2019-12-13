using System;

namespace HID_SIMPLE.HID
{
	public class ClassDataProcess
	{
		private HIDInterface hid = new HIDInterface();

		private ClassDataProcess.connectStatusStruct connectStatus = new ClassDataProcess.connectStatusStruct();

		public ClassDataProcess.isConnectedDelegate isConnectedFunc;

		public ClassDataProcess.PushReceiveDataDele pushReceiveData;

		public ClassDataProcess()
		{
		}

		public void HID_Close()
		{
			this.hid.StopAutoConnect();
		}

		public void HID_DataReceived(object sender, byte[] e)
		{
			if (this.pushReceiveData != null)
			{
				this.pushReceiveData(e);
			}
		}

		public void HID_Initial()
		{
			this.hid.StatusConnected = new HIDInterface.DelegateStatusConnected(this.HID_StatusConnected);
			this.hid.DataReceived = new HIDInterface.DelegateDataReceived(this.HID_DataReceived);
			HIDInterface.HidDevice hidDevice = new HIDInterface.HidDevice()
			{
				vID = 26214,
				pID = 9011,
				serial = ""
			};
			this.hid.AutoConnect(hidDevice);
		}

		public bool HID_SendBytes(byte[] data)
		{
			return this.hid.Send(data);
		}

		public void HID_StatusConnected(object sender, bool isConnect)
		{
			this.connectStatus.curStatus = isConnect;
			if (this.connectStatus.curStatus != this.connectStatus.preStatus)
			{
				this.connectStatus.preStatus = this.connectStatus.curStatus;
				if (!this.connectStatus.curStatus)
				{
					this.isConnectedFunc(false);
				}
				else
				{
					this.isConnectedFunc(true);
				}
			}
		}

		private struct connectStatusStruct
		{
			public bool preStatus;

			public bool curStatus;
		}

		public delegate void isConnectedDelegate(bool isConnected);

		public delegate void PushReceiveDataDele(byte[] datas);
	}
}