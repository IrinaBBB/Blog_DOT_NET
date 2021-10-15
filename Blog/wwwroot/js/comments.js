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
    $("#commentsButton").click(function(e) {
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

function updateComment(id, text, ownerName, created) {
    const commentString = renderComment(id, text, ownerName, created);
    $(commentString).prependTo($("#comments"));
    setButtonsOnClickListener();
}

function getComments(postId) {
    $.ajax(
        {
            type: "GET",
            url: `/api/comments/${postId}`,
            data: {},
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            headers: {
                Authorization: `Bearer ${localStorage.token}`
            },
            success: function(result) {
                listComments(result);
                setButtonsOnClickListener();
            },
            error: function(req, status, error) {
                console.log(error);
            }
        });
}

function setButtonsOnClickListener() {
    const editButtons = document.querySelectorAll('[data-action="EDIT_COMMENT"]');
    editButtons.forEach(button => {
        button.addEventListener("click",
            () => {
                const commentDom = button.parentNode.parentNode;
                const commentId = commentDom.querySelector('[data-attribute="COMMENT_ID"]').value;
                console.log(commentId);
            });
    });
    const deleteButtons = document.querySelectorAll('[data-action="DELETE_COMMENT"]');
    deleteButtons.forEach(button => {
        button.addEventListener("click",
            () => {
                const commentDom = button.parentNode.parentNode;
                const commentId = commentDom.querySelector('[data-attribute="COMMENT_ID"]').value;
                deleteComment(commentId);
            });
    });

}

function deleteComment(commentId) {
    Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, delete it!"
        })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                        url: `/api/comments/${commentId}`,
                        type: "DELETE",
                        headers: {
                            Authorization: `Bearer ${localStorage.token}`
                        }
                    })
                    .then(function() {
                    })
                    .done(function() {
                        getComments($("#postId").val());
                        Swal.fire(
                            "Poof!",
                            "Your comment has been deleted!",
                            "success");
                    })
                    .fail(function() {
                        Swal.fire("Something went wrong",
                            "We could not delete your post",
                            {
                                button: "Ok"
                            });
                        console.log("Something went wrong =(");
                    });
            } else {
                Swal.fire("Your post is safe!");
            }
        });
}

function listComments(comments) {
    var commentsString = "";
    $.each(comments,
        function(index, comment) {
            const commentString = renderComment(comment.id, comment.text, comment.ownerName, comment.created);
            commentsString += commentString;
        });

    $("#comments").html(commentsString);
};

function renderComment(id, text, ownerName, created) {
    const dateTime = parseDate(created);
    let commentString =
        `
            <div>
                <input data-attribute="COMMENT_ID" type="hidden" value=${id} />
                <div data-attribute="COMMENT_OWNER" class="text-secondary">
                    ${ownerName}
                </div>
                <div data-attribute="COMMENT_TEXT">${text}</div>
                <div class="font-italic">${dateTime}</div>
            `;
    if (ownerName === localStorage.getItem("username")) {
        commentString += `
            <div>
                <button data-action="EDIT_COMMENT" class="btn btn-info">
                    <i class="fas fa-edit"></i>
                </button>
                <button data-action="DELETE_COMMENT"
                class="btn btn-danger">
                    <i class="fas fa-trash-alt"></i>
                </button>
            </div>`;
    }
    commentString += `<hr/></div >`;
    return commentString;
}


function parseDate(date) {
    const day = date.split("T")[0];
    const timeFull = date.split("T")[1];
    const time = timeFull.split(".")[0];
    return `${day} ${time} `;
}