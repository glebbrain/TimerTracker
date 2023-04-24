using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TimerTracker.Libs;

namespace TimerTracker
{
    public partial class Form1 : Form
    {
        public string FileTaimTreker = "";
        Timer tm = null;
        int startValue = 0;
        string startTimer = "", endTimer = "";
        Form2 form2 = null;
        public string DirTaimTreker="";
        public string PathConfig = Path.Combine(Environment.CurrentDirectory, "config.ini");
        public bool IsTimerRunRoundState = false;
        public bool IsMinimizeMainWindow = false;
        public DateTime StartProgramTime = DateTime.Now;
        public Form1()
        {
            InitializeComponent();

            tm = new Timer();
            tm.Tick += new EventHandler(tm_Tick);
            tm.Interval = 1000;

            form2 = new Form2();

            DirTaimTreker = Path.Combine(Environment.CurrentDirectory, "logs");
            if (!Directory.Exists(DirTaimTreker))
            {
                Directory.CreateDirectory(DirTaimTreker);
            }
            FileTaimTreker = Path.Combine(DirTaimTreker, DateTime.Now.ToString("dd-MM-yyyy") + ".txt");
            if (File.Exists(FileTaimTreker))
            {
                textBox3.Text = File.ReadAllText(FileTaimTreker);
            }

            Ini.IniPath = PathConfig;
            if (File.Exists(PathConfig))
            {
                string IntervalList = Ini.readValue("Main", "IntervalList", "5,10,15,30,45,60,90,120");
                if (!String.IsNullOrEmpty(IntervalList) && !String.IsNullOrWhiteSpace(IntervalList)) 
                {
                    int a = -1;
                    // multiple values
                    if (IntervalList.IndexOf(',') > -1)
                    {
                        comboBox1.Items.Clear();
                        List<int> list = IntervalList.Split(',').ToList().Cast<object>().Select(x => int.Parse(x.ToString())).ToList();
                        list = list.Distinct().ToList();
                        list.Sort();
                        foreach (int i in list)
                        {
                            comboBox1.Items.Add(i);
                        }
                        
                    }
                    else if (IntervalList.Length > 0 && int.TryParse(IntervalList, out a) && a > 0)
                    {
                        comboBox1.Items.Clear();
                        comboBox1.Items.Add(a.ToString());
                    }
                }
                if (Ini.readValue("Main", "MinimizeMainWindow", "")=="")
                {
                    Ini.writeValue(
                        "Main",
                        "MinimizeMainWindow",
                        this.IsMinimizeMainWindow.ToString(),
                        "Minimize the main window when you press Start"
                    );
                }
                this.IsMinimizeMainWindow = Ini.readValue("Main", "MinimizeMainWindow", "False").ToLower()=="false" ? false : true;
                
                if (Ini.readValue("Main", "StartItemIntervals", "") == "")
                {
                    Ini.writeValue(
                        "Main",
                        "StartItemIntervals",
                        "30",
                        "The initial value of the interval for the timer at the start of the program"
                    );
                }
                comboBox1.Text = Ini.readValue("Main", "StartItemIntervals", "30").ToLower();
            }
            StartProgramTime = DateTime.Now;
            timer1.Start();
        }
        /// <summary>
        /// Write Ini values
        /// </summary>
        /// <param name="listItems"></param>
        private void WriteIni()
        {
            List<int> listItems = comboBox1.Items.Cast<Object>().Select(item => int.Parse(item.ToString())).ToList();
            Ini.writeValue(
                "Main",
                "IntervalList",
                String.Join(",", listItems),
                "The variable contains a list of intervals for the dropdown list"
            );

            Ini.writeValue(
                "Main",
                "StartItemIntervals",
                comboBox1.Text,
                "The initial value of the interval for the timer at the start of the program"
            );

            Ini.writeValue(
                "Main",
                "MinimizeMainWindow",
                this.IsMinimizeMainWindow.ToString(),
                "Minimize the main window when you press Start"
            );
        }
        private string Int2StringTime(int time) 
        {
            int hours = (time - (time % (60 * 60))) / (60 * 60);
            int minutes = (time - time % 60) / 60 - hours * 60;
            int seconds = time - hours * 60 * 60 - minutes * 60;
            return String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        public void tm_Tick(object sender, EventArgs e)
        {
            textBox2.Text = Int2StringTime(startValue);
            startValue++;
        }
        public List<double> AllTakenTimes = new List<double>();
        /// <summary>
        /// Stop timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button2_Click(object sender, EventArgs e)
        {
            endTimer = DateTime.Now.ToLongTimeString();
            string t = String.Format(
                    "[c {0} по {1}, затрачено: {2}] {3}",
                    startTimer,
                    endTimer,
                    (new DateTime() + (DateTime.Parse(endTimer) - DateTime.Parse(startTimer))).ToString("HH:mm:ss"),
                    textBox1.Text
                );

            AllTakenTimes.Add((DateTime.Parse(endTimer) - DateTime.Parse(startTimer)).TotalSeconds);
            toolStripStatusLabel2.Text = "Time taken: " + (new DateTime() + TimeSpan.FromSeconds(AllTakenTimes.Sum(x => x))).ToString("HH:mm:ss");


            if (FileTaimTreker != null && FileTaimTreker != "")
            {
                File.AppendAllLines(FileTaimTreker, new string[] { t });
                textBox3.Text = File.ReadAllText(FileTaimTreker);
            }
            else
            {
                textBox3.Text = t;
            }

            textBox1.Text = "";
            startTimer = "";
            endTimer = "";
            textBox2.Text = "00:00:00";
            
            // time in window
            form2.tm.Stop();
            form2.label1.Text = "30";
            form2.label1.ForeColor = Color.Lime;
            tm.Stop();
            form2.Hide();
            // Stop
            button2.Enabled = false;
            // Start
            button3.Enabled = true;
            button3.TabStop = true;
            button3.Select();
            // Round
            button6.Enabled = true;


            // say round: stop
            this.IsTimerRunRoundState = false;


           
            // if IsTimerRunRoundState == true :
            // Externally controlled:
            // form2.tm_Tick();
        }


        /// <summary>
        /// Start the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            startTimer = DateTime.Now.ToLongTimeString();
            textBox2.Text = "";
            startValue = 0;
            tm.Start();

            string s = comboBox1.Text.ToString();
            Program.countDown = int.Parse(s);
            form2.alltime = int.Parse(s);
            form2.startValue = int.Parse(s);
            form2.label1.Text = s;

            form2.label1.ForeColor = Color.Lime;
            form2.delit = form2.alltime / 3;
            if (form2.delit > 0)
            {
                form2.low = form2.alltime - form2.delit;
                form2.medium = form2.low - form2.delit;
                form2.hard = form2.medium - form2.delit;
            }

            form2.tm.Start();
            form2.Show();

            button3.Enabled = false;
            button6.Enabled = false;
            button2.Enabled = true;
            button2.TabStop = true;
            button2.Select();

            // Config.Main.IsMinimizeMainWindow == true ? FormWindowState.Minimized : nothing
            if (this.IsMinimizeMainWindow)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            IsTimerRunRoundState = false;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(FileTaimTreker, textBox3.Text);
        }
        /// <summary>
        /// Select timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            List<int> listItems = comboBox1.Items.Cast<Object>().Select(item => int.Parse(item.ToString())).ToList();
            int newValue = -1;
            if (int.TryParse(comboBox1.Text.ToString(), out newValue) && newValue>0)
            {
                listItems.Add(newValue);
                listItems.Sort();
                listItems = listItems.Distinct().ToList();
                if (!comboBox1.Items.Contains(newValue))
                {
                    comboBox1.Items.Add(newValue);
                }
            }
            WriteIni();
        }
        

        /// <summary>
        /// Open Config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (File.Exists(PathConfig))
            {
                string npp = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
                string npp1 = @"C:\Program Files\Notepad++\notepad++.exe";
                if (File.Exists(npp))
                {
                    Process.Start(npp, "\"" + PathConfig + "\"");
                }
                else if (File.Exists(npp1))
                {
                    Process.Start(npp1, "\"" + PathConfig + "\"");
                }
                else
                {
                    Process.Start("notepad", "\"" + PathConfig + "\"");
                }
            }
            else
            {
                WriteIni();
                button4.PerformClick();
            }
        }
        /// <summary>
        /// About
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                    "Нажмите ДА для перехода на видео инструкцию\r\n\r\n" +
                    "Нажмите НЕТ для перехода в канал поддержки\r\n\r\n" +
                    "Нажмите Отмена для закрытия этого сообщения\r\n",
                    "Подробнее о программе TimeTracker",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Asterisk
                );

            if (dr == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://www.youtube.com/c/GlebBrainOfficial");
            }
            else if (dr == DialogResult.No)
            {
                System.Diagnostics.Process.Start("https://boosty.to/glebbrain");
            }
        }

        /// <summary>
        /// The timer runs in a circle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            // Start Timer
            button3.PerformClick();
            // Timer is working in round state
            IsTimerRunRoundState = true;

            // Externally controlled:
            //form2.tm_Tick();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Start
            button3.TabStop = true;
            button3.Select();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            WriteIni();
        }

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://glebrain.ru/links.php");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Working time: ";
            DateTime dt = new DateTime() + (DateTime.Now - DateTime.Parse(StartProgramTime.ToString("dd.MM.yyyy HH:mm:ss")));
            if (dt!=null)
            {
                toolStripStatusLabel1.Text = "Working time: " + dt.ToString("HH:mm:ss");
            }
        }

        /// <summary>
        /// Clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllText(FileTaimTreker, textBox3.Text);

            if (MessageBox.Show("Do clear the log?", "Deleting the log from the field", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                textBox3.Text = "";
            }
        }

    }
}
