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

        public IContact NewContact(IContact other)
        {
            return new GContact(rs, other);
        }
    }


    public class GContact: ContactBase
    {
        private RequestSettings _rs;
        private Google.Contacts.Contact _item;
        //private bool _alreadyExistsOnGoogle = false;

        public GContact(RequestSettings rs, IContact other)
        {
            _rs = rs;
            _item = new Google.Contacts.Contact();
            MergeFrom(other);
        }

        public override string FullName { 
            get {
                if (_item.Name != null)
                {
                    return _item.Name.FullName;
                } 
                else
                {
                    return "";
                }
            } 
            set {
                if (_item.Name == null)
                {
                    _item.Name = new Name();
                }
                _item.Name.FullName = value; 
            } 
        }
        
        public override IEnumerable<string> Emails { 
            get { 
                List<string> l = new List<string>();
                foreach (EMail mail in _item.Emails)
                {
                    l.Add(mail.Address);
                }
                return l;
            } 
        }
        public override bool addMail(string mail)
        {
            System.Windows.Forms.MessageBox.Show("Adding " + mail +
                " as an email address for "
                + FullName + ".");
            if (_item.Emails.Where(m => m.Address.Equals(mail)).Count() > 0)
            {
                return false;
            }
            _item.Emails.Add(new EMail(mail));

            return true;
        }

        public override void Update()
        {
            ContactsRequest cr = new ContactsRequest(_rs);
            //if (alreadyExistsOnGoogle)
            //{
            //    cr.Update(_item);
            //} 
            //else
            //{
                Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
                cr.Insert(feedUri, _item);
            //}
        }


    }


}
