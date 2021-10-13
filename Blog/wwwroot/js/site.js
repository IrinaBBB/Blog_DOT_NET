var connection = new signalR.HubConnectionBuilder()
    .withUrl("/commentsHub")
    .build();

connection.on("ReceiveComment", renderCommentHub);
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
        const ownerId = "daf7cd13-3106-4af2-a13f-64b98a59a49e";
        addComment(text);
        createComment(ownerId, text, postId);
    });
});

function addComment(text) {
    console.log('addComment invoked');
    connection.invoke('AddComment', text);
}


function createComment(ownerId, text, postId) {

    $.ajax({
        url: "/api/comments",
        type: "POST",
        data: JSON.stringify({
            ownerId: ownerId,
            text: text,
            postId: postId
        }),
        contentType: "application/json; charset=utf-8",
        success: function(result) {
            //Swal.fire(
            //    "Success!",
            //    "Your comment has been created!",
            //    "success"
            //);
            $("#commentsField").val("");
        },
        error: function(req, status, error) {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Something went wrong!, We could not create your comment :("
            });
        }
    });
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
                console.log(result);
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
            const commentString = renderComment(comment);
            commentsString += commentString;
        });

    $("#comments").html(commentsString);
};

function renderComment(comment) {
    const dateTime = parseDate(comment.created);
    const commentString =
        `
            <div>
                <div class="text-secondary">
                ${comment.ownerName}
                </div>
                <div>${comment.text}</div>
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

function renderCommentHub(text, ownerId, created) {
    const dateTime = parseDate(created);
    const commentString =
        `
            <div>
                <div class="text-secondary">
                ${ownerId}}
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

    $(commentString).prependTo($("#comments"));
}


function parseDate(date) {
    const day = date.split("T")[0];
    const timeFull = date.split("T")[1];
    const time = timeFull.split(".")[0];
    return `${day} ${time}`;
}