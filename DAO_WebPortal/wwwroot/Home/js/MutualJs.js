
$(".replyBtn").on("click", function () {
    $(this).parent().append("<div class='comment-body mt-2'><textarea class='form-control' id='textarea-input' name='textarea-input' rows='3' placeholder='Your comment..'></textarea><div class='d-flex justify-content-end mt-1'><button class='btn btn-sm btn-block btn-outline-light active'> Reply</button ></div></div>")
})

$(".voteDetail").click(function () {
    window.location = $(this).data("href");
});