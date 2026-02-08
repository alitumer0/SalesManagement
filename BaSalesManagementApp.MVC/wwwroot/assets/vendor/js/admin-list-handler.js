document.addEventListener('DOMContentLoaded', function () {
    var sortAdminSelect = document.getElementById('sortAdmin');
    var sortAdminLinks = sortAdminSelect.querySelectorAll('.dropdown-item');

    function updateSortAdminFromUrl() {
        var urlParams = new URLSearchParams(window.location.search);
        var sortAdmin = urlParams.get('sortAdmin') || 'name';
        sortAdminLinks.forEach(function (link) {
            if (link.getAttribute('data-value') === sortAdmin) {
                link.classList.add('active');
            } else {
                link.classList.remove('active');
            }
        });
    }

    function updatePageLinks() {
        var selectedSortAdmin = document.querySelector('.dropdown-item.active').getAttribute('data-value');

        document.querySelectorAll('.pagination a').forEach(function (link) {
            var href = link.getAttribute('href');
            var url = new URL(href, window.location.href);
            var urlParams = new URLSearchParams(url.search);

            urlParams.set('sortAdmin', selectedSortAdmin);
            url.search = urlParams.toString();

            link.setAttribute('href', url.toString());
        });
    }

    function handleSortAdminChange(event) {
        var selectedSortAdmin = event.target.getAttribute('data-value');
        var urlParams = new URLSearchParams(window.location.search);
        urlParams.set('sortAdmin', selectedSortAdmin);
        urlParams.set('page', 1);
        window.location.search = urlParams.toString();
    }

    sortAdminLinks.forEach(function (link) {
        link.addEventListener('click', handleSortAdminChange);
    });

    updateSortAdminFromUrl();
    updatePageLinks();
});
