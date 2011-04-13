using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace GContactSync
{
    public class OContact : ContactBase
    {
        private Outlook.ContactItem _item;

        public OContact(Outlook.Application app, string name)
        {
            _item = (Outlook.ContactItem)app.CreateItem(Outlook.OlItemType.olContactItem);
            _item.FullName = name;
        }
        public OContact(Outlook.ContactItem item)
        {
            _item = item;
        }
        public OContact(Outlook.Application app, IContact other)
        {
            _item = (Outlook.ContactItem)app.CreateItem(Outlook.OlItemType.olContactItem);
            MergeFrom(other);
        }

        public override string FullName { get { return _item.FullName; } set { _item.FullName = value; } }

        public override IEnumerable<string> Emails
        {
            get
            {
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
            if (ContainsSomeInformation())
            {
                _item.Save();
            }
        }

    }
}
