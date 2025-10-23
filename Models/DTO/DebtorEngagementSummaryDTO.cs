namespace DebtRecoveryPlatform.Models.DTO
{
    public class DebtorEngagementSummaryDTO
    {
        public string Status { get; set; }

        public int Count { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PortfolioPercent { get; set; } // Store percentage value, e.g., 70.5
    }
}
