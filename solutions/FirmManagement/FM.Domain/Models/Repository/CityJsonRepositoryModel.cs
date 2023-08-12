namespace FM.Domain.Models.Repository
{
    public class CityJsonRepositoryModel
    {
        public string? Nom { get; set; }
        public string? Code { get; set; }
        public string? CodeDepartement { get; set; }
        public List<string>? CodesPostaux { get; set; }
    }
}
