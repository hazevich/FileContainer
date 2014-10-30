using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FileContainerUI
{
    public partial class WelcomeForm : Form
    {
        public WelcomeForm()
        {
            InitializeComponent();
        }

        private void flatButton1_Click(object sender, EventArgs e)
        {
            new FileContainerEditForm().ShowDialog();
        }

        private void flatButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "container files (*.cntr)|*.cntr";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                new FileContainerEditForm(openFileDialog.FileName).ShowDialog();
            }
        }
    }
}
