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
        public Transfer(int sendingUserId, decimal transferAmount, int recievingUserId, int typeId, int statusId)
        {
            SendingUserId = sendingUserId;
            TransferAmount = transferAmount;
            RecievingUserId = recievingUserId;
            TypeId = typeId;
            StatusId = statusId;

        }

        public int SendingUserId { get; set; }
        public decimal TransferAmount { get; set; }
        public int RecievingUserId { get; set; }
        
        public int TypeId { get; set; }
        public int StatusId { get; set; }
    }
}
