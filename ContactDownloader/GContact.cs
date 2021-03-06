﻿using System;
using System.Collections.Generic;
using System.Linq;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;

namespace GContactSync
{
    public class GContact : ContactBase
    {
        private RequestSettings _rs;
        private Google.Contacts.Contact _item;
        private bool _alreadyExistsOnGoogle = false;

        public GContact(RequestSettings rs, IContact other)
        {
            //System.Windows.Forms.MessageBox.Show("Creating a new Google contact for " + other.ToString() + " in memory");
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
            get {
                if (_item.Name != null) {
                    return _item.Name.FullName;
                } else {
                    return "";
                }
            }
            set {
                if (_item.Name == null) {
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
            //System.Windows.Forms.MessageBox.Show("Adding " + mail + " as an email address for " + this.ToString() + ".");
            EMail theMail = new EMail(mail, ContactsRelationships.IsOther);
            _item.Emails.Add(theMail);
            return true;
        }

        public override void Update()
        {
            try
            {
                ContactsRequest cr = new ContactsRequest(_rs);
                if (_alreadyExistsOnGoogle)
                {
                    //System.Windows.Forms.MessageBox.Show("Updating " + this.ToString() + " on Google");
                    cr.Update(_item);
                }
                else
                {
                    //System.Windows.Forms.MessageBox.Show("Inserting " + this.ToString() + " into Google");
                    Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
                    cr.Insert(feedUri, _item);
                }
            }
            catch (GDataRequestException ex)
            {
                System.Windows.Forms.MessageBox.Show("GDataRequestException while saving data for " + this.ToString() + ": " + ex.ResponseString);
            }
        }


    }
}
