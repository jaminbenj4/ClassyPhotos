using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ClassyPhotos
{
    [RunInstaller(true)]
    partial class ClassicServiceInstallerProcess : ServiceProcessInstaller
    {
        public ClassicServiceInstallerProcess()
        {
            this.Account = ServiceAccount.User;
        }
    }
}
