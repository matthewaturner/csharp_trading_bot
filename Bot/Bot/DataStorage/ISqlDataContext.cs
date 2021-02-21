using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Bot.DataStorage
{
    public interface ISqlDataContext
    {
        public int ExecuteCommand(string command);

        public void BulkInsert(DataTable data, IEnumerable<SqlBulkCopyColumnMapping> columnMappings, string tableName);
    }
}
