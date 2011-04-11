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
                        c.Update();
                    }
                    if (oc.MergeFrom(c))
                    {
                        oc.Update();
                    }
                }
                if (!foundMerge)
                {
                    m2.NewContact(c).Update();
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
                    m1.NewContact(oc).Update();
                }
            }
        }
    }
}
