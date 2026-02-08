namespace BaSalesManagementApp.MVC.Models.StudentVMs
{
    public class StudentCreateVM
    {
        /// <summary>
        /// Öğrenci Ekleme işleminde öğrencinin ismini temsil eder...
        /// </summary>
        public string? Name{ get; set; }
        /// <summary>
        /// Öğrenci Ekleme işleminde öğrencinin yaşını temsil eder...
        /// </summary>
        public int? Age { get; set; }
    }
}
