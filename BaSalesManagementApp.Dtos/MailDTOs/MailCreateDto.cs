namespace BaSalesManagementApp.Dtos.MailDTOs
{
    /// <summary>
    /// Yeni bir e-posta mesajı oluşturmak için kullanılan Veri Transfer Nesnesi (DTO)'yi temsil eder.
    /// </summary>
    public class MailCreateDto
    {
        /// <summary>
        /// E-postanın başlığını alır veya ayarlar.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// E-postanın konusunu alır veya ayarlar.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// E-postanın içerik gövdesini alır veya ayarlar.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Alıcının e-posta adresini alır veya ayarlar.
        /// </summary>
        public string ReceiverMailAddress { get; set; }

    }
}
