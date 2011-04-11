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
            try
            {
                GoogleContactDownloader gcd = new GoogleContactDownloader();
                gcd.Authenticate("xavier.nodet@gmail.com", "");
                OContactManager ocm = new OContactManager(this.Application);
                ContactMerger.Merge(gcd, ocm,
                                    gcd.GetContacts().Where(c => c.FullName != null && c.FullName.Contains("Marcy")),
                                    ocm.GetContacts().Where(c => c.FullName != null && c.FullName.Contains("Marcy")));
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Exception: " + ex);
                throw;
            }
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




    public class OContact : ContactBase
    {
        private Outlook.ContactItem _item;

        public OContact(Outlook.Application app, string name) {
            _item = (Outlook.ContactItem) app.CreateItem(Outlook.OlItemType.olContactItem);
            _item.FullName = name;
        }
        public OContact(Outlook.ContactItem item) {
            _item = item;
        }
        public OContact(Outlook.Application app, IContact other) {
            _item = (Outlook.ContactItem)app.CreateItem(Outlook.OlItemType.olContactItem);
            MergeFrom(other);
        }

        public override string FullName { get { return _item.FullName; } set { _item.FullName = value; } }
        
        public override IEnumerable<string> Emails { 
            get {
                List<string> l = new List<string>();
                if (!string.IsNullOrEmpty(_item.Email1Address))
                {
                    l.Add(_item.Email1Address);
                }
                if (!string.IsNullOrEmpty(_item.Email2Address))
                {
                    l.Add(_item.Email2Address);
                }
                if (!string.IsNullOrEmpty(_item.Email3Address))
                {
                    l.Add(_item.Email3Address);
                }
                return l;
            } 
        }

        public override bool addMail(string mail)
        {
            if (mail.Equals(_item.Email1Address) || mail.Equals(_item.Email2Address) || mail.Equals(_item.Email3Address))
            {
                // Already exists
                return false;
            }
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
                // No free slot
                return false;
            }
            return true;
        }

        public override void Update()
        {
            _item.Save();
        }


    }

    public class OContactManager : IContactManager
    {
        private Outlook.Application _application;
        public OContactManager(Outlook.Application app) {
            _application = app;
        }

        public bool Authenticate(string user, string pass) {
            return true;
        }

        public IEnumerable<IContact> GetContacts() {
            Outlook.MAPIFolder folderContacts = _application.ActiveExplorer().Session.
                GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);
            Outlook.Items searchFolder = folderContacts.Items;
            List<IContact> list = new List<IContact>();
            foreach (Outlook.ContactItem foundContact in searchFolder)
            {
                OContact oc = new OContact(foundContact);
                list.Add(oc);
            }
            return list;
        }

        public IContact NewContact(IContact other) {
            return new OContact(_application, other);
        }
    }




}
