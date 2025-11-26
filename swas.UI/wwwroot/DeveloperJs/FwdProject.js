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
    //$("#ddlfwdAction").change(function () { Comment by Kapoor

    //    mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html(), 0, "");
    //    mMsaterFwdTo(0, "ddlfwdCCTo", 8, 0, $("#SpnFwdStakeHolderId").html(), 0, "");
    //});

    //$("select[name='fwdoffrs']").change(function () {
    //    var selectedText = $(this).find("option:selected").text().trim();

    //    if (selectedText === "More") {
    //        $(this).hide();
    //        $("#loadFwdTo").show().focus();
    //        $("#searchBox").show().focus();
    //    } else {
    //        $(this).show();
    //        $("#searchBox").hide();
    //        $("#loadFwdTo").hide();
    //    }
    //});
    //$("#loadFwdTo").click(function () {
    //    mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html(), 0, "");
    //    $(this).hide();
    //    $("#searchBox").hide();
    //    $("select[name='fwdoffrs']").show().focus();
    //});
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





            // mMsaterFwdTo($(this).closest("tr").find("#SpnStakeHolderId").html(), 1);
        } else {
            $('.FwdDropdown').addClass('col-md-3');
            $('.FwdDropdown').addClass('col-md-6');
            $('.ProjectsFwdUnit').addClass('d-none');


            $("#searchBox").hide();

        }
    });

    //$("#ddlfwdSubStage").change(function () {

    //    mMsater(0, "ddlfwdAction", 9, $("#ddlfwdSubStage").val())
    //});

    //$("#ddlfwdAction").change(function () {

    //    mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnStakeHolderId").html())
    //});
    /* $(".btn-Undo").click(function () {*/
    $(document).on('click', '.btn-Undo',function () {
      
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

                //For Notification


                //AddNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2, $("#SpnCurrentUserStackholderID").html());


                //IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());

                //IsReadNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2);

                //$(this).closest("tr").removeClass("bold-text");

            }
        });
    });
    $(".btn-FwdHistorySent").click(function () {

        var projName = $(this).data('proj-name');
        var words = projName.split(" ");
        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
        //var finalTitle = "Mov History: " + shortProjName;
        var finalTitle = "Proj Name: " + projName;
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
        var finalTitle = "Proj Name: " + projName;
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

        var Isprocess = $(this).closest("tr").find("#SpnprojectIsProcess").html().trim() === "False" ? false : true;

        $('.FwdDropdown').addClass('col-md-3');
        $('.FwdDropdown').addClass('col-md-6');
        $('.ProjectsFwdUnit').addClass('d-none');


        $("#searchBox").val('');
        $("#searchBox").hide();
        TimeStampForcheckdate = new Date($(".TimeStampForcheckdate").html());
       
        var projName = $(this).data('proj-name');
        var words = projName.split(" ");
        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
        //var finalTitle = "Mov History: " + shortProjName;
        var finalTitle = "Proj Name: " + projName;
        $('#fwdModal').text(finalTitle);

        var projNameDetail = $(this).data('proj-name') + " " + "Move Details";
        $('.fwdtitle').text(projNameDetail);
   
        if (Isprocess== false) {

            mMsaterfwdStage($(this).closest("tr").find("#SpnStageId").html(), "ddlfwdStage", 5, 0, 1, 1)
        } 
       else if (words.includes("Re-Vetted")) {

            mMsaterfwdStage($(this).closest("tr").find("#SpnStageId").html(), "ddlfwdStage", 5, 0, 1, "Re-Vetted")
        } else {
            mMsaterfwdStage($(this).closest("tr").find("#SpnStageId").html(), "ddlfwdStage", 5, 0, 1)
        }
       

        mMsaterStage($(this).closest("tr").find("#SpnTimeStatusId").html(), "ddlfwdSubStage", 6, $(this).closest("tr").find("#SpnStageId").html(), $("#SpnStakeHolderId").html())
        /* mMsater(0, "ddlfwdAction", 9, $("#ddlfwdSubStage").val())*/
      
        mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 7, $(this).closest("tr").find("#SpnTimeStatusId").html())
        /*mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 9, $(this).closest("tr").find("#SpnTimeStatusId").html())*/
        mMsaterFwdTo($(this).closest("tr").find("#SpnTimeFromUnitId").html(), "ddlfwdFwdTo", 8, 0, $(this).closest("tr").find("#SpnStakeHolderId").html(), 0, "");
        mMsaterFwdTo(0, "ddlfwdCCTo", 8, 0, $(this).closest("tr").find("#SpnStakeHolderId").html(), 0, "");


        $("#spanFwdCurrentPslmId").html($(this).closest("tr").find("#SpnCurrentpsmId").html())
        $("#spanFwdProjectId").html($(this).closest("tr").find("#SpnCurrentProjId").html())
        $("#SpnFwdStakeHolderId").html($(this).closest("tr").find("#SpnStakeHolderId").html())
        $("#spanLegacyApproval").html($(this).closest("tr").find("#SpnApprove").html())

        var listItem = "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";

        $("#AttBody").html(listItem);

        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
        // IsReadNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2);
        setTimeout(function () {

            InboxNotificationCount();
        }, 200)

        $(this).closest("tr").removeClass("bold-text")

        $('#ProjFwd').modal('show');

        //$("#searchBox").autocomplete({
        //    minLength: 3,
        //    source: function (request, response) {
        //        var value = request.term;
        //        if (value) {
        //            mMsaterFwdTo($(this).closest("tr").find("#SpnTimeToUnitId").html(), "ddlfwdFwdTo", 8, 0, $(this).closest("tr").find("#SpnStakeHolderId").html(), 1, value);

        //        }
        //    }
        //});

        $(".Fwdtitle").html("Projects Move Details");
        $(".ProjectsFwd").removeClass("d-none");
        $(".Attmenthistory").addClass("d-none");



        // alert($(this).closest("tr").find("#SpnprojectStageId").html())
        //GetAllComments($(this).closest("tr").find("#SpnCurrentProjId").html());
    });

    // Ensure autocomplete works when the modal is opened or search box is focused
    //$("#searchBox").on("change", function () {
    //    var value =  $(this).autocomplete("search", $(this).val());  
    //});

    $(".btn-Obsn").click(function () {

        var projNameDetail = $(this).data('proj-name') + " " + "Move Details"
        var projName = $(this).data('proj-name');
        var words = projName.split(" ");
        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
        //var finalTitle = "Mov History: " + shortProjName;
        var finalTitle = "Proj Name: " + projNameDetail;
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

        fetchServerDate().then(function (S) {


            $('#TimeStampToProjfwd').attr('type', 'datetime-local');
            $('#TimeStampToProjfwd').val(S.todayDateTime); // Set today's date
            $('#TimeStampToProjfwd').prop('disabled', true); // Freeze input


        });
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
        var remarkslength = $("#Reamarks").val().length;       // length of text
        var attCount = $("#pdfFileInput")[0].files.length;

        if (remarkslength != 0 && attCount != 0) {
            Swal.fire({

                text: "Please Click On Upload File Button",
                icon: "warning",

                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "OK"
            })
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
        //$('.FwdDropdown').addClass('col-md-6');
        //$('.ProjectsFwdUnit').addClass('d-none');


        //$("#searchBox").hide()
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
                    }, 1000)
                    //setTimeout(function () {

                    //    UploadFiles();
                    //}, 1000)

                }
            });
        }
    });

    //$("#btnFwdConfirm").click(function () {
    //    location.reload();
    //    $('#ProjFwd').modal('hide');
    //});
});

$("#btnFwdConfirm").on("click", function () {
   
    SaveFwdTo($("#spanFwdCurrentPslmId").html());

});
function CheckFwdCondition(CurrentPslmId) {

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
            $('.FwdDropdown').addClass('col-md-6');
         $('.ProjectsFwdUnit').addClass('d-none');


            $("#searchBox").hide()
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
                    var legapproval = $("#spanLegacyApproval").html();


                    //var listItem = "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";
                    const rows = [
                        {
                            label: "To",
                            value: projectMoveData.fwdTo.id != "More"
                                ? projectMoveData.fwdTo.text
                                : projectMoveData.sentToUnit.text,
                            icon: "fa-user"
                        },
                        { label: "CC", value: projectMoveData.ccList.map(c => c.text).join(", ") || "-", icon: "fa-users" },
                        { label: "Stage", value: projectMoveData.stage.text || "-", icon: "fa-project-diagram" },
                        { label: "Sub Stage", value: projectMoveData.subStage.text || "-", icon: "fa-layer-group" },
                        { label: "Action", value: projectMoveData.action.text || "-", icon: "fa-tasks" },
                        { label: "Remarks", value: projectMoveData.remarks || "-", icon: "fa-comment-dots" }


                    ];

                    if (legapproval === "True") {
                        rows.push({
                            label: "Date Of FWD",
                            value: projectMoveData.fwdDate || "-",
                            icon: "fa-calendar-alt"
                        });
                    }

                    //forIcon
                    //<div class="me-3 flex-shrink-0">
                    //    <div class="icon-circle d-flex align-items-center justify-content-center">
                    //        <i class="fas ${row.icon} fa-lg"></i>
                    //    </div>
                    //</div >
                    // ---------- Render HTML ----------
                    const html = rows.map(row => `
  <div class="col-md-6 col-lg-4 mb-3">
    <div class="glass-card h-100 p-3">
          <div class="d-flex align-items-start">

       
        <div class="flex-grow-1">
          <div class="label mb-1">${row.label}</div>
          <div class="value">${row.value === "-" ? '<span class="placeholder">Not specified</span>' : row.value}</div>
        </div>
      </div>
    </div>
  </div>
`).join('');

                    $("#previewGrid").html(html);
                    CheckforPreviousapprovals()


                    //$("#DetailBody").html(listItem);

                    /*AttOnFWD();*/

                }
            }

        }
    });


}


function SaveFwdTo(CurrentPslmId, fd, allAttachments) {
    debugger;

    var psmdi = CurrentPslmId;
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
    var tocc = $("#ddlfwdCCTo").val();

    for (let i = 0; i < tocc.length; i++) {

        if (fwdunitid === tocc[i]) {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "The 'To' Unit and 'CC' Unit must not be the same!",

            });
            return false;
        }
    }

   
    //var currentDate = new Date();
    //var currentTime = currentDate.toLocaleTimeString('en-US', { hour12: false });
    //var time = $("#TimeStampToProjfwd").val();
    //var timeData = time + ' ' + currentTime;
    var formData = new FormData();
    var PsmId = $("#spanFwdCurrentPslmId").html()
    var ccidvalue = $("#ddlfwdCCTo").val();
    var userdata =
    {
        "ProjId": $("#spanFwdProjectId").html(),
        /* "StatusId": $("#ddlfwdSubStage").val(),*/

        "StatusActionsMappingId": $("#ddlfwdAction").val(),
        "Remarks": $("#txtRemarksfwd").val(),
        "ToUnitId": fwdunitid,

        //"TimeStamp": $("#TimeStampToProjfwd").val()
        "TimeStamp": TimeStamps,
        
        

    };
   

    //// Append userdata fields to FormData
    for (var key in userdata) {
        formData.append(key, userdata[key]);
    }

    //// Append files from fd (file input) to FormData
    //for (let [key, value] of fd.entries()) {
    //    if (value instanceof File) {
    //        formData.append("uploadfile[]", value);  // Appending files to formData
    //    }
    //}

    // Append currentpsmid to formData
    var currentpsmid = $("#spanFwdCurrentPslmId").html();
    formData.append("currentpsmid", currentpsmid);

    if (Array.isArray(ccidvalue)) {
        ccidvalue.forEach((value, index) => {
            formData.append(`Ccid[${index}]`, value);
        });
    } else {
        formData.append("Ccid", ccidvalue);
    }


    // Attach remarks and files (multiple attachments) directly to the form data
    if (allAttachments != null) {
        allAttachments.forEach((attachment, index) => {
            formData.append(`attachments[${index}].file`, attachment.file);  // Append file
            formData.append(`attachments[${index}].remarks`, attachment.remarks);  // Append remarks
        });
    };
 
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

                } if (response == -6) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "You cannot change the Stage before DDGIT Process ",

                    });

                }
                if (response == -7) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "Please select Send to Unit",

                    });
                    return false;

                }
               else {
                    $("#spanCurrentPslmId").html(response.psmId);

                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Project Successfully Submitted..!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                  //FwdProjConfirmtounit(CurrentPslmId);

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



var projectMoveData = {
    fwdTo: { id: "", text: "" },
    sentToUnit: { id: "", text: "" },
    ccList: [],
    stage: { id: "", text: "" },
    subStage: { id: "", text: "" },
    action: { id: "", text: "" },
    remarks: "",
    fwdDate: "",
    PDF: {
        remarks: "",
        attachment: ""
    },
};

$(document).on("change keyup", "#ddlfwdFwdTo, #searchBox, #ddlfwdCCTo, #ddlfwdStage, #ddlfwdSubStage, #ddlfwdAction, #txtRemarksfwd, #TimeStampToProjfwd,#pdfFileInput, #Reamarks", function () {

    const pdfFiles = $("#pdfFileInput")[0].files;
    const pdfList = pdfFiles.length > 0 ? Array.from(pdfFiles).map(f => f.name) : [];

    projectMoveData = {
        fwdTo: {
            id: $("#ddlfwdFwdTo").val(),
            text: $("#ddlfwdFwdTo option:selected").text()
        },
        sentToUnit: {
            id: $("#searchBox").val(),
            text: $("#searchBox option:selected").text()
        },
        ccList: $("#ddlfwdCCTo").val()
            ? $("#ddlfwdCCTo").val().map(id => ({
                id: id,
                text: $("#ddlfwdCCTo option[value='" + id + "']").text()
            }))
            : [],
        stage: {
            id: $("#ddlfwdStage").val(),
            text: $("#ddlfwdStage option:selected").text()
        },
        subStage: {
            id: $("#ddlfwdSubStage").val(),
            text: $("#ddlfwdSubStage option:selected").text()
        },
        action: {
            id: $("#ddlfwdAction").val(),
            text: $("#ddlfwdAction option:selected").text()
        },
        remarks: $("#txtRemarksfwd").val(),
        fwdDate: $("#TimeStampToProjfwd").val(),
        //PDF: {
        //    remarks: $("#Reamarks").val(),
        //    attachments: pdfList.map(file => ({
        //        name: file.name,
        //        url: URL.createObjectURL(file) // enables preview link
        //    }))
        //},
    };
});


$('#btnEditMove').on('click', function () {
    $(".ProjectsFwd").removeClass("d-none");
    $(".Attmenthistory").addClass("d-none");

})

function CheckforPreviousapprovals() {
    var userdata = {
        "ProjId": $("#spanFwdProjectId").html(),
        "StatusId": $("#ddlfwdSubStage").val(),
        "Actionsid": $("#ddlfwdAction").val(),
    };

    $.ajax({
        url: '/Projects/CheckPreviousApprovals',
        type: 'POST',
        data: userdata,
        success: function (response) {
            debugger;
            console.log(response);
            if (response.message.result !== "OK") {
                Swal.fire({
                    icon: 'warning',
                    title: 'Approval Required!',
                    html: response.message.result,
                    confirmButtonText: 'OK'
                });
            }
            else {
                // All good — continue your normal logic here
                // Example:
                // SubmitForward();
            }
        },
        error: function () {
            Swal.fire("Error", "Something went wrong!", "error");
        }
    });
}
