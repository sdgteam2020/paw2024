var memberTable = "";

$(document).ready(function () {
  
    InboxNotificationCount()
  
    GetProjCommentsByUnitId(0);
    $("#btnPending").addClass("border border-dark bold-border btn-small-large");

  
   

    $("#btnStatusUpdate").unbind().click(function () {

        requiredFields = $('#projectcommentforstackholder').find('.requiredField');
        var allFieldsComplete = true;
        requiredFields.each(function (index) {
            if (this.value.length == 0) {
                $(this).addClass('is-invalid');
                allFieldsComplete = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });

        //var fileInput = $("#uploadfile");
        //if (fileInput.val().trim() === "") {
        //    fileInput.addClass('is-invalid');
        //    allFieldsComplete = false;
        //} else {
        //    fileInput.removeClass('is-invalid');
        //}

        if (!allFieldsComplete) {


        }
        else {
            $('#uploadLoader').show();
            setTimeout(function () {
                SendMsg();
            },500);
           
        }
    });

});

$(".cmtbtn").unbind().click(function () {
    $(".cmtbtn").removeClass("border border-dark bold-border btn-small-large");

    // Add the 'border-dark', 'bold-border', 'btn-small-large' (custom size) classes to the clicked button
    $(this).addClass("border border-dark bold-border btn-small-large");

    // Get the corresponding unit ID based on the button ID
    var unitId = 0;
    switch ($(this).attr('id')) {
        case 'btnAccepted':
            unitId = 1;
            break;
        case 'btnObsn':
            unitId = 2;
            break;
        case 'btnRejected':
            unitId = 3;
            break;
        case 'btnInfo':
            unitId = 5;
            break;
        default:
            unitId = 0; // For btnPending
    }

    // Call the function to get project comments by the determined unit ID
    GetProjCommentsByUnitId(unitId);
});
function IsUnReadInbox(psmId) {

    $.ajax({
        url: '/Projects/IsUnReadInbox',
        type: 'POST',
        data: { "PsmId": psmId },
        success: function (response) {
            //console.log(response);

        }
    });
}


function GetProjCommentsByUnitId(Id) {
    var listItem = "";

    $("#DetailBody").html(listItem);
    /*let boldCount = 0;*/
    $.ajax({
        url: '/Projects/GetProjCommentsByUnitId',
        type: 'POST',
        data: { "StatusId": Id },
        success: function (response) {
           
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({
                        text: ""
                    });
                }
                else if (response == 0) {
                    listItem += "<tr><td class='text-center' colspan=6>No Record Found</td></tr>";

                    $("#DetailBody").html(listItem);

                }

                else {

                    var count = 0;
                    var commentFalseCount = 0;
                    for (var i = 0; i < response.length; i++) {
                        
                        var date = new Date(response[i].timeStamp);
                        var TimeStamp =
                            ("0" + date.getDate()).slice(-2) + '-' +
                            ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                            date.getFullYear() + ' ' +
                            ("0" + date.getHours()).slice(-2) + ':' +
                            ("0" + date.getMinutes()).slice(-2) + ':' +
                            ("0" + date.getSeconds()).slice(-2);


                        if (response[i].isComment == false) {
                            listItem += "<tr class='bold-text'>";
                            commentFalseCount++;
                            /* boldCount++;*/
                        } else {
                            listItem += "<tr>";
                        }
                        //listItem += "<td class='noExport d-none'><span class='noExport d-none' id='spnProjId'>" + response[i].projId + "</span><span class='noExport d-none' id='spnpsmId'>" + response[i].psmId + "</span></td>";
                        listItem += "<td class='noExport d-none'><span class='noExport d-none' id='spnProjId'>" + response[i].projId + "</span><span class='noExport d-none' id='spnpsmId'>" + response[i].psmId + "</span><span class='noExport d-none' id='DateType'>" + response[i].adminApprovalStatus + "</span></td>";
                        //listItem += "<td class='align-middle'><span id='divName'>" + count + "</span></td>";
                         listItem += "<td class='align-middle '>" + (count + 1) + "</td>";
                       // listItem += "<td class='align-middle ser-no'></td>";

                        listItem += "<td class='align-middle'>";
                        listItem += "<a  href='/Projects/ProjHistory?EncyID=" + encodeURIComponent(response[i].encyID) + "'>";
                        listItem += "<span id='projectName' class='projNameDetail' >" + response[i].projectName + "</span>";
                        listItem += "</a>";
                        listItem += "</td>";
                        listItem += "<td class='align-middle'><span id='stakeholder'>" + response[i].stakeholder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='TimeStamp'>" + TimeStamp + "</span></td>";
                        if (response[i].stkStatusId == 1) {
                            listItem += "<td class='align-middle'><span id='status'>Accepted</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button style='height:30px;width:30px' type='button' class='cls-btncomment btn-icon btn-round btn-success mr-1'><i class='fas fa-comment'></i></button></td>";

                        }
                        else if (response[i].stkStatusId == 5) {
                            listItem += "<td class='align-middle'><span id='status'>Info</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' style='height:30px;width:30px' class='cls-btncomment btn-icon btn-round btn-success mr-1'><i class='fas fa-comment'></i></button></td>";
                        }
                        else if (response[i].stkStatusId == 2) {
                            listItem += "<td class='align-middle'><span id='status'>Obsn</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' style='height:30px;width:30px' class='cls-btncomment btn-icon btn-round  btn-warning mr-1'><i class='fas fa-comment'></i></button></td>";

                        }
                        else if (response[i].stkStatusId == 3) {
                            listItem += "<td class='align-middle'><span id='status'>Rejected</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' style='height:30px;width:30px' class='cls-btncomment btn-icon btn-round btn-danger mr-1'><i class='fas fa-comment'></i></button></td>";
                        }
                        else {
                            listItem += "<td class='align-middle'><span id='status'>Pending</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' style='height:30px;width:30px' class='cls-btncomment btn-icon btn-round btn-danger mr-1'><i class='fas fa-comment'></i></button></td>";
                        }
                        listItem += "</tr>";
                        count++;

                    }
                    $("#ProjectCommentCount").text(commentFalseCount);

                    if (commentFalseCount > 0) {
                        $("#ProjectCommentCount").removeClass("d-none");
                    }
                    else {
                        $("#ProjectCommentCount").addClass("d-none");
                    }

                    if ($.fn.DataTable.isDataTable("#Comment")) {
                        $("#Comment").DataTable().clear().destroy();
                    }
                    $("#DetailBody").html(listItem);

                    initializeDataTable('#Comment');

                    /* $('#ProjectCommentCount').html(boldCount);*/

                    $("body").off("click").on("click", ".cls-btncomment", function () {
                /* $('.cls-btncomment').click(function () {*/
                        
                        var action = $(this).closest("tr").find("#status").html()
                        let stkid = 0;
                        switch (action) {
                            case 'Accepted':
                                stkid = 1;
                                break;
                            case 'Obsn':
                                stkid = 2;
                                break;
                            case 'Rejected':
                                stkid = 3;
                                break;
                            case 'Info':
                                stkid = 5;
                                break;
                            default:
                                stkid = 0; // For btnPending
                        }

                        if (stkid === 0)
                        {
                            $(".cmtbtn").removeClass("border border-dark bold-border btn-small-large");

                            $("#btnPending").addClass("border border-dark bold-border btn-small-large");

                        }
                        $("#ProjectcommentForStackHolderprojId").html($(this).closest("tr").find("#spnProjId").html());
                        $("#ProjectcommentForStackHolderPsmId").html($(this).closest("tr").find("#spnpsmId").html());
                        $("#ProjectcommentForStackHolderDate_type").html($(this).closest("tr").find("#DateType").html());

                        IsReadComment($(this).closest("tr").find("#spnProjId").html(), $(this).closest("tr").find("#spnpsmId").html());
                        // IsReadNotification($(this).closest("tr").find("#spnProjId").html(), 1);
                     $(this).closest("tr").removeClass("bold-text")
                    




                   
                    
                        reset()
                        mMsater(0, "ddlStatus", 4, 0)
                        $("#ProjCommentModal").modal('show');
                        GetAllComments($("#ProjectcommentForStackHolderPsmId").html(), $("#ProjectcommentForStackHolderprojId").html());

                        // Added from here for pop up heading with project name in comment (added by Divyanshu on 04/02/2025)
                        //var projName = $(this).closest("tr").find("#projectName").html() + "  " + "Comments";
                        //$('#addComment').text(projName);
                        var projName = $(this).closest("tr").find("#projectName").html();
                        var words = projName.split(" ");
                        // Limit to 6 words and add "..." if needed
                        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
                        //var finalTitle = "Mov History: " + shortProjName;
                        var finalTitle = "Mov History: " + projName;
                        $('#addComment').text(finalTitle);


                        const dateTypeText = $(this).closest("tr").find("#DateType").text().trim().toLowerCase();
                        const dateType = (dateTypeText === "true");

                        $("#ProjectcommentForStackHolderDate_type").text(dateType);
                        var pad = "00";
                        var datef2 = new Date();


                        var months = "" + (datef2.getMonth() + 1);
                        var days = "" + datef2.getDate();
                        var monthsans = pad.substring(0, pad.length - months.length) + months;
                        var dayans = pad.substring(0, pad.length - days.length) + days;
                        var year = datef2.getFullYear();
                        var hh = pad.substring(0, pad.length - `${datef2.getHours()}`.length) + `${datef2.getHours()}`;
                        var mm = pad.substring(0, pad.length - `${datef2.getMinutes()}`.length) + `${datef2.getMinutes()}`;
                        var ss = `${datef2.getSeconds()}`;

                        // Today's date and time in the required formats
                        var todayDate = `${year}-${monthsans}-${dayans}`;
                        var todayDateTime = `${year}-${monthsans}-${dayans}T${hh}:${mm}`;

                        if (dateType) {
                            $('#CommentDateFwd').attr('type', 'datetime-local');
                            $('#CommentDateFwd').attr('max', todayDateTime);
                            $('#CommentDateFwd').prop('disabled', false); // Allow user input
                        } else {
                            $('#CommentDateFwd').attr('type', 'date');
                            $('#CommentDateFwd').val(todayDate); // Set today's date
                            $('#CommentDateFwd').prop('disabled', true); // Freeze input
                        }
                        setTimeout(function () {
                          
                            GetProjCommentsByUnitId(stkid);
                        },200)
                      
                    });

                    $("body").on("click", ".projNameDetail", function () {

                        IsReadComment($(this).closest("tr").find("#spnProjId").html(), $(this).closest("tr").find("#spnpsmId").html());
                        // IsReadNotification($(this).closest("tr").find("#spnProjId").html(), 1);

                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=6>No Record Found</td></tr>";

                $("#DetailBody").html(listItem);

            }
        },
        error: function (result) {
            Swal.fire({
                text: ""
            });
        }
    });
}




function SendMsg() {
    debugger;
    var formData = new FormData();
    var totalFiles = document.getElementById("uploadfile").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("uploadfile").files[i];
        formData.append("uploadfile", file);

    }

    var dateValue = $('#CommentDateFwd').val();
    var currentDate = new Date();

    // Add server's current time if only a date is selected
    var commentDateTime = '';
    if ($('#CommentDateFwd').attr('type') === 'date') {
        if (!dateValue) {
            alert('Please select a date .');
            return;
        }
        var currentTime = currentDate.toTimeString().split(' ')[0]; // Get current time in HH:mm:ss
        commentDateTime = dateValue + ' ' + currentTime;
    } else if ($('#CommentDateFwd').attr('type') === 'datetime-local') {
        if (!dateValue) {
            alert('Please select date and time.');
            return;
        }
        commentDateTime = dateValue.replace('T', ' '); // Format datetime-local to space-separated
    }



    formData.append("Comments", $("#Comments").val());
    formData.append("StkStatusId", $("#ddlStatus").val());
    formData.append("ProjectId", $("#ProjectcommentForStackHolderprojId").html());
    formData.append("psmid", $("#ProjectcommentForStackHolderPsmId").html());
    //formData.append("CommentDate", $("#CommentDateFwd").val());
    formData.append("CommentDate", commentDateTime);
    $.ajax({
        type: "POST",
        url: '/Projects/SendCommentonProject',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $('#uploadLoader').hide();
            if (response == 0) {

            }
            if (response == 1) {
                $('#uploadLoader').hide();
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Comment Sent successfully',
                    showConfirmButton: false,
                    timer: 3000
                
                }).then(() => {
                    debugger;
                    //  if ($("#ddlStatus").val() == 1) {
                    FwdProjConfirm($("#ProjectcommentForStackHolderPsmId").html());
                    // }


                    GetAllComments($("#ProjectcommentForStackHolderPsmId").html(), $("#ProjectcommentForStackHolderprojId").html());
                    //GetProjCommentsByUnitId();
                    //IsUnReadComment($("#ProjectcommentForStackHolderprojId").html());
                    //GetNotification($("#ProjectcommentForStackHolderprojId").html());
                    UnReadNotification($("#ProjectcommentForStackHolderprojId").html(), 2);
                    IsUnReadComment($("#ProjectcommentForStackHolderprojId").html(), $("#ProjectcommentForStackHolderPsmId").html());
                    reset();
                })


            }
            else if (response == 6) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'Error',
                    /*title: 'No Amdts Allowed as the Project is Already Accepted By You !',*/
                    title: '<div style="text-align: left;">' +
                        '<ol style="margin: 0; padding-left: 20px; text-align: left;">' +
                        '<li>No Amdts Allowed as the Project is Already Accepted By You!</li>' +
                        '<li>However, only info is allowed after the project is accepted.</li>' +
                        '</ol>' +
                        '</div>',
                    showConfirmButton: true,

                });
            }
            else if (response == 8) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'Error',
                    title: 'Pdf Size allow less then 10 Mb !',
                    showConfirmButton: true,

                });
            }




            //IsReadComment($("#ProjectcommentForStackHolderprojId").html());
        },
        error: function (error) {

            $('#uploadLoader').hide();
        }
    });
}
function GetAllComments(PsmId, projId) {
    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            "PsmId": PsmId,
            "stakeholderId": 1,
            "ProjId": projId
        },
        success: function (data) {

            var commentContainer = '';
            var userDetails = '';
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    var date = new Date(data[i].date);
                    var formattedDate =
                        ("0" + date.getDate()).slice(-2) + '-' +
                        ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                        date.getFullYear() + ' ' +
                        ("0" + date.getHours()).slice(-2) + ':' +
                        ("0" + date.getMinutes()).slice(-2) + ':' +
                        ("0" + date.getSeconds()).slice(-2);
                    if (data[i].userDetails == null)
                        userDetails = '';
                    else
                        userDetails = data[i].userDetails

                    commentContainer += '<div class="comment-box" style="text-align: justify;">'; // Use text-align: justify for justified text
                    commentContainer += '<div class="comment-header">';
                    commentContainer += '<div>';
                    commentContainer += '<span style="font-family: Arial; color: #0793f7;">' + data[i].stakeholder + ' (' + userDetails + ') </span>';
                    commentContainer += '<div style="margin-left: 0px;" class="comment-meta">' + DateFormateddMMyyyyhhmmss(data[i].date) + '</div>';
                    commentContainer += '</div>';
                    commentContainer += '<div>';
                    if (data[i].status == "Accepted" || data[i].status == "Info")
                        commentContainer += '<span class="comment-meta badge badge-success text-white">' + data[i].status + '</span>';
                    else if (data[i].status == "Obsn")
                        commentContainer += '<span class="comment-meta badge badge-warning text-white">' + data[i].status + '</span>';
                    else
                        commentContainer += '<span class="comment-meta badge badge-danger text-white">' + data[i].status + '</span>';
                    if (data[i].attpath !== '' && data[i].attpath !== null) {
                        commentContainer += '<a href="/Home/WaterMark3?id=' + data[i].attpath + '" target="_blank">';
                        commentContainer += '<img src="/assets/images/icons/pdfimg.png" alt="PDF icon" style="width: 24px; height: 24px;">';
                        commentContainer += '</a>';
                    }


                    commentContainer += '</span>';
                    commentContainer += '</div>';
                    commentContainer += '</div>';
                    commentContainer += '<div class="comment-content formated-text"><p>' + data[i].comments + '</p></div>';
                    commentContainer += '</div>';
                }

                commentContainer += '</div>'; // Close the container
                $('#ChatBoxForStackholdercomment').empty().html(commentContainer);




            }

        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}



function IsReadComment(ProjId, PsmId) {
    $.ajax({
        url: '/Projects/IsReadComment',
        type: 'POST',
        data: { "ProjId": ProjId, "PsmId": PsmId },
        success: function (response) {
            //console.log(response);
        }
    })
}


function GetNotificationInbox(ProjId) {
    alert("om");
    $.ajax({
        url: '/Home/GetNotificationInbox',
        type: 'POST',
        data: { "ProjId": ProjId },
        success: function (response) {

        }
    })
}

function IsUnReadComment(ProjId, PsmId) {
    $.ajax({
        url: '/Projects/IsUnReadComment',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "PsmId": PsmId
        },
        success: function (response) {
            //console.log(response);
            /*window.location.reload();*/
        }
    })
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

//function IsUnReadNotification(ProjId) {

//    $.ajax({
//        url: '/Projects/IsUnReadNotification',
//        type: 'POST',
//        data: { "ProjId": ProjId },
//        success: function (response) {
//            console.log(response);

//        }
//    });
//}

//function IsCommentedUnreadNotification(ProjId) {

//    $.ajax({
//        url: '/Projects/IsCommentedUnreadNotification',
//        type: 'POST',
//        data: { "ProjId": ProjId },
//        success: function (response) {
//            console.log(response);

//        }
//    });
//}

//function IsReadNotification(ProjId) {

//    $.ajax({
//        url: '/Projects/IsReadNotification',
//        type: 'POST',
//        data: { "ProjId": ProjId },
//        success: function (response) {
//            console.log(response);

//        }
//    });
//}

function reset() {
    $("#Comments").val("");
    $("#ddlStatus").val(0);
    $("#uploadfile").val("");
}

function FwdProjConfirm(psmid) {
    $.ajax({
        url: '/Projects/FwdProjConfirm',
        type: 'POST',
        data: { "PslmId": psmid },
        success: function (response) {
            //console.log(response);


            if (response >= 1) {





            }

        }
    });
}

 
function InboxNotificationCount() {
    $.ajax({
        url: '/Notification/GetInboxUnreadCount', // Replace with your actual route
        type: 'GET',
        success: function (unreadCount) {
            debugger;
          
            $('#InboxCount').text(unreadCount);


            if (unreadCount > 0) {
                $("#InboxCount").removeClass("d-none");
            }
            else {
                $("#InboxCount").addClass("d-none");
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching unread count:', error);
        }
    });
}
function IsCommentedUnreadNotification(ProjId) {

    $.ajax({
        url: '/Projects/IsCommentedUnreadNotification',
        type: 'POST',
        data: { "ProjId": ProjId },
        success: function (response) {
            //console.log(response);

        }
    });
}

