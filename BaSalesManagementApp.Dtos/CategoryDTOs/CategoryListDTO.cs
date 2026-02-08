namespace BaSalesManagementApp.Dtos.CategoryDTOs
{
    public class CategoryListDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CompanyId { get; set; }
    }
}
