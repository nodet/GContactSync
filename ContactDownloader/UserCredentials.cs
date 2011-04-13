using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GContactSync
{
    public interface IUserCredentials {
        string User { get; }
        string Pass { get; }
        DialogResult ShowDialog();
    }


    public partial class UserCredentials : Form, IUserCredentials
    {
        public UserCredentials()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void UserCredentials_Load(object sender, EventArgs e)
        {

        }
    }


    public class UserCredentialsAsker {
        public static bool GetUserCredentials(IUserCredentials dialog, out string user, out string pass)
        {
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                user = null;
                pass = null;
                return false;
            }
            user = dialog.User;
            pass = dialog.Pass;
            return true;
        }
    }


}
