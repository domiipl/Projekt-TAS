$(".rating").rating({
  maxRating: 5,
  onRate: function rating(starSelected) {
    console.log(starSelected);
    return starSelected;
  }
});
