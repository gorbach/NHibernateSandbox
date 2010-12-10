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
            //Arrange
            var p = new Person() { Name = new string('*', 800000) };
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(p);
                tx.Commit();
            }
            Session.Clear();

            //Act
            var actual = Session.Get<Person>(p.Id);
            
            //Assert
            Assert.That(p.Name, Is.EqualTo(actual.Name));
        }
    }
}