using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NetIoTester;

namespace Test
{
    public partial class Form1 : Form
    {
        List<Test> testList = new List<Test>();
        TelnetConnection tc = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string[] pomPole;

                try
                {
                    while (!sr.EndOfStream)
                    {
                        pomPole = sr.ReadLine().Split(';');
                        testList.Add(new Test(pomPole[0], pomPole[1]));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                naplListBox();
                nastavCheckedListBox(true);
            }
        }

        void naplListBox()
        {
            foreach (Test item in testList)
            {
                checkedListBoxTesty.Items.Add(item);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            nastavCheckedListBox(true);
        }
        private void nastavCheckedListBox(bool nastav)
        {
            for (int i = 0; i < checkedListBoxTesty.Items.Count; i++)
            {
                checkedListBoxTesty.SetItemChecked(i, nastav);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            nastavCheckedListBox(false);
        }

        private LinkedList<Test> dejSeznamTestu()
        {
            LinkedList<Test> ll = new LinkedList<Test>();
            for (int i = 0; i < checkedListBoxTesty.Items.Count; i++)
            {
                if (checkedListBoxTesty.GetItemChecked(i))
                {
                    ll.AddLast(testList[i]);
                }
            }
            return ll;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {

            aktualizujProgressBar(0);


            try
            {
                tc = new TelnetConnection(textBoxIP.Text, int.Parse(textBoxPort.Text));
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string prijataData = "";
            int pocetTestu = dejSeznamTestu().Count;
            int i = 1;
            richTextBoxPrijato.Text += "Test (" + textBoxIP.Text + "): " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";

            foreach (Test item in dejSeznamTestu())
            {
                prijataData = "";
                richTextBoxPrijato.Text += tc.Read();

                tc.WriteLine(item.Vstup);
                prijataData = tc.Read();
                if (item.Prosel(prijataData))
                {
                    richTextBoxPrijato.Text += "OK: " + item.Vstup + " (" + prijataData.Trim() + ")" + "\r\n";
                }
                else
                {
                    richTextBoxPrijato.Text += "!!! FAIL: " + item.Vstup + " (" + prijataData.Trim() + ")" + "\r\n";
                }

                aktualizujProgressBar(100 / (pocetTestu / i));
                i++;
            }
            richTextBoxPrijato.Text += "\r\n";
        }

        private void aktualizujProgressBar(int procent)
        {
            toolStripProgressBar1.Value = procent;
        }
    }
}
