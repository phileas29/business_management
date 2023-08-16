namespace FM.Domain.Models.Service
{
    public class InvoiceCreateServiceModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string CombinedDateAndDescription => $"{Date:MM/dd/yyyy} - {Description}";
    }
}
