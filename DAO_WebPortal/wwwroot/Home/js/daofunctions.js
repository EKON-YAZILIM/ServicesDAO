//Share Job Post
function ShareJobPost() {
    ComingSoon();
}

//Start informal voting process, only job doer is authorized for this action
function StartInformalVoting(JobId) {
    $.confirm({
        title: 'Confirmation',
        content: 'Are you confirming that you submitted a valid evidence for job completion and start informal voting process ?',
        buttons: {
            cancel: {
                text: 'Cancel'
            },
            confirm: {
                text: 'Continue',
                btnClass: 'btn btn-primary',
                action: function () {
                    $.ajax({
                        url: "../StartInformalVoting/" + JobId,
                        type: "GET",
                        dataType: 'json',
                        success: function (result) {
                            if (result.success) {
                                toastr.success(result.message);
                            }
                            else {
                                toastr.warning(result.message);
                            }
                        },
                        failure: function (response) {
                            toastr.warning("Connection error");
                        },
                        error: function (response) {
                            toastr.error("Unexpected error");
                        }
                    });
                }
            }
        }
    });
}

function KYCModal() {
    $("#KYCModal").modal("toggle");
}

selectedJobId = 0;
function PayDosFeeModal(JobId) {
    $("#DosFeeModal").modal("toggle");
    selectedJobId = JobId;
}

function SubmitKYC() {
    $.ajax({
        url: "../SubmitKYC",
        type: "GET",
        dataType: 'json',
        success: function (result) {
            if (result.success) {
                toastr.success(result.message);
                $("#KYCModal").modal("toggle");
            }
            else {
                toastr.warning(result.message);
            }
        },
        failure: function (response) {
            toastr.warning("Connection error");
        },
        error: function (response) {
            toastr.error("Unexpected error");
        }
    });
}

function PayDosFee() {
    $.ajax({
        url: "../PayDosFee/" + selectedJobId,
        type: "GET",
        dataType: 'json',
        success: function (result) {
            if (result.success) {
                toastr.success(result.message);
                $("#DosFeeModal").modal("toggle");
            }
            else {
                toastr.warning(result.message);
            }
        },
        failure: function (response) {
            toastr.warning("Connection error");
        },
        error: function (response) {
            toastr.error("Unexpected error");
        }
    });
}

function GoToJobDetail(pageurl, jobid) {
    if (pageurl.indexOf("Job-Detail") == -1) {
        window.location.href = '../Job-Detail/' + jobid;
    }
}

//Features which are not available at the moment should call this methods
function ComingSoon() {
    toastr.warning("This feature will be available in the next version.");
}

function CheckNumbers(e) {
    $(e).val($(e).val().replace(/[^0-9.]/g, '').replace(/(\..*)\./g, '$1'));
}