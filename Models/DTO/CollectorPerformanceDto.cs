namespace DebtRecoveryPlatform.Models.DTO
{
    public class CollectorPerformanceDto
    {
        public string CollectorName { get; set; }

        public decimal AmountAssigned { get; set; }

        public decimal AmountCollected { get; set; }

        public decimal Outstanding { get; set; }

        public decimal Unactioned { get; set; }

        public decimal CollectionRate { get; set; } // Store percentage value, e.g., 70.5
    }
}
