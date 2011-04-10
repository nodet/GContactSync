using System;
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
            GetAllNodetFromGoogle();
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



        private void GetAllNodetFromGoogle()
        {
            GoogleContactDownloader gcd = new GoogleContactDownloader();
            gcd.Authenticate("xavier.nodet@gmail.com", "");
            IEnumerable<IContact> list = gcd.GetContacts();
            //System.Windows.Forms.MessageBox.Show("You have " + list.Count() + " contacts.");
            list = list.Where(c => c.FullName != null && c.FullName.Contains("Nodet"));
            //System.Windows.Forms.MessageBox.Show("You have " + list.Count() +
            //    " contacts with last names that contain "
            //    + "Nodet" + ".");
            foreach (IContact contact in list)
            {
                OContact oc = new OContact(this.Application, contact.FullName);
                foreach (string mail in contact.Emails)
                {
                    oc.addMail(mail);
                }
                oc.Update();
            }
        }

    
    }




    public class OContact : ContactBase
    {
        private Outlook.ContactItem _item;

        public OContact(Outlook.Application app, string name) {
            _item = (Outlook.ContactItem) app.CreateItem(Outlook.OlItemType.olContactItem);
            _item.FullName = name;
        }

        public override string FullName { get { return _item.FullName; } set { _item.FullName = value; } }
        
        public override IEnumerable<string> Emails { get { throw new NotImplementedException(); } }
        public override bool addMail(string mail)
        {
            if (string.IsNullOrEmpty(_item.Email1Address))
            {
                _item.Email1Address = mail;
            }
            else if (string.IsNullOrEmpty(_item.Email2Address))
            {
                _item.Email2Address = mail;
            }
            else if (string.IsNullOrEmpty(_item.Email3Address))
            {
                _item.Email3Address = mail;
            }
            else
            {
                return false;
            }
            return true;
        }

        public override void Update()
        {
            _item.Save();
        }


    }





}
