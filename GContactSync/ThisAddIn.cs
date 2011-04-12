using System.Collections.Generic;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using Google.GData.Client;

namespace GContactSync
{
    public partial class ThisAddIn
    {
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

        private void GetAllContactsAndMergeThem()
        {
            UserCredentials f = new UserCredentials();
            if (f.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string user = f.User;
            string pass = f.Pass;

            GoogleContactDownloader gcd = new GoogleContactDownloader(user, pass);
            OContactManager ocm = new OContactManager(this.Application);
            ContactMerger.Merge(gcd, ocm, gcd.GetContacts(), ocm.GetContacts());
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

        protected override bool internal_addMail(string mail)
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
            if (ContainsSomeInformation()) {
                _item.Save();
            }
        }

    }

    public class OContactManager : IContactManager
    {
        private Outlook.Application _application;
        public OContactManager(Outlook.Application app) {
            _application = app;
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
