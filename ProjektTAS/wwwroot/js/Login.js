function Login() {
    var xhr = new XMLHttpRequest();
    var url = "rest/v1/user/login";
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
        if (xhr.readyState === xhr.DONE && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            document.cookie = "SessionToken=" + xhr.responseText + "; path=/";
            window.location = "http://localhost:48013/Index";
        }
    };
    var login = document.getElementById("login").value;
    var password = document.getElementById("password").value;
    var data = JSON.stringify({ "Login": login, "Password": password });
    xhr.send(data);
}