function addComment(token) {
  var xhr = new XMLHttpRequest();
  var url = "http://localhost:48013/rest/v1/review/create";
  xhr.open("POST", url, true);
  xhr.setRequestHeader("Content-Type", "application/json");
  xhr.setRequestHeader("Authorization", "Bearer " + token.replace('"', ""));
  xhr.onreadystatechange = function() {
    if (xhr.readyState === xhr.DONE && xhr.status === 200) {
      console.log(xhr.responseText);
      window.location.reload(true);
    }
  };
  var productid = document.getElementById("id").innerHTML;
  var review = document.getElementById("comment").value;
  var rating = document.getElementById("productRating").value;
  var data = JSON.stringify({ productid: productid, review: review, rating: rating });
  xhr.send(data);
}
