namespace BaSalesManagementApp.MVC.Extensions
{
    public static class FormFileExtensions
    {

        /// <summary>
        /// Fotoğraf verisini Base64 formatina dönüştürür
        /// </summary>
        /// <param name="fileBytes">Dönüştürülecek byte dizisi</param>
        /// <returns>Base64 string formatında fotoğraf</returns>
        public static string? ToBase64String(this byte[]? fileBytes)
        {
            if (fileBytes == null || fileBytes.Length == 0)
                return null;
            return Convert.ToBase64String(fileBytes);
        }


        /// <summary>
        /// CompanyPhoto'yu byte array tipine dönüştürme metodu
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[]? ConvertFormFileToByteArray(this IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }
}
