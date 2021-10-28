$(document).ready(function () {
    const connectionNotification = new signalR.HubConnectionBuilder()
        .withUrl("/postNotification")
        .build();

    connectionNotification.on("displayNotification", () => {
        getNotification();
    });
    connectionNotification.start();
    $("#newPostWarning").click(function() {
        $("#newPostWarning").addClass("invisible");
    });
});

function getNotification() {
    $("#newPostWarning").removeClass("invisible");
}