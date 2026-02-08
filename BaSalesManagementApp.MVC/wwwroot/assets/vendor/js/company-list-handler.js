//document.addEventListener('DOMContentLoaded', function () {
//    var sortCompanySelect = document.getElementById('sortOrderCompany');
//    var sortCompanyLinks = sortCompanySelect.querySelectorAll('.dropdown-item');
//    var pageSizeSelect = document.getElementById('pageSizeSelect');

    //function updateSortCompanyFromUrl() {
    //    var urlParams = new URLSearchParams(window.location.search);
    //    var sortCompany = urlParams.get('sortOrderCompany') || 'name_asc';
    //    sortCompanyLinks.forEach(function (link) {
    //        if (link.getAttribute('data-value') === sortCompany) {
    //            link.classList.add('active');
    //        } else {
    //            link.classList.remove('active');
    //        }
    //    });
    //}

    //function updatePageLinks() {
    //    var selectedSortCompany = document.querySelector('.dropdown-item.active').getAttribute('data-value');

    //    document.querySelectorAll('.pagination a').forEach(function (link) {
    //        var href = link.getAttribute('href');
    //        var url = new URL(href, window.location.href);
    //        var urlParams = new URLSearchParams(url.search);

    //        urlParams.set('sortOrderCompany', selectedSortCompany);
    //        url.search = urlParams.toString();

    //        link.setAttribute('href', url.toString());
    //    });
    //}

    //function handleSortCompanyChange(event) {
    //    var selectedSortCompany = event.target.getAttribute('data-value');
    //    var urlParams = new URLSearchParams(window.location.search);
    //    urlParams.set('sortOrderCompany', selectedSortCompany);
    //    urlParams.set('page', 1);
    //    window.location.search = urlParams.toString();
    //}

    //sortCompanyLinks.forEach(function (link) {
    //    link.addEventListener('click', handleSortCompanyChange);
    //});


    /*updateSortCompanyFromUrl();*/
    /*updatePageLinks();*/

});