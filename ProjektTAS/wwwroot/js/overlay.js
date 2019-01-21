function overlaywl() {
    var overlay = document.getElementById("overlay");
    var specialbox = document.getElementById("specialbox");

    if (overlay.style.display === "block") {
        overlay.style.display = "none";
        specialbox.style.display = "none";
    } else {
        overlay.style.display = "block";
        specialbox.style.display = "block";
    }
}