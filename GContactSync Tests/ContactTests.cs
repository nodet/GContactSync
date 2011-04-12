using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GContactSync;

namespace GContactSync_Tests
{

    /// <summary>
    /// Summary description for ContactTests
    /// </summary>
    [TestClass]
    public class ContactTests
    {
        public ContactTests()
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
        public void TestTwoContactsWithSameFullNameAreEqual()
        {
            IContact c1 = new Contact("John Doe");
            IContact c2 = new Contact("John Doe", "john@doe.com");
            Assert.IsTrue(c1.IsSameAs(c2));
        }

        [TestMethod]
        public void TestTwoContactsWithDifferentFullNamesAreNotEqual()
        {
            IContact c1 = new Contact("John Doe");
            IContact c2 = new Contact("Nancy Botwin");
            Assert.IsFalse(c1.IsSameAs(c2));
        }

        [TestMethod]
        public void TestTwoContactsWithSameEmailAreEqual()
        {
            string mail = "john@doe.com";
            IContact c1 = new Contact("", mail);
            IContact c2 = new Contact("", mail);
            Assert.IsTrue(c1.IsSameAs(c2));
        }

        [TestMethod]
        public void TestTwoContactsWithSameEmailAreEqualEvenWithDifferentNames()
        {
            IContact c1 = new Contact("J. Doe", "john@doe.com");
            IContact c2 = new Contact("John Doe", "john@doe.com");
            Assert.IsTrue(c1.IsSameAs(c2));
        }

        [TestMethod]
        public void TestTwoContactsWithCommonEmailAreEqualEvenWithDifferentNames()
        {
            string mail = "john@doe.com";
            IContact c1 = new Contact("J. Doe", mail);
            IContact c2 = new Contact("John Doe", "john+antispam@doe.com");
            Assert.IsFalse(c1.IsSameAs(c2));
            c2.addMail(mail);
            Assert.IsTrue(c1.IsSameAs(c2));
        }

        [TestMethod]
        public void TestCanCopyContact()
        {
            string mail = "john@doe.com";
            IContact c1 = new Contact("J. Doe", mail);
            IContact c2 = new Contact(c1);
            Assert.IsTrue(c1.IsSameAs(c2));
            IEnumerable<string> A = c1.Emails;
            IEnumerable<string> B = c2.Emails;
            Assert.IsTrue(A.Count() == B.Count() && A.Intersect(B).Count() == B.Count());
        }

        [TestMethod]
        public void TestMergingCopiesFullNameIfDestNull()
        {
            string fn = "J. Doe";
            IContact c1 = new Contact(fn);
            IContact c2 = new Contact("");
            c2.MergeFrom(c1);
            Assert.AreEqual(c2.FullName, fn);
        }

        [TestMethod]
        public void TestMergingDoesNotCopyFullNameIfDestNotNull()
        {
            string fn = "J. Doe";
            IContact c1 = new Contact(fn);
            IContact c2 = new Contact("John Doe");
            c2.MergeFrom(c1);
            Assert.AreEqual(c2.FullName, "John Doe");
        }

        [TestMethod]
        public void TestMergingAddsEmailsFromOther()
        {
            IContact c1 = new Contact("J. Doe", "john@doe.com");
            IContact c2 = new Contact("John Doe", "j@doe.com");
            bool merged = c2.MergeFrom(c1);
            Assert.IsTrue(merged);
            Assert.AreEqual(c2.FullName, "John Doe");
            Assert.AreEqual(c2.Emails.Count(), 2);
            Assert.IsTrue(c2.Emails.Contains("john@doe.com"));
            Assert.IsTrue(c2.Emails.Contains("j@doe.com"));
        }

        [TestMethod]
        public void TestMergingDoesNotDuplicateEmails()
        {
            IContact c1 = new Contact("J. Doe", "j@doe.com");
            IContact c2 = new Contact("John Doe", "j@doe.com");
            bool merged = c2.MergeFrom(c1);
            Assert.IsFalse(merged);
            Assert.AreEqual(c2.FullName, "John Doe");
            Assert.AreEqual(c2.Emails.Count(), 1);
            Assert.IsTrue(c2.Emails.Contains("j@doe.com"));
        }
    
        [TestMethod]
        public void TestAnEmptyContactHasNoInformation()
        {
            IContact c1 = new Contact("");
            Assert.IsFalse(c1.ContainsSomeInformation());
        }
        [TestMethod]
        public void TestAContactWithANameHasSomeInformation()
        {
            IContact c1 = new Contact("John Doe");
            Assert.IsTrue(c1.ContainsSomeInformation());
        }
        [TestMethod]
        public void TestAContactWithAMailHasSomeInformation()
        {
            IContact c1 = new Contact("", "john@doe.com");
            Assert.IsTrue(c1.ContainsSomeInformation());
        }
        [TestMethod]
        public void TestInsertingEmptyEmailAddressIsNoop()
        {
            IContact c1 = new Contact("John Doe");
            Assert.IsFalse(c1.addMail(""));
            Assert.AreEqual(c1.Emails.Count(), 0);
        }

    }
}
