document.addEventListener('DOMContentLoaded', function () {
    var deleteModal = document.getElementById('deleteModal');
    deleteModal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;
        var adminId = button.getAttribute('data-admin-id');
        var confirmDeleteButton = deleteModal.querySelector('#confirmDeleteButton');
        confirmDeleteButton.href = '/Admin/Delete?adminId=' + adminId;
    });
});
