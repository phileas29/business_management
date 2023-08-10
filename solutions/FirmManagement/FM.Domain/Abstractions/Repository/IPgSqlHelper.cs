using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM.Domain.Abstractions.Repository
{
    public interface IPgSqlHelper
    {
        IDbConnection GetPgSqlContextHelper();
    }
}
