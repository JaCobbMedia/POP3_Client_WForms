using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP3_Client_WForms
{
    public class Message
    {
        public int Nr { get; set; }
        public int Size { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Sender { get; set; }
        public List<string> Text { get; set; }
        public List<string> AllText { get; set; }
        public bool Deleted { get; set; }

        public void ExtractMessage()
        {
            bool txt = false;
            int i = 0;
            foreach (string line in AllText)
            {
                if (line.StartsWith("--") && i < 2)
                {
                    txt = !txt;
                    i++;
                }

                if (txt)
                    Text.Add(line);

                if (line.StartsWith("From: "))
                    Sender = line.Substring(6);

                if (line.StartsWith("Date: "))
                    Date = line.Substring(6);

                if (line.StartsWith("Subject: "))
                    Subject = line.Substring(9);
            }
            if (i != 0)
                Text.RemoveRange(0, 3);
        }
    }
}
