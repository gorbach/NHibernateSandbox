using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernateSandbox.Utils
{
    public class SQLiteDialect : NHibernate.Dialect.SQLiteDialect
    {
        public override bool HasDataTypeInIdentityColumn
        {
            get { return false; }
        }

        public override string IdentityColumnString
        {
            get { return "integer"; }
        }

        public override bool SupportsIdentityColumns
        {
            get { return true; }
        }
    }
}
