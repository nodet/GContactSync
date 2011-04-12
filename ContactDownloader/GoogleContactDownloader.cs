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
        private string _user;
        private string _pass;

        public bool Authenticate(string user, string pass) {
            _user = user;
            _pass = pass;
            return Authenticate();
        }

        public bool Authenticate()
        {
            //ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri(user));
            //service = new ContactsService("unittests");

            //service.Credentials = new GDataCredentials(_user, _pass);

            rs = new RequestSettings("GContactSync", _user, _pass);
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
                    IContact c = new GContact(this, rs, gContact);
                    list.Add(c);
                }
            //}
            return list;
        }

        public IContact NewContact(IContact other)
        {
            return new GContact(this, rs, other);
        }
    }


    public class GContact: ContactBase
    {
        private RequestSettings _rs;
        private GoogleContactDownloader _gcd;
        private Google.Contacts.Contact _item;
        private bool _alreadyExistsOnGoogle = false;

        public GContact(GoogleContactDownloader gcd, RequestSettings rs, IContact other)
        {
            //System.Windows.Forms.MessageBox.Show("Creating a new Google contact for " + other.FullName + " in memory");
            _gcd = gcd;
            _rs = rs;
            _item = new Google.Contacts.Contact();
            _item.AtomEntry = new Google.GData.Contacts.ContactEntry();
            MergeFrom(other);
        }
        public GContact(GoogleContactDownloader gcd, RequestSettings rs, Google.Contacts.Contact gContact)
        {
            _gcd = gcd;
            _rs = rs;
            _item = gContact;
            _alreadyExistsOnGoogle = true;
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
        protected override bool internal_addMail(string mail)
        {
            if (_item.Emails.Where(m => m.Address.Equals(mail)).Count() > 0)
            {
                return false;
            }
            //System.Windows.Forms.MessageBox.Show("Adding " + mail + " as an email address for " + FullName + ".");
            EMail theMail = new EMail(mail, ContactsRelationships.IsOther);
            if (_item.Emails.Count() == 0)
            {
                theMail.Primary = true;
            }
            _item.Emails.Add(theMail);
            return true;
        }

        public override void Update()
        {
            ContactsRequest cr = new ContactsRequest(_rs);
            if (_alreadyExistsOnGoogle)
            {
                //System.Windows.Forms.MessageBox.Show("Updating " + FullName + " on Google");
                cr.Update(_item);
            } 
            else
            {
                //System.Windows.Forms.MessageBox.Show("Inserting " + FullName + " into Google");

                //ContactsService service = new ContactsService("GContactSync");
                //ContactsQuery q = new ContactsQuery(ContactsQuery.CreateContactsUri("xavier.nodet@gmail.com"));
                //ContactsFeed cf = service.Query(q);
                ////ContactEntry entry = ObjectModelHelper.CreateContactEntry(1);
                //ContactEntry insertedEntry = cf.Insert(_item.ContactEntry);

                Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
                cr.Insert(feedUri, _item);

                //_gcd.Authenticate();
                //cr.Insert(_item);
            }
        }


    }


}
