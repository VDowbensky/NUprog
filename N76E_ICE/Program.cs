using System;
using System.Windows.Forms;

namespace N76E_ICE
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Main_Form());
		}
	}
}