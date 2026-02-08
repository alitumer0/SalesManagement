// wwwroot/assets/Languages/Language.js

document.addEventListener('DOMContentLoaded', function () {
    var dropdownToggle = document.getElementById('dropdownMenuButton');
    var dropdownMenu = document.getElementById('Languages');

    dropdownToggle.addEventListener('click', function (event) {
        event.stopPropagation(); // Tıklama olayının dışa yayılmasını engeller
        dropdownMenu.classList.toggle('show'); // Menü görünürlüğünü değiştirir
    });

    // Menü dışında bir yere tıklanırsa menüyü kapat
    document.addEventListener('click', function () {
        dropdownMenu.classList.remove('show');
    });
});
