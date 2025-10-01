var TimeStampForcheckdate = new Date();
$(document).ready(function () {

    $("#searchBox").select2({
        dropdownParent: $('#ProjFwd') // or the container div of your popup
    });
    $("#ddlfwdCCTo").select2({
        maximumSelectionLength: 6,
        placeholder: "CC cannot have more than 6 stakeholders!",
        dropdownParent: $('#ProjFwd'), // or the container div of your popup
        language: {
            maximumSelected: function (args) {
                return "You can only select a maximum of 6 stakeholders.";
            }
        }
    });

    mMsaterfwdStage(0, "ddlfwdStage", 5, 0)

    $("#ddlfwdStage").change(function () {

        mMsaterStage(0, "ddlfwdSubStage", 6, $("#ddlfwdStage").val(), $("#SpnStakeHolderId").html())
    });


    $("#ddlfwdSubStage").change(function () {

        mMsater(0, "ddlfwdAction", 7, $("#ddlfwdSubStage").val())
    });
    mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html(), 0, "");
    $("#ddlfwdTo").change(function () {
        $("#ddlfwdCCTo").val("");
        mMsaterFwdTo(0, "ddlfwdCCTo", 8, 0, $("#SpnFwdStakeHolderId").html(), 0, "");
    });
    $("#toggleCcBtn").click(function () {

        $(".ProjectsCCFwd").removeClass("d-none")

    });

    $("select[name='fwdoffrs']").change(function () {

        var selectedText = $(this).find("option:selected").text().trim();

        if ($("select[name='fwdoffrs']").val() === "More") {
            $('.FwdDropdown').removeClass('col-md-6');
            $('.FwdDropdown').addClass('col-md-3');

            $('.ProjectsFwdUnit').removeClass('d-none');

            $("#searchBox").show().focus();

            if (selectedText === "More") {
                mMsater(0, "searchBox", 12, 0)
            }

        } else {
            $('.FwdDropdown').addClass('col-md-3');
            $('.FwdDropdown').addClass('col-md-6');
            $('.ProjectsFwdUnit').addClass('d-none');


            $("#searchBox").hide();

        }
    });

   
    $(".btn-Undo").click(function () {
        debugger;
        var projectName = $(this).closest("tr").find("a").data("proj-name");
        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());

        var words = projectName.split(" ");
        var shortProjName = words.length > 6 ? words.slice(0, 4).join(" ") + "..." : projectName;

       

        Swal.fire({
            title: `Enter Pull Back Remarks: ${shortProjName}`, // your dynamic title
            input: 'text',
            showCancelButton: true,
            confirmButtonText: 'OK',
            cancelButtonText: 'Cancel',

            /* Assign custom classes for just this SweetAlert instance */
            customClass: {
                popup: 'custom-swal-popup',
                confirmButton: 'custom-confirm-button',
                cancelButton: 'custom-cancel-button',
                input: 'custom-input-field',

                // Here is the important part:
                title: 'pullback-title'
            },

            preConfirm: (login) => {
                if (!login) {
                    Swal.showValidationMessage(`Please enter remarks for project: ${shortProjName}`);
                }
            },

            allowOutsideClick: () => !Swal.isLoading()
        }).then((result) => {
            if (result.isConfirmed) {

                PullBAckProject($(this).closest("tr").find("#SpnCurrentProjId").html(), $(this).closest("tr").find("#SpnCurrentpsmId").html(), result.value, $(this).closest("tr").find("#SpnprojectStageId").html());
                UndoNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2, $(this).closest("tr").find("#SpnprojectToUnitId").html());

            }
        });
    });
    $(".btn-FwdHistorySent").click(function () {

        var projName = $(this).data('proj-name');
        var words = projName.split(" ");
        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
        //var finalTitle = "Mov History: " + shortProjName;
        var finalTitle = "Mov History: " + projName;
        $('#lblHistory').text(finalTitle);
        $('#ProjFwdHistory').modal('show');

        GetProjectMovHistory($(this).closest("tr").find("#SpnCurrentProjId").html());

    });
    $(".btn-FwdHistory").click(function () {
        debugger;
        var projName = $(this).data('proj-name');
        var words = projName.split(" ");
        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
        //var finalTitle = "Mov History: " + shortProjName;
        var finalTitle = "Mov History: " + projName;
        $('#lblHistory').text(finalTitle);
        $('#ProjFwdHistory').modal('show');

        GetProjectMovHistory($(this).closest("tr").find("#SpnCurrentProjId").html());
        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
     // IsReadNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2);
        if ($(this).closest("tr").find("#IsccForRemoveRow").html() == "Cc") {
            $(this).closest('tr').remove();
        }
        $(this).closest("tr").removeClass("bold-text")
        setTimeout(function () {

            InboxNotificationCount();
        }, 500)

    });
    $(".btn-Fwd").click(function () {
        debugger;
        var Isprocess = $(this).closest("tr").find("#SpnprojectIsProcess").html() === "False" ? false : true;


        $('.FwdDropdown').addClass('col-md-6');
        $('.ProjectsFwdUnit').addClass('d-none');
        $("#searchBox").val('');
        $("#searchBox").hide();


        TimeStampForcheckdate = new Date($(".TimeStampForcheckdate").html());

        var projName = $(this).data('proj-name');
        var words = projName.split(" ");
        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
        //var finalTitle = "Mov History: " + shortProjName;
        var finalTitle = "Mov History: " + projName;
        $('#fwdModal').text(finalTitle);

        var projNameDetail = $(this).data('proj-name') + " " + "Move Details";
        $('.fwdtitle').text(projNameDetail);


        if (Isprocess == false)
        {
            mMsaterfwdStage($(this).closest("tr").find("#SpnStageId").html(), "ddlfwdStage", 5, 0, 1, 1)
        }
      else if (words.includes("Re-Vetted")) {

            mMsaterfwdStage($(this).closest("tr").find("#SpnStageId").html(), "ddlfwdStage", 5, 0, 1, "Re-Vetted")
        }
        else {
            mMsaterfwdStage($(this).closest("tr").find("#SpnStageId").html(), "ddlfwdStage", 5, 0, 1)
        }


        mMsaterStage($(this).closest("tr").find("#SpnTimeStatusId").html(), "ddlfwdSubStage", 6, $(this).closest("tr").find("#SpnStageId").html(), $("#SpnStakeHolderId").html())
        /* mMsater(0, "ddlfwdAction", 9, $("#ddlfwdSubStage").val())*/

        mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 7, $(this).closest("tr").find("#SpnTimeStatusId").html())
        /*mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 9, $(this).closest("tr").find("#SpnTimeStatusId").html())*/
        mMsaterFwdTo($(this).closest("tr").find("#SpnTimeToUnitId").html(), "ddlfwdFwdTo", 8, 0, $(this).closest("tr").find("#SpnStakeHolderId").html(), 0, "");
        mMsaterFwdTo(0, "ddlfwdCCTo", 8, 0, $(this).closest("tr").find("#SpnStakeHolderId").html(), 0, "");


        $("#spanFwdCurrentPslmId").html($(this).closest("tr").find("#SpnCurrentpsmId").html())
        $("#spanFwdProjectId").html($(this).closest("tr").find("#SpnCurrentProjId").html())
        $("#SpnFwdStakeHolderId").html($(this).closest("tr").find("#SpnStakeHolderId").html())


        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
        // IsReadNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2);
        setTimeout(function () {

            InboxNotificationCount();
        }, 200)

        $(this).closest("tr").removeClass("bold-text")

        $('#ProjFwd').modal('show');

       

        $(".Fwdtitle").html("Projects Move Details");
        $(".ProjectsFwd").removeClass("d-none");
        $(".Attmenthistory").addClass("d-none");



       
    });

   

    $(".btn-Obsn").click(function () {

        var projNameDetail = $(this).data('proj-name') + " " + "Move Details"
        var projName = $(this).data('proj-name');
        var words = projName.split(" ");
        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
        //var finalTitle = "Mov History: " + shortProjName;
        var finalTitle = "Mov History: " + projNameDetail;
        $('#fwdModal').text(finalTitle);
        $('.fwdtitle').text(projNameDetail);

        


        mMsaterfwdStage($(this).closest("tr").find("#SpnStageId").html(), "ddlfwdStage", 5, 0, 2,1)
        mMsaterStage($(this).closest("tr").find("#SpnTimeStatusId").html(), "ddlfwdSubStage", 6, $(this).closest("tr").find("#SpnStageId").html(), $("#SpnStakeHolderId").html())

        /*mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 9, $(this).closest("tr").find("#SpnTimeStatusId").html())*/
        mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 7, $(this).closest("tr").find("#SpnTimeStatusId").html())
        mMsaterFwdTo($(this).closest("tr").find("#SpnTimeToUnitId").html(), "ddlfwdFwdTo", 8, 0, $(this).closest("tr").find("#SpnStakeHolderId").html(), 0, "");
        mMsaterFwdTo(0, "ddlfwdCCTo", 8, 0, $(this).closest("tr").find("#SpnStakeHolderId").html(), 0, "");
        $("#spanFwdCurrentPslmId").html($(this).closest("tr").find("#SpnCurrentpsmId").html())
        $("#spanFwdProjectId").html($(this).closest("tr").find("#SpnCurrentProjId").html())
        $("#SpnFwdStakeHolderId").html($(this).closest("tr").find("#SpnStakeHolderId").html())


        $('#ProjFwd').modal('show');

        $(".Fwdtitle").html("Projects Obsn To Unit");

        $(".ProjectsFwd").removeClass("d-none");
        $(".Attmenthistory").addClass("d-none");


        var pad = "00";
        var datef2 = new Date();

        var months = "" + (datef2.getMonth() + 1);
        var days = "" + datef2.getDate();
        var monthsans = pad.substring(0, pad.length - months.length) + months;
        var dayans = pad.substring(0, pad.length - days.length) + days;
        var year = datef2.getFullYear();
        var hh = pad.substring(0, pad.length - `${datef2.getHours()}`.length) + `${datef2.getHours()}`;
        var mm = pad.substring(0, pad.length - `${datef2.getMinutes()}`.length) + `${datef2.getMinutes()}`;
        var todayDate = `${year}-${monthsans}-${dayans}`;



        $('#TimeStampToProjfwd').attr('type', 'date');
        $('#TimeStampToProjfwd').val(todayDate); // Set today's date
        $('#TimeStampToProjfwd').prop('disabled', true); // Freeze input



        //GetAllComments($(this).closest("tr").find("#SpnCurrentProjId").html());
    });

    $(".ProjName").click(function () {
        //var row = $(this).closest('tr');
        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
        // IsReadNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2);
        InboxNotificationCount()
        $(this).closest("tr").removeClass("bold-text")
        location.reload();

    });
    $("#btn-ibutton").click(function () {
        $('#Projibutton').modal('show');
    });
    $("#btnFwdNext").click(function () {

        if ($('#ddlfwdFwdTo').val() === 'More' && !$('#searchBox').val()) {
            alert("Please Select Send Unit To");
            return false;
        }

        requiredFields = $('#ProjFwd').find('.requiredField');
        var allFieldsComplete = true;
        requiredFields.each(function (index) {
            if (this.value.length == 0) {
                $(this).addClass('is-invalid');
                allFieldsComplete = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });


        if (allFieldsComplete) {

            CheckFwdCondition();

        }
        $('.FwdDropdown').addClass('col-md-6');
        $('.ProjectsFwdUnit').addClass('d-none');


        $("#searchBox").hide()
    });
    $(document).on('click', '#btnAttchMultiforpsmid', function () {
    
     

        requiredFields = $('#ProjFwd').find('.requiredFieldAttch');
       

        var allFieldsComplete = true;
        requiredFields.each(function (index) {
            if (this.value.length == 0) {
                $(this).addClass('is-invalid');
                allFieldsComplete = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        if (allFieldsComplete) {
            Swal.fire({
                title: "Are you sure?",
                text: "Do you Want Upload Pdf File",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, Upload it!"
            }).then((result) => {
                if (result.isConfirmed) {

                    $('.uploadLoader').removeClass('d-none')
                    setTimeout(function () {

                    AttOnFWD()
                    },1000)
                    //setTimeout(function () {
                       
                    //    UploadFiles();
                    //}, 1000)

                }
            });
        }
    });

    //$("#btnFwdConfirm").click(function () {
    //    SaveFwdTo($("#spanFwdCurrentPslmId").html());
    //    $('#ProjFwd').modal('hide');
    //});
});
function CheckFwdCondition() {

    var userdata =
    {
        "ProjId": $("#spanFwdProjectId").html(),
        "StatusId": $("#ddlfwdSubStage").val(),
    };

    $.ajax({
        url: '/Projects/CheckFwdCondition',
        type: 'POST',
        data: userdata,
        success: function (response) {
            //console.log(response);
            if (response != null) {

                if (response == true) {
                    if ($("#ddlfwdSubStage").val() != 1) {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "Sub Stage Already Approved / Completed!",

                        });
                    }
                    else {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "Project Already Sent For Comments!",

                        });
                    }
                }
                else if (response == false) {
                    $(".Fwdtitle").html("Projects Attch Details");
                    $(".ProjectsFwd").addClass("d-none");
                    $(".Attmenthistory").removeClass("d-none");
                 
                    var  listItem = "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";



                    $("#DetailBody").html(listItem);

                    /*AttOnFWD();*/
                    
                }
            }

        }
    });


}

$("#btnFwdConfirm").on("click", function () {
    // Send the allAttachments array along with form data
    SaveFwdTo($("#spanFwdCurrentPslmId").html(), "", "");

});

function SaveFwdTo(CurrentPslmId, fd, allAttachments) {
    debugger;


    var dateValue = $("#TimeStampToProjfwd").val();
    var currentDate = new Date();

    // Add server's current time if only a date is selected
    var TimeStamps = '';
    if ($('#TimeStampToProjfwd').attr('type') === 'date') {
        if (!dateValue) {
            alert('Please select a date .');
            return;
        }
        var currentTime = currentDate.toTimeString().split(' ')[0]; // Get current time in HH:mm:ss
        TimeStamps = dateValue + ' ' + currentTime;
    } else if ($('#TimeStampToProjfwd').attr('type') === 'datetime-local') {
        if (!dateValue) {
            alert('Please select date and time.');
            return;
        }
        TimeStamps = dateValue.replace('T', ' '); // Format datetime-local to space-separated
    }

    var fromDate = new Date(TimeStampForcheckdate);
    var toDate = new Date(TimeStamps);
    console.log("FromDate: ", fromDate, "Todate: ", toDate);
    // Compare the dates
    //if (fromDate > toDate) {
    //    // If "from" date is greater than "to" date
    //    Swal.fire({
    //        icon: "error",
    //        title: "Oops...",
    //        text: "Date of forwarding should be greater than the received date!",

    //    });
    //    e.preventDefault(); // Prevent form submission
    //}



    let fwdunitid = 0;
    if ($("#ddlfwdFwdTo").val() != 'More') {
        fwdunitid = $("#ddlfwdFwdTo").val()
    }
    else if ($("#ddlfwdFwdTo").val() == 'More') {

        fwdunitid = $("#searchBox").val()

    }
    //if (fwdunitid == null) {
    //    alert("Please Select Send To Unit")
    //    return false;
    //};


    //var currentDate = new Date();
    //var currentTime = currentDate.toLocaleTimeString('en-US', { hour12: false });
    //var time = $("#TimeStampToProjfwd").val();
    //var timeData = time + ' ' + currentTime;
    var PsmId = $("#spanFwdCurrentPslmId").html()
    var CcIdValues = $("#ddlfwdCCTo").val(); // Get all selected values

    // Initialize formData
    var formData = new FormData();

    // Append userdata fields to FormData
    var userdata = {
        "ProjId": $("#spanFwdProjectId").html(),
        "StatusActionsMappingId": $("#ddlfwdAction").val(),
        "Remarks": $("#txtRemarksfwd").val(),
        "ToUnitId": fwdunitid,
        "TimeStamp": TimeStamps,
    };

    // Append userdata fields to FormData
    for (var key in userdata) {
        formData.append(key, userdata[key]);
    }

    // Append currentpsmid to formData
    var currentpsmid = $("#spanFwdCurrentPslmId").html();
    formData.append("currentpsmid", currentpsmid);

    // Handle CcId if there are multiple selected values
    if (Array.isArray(CcIdValues)) {
        CcIdValues.forEach((value, index) => {
            formData.append(`CcId[${index}]`, value);  // Append each CcId separately
        });
    } else {
        formData.append("CcId", CcIdValues);  // For single value scenario
    }

    // Attach remarks and files (multiple attachments) directly to the form data
    if (allAttachments != "") {
        allAttachments.forEach((attachment, index) => {
            formData.append(`attachments[${index}].file`, attachment.file);  // Append file
            formData.append(`attachments[${index}].remarks`, attachment.remarks);  // Append remarks
        });
    }

  
    $.ajax({
        url: '/Projects/FwdToProject',
        type: 'POST',
        data: formData, // FormData is sent directly as the body of the request
        processData: false, // Don't process data as a query string
        contentType: false,
        success: function (response) {
            debugger;
            //console.log(response);
            if (response != null) {

                if (response == 9) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "The 'To' Unit and 'CC' Unit must not be the same!",

                    });
                } else if (response == 6) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "Record Not Save!",

                    });
                }

                if (response == -4) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "Another user from this unit has forwarded the project.",
                        confirmButtonText: "OK"
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.reload();
                        }
                    });
                }

                if (response == -5) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "You cannot WL Project Before (ACG)Remote Test",

                    });

                } if (response == -7) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "Please Select All Feilds",

                    });

                }
                else {
                    $("#spanCurrentPslmId").html(response.psmId);
                    FwdProjConfirm(CurrentPslmId);
                   
                    $('#ProjFwd').modal('hide');
                    window.location.reload();

                    // AddNotification($("#spanFwdProjectId").html(), 2,fwdunitid);
                    //IsReadNotification($("#spanFwdProjectId").html(), 2);
                }
            }

        }
    });
}

function PullBAckProject(ProjId, PslmId, UndoRemarks, StageId) {
    var userdata =
    {
        "ProjectId": ProjId,
        "PsmId": PslmId,
        "Remarks": UndoRemarks,
        "StageId": StageId


    };
    $.ajax({
        url: '/Projects/PullBAckProject',
        type: 'POST',
        data: userdata,
        success: function (response) {
            //console.log(response);
            if (response != null) {
                if (response == 2) {
                    location.reload();
                }
            }

        }
    });

}
//function Updateundo(ProjId, PslmId, UndoRemarks, StageId) {
//    var userdata =
//    {
//        "ProjectId": ProjId,
//        "PsmId": PslmId,
//        "Remarks": UndoRemarks,
//        "StageId": StageId


//    };
//    $.ajax({
//        url: '/Projects/UndoProject',
//        type: 'POST',
//        data: userdata,
//        success: function (response) {
//            console.log(response);
//            if (response != null) {
//                if (response == 2) {
//                    alert("Project Successfully Undo");
//                    location.reload();
//                }
//            }

//        }
//    });

//}
function reset() {

    $("#ddlfwdStage").val('');
    $("#ddlfwdSubStage").val('');
    $("#ddlfwdAction").val('');
    $("#ddlfwdFwdTo").val('');
    $("#txtRemarksfwd").val('');
    $("#TimeStampToProjfwd").val('');
}
function IsReadInbox(psmId) {

    $.ajax({
        url: '/Projects/IsReadInbox',
        type: 'POST',
        data: { "PsmId": psmId },
        success: function (response) {
            //console.log(response);

        }
    });
}

