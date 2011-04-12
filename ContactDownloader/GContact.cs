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
    public class GContact : ContactBase
    {
        private RequestSettings _rs;
        private Google.Contacts.Contact _item;
        private bool _alreadyExistsOnGoogle = false;

        public GContact(RequestSettings rs, IContact other)
        {
            //System.Windows.Forms.MessageBox.Show("Creating a new Google contact for " + other.FullName + " in memory");
            _rs = rs;
            _item = new Google.Contacts.Contact();
            _item.AtomEntry = new Google.GData.Contacts.ContactEntry();
            MergeFrom(other);
        }
        public GContact(RequestSettings rs, Google.Contacts.Contact gContact)
        {
            _rs = rs;
            _item = gContact;
            _alreadyExistsOnGoogle = true;
        }

        public override string FullName
        {
            get
            {
                if (_item.Name != null)
                {
                    return _item.Name.FullName;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (_item.Name == null)
                {
                    _item.Name = new Name();
                }
                _item.Name.FullName = value;
            }
        }

        public override IEnumerable<string> Emails
        {
            get
            {
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
                Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
                cr.Insert(feedUri, _item);
            }
        }


    }
}
