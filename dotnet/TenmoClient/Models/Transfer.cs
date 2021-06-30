using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    class Transfer
    {
        public Transfer()
        {

        }
        public Transfer(int sendingUserId, decimal transferAmount, int recievingUserId, string type, string status)
        {
            SendingUserId = sendingUserId;
            TransferAmount = transferAmount;
            RecievingUserId = recievingUserId;
            Type = type;
            Status = status;
        }

        public int SendingUserId { get; set; }
        public decimal TransferAmount { get; set; }
        public int RecievingUserId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; } = "Approved";
    }
}
