using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ClassyPhotos
{
    internal class MainProcess
    {
        public int RunProcess(string[] args)
        {
            bool install = false;
            bool uninstall = false;
            bool rethrow = false;

            try
            {
                foreach (string arg in args)
                {
                    string argValue = arg.ToLower();

                    switch (argValue)
                    {
                        case "-i":
                        case "-install":
                            install = true;
                            break;
                        case "-u":
                        case "-uninstall":
                            uninstall = true;
                            break;
                        default:
                            Console.Error.WriteLine("Argument not expected: " + arg);
                            break;
                    }
                }

                if (uninstall)
                {
                    ClassicServiceInstaller.Install(true, args);
                }
                if (install)
                {
                    ClassicServiceInstaller.Install(false, args);
                }
                else
                {
                    rethrow = true; // so that windows sees error...

                    ServiceBase[] services = { new ClassicService() };
                    ServiceBase.Run(services);

                    rethrow = false;
                }

                return 0;
            }
            catch (Exception ex)
            {
                if (rethrow)
                {
                    throw;
                }

                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
