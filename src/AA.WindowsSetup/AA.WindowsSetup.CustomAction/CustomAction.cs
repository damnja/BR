using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;

namespace AA.WindowsSetup.CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult InstallAction(Session session)
        {
			//System.Diagnostics.Debugger.Launch();
			try
            {
				session.Log("TAC driver installation has started");
				Process process = new Process();
				var location = session.CustomActionData["LOCATION"];
				var batchFile = location + "Install.bat";
				string arg = $"\"{Path.Combine(location, "blackridge.inf")}\"";

	            ProcessStartInfo proc = new ProcessStartInfo
	            {
		            FileName = batchFile,
					//CreateNoWindow = true,
					//WindowStyle = ProcessWindowStyle.Hidden,
		            Arguments = arg
	            };

	            process.StartInfo = proc;

	            if (process.Start())
	            {
		            process.WaitForExit();
		            return ActionResult.Success;
	            }

	            return ActionResult.Failure;
            }
            catch
            {
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult UninstallAction(Session session)
        {
			//System.Diagnostics.Debugger.Launch();
			try
            {
                session.Log("TAC driver unninstalation has started");
				var location = session.GetTargetPath("INSTALLFOLDER");
				var batchFile = location + "Delete.bat";

				Process proc = new Process();
                proc.StartInfo.FileName = batchFile;
                
                proc.StartInfo.Verb = "runas";

                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //proc.StartInfo.CreateNoWindow = true;
                proc.Start();

                return ActionResult.Success;
            }
            catch
            {
                return ActionResult.Failure;
            }
        }
    }
}
