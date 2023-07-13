namespace LittleFirmManagement.Models
{
    public class OutstandingInvoicesViewModel
    {
        public int SubjectId { get; set; }
        public DateTime ActionDate { get; set; }
        public List<int> InvoicesSelected { get; set; }

    }
}
