//Share Job Post
function ShareJobPost() {
    ComingSoon();
}

//Start informal voting process, only job doer is authorized for this action
function StartInformalVoting(JobId) {
    $.confirm({
        title: 'Confirmation',
        content: '<b>Are you confirming that you submitted a valid evidence for job completion and start informal voting process ?</b>' +

            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm1"><label class="form-check-label" for="flexCheckDefault">I hereby declare that all results, work product, etc. associated with these grant deliverable(s) will be made available under an open-source license. I acknowledge that I am legally responsible to ensure that all parts of this project are open-source.</label></div>' +

            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm2"><label class="form-check-label" for="flexCheckDefault">I hereby declare that this grant will benefit decentralization and open-source projects generally, pursuant to the mission statement of the ETA, which is to support open source and transparent scientific research of emerging technologies for community building by way of submitting grants to developers and scientists in Switzerland and abroad.</label></div>' +

            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm3"><label class="form-check-label" for="flexCheckDefault">I hereby declare that this proposed project increases the level of decentralization of various blockchain layer 1 platforms; will produce high-quality open source and transparent scientific research and/or developments that further the decentralization of platforms and organizations; delivers research and development initiatives that are globally applicable; and/or delivers reference implementations of the research and development.</label></div>' +

            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm4"><label class="form-check-label" for="flexCheckDefault">I hereby declare that my proposed project is in line with international transparency standards; my team has sufficient qualifications, experience and capacity to actually finish the proposed project.</label></div>' +

            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm5"><label class="form-check-label" for="flexCheckDefault">I hereby declare that I have not built tools and do not intend to build tools to attack a blockchain network.</label></div>' +

            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm6"><label class="form-check-label" for="flexCheckDefault">I hereby declare that I do not intend to use the Developer Grants for illegal market manipulation.</label></div>' +

            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm7"><label class="form-check-label" for="flexCheckDefault">I hereby declare that I have not previously failed to fulfill my contractual obligations under an earlier grant agreement between myself and the ETA and/or the DEVxDAO.</label></div>'
        ,
        columnClass: 'col-md-8 col-md-offset-2',
        buttons: {
            cancel: {
                text: 'Cancel'
            },
            confirm: {
                text: 'Continue',
                btnClass: 'btn btn-primary',
                action: function () {

                    var confirmationControl = true;

                    for (var i = 1; i < 8; i++) {
                        var checked = $("#checkConfirm" + i).is(':checked');

                        if (checked == false) {
                            confirmationControl = false;
                        }
                    }

                    if (confirmationControl == false) {
                        toastr.warning("You must confirm agreements before starting informal voting process.");

                        return false;
                    }

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