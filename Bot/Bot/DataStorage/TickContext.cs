using Bot.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Bot.DataStorage.Models
{
    public class TickContext : DbContext
    {
        private readonly string ConnectionString;

        private readonly string TableName;

        public TickContext(string connectionString, string tableName)
            : base(connectionString)
        {
            this.ConnectionString = !String.IsNullOrEmpty(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
            this.TableName = !String.IsNullOrEmpty(tableName) ? tableName : throw new ArgumentNullException(nameof(tableName));
        }

        public DbSet<Tick> Ticks { get; set; }

        public void BulkInsert(IEnumerable<Tick> ticks)
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Symbol", typeof(string)));
            data.Columns.Add(new DataColumn("DateTime", typeof(DateTime)));
            data.Columns.Add(new DataColumn("TickInterval", typeof(int)));
            data.Columns.Add(new DataColumn("Open", typeof(double)));
            data.Columns.Add(new DataColumn("High", typeof(double)));
            data.Columns.Add(new DataColumn("Low", typeof(double)));
            data.Columns.Add(new DataColumn("Close", typeof(double)));
            data.Columns.Add(new DataColumn("AdjClose", typeof(double)));
            data.Columns.Add(new DataColumn("Volume", typeof(double)));

            foreach (Tick t in ticks)
            {
                DataRow row = data.NewRow();
                row["Symbol"] = t.Symbol;
                row["DateTime"] = t.DateTime;
                row["TickInterval"] = t.TickInterval;
                row["Open"] = t.Open;
                row["High"] = t.High;
                row["Low"] = t.Low;
                row["Close"] = t.Close;
                row["AdjClose"] = t.AdjClose;
                row["Volume"] = t.Volume;
                data.Rows.Add(row);
            }

            List<SqlBulkCopyColumnMapping> columnMappings = new List<SqlBulkCopyColumnMapping>()
            {
               new SqlBulkCopyColumnMapping("Symbol", "Symbol"),
               new SqlBulkCopyColumnMapping("DateTime", "DateTime"),
               new SqlBulkCopyColumnMapping("TickInterval", "TickInterval"),
               new SqlBulkCopyColumnMapping("Open", "Open"),
               new SqlBulkCopyColumnMapping("High", "High"),
               new SqlBulkCopyColumnMapping("Low", "Low"),
               new SqlBulkCopyColumnMapping("Close", "Close"),
               new SqlBulkCopyColumnMapping("AdjClose", "AdjClose"),
               new SqlBulkCopyColumnMapping("Volume", "Volume")
            };

            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    columnMappings.ForEach(t => bulkCopy.ColumnMappings.Add(t));

                    bulkCopy.DestinationTableName = this.TableName;
                    bulkCopy.WriteToServer(data);
                }
                conn.Close();
            }
        }

        public void DeleteTicksForSymbol(string symbol, TickInterval tickInterval)
        {
            string sqlCommand = "DELETE FROM " + this.TableName + " WHERE [Symbol] = '" + symbol + "' AND [TickInterval] = " + (int)tickInterval;
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(sqlCommand, conn))
                {
                    int rowsAffected = comm.ExecuteNonQuery();
                    Console.WriteLine("Deleted " + rowsAffected + " rows from dbo.[" + this.TableName + "] where [Symbol] = " + symbol + " AND [TickInterval] = " + tickInterval);
                }
                conn.Close();
            }            
        }
                
    }
}
