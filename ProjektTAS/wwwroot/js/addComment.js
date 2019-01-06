function addComment() {
  var xhr = new XMLHttpRequest();
  var url = "rest/v1/review/create";
  xhr.open("POST", url, true);
  xhr.setRequestHeader("Content-Type", "application/json");
  xhr.onreadystatechange = function() {
    if (xhr.readyState === xhr.DONE && xhr.status === 200) {
      var json = JSON.parse(xhr.responseText);
      console.log(xhr.responseText);
      window.location =
        "http://localhost:48013/Index?token=" + xhr.responseText;
    }
  };
  var login = document.getElementById("login").value;
  var password = document.getElementById("password").value;
  var data = JSON.stringify({ Login: login, Password: password });
  xhr.send(data);
}
