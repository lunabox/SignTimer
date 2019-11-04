using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignTimer
{
    public partial class FormMain : Form
    {
        private string logPath = Directory.GetCurrentDirectory() + @"\sign.txt";

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
            UpdateSignTime();
            AutoStart(true, false);
        }

        /// <summary>
        /// 更新启动时间到log文件中
        /// </summary>
        private void UpdateSignTime()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(logPath, true))
            {
                string logTime = DateTime.Now.ToString();
                string computerBootTime = GetComputerStartTime().ToString();
                file.WriteLine(string.Format("签到时间：{0}，电脑本次开机时间：{1}", logTime, computerBootTime));
            }
        }

        /// <summary>
        /// 计算电脑是几点启动
        /// </summary>
        /// <returns></returns>
        private DateTime GetComputerStartTime()
        {
            int result = Environment.TickCount & Int32.MaxValue;      //获取系统启动后运行的毫秒数
            TimeSpan m_WorkTimeTemp = new TimeSpan(Convert.ToInt64(Convert.ToInt64(result) * 10000));

            DateTime startTime = DateTime.Now.AddDays(m_WorkTimeTemp.Days);
            startTime = startTime.AddHours(-m_WorkTimeTemp.Hours);
            startTime = startTime.AddMinutes(-m_WorkTimeTemp.Minutes);
            startTime = startTime.AddSeconds(-m_WorkTimeTemp.Seconds);
            //MessageBox.Show(startTime.ToString());
            return startTime;    //返回转换后的时间
        }

        /// <summary>  
        /// 修改程序在注册表中的键值  
        /// </summary>  
        /// <param name="isAuto">true:开机启动,false:不开机自启</param> 
        private void AutoStart(bool isAuto = true, bool showinfo = true)
        {
            try
            {
                if (isAuto == true)
                {
                    RegistryKey R_local = Registry.CurrentUser;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.SetValue(this.Text, Application.ExecutablePath);
                    R_run.Close();
                    R_local.Close();
                }
                else
                {
                    RegistryKey R_local = Registry.CurrentUser;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.DeleteValue(this.Text, false);
                    R_run.Close();
                    R_local.Close();
                }
            }
            catch (Exception)
            {
                if (showinfo)
                {
                    MessageBox.Show("您需要管理员权限修改", "提示");
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            labelNow.Text = now.ToLongDateString() + " " + now.ToLongTimeString();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonList_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", logPath);
        }
    }
}
