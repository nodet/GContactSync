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
        //private ContactsService service;
        private RequestSettings rs = null;

        public bool Authenticate(string user, string pass) {
            //ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri(user));
            //service = new ContactsService("unittests");

            //service.Credentials = new GDataCredentials(user, pass);

            rs = new RequestSettings("GContactSync", user, pass);
            // AutoPaging results in automatic paging in order to retrieve all contacts
            rs.AutoPaging = true;
            return true;
        }
        public IEnumerable<IContact> GetContacts()
        {
            //ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri(""));
            //ContactsFeed feed = service.Query(query);
            ContactsRequest cr = new ContactsRequest(rs);
            Feed<Google.Contacts.Contact> feed = cr.GetContacts();

            List<IContact> list = new List<IContact>();
            //if (feed != null && feed.Entries.Count > 0)
            //{
                foreach (Google.Contacts.Contact gContact in feed.Entries)
                {
                    Name n = gContact.Name;
                    string ns = "";
                    if (n != null) {
                        ns = n.FullName;
                    }
                    IContact c = new Contact(ns);
                    foreach (Google.GData.Extensions.EMail email in gContact.Emails)
                    {
                        c.addMail(email.Address);
                    }
                    list.Add(c);
                }
            //}
            return list;
        }
    }
}
