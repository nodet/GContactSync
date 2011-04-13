using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GContactSync;

namespace GContactSync_Tests
{

    /// <summary>
    /// Summary description for ContactMergerTests
    /// </summary>
    [TestClass]
    public class ContactMergerTests
    {
        public ContactMergerTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        public void TestMergeBothWays()
        {
            Contact c1 = new Contact("John Doe", "j@doe.com");
            IContactManager m1 = new MockContactManager
            {
                GetContactsImpl = () =>
                {
                    var l = new List<IContact>();
                    l.Add(c1);
                    return l;
                }

            };
            Contact c2 = new Contact("John Doe", "john@doe.com");
            IContactManager m2 = new MockContactManager
            {
                GetContactsImpl = () =>
                {
                    var l = new List<IContact>();
                    l.Add(c2);
                    return l;
                }

            };
            Assert.IsTrue (c1.Emails.Contains("j@doe.com"));
            Assert.IsFalse(c2.Emails.Contains("j@doe.com"));
            Assert.IsFalse(c1.Emails.Contains("john@doe.com"));
            Assert.IsTrue (c2.Emails.Contains("john@doe.com"));
            ContactMerger.Merge(m1, m2, m1.GetContacts(), m2.GetContacts());
            Assert.IsTrue(c1.Emails.Contains("j@doe.com"));
            Assert.IsTrue(c2.Emails.Contains("j@doe.com"));
            Assert.IsTrue(c1.Emails.Contains("john@doe.com"));
            Assert.IsTrue(c2.Emails.Contains("john@doe.com"));
        }

        [TestMethod]
        public void TestMergeOneWay()
        {
            List<IContact> l1 = new List<IContact>();
            Contact c1 = new Contact("John Doe", "j@doe.com");
            l1.Add(c1);
            List<IContact> l2 = new List<IContact>();

            IContactManager m1 = new MockContactManager {
                GetContactsImpl = () => { return l1; }
            };
            IContactManager m2 = new MockContactManager
            {
                GetContactsImpl = () => { return l2; }
            };
            ContactMerger.Merge(m1, m2, m1.GetContacts(), m2.GetContacts());

            Assert.AreEqual(l2.Count(), 1);
            IContact c2 = l2.ElementAt(0);
            Assert.AreEqual(c2.FullName, "John Doe");
            Assert.IsTrue(c2.Emails.Contains("j@doe.com"));
        }


        [TestMethod]
        public void TestMergeTheOtherWay()
        {
            List<IContact> l1 = new List<IContact>();
            List<IContact> l2 = new List<IContact>();
            Contact c2 = new Contact("John Doe", "j@doe.com");
            l2.Add(c2);

            IContactManager m1 = new MockContactManager
            {
                GetContactsImpl = () => { return l1; }
            };
            IContactManager m2 = new MockContactManager
            {
                GetContactsImpl = () => { return l2; }
            };
            ContactMerger.Merge(m1, m2, m1.GetContacts(), m2.GetContacts());

            Assert.AreEqual(l1.Count(), 1);
            IContact c1 = l1.ElementAt(0);
            Assert.AreEqual(c1.FullName, "John Doe");
            Assert.IsTrue(c1.Emails.Contains("j@doe.com"));
        }


        [TestMethod]
        public void TestContactIndexerFindsExistingContacts()
        {
            List<IContact> contacts = new List<IContact>();
            contacts.Add(new Contact("John Doe"));
            contacts.Add(new Contact("Another contact"));
            contacts.Add(new Contact("", "j@doe.com"));
            contacts.Add(new Contact("", "j@doe.com"));
            contacts.Add(new Contact((string)null));

            IContact c = new Contact("MultiMail");
            c.addMail("mm@foo.com");
            c.addMail("multimail@foo.com");
            contacts.Add(c);

            ContactIndexer indexer = new ContactIndexer(contacts);
            Assert.AreEqual(1, indexer.GetContactsFor("John Doe").Count());
            Assert.AreEqual(0, indexer.GetContactsFor("Foo Bar").Count());
            Assert.AreEqual(2, indexer.GetContactsFor("j@doe.com").Count());
            Assert.AreEqual(1, indexer.GetContactsFor("MultiMail").Count());
            Assert.AreEqual(1, indexer.GetContactsFor("mm@foo.com").Count());
            Assert.AreEqual(1, indexer.GetContactsFor("multimail@foo.com").Count());
            Assert.AreEqual(0, indexer.GetContactsFor(null).Count());

            Assert.AreEqual(1, indexer.GetSameContactsAs(new Contact("MultiMail")).Count());
            // Can match Name to email...
            Assert.AreEqual(2, indexer.GetSameContactsAs(new Contact("j@doe.com")).Count());
        }

        [TestMethod]
        public void TestMergerPerformance()
        {
            // Runtime should be pretty negligible, around 0.1s
            const int nb = 5000;
            List<IContact> l1 = new List<IContact>();
            List<IContact> l2 = new List<IContact>();
            for (int i = 0; i < nb; ++i)
            {
                string s1 = Guid.NewGuid().ToString();
                string s2 = Guid.NewGuid().ToString();
                string s3 = Guid.NewGuid().ToString();
                IContact c1 = new Contact(s1);
                c1.addMail(s2);
                c1.addMail(s3);
                l1.Add(c1);
                IContact c2 = new Contact(s1);
                c2.addMail(s2);
                c2.addMail(s3);
                l2.Add(c2);
            }
            IContactManager m1 = new MockContactManager
            {
                GetContactsImpl = () => { return l1; }
            };
            IContactManager m2 = new MockContactManager
            {
                GetContactsImpl = () => { return l2; }
            };
            ContactMerger.Merge(m1, m2, m1.GetContacts(), m2.GetContacts());
        }


        [TestMethod]
        public void TestWithNullNameAndSameMailIsNoop()
        {
            Contact c1 = new Contact("", "j@doe.com");
            Contact c2 = new Contact(null, "j@doe.com");
            Assert.IsFalse(c1.MergeFrom(c2));
            Assert.IsFalse(c2.MergeFrom(c1));
        }



    }
}