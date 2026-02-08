function togglePasswordVisibility(id) {
    var input = document.getElementById(id);
    var icon = input.nextElementSibling.querySelector('i');
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.replace('bx-hide', 'bx-show');
    } else {
        input.type = 'password';
        icon.classList.replace('bx-show', 'bx-hide');
    }
}

document.addEventListener('DOMContentLoaded', function () {
    var newPassword = document.getElementById('newPassword');
    var confirmNewPassword = document.getElementById('confirmNewPassword');
    var passwordRequirements = document.getElementById('password-requirements');
    var passwordStrength = document.getElementById('password-strength');
    var passwordMatch = document.getElementById('password-match');

    var lengthRequirement = document.getElementById('length');
    var uppercaseRequirement = document.getElementById('uppercase');
    var numberRequirement = document.getElementById('number');
    var specialRequirement = document.getElementById('special');

    newPassword.addEventListener('input', function () {
        var value = newPassword.value;
        var strength = 0;

        if (value.length >= 8) {
            lengthRequirement.classList.remove('invalid');
            lengthRequirement.classList.add('valid');
            strength++;
        } else {
            lengthRequirement.classList.remove('valid');
            lengthRequirement.classList.add('invalid');
        }

        if (/[A-Z]/.test(value)) {
            uppercaseRequirement.classList.remove('invalid');
            uppercaseRequirement.classList.add('valid');
            strength++;
        } else {
            uppercaseRequirement.classList.remove('valid');
            uppercaseRequirement.classList.add('invalid');
        }

        if (/\d/.test(value)) {
            numberRequirement.classList.remove('invalid');
            numberRequirement.classList.add('valid');
            strength++;
        } else {
            numberRequirement.classList.remove('valid');
            numberRequirement.classList.add('invalid');
        }

        if (/[!@#$%^&*(),.?":{}|<>]/.test(value)) {
            specialRequirement.classList.remove('invalid');
            specialRequirement.classList.add('valid');
            strength++;
        } else {
            specialRequirement.classList.remove('valid');
            specialRequirement.classList.add('invalid');
        }

        switch (strength) {
            case 1:
                passwordStrength.textContent = 'Weak';
                passwordStrength.className = 'weak';
                break;
            case 2:
                passwordStrength.textContent = 'Moderate';
                passwordStrength.className = 'moderate';
                break;
            case 3:
                passwordStrength.textContent = 'Strong';
                passwordStrength.className = 'strong';
                break;
            case 4:
                passwordStrength.textContent = 'Very Strong';
                passwordStrength.className = 'very-strong';
                break;
            default:
                passwordStrength.textContent = '';
                passwordStrength.className = '';
                break;
        }
    });

    confirmNewPassword.addEventListener('input', function () {
        if (newPassword.value === confirmNewPassword.value) {
            passwordMatch.textContent = 'Passwords match';
            passwordMatch.classList.remove('invalid');
            passwordMatch.classList.add('valid');
        } else {
            passwordMatch.textContent = 'Passwords do not match';
            passwordMatch.classList.remove('valid');
            passwordMatch.classList.add('invalid');
        }
    });
});
