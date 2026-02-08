$(document).ready(function () {

    // Ad alanını doğrulayan fonksiyon
    function validateName() {
        const name = $("#Name").val().trim();

        if (name.length === 0) {
            $("#nameError").text(nameRequiredMessage);
        }
        else {
            $("#nameError").text("");
        }
    }

    // Kategori seçimini doğrulayan fonksiyon
    function validateCategory() {
        const categoryId = $('#categorySelect').val();

        if (categoryId === "") {
            $('#categoryError').text(categoryNameRequiredMessage);
        } else {
            $('#categoryError').text("");
        }
    }

    // Şirket seçimini doğrulayan fonksiyon
    function validateCompany() {
        const companyId = $('#companySelect').val();

        if (companyId === "") {
            $('#companyError').text(companyNameRequiredMessage);
        } else {
            $('#companyError').text("");
        }
    }


    // Fiyat için validation
    function validatePrice() {
        const price = $("#Price").val().trim();

        if (price.length === 0) {
            $("#priceError").text(priceRequiredMessage);
        } else if (isNaN(price) || parseFloat(price) <= 0) {
            $("#priceError").text(priceInvalidMessage);
        } else if (!/^\d+(\.\d{1,2})?$/.test(price)) {
            $("#priceError").text(priceInvalidMessage);
        } else {
            $("#priceError").text("");
        }
    }


    $("#Name").on('keyup blur', validateName);
    $('#companySelect').on('change', validateCompany);
    $('#categorySelect').on('change', validateCategory);
    $('#Price').on('keyup blur', validatePrice);

    $("#employeeForm").submit(function (event) {
        validateName();
        validateCompany();
        validateCategory();
        validatePrice();
        let isValid = true;


        if ($("#nameError").text() || $("#companyError").text() || $("#priceError").text() || $("#categoryError").text()) {
            isValid = false;
        }

        if (!isValid) {
            event.preventDefault();
        }
    });
});

