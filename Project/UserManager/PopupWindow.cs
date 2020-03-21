using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserManager
{
    public partial class PopupWindow : Form
    {
        bool waterMarkActive = false;
        public PopupWindow()
        {
            InitializeComponent();

            //Focus on username textbox
            this.ActiveControl = textBoxUsername;

            //Add text hint
            this.waterMarkActive = true;
            this.textBoxUsername.ForeColor = Color.Gray;
            this.textBoxUsername.Text = "Type username here";
            this.textBoxUsername.GotFocus += (source, e) =>
            {
                if (this.waterMarkActive)
                {
                    this.waterMarkActive = false;
                    this.textBoxUsername.Text = "";
                    this.textBoxUsername.ForeColor = Color.Black;
                }
            };
            this.textBoxUsername.LostFocus += (source, e) =>
            {
                if (!this.waterMarkActive && string.IsNullOrEmpty(this.textBoxUsername.Text))
                {
                    this.waterMarkActive = true;
                    this.textBoxUsername.Text = "Type username here";
                    this.textBoxUsername.ForeColor = Color.Gray;
                }
            };
        }

        private void ClickLogin(object sender, EventArgs e)
        {   //close this form
            this.Close();
        }

        public string EnteredText
        {
            get
            {   //return Atlas Connection String
                return ("mongodb+srv://" + textBoxUsername.Text.Replace(" ","") + ":"+ textBoxPassword.Text + "@alpencluster-acbgw.mongodb.net/test?retryWrites=true&w=majority");
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
                this.Close();
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
