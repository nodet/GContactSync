using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Contacts;
using Google.Contacts;

namespace GContactSync
{
    public class GoogleContactDownloader : IContactManager
    {
        private RequestSettings _rs = null;
        private string _user;
        private string _pass;

        public bool Authenticate(string user, string pass) {
            _user = user;
            _pass = pass;
            return Authenticate();
        }

        public bool Authenticate()
        {
            _rs = new RequestSettings("GContactSync", _user, _pass);
            // AutoPaging results in automatic paging in order to retrieve all contacts
            _rs.AutoPaging = true;
            return true;
        }

        public IEnumerable<IContact> GetContacts()
        {
            ContactsRequest cr = new ContactsRequest(_rs);
            Feed<Google.Contacts.Contact> feed = cr.GetContacts();

            List<IContact> list = new List<IContact>();
            foreach (Google.Contacts.Contact gContact in feed.Entries)
            {
                IContact c = new GContact(_rs, gContact);
                list.Add(c);
            }
            return list;
        }

        public IContact NewContact(IContact other)
        {
            return new GContact(_rs, other);
        }
    }
}
