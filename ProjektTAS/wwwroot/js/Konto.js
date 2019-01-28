function ShowPassword() {
    var passwordForm = document.getElementById('temporaryPasswordBox');
    if (passwordForm.style.display === 'block') passwordForm.style.display = 'none';
    else passwordForm.style.display = 'block';
}

function ShowEmail() {
    var emailForm = document.getElementById('temporaryEmailBox');
    if (emailForm.style.display === 'block') emailForm.style.display = 'none';
    else emailForm.style.display = 'block';
}

function ChangePassword(token) {
    var xhr = new XMLHttpRequest();
    var url = "rest/v1/user/changepassword";
    token = token.replace("\"", "").replace("\"", "");
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + token);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === xhr.DONE && xhr.status === 200) {
            window.location = "http://localhost:48013/Account";
        }
    };
    var currentPassword = document.getElementById("currentPassword");
    var newPassword = document.getElementById("newPassword");
    var repeatedNewPassword = document.getElementById("repeatedPassword");
    var data = JSON.stringify({ "CurrentPassword": currentPassword.value, "NewPassword": newPassword.value, "RepeatedNewPassword": repeatedNewPassword.value });
    if (newPassword.value === repeatedNewPassword.value) xhr.send(data);
    else {
        newPassword.style.borderColor = '#ff0000';
        repeatedNewPassword.style.borderColor = '#ff0000';
    }
}

function ChangeEmail(token) {
    var xhr = new XMLHttpRequest();
    var url = "rest/v1/user/changeemail";
    token = token.replace("\"", "").replace("\"", "");
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + token);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === xhr.DONE && xhr.status === 200) {
            window.location = "http://localhost:48013/Account";
        }
    };
    var currentPassword = document.getElementById("currentPasswordAtEmail");
    var newEmail = document.getElementById("newEmail");
    var repeatedNewEmail = document.getElementById("repeatedEmail");
    var data = JSON.stringify({ "CurrentPassword": currentPassword.value, "NewEmail": newEmail.value, "RepeatedNewEmail": repeatedNewEmail.value });
    if (newEmail.value === repeatedNewEmail.value) xhr.send(data);
    else {
        newEmail.style.borderColor = '#ff0000';
        repeatedNewEmail.style.borderColor = '#ff0000';
    }
}