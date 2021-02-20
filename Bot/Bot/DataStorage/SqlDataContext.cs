﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Bot.DataStorage
{
    public class SqlDataContext : ISqlDataContext
    {
        private readonly string ConnectionString;
        public SqlDataContext(string connectionString)
        {
            this.ConnectionString = !String.IsNullOrEmpty(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public int ExecuteCommand(string command)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand comm = new SqlCommand(command, conn))
                    {
                        rowsAffected = comm.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                
            }
            catch(Exception ex)
            {
                throw new Exception("Error: Could not execute sql command", ex);
            }

            return rowsAffected;
        }

        public void BulkInsert(DataTable data, IEnumerable<SqlBulkCopyColumnMapping> columnMappings, string tableName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        foreach (var mapping in columnMappings)
                        {
                            bulkCopy.ColumnMappings.Add(mapping);
                        }

                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.WriteToServer(data);
                    }
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not execute sql bulk insert", ex);
            }
        }
    }
}
