using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POP3_Client_WForms
{
    public partial class MainForm : Form
    {
        private Client client;
        private List<Message> pageList;
        private LoginForm loginForm;

        private DataTable dt;

        private int listPoint = 1;

        public MainForm(Client client, LoginForm loginForm)
        {
            InitializeComponent();
            this.client = client;
            this.loginForm = loginForm;
        }

        private void receiveButton_Click(object sender, EventArgs e)
        {
            try
            {
                client.GetMailStats();

                pageList = client.GetMessages(client.NumberOfMessages);

                dt = new DataTable("Inbox");
                dt.Columns.Add("Subject", typeof(string));
                dt.Columns.Add("Sender", typeof(string));
                dt.Columns.Add("Date", typeof(string));

                dataGridView1.DataSource = dt;

                foreach (var item in pageList)
                {
                    dt.Rows.Add(new object[] { item.Subject, item.Sender, item.Date });
                }
            }
            catch(Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }


        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            listPoint = client.NumberOfMessages - e.RowIndex;
            string fullText = "";
            Message msg = client.GetMessage(listPoint);
            if (!msg.Deleted)
            {
                foreach (string line in msg.Text)
                {
                    fullText += line + "\r\n";
                }
                textBox1.Text = fullText;

                toolStripStatusLabel1.Text = listPoint.ToString() + " message size: " + client.GetMessageSize(listPoint).ToString();
            }
            else
            {
                textBox1.Text = "--MESSAGE DELETED--";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(listPoint != 0)
            {
                if (!client.DeleteMessage(listPoint))
                    toolStripStatusLabel1.Text = "Failed to delete letter #" + listPoint;
                else
                {
                    dataGridView1.Rows[client.NumberOfMessages - listPoint].DefaultCellStyle.BackColor = Color.Red;
                    toolStripStatusLabel1.Text = "Successfully deleted letter #" + listPoint;
                }
  
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (!client.ResetDeletedMessages())
                toolStripStatusLabel1.Text = "Failed to reset deleted letters";
            else
            {
                for(int i = 0; i < client.NumberOfMessages; i++)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }
                toolStripStatusLabel1.Text = "Successfully reset";
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            if(!client.EndConnection())
                toolStripStatusLabel1.Text = "Logged out, failed to delete letters";
            else
                toolStripStatusLabel1.Text = "Successfully logged out";

            this.Close();
            loginForm.Returned(toolStripStatusLabel1.Text);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!client.CheckConnection())
                toolStripStatusLabel1.Text = "Connection status: OK";
            else
                toolStripStatusLabel1.Text = "Connection status: not OK";
        }
    }
}
