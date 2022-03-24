//Share Job Post
function ShareJobPost() {
    ComingSoon();
}

//Start informal voting process, only job doer is authorized for this action
function StartInformalVoting(JobId) {
    $.confirm({
        title: 'Confirmation',
        content: '<b>Are you confirming that you submitted a valid evidence for job completion and start informal voting process ?</b>' +           
            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm1"><label class="form-check-label text-justify" for="flexCheckDefault">I hereby declare that all results, work product, etc. associated with my bid and associated work product will be made available under an open-source license. I acknowledge that I am legally responsible to ensure that all parts of this project are open-source. </label></div>' +
            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm2"><label class="form-check-label text-justify" for="flexCheckDefault">     I hereby declare that my bid and associated work product will benefit decentralization and open-source projects generally, pursuant to the mission statement of OSSA, which is to support open source and transparent scientific research of emerging technologies for community building by way of submitting grants to developers and scientists in Switzerland and abroad. </label></div>' +
            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm3"><label class="form-check-label text-justify" for="flexCheckDefault">I hereby declare that my bid and associated work product is in line with international transparency standards; will be published on Github under the CRDAO repo, and my team and I have sufficient qualifications, experience and capacity to actually finish my bid and associated work product. </label></div>' +
            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm4"><label class="form-check-label text-justify" for="flexCheckDefault">I hereby declare that I have not built tools and do not intend to build tools to attack the CRDAO and OSSA. </label></div>' +
            '<div class="form-check m-2"><input class="form-check-input" type="checkbox" value="" id="checkConfirm5"><label class="form-check-label text-justify" for="flexCheckDefault">I hereby declare that I have not previously failed to fulfill my contractual obligations under an earlier bid and associated work product between myself and the CRDAO and OSSA.</label></div>' +
            '<div class="row mt-4" style="width:100%"><div class="col-md-6"><label>Pull Request Link:</label><input type="email" class="form-control" id="prLink" placeholder="Paste your reviews GitHub pull request link here."></div>' +
            '<div class="form-group col-md-6"><label>Review Result:</label><select class="form-control" id="revResult"><option>PASS</option><option>PASS with Notes</option><option>FAIL</option></select></div></div>',

        columnClass: 'col-md-8 col-md-offset-2',
        buttons: {
            cancel: {
                text: 'Cancel'
            },
            confirm: {
                text: 'Continue',
                btnClass: 'btn btn-primary',
                action: function() {

                    var confirmationControl = true;

                    for (var i = 1; i < 6; i++) {
                        var checked = $("#checkConfirm" + i).is(':checked');

                        if (checked == false) {
                            confirmationControl = false;
                        }
                    }

                    if (confirmationControl == false) {
                        toastr.warning("You must confirm agreements before starting informal voting process.");

                        return false;
                    }

                    var token = $('input[name="__RequestVerificationToken"]', token).val();

                    /*  var comment = CKEDITOR.instances["textarea-evidence"].getData();*/

                    if ($('#prLink').val() == "") {
                        toastr.warning("Pull request link cannot be empty.");
                        return;
                    }

                    var comment = '<div><b>Recommendation:' + $('#revResult option:selected').text() + '</b><p>Pull Request Link: <a href="' + $('#prLink').val() + '">' + $('#prLink').val() + '</a></p></div>';
  
                    $.ajax({
                        type: "POST",
                        url: "../Home/AddNewComment",
                        data: { "JobId": JobId, "CommentId": 0, "Comment": comment, "__RequestVerificationToken": token },
                        success: function (result) {                   
                        },
                        failure: function (response) {
                        },
                        error: function (response) {
                        }
                    });

                    $.ajax({
                        url: "../StartInformalVoting/" + JobId,
                        type: "GET",
                        dataType: 'json',
                        success: function(result) {
                            if (result.success) {
                                window.location.reload();
                            } else {
                                toastr.warning(result.message);
                            }
                        },
                        failure: function(response) {
                            toastr.warning("Connection error");
                        },
                        error: function(response) {
                            toastr.error("Unexpected error");
                        }
                    });

                }

            }
        }
    });
}

selectedJobId = 0;

function PayDosFeeModal(JobId) {
    $("#DosFeeModal").modal("toggle");
    selectedJobId = JobId;
}

function PayDosFee() {
    $.ajax({
        url: "../PayDosFee/" + selectedJobId,
        type: "GET",
        dataType: 'json',
        success: function(result) {
            if (result.success) {
                toastr.success(result.message);
                $("#DosFeeModal").modal("toggle");
                setTimeout(function() { window.location.reload() }, 5000)
            } else {
                toastr.warning(result.message);
            }
        },
        failure: function(response) {
            toastr.warning("Connection error");
        },
        error: function(response) {
            toastr.error("Unexpected error");
        }
    });
}

function GoToJobDetail(jobid) {
    window.location.href = '../Job-Detail/' + jobid;
}

//Features which are not available at the moment should call this methods
function ComingSoon() {
    toastr.warning("This feature will be available in the next version.");
}

function OpenFlagModal(jobid) {

    $.confirm({
        title: '<i class="fas fa-flag me-2"></i>Flag Confirmation',
        content: '' +
            '<div class="form-group">' +
            '<textarea type="text" placeholder="Please explain why are you flagging this job ?" rows=5 class="flagreason form-control"></textarea>' +
            '</div>',
        buttons: {
            formSubmit: {
                text: 'Submit',
                btnClass: 'btn-primary btn-submit',
                action: function() {
                    var flagreason = this.$content.find('.flagreason').val();
                    if (!flagreason) {
                        toastr.warning("Please provide a flag reason");
                        return false;
                    }

                    $(".btn-submit").addClass("d-none");

                    $.ajax({
                        url: "../Home/FlagJob?jobid=" + jobid + "&flagreason=" + flagreason,
                        type: "GET",
                        dataType: 'json',
                        success: function(result) {
                            if (result.success) {
                                window.location.reload();
                            } else {
                                toastr.warning(result.message);

                                $(".btn-submit").removeClass("d-none");
                            }
                        },
                        failure: function(response) {
                            toastr.warning("Connection error");
                            $(".btn-submit").removeClass("d-none");
                        },
                        error: function(response) {
                            toastr.error("Unexpected error");
                            $(".btn-submit").removeClass("d-none");
                        }
                    });

                    return false;
                }
            },
            cancel: function() {
                //close
            },
        }
    });
}

function RemoveFlag(jobid) {
    $.confirm({
        title: 'Confirmation',
        content: 'Are you sure you want to remove flag from this job ?',
        buttons: {
            cancel: {
                text: 'Cancel'
            },
            confirm: {
                text: 'Continue',
                btnClass: 'btn btn-primary',
                action: function() {

                    $.ajax({
                        type: "GET",
                        url: "../Home/RemoveFlag",
                        data: {
                            "jobid": jobid,
                        },
                        success: function(result) {
                            if (result.success) {
                                window.location.reload();
                            } else {
                                toastr.warning(result.message);
                            }
                        }
                    });

                }
            }
        }
    });
}

//Get query string parameter function
function getQueryParameter(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function RestartJob(jobid){
    $.confirm({
        title: 'Confirmation',
        content: 'A new job record will be created and job flow will be restarted.',
        buttons: {
            cancel: {
                text: 'Cancel'
            },
            confirm: {
                text: 'Continue',
                btnClass: 'btn btn-primary',
                action: function() {

                    $.ajax({
                        type: "GET",
                        url: "../Home/RestartJob",
                        data: {
                            "jobid": jobid,
                        },
                        success: function(result) {
                            if (result.success) {
                                window.location.href = "../Auctions";
                            } else {
                                toastr.warning(result.message);
                            }
                        }
                    });

                }
            }
        }
    });    
}

//Input mask for number inputs
$('input.number').on("input", function(e) {
    $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
})