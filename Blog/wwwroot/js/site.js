var connection = new signalR.HubConnectionBuilder()
    .withUrl("/commentsHub")
    .build();

connection.on("ReceiveComment", updateComment);
connection.start();


$(document).ready(function() {
    $("#commentsField").focus(function(e) {
        $("#commentsButton").removeClass("invisible");
    });
    $("#commentsCancel").click(function() {
        $("#commentsButton").addClass("invisible");
        $("#commentsField").val("");
    });
    $("#headingThree").click(function() {
        $("#comments").html("<p>Loading...</p>");
        const postId = $("#postId").val();
        getComments(postId);
    });
    $("#commentsButton").click(function (e) {
        e.preventDefault();
        const text = $("#commentsField").val();
        const postId = $("#postId").val();
        addComment(text, postId);
    });
});

function addComment(text, postId) {
    if (text && postId) {
        connection.invoke('AddComment', text, postId);
        $("#commentsField").val("");
    }
}

function updateComment(text, ownerName, created) {
    var commentString = renderComment(text, ownerName, created);
    $(commentString).prependTo($("#comments"));
}

function getComments(postId) {
    $.ajax(
        {
            type: "GET",
            url: `/api/comments/${postId}`,
            data: {},
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function(result) {
                listComments(result);
            },
            error: function(req, status, error) {
                console.log(error);
            }
        });

}

function listComments(comments) {
    var commentsString = "";
    $.each(comments,
        function (index, comment) {
            const commentString = renderComment(comment.text, comment.ownerName, comment.created);
            commentsString += commentString;
        });

    $("#comments").html(commentsString);
};

function renderComment(text, ownerName, created) {
    const dateTime = parseDate(created);
    const commentString =
        `
            <div>
                <div class="text-secondary">
                ${ownerName}
                </div>
                <div>${text}</div>
                <div class="font-italic">${dateTime}</div>
            </div>
            <div>
                <a asp-controller="Comment" asp-action="Edit" asp-route-id="@comment.Id" class="btn btn-danger"
                    style="background-color: transparent; border: none;">
                <i class="fas fa-edit"></i>
                </a>
                <a asp-controller="Comment" asp-action="Delete" asp-route-commentId="@comment.Id" asp-route-postId="@Model.Id"
                class="btn btn-danger" style="background-color: transparent; border: none;">
                    <i class="fas fa-trash-alt"></i>
                </a>
            </div>
            <hr/>
        </div>         
    `;
    return commentString;
}


function parseDate(date) {
    const day = date.split("T")[0];
    const timeFull = date.split("T")[1];
    const time = timeFull.split(".")[0];
    return `${day} ${time}`;
}