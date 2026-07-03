$(document).ready(function () {
    mMsaterfwdStage(0, "ddlfwdStage", 5, 0)

    $("#ddlfwdStage").change(function () {
        mMsaterStage(0, "ddlfwdSubStage", 6, $("#ddlfwdStage").val(), 0)
    });

    $("#ddlfwdSubStage").change(function () {

        mMsater(0, "ddlfwdAction", 11, $("#ddlfwdSubStage").val())
    });

    $("#ddlfwdAction").change(function () {

        mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html(), 0, "");
    });
    $("#txtProjectName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 1) {
                let projName = request.term;
                let param = { "ProjName": projName };
                $.ajax({
                    url: '/Projects/GetALLByProjectName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {

                                return { label: item.name, value: item.id };

                            }))
                        }
                        else {

                            $("#txtProjectName").val("");
                            alert("Project not found.")
                        }
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            $("#txtProjectName").val(i.item.label);
            $("#btnAuditlog").attr("data-projid", i.item.value);
            GetProjectMovement(i.item.value);
        },
        appendTo: '#suggesstion-box'
    });

    $("#btnFwdNext").click(function () {
       
         let requiredFields = $('#ProjFwd').find('.requiredField');
        let  allFieldsComplete = true;
        requiredFields.each(function (index) {
            if (this.value.length == 0) {
                $(this).addClass('is-invalid');
                allFieldsComplete = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        if (allFieldsComplete) {

            AttechHistory();
            SaveFwdTo($("#spanEditPslmId").html());

        }
    });


    function SaveFwdTo(CurrentPslmId) {
        let dateValue = $("#TimeStampToProjfwd").val();
        let currentDate = new Date();
        let TimeStamps = '';
        if ($('#TimeStampToProjfwd').attr('type') === 'date') {
            if (!dateValue) {
                alert('Please select a date .');
                return;
            }
            let currentTime = currentDate.toTimeString().split(' ')[0]; // Get current time in HH:mm:ss
            TimeStamps = dateValue + ' ' + currentTime;
        } else if ($('#TimeStampToProjfwd').attr('type') === 'datetime-local') {
            if (!dateValue) {
                alert('Please select date and time.');
                return;
            }
            TimeStamps = dateValue.replace('T', ' '); // Format datetime-local to space-separated
        }

        let userdata =
        {
            "ProjId": $("#spanProjectId").html(),
            "PsmId": $("#spanEditPslmId").html(),
            
            "StatusActionsMappingId": $("#ddlfwdAction").val(),
            "Remarks": $("#txtRemarksfwd").val(),
            "ToUnitId": $("#ddlfwdFwdTo").val(),
            "TimeStamp": TimeStamps
        };
        $.ajax({
            url: '/Projects/ProjectMovementUpdate',
            type: 'POST',
            data: userdata,
            success: function (response) {
                if (response != null) {
                    $(".Fwdtitle").html("Projects Attch Details");
                    $(".ProjectsFwd").addClass("d-none");
                    $(".Attmenthistory").removeClass("d-none");

                }

            }
        });
    }

    $("#btnAttchMultiforpsmid").click(function () {
     
        requiredFields = $('#ProjFwd').find('.requiredFieldAttch');
        let allFieldsComplete = true;
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
                    $('#uploadLoader').show();
                    setTimeout(function () {
                        UploadFiles();
                    }, 1000)
                   
                }
            });
        }
    });

    $("#btnFwdConfirm").click(function () {

        $('#ProjFwdEdit').modal('hide');
        GetProjectMovement($("#spanProjectId").html());
    });
});


function UploadFiles() {

    
    let formData = new FormData();
    let totalFiles = document.getElementById("pdfFileInput").files.length;
    for (let i = 0; i < totalFiles; i++) {
        let file = document.getElementById("pdfFileInput").files[i];
        formData.append("uploadfile", file);
        formData.append("Reamarks", $("#Reamarks").val());
        formData.append("PsmId", $("#spanEditPslmId").html());

    }

    $.ajax({
        type: "POST",
        url: '/Projects/UploadMultiFile',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $('#uploadLoader').hide();
            if (response == 1) {
                AttechHistory();
                $("#Reamarks").val("");
                $("#pdfFileInput").val("");
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Upload success",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
            else if (response == -2) {

                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Only Pdf File Upload!",
                });
            }
        },
        error: function (error) {
            $('#uploadLoader').hide();
            $(".error-msg").removeClass("d-none")
            $("#error-msg").html("Somthing is wrong");

        }
    });
}

function AttechHistory() {

    let listItem = "";
    let userdata =
    {
        "PslmId": $("#spanEditPslmId").html(),

    };
    $.ajax({
        url: '/Projects/GetAtthHistoryByProjectId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({
                        text: ""
                    });
                }
                else if (response == 0) {
                    listItem += "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";
                    $("#DetailBody3").html(listItem);
                    $("#lblTotal").html(0);
                }

                else {
                    for (let i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnattId'>" + response[i].attId + "</span><span id='spnpsmId'>" + response[i].psmId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";
                        listItem += "<td class='align-middle'><span id='comdName'>" + response[i].reamarks + "</span></td>";
                        listItem += "<td class='align-middle'><span id='corpsName'><a class='link-success' target='_blank' href=/uploads/" + response[i].attPath + ">" + response[i].actFileName + "</a></span></td>";
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].timeStamp + "</span></td>";

                        
                        listItem += "</tr>";
                    }

                    $("#DetailBody3").html(listItem);
                    $("#lblTotal").html(response.length);

                    let rows;

                    $("body").on("click", ".cls-btnDelete", function () {

                        Swal.fire({
                            title: 'Are you sure?',
                            text: "You want to Delete ",
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonColor: '#072697',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Yes, Delete It!'
                        }).then((result) => {
                            if (result.value) {

                                Deleteattechment($(this).closest("tr").find("#spnattId").html());
                            }
                        });
                    });
                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";
                $("#SoftwareTypes").DataTable().destroy();
                $("#DetailBody3").html(listItem);
                $("#lblTotal").html(0);
            }
        },
        error: function (result) {
            Swal.fire({
                text: ""
            });
        }
    });
}

function Deleteattechment(AttechId) {
    $.ajax({
        url: '/Projects/DeleteAttech',
        type: 'POST',
        data: { "AttechId": AttechId },
        success: function (response) {
            if (response == 1) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Record Deleted successfully',
                    showConfirmButton: false,
                    timer: 1500
                });
                AttechHistory();
            }
        }
    });
}
function GetProjectMovement(ProjectId) {
    debugger;
    $.ajax({
        url: '/Projects/GetProjectMov',
        type: 'POST',
        data: { "Id": ProjectId },
        success: function (response) {

            if (!isValidResponse(response)) return;

            if (response == -1) {
                Swal.fire({ text: "" });
                return;
            }

            if (response == 0) {
                renderNoRecord();
                return;
            }
           
            renderProjectMovement(response);
            initializeMovementTable();
            bindMovementEvents(response[0].projName);
        }
    });
}
function isValidResponse(response) {
    $("#btnAuditlog").addClass('d-none');
    return response !== "null" && response !== null;
}
function renderNoRecord() {
    $("#btnAuditlog").addClass('d-none');
    $("#DetailBody").html("<tr><td class='text-center' colspan=5>No Record Found</td></tr>");
    $("#lblTotal").html(0);
}

function renderProjectMovement(response) {
    $("#btnAuditlog").removeClass('d-none');
    let count = 1;

    const html = response.map(item => {
        const row = buildRow(item, count);
        count++;
        return row;
    }).join('');

    $("#ProjectMovement").html(html);
}

function buildRow(item, count) {

    return `
    <tr>
        <td class='d-none'>
            <span id='spnpsmId' class='d-none'>${item.psmIds}</span>
            <span id='spneditstakeHolderId' class='d-none'>${item.stakeHolderId}</span>
            <span id='spnStageId' class='d-none'>${item.stageId}</span>
            <span id='spanProjId' class='d-none'>${item.projId}</span>
            <span id='spnStatusId' class='d-none'>${item.statusId}</span>
            <span id='spnActionId' class='d-none'>${item.actionId}</span>
            <span id='spnToUnitId' class='d-none'>${item.toUnitId}</span>
        </td>

        <td>${count}</td>

        <td>
            <span id='spnDate' class='d-none'>${item.dateTimeOfUpdate}</span>
            ${DateFormateddMMyyyyhhmmss(item.dateTimeOfUpdate)}
        </td>

        <td>${item.fromUnitName}</td>
        <td>${item.toUnitName}</td>
        <td>${item.stage}</td>

        <td>${item.isComment ? item.stautsForComment : item.status}</td>

        <td>${item.action}</td>

        <td>
            <span id='spnremarks'>
                ${item.isComment ? "--For Comments--" : item.remarks}
            </span>
        </td>

        <td>
            ${item.attCnt > 0 ? `
                <a href='javascript:void(0);' class='anchorDetail' data-id='${item.psmIds}'>
                    <img src='/assets/images/icons/attachemnts_clip.png' class='attach-clip-icon'>
                </a>` : ''}
        </td>

        <td>
            ${item.isComment
            ? `<button class='btn btn-primary cls-editCmt' data-psmid='${item.psmIds}'>
                        <i class='fas fa-edit'></i> EditCmt
                   </button>`
            : `<span class='btn btn-primary cls-btnedit'>Edit</span>`
        }
        </td>
    </tr>`;
}

function initializeMovementTable() {
    $('#moventdata').DataTable({
        lengthChange: true,
        retrieve: true,
        destroy: true,
        searching: true,
        stateSave: true,
        order: [[10, "asc"]],
        paging: true,
        dom: 'lBfrtip',
        buttons: ['copy', 'excel', 'csv']
    });
}

function bindMovementEvents(projname) {

    // REMOVE duplicate binding issue
    $("body").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {

        $('#ProjFwdEdit').modal('show');

        let row = $(this).closest("tr");

        let date = row.find("#spnDate").html();
        let currentTime = date.slice(0, 19);

        $(".ProjectsFwd").removeClass("d-none");
        $(".Attmenthistory").addClass("d-none");

        $("#spanProjectId").html(row.find("#spanProjId").html());
        $("#spanEditPslmId").html(row.find("#spnpsmId").html());
        $("#txtRemarksfwd").val(row.find("#spnremarks").html());
        $("#TimeStampToProjfwd").val(currentTime);
        $("#SpnFwdStakeHolderId").html(row.find("#spneditstakeHolderId").html());

        mMsaterfwdStage(row.find("#spnStageId").html(), "ddlfwdStage", 5, 0, 1);
        mMsaterStage(row.find("#spnStatusId").html(), "ddlfwdSubStage", 6, row.find("#spnStageId").html(), 0);
        mMsater(row.find("#spnActionId").html(), "ddlfwdAction", 11, row.find("#spnStatusId").html());
        mMsaterFwdTo(row.find("#spnToUnitId").html(), "ddlfwdFwdTo", 8, 0, row.find("#spnToUnitId").html(), 0, "edit");
    });

    $(document).off("click", ".cls-editCmt").on("click", ".cls-editCmt", function (e) {

        e.preventDefault();

        let psmid = $(this).data("psmid");

        mMsater(0, "ddlStatus", 4, 0);

        $('#EditComments').modal('show');

        let words = projname.split(" ");
        let shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projname;

        $('#Projname').text("Edit Comments for: " + shortProjName);

        GetAllCommentsForEdit(psmid, 0);
    });
}

$(document).ready(function () {
    

    let TeamDetailPostBackURL = '/Projects/AttDetails';
    $(document).on('click', '.anchorDetail', function () {
        let $buttonClicked = $(this);
        let id = $buttonClicked.attr('data-id');

        if (!id) {
            alert("No PsmId found.");
            return;
        }

        $.ajax({
            type: "GET",
            url: TeamDetailPostBackURL,
            contentType: "application/json; charset=utf-8",
            data: { "Id": id },
            datatype: "json",
            success: function (data) {
                $('#myModalContenthistoryAttechment').html(data);
                $('#myModalPagehistoryAttechment').modal('show');
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });

    $(document).on('click', '.pdf', function () {
        $('#ViewRecordsHistory').modal('show');
    });
});


let TeamDetailPostBackURL = '/Projects/AttDetails';
$(document).on('click', '.anchorDetail', function () {


    let $buttonClicked = $(this);
    let id = $buttonClicked.attr('data-id');

    if (!id) {
        alert("No PsmId found.");
        return;
    }
    let options = { "backdrop": "static", keyboard: true };
    $.ajax({
        type: "GET",
        url: TeamDetailPostBackURL,
        contentType: "application/json; charset=utf-8",
        data: { "Id": id },
        datatype: "json",
        success: function (data) {
            $('#myModalContent').html(data);
            $('#myModal').modal(options);
            $('#myModal').modal('show');
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
});




function GetAllCommentsForEdit(PsmId, projId) {
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

                    if (data[i].userDetails == null)
                        userDetails = '';
                    else
                        userDetails = data[i].userDetails;

                    commentContainer += '<div class="comment-box">';
                    commentContainer += '<div class="comment-header">';
                    commentContainer += '<div>';

                    commentContainer += '<span class="comment-user">' +
                        data[i].stakeholder + ' (' + userDetails + ')' +
                        '</span>';

                    commentContainer += '<div class="comment-meta">' +
                        DateFormateddMMyyyyhhmmss(data[i].date) +
                        '</div>';

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

                    commentContainer += '</div>';
                    commentContainer += '</div>';

                    commentContainer += '<div class="comment-content formated-text">' +
                        '<p>' + data[i].comments + '</p>' +
                        '</div>';

                    commentContainer += '<button class="btn btn-warning editComments" data-stkcommentid="' +
                        data[i].stkCommentId + '">' +
                        '<i class="fas fa-edit"></i> Edit' +
                        '</button>';

                    commentContainer += '</div>';
                }

                $('#ChatBoxForStackholdercomment').empty().html(commentContainer);
            }
        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}




$(document).on("click", ".editComments", function () {
    let stkcommentid = $(this).data("stkcommentid"); // get the id from button

    $.ajax({
        url: '/Projects/GetStkCommentBystkId',
        type: 'Get',
        data: { PsmId: stkcommentid }, // send your id to server
        success: function (response) {

            $("#edtCmts").val(response.comments);

            $("#ddlStatus").val(response.stkStatusId);

            $("#CommentDateFwd").val(response.date ? response.date : '');
            $("#StkcommentId").val(response.stkCommentId)
            $("#StkPsmid").val(response.psmId)
        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
        }
    });
});
$(document).on("click", "#btnCommentUpdate", function () {

    let stkcomment = {
        comments: $("#edtCmts").val(),
        ddlstatus: $("#ddlStatus").val(),
        CommentDateFwd: $("#CommentDateFwd").val(),
        stkcommentid: $("#StkcommentId").val()
    };

    $("#edtCmts, #ddlStatus, #CommentDateFwd, #StkcommentId").removeClass("is-invalid");

    let isValid = true;
    if (!stkcomment.comments) {
        $("#edtCmts").addClass("is-invalid");
        isValid = false;
    }
    if (!stkcomment.ddlstatus) {
        $("#ddlStatus").addClass("is-invalid");
        isValid = false;
    }
    if (!stkcomment.CommentDateFwd) {
        $("#CommentDateFwd").addClass("is-invalid");
        isValid = false;
    }
    if (!stkcomment.stkcommentid) {
        $("#StkcommentId").addClass("is-invalid");
        alert("Please Click on Edit Button")
        isValid = false;
    }
    if (!isValid) {
        return false;
    }


    $.ajax({
        url: '/Projects/UpdateStkcomments',
        type: 'POST',
        data: stkcomment,
        success: function (response) {
            if (response === 1) {
                Swal.fire({
                    icon: 'success',
                    title: 'Updated!',
                    text: 'Comment updated successfully.',
                    timer: 2000,
                    showConfirmButton: false
                });


                $("#edtCmts").val('');
                $("#ddlStatus").val('');
                $("#CommentDateFwd").val('');
                GetAllCommentsForEdit($("#StkPsmid").val());

            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Failed!',
                    text: 'Something went wrong while updating.',
                });
            }
        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Server error occurred.',
            });
        }
    });

});

$('#EditComments').on('hidden.bs.modal', function () {
    $("#edtCmts").val('');
    $("#ddlStatus").val('');
    $("#CommentDateFwd").val('');
});


