﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace GContactSync
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            AccessContacts("a");
        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        private void AccessContacts(string findLastName)
        {
            Outlook.MAPIFolder folderContacts = this.Application.ActiveExplorer().Session.
                GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);
            Outlook.Items searchFolder = folderContacts.Items;
            int counter = 0;
            foreach (Outlook.ContactItem foundContact in searchFolder)
            {
                if (foundContact.LastName.Contains(findLastName))
                {
                    //foundContact.Display(false);
                    counter = counter + 1;
                }
            }
            System.Windows.Forms.MessageBox.Show("You have " + counter +
                " contacts with last names that contain "
                + findLastName + ".");
        }

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
    }
}
