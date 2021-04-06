callback = function () {
    var ele = document.getElementsByClassName("link");
    ele[0].firstChild.remove()// = "";
};

if (document.readyState === "complete" || (document.readyState !== "loading" && !document.documentElement.doScroll)) {
    callback();
} else {
    document.addEventListener("DOMContentLoaded", callback);
}