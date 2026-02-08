$(document).ready(function () {

    // jQuery yüklü mü kontrol
    if (typeof $ === 'undefined') {
        console.error("❌ jQuery yüklü değil. myprofile.js çalışmaz!");
        return;
    }

    // Fotoğraf önizleme
    $('#photoInput').on('change', function (event) {
        var file = event.target.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#profileImage').attr('src', e.target.result);
                $('#removePhotoInput').val('false');
            };
            reader.readAsDataURL(file);
        }
    });

    // Form submit işlemi (AJAX)
    $('#profileForm').on('submit', function (event) {
        event.preventDefault(); // Normal postu engelle

        var formData = new FormData(this);
        var $alertContainer = $('#alertContainer');

        // Önceki mesajları temizle
        $alertContainer.empty();

        $.ajax({
            type: $(this).attr('method'),
            url: $(this).attr('action'),
            data: formData,
            processData: false,
            contentType: false,
            success: function () {
                $alertContainer.html(`
                    <div class="alert alert-success alert-dismissible fade show mt-3" role="alert">
                        ✅ Profiliniz başarıyla güncellendi!
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    </div>
                `);
            },
            error: function (xhr, status, error) {
                console.error('Form gönderim hatası:', error);
                $alertContainer.html(`
                    <div class="alert alert-danger alert-dismissible fade show mt-3" role="alert">
                        ❌ Profil güncelleme sırasında bir hata oluştu.
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    </div>
                `);
            }
        });
    });

});

// Fotoğraf sil
function removePhoto() {
    $('#profileImage').attr('src', '/assets/img/ProfilePhotos/photo.jpg');
    $('#removePhotoInput').val('true');
}
