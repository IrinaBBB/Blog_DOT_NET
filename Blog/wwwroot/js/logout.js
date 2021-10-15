$(document).ready(function () {
    $("#logout").on("submit",
        function () {
            logOut();
        });
});


function logOut() {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
}