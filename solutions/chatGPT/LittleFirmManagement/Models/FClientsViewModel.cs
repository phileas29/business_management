namespace LittleFirmManagement.Models
{
    public class FClientsViewModel
    {
        public List<FCategoryType> CategoryTypes { get; set; }
        public Dictionary<int, List<FCategory>> CategoriesByType { get; set; }
    }
}
