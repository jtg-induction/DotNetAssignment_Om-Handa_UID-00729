using DotNet_Assignment.Data;
using DotNet_Assignment.Repository.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DotNet_Assignment.Repository.Transaction
{
    public class Transaction : ITransaction {

        private readonly DbContextTransaction _transaction;

        public Transaction(DbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        /// <summary>
        /// Commits a transaction
        /// </summary>
        public void Commit()
        {
            _transaction.Commit();
        }

        /// <summary>
        /// Disposes a transaction
        /// </summary>
        public void Dispose()
        {
            _transaction.Dispose();
        }
    }

    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Marks as beginning of a transaction
        /// </summary>
        /// <returns>Transaction</returns>
        public ITransaction BeginTransaction()
        {
            return new Transaction(_context.Database.BeginTransaction());
        }
    }
}
