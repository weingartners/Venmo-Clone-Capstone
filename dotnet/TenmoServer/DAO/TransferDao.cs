using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class TransferDao : ITransferDao
    {

        private readonly string connectionString;
        public TransferDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public bool SaveTransfer(int sendingUserId, decimal transferAmount, int receivingUserId, int typeId, int statusId)
        {

            try
            {
                int sendingAccountId = 0;
                int receivingAccountId = 0;
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();


                    SqlCommand cmd = new SqlCommand("SELECT account_id FROM accounts WHERE user_id = @user_id", conn);
                    cmd.Parameters.AddWithValue("@user_id", sendingUserId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        sendingAccountId = Convert.ToInt32(reader["account_id"]);
                    }
                    reader.Close();
                    SqlCommand cmd2 = new SqlCommand("SELECT account_id FROM accounts WHERE user_id = @user_id", conn);
                    cmd2.Parameters.AddWithValue("@user_id", receivingUserId);
                    SqlDataReader reader2 = cmd2.ExecuteReader();
                    if (reader2.Read())
                    {
                        receivingAccountId = Convert.ToInt32(reader2["account_id"]);
                    }
                    reader2.Close();

                    cmd = new SqlCommand("INSERT INTO transfers(transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES(@transfer_type_id, @transfer_status_id, @account_from, @account_to, @amount)", conn);
                    cmd.Parameters.AddWithValue("@transfer_type_id", typeId);
                    cmd.Parameters.AddWithValue("@transfer_status_id", statusId);
                    cmd.Parameters.AddWithValue("@account_from",sendingAccountId);
                    cmd.Parameters.AddWithValue("@account_to", receivingAccountId);
                    cmd.Parameters.AddWithValue("@amount", transferAmount);
                    cmd.ExecuteNonQuery();

                    

                    return true;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public List<Transfer> GetTransfersById(int id)
        {
            List<Transfer> transfers = new List<Transfer>();
            try
            {
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    
                    SqlCommand cmd = new SqlCommand("SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.amount, a.user_id AS sending_id, u.username AS sending_username, aa.user_id AS receiving_id, uu.username AS receiving_username " +
                                                    "FROM transfers t " +
                                                    "JOIN accounts a ON a.account_id = t.account_from " +
                                                    "JOIN users u ON u.user_id = a.user_id " +
                                                    "JOIN accounts aa ON aa.account_id = t.account_to " +
                                                    "JOIN users uu ON uu.user_id = aa.user_id " +
                                                    "WHERE u.user_id = @user_id; ", conn);
                    cmd.Parameters.AddWithValue("@user_id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        transfers.Add(GetTransferFromReader(reader));
                    }
                    reader.Close();
                    
                    SqlCommand cmd2 = new SqlCommand("SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.amount, a.user_id AS sending_id, u.username AS sending_username, aa.user_id AS receiving_id, uu.username AS receiving_username FROM transfers t JOIN accounts a ON a.account_id = t.account_from JOIN users u ON u.user_id = a.user_id JOIN accounts aa ON aa.account_id = t.account_to JOIN users uu ON uu.user_id = aa.user_id WHERE uu.user_id = @user_id; ", conn);
                    cmd2.Parameters.AddWithValue("@user_id", id);
                    SqlDataReader reader2 = cmd2.ExecuteReader();
                    
                    while (reader2.Read())
                    {
                        transfers.Add(GetTransferFromReader(reader2));
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return transfers;
        }

        public List<Transfer> GetRequests(int id)
        {
                List<Transfer> transfers = new List<Transfer>();

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.amount, a.user_id AS sending_id, u.username AS sending_username, aa.user_id AS receiving_id, uu.username AS receiving_username " +
                                                    "FROM transfers t " +
                                                    "JOIN accounts a ON a.account_id = t.account_from " +
                                                    "JOIN users u ON u.user_id = a.user_id " +
                                                    "JOIN accounts aa ON aa.account_id = t.account_to " +
                                                    "JOIN users uu ON uu.user_id = aa.user_id " +
                                                    "WHERE u.user_id = @user_id AND t.transfer_status_id = 1; ", conn);
                    cmd.Parameters.AddWithValue("@user_id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        transfers.Add(GetTransferFromReader(reader));
                    }
                }
                }
                catch (SqlException)
                {
                    throw;
                }
            return transfers;
            }

        public void UpdateStatus(int transferId, int statusId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE transfers SET transfer_status_id = @transfer_status_id WHERE transfer_id = @transfer_id;", conn);
                    cmd.Parameters.AddWithValue("@transfer_id", transferId);
                    cmd.Parameters.AddWithValue("@transfer_status_id", statusId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer t = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TypeId = Convert.ToInt32(reader["transfer_type_id"]),
                StatusId = Convert.ToInt32(reader["transfer_status_id"]),
                TransferAmount = Convert.ToDecimal(reader["amount"]),
                SendingUserId = Convert.ToInt32(reader["sending_id"]),
                FromUserName = Convert.ToString(reader["sending_username"]),
                ToUserName = Convert.ToString(reader["receiving_username"]),
                ReceivingUserId = Convert.ToInt32(reader["receiving_id"])

            };
            return t;
        }
    }
}
