﻿using System.Collections.Generic;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using Google.GData.Client;

namespace GContactSync
{
    public partial class ThisAddIn
    {
        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }



        #endregion

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                GetAllContactsAndMergeThem();
            }
            // Not re-throwing: it seems to disable the addin
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Exception: " + ex);
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        private void GetAllContactsAndMergeThem()
        {
            string user;
            string pass;
            UserCredentials dialog = new UserCredentials();
            if (!UserCredentialsAsker.GetUserCredentials(dialog, out user, out pass)) {
                return;
            }

            GoogleContactDownloader gcd = new GoogleContactDownloader(user, pass);
            OContactManager ocm = new OContactManager(this.Application);
            List<IContact> l1 = new List<IContact>();
            List<IContact> l2 = new List<IContact>();
            l1.AddRange(gcd.GetContacts());
            l2.AddRange(ocm.GetContacts());
            System.Windows.Forms.MessageBox.Show("Starting merge...");
            ContactMerger.Merge(gcd, ocm, l1, l2);
            System.Windows.Forms.MessageBox.Show("Done!");
        }

    }





}
