using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster {
	static class PuppetMaster {
		private static List<List<string>> operatorList = new List<List<string>>();
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new PuppetMasterForm());
		}
	}
}
