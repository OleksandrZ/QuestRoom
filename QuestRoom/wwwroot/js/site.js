// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var loadFile = function (event) {
    document.getElementById("images").innerHTML = "";
    for (var i = 0; i < event.target.files.length; i++) {
        if (event.target.files[i].type === "image/jpeg") {
            var image = document.createElement("img");
            image.classList.add("img");
            image.src = URL.createObjectURL(event.target.files[i]);
            document.getElementById("images").appendChild(image);
        }
    }
}