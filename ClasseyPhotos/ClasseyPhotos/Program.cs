using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ClassyPhotos
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
        {
            MainProcess mainProcess = new MainProcess();

            return mainProcess.RunProcess(args);
        }
    }
}
