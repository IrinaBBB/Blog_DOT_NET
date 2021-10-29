const connectionNotification = new signalR.HubConnectionBuilder()
    .withUrl("/postNotification")
    .build();

connectionNotification.on("displayNotification", () => {
    getNotification();
});
connectionNotification.start();


$(document).ready(function () {
    $("#newPostWarning").click(function() {
        $("#newPostWarning").addClass("invisible");
    });
});

function getNotification() {
    $("#newPostWarning").removeClass("invisible");
}

function subscribeToBlog(blogId) {
    $.ajax(
        {
            type: "POST",
            url: `/api/comments/subscribeToBlog/${blogId}`,
            data: {},
            contentType: "application/json;charset=utf-8",
            headers: {
                Authorization: `Bearer ${localStorage.token}`
            },
            success: function (result) {
                
            },
            error: function (req, status, error) {
                console.log(error);
            }
        });
}

function unsubscribeToBlog(blogId) {
    $.ajax(
        {
            type: "POST",
          url: `/api/comments/unsubscribeToBlog/${blogId}`,
            data: {},
            contentType: "application/json;charset=utf-8",
            headers: {
                Authorization: `Bearer ${localStorage.token}`
            },
            success: function (result) {

            },
            error: function (req, status, error) {
                console.log(error);
            }
        });
}


