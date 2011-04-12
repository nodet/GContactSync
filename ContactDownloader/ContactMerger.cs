using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GContactSync
{
    public class ContactMerger
    {
        public static void Merge(IContactManager m1, IContactManager m2,
                                 IEnumerable<IContact> l1, IEnumerable<IContact> l2)
        {
            foreach (IContact c in l1)
            {
                bool foundMerge = false;
                foreach (IContact oc in l2)
                {
                    if (!c.IsSameAs(oc)) continue;

                    foundMerge = true;
                    if (c.MergeFrom(oc))
                    {
                        //System.Windows.Forms.MessageBox.Show("Updating Google information for " + c.FullName);
                        c.Update();
                    }
                    if (oc.MergeFrom(c))
                    {
                        //System.Windows.Forms.MessageBox.Show("Updating Outlook information for " + c.FullName);
                        oc.Update();
                    }
                }
                if (!foundMerge)
                {
                    //System.Windows.Forms.MessageBox.Show("Copy information from Google to Outlook for " + c.FullName);
                    IContact newContact = m2.NewContact(c);
                    newContact.Update();
                }
            }

            foreach(IContact oc in l2) {
                bool foundMerge = false;
                foreach (IContact c in l1)
                {
                    if (!c.IsSameAs(oc)) continue;
                    foundMerge = true;
                }
                if (!foundMerge)
                {
                    //System.Windows.Forms.MessageBox.Show("Copy information from Outlook to Google for " + oc.FullName);
                    IContact newContact = m1.NewContact(oc);
                    newContact.Update();
                }
            }
        }
    }
}
