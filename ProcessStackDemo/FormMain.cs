using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessStackDemo
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ProcessFrameTime.Elapsed = timer1.Interval;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProcessFrame.MoveStep();
            txtBox.Text = ProcessFrame.CallStack();
        }

        private void btnPrepareBreakfast_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            txtMsg.Clear();

            ProcessFrame.Emit(PrepareBreakfast()).ContinueWith(() =>
            {
                button1.Enabled = true;
            });
        }

        private ProcessFrame PrepareBreakfast()
        {
            var pourCoffee = ProcessFrame.Create("PourCoffee", (p) =>
            {
                switch (p.Step)
                {
                case ProcessFrame.ENTER:
                    Message("coffe prepare");
                    p.Delay(1000);
                    break;
                case 1:
                    Message("coffe done!");
                    p.Exit();
                    break;
                };
            });

            var fryEgg = ProcessFrame.Create("FryEgg", (p) =>
             {
                 switch (p.Step)
                 {
                 case ProcessFrame.ENTER:
                     Message("fry egg prepare");
                     p.Delay(5000);
                     break;
                 case 1:
                     Message("fry eggs done!");
                     p.Exit();
                     break;
                 };
             });

            var breadToast = ProcessFrame.Create("BreadToast", (p) =>
            {
                switch (p.Step)
                {
                case ProcessFrame.ENTER:
                    Message("bread toast prepare");
                    p.Delay(10000);
                    break;
                case 1:
                    Message("bread toast done!");
                    p.Exit();
                    break;
                };
            });

            var fryBacon = ProcessFrame.Create("FryBacon", (p) =>
            {
                switch (p.Step)
                {
                case ProcessFrame.ENTER:
                    Message("fry bacon prepare");
                    p.Delay(8000);
                    break;
                case 1:
                    Message("fry bacon done!");
                    p.Exit();
                    break;
                };
            });

            return ProcessFrame.Create((p) =>
            {
                switch (p.Step)
                {
                case ProcessFrame.ENTER:
                    Message("breakfast prepare");
                    ProcessFrame.Emit(breadToast);
                    p.aWait(pourCoffee);
                    break;
                case 1:
                    p.aWait(fryEgg);
                    p.aWait(fryBacon);
                    break;
                case 2:
                    p.aWait(breadToast);
                    break;
                case 3:
                    Message("breakfast done!");
                    p.Exit();
                    break;
                }
            });
        }

        private void Message(string msg)
        {
            var time = DateTime.Now.ToString("mm:ss:ff");
            txtMsg.Text += $"[{time}] {msg}";
            txtMsg.Text += Environment.NewLine;
        }

        private void btnPause_Resume_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            button2.Text = timer1.Enabled ? "Pause" : "Resume";
        }

        private void btnProcuce_Consume_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            txtMsg.Clear();

            ProcessFrame.Emit(CreateProduceConsume(5)).ContinueWith(() =>
            {
                button3.Enabled = true;
            });
        }

        private ProcessFrame CreateProduceConsume(int n)
        {
            var randon = new Random();
            var qItems = new Queue<int>();
            int i = 0;

            var produce = ProcessFrame.Create("Produce", (p) =>
            {
                switch (p.Step)
                {
                case ProcessFrame.ENTER:
                    if (i >= n)
                    {
                        qItems.Enqueue(-1);
                        p.Exit();
                        return;
                    }
                    p.Delay(randon.Next(200, 600));
                    break;
                case 1:
                    qItems.Enqueue(i++);
                    Message($"producing [ {string.Join(" ", qItems)} ]");
                    p.SetStep(0);
                    break;
                }
            });

            var consume = ProcessFrame.Create("Consume", (p) =>
            {
                switch (p.Step)
                {
                case ProcessFrame.ENTER:
                    if (qItems.Count > 0)
                    {
                        int itemId = qItems.Dequeue();
                        Message($"consuming [ {string.Join(" ", qItems)} ]");
                        if (itemId == -1)
                        {
                            p.Exit();
                            return;
                        }
                        p.Delay(randon.Next(500, 1000));
                    }
                    break;

                case 1:
                    p.SetStep(0);
                    break;
                }
            });

            var main = ProcessFrame.Create("Main", (p) =>
            {
                switch (p.Step)
                {
                case ProcessFrame.ENTER:
                    p.aWait(produce);
                    p.aWait(consume);
                    break;
                case 1:
                    p.Exit();
                    break;
                }
            });
            return main;
        }
    }


}
