using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SldWorksLogic;

namespace WindowsForms
{
    public partial class Form1 : Form
    {
        private SldWorksWorker worker = new SldWorksWorker();

        public Form1()
        {
            InitializeComponent();
        }

        private void loadElementsButton_Click(object sender, EventArgs e)
        {
            worker.AddUnits();
        }

        private void createMates_Click(object sender, EventArgs e)
        {
            worker.CreateMates();
        }
    }
}
