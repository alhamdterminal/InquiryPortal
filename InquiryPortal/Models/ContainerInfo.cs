namespace InquiryPortal.Models
{
    public class ContainerInfo
    {
        public string? ContainerNo { get; set; }
        public string? igmNo { get; set; }
        public string? BLNo { get; set; }
        public string? IndexNo { get; set; }
        public string? Line { get; set; }
        public object? Size { get; set; }
        public string? SizeType { get; set; }
        public string? Category { get; set; }
        public string? GoodsDescription { get; set; }
        public string? PortOfDischarge { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DeliverDate { get; set; }
        public string? Vessel { get; set; }
        public string? Voyage { get; set; }
        public string? ShipperSeal { get; set; }
        public DateTime? PortDischargeDate { get; set; }
        public decimal? GrossWeight { get; set; }
    }
}
