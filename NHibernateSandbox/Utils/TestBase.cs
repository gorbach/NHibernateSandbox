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
            
            //ConfigureForSQLite(configuration);
            //ConfigureForMSSQL(configuration);
            ConfigureForOracle(configuration);

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
                                       .ProxyFactoryFactory(typeof(NHibernate.ByteCode.LinFu.ProxyFactoryFactory))
                                       .FormatSql()
                                       .ShowSql
                                       );
        }

        private void ConfigureForOracle(FluentConfiguration configuration)
        {
            configuration.Database(OracleClientConfiguration.Oracle10
                                       .ConnectionString("Data Source=ORA10.SCORTO.LOCAL;User Id=gor;Password=gor;")
                                       .Dialect(typeof (NHibernate.Dialect.Oracle10gDialect).AssemblyQualifiedName)
                                       .Driver(typeof(NHibernate.Driver.OracleClientDriver).AssemblyQualifiedName)
                                       .ProxyFactoryFactory(typeof(NHibernate.ByteCode.LinFu.ProxyFactoryFactory))
                                       .FormatSql()
                                       .ShowSql
                                       );
        }

        private void ConfigureForSQLite(FluentConfiguration configuration)
        {
            configuration.Database( SQLiteConfiguration.Standard
                                        .ProxyFactoryFactory(typeof(NHibernate.ByteCode.LinFu.ProxyFactoryFactory))
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

            configuration.SetProperty(Environment.GenerateStatistics, "true");
            configuration.SetProperty(Environment.UseMinimalPuts, "true");
            configuration.SetProperty("cache.use_second_level_cache", "true");
            configuration.SetProperty("cache.use_query_cache", "true");
            configuration.SetProperty("cache.provider_class", "NHibernate.Cache.HashtableCacheProvider");

            configuration.SetProperty(Environment.ReleaseConnections, "on_close");

            //new SchemaExport(configuration).Create(true, true);
        }

        public ISession CreateSession()
        {
            ISession session = sessionFactory.OpenSession();
            new SchemaExport(_configuration).Execute(true, true, false, session.Connection, null);
            return session;
        }
    }
}