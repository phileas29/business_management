namespace LittleFirmManagement.Models
{
    public class FCategoriesViewModel
    {
        public List<FCategoryType> CategoryTypes { get; set; }
        public Dictionary<int, List<FCategory>> CategoriesByType { get; set; }
    }
}
