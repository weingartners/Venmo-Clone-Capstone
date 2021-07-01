namespace TenmoClient.Models
{
    class Transfer
    {
        public Transfer()
        {

        }
        public Transfer(int sendingUserId, decimal transferAmount, int receivingUserId, int typeId, int statusId)
        {
            SendingUserId = sendingUserId;
            TransferAmount = transferAmount;
            ReceivingUserId = receivingUserId;
            TypeId = typeId;
            StatusId = statusId;
            
        }

        public int TransferId { get; set; }
        public int SendingUserId { get; set; }
        public decimal TransferAmount { get; set; }
        public int ReceivingUserId { get; set; }
        
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
}
