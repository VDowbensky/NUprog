using System;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace HID_SIMPLE.HID
{
	public class HIDInterface : IDisposable
	{
		private HIDInterface.HidDevice lowHidDevice = new HIDInterface.HidDevice();

		public HIDInterface.DelegateDataReceived DataReceived;

		public HIDInterface.DelegateStatusConnected StatusConnected;

		public bool bConnected = false;

		public Hid oSp = new Hid();

		private static HIDInterface m_oInstance;

		private bool ContinueConnectFlag = true;

		private BackgroundWorker ReadWriteThread = new BackgroundWorker();

		public HIDInterface()
		{
			HIDInterface.m_oInstance = this;
			this.oSp.DataReceived = new Hid.DelegateDataReceived(this.HidDataReceived);
			this.oSp.DeviceRemoved = new Hid.DelegateStatusConnected(this.HidDeviceRemoved);
		}

		public void AutoConnect(HIDInterface.HidDevice hidDevice)
		{
			this.lowHidDevice = hidDevice;
			this.ContinueConnectFlag = true;
			this.ReadWriteThread.DoWork += new DoWorkEventHandler(this.ReadWriteThread_DoWork);
			this.ReadWriteThread.WorkerSupportsCancellation = true;
			this.ReadWriteThread.RunWorkerAsync();
		}

		public bool Connect(HIDInterface.HidDevice hidDevice)
		{
			bool flag;
			HIDInterface.ReusltString reusltString = new HIDInterface.ReusltString();
			if (this.oSp.OpenDevice(hidDevice.vID, hidDevice.pID, hidDevice.serial) != Hid.HID_RETURN.SUCCESS)
			{
				this.bConnected = false;
				reusltString.Result = false;
				reusltString.message = "Device Connect Error";
				this.RaiseEventConnectedState(reusltString.Result);
				flag = false;
			}
			else
			{
				this.bConnected = true;
				reusltString.Result = true;
				reusltString.message = "Connect Success!";
				this.RaiseEventConnectedState(reusltString.Result);
				flag = true;
			}
			return flag;
		}

		public void DisConnect()
		{
			this.bConnected = false;
			Thread.Sleep(200);
			if (this.oSp != null)
			{
				this.oSp.CloseDevice();
			}
		}

		public void Dispose()
		{
			try
			{
				this.DisConnect();
				this.oSp.DataReceived -= new Hid.DelegateDataReceived(this.HidDataReceived);
				this.oSp.DeviceRemoved -= new Hid.DelegateStatusConnected(this.HidDeviceRemoved);
				this.ReadWriteThread.DoWork -= new DoWorkEventHandler(this.ReadWriteThread_DoWork);
				this.ReadWriteThread.CancelAsync();
				this.ReadWriteThread.Dispose();
			}
			catch
			{
			}
		}

		~HIDInterface()
		{
			this.Dispose();
		}

		public void HidDataReceived(object sender, report e)
		{
			try
			{
				byte[] numArray = new byte[64];
				Array.Copy(e.reportBuff, 0, numArray, 0, 64);
				this.RaiseEventDataReceived(numArray);
			}
			catch
			{
				HIDInterface.ReusltString reusltString = new HIDInterface.ReusltString()
				{
					Result = false,
					message = "Receive Error"
				};
				this.RaiseEventConnectedState(reusltString.Result);
			}
		}

		private void HidDeviceRemoved(object sender, EventArgs e)
		{
			this.bConnected = false;
			HIDInterface.ReusltString reusltString = new HIDInterface.ReusltString()
			{
				Result = false,
				message = "Device Remove"
			};
			this.RaiseEventConnectedState(reusltString.Result);
			if (this.oSp != null)
			{
				this.oSp.CloseDevice();
			}
		}

		protected virtual void RaiseEventConnectedState(bool isConnect)
		{
			if (this.StatusConnected != null)
			{
				this.StatusConnected(this, isConnect);
			}
		}

		protected virtual void RaiseEventDataReceived(byte[] buf)
		{
			if (this.DataReceived != null)
			{
				this.DataReceived(this, buf);
			}
		}

		private void ReadWriteThread_DoWork(object sender, DoWorkEventArgs e)
		{
			while (this.ContinueConnectFlag)
			{
				try
				{
					if (!this.bConnected)
					{
						this.Connect(this.lowHidDevice);
					}
					Thread.Sleep(500);
				}
				catch
				{
				}
			}
		}

		public bool Send(byte[] byData)
		{
			bool flag;
			flag = (this.oSp.Write(new report(0, byData)) == Hid.HID_RETURN.SUCCESS ? true : false);
			return flag;
		}

		public bool Send(string strData)
		{
			return this.Send(Encoding.Unicode.GetBytes(strData));
		}

		public void StopAutoConnect()
		{
			try
			{
				this.ContinueConnectFlag = false;
				this.Dispose();
			}
			catch
			{
			}
		}

		public delegate void DelegateDataReceived(object sender, byte[] data);

		public delegate void DelegateStatusConnected(object sender, bool isConnect);

		public struct HidDevice
		{
			public ushort vID;

			public ushort pID;

			public string serial;
		}

		public enum MessagesType
		{
			Message,
			Error
		}

		public struct ReusltString
		{
			public bool Result;

			public string message;
		}

		public struct TagInfo
		{
			public string AntennaPort;

			public string EPC;
		}
	}
}