using File_NSP;
using HID_SIMPLE.HID;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace N76E_ICE
{
	public class Main_Form : Form
	{
		private ClassDataProcess HID_CDProcess = new ClassDataProcess();

		private file_loader File_LD = new file_loader();

		public bool HID_CH552_Connect = false;

		public bool Target_N76E_Connect = false;

		public bool COM_Status = false;

		public bool Auto_Program_Status = false;

		private byte[] TX_data_buff = new byte[64];

		private int[] TX_data_len = new int[1];

		private byte[] RX_data_buff = new byte[64];

		private int[] RX_data_len = new int[1];

		private byte ICP_OPA_CODE = 0;

		private file_loader.fileinfo APROM_Info = new file_loader.fileinfo();

		private file_loader.fileinfo LDROM_Info = new file_loader.fileinfo();

		private file_loader.fileinfo ReadROM_Info = new file_loader.fileinfo();

		public byte[] CFG_Buff = new byte[4];

		public ushort LDROM_Addr;

		private Main_Form.Program_str Program_Info = new Main_Form.Program_str();

		private uint COM_TX_Nums = 0;

		private uint COM_RX_Nums = 0;

		private Thread Thread_ComRX;

		private System.Timers.Timer aTimer = new System.Timers.Timer();

		private System.Timers.Timer Timer_Timeout = new System.Timers.Timer();

		private ushort Read_Addr;

		private ushort Read_Nums;

		private ushort Read_Now_Nums;

		private bool Read_err;

		private FileSystemWatcher APROM_Watcher;

		private FileSystemWatcher LDROM_Watcher;

		private bool before_checkBox_COM_EN = true;

		private uint True_Baud_Rate;

		private Queue COM_RX_Queue = new Queue();

		private IContainer components = null;

		private TextBox textBox_Log;

		private TextBox textBox_HEX_View;

		private TextBox textBox_COM_TX;

		private Button button_DebugClear;

		private Button button_SaveDebug;

		private Button button_Connect;

		private Label label_Connect_State;

		private Button button_APROM_Open;

		private CheckBox checkBox_APROM_EN;

		private CheckBox checkBox_LDROM_EN;

		private TextBox textBox_APROM_File_Path;

		private TextBox textBox_LDROM_File_Path;

		private Label label1;

		private Label label2;

		private Button button_LDROM_Open;

		private Button button_APROM_View;

		private Button button_LDROM_View;

		private Label label5;

		private CheckBox checkBox_CFG_EN;

		private Button checkBox_CFG_Set;

		private Button checkBox_CFG_Read;

		private TextBox textBox_CFG0;

		private TextBox textBox_CFG1;

		private TextBox textBox_CFG2;

		private TextBox textBox_CFG4;

		private Label label6;

		private Label label7;

		private Label label8;

		private Label label9;

		private Button button_Chip_Read;

		private Button button_Program;

		private Button button_Chip_Erase;

		private TextBox textBox_Read_File_Path;

		private Button button_Read_Save_Path;

		private Button button_Read_Save;

		private Button button_Auto_Program;

		private CheckBox checkBox_Earse_EN;

		private CheckBox checkBox_COM_EN;

		private ComboBox combo_Baud_Rate;

		private Button button_COM_TX;

		private Button button_COM_OPEN;

		private Label label3;

		private CheckBox checkBox_HEX_EN;

		private Button button_Open_CFG;

		private Button button_Save_CFG;

		private CheckBox checkBox_Verify;

		private CheckBox checkBox_Run;

		private CheckBox checkBox_Better_Erase;

		private Button button_Clear_Counter;

		private Label label4;

		private Label label_TX_Counter;

		private Label label11;

		private Label label_RX_Counter;

		public Main_Form()
		{
			this.InitializeComponent();
		}

		private void Auto_Program_Off(bool Speak_err)
		{
			if (this.Auto_Program_Status)
			{
				if (this.checkBox_APROM_EN.Checked)
				{
					this.APROM_Watcher.EnableRaisingEvents = false;
					this.APROM_Watcher.Changed -= new FileSystemEventHandler(this.ROM_Changed);
					this.APROM_Watcher = null;
				}
				if (this.checkBox_LDROM_EN.Checked)
				{
					this.LDROM_Watcher.EnableRaisingEvents = false;
					this.LDROM_Watcher.Changed -= new FileSystemEventHandler(this.ROM_Changed);
					this.LDROM_Watcher = null;
				}
				this.checkBox_APROM_EN.Enabled = true;
				this.button_APROM_Open.Enabled = true;
				this.textBox_HEX_View.AllowDrop = true;
				this.textBox_APROM_File_Path.AllowDrop = true;
				this.textBox_APROM_File_Path.ReadOnly = false;
				this.checkBox_LDROM_EN.Enabled = true;
				this.button_LDROM_Open.Enabled = true;
				this.textBox_LDROM_File_Path.AllowDrop = true;
				this.textBox_LDROM_File_Path.ReadOnly = false;
				this.button_Auto_Program.Text = "自动烧录";
				this.Auto_Program_Status = false;
				if (Speak_err)
				{
					MessageBox.Show("出现错误造成自动烧录监控退出", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}

		private void Auto_Program_On()
		{
			if (!this.Auto_Program_Status)
			{
				if ((this.checkBox_APROM_EN.Checked ? true : this.checkBox_LDROM_EN.Checked))
				{
					if (this.checkBox_APROM_EN.Checked)
					{
						this.APROM_Info.FilePath = this.textBox_APROM_File_Path.Text;
						if (!this.File_LD.UpdateFileInfo(ref this.APROM_Info))
						{
							return;
						}
						else if (!this.APROM_Info.useful)
						{
							return;
						}
					}
					if (this.checkBox_LDROM_EN.Checked)
					{
						this.LDROM_Info.FilePath = this.textBox_LDROM_File_Path.Text;
						if (!this.File_LD.UpdateFileInfo(ref this.LDROM_Info))
						{
							return;
						}
						else if (!this.LDROM_Info.useful)
						{
							return;
						}
					}
					this.checkBox_APROM_EN.Enabled = false;
					this.button_APROM_Open.Enabled = false;
					this.textBox_HEX_View.AllowDrop = false;
					this.textBox_APROM_File_Path.AllowDrop = false;
					this.textBox_APROM_File_Path.ReadOnly = true;
					this.checkBox_LDROM_EN.Enabled = false;
					this.button_LDROM_Open.Enabled = false;
					this.textBox_LDROM_File_Path.AllowDrop = false;
					this.textBox_LDROM_File_Path.ReadOnly = true;
					this.button_Auto_Program.Text = "停止监控";
					if (this.checkBox_APROM_EN.Checked)
					{
						this.APROM_Watcher = new FileSystemWatcher(Path.GetDirectoryName(this.APROM_Info.FilePath))
						{
							Filter = Path.GetFileName(this.APROM_Info.FilePath),
							EnableRaisingEvents = true
						};
						this.APROM_Watcher.Changed += new FileSystemEventHandler(this.ROM_Changed);
					}
					if (this.checkBox_LDROM_EN.Checked)
					{
						this.LDROM_Watcher = new FileSystemWatcher(Path.GetDirectoryName(this.LDROM_Info.FilePath))
						{
							Filter = Path.GetFileName(this.LDROM_Info.FilePath),
							EnableRaisingEvents = true
						};
						this.LDROM_Watcher.Changed += new FileSystemEventHandler(this.ROM_Changed);
					}
					this.Auto_Program_Status = true;
				}
				else
				{
					MessageBox.Show("没有烧录任务！", "错误提示");
				}
			}
		}

		private void button_APROM_Open_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Title = "请选择写入APROM的文件路径",
				Filter = "二进制文件(*.bin;*.hex)|*.bin;*.hex",
				RestoreDirectory = true,
				InitialDirectory = Application.StartupPath
			};
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.textBox_APROM_File_Path.Text = openFileDialog.FileName;
				this.button_APROM_View_Click(sender, e);
			}
		}

		private void button_APROM_View_Click(object sender, EventArgs e)
		{
			this.APROM_Info.FilePath = this.textBox_APROM_File_Path.Text;
			this.File_LD.UpdateFileInfo(ref this.APROM_Info);
			string str = "APROM_File:";
			this.File_LD.UpdateFile2Hex_Viewer(this.APROM_Info, ref str);
			if (this.APROM_Info.useful)
			{
				this.textBox_HEX_View.Clear();
				this.textBox_HEX_View.AppendText(str);
				this.textBox_HEX_View.SelectionStart = 0;
				this.textBox_HEX_View.ScrollToCaret();
			}
			str = null;
		}

		private void button_Auto_Program_Click(object sender, EventArgs e)
		{
		}

		private void button_Chip_Erase_Click(object sender, EventArgs e)
		{
			this.TX_data_buff[0] = 7;
			this.TX_data_buff[1] = 194;
			this.Set_TX_Task(this.TX_data_buff);
		}

		private void button_Chip_Read_Click(object sender, EventArgs e)
		{
			this.TX_data_buff[0] = 4;
			this.Read_Nums = 18432;
			this.U16_to_U8(this.TX_data_buff, 1, this.Read_Nums);
			this.Read_Addr = 0;
			this.U16_to_U8(this.TX_data_buff, 3, this.Read_Addr);
			this.Read_Now_Nums = 0;
			this.ReadROM_Info.useful = false;
			this.button_Read_Save.Enabled = false;
			this.Read_err = false;
			this.DisplayLog("整片读取...");
			this.Set_TX_Task(this.TX_data_buff);
		}

		private void button_Clear_Counter_Click(object sender, EventArgs e)
		{
			this.COM_RX_Nums = 0;
			this.COM_TX_Nums = 0;
			this.Refresh_Com_Nums();
		}

		private void button_COM_OPEN_Click(object sender, EventArgs e)
		{
			this.OPEN_COM();
		}

		private void button_COM_TX_Click(object sender, EventArgs e)
		{
			byte[] bytes;
			if (this.textBox_COM_TX.Text != "")
			{
				if (!this.checkBox_HEX_EN.Checked)
				{
					bytes = Encoding.Default.GetBytes(this.textBox_COM_TX.Text);
				}
				else
				{
					bytes = this.File_LD.Str_HEX_to_Byte(this.textBox_COM_TX.Text);
					if (bytes == null)
					{
						return;
					}
				}
				this.COM_TX_Nums += (int)bytes.Length;
				this.Refresh_Com_Nums();
				this.button_COM_TX.Enabled = false;
				this.combo_Baud_Rate.Enabled = false;
				uint length = (uint)bytes.GetLength(0);
				uint num = 0;
				uint num1 = 62;
				this.TX_data_buff[0] = 11;
				while (length != 0)
				{
					if (this.COM_Status)
					{
						if (length <= num1)
						{
							this.TX_data_buff[1] = (byte)length;
							Array.Copy(bytes, (long)num, this.TX_data_buff, (long)2, (long)length);
							length = 0;
						}
						else
						{
							this.TX_data_buff[1] = (byte)num1;
							Array.Copy(bytes, (long)num, this.TX_data_buff, (long)2, (long)num1);
							num += num1;
							length -= num1;
						}
						this.HID_CDProcess.HID_SendBytes(this.TX_data_buff);
					}
					else
					{
						break;
					}
				}
				this.combo_Baud_Rate.Enabled = true;
				if (this.COM_Status)
				{
					this.button_COM_TX.Enabled = true;
				}
			}
		}

		private void button_Connect_Click(object sender, EventArgs e)
		{
			if (!this.Target_N76E_Connect)
			{
				this.TX_data_buff[0] = 2;
			}
			else
			{
				this.TX_data_buff[0] = 3;
			}
			this.Program_Info.Com_en = false;
			this.Close_COM();
			this.Set_TX_Task(this.TX_data_buff);
		}

		private void button_DebugClear_Click(object sender, EventArgs e)
		{
			this.textBox_Log.Clear();
		}

		private void button_LDROM_Open_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Title = "请选择写入LDROM的文件路径",
				Filter = "二进制文件(*.bin;*.hex)|*.bin;*.hex",
				RestoreDirectory = true,
				InitialDirectory = Application.StartupPath
			};
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.textBox_LDROM_File_Path.Text = openFileDialog.FileName;
				this.button_LDROM_View_Click(sender, e);
			}
		}

		private void button_LDROM_View_Click(object sender, EventArgs e)
		{
			this.LDROM_Info.FilePath = this.textBox_LDROM_File_Path.Text;
			this.File_LD.UpdateFileInfo(ref this.LDROM_Info);
			string str = "LDROM_File:";
			this.File_LD.UpdateFile2Hex_Viewer(this.LDROM_Info, ref str);
			if (this.LDROM_Info.useful)
			{
				this.textBox_HEX_View.Clear();
				this.textBox_HEX_View.AppendText(str);
				this.textBox_HEX_View.SelectionStart = 0;
				this.textBox_HEX_View.ScrollToCaret();
			}
			str = null;
		}

		private void button_Open_CFG_Click(object sender, EventArgs e)
		{
			this.Config_File_Load(sender, e, false);
		}

		private void button_Program_Click(object sender, EventArgs e)
		{
			this.Close_COM();
			this.Program_Begin(sender, e);
		}

		private void button_Read_Save_Click(object sender, EventArgs e)
		{
			if (this.ReadROM_Info.useful)
			{
				this.ReadROM_Info.FilePath = this.textBox_Read_File_Path.Text;
				if (this.File_LD.Check_File_Path(this.ReadROM_Info.FilePath))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(this.textBox_Read_File_Path.Text, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
					{
						binaryWriter.Write(this.ReadROM_Info.data_buff, 0, (int)this.ReadROM_Info.St_Size);
					}
					this.DisplayLog(string.Concat("已保存BIN到:", this.ReadROM_Info.FilePath));
				}
			}
		}

		private void button_Read_Save_Path_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				Title = "请选择回读文件存放路径",
				Filter = "二进制(*.bin)|*.bin",
				RestoreDirectory = true,
				InitialDirectory = Application.StartupPath,
				FileName = "ReadBin_N76E003"
			};
			if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.textBox_Read_File_Path.Text = saveFileDialog.FileName;
			}
		}

		private void button_Save_CFG_Click(object sender, EventArgs e)
		{
			Configuration str = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
			KeyValueConfigurationElement item = str.AppSettings.Settings["APROM_Enable"];
			bool @checked = this.checkBox_APROM_EN.Checked;
			item.Value = @checked.ToString();
			KeyValueConfigurationElement keyValueConfigurationElement = str.AppSettings.Settings["LDROM_Enable"];
			@checked = this.checkBox_LDROM_EN.Checked;
			keyValueConfigurationElement.Value = @checked.ToString();
			KeyValueConfigurationElement item1 = str.AppSettings.Settings["CONFIG_Enable"];
			@checked = this.checkBox_CFG_EN.Checked;
			item1.Value = @checked.ToString();
			KeyValueConfigurationElement str1 = str.AppSettings.Settings["Erase_Chip_Enable"];
			@checked = this.checkBox_Earse_EN.Checked;
			str1.Value = @checked.ToString();
			KeyValueConfigurationElement keyValueConfigurationElement1 = str.AppSettings.Settings["Verify_Enable"];
			@checked = this.checkBox_Verify.Checked;
			keyValueConfigurationElement1.Value = @checked.ToString();
			KeyValueConfigurationElement item2 = str.AppSettings.Settings["Open_COM_Enable"];
			@checked = this.checkBox_COM_EN.Checked;
			item2.Value = @checked.ToString();
			KeyValueConfigurationElement str2 = str.AppSettings.Settings["Run_Enable"];
			@checked = this.checkBox_Run.Checked;
			str2.Value = @checked.ToString();
			KeyValueConfigurationElement keyValueConfigurationElement2 = str.AppSettings.Settings["Less_Erase_Enable"];
			@checked = this.checkBox_Better_Erase.Checked;
			keyValueConfigurationElement2.Value = @checked.ToString();
			KeyValueConfigurationElement item3 = str.AppSettings.Settings["COM_HEX_Enable"];
			@checked = this.checkBox_HEX_EN.Checked;
			item3.Value = @checked.ToString();
			str.AppSettings.Settings["Baud_Rate"].Value = this.combo_Baud_Rate.SelectedIndex.ToString();
			str.AppSettings.Settings["APROM_File_Path"].Value = this.textBox_APROM_File_Path.Text;
			str.AppSettings.Settings["LDROM_File_Path"].Value = this.textBox_LDROM_File_Path.Text;
			if (this.textBox_Read_File_Path.Text != string.Concat(Application.StartupPath, "\\ReadBin.bin"))
			{
				str.AppSettings.Settings["Read_File_Save_Path"].Value = this.textBox_Read_File_Path.Text;
			}
			str.AppSettings.Settings["CONFIG0"].Value = this.CFG_Buff[0].ToString("X2");
			str.AppSettings.Settings["CONFIG1"].Value = this.CFG_Buff[1].ToString("X2");
			str.AppSettings.Settings["CONFIG2"].Value = this.CFG_Buff[2].ToString("X2");
			str.AppSettings.Settings["CONFIG4"].Value = this.CFG_Buff[3].ToString("X2");
			str.AppSettings.Settings["LDROM_BEGIN_ADDR"].Value = this.LDROM_Addr.ToString("X4");
			str.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection("appSettings");
		}

		private void button_SaveDebug_Click(object sender, EventArgs e)
		{
			string startupPath = Application.StartupPath;
			DateTime now = DateTime.Now;
			string str = string.Concat(startupPath, "Log", now.ToString("yyyyMMdd-HH-mm-ss"), ".txt");
			StreamWriter streamWriter = new StreamWriter(str, true);
			streamWriter.Write(this.textBox_Log.Text);
			streamWriter.Close();
			this.DisplayLog(string.Concat("日志输出到文件完成 路径:", str));
		}

		private void checkBox_CFG_EN_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.checkBox_CFG_EN.Checked)
			{
				this.checkBox_Earse_EN.Enabled = true;
				this.checkBox_Better_Erase.Enabled = true;
				this.checkBox_Better_Erase.Checked = true;
				this.checkBox_Earse_EN.Checked = false;
			}
			else
			{
				this.checkBox_Earse_EN.Checked = true;
				this.checkBox_Earse_EN.Enabled = false;
				this.checkBox_Better_Erase.Checked = false;
				this.checkBox_Better_Erase.Enabled = false;
			}
		}

		private void checkBox_CFG_Read_Click(object sender, EventArgs e)
		{
			this.TX_data_buff[0] = 8;
			this.Set_TX_Task(this.TX_data_buff);
		}

		private void checkBox_CFG_Set_Click(object sender, EventArgs e)
		{
			Form_cfg formCfg = new Form_cfg(this.CFG_Buff, this.LDROM_Addr)
			{
				StartPosition = FormStartPosition.CenterParent,
				GetDataHandler = new Form_cfg.Get_CFG_Value(this.Get_CFG_Value)
			};
			formCfg.ShowDialog();
		}

		private void checkBox_Earse_EN_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.checkBox_Earse_EN.Checked)
			{
				this.checkBox_Better_Erase.Enabled = true;
				this.checkBox_Better_Erase.Checked = true;
			}
			else
			{
				this.checkBox_Better_Erase.Checked = false;
				this.checkBox_Better_Erase.Enabled = false;
			}
		}

		private void checkBox_HEX_EN_Click(object sender, EventArgs e)
		{
			if (!this.checkBox_HEX_EN.Checked)
			{
				string str = this.File_LD.Str_HEX_to_Ascii(this.textBox_COM_TX.Text);
				if (str != null)
				{
					this.textBox_COM_TX.Text = str;
				}
				else
				{
					this.checkBox_HEX_EN.Checked = true;
				}
			}
			else
			{
				this.textBox_COM_TX.Text = this.File_LD.Str_Ascii_to_HEX(this.textBox_COM_TX.Text);
			}
		}

		private void checkBox_Run_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.checkBox_Run.Checked)
			{
				this.before_checkBox_COM_EN = this.checkBox_COM_EN.Checked;
				this.checkBox_COM_EN.Enabled = false;
				this.checkBox_COM_EN.Checked = false;
			}
			else
			{
				this.checkBox_COM_EN.Enabled = true;
				this.checkBox_COM_EN.Checked = this.before_checkBox_COM_EN;
			}
		}

		private void Chip_Read_Service()
		{
			if ((byte)(this.Read_Addr & 255) != this.RX_data_buff[1])
			{
				this.Read_err = true;
				this.DisplayErr(string.Concat("读取错误 addr0x", this.Read_Addr.ToString("X4")));
			}
			else if (!this.Read_err)
			{
				if (this.Read_Nums - this.Read_Now_Nums <= 62)
				{
					Array.Copy(this.RX_data_buff, 2, this.ReadROM_Info.data_buff, (int)this.Read_Addr, (int)(this.Read_Nums - this.Read_Now_Nums));
					this.ReadROM_Info.useful = true;
					this.button_Read_Save.Enabled = true;
					this.DisplayLog("读取完成");
					string str = "Chip_Read:";
					this.ReadROM_Info.FilenName = null;
					this.File_LD.UpdateFile2Hex_Viewer(this.ReadROM_Info, ref str);
					this.textBox_HEX_View.Text = str;
					str = null;
					this.ICP_OPA_CODE = 0;
				}
				else
				{
					Array.Copy(this.RX_data_buff, 2, this.ReadROM_Info.data_buff, (int)this.Read_Addr, 62);
					this.Read_Addr = (ushort)(this.Read_Addr + 62);
					this.Read_Now_Nums = (ushort)(this.Read_Now_Nums + 62);
					this.SetTimeOut(2000);
				}
			}
		}

		private void Close_COM()
		{
			this.button_COM_OPEN.Text = "打开串口";
			this.COM_Status = false;
			this.button_COM_TX.Enabled = false;
		}

		private void COM_RX_Service(byte[] rx_data)
		{
			uint rxData = rx_data[1];
			if (rxData != 0)
			{
				this.COM_RX_Nums += rxData;
				byte[] numArray = new byte[rxData];
				Array.Copy(rx_data, (long)2, numArray, (long)0, (long)rxData);
				this.COM_RX_Queue.Enqueue(numArray);
			}
		}

		private void COM_RX_SHOW()
		{
			uint i;
			byte num = 0;
			uint num1 = 0;
			while (true)
			{
				if ((this.COM_RX_Queue.Count <= 0 ? true : !this.COM_Status))
				{
					if (this.COM_Status)
					{
						num1++;
						if (num1 > 10)
						{
							num1 = 11;
							if (num > 127)
							{
								num = 0;
							}
						}
					}
					else
					{
						num = 0;
						this.COM_RX_Queue.Clear();
					}
					Thread.Sleep(10);
				}
				else
				{
					num1 = 0;
					string str = "";
					byte[] numArray = (byte[])this.COM_RX_Queue.Dequeue();
					if (!this.checkBox_HEX_EN.Checked)
					{
						uint num2 = 0;
						for (i = 0; (ulong)i < (long)((int)numArray.Length); i++)
						{
							if (numArray[i] > 127)
							{
								num2++;
							}
						}
						if (num < 127)
						{
							if (num2 % 2 != 0)
							{
								num = numArray[(int)numArray.Length - 1];
								byte[] numArray1 = new byte[(int)numArray.Length - 1];
								Array.Copy(numArray, 0, numArray1, 0, (int)numArray.Length - 1);
								str = Encoding.Default.GetString(numArray1);
							}
							else
							{
								num = 0;
								str = Encoding.Default.GetString(numArray);
							}
						}
						else if (num2 % 2 != 0)
						{
							byte[] numArray2 = new byte[(int)numArray.Length + 1];
							Array.Copy(numArray, 0, numArray2, 1, (int)numArray.Length);
							numArray2[0] = num;
							num = 0;
							str = Encoding.Default.GetString(numArray2);
						}
						else
						{
							byte[] numArray3 = new byte[(int)numArray.Length];
							Array.Copy(numArray, 0, numArray3, 1, (int)numArray.Length - 1);
							numArray3[0] = num;
							num = numArray[(int)numArray.Length - 1];
							str = Encoding.Default.GetString(numArray3);
						}
					}
					else
					{
						for (i = 0; (ulong)i < (long)((int)numArray.Length); i++)
						{
							str = string.Concat(str, numArray[i].ToString("X2"), " ");
						}
					}
					base.Invoke(new MethodInvoker(() => {
						this.textBox_Log.AppendText(str);
						this.Refresh_Com_Nums();
					}));
				}
			}
		}

		private void combo_Baud_Rate_SelectedValueChanged(object sender, EventArgs e)
		{
			if (this.COM_Status)
			{
				this.COM_Status = false;
				this.OPEN_COM();
			}
		}

		private void Config_File_Load(object sender, EventArgs e, bool First_Star)
		{
			Configuration str;
			int num;
			if (!First_Star)
			{
				OpenFileDialog openFileDialog = new OpenFileDialog()
				{
					Title = "请选择配置文件的路径",
					Filter = "配置文件(*.config)|*.config",
					RestoreDirectory = true,
					InitialDirectory = Application.StartupPath
				};
				if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				{
					return;
				}
				else
				{
					string str1 = string.Concat(Path.GetDirectoryName(openFileDialog.FileName), "\\", Path.GetFileNameWithoutExtension(openFileDialog.FileName));
					str = ConfigurationManager.OpenExeConfiguration(str1);
					if (!str.HasFile)
					{
						return;
					}
				}
			}
			else
			{
				str = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
			}
			if (!str.HasFile)
			{
				this.checkBox_APROM_EN.Checked = true;
				this.checkBox_LDROM_EN.Checked = false;
				this.checkBox_CFG_EN.Checked = false;
				this.checkBox_Earse_EN.Checked = false;
				this.checkBox_Verify.Checked = false;
				this.checkBox_COM_EN.Checked = true;
				this.checkBox_Run.Checked = true;
				this.checkBox_Better_Erase.Checked = true;
				this.textBox_Read_File_Path.Text = string.Concat(Application.StartupPath, "\\ReadBin.bin");
				this.combo_Baud_Rate.SelectedIndex = 3;
				this.checkBox_HEX_EN.Checked = false;
				this.CFG_Buff[0] = 255;
				this.CFG_Buff[1] = 255;
				this.CFG_Buff[2] = 255;
				this.CFG_Buff[3] = 255;
				this.Refresh_config_byte();
				this.LDROM_Addr = 16384;
			}
			else
			{
				string item = ConfigurationManager.AppSettings["APROM_Enable"];
				if ((item == "True" || item == "true" || item == "TRUE" ? false : item != "1"))
				{
					this.checkBox_APROM_EN.Checked = false;
				}
				else
				{
					this.checkBox_APROM_EN.Checked = true;
				}
				item = ConfigurationManager.AppSettings["LDROM_Enable"];
				if ((item == "True" || item == "true" || item == "TRUE" ? false : item != "1"))
				{
					this.checkBox_LDROM_EN.Checked = false;
				}
				else
				{
					this.checkBox_LDROM_EN.Checked = true;
				}
				item = ConfigurationManager.AppSettings["Less_Erase_Enable"];
				if ((item == "True" || item == "true" || item == "TRUE" ? false : item != "1"))
				{
					this.checkBox_Better_Erase.Checked = false;
				}
				else
				{
					this.checkBox_Better_Erase.Checked = true;
				}
				item = ConfigurationManager.AppSettings["Erase_Chip_Enable"];
				if ((item == "True" || item == "true" || item == "TRUE" ? false : item != "1"))
				{
					this.checkBox_Earse_EN.Checked = false;
				}
				else
				{
					this.checkBox_Better_Erase.Checked = false;
					this.checkBox_Better_Erase.Enabled = false;
					this.checkBox_Earse_EN.Checked = true;
				}
				item = ConfigurationManager.AppSettings["CONFIG_Enable"];
				if ((item == "True" || item == "true" || item == "TRUE" ? false : item != "1"))
				{
					this.checkBox_CFG_EN.Checked = false;
				}
				else
				{
					this.checkBox_Better_Erase.Checked = false;
					this.checkBox_Better_Erase.Enabled = false;
					this.checkBox_Earse_EN.Checked = true;
					this.checkBox_Earse_EN.Enabled = false;
					this.checkBox_CFG_EN.Checked = true;
				}
				item = ConfigurationManager.AppSettings["Verify_Enable"];
				if ((item == "True" || item == "true" || item == "TRUE" ? false : item != "1"))
				{
					this.checkBox_Verify.Checked = false;
				}
				else
				{
					this.checkBox_Verify.Checked = true;
				}
				item = ConfigurationManager.AppSettings["Open_COM_Enable"];
				if ((item == "True" || item == "true" || item == "TRUE" ? false : item != "1"))
				{
					this.checkBox_COM_EN.Checked = false;
				}
				else
				{
					this.checkBox_COM_EN.Checked = true;
				}
				item = ConfigurationManager.AppSettings["Run_Enable"];
				if ((item == "True" || item == "true" || item == "TRUE" ? false : item != "1"))
				{
					this.checkBox_Run.Checked = false;
					this.before_checkBox_COM_EN = this.checkBox_COM_EN.Checked;
					this.checkBox_COM_EN.Enabled = false;
					this.checkBox_COM_EN.Checked = false;
				}
				else
				{
					this.checkBox_Run.Checked = true;
				}
				this.textBox_APROM_File_Path.Text = ConfigurationManager.AppSettings["APROM_File_Path"];
				this.textBox_LDROM_File_Path.Text = ConfigurationManager.AppSettings["LDROM_File_Path"];
				item = ConfigurationManager.AppSettings["Read_File_Save_Path"];
				if (item != "")
				{
					this.textBox_Read_File_Path.Text = item;
				}
				else
				{
					this.textBox_Read_File_Path.Text = string.Concat(Application.StartupPath, "\\ReadBin.bin");
				}
				item = ConfigurationManager.AppSettings["Baud_Rate"];
				if ((!int.TryParse(item, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out num) || num < 0 ? true : num > 11))
				{
					this.combo_Baud_Rate.SelectedIndex = 3;
				}
				else
				{
					this.combo_Baud_Rate.SelectedIndex = num;
				}
				item = ConfigurationManager.AppSettings["COM_HEX_Enable"];
				if ((item == "True" || item == "true" || item == "TRUE" ? false : item != "1"))
				{
					this.checkBox_HEX_EN.Checked = false;
				}
				else
				{
					this.checkBox_HEX_EN.Checked = true;
				}
				item = ConfigurationManager.AppSettings["CONFIG0"];
				this.CFG_Buff[0] = this.Get_Hex(item);
				ref byte cFGBuff = ref this.CFG_Buff[0];
				cFGBuff = (byte)(cFGBuff | 73);
				item = ConfigurationManager.AppSettings["CONFIG1"];
				this.CFG_Buff[1] = this.Get_Hex(item);
				ref byte numPointer = ref this.CFG_Buff[1];
				numPointer = (byte)(numPointer | 248);
				item = ConfigurationManager.AppSettings["CONFIG2"];
				this.CFG_Buff[2] = this.Get_Hex(item);
				ref byte cFGBuff1 = ref this.CFG_Buff[2];
				cFGBuff1 = (byte)(cFGBuff1 | 67);
				item = ConfigurationManager.AppSettings["CONFIG4"];
				this.CFG_Buff[3] = this.Get_Hex(item);
				ref byte numPointer1 = ref this.CFG_Buff[3];
				numPointer1 = (byte)(numPointer1 | 15);
				this.Refresh_config_byte();
				item = ConfigurationManager.AppSettings["LDROM_BEGIN_ADDR"];
				if (item.Length >= 9)
				{
					this.LDROM_Addr = 16384;
				}
				else if ((!int.TryParse(item, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out num) || num < 0 ? true : num >= 18432))
				{
					this.LDROM_Addr = 16384;
				}
				else
				{
					if ((num & 127) != 0)
					{
						num &= 65408;
						str.AppSettings.Settings["LDROM_BEGIN_ADDR"].Value = num.ToString("X4");
						str.Save(ConfigurationSaveMode.Modified);
						ConfigurationManager.RefreshSection("appSettings");
					}
					this.LDROM_Addr = (ushort)num;
				}
			}
		}

		public void DisplayErr(string err)
		{
			TextBox textBoxLog = this.textBox_Log;
			DateTime now = DateTime.Now;
			textBoxLog.AppendText(string.Concat("Err:", now.ToString("HH:mm:ss:fff "), err, "\r\n"));
		}

		public void DisplayLog(string log)
		{
			TextBox textBoxLog = this.textBox_Log;
			DateTime now = DateTime.Now;
			textBoxLog.AppendText(string.Concat("Log:", now.ToString("HH:mm:ss:fff "), log, "\r\n"));
		}

		protected override void Dispose(bool disposing)
		{
			if ((!disposing ? false : this.components != null))
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		public void Get_CFG_Value(byte[] cfg_data, ushort ld_addr)
		{
			Array.Copy(cfg_data, 0, this.CFG_Buff, 0, 4);
			this.Refresh_config_byte();
			this.LDROM_Addr = ld_addr;
		}

		private byte Get_Hex(string str)
		{
			int num;
			byte num1;
			if (str == null)
			{
				num1 = 255;
			}
			else if (str.Length > 8)
			{
				num1 = 255;
			}
			else if ((!int.TryParse(str, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out num) || num < 0 ? true : num > 255))
			{
				num1 = 255;
			}
			else
			{
				num1 = (byte)num;
			}
			return num1;
		}

		public void HID_Connected()
		{
			this.label_Connect_State.ForeColor = Color.Aqua;
			this.label_Connect_State.Text = "已连接HID设备";
			this.ICP_OPA_CODE = 0;
			this.TX_data_buff[0] = 1;
			this.TX_data_buff[1] = 90;
			this.Set_TX_Task(this.TX_data_buff);
			this.button_COM_OPEN.Enabled = true;
			this.Close_COM();
			this.Program_Info.Com_en = false;
			this.button_Program.Enabled = true;
		}

		public void HID_Connected_Status(bool isConnected)
		{
			base.Invoke(new MethodInvoker(() => {
				if (!isConnected)
				{
					this.HID_CH552_Connect = false;
					this.HID_Disconnected();
				}
				else
				{
					this.HID_CH552_Connect = true;
					this.HID_Connected();
				}
			}));
		}

		public void HID_DataReceived(byte[] data)
		{
			if (data[0] != 11)
			{
				this.Timer_Timeout.Stop();
				this.Timer_Timeout.Enabled = false;
				Array.Copy(data, 0, this.RX_data_buff, 0, 64);
				base.Invoke(new MethodInvoker(() => {
					if ((this.ICP_OPA_CODE <= 0 ? false : this.RX_data_buff[0] == this.ICP_OPA_CODE))
					{
						switch (this.ICP_OPA_CODE)
						{
							case 1:
							{
								if ((this.RX_data_buff[1] != 82 ? true : this.RX_data_buff[9] != 16))
								{
									this.DisplayErr("不支持的编程器");
								}
								else
								{
									uint num = this.U8_to_U32(this.RX_data_buff, 3);
									this.label_Connect_State.ForeColor = Color.Blue;
									this.label_Connect_State.Text = string.Concat("编程器ID:", num.ToString("X2"));
									string[] str = new string[] { "已连接ICP编程器 ID:", num.ToString("X2"), "  版本:", null, null, null };
									int rXDataBuff = this.RX_data_buff[7] >> 4;
									str[3] = rXDataBuff.ToString("");
									str[4] = ".";
									rXDataBuff = this.RX_data_buff[7] & 15;
									str[5] = rXDataBuff.ToString("");
									this.DisplayLog(string.Concat(str));
									this.button_Connect.Enabled = true;
								}
								this.ICP_OPA_CODE = 0;
								break;
							}
							case 2:
							{
								if (this.RX_data_buff[1] == 0)
								{
									this.checkBox_CFG_Read.Enabled = true;
									this.button_Chip_Read.Enabled = true;
									this.button_Program.Enabled = true;
									this.button_Chip_Erase.Enabled = true;
									this.button_COM_OPEN.Enabled = false;
									this.Close_COM();
									this.button_Connect.Text = "断开";
									this.DisplayLog("目标已连接");
									this.label_Connect_State.ForeColor = Color.Green;
									this.label_Connect_State.Text = "目标已连接";
									this.Target_N76E_Connect = true;
									this.ICP_OPA_CODE = 0;
									this.Program_Service();
								}
								else
								{
									this.Target_N76E_Connect = false;
									this.Program_Info.Program_Err = true;
									if (this.RX_data_buff[1] == 1)
									{
										this.DisplayErr("目标无响应");
									}
									if (this.RX_data_buff[1] == 2)
									{
										this.DisplayErr("不支持的目标型号");
									}
									this.button_COM_OPEN.Enabled = true;
									this.Auto_Program_Off(true);
									this.ICP_OPA_CODE = 0;
								}
								break;
							}
							case 3:
							{
								this.DisplayLog("目标已断开");
								this.label_Connect_State.ForeColor = Color.Blue;
								this.label_Connect_State.Text = "已连接编程器";
								this.button_Connect.Text = "连接";
								this.Target_N76E_Connect = false;
								this.checkBox_CFG_Read.Enabled = false;
								this.button_Chip_Read.Enabled = false;
								this.button_Chip_Erase.Enabled = false;
								this.button_COM_OPEN.Enabled = true;
								this.ICP_OPA_CODE = 0;
								if (this.Program_Info.Com_en)
								{
									this.OPEN_COM();
									this.Program_Info.Com_en = false;
									this.ICP_OPA_CODE = 10;
								}
								break;
							}
							case 4:
							{
								this.Chip_Read_Service();
								break;
							}
							case 5:
							{
								this.ICP_OPA_CODE = 0;
								this.Program_Service();
								break;
							}
							case 6:
							{
								this.ICP_OPA_CODE = 0;
								break;
							}
							case 7:
							{
								if (this.RX_data_buff[1] != 1)
								{
									this.DisplayErr("擦除失败07");
									this.Program_Info.Program_Err = true;
								}
								else
								{
									this.DisplayLog("擦除完成");
								}
								this.ICP_OPA_CODE = 0;
								this.Program_Service();
								break;
							}
							case 8:
							{
								this.DisplayLog(string.Concat(new string[] { "目标配置位:0-", this.RX_data_buff[1].ToString("X2"), " 1-", this.RX_data_buff[2].ToString("X2"), " 2-", this.RX_data_buff[3].ToString("X2"), " 4-", this.RX_data_buff[4].ToString("X2") }));
								this.CFG_Buff[0] = this.RX_data_buff[1];
								this.CFG_Buff[1] = this.RX_data_buff[2];
								this.CFG_Buff[2] = this.RX_data_buff[3];
								this.CFG_Buff[3] = this.RX_data_buff[4];
								this.Refresh_config_byte();
								this.ICP_OPA_CODE = 0;
								break;
							}
							case 9:
							{
								this.Program_Service();
								break;
							}
							case 10:
							{
								if (this.RX_data_buff[1] != 0)
								{
									this.COM_Status = true;
									this.button_COM_OPEN.Text = "关闭串口";
									this.button_COM_TX.Enabled = true;
								}
								else
								{
									this.COM_Status = false;
									this.button_COM_OPEN.Text = "打开串口";
									this.button_COM_TX.Enabled = false;
								}
								break;
							}
							default:
							{
								this.ICP_OPA_CODE = 0;
								break;
							}
						}
					}
				}));
			}
			else if (this.COM_Status)
			{
				this.COM_RX_Service(data);
			}
		}

		public void HID_Disconnected()
		{
			this.label_Connect_State.ForeColor = Color.Red;
			this.label_Connect_State.Text = "无连接";
			this.DisplayLog("ICP编程器 已拔出");
			this.button_Connect.Text = "连接";
			this.Target_N76E_Connect = false;
			this.button_Connect.Enabled = false;
			this.checkBox_CFG_Read.Enabled = false;
			this.button_Chip_Read.Enabled = false;
			this.button_Program.Enabled = false;
			this.button_Auto_Program.Enabled = false;
			this.button_Chip_Erase.Enabled = false;
			this.button_COM_OPEN.Enabled = false;
			this.Close_COM();
			this.Program_Info.Com_en = false;
			this.Auto_Program_Off(false);
			this.ICP_OPA_CODE = 0;
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Main_Form));
			this.textBox_Log = new TextBox();
			this.textBox_HEX_View = new TextBox();
			this.textBox_COM_TX = new TextBox();
			this.button_DebugClear = new Button();
			this.button_SaveDebug = new Button();
			this.button_Connect = new Button();
			this.label_Connect_State = new Label();
			this.button_APROM_Open = new Button();
			this.checkBox_APROM_EN = new CheckBox();
			this.checkBox_LDROM_EN = new CheckBox();
			this.textBox_APROM_File_Path = new TextBox();
			this.textBox_LDROM_File_Path = new TextBox();
			this.label1 = new Label();
			this.label2 = new Label();
			this.button_LDROM_Open = new Button();
			this.button_APROM_View = new Button();
			this.button_LDROM_View = new Button();
			this.label5 = new Label();
			this.checkBox_CFG_EN = new CheckBox();
			this.checkBox_CFG_Set = new Button();
			this.checkBox_CFG_Read = new Button();
			this.textBox_CFG0 = new TextBox();
			this.textBox_CFG1 = new TextBox();
			this.textBox_CFG2 = new TextBox();
			this.textBox_CFG4 = new TextBox();
			this.label6 = new Label();
			this.label7 = new Label();
			this.label8 = new Label();
			this.label9 = new Label();
			this.button_Chip_Read = new Button();
			this.button_Program = new Button();
			this.button_Chip_Erase = new Button();
			this.textBox_Read_File_Path = new TextBox();
			this.button_Read_Save_Path = new Button();
			this.button_Read_Save = new Button();
			this.button_Auto_Program = new Button();
			this.checkBox_Earse_EN = new CheckBox();
			this.checkBox_COM_EN = new CheckBox();
			this.combo_Baud_Rate = new ComboBox();
			this.button_COM_TX = new Button();
			this.button_COM_OPEN = new Button();
			this.label3 = new Label();
			this.checkBox_HEX_EN = new CheckBox();
			this.button_Open_CFG = new Button();
			this.button_Save_CFG = new Button();
			this.checkBox_Verify = new CheckBox();
			this.checkBox_Run = new CheckBox();
			this.checkBox_Better_Erase = new CheckBox();
			this.button_Clear_Counter = new Button();
			this.label4 = new Label();
			this.label_TX_Counter = new Label();
			this.label11 = new Label();
			this.label_RX_Counter = new Label();
			base.SuspendLayout();
			this.textBox_Log.BackColor = SystemColors.Window;
			this.textBox_Log.Location = new Point(12, 203);
			this.textBox_Log.Multiline = true;
			this.textBox_Log.Name = "textBox_Log";
			this.textBox_Log.ReadOnly = true;
			this.textBox_Log.ScrollBars = ScrollBars.Vertical;
			this.textBox_Log.Size = new System.Drawing.Size(484, 361);
			this.textBox_Log.TabIndex = 1;
			this.textBox_HEX_View.AllowDrop = true;
			this.textBox_HEX_View.BackColor = SystemColors.Window;
			this.textBox_HEX_View.Location = new Point(12, 12);
			this.textBox_HEX_View.Multiline = true;
			this.textBox_HEX_View.Name = "textBox_HEX_View";
			this.textBox_HEX_View.ReadOnly = true;
			this.textBox_HEX_View.ScrollBars = ScrollBars.Vertical;
			this.textBox_HEX_View.Size = new System.Drawing.Size(484, 185);
			this.textBox_HEX_View.TabIndex = 2;
			this.textBox_HEX_View.DragDrop += new DragEventHandler(this.textBox_HEX_View_DragDrop);
			this.textBox_HEX_View.DragEnter += new DragEventHandler(this.textBox_HEX_View_DragEnter);
			this.textBox_COM_TX.Location = new Point(12, 570);
			this.textBox_COM_TX.Multiline = true;
			this.textBox_COM_TX.Name = "textBox_COM_TX";
			this.textBox_COM_TX.Size = new System.Drawing.Size(422, 73);
			this.textBox_COM_TX.TabIndex = 3;
			this.textBox_COM_TX.KeyPress += new KeyPressEventHandler(this.textBox_COM_TX_KeyPress);
			this.button_DebugClear.Location = new Point(505, 526);
			this.button_DebugClear.Name = "button_DebugClear";
			this.button_DebugClear.Size = new System.Drawing.Size(90, 38);
			this.button_DebugClear.TabIndex = 10;
			this.button_DebugClear.Text = "清空日志";
			this.button_DebugClear.UseVisualStyleBackColor = true;
			this.button_DebugClear.Click += new EventHandler(this.button_DebugClear_Click);
			this.button_SaveDebug.Location = new Point(625, 526);
			this.button_SaveDebug.Name = "button_SaveDebug";
			this.button_SaveDebug.Size = new System.Drawing.Size(93, 36);
			this.button_SaveDebug.TabIndex = 11;
			this.button_SaveDebug.Text = "保存日志";
			this.button_SaveDebug.UseVisualStyleBackColor = true;
			this.button_SaveDebug.Click += new EventHandler(this.button_SaveDebug_Click);
			this.button_Connect.Location = new Point(502, 12);
			this.button_Connect.Name = "button_Connect";
			this.button_Connect.Size = new System.Drawing.Size(93, 27);
			this.button_Connect.TabIndex = 12;
			this.button_Connect.Text = "连接";
			this.button_Connect.UseVisualStyleBackColor = true;
			this.button_Connect.Click += new EventHandler(this.button_Connect_Click);
			this.label_Connect_State.AutoSize = true;
			this.label_Connect_State.BackColor = SystemColors.Control;
			this.label_Connect_State.ForeColor = SystemColors.ActiveCaptionText;
			this.label_Connect_State.Location = new Point(601, 18);
			this.label_Connect_State.Name = "label_Connect_State";
			this.label_Connect_State.Size = new System.Drawing.Size(67, 15);
			this.label_Connect_State.TabIndex = 54;
			this.label_Connect_State.Text = "连接状态";
			this.button_APROM_Open.Location = new Point(615, 45);
			this.button_APROM_Open.Name = "button_APROM_Open";
			this.button_APROM_Open.Size = new System.Drawing.Size(60, 27);
			this.button_APROM_Open.TabIndex = 55;
			this.button_APROM_Open.Text = "载入";
			this.button_APROM_Open.UseVisualStyleBackColor = true;
			this.button_APROM_Open.Click += new EventHandler(this.button_APROM_Open_Click);
			this.checkBox_APROM_EN.AutoSize = true;
			this.checkBox_APROM_EN.Location = new Point(555, 50);
			this.checkBox_APROM_EN.Name = "checkBox_APROM_EN";
			this.checkBox_APROM_EN.Size = new System.Drawing.Size(59, 19);
			this.checkBox_APROM_EN.TabIndex = 57;
			this.checkBox_APROM_EN.Text = "烧写";
			this.checkBox_APROM_EN.UseVisualStyleBackColor = true;
			this.checkBox_LDROM_EN.AutoSize = true;
			this.checkBox_LDROM_EN.Location = new Point(555, 114);
			this.checkBox_LDROM_EN.Name = "checkBox_LDROM_EN";
			this.checkBox_LDROM_EN.Size = new System.Drawing.Size(59, 19);
			this.checkBox_LDROM_EN.TabIndex = 59;
			this.checkBox_LDROM_EN.Text = "烧写";
			this.checkBox_LDROM_EN.UseVisualStyleBackColor = true;
			this.textBox_APROM_File_Path.AllowDrop = true;
			this.textBox_APROM_File_Path.Location = new Point(502, 78);
			this.textBox_APROM_File_Path.Name = "textBox_APROM_File_Path";
			this.textBox_APROM_File_Path.Size = new System.Drawing.Size(253, 25);
			this.textBox_APROM_File_Path.TabIndex = 60;
			this.textBox_APROM_File_Path.DragDrop += new DragEventHandler(this.textBox_APROM_File_Path_DragDrop);
			this.textBox_APROM_File_Path.DragEnter += new DragEventHandler(this.textBox_APROM_File_Path_DragEnter);
			this.textBox_LDROM_File_Path.AllowDrop = true;
			this.textBox_LDROM_File_Path.Location = new Point(502, 141);
			this.textBox_LDROM_File_Path.Name = "textBox_LDROM_File_Path";
			this.textBox_LDROM_File_Path.Size = new System.Drawing.Size(253, 25);
			this.textBox_LDROM_File_Path.TabIndex = 61;
			this.textBox_LDROM_File_Path.DragDrop += new DragEventHandler(this.textBox_LDROM_File_Path_DragDrop);
			this.textBox_LDROM_File_Path.DragEnter += new DragEventHandler(this.textBox_LDROM_File_Path_DragEnter);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(502, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 15);
			this.label1.TabIndex = 62;
			this.label1.Text = "APROM";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(502, 115);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 15);
			this.label2.TabIndex = 63;
			this.label2.Text = "LDROM";
			this.button_LDROM_Open.Location = new Point(615, 109);
			this.button_LDROM_Open.Name = "button_LDROM_Open";
			this.button_LDROM_Open.Size = new System.Drawing.Size(60, 27);
			this.button_LDROM_Open.TabIndex = 64;
			this.button_LDROM_Open.Text = "载入";
			this.button_LDROM_Open.UseVisualStyleBackColor = true;
			this.button_LDROM_Open.Click += new EventHandler(this.button_LDROM_Open_Click);
			this.button_APROM_View.Location = new Point(695, 45);
			this.button_APROM_View.Name = "button_APROM_View";
			this.button_APROM_View.Size = new System.Drawing.Size(60, 27);
			this.button_APROM_View.TabIndex = 65;
			this.button_APROM_View.Text = "查看";
			this.button_APROM_View.UseVisualStyleBackColor = true;
			this.button_APROM_View.Click += new EventHandler(this.button_APROM_View_Click);
			this.button_LDROM_View.Location = new Point(695, 109);
			this.button_LDROM_View.Name = "button_LDROM_View";
			this.button_LDROM_View.Size = new System.Drawing.Size(60, 27);
			this.button_LDROM_View.TabIndex = 66;
			this.button_LDROM_View.Text = "查看";
			this.button_LDROM_View.UseVisualStyleBackColor = true;
			this.button_LDROM_View.Click += new EventHandler(this.button_LDROM_View_Click);
			this.label5.AutoSize = true;
			this.label5.Location = new Point(502, 176);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(52, 15);
			this.label5.TabIndex = 69;
			this.label5.Text = "配置字";
			this.checkBox_CFG_EN.AutoSize = true;
			this.checkBox_CFG_EN.Location = new Point(555, 175);
			this.checkBox_CFG_EN.Name = "checkBox_CFG_EN";
			this.checkBox_CFG_EN.Size = new System.Drawing.Size(59, 19);
			this.checkBox_CFG_EN.TabIndex = 70;
			this.checkBox_CFG_EN.Text = "烧写";
			this.checkBox_CFG_EN.UseVisualStyleBackColor = true;
			this.checkBox_CFG_EN.CheckedChanged += new EventHandler(this.checkBox_CFG_EN_CheckedChanged);
			this.checkBox_CFG_Set.Location = new Point(615, 170);
			this.checkBox_CFG_Set.Name = "checkBox_CFG_Set";
			this.checkBox_CFG_Set.Size = new System.Drawing.Size(60, 27);
			this.checkBox_CFG_Set.TabIndex = 71;
			this.checkBox_CFG_Set.Text = "设置";
			this.checkBox_CFG_Set.UseVisualStyleBackColor = true;
			this.checkBox_CFG_Set.Click += new EventHandler(this.checkBox_CFG_Set_Click);
			this.checkBox_CFG_Read.Location = new Point(695, 170);
			this.checkBox_CFG_Read.Name = "checkBox_CFG_Read";
			this.checkBox_CFG_Read.Size = new System.Drawing.Size(60, 27);
			this.checkBox_CFG_Read.TabIndex = 72;
			this.checkBox_CFG_Read.Text = "读取";
			this.checkBox_CFG_Read.UseVisualStyleBackColor = true;
			this.checkBox_CFG_Read.Click += new EventHandler(this.checkBox_CFG_Read_Click);
			this.textBox_CFG0.Location = new Point(565, 210);
			this.textBox_CFG0.Name = "textBox_CFG0";
			this.textBox_CFG0.ReadOnly = true;
			this.textBox_CFG0.Size = new System.Drawing.Size(60, 25);
			this.textBox_CFG0.TabIndex = 73;
			this.textBox_CFG1.Location = new Point(695, 210);
			this.textBox_CFG1.Name = "textBox_CFG1";
			this.textBox_CFG1.ReadOnly = true;
			this.textBox_CFG1.Size = new System.Drawing.Size(60, 25);
			this.textBox_CFG1.TabIndex = 74;
			this.textBox_CFG2.Location = new Point(565, 241);
			this.textBox_CFG2.Name = "textBox_CFG2";
			this.textBox_CFG2.ReadOnly = true;
			this.textBox_CFG2.Size = new System.Drawing.Size(60, 25);
			this.textBox_CFG2.TabIndex = 75;
			this.textBox_CFG4.Location = new Point(695, 241);
			this.textBox_CFG4.Name = "textBox_CFG4";
			this.textBox_CFG4.ReadOnly = true;
			this.textBox_CFG4.Size = new System.Drawing.Size(60, 25);
			this.textBox_CFG4.TabIndex = 76;
			this.label6.AutoSize = true;
			this.label6.Location = new Point(502, 213);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(63, 15);
			this.label6.TabIndex = 77;
			this.label6.Text = "CONFIG0";
			this.label7.AutoSize = true;
			this.label7.Location = new Point(630, 213);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(63, 15);
			this.label7.TabIndex = 78;
			this.label7.Text = "CONFIG1";
			this.label8.AutoSize = true;
			this.label8.Location = new Point(502, 244);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(63, 15);
			this.label8.TabIndex = 79;
			this.label8.Text = "CONFIG2";
			this.label9.AutoSize = true;
			this.label9.Location = new Point(630, 244);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(63, 15);
			this.label9.TabIndex = 80;
			this.label9.Text = "CONFIG4";
			this.button_Chip_Read.Location = new Point(505, 276);
			this.button_Chip_Read.Name = "button_Chip_Read";
			this.button_Chip_Read.Size = new System.Drawing.Size(90, 27);
			this.button_Chip_Read.TabIndex = 81;
			this.button_Chip_Read.Text = "整片读取";
			this.button_Chip_Read.UseVisualStyleBackColor = true;
			this.button_Chip_Read.Click += new EventHandler(this.button_Chip_Read_Click);
			this.button_Program.Location = new Point(505, 340);
			this.button_Program.Name = "button_Program";
			this.button_Program.Size = new System.Drawing.Size(90, 32);
			this.button_Program.TabIndex = 82;
			this.button_Program.Text = "烧录";
			this.button_Program.UseVisualStyleBackColor = true;
			this.button_Program.Click += new EventHandler(this.button_Program_Click);
			this.button_Chip_Erase.Location = new Point(505, 425);
			this.button_Chip_Erase.Name = "button_Chip_Erase";
			this.button_Chip_Erase.Size = new System.Drawing.Size(90, 32);
			this.button_Chip_Erase.TabIndex = 83;
			this.button_Chip_Erase.Text = "整片擦除";
			this.button_Chip_Erase.UseVisualStyleBackColor = true;
			this.button_Chip_Erase.Click += new EventHandler(this.button_Chip_Erase_Click);
			this.textBox_Read_File_Path.Location = new Point(502, 309);
			this.textBox_Read_File_Path.Name = "textBox_Read_File_Path";
			this.textBox_Read_File_Path.Size = new System.Drawing.Size(253, 25);
			this.textBox_Read_File_Path.TabIndex = 84;
			this.button_Read_Save_Path.Location = new Point(695, 276);
			this.button_Read_Save_Path.Name = "button_Read_Save_Path";
			this.button_Read_Save_Path.Size = new System.Drawing.Size(60, 27);
			this.button_Read_Save_Path.TabIndex = 86;
			this.button_Read_Save_Path.Text = "路径";
			this.button_Read_Save_Path.UseVisualStyleBackColor = true;
			this.button_Read_Save_Path.Click += new EventHandler(this.button_Read_Save_Path_Click);
			this.button_Read_Save.Location = new Point(615, 276);
			this.button_Read_Save.Name = "button_Read_Save";
			this.button_Read_Save.Size = new System.Drawing.Size(60, 27);
			this.button_Read_Save.TabIndex = 87;
			this.button_Read_Save.Text = "保存";
			this.button_Read_Save.UseVisualStyleBackColor = true;
			this.button_Read_Save.Click += new EventHandler(this.button_Read_Save_Click);
			this.button_Auto_Program.Location = new Point(505, 382);
			this.button_Auto_Program.Name = "button_Auto_Program";
			this.button_Auto_Program.Size = new System.Drawing.Size(90, 32);
			this.button_Auto_Program.TabIndex = 88;
			this.button_Auto_Program.Text = "自动烧录";
			this.button_Auto_Program.UseVisualStyleBackColor = true;
			this.button_Auto_Program.Click += new EventHandler(this.button_Auto_Program_Click);
			this.checkBox_Earse_EN.AutoSize = true;
			this.checkBox_Earse_EN.Location = new Point(625, 345);
			this.checkBox_Earse_EN.Name = "checkBox_Earse_EN";
			this.checkBox_Earse_EN.Size = new System.Drawing.Size(134, 19);
			this.checkBox_Earse_EN.TabIndex = 89;
			this.checkBox_Earse_EN.Text = "下载前整片擦除";
			this.checkBox_Earse_EN.UseVisualStyleBackColor = true;
			this.checkBox_Earse_EN.CheckedChanged += new EventHandler(this.checkBox_Earse_EN_CheckedChanged);
			this.checkBox_COM_EN.AutoSize = true;
			this.checkBox_COM_EN.Location = new Point(625, 395);
			this.checkBox_COM_EN.Name = "checkBox_COM_EN";
			this.checkBox_COM_EN.Size = new System.Drawing.Size(134, 19);
			this.checkBox_COM_EN.TabIndex = 90;
			this.checkBox_COM_EN.Text = "下载后打开串口";
			this.checkBox_COM_EN.UseVisualStyleBackColor = true;
			this.combo_Baud_Rate.DropDownStyle = ComboBoxStyle.DropDownList;
			this.combo_Baud_Rate.FormattingEnabled = true;
			this.combo_Baud_Rate.Location = new Point(666, 568);
			this.combo_Baud_Rate.Name = "combo_Baud_Rate";
			this.combo_Baud_Rate.Size = new System.Drawing.Size(89, 23);
			this.combo_Baud_Rate.TabIndex = 91;
			this.combo_Baud_Rate.SelectedValueChanged += new EventHandler(this.combo_Baud_Rate_SelectedValueChanged);
			this.button_COM_TX.Location = new Point(440, 570);
			this.button_COM_TX.Name = "button_COM_TX";
			this.button_COM_TX.Size = new System.Drawing.Size(56, 73);
			this.button_COM_TX.TabIndex = 92;
			this.button_COM_TX.Text = "发送";
			this.button_COM_TX.UseVisualStyleBackColor = true;
			this.button_COM_TX.Click += new EventHandler(this.button_COM_TX_Click);
			this.button_COM_OPEN.Location = new Point(505, 573);
			this.button_COM_OPEN.Name = "button_COM_OPEN";
			this.button_COM_OPEN.Size = new System.Drawing.Size(90, 44);
			this.button_COM_OPEN.TabIndex = 93;
			this.button_COM_OPEN.Text = "打开串口";
			this.button_COM_OPEN.UseVisualStyleBackColor = true;
			this.button_COM_OPEN.Click += new EventHandler(this.button_COM_OPEN_Click);
			this.label3.AutoSize = true;
			this.label3.Location = new Point(601, 573);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 15);
			this.label3.TabIndex = 94;
			this.label3.Text = "波特率";
			this.checkBox_HEX_EN.AutoSize = true;
			this.checkBox_HEX_EN.Location = new Point(600, 598);
			this.checkBox_HEX_EN.Name = "checkBox_HEX_EN";
			this.checkBox_HEX_EN.Size = new System.Drawing.Size(75, 19);
			this.checkBox_HEX_EN.TabIndex = 95;
			this.checkBox_HEX_EN.Text = "16进制";
			this.checkBox_HEX_EN.UseVisualStyleBackColor = true;
			this.checkBox_HEX_EN.Click += new EventHandler(this.checkBox_HEX_EN_Click);
			this.button_Open_CFG.Location = new Point(505, 470);
			this.button_Open_CFG.Name = "button_Open_CFG";
			this.button_Open_CFG.Size = new System.Drawing.Size(90, 38);
			this.button_Open_CFG.TabIndex = 96;
			this.button_Open_CFG.Text = "载入配置";
			this.button_Open_CFG.UseVisualStyleBackColor = true;
			this.button_Open_CFG.Click += new EventHandler(this.button_Open_CFG_Click);
			this.button_Save_CFG.Location = new Point(625, 470);
			this.button_Save_CFG.Name = "button_Save_CFG";
			this.button_Save_CFG.Size = new System.Drawing.Size(93, 38);
			this.button_Save_CFG.TabIndex = 97;
			this.button_Save_CFG.Text = "保存配置";
			this.button_Save_CFG.UseVisualStyleBackColor = true;
			this.button_Save_CFG.Click += new EventHandler(this.button_Save_CFG_Click);
			this.checkBox_Verify.AutoSize = true;
			this.checkBox_Verify.Location = new Point(625, 370);
			this.checkBox_Verify.Name = "checkBox_Verify";
			this.checkBox_Verify.Size = new System.Drawing.Size(104, 19);
			this.checkBox_Verify.TabIndex = 98;
			this.checkBox_Verify.Text = "下载后校验";
			this.checkBox_Verify.UseVisualStyleBackColor = true;
			this.checkBox_Run.AutoSize = true;
			this.checkBox_Run.Location = new Point(625, 420);
			this.checkBox_Run.Name = "checkBox_Run";
			this.checkBox_Run.Size = new System.Drawing.Size(104, 19);
			this.checkBox_Run.TabIndex = 99;
			this.checkBox_Run.Text = "下载后运行";
			this.checkBox_Run.UseVisualStyleBackColor = true;
			this.checkBox_Run.CheckedChanged += new EventHandler(this.checkBox_Run_CheckedChanged);
			this.checkBox_Better_Erase.AutoSize = true;
			this.checkBox_Better_Erase.Location = new Point(625, 445);
			this.checkBox_Better_Erase.Name = "checkBox_Better_Erase";
			this.checkBox_Better_Erase.Size = new System.Drawing.Size(134, 19);
			this.checkBox_Better_Erase.TabIndex = 100;
			this.checkBox_Better_Erase.Text = "优化擦写和速度";
			this.checkBox_Better_Erase.TextAlign = ContentAlignment.TopCenter;
			this.checkBox_Better_Erase.UseVisualStyleBackColor = true;
			this.button_Clear_Counter.Location = new Point(695, 616);
			this.button_Clear_Counter.Name = "button_Clear_Counter";
			this.button_Clear_Counter.Size = new System.Drawing.Size(53, 27);
			this.button_Clear_Counter.TabIndex = 101;
			this.button_Clear_Counter.Text = "清零";
			this.button_Clear_Counter.UseVisualStyleBackColor = true;
			this.button_Clear_Counter.Click += new EventHandler(this.button_Clear_Counter_Click);
			this.label4.AutoSize = true;
			this.label4.Location = new Point(502, 628);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(31, 15);
			this.label4.TabIndex = 102;
			this.label4.Text = "TX:";
			this.label_TX_Counter.AutoSize = true;
			this.label_TX_Counter.Location = new Point(526, 628);
			this.label_TX_Counter.Name = "label_TX_Counter";
			this.label_TX_Counter.Size = new System.Drawing.Size(39, 15);
			this.label_TX_Counter.TabIndex = 103;
			this.label_TX_Counter.Text = "nums";
			this.label11.AutoSize = true;
			this.label11.Location = new Point(594, 628);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(31, 15);
			this.label11.TabIndex = 104;
			this.label11.Text = "RX:";
			this.label_RX_Counter.AutoSize = true;
			this.label_RX_Counter.Location = new Point(622, 628);
			this.label_RX_Counter.Name = "label_RX_Counter";
			this.label_RX_Counter.Size = new System.Drawing.Size(39, 15);
			this.label_RX_Counter.TabIndex = 105;
			this.label_RX_Counter.Text = "nums";
			base.AutoScaleDimensions = new SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(767, 655);
			base.Controls.Add(this.label_RX_Counter);
			base.Controls.Add(this.label11);
			base.Controls.Add(this.label_TX_Counter);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.button_Clear_Counter);
			base.Controls.Add(this.checkBox_Better_Erase);
			base.Controls.Add(this.checkBox_Run);
			base.Controls.Add(this.checkBox_Verify);
			base.Controls.Add(this.button_Save_CFG);
			base.Controls.Add(this.button_Open_CFG);
			base.Controls.Add(this.checkBox_HEX_EN);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.button_COM_OPEN);
			base.Controls.Add(this.button_COM_TX);
			base.Controls.Add(this.combo_Baud_Rate);
			base.Controls.Add(this.checkBox_COM_EN);
			base.Controls.Add(this.checkBox_Earse_EN);
			base.Controls.Add(this.button_Auto_Program);
			base.Controls.Add(this.button_Read_Save);
			base.Controls.Add(this.button_Read_Save_Path);
			base.Controls.Add(this.textBox_Read_File_Path);
			base.Controls.Add(this.button_Chip_Erase);
			base.Controls.Add(this.button_Program);
			base.Controls.Add(this.button_Chip_Read);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.label7);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.textBox_CFG4);
			base.Controls.Add(this.textBox_CFG2);
			base.Controls.Add(this.textBox_CFG1);
			base.Controls.Add(this.textBox_CFG0);
			base.Controls.Add(this.checkBox_CFG_Read);
			base.Controls.Add(this.checkBox_CFG_Set);
			base.Controls.Add(this.checkBox_CFG_EN);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.button_LDROM_View);
			base.Controls.Add(this.button_APROM_View);
			base.Controls.Add(this.button_LDROM_Open);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.textBox_LDROM_File_Path);
			base.Controls.Add(this.textBox_APROM_File_Path);
			base.Controls.Add(this.checkBox_LDROM_EN);
			base.Controls.Add(this.button_APROM_Open);
			base.Controls.Add(this.label_Connect_State);
			base.Controls.Add(this.button_Connect);
			base.Controls.Add(this.button_SaveDebug);
			base.Controls.Add(this.button_DebugClear);
			base.Controls.Add(this.textBox_COM_TX);
			base.Controls.Add(this.textBox_HEX_View);
			base.Controls.Add(this.textBox_Log);
			base.Controls.Add(this.checkBox_APROM_EN);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.Name = "Main_Form";
			this.Text = "N76E003 ICP下载上位机  -  posystorage";
			base.FormClosing += new FormClosingEventHandler(this.Main_Form_FormClosing);
			base.Load += new EventHandler(this.Main_Form_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Close_COM();
			this.Thread_ComRX.Abort();
			this.Auto_Program_Off(false);
			if (this.Target_N76E_Connect)
			{
				this.TX_data_buff[0] = 3;
				this.Set_TX_Task(this.TX_data_buff);
				Thread.Sleep(10);
			}
			if (this.HID_CH552_Connect)
			{
				this.HID_CDProcess.HID_Close();
			}
		}

		private void Main_Form_Load(object sender, EventArgs e)
		{
			this.combo_Baud_Rate.Items.Add("500000");
			this.combo_Baud_Rate.Items.Add("256000");
			this.combo_Baud_Rate.Items.Add("128000");
			this.combo_Baud_Rate.Items.Add("115200");
			this.combo_Baud_Rate.Items.Add("76800");
			this.combo_Baud_Rate.Items.Add("57600");
			this.combo_Baud_Rate.Items.Add("43000");
			this.combo_Baud_Rate.Items.Add("38400");
			this.combo_Baud_Rate.Items.Add("19200");
			this.combo_Baud_Rate.Items.Add("14400");
			this.combo_Baud_Rate.Items.Add("9600");
			this.combo_Baud_Rate.Items.Add("4800");
			this.Refresh_Com_Nums();
			this.Config_File_Load(sender, e, true);
			this.button_Read_Save.Enabled = false;
			this.button_Connect.Enabled = false;
			this.checkBox_CFG_Read.Enabled = false;
			this.button_Chip_Read.Enabled = false;
			this.button_Program.Enabled = false;
			this.button_Auto_Program.Enabled = false;
			this.button_Chip_Erase.Enabled = false;
			this.button_COM_OPEN.Enabled = false;
			this.Close_COM();
			this.HID_CH552_Connect = false;
			this.label_Connect_State.ForeColor = Color.Red;
			this.label_Connect_State.Text = "无连接";
			this.HID_CDProcess.HID_Initial();
			this.HID_CDProcess.isConnectedFunc = new ClassDataProcess.isConnectedDelegate(this.HID_Connected_Status);
			this.HID_CDProcess.pushReceiveData = new ClassDataProcess.PushReceiveDataDele(this.HID_DataReceived);
			this.ReadROM_Info.data_buff = new byte[18432];
			this.ReadROM_Info.St_Size = 18432;
			this.ReadROM_Info.useful = false;
			this.APROM_Info.useful = false;
			this.LDROM_Info.useful = false;
			this.Thread_ComRX = new Thread(new ThreadStart(this.COM_RX_SHOW));
			this.Thread_ComRX.Start();
			this.SetTimerParam();
		}

		private void OPEN_COM()
		{
			uint num;
			this.TX_data_buff[0] = 10;
			if (!this.COM_Status)
			{
				this.TX_data_buff[1] = 1;
				if (this.combo_Baud_Rate.Text.Length <= 8)
				{
					uint num1 = Convert.ToUInt32(this.combo_Baud_Rate.Text, 10);
					if (num1 >= 6000)
					{
						num = (uint)((double)(1500000f / (float)((float)num1)) + 0.5);
						this.True_Baud_Rate = 1500000 / num;
						this.TX_data_buff[3] = 0;
						if (num < 2)
						{
							return;
						}
						this.TX_data_buff[2] = (byte)num;
					}
					else
					{
						num = (uint)((double)(750000f / (float)((float)num1)) + 0.5);
						this.True_Baud_Rate = 750000 / num;
						this.TX_data_buff[3] = 1;
						if (num > 255)
						{
							return;
						}
						this.TX_data_buff[2] = (byte)num;
					}
					float trueBaudRate = ((float)((float)this.True_Baud_Rate) - (float)((float)num1)) / (float)((float)num1) * 100f;
					this.DisplayLog(string.Concat(new string[] { "打开串口 波特率设置:", num1.ToString(), " 实际:", this.True_Baud_Rate.ToString(), " 误差:", trueBaudRate.ToString("f2"), "%" }));
				}
				else
				{
					return;
				}
			}
			else
			{
				this.TX_data_buff[1] = 0;
			}
			this.Set_TX_Task(this.TX_data_buff);
		}

		private bool Program_Begin(object sender, EventArgs e)
		{
			bool flag;
			if (this.checkBox_Earse_EN.Checked)
			{
				if ((this.checkBox_APROM_EN.Checked || this.checkBox_LDROM_EN.Checked ? false : !this.checkBox_CFG_EN.Checked))
				{
					MessageBox.Show("没有烧录任务！", "错误提示");
					flag = false;
					return flag;
				}
			}
			else if ((this.checkBox_APROM_EN.Checked ? false : !this.checkBox_LDROM_EN.Checked))
			{
				MessageBox.Show("没有烧录任务！", "错误提示");
				flag = false;
				return flag;
			}
			if (this.checkBox_APROM_EN.Checked)
			{
				this.APROM_Info.FilePath = this.textBox_APROM_File_Path.Text;
				if (!this.File_LD.UpdateFileInfo(ref this.APROM_Info))
				{
					flag = false;
					return flag;
				}
				else if (!this.APROM_Info.useful)
				{
					flag = false;
					return flag;
				}
			}
			if (this.checkBox_LDROM_EN.Checked)
			{
				this.LDROM_Info.FilePath = this.textBox_LDROM_File_Path.Text;
				if (!this.File_LD.UpdateFileInfo(ref this.LDROM_Info))
				{
					flag = false;
					return flag;
				}
				else if (!this.LDROM_Info.useful)
				{
					flag = false;
					return flag;
				}
			}
			if (!this.checkBox_LDROM_EN.Checked)
			{
				if (this.checkBox_APROM_EN.Checked)
				{
					if (this.APROM_Info.St_Size > 18432)
					{
						MessageBox.Show("APROM 空间不足");
						flag = false;
						return flag;
					}
				}
				goto Label5;
			}
			else if ((this.LDROM_Addr & 127) != 0)
			{
				MessageBox.Show("LDROM写入地址没有对齐扇区(128字节)");
				flag = false;
			}
			else if ((long)(18432 - this.LDROM_Addr) >= (ulong)this.LDROM_Info.St_Size)
			{
				if (this.checkBox_APROM_EN.Checked)
				{
					if (this.LDROM_Addr < this.APROM_Info.St_Size)
					{
						MessageBox.Show("APROM 空间不足");
						flag = false;
						return flag;
					}
				}
				goto Label5;
			}
			else
			{
				MessageBox.Show("LDROM 空间不足");
				flag = false;
			}
			return flag;
		Label5:
			this.DisplayLog("开始编程");
			this.Program_Info.Program_Working = true;
			this.Program_Info.Erase_Chip = this.checkBox_Earse_EN.Checked;
			this.Program_Info.Aprom_en = this.checkBox_APROM_EN.Checked;
			this.Program_Info.Ldrom_en = this.checkBox_LDROM_EN.Checked;
			this.Program_Info.Config_en = this.checkBox_CFG_EN.Checked;
			this.Program_Info.Verify_en = this.checkBox_Verify.Checked;
			this.Program_Info.Com_en = this.checkBox_COM_EN.Checked;
			this.Program_Info.Less_Erase_en = this.checkBox_Better_Erase.Checked;
			this.Program_Info.After_Run_en = this.checkBox_Run.Checked;
			this.Program_Info.Program_Err = false;
			this.Program_Info.Process = 0;
			if (this.Target_N76E_Connect)
			{
				this.Program_Service();
			}
			else
			{
				this.TX_data_buff[0] = 2;
				this.Set_TX_Task(this.TX_data_buff);
			}
			flag = true;
			return flag;
		}

		private void Program_Service()
		{
			if (this.Program_Info.Program_Working)
			{
				if (!this.Program_Info.Program_Err)
				{
					switch (this.Program_Info.Process)
					{
						case 0:
						{
							if (!this.Program_Info.Erase_Chip)
							{
								if (!this.Program_Info.Aprom_en)
								{
									this.Program_Info.Process = 6;
									this.Program_Info.addr_now = this.LDROM_Addr;
									this.Program_Info.nums = (ushort)this.LDROM_Info.St_Size;
								}
								else
								{
									this.Program_Info.Process = 5;
									this.Program_Info.addr_now = 0;
									this.Program_Info.nums = (ushort)this.APROM_Info.St_Size;
								}
								this.RX_data_buff[1] = 1;
								this.Program_Service();
							}
							else
							{
								this.Program_Info.Process = 1;
								this.TX_data_buff[0] = 7;
								this.TX_data_buff[1] = 194;
								this.Set_TX_Task(this.TX_data_buff);
							}
							break;
						}
						case 1:
						{
							if (this.Program_Info.Config_en)
							{
								this.Program_Info.Process = 2;
								this.TX_data_buff[0] = 9;
								uint cFGBuff = this.CFG_Buff[0];
								cFGBuff += this.CFG_Buff[1];
								cFGBuff += this.CFG_Buff[2];
								cFGBuff += this.CFG_Buff[3];
								this.TX_data_buff[1] = this.CFG_Buff[0];
								this.TX_data_buff[2] = this.CFG_Buff[1];
								this.TX_data_buff[3] = this.CFG_Buff[2];
								this.TX_data_buff[4] = this.CFG_Buff[3];
								this.TX_data_buff[5] = (byte)cFGBuff;
								this.Set_TX_Task(this.TX_data_buff);
								break;
							}
							else if (!this.Program_Info.Aprom_en)
							{
								if (this.Program_Info.Ldrom_en)
								{
									this.Program_Info.Process = 4;
									this.Program_Info.addr_now = this.LDROM_Addr;
									this.Program_Info.nums = (ushort)this.LDROM_Info.St_Size;
									this.RX_data_buff[1] = 1;
									this.Program_Service();
								}
								break;
							}
							else
							{
								this.Program_Info.Process = 3;
								this.Program_Info.addr_now = 0;
								this.Program_Info.nums = (ushort)this.APROM_Info.St_Size;
								this.RX_data_buff[1] = 1;
								this.Program_Service();
								break;
							}
						}
						case 2:
						{
							if (this.RX_data_buff[1] == 0)
							{
								this.Program_Info.Program_Err = true;
							}
							if (this.RX_data_buff[2] != this.CFG_Buff[0])
							{
								this.Program_Info.Program_Err = true;
							}
							if (this.RX_data_buff[3] != this.CFG_Buff[1])
							{
								this.Program_Info.Program_Err = true;
							}
							if (this.RX_data_buff[4] != this.CFG_Buff[2])
							{
								this.Program_Info.Program_Err = true;
							}
							if (this.RX_data_buff[5] != this.CFG_Buff[3])
							{
								this.Program_Info.Program_Err = true;
							}
							if (!this.Program_Info.Program_Err)
							{
								this.DisplayLog("配置位已写入");
								if (!this.Program_Info.Aprom_en)
								{
									if (!this.Program_Info.Ldrom_en)
									{
										this.Program_Info.Process = 9;
									}
									else
									{
										this.Program_Info.Process = 4;
										this.Program_Info.addr_now = this.LDROM_Addr;
										this.Program_Info.nums = (ushort)this.LDROM_Info.St_Size;
									}
									this.RX_data_buff[1] = 1;
									this.Program_Service();
									break;
								}
								else
								{
									this.Program_Info.Process = 3;
									this.Program_Info.addr_now = 0;
									this.Program_Info.nums = (ushort)this.APROM_Info.St_Size;
									this.RX_data_buff[1] = 1;
									this.Program_Service();
									break;
								}
							}
							else
							{
								this.DisplayErr("配置位烧写错误");
								break;
							}
						}
						case 3:
						{
							if (this.RX_data_buff[1] != 0)
							{
								if (this.Program_Info.nums <= 60)
								{
									this.TX_data_buff[0] = 5;
									this.TX_data_buff[1] = (byte)this.Program_Info.nums;
									this.U16_to_U8(this.TX_data_buff, 2, this.Program_Info.addr_now);
									Array.Copy(this.APROM_Info.data_buff, (int)this.Program_Info.addr_now, this.TX_data_buff, 4, (int)this.Program_Info.nums);
									this.Set_TX_Task(this.TX_data_buff);
									this.DisplayLog("已写入APROM");
									if (this.Program_Info.Ldrom_en)
									{
										this.Program_Info.Process = 4;
										this.Program_Info.addr_now = this.LDROM_Addr;
										this.Program_Info.nums = (ushort)this.LDROM_Info.St_Size;
									}
									else if (!this.Program_Info.Verify_en)
									{
										this.Program_Info.Process = 9;
									}
									else
									{
										this.Program_Info.Process = 7;
										this.Program_Info.addr_now = 0;
										this.Program_Info.nums = (ushort)this.APROM_Info.St_Size;
									}
								}
								else
								{
									this.TX_data_buff[0] = 5;
									this.TX_data_buff[1] = 60;
									this.U16_to_U8(this.TX_data_buff, 2, this.Program_Info.addr_now);
									Array.Copy(this.APROM_Info.data_buff, (int)this.Program_Info.addr_now, this.TX_data_buff, 4, 60);
									ref ushort addrNow = ref this.Program_Info.addr_now;
									addrNow = (ushort)(addrNow + 60);
									ref ushort programInfo = ref this.Program_Info.nums;
									programInfo = (ushort)(programInfo - 60);
									this.Set_TX_Task(this.TX_data_buff);
								}
								break;
							}
							else
							{
								this.Program_Info.Program_Err = true;
								this.DisplayErr(string.Concat("APROM烧写错误 addr:0x", this.Program_Info.addr_now.ToString("X4")));
								break;
							}
						}
						case 4:
						{
							if (this.RX_data_buff[1] != 0)
							{
								if (this.Program_Info.nums <= 60)
								{
									this.TX_data_buff[0] = 5;
									this.TX_data_buff[1] = (byte)this.Program_Info.nums;
									this.U16_to_U8(this.TX_data_buff, 2, this.Program_Info.addr_now);
									Array.Copy(this.APROM_Info.data_buff, (int)(this.Program_Info.addr_now - this.LDROM_Addr), this.TX_data_buff, 4, (int)this.Program_Info.nums);
									this.Set_TX_Task(this.TX_data_buff);
									this.DisplayLog("已写入LDROM");
									if (!this.Program_Info.Verify_en)
									{
										this.Program_Info.Process = 9;
									}
									else if (!this.Program_Info.Aprom_en)
									{
										this.Program_Info.Process = 8;
										this.Program_Info.addr_now = this.LDROM_Addr;
										this.Program_Info.nums = (ushort)this.LDROM_Info.St_Size;
									}
									else
									{
										this.Program_Info.Process = 7;
										this.Program_Info.addr_now = 0;
										this.Program_Info.nums = (ushort)this.APROM_Info.St_Size;
									}
								}
								else
								{
									this.TX_data_buff[0] = 5;
									this.TX_data_buff[1] = 60;
									this.U16_to_U8(this.TX_data_buff, 2, this.Program_Info.addr_now);
									Array.Copy(this.LDROM_Info.data_buff, (int)(this.Program_Info.addr_now - this.LDROM_Addr), this.TX_data_buff, 4, 60);
									ref ushort numPointer = ref this.Program_Info.addr_now;
									numPointer = (ushort)(numPointer + 60);
									ref ushort programInfo1 = ref this.Program_Info.nums;
									programInfo1 = (ushort)(programInfo1 - 60);
									this.Set_TX_Task(this.TX_data_buff);
								}
								break;
							}
							else
							{
								this.Program_Info.Program_Err = true;
								this.DisplayErr(string.Concat("LDROM烧写错误 addr:0x", this.Program_Info.addr_now.ToString("X4")));
								break;
							}
						}
						case 5:
						{
							if (this.RX_data_buff[1] != 0)
							{
								if (this.Program_Info.nums <= 128)
								{
									if (!this.Program_Info.Less_Erase_en)
									{
										this.Program_Write_3package(this.Program_Info.addr_now, this.APROM_Info.data_buff, this.Program_Info.addr_now, 69, (byte)this.Program_Info.nums);
									}
									else
									{
										this.Program_Write_3package(this.Program_Info.addr_now, this.APROM_Info.data_buff, this.Program_Info.addr_now, 133, (byte)this.Program_Info.nums);
									}
									this.DisplayLog("已写入APROM");
									if (this.Program_Info.Ldrom_en)
									{
										this.Program_Info.Process = 6;
										this.Program_Info.addr_now = this.LDROM_Addr;
										this.Program_Info.nums = (ushort)this.LDROM_Info.St_Size;
									}
									else if (!this.Program_Info.Verify_en)
									{
										this.Program_Info.Process = 9;
									}
									else
									{
										this.Program_Info.Process = 7;
										this.Program_Info.addr_now = 0;
										this.Program_Info.nums = (ushort)this.APROM_Info.St_Size;
									}
								}
								else
								{
									if (!this.Program_Info.Less_Erase_en)
									{
										this.Program_Write_3package(this.Program_Info.addr_now, this.APROM_Info.data_buff, this.Program_Info.addr_now, 69, 128);
									}
									else
									{
										this.Program_Write_3package(this.Program_Info.addr_now, this.APROM_Info.data_buff, this.Program_Info.addr_now, 133, 128);
									}
									ref ushort addrNow1 = ref this.Program_Info.addr_now;
									addrNow1 = (ushort)(addrNow1 + 128);
									ref ushort numPointer1 = ref this.Program_Info.nums;
									numPointer1 = (ushort)(numPointer1 - 128);
								}
								break;
							}
							else
							{
								this.Program_Info.Program_Err = true;
								this.DisplayErr(string.Concat("APROM烧写错误 addr:0x", this.Program_Info.addr_now.ToString("X4")));
								break;
							}
						}
						case 6:
						{
							if (this.RX_data_buff[1] != 0)
							{
								if (this.Program_Info.nums <= 128)
								{
									if (!this.Program_Info.Less_Erase_en)
									{
										this.Program_Write_3package(this.Program_Info.addr_now, this.LDROM_Info.data_buff, (ushort)(this.Program_Info.addr_now - this.LDROM_Addr), 69, (byte)this.Program_Info.nums);
									}
									else
									{
										this.Program_Write_3package(this.Program_Info.addr_now, this.LDROM_Info.data_buff, (ushort)(this.Program_Info.addr_now - this.LDROM_Addr), 133, (byte)this.Program_Info.nums);
									}
									this.DisplayLog("已写入LDROM");
									if (!this.Program_Info.Verify_en)
									{
										this.Program_Info.Process = 9;
									}
									else if (!this.Program_Info.Aprom_en)
									{
										this.Program_Info.Process = 8;
										this.Program_Info.addr_now = this.LDROM_Addr;
										this.Program_Info.nums = (ushort)this.LDROM_Info.St_Size;
									}
									else
									{
										this.Program_Info.Process = 7;
										this.Program_Info.addr_now = 0;
										this.Program_Info.nums = (ushort)this.APROM_Info.St_Size;
									}
								}
								else
								{
									if (!this.Program_Info.Less_Erase_en)
									{
										this.Program_Write_3package(this.Program_Info.addr_now, this.LDROM_Info.data_buff, (ushort)(this.Program_Info.addr_now - this.LDROM_Addr), 69, 128);
									}
									else
									{
										this.Program_Write_3package(this.Program_Info.addr_now, this.LDROM_Info.data_buff, (ushort)(this.Program_Info.addr_now - this.LDROM_Addr), 133, 128);
									}
									ref ushort addrNow2 = ref this.Program_Info.addr_now;
									addrNow2 = (ushort)(addrNow2 + 128);
									ref ushort programInfo2 = ref this.Program_Info.nums;
									programInfo2 = (ushort)(programInfo2 - 128);
								}
								break;
							}
							else
							{
								this.Program_Info.Program_Err = true;
								this.DisplayErr(string.Concat("LDROM烧写错误 addr:0x", this.Program_Info.addr_now.ToString("X4")));
								break;
							}
						}
						case 7:
						{
							if (this.RX_data_buff[1] != 0)
							{
								if (this.Program_Info.nums <= 128)
								{
									this.Program_Write_3package(this.Program_Info.addr_now, this.APROM_Info.data_buff, this.Program_Info.addr_now, 197, (byte)this.Program_Info.nums);
									this.DisplayLog("已校验APROM");
									if (!this.Program_Info.Ldrom_en)
									{
										this.Program_Info.Process = 9;
									}
									else
									{
										this.Program_Info.Process = 8;
										this.Program_Info.addr_now = this.LDROM_Addr;
										this.Program_Info.nums = (ushort)this.LDROM_Info.St_Size;
									}
								}
								else
								{
									this.Program_Write_3package(this.Program_Info.addr_now, this.APROM_Info.data_buff, this.Program_Info.addr_now, 197, 128);
									ref ushort numPointer2 = ref this.Program_Info.addr_now;
									numPointer2 = (ushort)(numPointer2 + 128);
									ref ushort programInfo3 = ref this.Program_Info.nums;
									programInfo3 = (ushort)(programInfo3 - 128);
								}
								break;
							}
							else
							{
								this.Program_Info.Program_Err = true;
								this.DisplayErr(string.Concat("APROM校验错误 addr:0x", this.Program_Info.addr_now.ToString("X4")));
								break;
							}
						}
						case 8:
						{
							if (this.RX_data_buff[1] != 0)
							{
								if (this.Program_Info.nums <= 128)
								{
									this.Program_Write_3package(this.Program_Info.addr_now, this.LDROM_Info.data_buff, (ushort)(this.Program_Info.addr_now - this.LDROM_Addr), 133, (byte)this.Program_Info.nums);
									this.DisplayLog("已校验LDROM");
									this.Program_Info.Process = 9;
								}
								else
								{
									this.Program_Write_3package(this.Program_Info.addr_now, this.LDROM_Info.data_buff, (ushort)(this.Program_Info.addr_now - this.LDROM_Addr), 197, 128);
									ref ushort addrNow3 = ref this.Program_Info.addr_now;
									addrNow3 = (ushort)(addrNow3 + 128);
									ref ushort numPointer3 = ref this.Program_Info.nums;
									numPointer3 = (ushort)(numPointer3 - 128);
								}
								break;
							}
							else
							{
								this.Program_Info.Program_Err = true;
								this.DisplayErr(string.Concat("LDROM校验错误 addr:0x", this.Program_Info.addr_now.ToString("X4")));
								break;
							}
						}
						case 9:
						{
							if (this.RX_data_buff[1] != 0)
							{
								this.DisplayLog("编程完成");
								if (this.Program_Info.After_Run_en)
								{
									this.TX_data_buff[0] = 3;
									this.Set_TX_Task(this.TX_data_buff);
								}
								this.Program_Info.Program_Working = false;
								this.Program_Info.Program_Err = false;
								break;
							}
							else
							{
								this.Program_Info.Program_Err = true;
								this.DisplayErr("编程错误");
								break;
							}
						}
						default:
						{
							this.Program_Info.Program_Err = true;
							this.DisplayErr("烧写流程异常");
							break;
						}
					}
				}
				else
				{
					this.Auto_Program_Off(true);
					this.Program_Info.Program_Working = false;
				}
			}
		}

		private void Program_Write_3package(ushort Write_Addr, byte[] cache, ushort Index, byte cmd, byte nums)
		{
			byte[] numArray = new byte[128];
			if (nums >= 128)
			{
				Array.Copy(cache, (int)Index, numArray, 0, 128);
			}
			else
			{
				Array.Copy(cache, (int)Index, numArray, 0, (int)nums);
				for (byte i = nums; i < 128; i = (byte)(i + 1))
				{
					numArray[i] = 255;
				}
			}
			Index = 0;
			this.TX_data_buff[0] = cmd;
			this.TX_data_buff[1] = 56;
			this.U16_to_U8(this.TX_data_buff, 2, Write_Addr);
			this.TX_data_buff[4] = 1;
			Array.Copy(numArray, (int)Index, this.TX_data_buff, 5, (int)this.TX_data_buff[1]);
			this.Set_TX_Task(this.TX_data_buff);
			Write_Addr = (ushort)(Write_Addr + this.TX_data_buff[1]);
			Index = (ushort)(Index + this.TX_data_buff[1]);
			this.TX_data_buff[0] = cmd;
			this.TX_data_buff[1] = 56;
			this.U16_to_U8(this.TX_data_buff, 2, Write_Addr);
			this.TX_data_buff[4] = 2;
			Array.Copy(numArray, (int)Index, this.TX_data_buff, 5, (int)this.TX_data_buff[1]);
			this.Set_TX_Task(this.TX_data_buff);
			Write_Addr = (ushort)(Write_Addr + this.TX_data_buff[1]);
			Index = (ushort)(Index + this.TX_data_buff[1]);
			this.TX_data_buff[0] = cmd;
			this.TX_data_buff[1] = 16;
			this.U16_to_U8(this.TX_data_buff, 2, Write_Addr);
			this.TX_data_buff[4] = 3;
			Array.Copy(numArray, (int)Index, this.TX_data_buff, 5, (int)this.TX_data_buff[1]);
			this.Set_TX_Task(this.TX_data_buff);
		}

		private void Refresh_Com_Nums()
		{
			this.label_TX_Counter.Text = this.COM_TX_Nums.ToString();
			this.label_RX_Counter.Text = this.COM_RX_Nums.ToString();
		}

		public void Refresh_config_byte()
		{
			this.textBox_CFG0.Text = string.Concat("0x", this.CFG_Buff[0].ToString("X2"));
			this.textBox_CFG1.Text = string.Concat("0x", this.CFG_Buff[1].ToString("X2"));
			this.textBox_CFG2.Text = string.Concat("0x", this.CFG_Buff[2].ToString("X2"));
			this.textBox_CFG4.Text = string.Concat("0x", this.CFG_Buff[3].ToString("X2"));
		}

		private void ROM_Changed(object sender, FileSystemEventArgs e)
		{
			base.Invoke(new MethodInvoker(() => {
				if (!this.Program_Begin(sender, e))
				{
					this.Auto_Program_Off(true);
				}
			}));
		}

		public void Set_TX_Task(byte[] data)
		{
			this.ICP_OPA_CODE = (byte)(data[0] & 31);
			this.SetTimeOut(2000);
			this.HID_CDProcess.HID_SendBytes(this.TX_data_buff);
		}

		public void SetTimeOut(uint Time)
		{
			this.Timer_Timeout.Stop();
			this.Timer_Timeout.Enabled = false;
			this.Timer_Timeout.Elapsed += new ElapsedEventHandler(this.TimeOutCallback);
			this.Timer_Timeout.Interval = (double)((float)Time);
			this.Timer_Timeout.AutoReset = false;
			this.Timer_Timeout.Enabled = true;
		}

		public void SetTimerParam()
		{
			this.aTimer.Elapsed += new ElapsedEventHandler(this.TimerCallback);
			this.aTimer.Interval = 10000;
			this.aTimer.AutoReset = true;
			this.aTimer.Enabled = true;
		}

		private void textBox_APROM_File_Path_DragDrop(object sender, DragEventArgs e)
		{
			string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (this.File_LD.Check_File_Path(data[0]))
			{
				this.textBox_APROM_File_Path.Text = data[0];
				this.APROM_Info.FilePath = data[0];
				this.button_APROM_View_Click(sender, e);
			}
		}

		private void textBox_APROM_File_Path_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.None;
			}
			else
			{
				e.Effect = DragDropEffects.Move;
			}
		}

		private void textBox_COM_TX_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (this.checkBox_HEX_EN.Checked)
			{
				e.Handled = "0123456789ABCDEF \b".IndexOf(char.ToUpper(e.KeyChar)) < 0;
			}
		}

		private void textBox_HEX_View_DragDrop(object sender, DragEventArgs e)
		{
			string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (this.File_LD.Check_File_Path(data[0]))
			{
				this.textBox_APROM_File_Path.Text = data[0];
				this.APROM_Info.FilePath = data[0];
				this.button_APROM_View_Click(sender, e);
			}
		}

		private void textBox_HEX_View_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.None;
			}
			else
			{
				e.Effect = DragDropEffects.Move;
			}
		}

		private void textBox_LDROM_File_Path_DragDrop(object sender, DragEventArgs e)
		{
			string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (this.File_LD.Check_File_Path(data[0]))
			{
				this.textBox_LDROM_File_Path.Text = data[0];
				this.LDROM_Info.FilePath = data[0];
				this.button_LDROM_View_Click(sender, e);
			}
		}

		private void textBox_LDROM_File_Path_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.None;
			}
			else
			{
				e.Effect = DragDropEffects.Move;
			}
		}

		private void TimeOutCallback(object source, ElapsedEventArgs e)
		{
			this.Timer_Timeout.Stop();
			this.Timer_Timeout.Enabled = false;
			base.Invoke(new MethodInvoker(() => {
				if (this.ICP_OPA_CODE > 0)
				{
					if (this.ICP_OPA_CODE == 1)
					{
						this.DisplayErr("ICP编程器无响应");
					}
					else
					{
						this.DisplayErr("通讯超时");
						this.button_Connect.Text = "连接";
					}
					this.Target_N76E_Connect = false;
					this.HID_CH552_Connect = false;
					this.label_Connect_State.ForeColor = Color.Red;
					this.label_Connect_State.Text = "无连接";
					this.button_Connect.Enabled = false;
					this.checkBox_CFG_Read.Enabled = false;
					this.button_Chip_Read.Enabled = false;
					this.button_Program.Enabled = false;
					this.button_Auto_Program.Enabled = false;
					this.button_Chip_Erase.Enabled = false;
					this.Close_COM();
					this.Program_Info.Com_en = false;
					this.button_COM_OPEN.Enabled = false;
					this.Auto_Program_Off(true);
					this.ICP_OPA_CODE = 0;
				}
			}));
		}

		private void TimerCallback(object source, ElapsedEventArgs e)
		{
			GC.Collect();
		}

		public void U16_to_U8(byte[] args, byte i, ushort Dat_In)
		{
			args[i] = (byte)Dat_In;
			args[i + 1] = (byte)(Dat_In >> 8);
		}

		public void U32_to_U8(byte[] args, byte i, uint Dat_In)
		{
			args[i] = (byte)Dat_In;
			args[i + 1] = (byte)(Dat_In >> 8);
			args[i + 2] = (byte)(Dat_In >> 16);
			args[i + 3] = (byte)(Dat_In >> 24);
		}

		public ushort U8_to_U16(byte[] args, byte i)
		{
			uint num = (uint)(args[i] | args[i + 1] << 8 & 65280);
			return (ushort)num;
		}

		public uint U8_to_U32(byte[] args, byte i)
		{
			uint num = (uint)(args[i] | args[i + 1] << 8 & 65280 | args[i + 2] << 16 & 16711680 | args[i + 3] << 24 & -16777216);
			return num;
		}

		private struct Program_str
		{
			public bool Program_Working;

			public bool Erase_Chip;

			public bool Aprom_en;

			public bool Ldrom_en;

			public bool Config_en;

			public bool Verify_en;

			public bool Com_en;

			public bool After_Run_en;

			public bool Less_Erase_en;

			public bool Program_Err;

			public byte Process;

			public ushort addr_now;

			public ushort nums;
		}
	}
}