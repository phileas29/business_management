namespace FM.Domain.Models
{
    public class ClientGenerateTaxCertificatesServiceModel
    {
        public DateTime Date { get; set; }
        public decimal Duration { get; set; }
        public int HourlyRate { get; set; }
        public int Amount { get; set; }
    }
}
