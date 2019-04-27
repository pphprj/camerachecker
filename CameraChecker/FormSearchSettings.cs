using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CameraCheckerApp
{
    public partial class FormSearchSettings : Form
    {
        public FormSearchSettings()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public String Login
        {
            get { return textBoxLogin.Text; }
        }

        public String Password
        {
            get { return textBoxPassword.Text; }
        }
    }
}
