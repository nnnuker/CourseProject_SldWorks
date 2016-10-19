using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SldWorksLogic;

namespace WindowsForms
{
    public partial class Form1 : Form
    {
        private SldWorksWorker worker;

        public Form1()
        {
            InitializeComponent();

            InitSldWorker();
        }

        private void InitSldWorker()
        {
            try
            {
                worker = new SldWorksWorker();
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (InvalidComObjectException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadElementsButton_Click(object sender, EventArgs e)
        {
            worker?.AddUnits();
        }

        private void createMates_Click(object sender, EventArgs e)
        {
            worker?.CreateMates();
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            InitSldWorker();
        }
    }
}
