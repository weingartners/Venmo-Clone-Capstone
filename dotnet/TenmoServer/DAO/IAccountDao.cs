using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        List<decimal> GetBalances(int userId);

        List<Account> GetAccounts(int userId);

    }
}
