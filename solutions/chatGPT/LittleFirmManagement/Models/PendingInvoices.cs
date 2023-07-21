namespace LittleFirmManagement.Models
{
    public class PendingInvoicesViewModel
    {
        public int SubjectId { get; set; }
        public DateTime ActionDate { get; set; }
        public List<int> InvoicesSelected { get; set; }

    }
}
