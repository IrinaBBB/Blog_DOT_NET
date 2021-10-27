$(document).ready(function () {
    $("#createPostButton").click(function (e) {
        e.preventDefault();
        let tagIdsString = "";
        const tagIdInputs = document.querySelectorAll('[data-attribute="TAG_ID"]');
        tagIdInputs.forEach(tagIdInput => {
            if (tagIdInput.checked) {
                tagIdsString += `${tagIdInput.value}/`;
            }
        });
        $("#TagIds").val(tagIdsString.slice(0, -1));
        $("#createPostForm").submit();
    });
});