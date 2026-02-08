$(document).ready(function () {


    function validateFirstName() {
        const firstName = $("#FirstName").val().trim();

        if (firstName.length === 0) {
            $("#firstNameError").text(firstNameRequiredMessage);
        } else if (firstName.length < 2) {
            $("#firstNameError").text(firstNameMinLengthMessage);
        } else if (firstName.length > 50) {
            $("#firstNameError").text(firstNameMaxLengthMessage);
        } else if (/\d/.test(firstName)) {
            $("#firstNameError").text(firstNameNumericForbiddenMessage);
        } else {
            $("#firstNameError").text(""); // Hata yok
        }
    }


    function validateLastName() {
        const lastName = $("#LastName").val().trim();

        if (lastName.length === 0) {
            $("#lastNameError").text(lastNameRequiredMessage);
        } else if (lastName.length < 2) {
            $("#lastNameError").text(lastNameMinLengthMessage);
        } else if (lastName.length > 50) {
            $("#lastNameError").text(lastNameMaxLengthMessage);
        } else if (/\d/.test(lastName)) {
            $("#lastNameError").text(lastNameNumericForbiddenMessage);
        } else {
            $("#lastNameError").text("");
        }
    }


    function validateEmail() {
        const email = $("#Email").val().trim();
        const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        const allowedDomains = ["gmail.com", "yahoo.com", "outlook.com", "bilgeadam.com", "bilgeadamboost.com", "hotmail.com"];

        if (email.length === 0) {
            $("#emailError").text(emailRequiredMessage);
        } else if (!emailPattern.test(email)) {
            $("#emailError").text(emailValidRequiredMessage);
        } else {
            const emailDomain = email.split('@')[1];
            if (!allowedDomains.includes(emailDomain)) {
                $("#emailError").text(emailInvalidDomainMessage);
            } else {
                $("#emailError").text("");
            }
        }
    }


    function validatePhoto() {
        const photo = $("#Photo")[0].files[0];
        const maxFileSize = 500 * 1024;
        const allowedExtensions = [".jpg", ".jpeg", ".png"];

        if (photo) {
            const fileExtension = photo.name.split('.').pop().toLowerCase();

            if (photo.size > maxFileSize) {
                $("#photoError").text(photoSizeExceededMessage);
            } else if (!allowedExtensions.includes("." + fileExtension)) {
                $("#photoError").text(photoInvalidFileTypeMessage);
            } else {
                $("#photoError").text("");
            }
        } else {
            $("#photoError").text("");
        }
    }

    function validateCompany() {
        const companyId = $('#companySelect').val();

        if (companyId === "") {
            $('#companyError').text(companyNameRequiredMessage);
        }
        else {
            $('#companyError').text("");
        }
    }

    $("#FirstName").on('keyup blur', validateFirstName);
    $("#LastName").on('keyup blur', validateLastName);
    $("#Email").on('keyup blur', validateEmail);
    $("#Photo").on('change', validatePhoto);
    $('#companySelect').on('change', validateCompany);

    $("#employeeForm").submit(function (event) {
        validateFirstName();
        validateLastName();
        validateEmail();
        validatePhoto();
        validateCompany();

        let isValid = true;

        if ($("#firstNameError").text() || $("#lastNameError").text() || $("#emailError").text() || $("#photoError").text() || $('#companyError').text()) {
            isValid = false;
        }

        if (!isValid) {
            event.preventDefault();
        }
    });
});
