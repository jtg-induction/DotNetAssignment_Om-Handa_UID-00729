using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Transaction
{
    public interface ITransaction : IDisposable
    {
        void Commit();
    }

    public interface ITransactionRepository
    {
        ITransaction BeginTransaction();
    }
}
