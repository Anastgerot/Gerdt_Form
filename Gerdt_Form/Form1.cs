using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Gerdt_Form
{
    public struct Message
    {
        public int id;
        public int size;
    }
    public partial class Form1 : Form
    {
        private System.Timers.Timer timer;
        private int clientID = 0;
        private Process process;
        public Form1()
        {
            InitializeComponent();
            //AllocConsole();
        }
        //[DllImport("kernel32.dll")]
        //static extern bool AllocConsole();



        [DllImport(@"C:\Users\anast\OneDrive\Документы\GitHub\Gerdt_SystemProgram3\Gerdt_Form\x64\Debug\Gerdt_DLL.dll", CharSet = CharSet.Unicode)]
        public static extern void sendCommand(int commandId, string message);

        [DllImport(@"C:\Users\anast\OneDrive\Документы\GitHub\Gerdt_SystemProgram3\Gerdt_Form\x64\Debug\Gerdt_DLL.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr getCountClients();



        private void UpdateListBox()
        {
            sendCommand(4, "");
            IntPtr ptr = getCountClients();
            if (ptr != IntPtr.Zero)
            {
                int selectedIndex = ListBox.SelectedIndex;
                int topIndex = ListBox.TopIndex;

                ListBox.BeginUpdate();
                ListBox.Items.Clear();

                ListBox.Items.Add("Все клиенты");

                clientID = 0;
                string result = Marshal.PtrToStringUni(ptr);
                var clientsArray = result.Split('|');

                for (var i = 0; i < clientsArray.Length - 1; i++)
                {
                    clientID++;
                    ListBox.Items.Add("Клиент №" + clientsArray[i]);
                }

                if (selectedIndex >= 0 && selectedIndex < ListBox.Items.Count)
                    ListBox.SelectedIndex = selectedIndex;
                else
                    ListBox.SelectedIndex = 0;

                if (topIndex < ListBox.Items.Count)
                    ListBox.TopIndex = topIndex;

                ListBox.EndUpdate(); 
            }
        }

        private void Send_Click(object sender, EventArgs e)
        {

            if (TextBox.Text.Length == 0)
            {
                MessageBox.Show("Введите сообщение");
                return;
            }

            if (ListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента");
                return;
            }

            if (ListBox.Items.Count <= 1)
            {
                MessageBox.Show("Нет доступных клиентов для отправки сообщений");
                return;
            }

            string selectedThread = ListBox.SelectedItem.ToString();
            int threadId = 0;

            if (selectedThread != "Все клиенты")
            {
                string idStr = selectedThread.Replace("Клиент №", "").Trim();
                if (!int.TryParse(idStr, out threadId))
                {
                    MessageBox.Show("Не удалось определить ID клиента");
                    return;
                }
            }

            string message = $"{threadId}|{TextBox.Text}";
            sendCommand(2, message); 
            TextBox.Clear();
        }

        private void OnTimeout(Object source, ElapsedEventArgs e)
        {
            UpdateListBox();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            sendCommand(0, "");
            Thread.Sleep(200);
            UpdateListBox();

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimeout;
            timer.AutoReset = true;
            timer.Enabled = true;
        }
    }
}
