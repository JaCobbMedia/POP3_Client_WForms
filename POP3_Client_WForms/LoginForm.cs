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
    public partial class LoginForm : Form
    {
        string server = "mail.stud.vu.lt";
        int port = 110;
        Client client;
        public LoginForm()
        {
            InitializeComponent();
            client = new Client(server, port);
            try
            {
                client.Connect();
                toolStripStatusLabel1.Text = "Successfully connected to server";
            }
            catch(Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(client.LogIn(usernameBox.Text, passwordBox.Text))
            {
                var mainForm = new MainForm(client,this);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                toolStripStatusLabel1.Text = "Wrong username or password";
                usernameBox.Text = "";
                passwordBox.Text = "";
            }
                
        }
        public void Returned(string message)
        {
            this.Show();
            toolStripStatusLabel1.Text = message;
        }
    }
}
