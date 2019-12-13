using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace File_NSP
{
	internal class file_loader
	{
		public file_loader()
		{
		}

		public bool Check_File_Path(string path_text)
		{
			bool flag;
			if (path_text == string.Empty)
			{
				MessageBox.Show("请先选择文件路径！", "错误提示");
				flag = false;
			}
			else if (!(new Regex("^([a-zA-Z]:\\\\)?[^\\/\\:\\*\\?\\\"\\<\\>\\|\\,]*$")).Match(path_text).Success)
			{
				MessageBox.Show("请勿在文件名中包含\\ / : * ？ \" < > |等字符，请重新输入有效文件名！", "错误提示");
				flag = false;
			}
			else if (Path.GetFileName(path_text) == "")
			{
				MessageBox.Show("这是一个文件夹！", "错误提示");
				flag = false;
			}
			else if (!Directory.Exists(path_text))
			{
				string extension = Path.GetExtension(path_text);
				if ((extension == ".bin" || extension == ".BIN" || extension == ".hex" ? false : extension != ".HEX"))
				{
					MessageBox.Show("不支持的文件格式", "错误提示");
					flag = false;
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				MessageBox.Show("这是一个文件夹！", "错误提示");
				flag = false;
			}
			return flag;
		}

		private static byte Hex2Bin(char[] p, ushort i)
		{
			byte binBinChar = 0;
			binBinChar = file_loader.HexCharToBinBinChar(p[i]);
			binBinChar = (byte)(binBinChar << 4);
			binBinChar = (byte)(binBinChar | file_loader.HexCharToBinBinChar(p[1 + i]));
			return binBinChar;
		}

		private static byte HexByte_2Bin(byte[] p, uint i)
		{
			byte binBinChar = 0;
			binBinChar = file_loader.HexCharToBinBinChar(p[i]);
			binBinChar = (byte)(binBinChar << 4);
			binBinChar = (byte)(binBinChar | file_loader.HexCharToBinBinChar(p[1 + i]));
			return binBinChar;
		}

		private static byte HexCharToBinBinChar(char c)
		{
			byte num;
			if ((c < '0' ? false : c <= '9'))
			{
				num = (byte)(c - 48);
			}
			else if ((c < 'a' ? false : c <= 'z'))
			{
				num = (byte)(c - 97 + 10);
			}
			else if ((c < 'A' ? true : c > 'Z'))
			{
				num = 255;
			}
			else
			{
				num = (byte)(c - 65 + 10);
			}
			return num;
		}

		private static bool HexFormatUncode(char[] src, ref file_loader.BinFarmat p)
		{
			bool flag;
			byte num = 0;
			byte[] numArray = new byte[4];
			ushort length = (ushort)((int)src.Length);
			ushort num1 = 0;
			ushort num2 = 0;
			if (length > 512)
			{
				flag = false;
			}
			else if (length < 11)
			{
				flag = false;
			}
			else if (src[0] != ':')
			{
				flag = false;
			}
			else if ((length - 1) % 2 == 0)
			{
				byte num3 = (byte)((length - 1) / 2);
				while (num1 < 4)
				{
					num2 = (ushort)((num1 << 1) + 1);
					numArray[num1] = file_loader.Hex2Bin(src, num2);
					num = (byte)(num + numArray[num1]);
					num1 = (ushort)(num1 + 1);
				}
				p.len = numArray[0];
				p.addr = numArray[1];
				ref ushort numPointer = ref p.addr;
				numPointer = (ushort)(numPointer << 8);
				ref ushort numPointer1 = ref p.addr;
				numPointer1 = (ushort)(numPointer1 + numArray[2]);
				p.type = numArray[3];
				while (num1 < num3)
				{
					num2 = (ushort)((num1 << 1) + 1);
					p.data[num1 - 4] = file_loader.Hex2Bin(src, num2);
					num = (byte)(num + p.data[num1 - 4]);
					num1 = (ushort)(num1 + 1);
				}
				if (p.len == num3 - 5)
				{
					flag = (num == 0 ? true : false);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public string Str_Ascii_to_HEX(string input_str)
		{
			byte[] bytes = Encoding.Default.GetBytes(input_str);
			string str = "";
			for (uint i = 0; (ulong)i < (long)bytes.GetLength(0); i++)
			{
				str = string.Concat(str, bytes[i].ToString("X2"), " ");
			}
			return str;
		}

		public string Str_HEX_to_Ascii(string input_str)
		{
			string str;
			byte[] numArray = this.Str_HEX_to_Byte(input_str);
			if (numArray != null)
			{
				str = Encoding.Default.GetString(numArray);
			}
			else
			{
				str = null;
			}
			return str;
		}

		public byte[] Str_HEX_to_Byte(string input_str)
		{
			uint i;
			byte[] numArray;
			uint num = 0;
			byte[] bytes = Encoding.Default.GetBytes(input_str);
			byte[] numArray1 = new byte[bytes.GetLength(0)];
			for (i = 0; (ulong)i < (long)bytes.GetLength(0); i++)
			{
				if (bytes[i] != 32)
				{
					numArray1[num] = bytes[i];
					num++;
				}
			}
			if (num == 0)
			{
				numArray = null;
			}
			else if (num % 2 == 0)
			{
				byte[] numArray2 = new byte[num / 2];
				for (i = 0; i < num; i += 2)
				{
					numArray2[i / 2] = file_loader.HexByte_2Bin(numArray1, i);
				}
				numArray = numArray2;
			}
			else
			{
				MessageBox.Show("输入数量需要为偶数", "不能解析HEX输入字符串");
				numArray = null;
			}
			return numArray;
		}

		public void UpdateFile2Hex_Viewer(file_loader.fileinfo filestr, ref string Hex_Show)
		{
			if (filestr.useful)
			{
				Hex_Show = string.Concat(Hex_Show, filestr.FilenName);
				Hex_Show = string.Concat(new string[] { Hex_Show, "  大小:0x", filestr.St_Size.ToString("X4"), "(", filestr.St_Size.ToString(), ")" });
				for (uint i = 0; i < filestr.St_Size; i++)
				{
					if (i % 16 == 0)
					{
						Hex_Show = string.Concat(Hex_Show, "\r\n", i.ToString("X4"), " ");
					}
					if (i % 4 == 0)
					{
						Hex_Show = string.Concat(Hex_Show, " ");
					}
					Hex_Show = string.Concat(Hex_Show, filestr.data_buff[i].ToString("X2"), " ");
				}
			}
		}

		public bool UpdateFileInfo(ref file_loader.fileinfo filestr)
		{
			bool flag;
			filestr.useful = false;
			if (!this.Check_File_Path(filestr.FilePath))
			{
				flag = false;
			}
			else if (File.Exists(filestr.FilePath))
			{
				filestr.FilenName = Path.GetFileName(filestr.FilePath);
				string extension = Path.GetExtension(filestr.FilePath);
				if ((extension == ".bin" ? true : extension == ".BIN"))
				{
					if (!this.UpdateFileInfo_Bin(ref filestr))
					{
						flag = false;
						return flag;
					}
				}
				else if ((extension == ".hex" ? true : extension == ".HEX"))
				{
					if (!file_loader.UpdateFileInfo_Hex(ref filestr))
					{
						MessageBox.Show("不能解析HEX文件", "错误提示");
						flag = false;
						return flag;
					}
				}
				flag = true;
			}
			else
			{
				MessageBox.Show("文件不存在");
				flag = false;
			}
			return flag;
		}

		private bool UpdateFileInfo_Bin(ref file_loader.fileinfo filestr)
		{
			bool flag;
			BinaryReader binaryReader = new BinaryReader(File.Open(filestr.FilePath, FileMode.Open, FileAccess.Read));
			filestr.St_Size = (uint)binaryReader.BaseStream.Length;
			if (filestr.St_Size == 0)
			{
				MessageBox.Show("空文件", "错误提示");
				binaryReader.Close();
				binaryReader.Dispose();
				flag = false;
			}
			else if (filestr.St_Size <= 18432)
			{
				filestr.data_buff = null;
				filestr.data_buff = new byte[filestr.St_Size];
				binaryReader.Read(filestr.data_buff, 0, (int)filestr.St_Size);
				filestr.useful = true;
				flag = true;
			}
			else
			{
				MessageBox.Show("文件大于18k", "错误提示");
				binaryReader.Close();
				binaryReader.Dispose();
				flag = false;
			}
			return flag;
		}

		private static bool UpdateFileInfo_Hex(ref file_loader.fileinfo filestr)
		{
			file_loader.BinFarmat binFarmat = new file_loader.BinFarmat();
			bool flag;
			StreamReader streamReader = new StreamReader(File.Open(filestr.FilePath, FileMode.Open, FileAccess.Read));
			filestr.useful = false;
			filestr.St_Size = (uint)streamReader.BaseStream.Length;
			if (filestr.St_Size == 0)
			{
				MessageBox.Show("空文件", "错误提示");
				streamReader.Close();
				streamReader.Dispose();
				flag = false;
			}
			else if (filestr.St_Size <= 65536)
			{
				filestr.data_buff = null;
				filestr.data_buff = new byte[18432];
				filestr.St_Size = 0;
				binFarmat.data = new byte[255];
				binFarmat.addr = 0;
				binFarmat.len = 0;
				binFarmat.type = 0;
				uint num = 0;
				ushort num1 = 0;
				while (true)
				{
					string str = streamReader.ReadLine();
					if (str == null)
					{
						filestr.data_buff = null;
						streamReader.Close();
						streamReader.Dispose();
						flag = false;
						return flag;
					}
					char[] chrArray = null;
					chrArray = new char[filestr.St_Size];
					if (!file_loader.HexFormatUncode(str.ToCharArray(), ref binFarmat))
					{
						filestr.data_buff = null;
						streamReader.Close();
						streamReader.Dispose();
						flag = false;
						return flag;
					}
					switch (binFarmat.type)
					{
						case 0:
						{
							num1 = binFarmat.addr;
							if (num1 + num + binFarmat.len > 18432)
							{
								MessageBox.Show("文件地址超过MCU容量", "错误提示");
								filestr.data_buff = null;
								streamReader.Close();
								streamReader.Dispose();
								flag = false;
								return flag;
							}
							if (num1 + num + binFarmat.len > filestr.St_Size)
							{
								filestr.St_Size = num1 + num + binFarmat.len;
							}
							Array.Copy(binFarmat.data, (long)0, filestr.data_buff, (long)(num1 + num), (long)binFarmat.len);
							break;
						}
						case 1:
						{
							goto Label5;
						}
						case 2:
						{
							num = (uint)(binFarmat.addr << 2);
							break;
						}
						case 3:
						{
							filestr.data_buff = null;
							streamReader.Close();
							streamReader.Dispose();
							flag = false;
							return flag;
						}
						case 4:
						{
							num = (uint)(binFarmat.addr << 16);
							break;
						}
						case 5:
						{
							break;
						}
						default:
						{
							filestr.data_buff = null;
							streamReader.Close();
							streamReader.Dispose();
							flag = false;
							return flag;
						}
					}
				}
			Label5:
				filestr.useful = true;
				streamReader.Close();
				streamReader.Dispose();
				flag = true;
			}
			else
			{
				MessageBox.Show("Hex文件太大", "错误提示");
				streamReader.Close();
				streamReader.Dispose();
				flag = false;
			}
			return flag;
			filestr.data_buff = null;
			streamReader.Close();
			streamReader.Dispose();
			flag = false;
			return flag;
		}

		public struct BinFarmat
		{
			public byte len;

			public byte type;

			public ushort addr;

			public byte[] data;
		}

		public struct fileinfo
		{
			public string FilenName;

			public string FilePath;

			public bool useful;

			public uint St_Size;

			public byte[] data_buff;
		}
	}
}