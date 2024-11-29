namespace InquiryPortal.Models
{
    public class InvoiceInquiry
    {
        public string Igm { get; set; }

        public int IndexNo { get; set; }

        public DateTime BillDate { get; set; }

        public decimal Totalwithoutgst { get; set; } // Add more fields as required

        public decimal Totalwithgst { get; set; } // Add more fields as required
    }
}
