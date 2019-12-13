using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace N76E_ICE
{
	public class Form_cfg : Form
	{
		public byte[] CFG_data_Buff = new byte[4];

		private ushort LDROM_ADDR;

		public Form_cfg.Get_CFG_Value GetDataHandler;

		private IContainer components = null;

		private GroupBox groupBox1;

		private RadioButton radioButton_BS_LDROM;

		private RadioButton radioButton_BS_APROM;

		private GroupBox groupBox2;

		private RadioButton radioButton_LDSIZE_4K;

		private RadioButton radioButton_LDSIZE_3K;

		private RadioButton radioButton_LDSIZE_1K;

		private RadioButton radioButton_LDSIZE_0K;

		private RadioButton radioButton_LDSIZE_2K;

		private CheckBox checkBox_SECURITY_LOCK;

		private CheckBox checkBox_OCD_ENABLE;

		private GroupBox groupBox3;

		private RadioButton radioButton_RPD_INPUT;

		private RadioButton radioButton_RPD_RESET;

		private GroupBox groupBox4;

		private RadioButton radioButton_OCDPWM_CONTI;

		private RadioButton radioButton_OCDPWM_TRI;

		private GroupBox groupBox5;

		private CheckBox checkBox_BROWN_OUT_IAP;

		private CheckBox checkBox_BROWN_OUT_RESET;

		private CheckBox checkBox_BROWN_OUT_ENABLE;

		private RadioButton radioButton_BOV_4V4;

		private RadioButton radioButton_BOV_3V7;

		private RadioButton radioButton_BOV_2V7;

		private RadioButton radioButton_BOV_2V2;

		private GroupBox groupBox6;

		private RadioButton radioButton_WDT_ENABLE_STOP;

		private RadioButton radioButton_WDT_DISABLE;

		private ComboBox comboBox_ADDR;

		private Button button_OK;

		private Button button_Cancel;

		private RadioButton radioButton_WDT_ENABLE_KEEP;

		private Label label6;

		private GroupBox groupBox7;

		private Label label9;

		private TextBox textBox_CFG0;

		private Label label8;

		private TextBox textBox_CFG1;

		private Label label7;

		private TextBox textBox_CFG2;

		private Label label1;

		private TextBox textBox_CFG4;

		public Form_cfg(byte[] cfg_data, ushort Ldrom_ad)
		{
			this.InitializeComponent();
			Array.Copy(cfg_data, 0, this.CFG_data_Buff, 0, 4);
			this.LDROM_ADDR = Ldrom_ad;
			this.ConfigToGUI();
			this.Refresh_config_byte();
			this.comboBox_ADDR.Items.Add("2000");
			this.comboBox_ADDR.Items.Add("2400");
			this.comboBox_ADDR.Items.Add("2800");
			this.comboBox_ADDR.Items.Add("2C00");
			this.comboBox_ADDR.Items.Add("3000");
			this.comboBox_ADDR.Items.Add("3400");
			this.comboBox_ADDR.Items.Add("3800");
			this.comboBox_ADDR.Items.Add("3C00");
			this.comboBox_ADDR.Items.Add("4000");
			this.comboBox_ADDR.Items.Add("4400");
			this.comboBox_ADDR.Text = this.LDROM_ADDR.ToString("X4");
			this.radioButton_BS_APROM.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_BS_LDROM.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_OCDPWM_TRI.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_OCDPWM_CONTI.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.checkBox_OCD_ENABLE.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.checkBox_SECURITY_LOCK.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_RPD_RESET.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_RPD_INPUT.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_LDSIZE_0K.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_LDSIZE_1K.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_LDSIZE_2K.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_LDSIZE_3K.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_LDSIZE_4K.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.checkBox_BROWN_OUT_ENABLE.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.checkBox_BROWN_OUT_IAP.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.checkBox_BROWN_OUT_RESET.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_BOV_4V4.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_BOV_2V7.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_BOV_2V2.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_BOV_3V7.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_WDT_DISABLE.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_WDT_ENABLE_STOP.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_WDT_ENABLE_KEEP.MouseClick += new MouseEventHandler(this.Form_cfg_MousClick);
			this.radioButton_LDSIZE_0K.MouseClick += new MouseEventHandler(this.LDROM_MousClick);
			this.radioButton_LDSIZE_1K.MouseClick += new MouseEventHandler(this.LDROM_MousClick);
			this.radioButton_LDSIZE_2K.MouseClick += new MouseEventHandler(this.LDROM_MousClick);
			this.radioButton_LDSIZE_3K.MouseClick += new MouseEventHandler(this.LDROM_MousClick);
			this.radioButton_LDSIZE_4K.MouseClick += new MouseEventHandler(this.LDROM_MousClick);
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			ushort lDROMADDR;
			this.GUIToCFG();
			ref byte cFGDataBuff = ref this.CFG_data_Buff[0];
			cFGDataBuff = (byte)(cFGDataBuff | 73);
			ref byte numPointer = ref this.CFG_data_Buff[1];
			numPointer = (byte)(numPointer | 248);
			ref byte cFGDataBuff1 = ref this.CFG_data_Buff[2];
			cFGDataBuff1 = (byte)(cFGDataBuff1 | 67);
			ref byte numPointer1 = ref this.CFG_data_Buff[3];
			numPointer1 = (byte)(numPointer1 | 15);
			if (this.comboBox_ADDR.Text.Length <= 8)
			{
				uint num = Convert.ToUInt32(this.comboBox_ADDR.Text, 16);
				if (num < 18432)
				{
					this.LDROM_ADDR = (ushort)num;
					if ((this.LDROM_ADDR & 127) != 0)
					{
						lDROMADDR = (ushort)(this.LDROM_ADDR & 65408);
						if (MessageBox.Show(string.Concat(new string[] { "设置的LDROM地址0x", this.LDROM_ADDR.ToString("X4"), "未对齐flash扇区（128字节）,是否设置为:0x", lDROMADDR.ToString("X4"), "?" }), "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.OK)
						{
							goto Label1;
						}
						return;
					}
				Label2:
					this.GetDataHandler(this.CFG_data_Buff, this.LDROM_ADDR);
					base.Close();
				}
				else
				{
					MessageBox.Show(string.Concat("设置的LDROM地址0x", num.ToString("X"), "超出范围"), "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			else
			{
				MessageBox.Show("请设置正确的LDROM地址", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			return;
		Label1:
			this.LDROM_ADDR = lDROMADDR;
			goto Label2;
		}

		private void comboBox_ADDR_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = "0123456789ABCDEF\b".IndexOf(char.ToUpper(e.KeyChar)) < 0;
		}

		private void ConfigToGUI()
		{
			int cFGDataBuff;
			this.radioButton_BS_APROM.Checked = this.Dat_get_Bool(this.CFG_data_Buff[0] & 128);
			this.radioButton_BS_LDROM.Checked = this.Dat_get_Neg_Bool(this.CFG_data_Buff[0] & 128);
			this.radioButton_OCDPWM_TRI.Checked = this.Dat_get_Bool(this.CFG_data_Buff[0] & 32);
			this.radioButton_OCDPWM_CONTI.Checked = this.Dat_get_Neg_Bool(this.CFG_data_Buff[0] & 32);
			this.checkBox_OCD_ENABLE.Checked = this.Dat_get_Neg_Bool(this.CFG_data_Buff[0] & 16);
			this.radioButton_RPD_RESET.Checked = this.Dat_get_Bool(this.CFG_data_Buff[0] & 4);
			this.radioButton_RPD_INPUT.Checked = this.Dat_get_Neg_Bool(this.CFG_data_Buff[0] & 4);
			this.checkBox_SECURITY_LOCK.Checked = this.Dat_get_Neg_Bool(this.CFG_data_Buff[0] & 2);
			this.radioButton_LDSIZE_0K.Checked = false;
			this.radioButton_LDSIZE_1K.Checked = false;
			this.radioButton_LDSIZE_2K.Checked = false;
			this.radioButton_LDSIZE_3K.Checked = false;
			this.radioButton_LDSIZE_4K.Checked = false;
			switch (this.CFG_data_Buff[1] & 7)
			{
				case 4:
				{
					this.radioButton_LDSIZE_3K.Checked = true;
					break;
				}
				case 5:
				{
					this.radioButton_LDSIZE_2K.Checked = true;
					break;
				}
				case 6:
				{
					this.radioButton_LDSIZE_1K.Checked = true;
					break;
				}
				case 7:
				{
					this.radioButton_LDSIZE_0K.Checked = true;
					break;
				}
				default:
				{
					this.radioButton_LDSIZE_4K.Checked = true;
					break;
				}
			}
			this.checkBox_BROWN_OUT_ENABLE.Checked = this.Dat_get_Bool(this.CFG_data_Buff[2] & 128);
			this.checkBox_BROWN_OUT_IAP.Checked = this.Dat_get_Bool(this.CFG_data_Buff[2] & 8);
			this.checkBox_BROWN_OUT_RESET.Checked = this.Dat_get_Bool(this.CFG_data_Buff[2] & 4);
			this.radioButton_BOV_4V4.Checked = false;
			this.radioButton_BOV_3V7.Checked = false;
			this.radioButton_BOV_2V7.Checked = false;
			this.radioButton_BOV_2V2.Checked = false;
			int num = this.CFG_data_Buff[2] & 48;
			if (num <= 16)
			{
				if (num == 0)
				{
					this.radioButton_BOV_4V4.Checked = true;
				}
				else
				{
					if (num != 16)
					{
						this.radioButton_WDT_DISABLE.Checked = false;
						this.radioButton_WDT_ENABLE_STOP.Checked = false;
						this.radioButton_WDT_ENABLE_KEEP.Checked = false;
						cFGDataBuff = this.CFG_data_Buff[3] & 240;
						if (cFGDataBuff == 80)
						{
							this.radioButton_WDT_ENABLE_STOP.Checked = true;
						}
						else if (cFGDataBuff == 240)
						{
							this.radioButton_WDT_DISABLE.Checked = true;
						}
						else
						{
							this.radioButton_WDT_ENABLE_KEEP.Checked = true;
						}
						return;
					}
					this.radioButton_BOV_3V7.Checked = true;
				}
			}
			else if (num == 32)
			{
				this.radioButton_BOV_2V7.Checked = true;
			}
			else
			{
				if (num != 48)
				{
					this.radioButton_WDT_DISABLE.Checked = false;
					this.radioButton_WDT_ENABLE_STOP.Checked = false;
					this.radioButton_WDT_ENABLE_KEEP.Checked = false;
					cFGDataBuff = this.CFG_data_Buff[3] & 240;
					if (cFGDataBuff == 80)
					{
						this.radioButton_WDT_ENABLE_STOP.Checked = true;
					}
					else if (cFGDataBuff == 240)
					{
						this.radioButton_WDT_DISABLE.Checked = true;
					}
					else
					{
						this.radioButton_WDT_ENABLE_KEEP.Checked = true;
					}
					return;
				}
				this.radioButton_BOV_2V2.Checked = true;
			}
			this.radioButton_WDT_DISABLE.Checked = false;
			this.radioButton_WDT_ENABLE_STOP.Checked = false;
			this.radioButton_WDT_ENABLE_KEEP.Checked = false;
			cFGDataBuff = this.CFG_data_Buff[3] & 240;
			if (cFGDataBuff == 80)
			{
				this.radioButton_WDT_ENABLE_STOP.Checked = true;
			}
			else if (cFGDataBuff == 240)
			{
				this.radioButton_WDT_DISABLE.Checked = true;
			}
			else
			{
				this.radioButton_WDT_ENABLE_KEEP.Checked = true;
			}
			return;
			this.radioButton_WDT_DISABLE.Checked = false;
			this.radioButton_WDT_ENABLE_STOP.Checked = false;
			this.radioButton_WDT_ENABLE_KEEP.Checked = false;
			cFGDataBuff = this.CFG_data_Buff[3] & 240;
			if (cFGDataBuff == 80)
			{
				this.radioButton_WDT_ENABLE_STOP.Checked = true;
			}
			else if (cFGDataBuff == 240)
			{
				this.radioButton_WDT_DISABLE.Checked = true;
			}
			else
			{
				this.radioButton_WDT_ENABLE_KEEP.Checked = true;
			}
		}

		private bool Dat_get_Bool(int data)
		{
			return (data != 0 ? true : false);
		}

		private bool Dat_get_Neg_Bool(int data)
		{
			return (data != 0 ? false : true);
		}

		protected override void Dispose(bool disposing)
		{
			if ((!disposing ? false : this.components != null))
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void Form_cfg_MousClick(object sender, MouseEventArgs e)
		{
			this.GUIToCFG();
			this.ConfigToGUI();
		}

		private void GUIToCFG()
		{
			this.CFG_data_Buff[0] = 73;
			this.CFG_data_Buff[1] = 248;
			this.CFG_data_Buff[2] = 67;
			this.CFG_data_Buff[3] = 15;
			if (this.radioButton_BS_APROM.Checked)
			{
				ref byte cFGDataBuff = ref this.CFG_data_Buff[0];
				cFGDataBuff = (byte)(cFGDataBuff | 128);
			}
			if (this.radioButton_OCDPWM_TRI.Checked)
			{
				ref byte numPointer = ref this.CFG_data_Buff[0];
				numPointer = (byte)(numPointer | 32);
			}
			if (!this.checkBox_OCD_ENABLE.Checked)
			{
				ref byte cFGDataBuff1 = ref this.CFG_data_Buff[0];
				cFGDataBuff1 = (byte)(cFGDataBuff1 | 16);
			}
			if (this.radioButton_RPD_RESET.Checked)
			{
				ref byte numPointer1 = ref this.CFG_data_Buff[0];
				numPointer1 = (byte)(numPointer1 | 4);
			}
			if (!this.checkBox_SECURITY_LOCK.Checked)
			{
				ref byte cFGDataBuff2 = ref this.CFG_data_Buff[0];
				cFGDataBuff2 = (byte)(cFGDataBuff2 | 2);
			}
			if (this.radioButton_LDSIZE_0K.Checked)
			{
				ref byte numPointer2 = ref this.CFG_data_Buff[1];
				numPointer2 = (byte)(numPointer2 | 7);
			}
			if (this.radioButton_LDSIZE_1K.Checked)
			{
				ref byte cFGDataBuff3 = ref this.CFG_data_Buff[1];
				cFGDataBuff3 = (byte)(cFGDataBuff3 | 6);
			}
			if (this.radioButton_LDSIZE_2K.Checked)
			{
				ref byte numPointer3 = ref this.CFG_data_Buff[1];
				numPointer3 = (byte)(numPointer3 | 5);
			}
			if (this.radioButton_LDSIZE_3K.Checked)
			{
				ref byte cFGDataBuff4 = ref this.CFG_data_Buff[1];
				cFGDataBuff4 = (byte)(cFGDataBuff4 | 4);
			}
			if (this.checkBox_BROWN_OUT_ENABLE.Checked)
			{
				ref byte numPointer4 = ref this.CFG_data_Buff[2];
				numPointer4 = (byte)(numPointer4 | 128);
			}
			if (this.checkBox_BROWN_OUT_IAP.Checked)
			{
				ref byte cFGDataBuff5 = ref this.CFG_data_Buff[2];
				cFGDataBuff5 = (byte)(cFGDataBuff5 | 8);
			}
			if (this.checkBox_BROWN_OUT_RESET.Checked)
			{
				ref byte numPointer5 = ref this.CFG_data_Buff[2];
				numPointer5 = (byte)(numPointer5 | 4);
			}
			if (this.radioButton_BOV_3V7.Checked)
			{
				ref byte cFGDataBuff6 = ref this.CFG_data_Buff[2];
				cFGDataBuff6 = (byte)(cFGDataBuff6 | 16);
			}
			if (this.radioButton_BOV_2V7.Checked)
			{
				ref byte numPointer6 = ref this.CFG_data_Buff[2];
				numPointer6 = (byte)(numPointer6 | 32);
			}
			if (this.radioButton_BOV_2V2.Checked)
			{
				ref byte cFGDataBuff7 = ref this.CFG_data_Buff[2];
				cFGDataBuff7 = (byte)(cFGDataBuff7 | 48);
			}
			if (this.radioButton_WDT_DISABLE.Checked)
			{
				ref byte numPointer7 = ref this.CFG_data_Buff[3];
				numPointer7 = (byte)(numPointer7 | 240);
			}
			if (this.radioButton_WDT_ENABLE_STOP.Checked)
			{
				ref byte cFGDataBuff8 = ref this.CFG_data_Buff[3];
				cFGDataBuff8 = (byte)(cFGDataBuff8 | 80);
			}
			this.Refresh_config_byte();
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Form_cfg));
			this.groupBox1 = new GroupBox();
			this.radioButton_BS_LDROM = new RadioButton();
			this.radioButton_BS_APROM = new RadioButton();
			this.groupBox2 = new GroupBox();
			this.radioButton_LDSIZE_4K = new RadioButton();
			this.radioButton_LDSIZE_2K = new RadioButton();
			this.radioButton_LDSIZE_3K = new RadioButton();
			this.radioButton_LDSIZE_1K = new RadioButton();
			this.radioButton_LDSIZE_0K = new RadioButton();
			this.checkBox_SECURITY_LOCK = new CheckBox();
			this.checkBox_OCD_ENABLE = new CheckBox();
			this.groupBox3 = new GroupBox();
			this.radioButton_RPD_INPUT = new RadioButton();
			this.radioButton_RPD_RESET = new RadioButton();
			this.groupBox4 = new GroupBox();
			this.radioButton_OCDPWM_CONTI = new RadioButton();
			this.radioButton_OCDPWM_TRI = new RadioButton();
			this.groupBox5 = new GroupBox();
			this.checkBox_BROWN_OUT_IAP = new CheckBox();
			this.checkBox_BROWN_OUT_RESET = new CheckBox();
			this.checkBox_BROWN_OUT_ENABLE = new CheckBox();
			this.radioButton_BOV_4V4 = new RadioButton();
			this.radioButton_BOV_3V7 = new RadioButton();
			this.radioButton_BOV_2V7 = new RadioButton();
			this.radioButton_BOV_2V2 = new RadioButton();
			this.groupBox6 = new GroupBox();
			this.radioButton_WDT_ENABLE_KEEP = new RadioButton();
			this.radioButton_WDT_ENABLE_STOP = new RadioButton();
			this.radioButton_WDT_DISABLE = new RadioButton();
			this.comboBox_ADDR = new ComboBox();
			this.button_OK = new Button();
			this.button_Cancel = new Button();
			this.label6 = new Label();
			this.groupBox7 = new GroupBox();
			this.label9 = new Label();
			this.textBox_CFG0 = new TextBox();
			this.label8 = new Label();
			this.textBox_CFG1 = new TextBox();
			this.label7 = new Label();
			this.textBox_CFG2 = new TextBox();
			this.label1 = new Label();
			this.textBox_CFG4 = new TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox7.SuspendLayout();
			base.SuspendLayout();
			this.groupBox1.Controls.Add(this.radioButton_BS_LDROM);
			this.groupBox1.Controls.Add(this.radioButton_BS_APROM);
			this.groupBox1.Location = new Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(171, 80);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Boot Options";
			this.radioButton_BS_LDROM.AutoSize = true;
			this.radioButton_BS_LDROM.Location = new Point(10, 50);
			this.radioButton_BS_LDROM.Name = "radioButton_BS_LDROM";
			this.radioButton_BS_LDROM.Size = new System.Drawing.Size(68, 19);
			this.radioButton_BS_LDROM.TabIndex = 1;
			this.radioButton_BS_LDROM.TabStop = true;
			this.radioButton_BS_LDROM.Text = "LDROM";
			this.radioButton_BS_LDROM.UseVisualStyleBackColor = true;
			this.radioButton_BS_APROM.AutoSize = true;
			this.radioButton_BS_APROM.Location = new Point(10, 25);
			this.radioButton_BS_APROM.Name = "radioButton_BS_APROM";
			this.radioButton_BS_APROM.Size = new System.Drawing.Size(68, 19);
			this.radioButton_BS_APROM.TabIndex = 0;
			this.radioButton_BS_APROM.TabStop = true;
			this.radioButton_BS_APROM.Text = "APROM";
			this.radioButton_BS_APROM.UseVisualStyleBackColor = true;
			this.groupBox2.Controls.Add(this.radioButton_LDSIZE_4K);
			this.groupBox2.Controls.Add(this.radioButton_LDSIZE_2K);
			this.groupBox2.Controls.Add(this.radioButton_LDSIZE_3K);
			this.groupBox2.Controls.Add(this.radioButton_LDSIZE_1K);
			this.groupBox2.Controls.Add(this.radioButton_LDSIZE_0K);
			this.groupBox2.Location = new Point(12, 98);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(171, 154);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "LDROM Size Options";
			this.radioButton_LDSIZE_4K.AutoSize = true;
			this.radioButton_LDSIZE_4K.Location = new Point(10, 125);
			this.radioButton_LDSIZE_4K.Name = "radioButton_LDSIZE_4K";
			this.radioButton_LDSIZE_4K.Size = new System.Drawing.Size(116, 19);
			this.radioButton_LDSIZE_4K.TabIndex = 4;
			this.radioButton_LDSIZE_4K.TabStop = true;
			this.radioButton_LDSIZE_4K.Text = "LDROM = 4KB";
			this.radioButton_LDSIZE_4K.UseVisualStyleBackColor = true;
			this.radioButton_LDSIZE_2K.AutoSize = true;
			this.radioButton_LDSIZE_2K.Location = new Point(10, 75);
			this.radioButton_LDSIZE_2K.Name = "radioButton_LDSIZE_2K";
			this.radioButton_LDSIZE_2K.Size = new System.Drawing.Size(116, 19);
			this.radioButton_LDSIZE_2K.TabIndex = 2;
			this.radioButton_LDSIZE_2K.TabStop = true;
			this.radioButton_LDSIZE_2K.Text = "LDROM = 2KB";
			this.radioButton_LDSIZE_2K.UseVisualStyleBackColor = true;
			this.radioButton_LDSIZE_3K.AutoSize = true;
			this.radioButton_LDSIZE_3K.Location = new Point(10, 99);
			this.radioButton_LDSIZE_3K.Name = "radioButton_LDSIZE_3K";
			this.radioButton_LDSIZE_3K.Size = new System.Drawing.Size(116, 19);
			this.radioButton_LDSIZE_3K.TabIndex = 3;
			this.radioButton_LDSIZE_3K.TabStop = true;
			this.radioButton_LDSIZE_3K.Text = "LDROM = 3KB";
			this.radioButton_LDSIZE_3K.UseVisualStyleBackColor = true;
			this.radioButton_LDSIZE_1K.AutoSize = true;
			this.radioButton_LDSIZE_1K.Location = new Point(10, 50);
			this.radioButton_LDSIZE_1K.Name = "radioButton_LDSIZE_1K";
			this.radioButton_LDSIZE_1K.Size = new System.Drawing.Size(116, 19);
			this.radioButton_LDSIZE_1K.TabIndex = 1;
			this.radioButton_LDSIZE_1K.TabStop = true;
			this.radioButton_LDSIZE_1K.Text = "LDROM = 1KB";
			this.radioButton_LDSIZE_1K.UseVisualStyleBackColor = true;
			this.radioButton_LDSIZE_0K.AutoSize = true;
			this.radioButton_LDSIZE_0K.Location = new Point(10, 25);
			this.radioButton_LDSIZE_0K.Name = "radioButton_LDSIZE_0K";
			this.radioButton_LDSIZE_0K.Size = new System.Drawing.Size(92, 19);
			this.radioButton_LDSIZE_0K.TabIndex = 0;
			this.radioButton_LDSIZE_0K.TabStop = true;
			this.radioButton_LDSIZE_0K.Text = "No LDROM";
			this.radioButton_LDSIZE_0K.UseVisualStyleBackColor = true;
			this.checkBox_SECURITY_LOCK.AutoSize = true;
			this.checkBox_SECURITY_LOCK.Location = new Point(200, 12);
			this.checkBox_SECURITY_LOCK.Name = "checkBox_SECURITY_LOCK";
			this.checkBox_SECURITY_LOCK.Size = new System.Drawing.Size(133, 19);
			this.checkBox_SECURITY_LOCK.TabIndex = 2;
			this.checkBox_SECURITY_LOCK.Text = "Security Lock";
			this.checkBox_SECURITY_LOCK.UseVisualStyleBackColor = true;
			this.checkBox_OCD_ENABLE.AutoSize = true;
			this.checkBox_OCD_ENABLE.Location = new Point(200, 37);
			this.checkBox_OCD_ENABLE.Name = "checkBox_OCD_ENABLE";
			this.checkBox_OCD_ENABLE.Size = new System.Drawing.Size(109, 19);
			this.checkBox_OCD_ENABLE.TabIndex = 3;
			this.checkBox_OCD_ENABLE.Text = "OCD Enable";
			this.checkBox_OCD_ENABLE.UseVisualStyleBackColor = true;
			this.groupBox3.Controls.Add(this.radioButton_RPD_INPUT);
			this.groupBox3.Controls.Add(this.radioButton_RPD_RESET);
			this.groupBox3.Location = new Point(189, 92);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(198, 74);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "P2.0/RST Pin Function";
			this.radioButton_RPD_INPUT.AutoSize = true;
			this.radioButton_RPD_INPUT.Location = new Point(10, 50);
			this.radioButton_RPD_INPUT.Name = "radioButton_RPD_INPUT";
			this.radioButton_RPD_INPUT.Size = new System.Drawing.Size(140, 19);
			this.radioButton_RPD_INPUT.TabIndex = 1;
			this.radioButton_RPD_INPUT.TabStop = true;
			this.radioButton_RPD_INPUT.Text = "input-only pin";
			this.radioButton_RPD_INPUT.UseVisualStyleBackColor = true;
			this.radioButton_RPD_RESET.AutoSize = true;
			this.radioButton_RPD_RESET.Location = new Point(10, 25);
			this.radioButton_RPD_RESET.Name = "radioButton_RPD_RESET";
			this.radioButton_RPD_RESET.Size = new System.Drawing.Size(100, 19);
			this.radioButton_RPD_RESET.TabIndex = 0;
			this.radioButton_RPD_RESET.TabStop = true;
			this.radioButton_RPD_RESET.Text = "reset pin";
			this.radioButton_RPD_RESET.UseVisualStyleBackColor = true;
			this.groupBox4.Controls.Add(this.radioButton_OCDPWM_CONTI);
			this.groupBox4.Controls.Add(this.radioButton_OCDPWM_TRI);
			this.groupBox4.Location = new Point(189, 173);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(198, 79);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "PWM state under OCD";
			this.radioButton_OCDPWM_CONTI.AutoSize = true;
			this.radioButton_OCDPWM_CONTI.Location = new Point(10, 51);
			this.radioButton_OCDPWM_CONTI.Name = "radioButton_OCDPWM_CONTI";
			this.radioButton_OCDPWM_CONTI.Size = new System.Drawing.Size(132, 19);
			this.radioButton_OCDPWM_CONTI.TabIndex = 1;
			this.radioButton_OCDPWM_CONTI.TabStop = true;
			this.radioButton_OCDPWM_CONTI.Text = "PWM continues";
			this.radioButton_OCDPWM_CONTI.UseVisualStyleBackColor = true;
			this.radioButton_OCDPWM_TRI.AutoSize = true;
			this.radioButton_OCDPWM_TRI.Location = new Point(10, 26);
			this.radioButton_OCDPWM_TRI.Name = "radioButton_OCDPWM_TRI";
			this.radioButton_OCDPWM_TRI.Size = new System.Drawing.Size(100, 19);
			this.radioButton_OCDPWM_TRI.TabIndex = 0;
			this.radioButton_OCDPWM_TRI.TabStop = true;
			this.radioButton_OCDPWM_TRI.Text = "Tri-state";
			this.radioButton_OCDPWM_TRI.UseVisualStyleBackColor = true;
			this.groupBox5.Controls.Add(this.checkBox_BROWN_OUT_IAP);
			this.groupBox5.Controls.Add(this.checkBox_BROWN_OUT_RESET);
			this.groupBox5.Controls.Add(this.checkBox_BROWN_OUT_ENABLE);
			this.groupBox5.Controls.Add(this.radioButton_BOV_4V4);
			this.groupBox5.Controls.Add(this.radioButton_BOV_3V7);
			this.groupBox5.Controls.Add(this.radioButton_BOV_2V7);
			this.groupBox5.Controls.Add(this.radioButton_BOV_2V2);
			this.groupBox5.Location = new Point(12, 258);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(375, 80);
			this.groupBox5.TabIndex = 6;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Brown-out Detector Options";
			this.checkBox_BROWN_OUT_IAP.AutoSize = true;
			this.checkBox_BROWN_OUT_IAP.Location = new Point(207, 50);
			this.checkBox_BROWN_OUT_IAP.Name = "checkBox_BROWN_OUT_IAP";
			this.checkBox_BROWN_OUT_IAP.Size = new System.Drawing.Size(141, 19);
			this.checkBox_BROWN_OUT_IAP.TabIndex = 7;
			this.checkBox_BROWN_OUT_IAP.Text = "BOD forbid IAP";
			this.checkBox_BROWN_OUT_IAP.UseVisualStyleBackColor = true;
			this.checkBox_BROWN_OUT_RESET.AutoSize = true;
			this.checkBox_BROWN_OUT_RESET.Location = new Point(94, 50);
			this.checkBox_BROWN_OUT_RESET.Name = "checkBox_BROWN_OUT_RESET";
			this.checkBox_BROWN_OUT_RESET.Size = new System.Drawing.Size(109, 19);
			this.checkBox_BROWN_OUT_RESET.TabIndex = 6;
			this.checkBox_BROWN_OUT_RESET.Text = "BOD_RST_EN";
			this.checkBox_BROWN_OUT_RESET.UseVisualStyleBackColor = true;
			this.checkBox_BROWN_OUT_ENABLE.AutoSize = true;
			this.checkBox_BROWN_OUT_ENABLE.Location = new Point(6, 50);
			this.checkBox_BROWN_OUT_ENABLE.Name = "checkBox_BROWN_OUT_ENABLE";
			this.checkBox_BROWN_OUT_ENABLE.Size = new System.Drawing.Size(77, 19);
			this.checkBox_BROWN_OUT_ENABLE.TabIndex = 5;
			this.checkBox_BROWN_OUT_ENABLE.Text = "BOD_EN";
			this.checkBox_BROWN_OUT_ENABLE.UseVisualStyleBackColor = true;
			this.radioButton_BOV_4V4.AutoSize = true;
			this.radioButton_BOV_4V4.Location = new Point(283, 25);
			this.radioButton_BOV_4V4.Name = "radioButton_BOV_4V4";
			this.radioButton_BOV_4V4.Size = new System.Drawing.Size(60, 19);
			this.radioButton_BOV_4V4.TabIndex = 3;
			this.radioButton_BOV_4V4.TabStop = true;
			this.radioButton_BOV_4V4.Text = "4.4V";
			this.radioButton_BOV_4V4.UseVisualStyleBackColor = true;
			this.radioButton_BOV_3V7.AutoSize = true;
			this.radioButton_BOV_3V7.Location = new Point(187, 25);
			this.radioButton_BOV_3V7.Name = "radioButton_BOV_3V7";
			this.radioButton_BOV_3V7.Size = new System.Drawing.Size(60, 19);
			this.radioButton_BOV_3V7.TabIndex = 2;
			this.radioButton_BOV_3V7.TabStop = true;
			this.radioButton_BOV_3V7.Text = "3.7V";
			this.radioButton_BOV_3V7.UseVisualStyleBackColor = true;
			this.radioButton_BOV_2V7.AutoSize = true;
			this.radioButton_BOV_2V7.Location = new Point(94, 25);
			this.radioButton_BOV_2V7.Name = "radioButton_BOV_2V7";
			this.radioButton_BOV_2V7.Size = new System.Drawing.Size(60, 19);
			this.radioButton_BOV_2V7.TabIndex = 1;
			this.radioButton_BOV_2V7.TabStop = true;
			this.radioButton_BOV_2V7.Text = "2.7V";
			this.radioButton_BOV_2V7.UseVisualStyleBackColor = true;
			this.radioButton_BOV_2V2.AutoSize = true;
			this.radioButton_BOV_2V2.Location = new Point(7, 25);
			this.radioButton_BOV_2V2.Name = "radioButton_BOV_2V2";
			this.radioButton_BOV_2V2.Size = new System.Drawing.Size(60, 19);
			this.radioButton_BOV_2V2.TabIndex = 0;
			this.radioButton_BOV_2V2.TabStop = true;
			this.radioButton_BOV_2V2.Text = "2.2V";
			this.radioButton_BOV_2V2.UseVisualStyleBackColor = true;
			this.groupBox6.Controls.Add(this.radioButton_WDT_ENABLE_KEEP);
			this.groupBox6.Controls.Add(this.radioButton_WDT_ENABLE_STOP);
			this.groupBox6.Controls.Add(this.radioButton_WDT_DISABLE);
			this.groupBox6.Location = new Point(12, 344);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(375, 103);
			this.groupBox6.TabIndex = 7;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "WatchDog Timer Options";
			this.radioButton_WDT_ENABLE_KEEP.AutoSize = true;
			this.radioButton_WDT_ENABLE_KEEP.Location = new Point(6, 75);
			this.radioButton_WDT_ENABLE_KEEP.Name = "radioButton_WDT_ENABLE_KEEP";
			this.radioButton_WDT_ENABLE_KEEP.Size = new System.Drawing.Size(364, 19);
			this.radioButton_WDT_ENABLE_KEEP.TabIndex = 2;
			this.radioButton_WDT_ENABLE_KEEP.TabStop = true;
			this.radioButton_WDT_ENABLE_KEEP.Text = "Enable and keep running in Idle/Power-down";
			this.radioButton_WDT_ENABLE_KEEP.UseVisualStyleBackColor = true;
			this.radioButton_WDT_ENABLE_STOP.AutoSize = true;
			this.radioButton_WDT_ENABLE_STOP.Location = new Point(6, 50);
			this.radioButton_WDT_ENABLE_STOP.Name = "radioButton_WDT_ENABLE_STOP";
			this.radioButton_WDT_ENABLE_STOP.Size = new System.Drawing.Size(300, 19);
			this.radioButton_WDT_ENABLE_STOP.TabIndex = 1;
			this.radioButton_WDT_ENABLE_STOP.TabStop = true;
			this.radioButton_WDT_ENABLE_STOP.Text = "Enable and stop in Idle/Power-down";
			this.radioButton_WDT_ENABLE_STOP.UseVisualStyleBackColor = true;
			this.radioButton_WDT_DISABLE.AutoSize = true;
			this.radioButton_WDT_DISABLE.Location = new Point(6, 25);
			this.radioButton_WDT_DISABLE.Name = "radioButton_WDT_DISABLE";
			this.radioButton_WDT_DISABLE.Size = new System.Drawing.Size(84, 19);
			this.radioButton_WDT_DISABLE.TabIndex = 0;
			this.radioButton_WDT_DISABLE.TabStop = true;
			this.radioButton_WDT_DISABLE.Text = "Disable";
			this.radioButton_WDT_DISABLE.UseVisualStyleBackColor = true;
			this.comboBox_ADDR.FormattingEnabled = true;
			this.comboBox_ADDR.Location = new Point(307, 62);
			this.comboBox_ADDR.Name = "comboBox_ADDR";
			this.comboBox_ADDR.Size = new System.Drawing.Size(75, 23);
			this.comboBox_ADDR.TabIndex = 8;
			this.comboBox_ADDR.KeyPress += new KeyPressEventHandler(this.comboBox_ADDR_KeyPress);
			this.button_OK.Location = new Point(106, 517);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 30);
			this.button_OK.TabIndex = 9;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new EventHandler(this.button_OK_Click);
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new Point(219, 517);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 30);
			this.button_Cancel.TabIndex = 10;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new EventHandler(this.button_Cancel_Click);
			this.label6.AutoSize = true;
			this.label6.Location = new Point(196, 66);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(111, 15);
			this.label6.TabIndex = 78;
			this.label6.Text = "LDROM addr 0x";
			this.groupBox7.Controls.Add(this.label9);
			this.groupBox7.Controls.Add(this.textBox_CFG0);
			this.groupBox7.Controls.Add(this.label8);
			this.groupBox7.Controls.Add(this.textBox_CFG1);
			this.groupBox7.Controls.Add(this.label7);
			this.groupBox7.Controls.Add(this.textBox_CFG2);
			this.groupBox7.Controls.Add(this.label1);
			this.groupBox7.Controls.Add(this.textBox_CFG4);
			this.groupBox7.Location = new Point(9, 453);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(369, 58);
			this.groupBox7.TabIndex = 79;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Config Value";
			this.label9.AutoSize = true;
			this.label9.Location = new Point(283, 25);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(15, 15);
			this.label9.TabIndex = 88;
			this.label9.Text = "4";
			this.textBox_CFG0.Location = new Point(26, 22);
			this.textBox_CFG0.Name = "textBox_CFG0";
			this.textBox_CFG0.ReadOnly = true;
			this.textBox_CFG0.Size = new System.Drawing.Size(60, 25);
			this.textBox_CFG0.TabIndex = 81;
			this.label8.AutoSize = true;
			this.label8.Location = new Point(187, 25);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(15, 15);
			this.label8.TabIndex = 87;
			this.label8.Text = "2";
			this.textBox_CFG1.Location = new Point(115, 22);
			this.textBox_CFG1.Name = "textBox_CFG1";
			this.textBox_CFG1.ReadOnly = true;
			this.textBox_CFG1.Size = new System.Drawing.Size(60, 25);
			this.textBox_CFG1.TabIndex = 82;
			this.label7.AutoSize = true;
			this.label7.Location = new Point(94, 25);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(15, 15);
			this.label7.TabIndex = 86;
			this.label7.Text = "1";
			this.textBox_CFG2.Location = new Point(210, 22);
			this.textBox_CFG2.Name = "textBox_CFG2";
			this.textBox_CFG2.ReadOnly = true;
			this.textBox_CFG2.Size = new System.Drawing.Size(60, 25);
			this.textBox_CFG2.TabIndex = 83;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(6, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(15, 15);
			this.label1.TabIndex = 85;
			this.label1.Text = "0";
			this.textBox_CFG4.Location = new Point(303, 22);
			this.textBox_CFG4.Name = "textBox_CFG4";
			this.textBox_CFG4.ReadOnly = true;
			this.textBox_CFG4.Size = new System.Drawing.Size(60, 25);
			this.textBox_CFG4.TabIndex = 84;
			base.AcceptButton = this.button_OK;
			base.AutoScaleDimensions = new SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.button_Cancel;
			base.ClientSize = new System.Drawing.Size(399, 559);
			base.Controls.Add(this.groupBox7);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.button_Cancel);
			base.Controls.Add(this.button_OK);
			base.Controls.Add(this.comboBox_ADDR);
			base.Controls.Add(this.groupBox6);
			base.Controls.Add(this.groupBox5);
			base.Controls.Add(this.groupBox4);
			base.Controls.Add(this.groupBox3);
			base.Controls.Add(this.checkBox_OCD_ENABLE);
			base.Controls.Add(this.checkBox_SECURITY_LOCK);
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.groupBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Form_cfg";
			this.Text = "Chip Options";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void LDROM_MousClick(object sender, MouseEventArgs e)
		{
			if (this.radioButton_LDSIZE_0K.Checked)
			{
				this.comboBox_ADDR.Text = this.LDROM_ADDR.ToString("X4");
			}
			if (this.radioButton_LDSIZE_1K.Checked)
			{
				this.comboBox_ADDR.Text = "4400";
			}
			if (this.radioButton_LDSIZE_2K.Checked)
			{
				this.comboBox_ADDR.Text = "4000";
			}
			if (this.radioButton_LDSIZE_3K.Checked)
			{
				this.comboBox_ADDR.Text = "3C00";
			}
			if (this.radioButton_LDSIZE_4K.Checked)
			{
				this.comboBox_ADDR.Text = "3800";
			}
		}

		private void Refresh_config_byte()
		{
			this.textBox_CFG0.Text = string.Concat("0x", this.CFG_data_Buff[0].ToString("X2"));
			this.textBox_CFG1.Text = string.Concat("0x", this.CFG_data_Buff[1].ToString("X2"));
			this.textBox_CFG2.Text = string.Concat("0x", this.CFG_data_Buff[2].ToString("X2"));
			this.textBox_CFG4.Text = string.Concat("0x", this.CFG_data_Buff[3].ToString("X2"));
		}

		public delegate void Get_CFG_Value(byte[] cfg_data, ushort ld_addr);
	}
}