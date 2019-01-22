function addComment(token, productid) {
  var review = document.getElementById("comment").value;
  var rating = document.getElementById("productRating").value;
  $('.ui.form')
    .form({
      on: 'blur',
      fields: {
        review: {
          identifier: 'comment',
          rules: [{
            type: 'empty',
            prompt: 'Opinia nie została dodana'
          },
          {
            type: 'minLength[6]',
            prompt: 'Opinia musi zawierać co najmniej 6 znaków'
          }]
        },
        rating: {
          identifier: 'productRating',
          rules: [{
            type: 'empty',
            prompt: 'Produkt nie został oceniony'
          }]
        }
      }
    }).api({
      url: 'http://localhost:48013/rest/v1/review/create',
      method: 'POST',
      beforeXHR: (xhr) => {
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.setRequestHeader("Authorization", "Bearer " + token.replace('"', ""));
      },
      beforeSend: function (settings) {
        settings.data = JSON.stringify({ productId: productid, review: review, rating: rating });
        return settings;
      },
      onSuccess: function (result) {
        window.location.reload(true);
      }
    });
}