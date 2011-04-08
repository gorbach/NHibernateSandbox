using System.Linq;
using NHibernate.Linq;
using NHibernateSandbox.Model;
using NHibernateSandbox.Utils;
using NUnit.Framework;

namespace NHibernateSandbox.Tests
{
    [TestFixture]
    public class PersonFixture : TestBase
    {
        [Test]
        public void CanInsertPerson ()
        {
            Session = CreateSession();
            //Arrange
            var p1 = new Person() { Name = new string('*', 1000) };
            var p3 = new Person() { Name = new string('*', 3000) };
            var p5 = new Person() { Name = new string('*', 5000) };
            using(var tx = Session.BeginTransaction())
            {
                Session.Save(p1);
                Session.Save(p3);
                Session.Save(p5);
                tx.Commit();
            }
            Session.Clear();

            //Act
            var actual1 = Session.Get<Person>(p1.Id);
            var actual3 = Session.Get<Person>(p3.Id);
            var actual5 = Session.Get<Person>(p5.Id);
            
            //Assert
            Assert.That(p1.Name, Is.EqualTo(actual1.Name));
            Assert.That(p3.Name, Is.EqualTo(actual3.Name));
            Assert.That(p5.Name, Is.EqualTo(actual5.Name));
        }

        [Test]
        public void CanQueryWithBoolTypeBySimpleLinq()
        {
            Session = CreateSession();
            //Arrange
            var p1 = new Person() { Name = "P1", Deleted = false};
            var p3 = new Person() { Name = "P2", Deleted = false };
            var p5 = new Person() { Name = "P3", Deleted = true };
            
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(p1);
                Session.Save(p3);
                Session.Save(p5);
                tx.Commit();
            }

            Session.Clear();

            //Act
            var actual = Session.Query<Person>().Where(p => p.Deleted).ToList();

            //Assert
            Assert.That(actual.Count, Is.EqualTo(2));
        }
        [Test]
        public void CanQueryWithBoolType()
        {
            Session = CreateSession();
            //Arrange
            var p1 = new Person() { Name = "P1", Deleted = false};
            var p3 = new Person() { Name = "P2", Deleted = false };
            var p5 = new Person() { Name = "P3", Deleted = true };
            
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(p1);
                Session.Save(p3);
                Session.Save(p5);
                tx.Commit();
            }

            Session.Clear();

            //Act
            var actual = Session.Query<Person>().Where(p => p.Deleted == true).ToList();

            //Assert
            Assert.That(actual.Count, Is.EqualTo(2));
        }
    }
}