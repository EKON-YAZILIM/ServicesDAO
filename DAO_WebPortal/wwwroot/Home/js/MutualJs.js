
$(".replyBtn").on("click", function () {
    $(this).parent().append("<div class='comment-body mt-2'><textarea class='form-control' id='textarea-input' name='textarea-input' rows='3' placeholder='Your comment..'></textarea></div>")
})