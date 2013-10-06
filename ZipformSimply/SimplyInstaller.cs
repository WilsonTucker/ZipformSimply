using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace ZipformSimply
{
    [RunInstaller(true)]
    public partial class SimplyInstaller : System.Configuration.Install.Installer
    {
        public SimplyInstaller()
        {
            InitializeComponent();
        }
    }
}
