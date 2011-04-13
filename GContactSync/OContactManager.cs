using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace GContactSync
{
    public class OContactManager : IContactManager
    {
        private Outlook.Application _application;
        public OContactManager(Outlook.Application app)
        {
            _application = app;
        }

        public IEnumerable<IContact> GetContacts()
        {
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

        public IContact NewContact(IContact other)
        {
            return new OContact(_application, other);
        }
    }
}
