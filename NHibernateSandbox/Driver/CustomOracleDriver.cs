    using System.Data;
    using System.Data.OracleClient;
    using NHibernate.Driver;
    using NHibernate.SqlTypes;

    
namespace NHibernateSandbox.Driver
{
    /// <summary>
    /// This provider fixes oracle problem with clobs.
    /// if the sqltype is nclob, and string 2000 > len < 4000
    /// then it throws strange exception.
    /// Just use this provider and specify in your fluent mapping clobs like this
    /// Map(x => x.Name).CustomType("StringClob");
    /// </summary>
    public class CustomOracleDriver : OracleClientDriver
    {
        protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType)
        {
            base.InitializeParameter(dbParam, name, sqlType);
            if (sqlType is StringClobSqlType)
            {
                ((OracleParameter)dbParam).OracleType = OracleType.NClob;
            }
        }
    }
}