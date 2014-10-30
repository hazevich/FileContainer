using System;
using System.Threading;
using System.Windows.Forms;

namespace FileContainerUI.Waiter
{
    public partial class WaiterForm : Form
    {
        public WaiterForm()
        {
            InitializeComponent();
        }

        public void Show(Action job)
        {
            new Thread(new ThreadStart(() =>
            {
                job();
                this.Invoke(new MethodInvoker(() => this.Dispose())); ;
            })).Start();
            this.ShowDialog();
        }
    }
}
