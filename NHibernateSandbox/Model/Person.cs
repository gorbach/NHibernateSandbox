using FluentNHibernate.Mapping;
using NHibernateSandbox.Model;

namespace NHibernateSandbox.Model
{
    public class Person
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}

namespace NHibernateSandbox.Mappings
{
    public class PersonMap : ClassMap<Person>
    {
        public PersonMap()
        {
            Id(x => x.Id).GeneratedBy.Native("SEQ_PERSON");
            Map(x => x.Name).Length(512);
        }
    }
}