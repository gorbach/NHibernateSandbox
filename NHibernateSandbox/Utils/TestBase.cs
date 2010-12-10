using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernateSandbox.Utils
{
    public abstract class TestBase
    {
        protected static ISessionFactory sessionFactory;
        private static Configuration _configuration;
        private List<Assembly> _hbmAssemblies = new List<Assembly>();
        
        protected ISession Session;

        protected TestBase()
        {
            InitialiseNHibernate();
            Session = CreateSession();
        }

        public void InitialiseNHibernate()
        {
            var configuration  = Fluently.Configure();
            
            ConfigureForSQLite(configuration);
            //ConfigureForMSSQL(configuration);

            configuration.Mappings((m) =>
                                        {
                                            m.FluentMappings.Conventions.Add(AutoImport.Never());
                                            m.FluentMappings.AddFromAssembly(GetType().Assembly);
                                        });

            configuration.ExposeConfiguration(BuildSchema);

            sessionFactory = configuration.BuildSessionFactory();
        }

        private void ConfigureForMSSQL(FluentConfiguration configuration)
        {
            configuration.Database(MsSqlConfiguration.MsSql2005
                                       .ConnectionString("Data Source=gor-laptop;Initial Catalog=NHibernateSandbox;Persist Security Info=True;User ID=sa;Password=sa123456;Asynchronous Processing=true;MultipleActiveResultSets=True")
                                       .Dialect(typeof (NHibernate.Dialect.MsSql2005Dialect).AssemblyQualifiedName)
                                       .Driver(typeof(NHibernate.Driver.SqlClientDriver).AssemblyQualifiedName)
                                       .ProxyFactoryFactory(typeof(NHibernate.ByteCode.Castle.ProxyFactoryFactory))
                                       .FormatSql()
                                       .ShowSql
                                       );
        }

        private void ConfigureForSQLite(FluentConfiguration configuration)
        {
            configuration.Database( SQLiteConfiguration.Standard
                                        .ProxyFactoryFactory(typeof(NHibernate.ByteCode.Castle.ProxyFactoryFactory))
                                        .InMemory()
                                        .Dialect(typeof(SQLiteDialect).AssemblyQualifiedName)
                                        .Driver(typeof (SQLite20Driver).AssemblyQualifiedName)
                                        .FormatSql()
                                        .ShowSql
                );
        }

        private static void BuildSchema(Configuration configuration)
        {
            _configuration = configuration;

            configuration.SetProperty(Environment.ReleaseConnections, "on_close");

            //new SchemaExport(configuration).Create(false, true);
        }

        public ISession CreateSession()
        {
            ISession session = sessionFactory.OpenSession();
            new SchemaExport(_configuration).Execute(true, true, false, session.Connection, null);
            return session;
        }
    }
}