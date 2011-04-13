using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GContactSync
{
    public class ContactIndexer
    {
        private Dictionary<string, List<IContact>> _dict = new Dictionary<string, List<IContact>>();

        public ContactIndexer(IEnumerable<IContact> contacts) {
            foreach (IContact c in contacts) {
                AddIntoIndex(c, c.FullName);
                foreach (string email in c.Emails) {
                    AddIntoIndex(c, email);
                }
            }
        }
        public IEnumerable<IContact> GetContactsFor(string key)
        {
            List<IContact> l;
            return ((key != null) && _dict.TryGetValue(key, out l) ? l : new List<IContact>());
        }

        public IEnumerable<IContact> GetSameContactsAs(IContact c) {
            var hash = new HashSet<IContact>();
            hash.UnionWith(GetContactsFor(c.FullName));
            foreach (string email in c.Emails)
            {
                hash.UnionWith(GetContactsFor(email));
            }
            return hash;
        }


        private void AddIntoIndex(IContact c, string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return;
            }
            List<IContact> l;
            if (!_dict.TryGetValue(s, out l))
            {
                l = new List<IContact>();
                _dict.Add(s, l);
            }
            l.Add(c);
        }

    }

    public class ContactMerger
    {
        public static void Merge(IContactManager m1, IContactManager m2,
                                 IEnumerable<IContact> l1, IEnumerable<IContact> l2)
        {
            var l2Index = new ContactIndexer(l2);
            foreach (IContact c in l1)
            {
                bool foundMerge = false;
                foreach (IContact oc in l2Index.GetSameContactsAs(c))
                {
                    if (!c.IsSameAs(oc)) continue;

                    foundMerge = true;
                    if (c.MergeFrom(oc))
                    {
                        //System.Windows.Forms.MessageBox.Show("Updating Google information for " + c.ToString());
                        c.Update();
                    }
                    if (oc.MergeFrom(c))
                    {
                        //System.Windows.Forms.MessageBox.Show("Updating Outlook information for " + c.ToString());
                        oc.Update();
                    }
                }
                if (!foundMerge)
                {
                    //System.Windows.Forms.MessageBox.Show("Copy information from Google to Outlook for " + c.ToString());
                    IContact newContact = m2.NewContact(c);
                    newContact.Update();
                }
            }
            
            var l1Index = new ContactIndexer(l1);
            foreach (IContact oc in l2)
            {
                bool foundMerge = false;
                foreach (IContact c in l1Index.GetSameContactsAs(oc))
                {
                    if (!c.IsSameAs(oc)) continue;
                    foundMerge = true;
                }
                if (!foundMerge)
                {
                    //System.Windows.Forms.MessageBox.Show("Copy information from Outlook to Google for " + oc.ToString());
                    IContact newContact = m1.NewContact(oc);
                    newContact.Update();
                }
            }
        }
    }
}
