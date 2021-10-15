$(document).ready(function () {
    $("#loginButton").on("click",
        function () {
            const email = $("#Input_Email").val();
            const password = $("#Input_Password").val();
            getToken(email, password);
        });
});


function getToken(userName, userPassword) {
    $.ajax({
            url: "/api/account/login",
            type: "POST",
            data: JSON.stringify({
                username: userName,
                password: userPassword
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        })
        .done(function(result) {
            localStorage.setItem("token", result.token);
            localStorage.setItem("username", result.username);
            $('#account').submit();
        })
        .fail(function() {
            console.log("Login error");
        });
}