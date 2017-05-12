using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ClassyPhotos
{
    [RunInstaller(true)]
    partial class ClassicServiceInstaller : ServiceInstaller
    {
        public ClassicServiceInstaller()
        {
            this.Description = "Manages Wallpaper Properties";
            this.DisplayName = "Classy.Photos.Service";
            this.ServiceName = "Classy.Photos.Service";
            this.StartType = ServiceStartMode.Automatic;
        }

        public static void Install(bool undo, string[] args)
        {
            try
            {
                Console.WriteLine(undo ? "Uninstalling" : "Installing");
                using (AssemblyInstaller inst = new AssemblyInstaller(typeof(Program).Assembly, args))
                {
                    IDictionary state = new Hashtable();
                    inst.UseNewContext = true;
                    try
                    {
                        if (undo)
                        {
                            inst.Uninstall(state);
                        }
                        else
                        {
                            inst.Install(state);
                            inst.Commit(state);
                        }
                    }
                    catch
                    {
                        try
                        {
                            inst.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
    }
}
