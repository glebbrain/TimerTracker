using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimerTracker
{
    public partial class Form2 : Form
    {
        private Point mouseOffset;
        private bool isMouseDown = false;

        public Timer tm = null;
        public int startValue = 0;
        public int alltime = 0;

        public int delit = 0;
        public int low = 0;
        public int medium = 0;
        public int hard = 0;

        public Form2()
        {
            InitializeComponent();
            tm = new Timer();
            tm.Tick += new EventHandler(tm_Tick);
            tm.Interval = 60000;
            this.Left = 10;
            this.Top = 10;

        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public void tm_Tick(object sender, EventArgs e)
        {
            startValue--;
            label1.Text = startValue.ToString();
            if (startValue == 0)
            {
                tm.Stop();
                Program.frm1.Show();
                Program.frm1.WindowState = FormWindowState.Normal;
                SetForegroundWindow(Program.frm1.Handle);
                this.Hide();

                if (Program.frm1.IsTimerRunRoundState)
                {
                    Program.frm1.button2.PerformClick();
                    Program.frm1.button6.PerformClick();
                }
            }
            else if (startValue > low)
            {
                label1.ForeColor = Color.Lime;
            }
            else if (startValue > medium)
            {
                label1.ForeColor = Color.Yellow;
            }
            else if (startValue > 0)
            {
                label1.ForeColor = Color.Red;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //this.Width = 100;
            //this.Height = 50;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tm.Stop();
            Program.frm1.Show();
            Program.frm1.WindowState = FormWindowState.Normal;
            SetForegroundWindow(Program.frm1.Handle);
            this.Hide();
            Program.frm1.button2_Click(sender, e);
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            System.Drawing.Rectangle w = Screen.PrimaryScreen.WorkingArea;
            this.SetDesktopLocation(w.Width - this.Width - 10, w.Height - this.Height - 10);
        }


        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            //base.Capture = false;
            //Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            //this.WndProc(ref m);
            int xOffset;
            int yOffset;

            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight -
                    SystemInformation.FrameBorderSize.Height;
                mouseOffset = new Point(xOffset, yOffset);
                isMouseDown = true;
            }
        }

        private void Form2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void Form2_MouseUp(object sender, MouseEventArgs e)
        {
            // Changes the isMouseDown field so that the form does
            // not move unless the user is pressing the left mouse button.
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tm.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tm.Stop();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            int xOffset;
            int yOffset;

            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight -
                    SystemInformation.FrameBorderSize.Height;
                mouseOffset = new Point(xOffset, yOffset);
                isMouseDown = true;
            }
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            tm.Stop();
            Program.frm1.Show();
            Program.frm1.WindowState = FormWindowState.Normal;
            SetForegroundWindow(Program.frm1.Handle);
            this.Hide();
            label1.ForeColor = Color.Lime;
            Program.frm1.button2_Click(sender, e);

        }

    }
}
