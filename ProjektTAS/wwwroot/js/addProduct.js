function addProduct(token) {
  var xhr = new XMLHttpRequest();
  var url = "http://localhost:48013/rest/v1/product/create";
  xhr.open("POST", url, true);
  xhr.setRequestHeader("Content-Type", "application/json");
  xhr.setRequestHeader("Authorization", "Bearer " + token.replace('"', ""));
  xhr.onreadystatechange = function() {
    if (xhr.readyState === xhr.DONE && xhr.status === 200) {
      console.log(xhr.responseText);
      window.location.reload(true);
    }
  };
  // var categoryId = ;
  // var UserId = ;
  var nazwa = document.getElementById("nazwa").value;
  var cena = document.getElementById("cena").value;
  var data = JSON.stringify({
    CategoryId: id,
    UserId: comment,
    Name: nazwa,
    Price: cena
  });
  xhr.send(data);
}
