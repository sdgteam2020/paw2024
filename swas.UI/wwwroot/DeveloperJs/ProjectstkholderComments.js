
let currentCommentStatusId = 0;

// Cache data separately for every status
let commentDataCache = {
    0: [],
    1: [],
    2: [],
    3: [],
    5: [],
    6: []
};

function filterProjectCommentsByType(statusId, selectedType) {

    let allData = commentDataCache[statusId] || [];
    let filteredData = allData;

    if (selectedType === 2) {

        // STC Auto
        filteredData = allData.filter(function (project) {

            return project.aiml === false ||
                project.aiml === 0 ||
                project.aiml === "0";
        });

    }
    else if (selectedType === 3) {

        // STC AI/ML
        filteredData = allData.filter(function (project) {

            return project.aiml === true ||
                project.aiml === 1 ||
                project.aiml === "1";
        });
    }

    bindProjectComments(filteredData);
}
$(document).ready(function () {

     
    InboxNotificationCount()
  
    GetProjCommentsByUnitId(0);
    $("#btnPending")
        .addClass(
            "border border-dark bold-border btn-small-large"
        );

   
    $(document)
        .off("click", ".cmtbtn")
        .on("click", ".cmtbtn", function () {

            $(".cmtbtn")
                .removeClass(
                    "border border-dark bold-border btn-small-large"
                );

            $(this)
                .addClass(
                    "border border-dark bold-border btn-small-large"
                );


            switch ($(this).attr("id")) {

                case "btnAccepted":
                    currentCommentStatusId = 1;
                    break;

                case "btnObsn":
                    currentCommentStatusId = 2;
                    break;

                case "btnRejected":
                    currentCommentStatusId = 3;
                    break;

                case "btnInfo":
                    currentCommentStatusId = 5;
                    break;

                case "btnNA":
                    currentCommentStatusId = 6;
                    break;

                default:
                    currentCommentStatusId = 0;
                    break;
            }


            // Fetch ALL project types for this status
            GetProjCommentsByUnitId(
                currentCommentStatusId
            );
        });

        $(document)
        .off("change", "#ddlProjectType")
        .on("change", "#ddlProjectType", function () {
            debugger;
            const selectedType =
                Number($(this).val()) || 1;

            filterProjectCommentsByType(
                currentCommentStatusId,
                selectedType
            );
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
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {

            if (response.type === 400) {
                return showError(response.message);
            }

            if (response.type === 401) {
                return showError(response.message);
            }

            if (response.type === 404) {
                return showError(response.message);
            }

            if (response === -1) {
                return showError("Something went wrong.");
            }

            if (response > 0) {
                console.log("Marked as unread successfully.");
            }
        },
        error: function () {
            showError("Server error occurred.");
        }
    });
}

function showError(msg) {
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: msg
    });
}
$(document).on('change', '#selectAll', function () {
    $('.rowCheck').prop('checked', this.checked);
   
    updateSelectionState();
});
function GetCommentBadgeCount(id) {
    $.ajax({
        url: '/Projects/GetProjectUnreadCound',
        type: 'POST',
       data: { StatusId: id },
        success: function (response) {
            console.log(response); // handle response here
        },
        error: function (error) {
            console.error(error);
        }
    });
}


function GetProjCommentsByUnitId(statusId) {
    let listItem = "";

    $("#DetailBody").html(listItem);
    
    $.ajax({
        url: '/Projects/GetProjCommentsByUnitId',
        type: 'POST',
        data: { "StatusId": statusId },
        success: function (response) {
            
            if (response == null || response === "null") {

                commentDataCache[statusId] = [];

                bindProjectComments([]);
                return;
            }

            if (response === -1) {

                    Swal.fire({
                        text: ""
                    });

                return;
            }

            if (response === 0) {

                commentDataCache[statusId] = [];

                bindProjectComments([]);
                return;
            }

            // Save full data for this status
            commentDataCache[statusId] = response || [];

            const selectedType =
                Number($("#ddlProjectType").val()) || 1;

            // Filter cached data
            filterProjectCommentsByType(
                statusId,
                selectedType
            );
        },

        error: function () {

            Swal.fire({
                text: ""
            });
        }
    });
}

function bindProjectComments(response) {

    let listItem = "";

    // Destroy existing DataTable FIRST
    if (!response || response.length === 0) {

        if ($.fn.DataTable.isDataTable("#Comment")) {
            $("#Comment").DataTable().clear().destroy();
        }

        $("#DetailBody").empty();

        initializeDataTable("#Comment");

        return;
    }
    $('.hdnotapp').addClass('d-none');
    let count = 0;
    let commentFalseCount = 0;

    for (let i = 0; i < response.length; i++) {

        const item = response[i];

        let date = new Date(item.timeStamp);

        let TimeStamp =
            ("0" + date.getDate()).slice(-2) + "-" +
            ("0" + (date.getMonth() + 1)).slice(-2) + "-" +
            date.getFullYear() + " " +
            ("0" + date.getHours()).slice(-2) + ":" +
            ("0" + date.getMinutes()).slice(-2) + ":" +
            ("0" + date.getSeconds()).slice(-2);


        if (item.isComment === false) {

            listItem += "<tr class='bold-text'>";
            commentFalseCount++;

        } else {

            listItem += "<tr>";
        }


        listItem +=
            "<td class='noExport d-none'>" +

            "<span class='noExport d-none spnProjId'>" +
            item.projId +
            "</span>" +

            "<span class='noExport d-none spnpsmId'>" +
            item.psmId +
            "</span>" +

            "<span class='noExport d-none DateType'>" +
            item.adminApprovalStatus +
            "</span>" +

            "</td>";


        listItem += `<td class='noExport'>`;
        if (item.stkStatusId != 1) {
          
            listItem +=
                
                "<input type='checkbox' " +
                "class='rowCheck' " +
                "value='" + item.projId + "' " +
                "data-psmid='" + item.psmId + "' " +
                "data-projname='" + item.projectName + "'>";
        }
        listItem += `</td>`
        listItem += 
            "<td class='align-middle sorting'>" +
            (count + 1) +
            "</td>";


        listItem +=
            "<td class='align-middle sorting'>" +
            "<span>" +
            item.projId +
            "</span>" +
            "</td>";


        listItem +=
            "<td class='align-middle RefLetter-container'>";

        listItem +=
            "<a href='/Projects/ProjHistory?EncyID=" +
            encodeURIComponent(item.encyID) +
            "' target='_blank'>";

        listItem +=
            "<span class='projNameDetail noExport'>" +
            trimByChars(item.projectName, 35) +
            "</span>";

        listItem += "</a>";

        listItem +=
            "<div class='projectName RefLetter'>" +
            breakLinesByWords(item.projectName,5) +
            "</div>";

        listItem += "</td>";


        listItem +=
            "<td class='align-middle'>" +
            "<span class='stakeholder'>" +
            item.stakeholder +
            "</span>" +
            "</td>";


        listItem +=
            "<td class='align-middle'>" +
            "<span class='TimeStamp'>" +
            TimeStamp +
            "</span>" +
            "</td>";


        if (item.stkStatusId === 1) {

            listItem +=
                "<td class='align-middle'>" +
                "<span class='status'>Accepted</span>" +
                "</td>";

            listItem +=
                "<td class='align-middle'>" +
                "<button type='button' " +
                "class='cls-btncomment btn-icon btn-round btn-success mr-1'>" +
                "<i class='fas fa-comment'></i>" +
                "</button>" +
                "</td>";
        }

        else if (item.stkStatusId === 5) {

            listItem +=
                "<td class='align-middle'>" +
                "<span class='status'>Info</span>" +
                "</td>";

            listItem +=
                "<td class='align-middle'>" +
                "<button type='button' " +
                "class='cls-btncomment btn-icon btn-round btn-success mr-1'>" +
                "<i class='fas fa-comment'></i>" +
                "</button>" +
                "</td>";
        }

        else if (item.stkStatusId === 2) {

            listItem +=
                "<td class='align-middle'>" +
                "<span class='status'>Obsn</span>" +
                "</td>";

            listItem +=
                "<td class='align-middle'>" +
                "<button type='button' " +
                "class='cls-btncomment btn-icon btn-round btn-warning mr-1'>" +
                "<i class='fas fa-comment'></i>" +
                "</button>" +
                "</td>";
        }

        else if (item.stkStatusId === 3) {

            listItem +=
                "<td class='align-middle'>" +
                "<span class='status'>Rejected</span>" +
                "</td>";

            listItem +=
                "<td class='align-middle'>" +
                "<button type='button' " +
                "class='cls-btncomment btn-icon btn-round btn-danger mr-1'>" +
                "<i class='fas fa-comment'></i>" +
                "</button>" +
                "</td>";
        }

        else if (item.stkStatusId === 6) {

            listItem +=
                "<td class='align-middle'>" +
                "<span class='status'>Not Applicable</span>" +
                "</td>";

            listItem +=
                "<td class='align-middle'>" +
                "<button type='button' " +
                "class='cls-btncomment btn-icon btn-round btn-secondary mr-1'>" +
                "<i class='fas fa-comment'></i>" +
                "</button>" +
                "</td>";
        }

        else {

            listItem +=
                "<td class='align-middle'>" +
                "<span class='status'>Pending</span>" +
                "</td>";

            listItem +=
                "<td class='align-middle'>" +
                "<button type='button' " +
                "class='cls-btncomment btn-icon btn-round btn-danger mr-1'>" +
                "<i class='fas fa-comment'></i>" +
                "</button>" +
                "</td>";
        }


        listItem += "</tr>";

        count++;
    }

    if ($.fn.DataTable.isDataTable("#Comment")) {
        $("#Comment").DataTable().clear().destroy();
    }
    $("#DetailBody").html(listItem);

    initializeDataTable("#Comment");

    const hasselectedtablerow = response.some(item => item.stkStatusId !== 1);
        
    const table = $("#Comment").DataTable();

    table.column("#notapplicableHeader").visible(hasselectedtablerow);

    IsReadComment(0, 0);

    bindProjectCommentEvents();
}

function bindProjectCommentEvents() {

    $("body")
        .off("click", ".cls-btncomment")
        .on("click", ".cls-btncomment", function () {

            $(".custom-modal")
                .addClass("custom-modal-size");

            const self = this;

            const action =
                $(self)
                    .closest("tr")
                    .find(".status")
                    .html();

            fetchServerDate().then(function (S) {

                let stkid = 0;

                switch (action) {

                    case "Accepted":
                        stkid = 1;
                        break;

                    case "Obsn":
                        stkid = 2;
                        break;

                    case "Rejected":
                        stkid = 3;
                        break;

                    case "Info":
                        stkid = 5;
                        break;

                    case "Not Applicable":
                        stkid = 6;
                        break;

                    default:
                        stkid = 0;
                        break;
                }


                const row =
                    $(self).closest("tr");


                $("#ProjectcommentForStackHolderprojId")
                    .html(
                        row.find(".spnProjId").html()
                    );


                $("#ProjectcommentForStackHolderPsmId")
                    .html(
                        row.find(".spnpsmId").html()
                    );


                $("#ProjectcommentForStackHolderDate_type")
                    .html(
                        row.find(".DateType").html()
                    );


                IsReadComment(
                    row.find(".spnProjId").html(),
                    row.find(".spnpsmId").html()
                );


                row.removeClass("bold-text");


                reset();

                mMsater(
                    0,
                    "ddlStatus",
                    4,
                    0
                );


                $("#ProjCommentModal")
                    .modal("show");


                GetAllComments(
                    $("#ProjectcommentForStackHolderPsmId").html(),
                    $("#ProjectcommentForStackHolderprojId").html()
                );


                const projName =
                    row.find(".projectName").html();


                $("#addComment")
                    .text(
                        "Project Name: " + projName
                    );


                const dateTypeText =
                    row.find(".DateType")
                        .text()
                        .trim()
                        .toLowerCase();


                const dateType =
                    dateTypeText === "true";


                $("#ProjectcommentForStackHolderDate_type")
                    .text(dateType);


                const formattedDateTime =
                    new Date(S.todayDateTime)
                        .toISOString()
                        .slice(0, 16);


                $("#CommentDateFwd")
                    .attr(
                        "type",
                        "datetime-local"
                    );


                if (dateType) {

                    $("#CommentDateFwd")
                        .attr(
                            "max",
                            formattedDateTime
                        )
                        .prop(
                            "disabled",
                            false
                        )
                        .val(
                            formattedDateTime
                        );

                } else {

                    $("#CommentDateFwd")
                        .prop(
                            "disabled",
                            true
                        )
                        .val(
                            S.todayDateTime
                        );
                }
            });
        });


    $("body")
        .off("click", ".projNameDetail")
        .on("click", ".projNameDetail", function () {

            const row =
                $(this).closest("tr");

            IsReadComment(
                row.find(".spnProjId").html(),
                row.find(".spnpsmId").html()
            );
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

    //alert($("#ProjectcommentForStackHolderprojId").text().trim());

    formData.append("Comments", encryptData($("#Comments").val()));
    formData.append("StkStatusId", encryptData($("#ddlStatus").val()));
    formData.append("ProjectId", encryptData($("#ProjectcommentForStackHolderprojId").text().trim()));
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

        success: function(response) {
            $('#uploadLoader').hide();

            try {
                
                console.log("SendCommentonProject response:", response);

                // -----------------------------------------
                // Normalize backend response
                // -----------------------------------------
                let status = null;
                let message = null;

                if (typeof response === "object" && response !== null) {
                    status = response.status;
                    message = response.message;
                }
                else {
                    status = response;
                }

                // In case status comes as string
                status = Number(status);


                // -----------------------------------------
                // SUCCESS
                // -----------------------------------------
                if (status === 1) {

                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: 'Comment Sent successfully',
                        showConfirmButton: false,
                        timer: 3000
                    }).then(() => {

                        if ($("#ddlStatus").val() == 1) {
                            FwdProjConfirm(
                                $("#ProjectcommentForStackHolderPsmId").html()
                            );
                        }

                        GetAllComments(
                            $("#ProjectcommentForStackHolderPsmId").html(),
                            $("#ProjectcommentForStackHolderprojId").html()
                        );

                        UnReadNotification(
                            $("#ProjectcommentForStackHolderprojId").html(),
                            2
                        );

                        IsUnReadComment(
                            $("#ProjectcommentForStackHolderprojId").html(),
                            $("#ProjectcommentForStackHolderPsmId").html()
                        );

                        // Refresh ONLY the currently opened tab
                        if (typeof currentCommentStatusId !== "undefined") {
                            GetProjCommentsByUnitId(currentCommentStatusId);
                        }

                        reset();
                    });

                    return;
                }


                // -----------------------------------------
                // NOT SAVED / ACTION NOT ALLOWED
                // -----------------------------------------
                if (status === 6) {

                    Swal.fire({
                        position: 'top-end',
                        icon: 'error',
                        title: 'Action Not Allowed',
                        text: message ||
                            "This project has already been accepted. Only Info comments are allowed."
                    });

                    return;
                }


                // -----------------------------------------
                // FILE TOO LARGE
                // -----------------------------------------
                if (status === 8) {

                    Swal.fire({
                        position: 'top-end',
                        icon: 'error',
                        title: 'File too large',
                        text: message ||
                            'PDF size must be less than 10 MB',
                        showConfirmButton: true
                    });

                    return;
                }


                // -----------------------------------------
                // INVALID DATE
                // -----------------------------------------
                if (status === 404) {

                    Swal.fire({
                        position: 'top-end',
                        icon: 'warning',
                        title: 'Invalid Comment Date',
                        text: message ||
                            'You cannot select a date before the processed date.',
                        showConfirmButton: true
                    });

                    return;
                }


                // -----------------------------------------
                // SESSION EXPIRED
                // -----------------------------------------
                if (status === 401 || status === -401) {

                    Swal.fire({
                        icon: 'warning',
                        title: 'Session Expired',
                        text: message ||
                            'Your session has expired. Please login again.'
                    }).then(() => {
                        window.location.reload();
                    });

                    return;
                }


                // -----------------------------------------
                // BAD REQUEST
                // -----------------------------------------
                if (status === -400) {

                    Swal.fire({
                        icon: 'error',
                        title: 'Invalid Data',
                        text: message ||
                            'Invalid data was submitted. Please check your input.'
                    });

                    return;
                }


                // -----------------------------------------
                // DECRYPTION / SECURITY ERROR
                // -----------------------------------------
                if (status === -500) {

                    Swal.fire({
                        icon: 'error',
                        title: 'Security Error',
                        text: message ||
                            'Unable to process the request. The submitted data may be invalid or tampered with.'
                    });

                    return;
                }


                // -----------------------------------------
                // GENERAL SERVER EXCEPTION
                // -----------------------------------------
                if (status === -1 || status === 500) {

                    Swal.fire({
                        icon: 'error',
                        title: 'Server Error',
                        text: message ||
                            'Something went wrong on the server. Please try again.'
                    });

                    return;
                }


                // -----------------------------------------
                // UNKNOWN RESPONSE
                // -----------------------------------------
                Swal.fire({
                    icon: 'error',
                    title: 'Unexpected Response',
                    text: message ||
                        'An unexpected response was received from the server.'
                });

            }
            catch (e) {

                console.error("UI response handling error:", e);

                Swal.fire({
                    icon: 'error',
                    title: 'UI Error',
                    text: 'Something went wrong while processing the server response.'
                });
            }
        },

        error: function(xhr, status, error) {

            $('#uploadLoader').hide();

            console.error("AJAX ERROR:", {
                httpStatus: xhr.status,
                status: status,
                error: error,
                responseText: xhr.responseText
            });

            let message = "Something went wrong. Please try again.";

            if (xhr.status === 0) {

                message = "Network error. Please check your internet connection.";

            }
            else if (xhr.status === 400) {

                message = "Bad request. Please check the submitted data.";

            }
            else if (xhr.status === 401) {

                Swal.fire({
                    icon: 'warning',
                    title: 'Session Expired',
                    text: 'Please login again.'
                }).then(() => {
                    window.location.reload();
                });

                return;
            }
            else if (xhr.status === 403) {

                message = "You are not authorized to perform this action.";

            }
            else if (xhr.status === 404) {

                message = "The requested API was not found.";

            }
            else if (xhr.status === 413) {

                message = "The uploaded file or request is too large.";

            }
            else if (xhr.status === 500) {

                message = "Server error. Please contact the administrator.";

            }
            else if (xhr.responseText) {

                try {

                    const response = JSON.parse(xhr.responseText);

                    message =
                        response.message ||
                        response.Message ||
                        message;

                }
                catch (e) {

                    console.warn(
                        "Could not parse server error response:",
                        e
                    );
                }
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

    let userdata = { "ProjId": ProjId, "PsmId": PsmId };
    let encrypted_payload = encryptData(userdata);
    $.ajax({
        url: '/Projects/IsReadComment',
        type: 'POST',
        data: { encrypted_payload: encrypted_payload },
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

            
        
$(document).on('change', '.rowCheck', function () {
    // uncheck "select all" if any row is unchecked
    if (!this.checked) {
        $('#selectAll').prop('checked', false);
    } else if ($('.rowCheck:checked').length === $('.rowCheck').length) {
        $('#selectAll').prop('checked', true);
    }
    updateSelectionState();
});

function updateSelectionState() {
    const count = $('.rowCheck:checked').length;
    $('#btnMarkNA').prop('disabled', count === 0);

    const $count = $('#selCount');
    if (count > 0) {
        $count.text(count).addClass('show');
    } else {
        $count.text('').removeClass('show');
    }
}

// --- Bulk "Mark as N/A" action ---

$(document).off("click", "#btnMarkNA")
    .on("click", "#btnMarkNA", function () {

        const items = $('.rowCheck:checked').map(function () {
            return {
                ProjectId: parseInt($(this).val()),
                PsmId: parseInt($(this).data('psmid'))
            };
        }).get();

        if (items.length === 0) return;
        if (!confirm(`Mark ${items.length} project(s) as Not Applicable?`)) return;

        // Keep a reference to the checked rows BEFORE disabling anything
        const $rowsToRemove = $('.rowCheck:checked').closest('tr');

        $('#btnMarkNA').prop('disabled', true).text('Submitting...');

        $.ajax({
            url: '/Projects/SendBulkNotApplicable',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(items),
            success: function (response) {
                // response is an array: [{ projectId, response: {...} }, ...]
                const blocked = [];
                const succeeded = [];

                response.forEach(function (item) {
                    const res = item.response;
                    // nmum.Save is presumably an object/value distinct from the 403 case
                    if (res && res.status === 6) {
                        blocked.push(item.projectId);
                    } else if (res === 0 || (res && res.status && res.status !== 200)) {
                        // treat as a generic failure if you want to separate 0/false results too
                        blocked.push(item.projectId);
                    } else {
                        succeeded.push(item.projectId);
                    }
                });

                // Remove only the rows that actually succeeded
                if (succeeded.length > 0) {
                    if ($.fn.DataTable && $.fn.DataTable.isDataTable('#Comment')) {
                        var table = $('#Comment').DataTable();
                        $rowsToRemove.each(function () {
                            var $row = $(this);
                            var rowProjId = parseInt($row.find('.rowCheck').val());
                            if (succeeded.includes(rowProjId)) {
                                table.row($row).remove();
                            }
                        });
                        table.draw(false);
                    } else {
                        $rowsToRemove.each(function () {
                            var $row = $(this);
                            var rowProjId = parseInt($row.find('.rowCheck').val());
                            if (succeeded.includes(rowProjId)) {
                                $row.remove();
                            }
                        });
                    }
                }

                // Show the blocked/error message if any project was rejected
                if (blocked.length > 0) {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'error',
                        title: '<div style="text-align: left;">' +
                            '<ol style="margin: 0; padding-left: 20px; text-align: left;">' +
                            '<li>No Amdts Allowed as the Project is Already Accepted By You!</li>' +
                            '<li>However, only info is allowed after the project is accepted.</li>' +
                            (blocked.length > 1 || succeeded.length > 0
                                ? `<li>Affected Proj ID(s): ${blocked.join(', ')}</li>`
                                : '') +
                            '</ol>' +
                            '</div>',
                        showConfirmButton: true,
                    });
                }

                $('#btnMarkNA').prop('disabled', true).text('Mark Selected as N/A');
                $('#selCount').text('');
                $('#selectAll').prop('checked', false);
            },
            error: function () {
                alert('Something went wrong while submitting.');
                $('#btnMarkNA').prop('disabled', false).text('Mark Selected as N/A');
            }
        });
    });

function finishBulkAction(failed) {
    $('#btnMarkNA').text('Mark Selected as N/A');
    if (failed.length > 0) {
        alert('Failed to update project(s): ' + failed.join(', '));
    } else {
        alert('Selected projects marked as Not Applicable.');
    }
    // Reload/refresh table data (DataTable, ajax reload, etc.)
    $('#Comment').DataTable().ajax.reload(); // if using DataTables server-side
    $('#selectAll').prop('checked', false);
    updateSelectionState();
}
