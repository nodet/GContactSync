﻿using System;
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

    }
}