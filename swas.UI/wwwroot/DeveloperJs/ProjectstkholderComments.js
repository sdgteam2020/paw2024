//let memberTable = "";

$(document).ready(function () {

     
    InboxNotificationCount()
  
    GetProjCommentsByUnitId(0);
    $("#btnPending").addClass("border border-dark bold-border btn-small-large");

    $(".cmtbtn").unbind().click(function () {
        $(".cmtbtn").removeClass("border border-dark bold-border btn-small-large");
        $(this).addClass("border border-dark bold-border btn-small-large");
        let unitId = 0;
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
        GetProjCommentsByUnitId(unitId);
    });
   

    $("#btnStatusUpdate").unbind().click(function () {
    
        requiredFields = $('#projectcommentforstackholder').find('.requiredField');
        let allFieldsComplete = true;
        requiredFields.each(function (index) {
            if (this.value.length == 0) {
                $(this).addClass('is-invalid');
                allFieldsComplete = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });

       

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

function IsUnReadInbox(psmId) {

    $.ajax({
        url: '/Projects/IsUnReadInbox',
        type: 'POST',
        data: { "PsmId": psmId },
        success: function (response) {

        }
    });
}


function GetCommentBadgeCount(id) {
    $.ajax({
        url: '/Projects/GetProjectUnreadCound',
        type: 'POST',
        data: { StatusId: Id },
        success: function (response) {
            console.log(response); // handle response here
        },
        error: function (error) {
            console.error(error);
        }
    });
}


function GetProjCommentsByUnitId(Id) {
    let listItem = "";

    $("#DetailBody").html(listItem);
    
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

                    let count = 0;
                    let commentFalseCount = 0;
                    for (let i = 0; i < response.length; i++) {
                        let date = new Date(response[i].timeStamp);
                        let TimeStamp =
                            ("0" + date.getDate()).slice(-2) + '-' +
                            ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                            date.getFullYear() + ' ' +
                            ("0" + date.getHours()).slice(-2) + ':' +
                            ("0" + date.getMinutes()).slice(-2) + ':' +
                            ("0" + date.getSeconds()).slice(-2);


                        if (response[i].isComment == false) {
                            listItem += "<tr class='bold-text'>";
                            commentFalseCount++;
                            
                        } else {
                            listItem += "<tr>";
                        }
                        listItem += "<td class='noExport d-none'><span class='noExport d-none' id='spnProjId'>" + response[i].projId + "</span><span class='noExport d-none' id='spnpsmId'>" + response[i].psmId + "</span><span class='noExport d-none' id='DateType'>" + response[i].adminApprovalStatus + "</span></td>";
                        listItem += "<td class='align-middle sorting'>" + (count + 1) + "</td>";
                        

                        listItem += "<td class='align-middle RefLetter-container nowrap'>";

                        listItem += "<a href='/Projects/ProjHistory?EncyID=" +
                            encodeURIComponent(response[i].encyID) + "'>";

                        listItem += "<div class='tooltip-container ' data-tooltip='" +
                            response[i].projectName + "'>";

                        listItem += "<span class='projNameDetail short-text noExport'>" +
                            trimByChars(response[i].projectName, 40) +
                            "</span>";

                        listItem += "<span class='tooltip tooltip-text' id='projectNameforcomment'>" +
                            response[i].projectName +
                            "</span>";

                        listItem += "</div>"; // tooltip-container
                        listItem += "</a>";

                        listItem += "<div class='RefLetter'>" +
                            breakLinesByWords(response[i].projectName, 3) +
                            "</div>";

                        listItem += "</td>";

                        listItem += "<td class='align-middle'><span id='stakeholder'>" + response[i].stakeholder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='TimeStamp'>" + TimeStamp + "</span></td>";
                        if (response[i].stkStatusId == 1) {
                            listItem += "<td class='align-middle'><span id='status'>Accepted</span></td>";
                            listItem += "<td class='align-middle '><span id='btnedit'><button type='button'  class='cls-btncomment btn-icon btn-round btn-success mr-1'><i class='fas fa-comment'></i></button></td>";

                        }
                        else if (response[i].stkStatusId == 5) {
                            listItem += "<td class='align-middle'><span id='status'>Info</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button'  class='cls-btncomment btn-icon btn-round btn-success mr-1'><i class='fas fa-comment'></i></button></td>";
                        }
                        else if (response[i].stkStatusId == 2) {
                            listItem += "<td class='align-middle'><span id='status'>Obsn</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button'  class='cls-btncomment btn-icon btn-round  btn-warning mr-1'><i class='fas fa-comment'></i></button></td>";

                        }
                        else if (response[i].stkStatusId == 3) {
                            listItem += "<td class='align-middle'><span id='status'>Rejected</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btncomment btn-icon btn-round btn-danger mr-1'><i class='fas fa-comment'></i></button></td>";
                        }
                        else {
                            listItem += "<td class='align-middle'><span id='status'>Pending</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button'  class='cls-btncomment btn-icon btn-round btn-danger mr-1'><i class='fas fa-comment'></i></button></td>";
                        }
                        listItem += "</tr>";
                        count++;

                    }
                   /* $("#ProjectCommentCount").text(commentFalseCount);*/

                    IsReadComment(0, 0);
                   

                    if ($.fn.DataTable.isDataTable("#Comment")) {
                        $("#Comment").DataTable().clear().destroy();
                    }
                    $("#DetailBody").html(listItem);

                    initializeDataTable('#Comment');

                    

                    $("body").off("click").on("click", ".cls-btncomment", function () {
                        debugger;
                        $(".custom-modal").addClass("custom-modal-size")
                        let self = this;

                            let action = $(self).closest("tr").find("#status").html();
                        fetchServerDate().then(function (S) {
                            
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

                            if (stkid === 0) {
                                $(".cmtbtn").removeClass("border border-dark bold-border btn-small-large");
                                $("#btnPending").addClass("border border-dark bold-border btn-small-large");
                            }
                            $("#ProjectcommentForStackHolderprojId").html($(self).closest("tr").find("#spnProjId").html());
                            $("#ProjectcommentForStackHolderPsmId").html($(self).closest("tr").find("#spnpsmId").html());
                            $("#ProjectcommentForStackHolderDate_type").html($(self).closest("tr").find("#DateType").html());
                            IsReadComment($(self).closest("tr").find("#spnProjId").html(), $(self).closest("tr").find("#spnpsmId").html());
                            $(self).closest("tr").removeClass("bold-text");

                            reset();
                            mMsater(0, "ddlStatus", 4, 0);
                            $("#ProjCommentModal").modal('show');
                            GetAllComments($("#ProjectcommentForStackHolderPsmId").html(), $("#ProjectcommentForStackHolderprojId").html());
                            let projName = $(self).closest("tr").find("#projectNameforcomment").html();
                            let words = projName.split(" ");
                            let shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
                            let finalTitle = "Project Name: " + projName;
                            $('#addComment').text(finalTitle);

                            const dateTypeText = $(self).closest("tr").find("#DateType").text().trim().toLowerCase();
                            const dateType = (dateTypeText === "true");

                            $("#ProjectcommentForStackHolderDate_type").text(dateType);
                            let pad = "00";
                            let datef2 = new Date();
                            let months = "" + (datef2.getMonth() + 1);
                            let days = "" + datef2.getDate();
                            let monthsans = pad.substring(0, pad.length - months.length) + months;
                            let dayans = pad.substring(0, pad.length - days.length) + days;
                            let year = datef2.getFullYear();
                            let hh = pad.substring(0, pad.length - `${datef2.getHours()}`.length) + `${datef2.getHours()}`;
                            let mm = pad.substring(0, pad.length - `${datef2.getMinutes()}`.length) + `${datef2.getMinutes()}`;
                            let ss = `${datef2.getSeconds()}`;

                            let todayDate = `${year}-${monthsans}-${dayans}`;
                            let todayDateTime = `${year}-${monthsans}-${dayans}T${hh}:${mm}`;

                            const formattedDateTime = new Date(S.todayDateTime).toISOString().slice(0, 16);  // Convert to YYYY-MM-DDTHH:MM
                            if (dateType) {
                               
                                $('#CommentDateFwd').attr('type', 'datetime-local');
                                $('#CommentDateFwd').attr('max', formattedDateTime);
                                $('#CommentDateFwd').prop('disabled', false); // Allow user input
                                $('#CommentDateFwd').val(formattedDateTime);
                            } else {
                               
                                $('#CommentDateFwd').attr('type', 'datetime-local');
                                $('#CommentDateFwd').val(S.todayDateTime); // Set today's date
                                $('#CommentDateFwd').prop('disabled', true); // Freeze input
                            }
                           
                        });
                    });



                    $("body").on("click", ".projNameDetail", function () {

                        IsReadComment($(this).closest("tr").find("#spnProjId").html(), $(this).closest("tr").find("#spnpsmId").html());

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

    let formData = new FormData();
    let totalFiles = document.getElementById("uploadfile").files.length;
    for (let i = 0; i < totalFiles; i++) {
        let file = document.getElementById("uploadfile").files[i];
        formData.append("uploadfile", file);

    }

    let dateValue = $('#CommentDateFwd').val();
    let currentDate = new Date();
    let commentDateTime = '';
    if ($('#CommentDateFwd').attr('type') === 'date') {
        if (!dateValue) {
            alert('Please select a date .');
            return;
        }
        let currentTime = currentDate.toTimeString().split(' ')[0]; // Get current time in HH:mm:ss
        commentDateTime = dateValue + ' ' + currentTime;
    } else if ($('#CommentDateFwd').attr('type') === 'datetime-local') {
        if (!dateValue) {
            alert('Please select date and time.');
            return;
        }
        commentDateTime = dateValue.replace('T', ' '); // Format datetime-local to space-separated
    }



    formData.append("Comments", encryptData($("#Comments").val()));
    formData.append("StkStatusId", encryptData($("#ddlStatus").val()));
    formData.append("ProjectId", encryptData($("#ProjectcommentForStackHolderprojId").html()));
    formData.append("psmid", encryptData($("#ProjectcommentForStackHolderPsmId").html()));
    formData.append("CommentDate", encryptData(commentDateTime));


    $.ajax({
        type: "POST",
        url: '/Projects/SendCommentonProject',
        data: formData,
        contentType: false,
        processData: false,

        beforeSend: function () {
            $('#uploadLoader').show();
        },

        success: function (response) {
            $('#uploadLoader').hide();

            try {
                if (response == 0) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Something went wrong!',
                        text: 'Unable to save comment.',
                    });
                }

                else if (response == 1) {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: 'Comment Sent successfully',
                        showConfirmButton: false,
                        timer: 3000
                    }).then(() => {

                        if ($("#ddlStatus").val() == 1) {
                            FwdProjConfirm($("#ProjectcommentForStackHolderPsmId").html());
                        }

                        GetAllComments(
                            $("#ProjectcommentForStackHolderPsmId").html(),
                            $("#ProjectcommentForStackHolderprojId").html()
                        );

                        UnReadNotification($("#ProjectcommentForStackHolderprojId").html(), 2);

                        IsUnReadComment(
                            $("#ProjectcommentForStackHolderprojId").html(),
                            $("#ProjectcommentForStackHolderPsmId").html()
                        );

                        reset();
                    });
                }

                else if (response == 6) {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'error',
                        title: 'Action Not Allowed',
                        html: `
                        <div style="text-align:left;">
                            <ol>
                                <li>No Amdts Allowed as the Project is Already Accepted By You!</li>
                                <li>Only info is allowed after acceptance.</li>
                            </ol>
                        </div>
                    `,
                        showConfirmButton: true
                    });
                }

                else if (response == 8) {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'error',
                        title: 'File too large',
                        text: 'PDF size must be less than 10 MB',
                        showConfirmButton: true
                    });
                }

                // 🔥 HANDLE CUSTOM BACKEND ERRORS
                else if (response == -400) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Invalid Data',
                        text: 'Bad request or invalid input.'
                    });
                }

                else if (response == -401) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Session Expired',
                        text: 'Please login again.'
                    }).then(() => {
                        window.location.reload();
                    });
                }

                else if (response == -500) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Security Error',
                        text: 'Decryption failed or data tampered.'
                    });
                }

                else {
                    // 🔥 UNKNOWN RESPONSE
                    Swal.fire({
                        icon: 'error',
                        title: 'Unexpected Error',
                        text: 'Unknown response from server.'
                    });
                }

            } catch (e) {
                console.error("UI handling error:", e);

                Swal.fire({
                    icon: 'error',
                    title: 'UI Error',
                    text: 'Something went wrong while processing response.'
                });
            }
        },

        error: function (xhr, status, error) {
            $('#uploadLoader').hide();

            console.error("AJAX ERROR:", {
                status: status,
                error: error,
                responseText: xhr.responseText
            });

            let message = "Something went wrong. Please try again.";

            if (xhr.status === 0) {
                message = "Network error. Check your internet connection.";
            }
            else if (xhr.status === 404) {
                message = "API not found (404).";
            }
            else if (xhr.status === 500) {
                message = "Server error (500). Please contact admin.";
            }
            else if (xhr.responseText) {
                message = xhr.responseText;
            }

            Swal.fire({
                icon: 'error',
                title: 'Request Failed',
                text: message
            });
        }
    });
}
function GetAllComments(PsmId, projId) {

    let user_ids =
    {
        "PsmId": PsmId,

        "ProjId": projId
        }

    let encrypted_ids = encryptData(user_ids)
    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            encrypted_ids: encrypted_ids
        },
        success: function (data) {
           
            let commentContainer = '';
            let userDetails = '';
            if (data != null) {
                for (let i = 0; i < data.length; i++) {
                    let date = new Date(data[i].date);
                    let formattedDate =
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

                    commentContainer += '<div class="comment-box">';
                    commentContainer += '<div class="comment-header">';
                    commentContainer += '<div>';
                    commentContainer += '<span>' + data[i].stakeholder + ' (' + userDetails + ') </span>';
                    commentContainer += '<div class="comment-meta">' + DateFormateddMMyyyyhhmmss(data[i].date) + '</div>';
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
                        commentContainer += '<img src="/assets/images/icons/pdfimg.png" alt="PDF icon" class="pdf-icon">';
                        commentContainer += '</a>';
                    }

                    commentContainer += '</span>';
                    commentContainer += '</div>';
                    commentContainer += '</div>';
                    commentContainer += '<div class="comment-content formated-text"><p>' + data[i].comments + '</p></div>';
                    commentContainer += '</div>';
                }

                $('#ChatBoxForStackholdercomment').empty().html(commentContainer);
            }

        },
        error: function () {
            alert('Error fetching comments.6');
        }
    });
}




function IsReadComment(ProjId, PsmId) {
    $.ajax({
        url: '/Projects/IsReadComment',
        type: 'POST',
        data: { "ProjId": ProjId, "PsmId": PsmId },
        success: function (response) {
            if (response > 0) {
                $("#ProjectCommentCount").removeClass("d-none");
                $("#ProjectCommentCount").text(response);
            }
            else {
                $("#ProjectCommentCount").addClass("d-none");
            }
            

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
          
        }
    })
}

function IsReadInbox(psmId) {

    $.ajax({
        url: '/Projects/IsReadInbox',
        type: 'POST',
        data: { "PsmId": psmId },
        success: function (response) {

        }
    });
}

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
            console.log(response);
           
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

        }
    });
}


document.addEventListener('DOMContentLoaded', function () {
    const datePicker = document.getElementById('CommentDateFwd');
    if (datePicker) {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        const formattedDate = `${year}-${month}-${day}T${hours}:${minutes}`;
        datePicker.max = formattedDate;
    }
});
$('#ProjCommentModal').on('hidden.bs.modal', function (e) {
    $('#CommentDateFwd').val('');
});

            