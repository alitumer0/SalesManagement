namespace BaSalesManagementApp.Dtos.CategoryDTOs
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Guid? CompanyId { get; set; }


    }
}
