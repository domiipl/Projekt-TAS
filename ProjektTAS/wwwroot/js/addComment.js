function addComment(token) {
    var xhr = new XMLHttpRequest();
    var url = "rest/v1/review/create";
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + token.replace("\"",""));
    xhr.onreadystatechange = function () {
        if (xhr.readyState === xhr.DONE && xhr.status === 200) {
            console.log(xhr.responseText);
            window.location.reload(true);
        }
    };
    var id = document.getElementById("id").value;
    var comment = document.getElementById("comment").value;
    var rating = document.getElementById("rating").value;
    var data = JSON.stringify({ productid: id, review: comment, rating: rating });
    xhr.send(data);
}
