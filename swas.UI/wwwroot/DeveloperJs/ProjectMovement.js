$(document).ready(function () {
    mMsaterfwdStage(0, "ddlfwdStage", 5, 0)

    $("#ddlfwdStage").change(function () {
        mMsaterStage(0, "ddlfwdSubStage", 6, $("#ddlfwdStage").val(), 0)
    });
    //$("#ddlfwdSubStage").change(function () {

    //    mMsater(0, "ddlfwdAction", 7, $("#ddlfwdSubStage").val())
    //});

    $("#ddlfwdSubStage").change(function () {

        mMsater(0, "ddlfwdAction", 11, $("#ddlfwdSubStage").val())
    });

    $("#ddlfwdAction").change(function () {

        mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html(), 0, "");
    });

    //GetProjectMovement();
    $("#txtProjectName").autocomplete({
        source: function (request, response) {
            //alert("Hey");
            if (request.term.length > 1) {
                var projName = request.term;
                var param = { "ProjName": projName };
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
            GetProjectMovement(i.item.value);
        },
        appendTo: '#suggesstion-box'
    });

    $("#btnFwdNext").click(function () {
       
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

            AttechHistory();
            SaveFwdTo($("#spanEditPslmId").html());

        }
    });


    function SaveFwdTo(CurrentPslmId) {
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

        var userdata =
        {
            "ProjId": $("#spanProjectId").html(),
            "PsmId": $("#spanEditPslmId").html(),
            /* "StatusId": $("#ddlfwdSubStage").val(),*/
            "StatusActionsMappingId": $("#ddlfwdAction").val(),
            "Remarks": $("#txtRemarksfwd").val(),
            "ToUnitId": $("#ddlfwdFwdTo").val(),

            //"TimeStamp": $("#TimeStampToProjfwd").val()
            "TimeStamp": TimeStamps
        };
        //console.log("Fwd Data :", userdata)
        $.ajax({
            url: '/Projects/ProjectMovementUpdate',
            type: 'POST',
            data: userdata,
            success: function (response) {
                // console.log(response);
                if (response != null) {
                    /*$("#spanEditPslmId").html(response.psmId);*/
                    //FwdProjConfirm(CurrentPslmId);
                    $(".Fwdtitle").html("Projects Attch Details");
                    $(".ProjectsFwd").addClass("d-none");
                    $(".Attmenthistory").removeClass("d-none");

                }

            }
        });
    }

    $("#btnAttchMultiforpsmid").click(function () {

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
                    $('#uploadLoader').show();
                    setTimeout(function () {
                        UploadFiles();
                    }, 1000)
                   
                }
            });
        }
    });

    //$("#btnFwdConfirm").click(function () {

    //    $('#ProjFwdEdit').modal('hide');
    //    GetProjectMovement($("#spanProjectId").html());
    //});
});


function UploadFiles() {
    var formData = new FormData();
    var totalFiles = document.getElementById("pdfFileInput").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("pdfFileInput").files[i];
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

    var listItem = "";
    var userdata =
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

                    // { attId: 8, psmId: 8, attPath: 'Swas_22ed1265-b2a0-4008-b7ff-b3eb5f704849.pdf', actionId: 0, timeStamp: '2024-05-02T16:17:45.6016413', … }
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnattId'>" + response[i].attId + "</span><span id='spnpsmId'>" + response[i].psmId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";
                        listItem += "<td class='align-middle'><span id='comdName'>" + response[i].reamarks + "</span></td>";
                        listItem += "<td class='align-middle'><span id='corpsName'><a class='link-success' target='_blank' href=/uploads/" + response[i].attPath + ">" + response[i].actFileName + "</a></span></td>";
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].timeStamp + "</span></td>";

                        /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";
                    }

                    $("#DetailBody3").html(listItem);
                    $("#lblTotal").html(response.length);

                    var rows;

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
            //console.log(response);
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

    var listItem = "";

    $.ajax({
        url: '/Projects/GetProjectMov',
        type: 'Post',
        data: {
            "Id": ProjectId
        },
        success: function (response) {
            debugger;
            var projname = response[0].projName;
            //console.log("GetProjectMov: ", response);
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({
                        text: ""
                    });
                }
                else if (response == 0) {

                    listItem += "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }

                else {
                    var count = 1;
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnpsmId' class='d-none'>" + response[i].psmIds + "</span><span id='spneditstakeHolderId' class='d-none'>" + response[i].stakeHolderId + "</span>";
                        listItem += "<span id='spnStageId' class='d-none'>" + response[i].stageId + "</span>";
                        listItem += "<span id='spanProjId' class='d-none'>" + response[i].projId + "</span>";
                        listItem += "<span id='spnStatusId' class='d-none'>" + response[i].statusId + "</span>";
                        listItem += "<span id='spnActionId' class='d-none'>" + response[i].actionId + "</span>";
                        listItem += "<span id='spnToUnitId' class='d-none'>" + response[i].toUnitId + "</span>";

                        listItem += "</td>";
                        listItem += "<td>" + count + "</td>";
                        listItem += "<td class=''><span id='spnDate' class='d-none'>" + response[i].dateTimeOfUpdate + "</span><span id=''>" + DateFormateddMMyyyyhhmmss(response[i].dateTimeOfUpdate) + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].fromUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ToUnitName'>" + response[i].toUnitName + "</span></td>";

                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].stage + "</span></td>";
                        if (response[i].isComment == true) {

                            listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].stautsForComment + "</span></td>";
                        } else {

                            listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].status + "</span></td>";
                        }
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].action + "</span></td>";
                        if (response[i].isComment == true) {
                            listItem += "<td class='align-middle'><span id='spnremarks'>" + "--For Comments--" + "</span></td>";
                        } else {

                            listItem += "<td class='align-middle'><span id='spnremarks'>" + response[i].remarks + "</span></td>";
                        }


                        if (response[i].attCnt > 0) {
                            listItem += "<td><a href='javascript:void(0);' class='anchorDetail' data-id='" + response[i].psmIds + "'>" +
                                "<img src='/assets/images/icons/attachemnts_clip.png' alt='Icon' style='width: 31px; height: 29px; margin-right: 0px;'>" +
                                "</a></td>";
                        } else {
                            listItem += "<td></td>"; // Add an empty cell when there's no attachment
                        }

                        if (response[i].isComment == true) {

                            // ✅ 1. Correct HTML generation
                            listItem += "<td class='align-middle'>" +
                                "<span id='ToUnitName'>" +
                                "<button class='btn btn-primary cls-editCmt' data-psmid='" + response[i].psmIds + "'>" +
                                "<i class='fas fa-edit'></i> EditCmt" +
                                "</button>" +
                                "</span></td>";



                        } else {

                            listItem += "<td class='align-middle'><span id='FromUnitName'><span class='btn btn-primary cls-btnedit'>Edit</span></td>";
                        }


                        listItem += "</tr>";
                        count++;
                    }


                    $("#ProjectMovement").html(listItem);

                    var table = $('#moventdata').DataTable({
                        lengthChange: true,
                        retrieve: true,
                        Destroy: true,

                        searching: true,
                        stateSave: true,
                        "order": [[10, "asc"]],
                        "ordering": true,
                        "paging": true,
                        dom: 'lBfrtip',
                        //buttons: [
                        //    'copy',
                        //    'excel',
                        //    'csv',

                        //],
                        buttons: [
                            {
                                extend: 'copy',
                                exportOptions: {
                                    columns: ':not(:first-child):not(:nth-last-child(2))' // Excludes "Ser No" (first column) and "#Att" (second last column)
                                }
                            },
                            {
                                extend: 'excel',
                                exportOptions: {
                                    columns: ':not(:first-child):not(:nth-last-child(2))' // Excludes "Ser No" and "#Att"
                                }
                            },
                            {
                                extend: 'csv',
                                exportOptions: {
                                    columns: ':not(:first-child):not(:nth-last-child(2))' // Excludes "Ser No" and "#Att"
                                }
                            }
                        ],
                        searchBuilder: {
                            conditions: {
                                num: {
                                    'MultipleOf': {
                                        conditionName: 'Multiple Of',
                                        init: function (that, fn, preDefined = null) {
                                            var el = $('<input/>').on('input', function () { fn(that, this) });

                                            if (preDefined !== null) {
                                                $(el).val(preDefined[0]);
                                            }

                                            return el;
                                        },
                                        inputValue: function (el) {
                                            return $(el[0]).val();
                                        },
                                        isInputValid: function (el, that) {
                                            return $(el[0]).val().length !== 0;
                                        },
                                        search: function (value, comparison) {
                                            return value % comparison === 0;
                                        }
                                    }
                                }
                            }
                        }
                    });


                    $("body").unbind().on("click", ".cls-btnedit", function () {
                        $('#ProjFwdEdit').modal('show');

                        var date = $(this).closest("tr").find("#spnDate").html()
                        var currentTime = date.slice(0, 19);
                        //alert(currentTime);
                      /*  TimeStamps = dateValue.replace('T', ' ');*/
                        $(".ProjectsFwd").removeClass("d-none");
                        $(".Attmenthistory").addClass("d-none");
                        //alert($(this).closest("tr").find("#spnDate").html());
                        $("#spanProjectId").html($(this).closest("tr").find("#spanProjId").html());
                        $("#spanEditPslmId").html($(this).closest("tr").find("#spnpsmId").html());
                        $("#txtRemarksfwd").val($(this).closest("tr").find("#spnremarks").html());
                        $("#TimeStampToProjfwd").val(currentTime);
                        $("#SpnFwdStakeHolderId").html($(this).closest("tr").find("#spneditstakeHolderId").html());
                        //$("#ddlfwdFwdTo").html($(this).closest("tr").find("#ToUnitName").html());

                        mMsaterfwdStage($(this).closest("tr").find("#spnStageId").html(), "ddlfwdStage", 5, 0, 1)
                        mMsaterStage($(this).closest("tr").find("#spnStatusId").html(), "ddlfwdSubStage", 6, $(this).closest("tr").find("#spnStageId").html(), 0)
                        /*mMsater($(this).closest("tr").find("#spnActionId").html(), "ddlfwdAction", 7, $(this).closest("tr").find("#spnStatusId").html())*/
                        mMsater($(this).closest("tr").find("#spnActionId").html(), "ddlfwdAction", 11, $(this).closest("tr").find("#spnStatusId").html())
                        //mMsaterFwdTo($(this).closest("tr").find("#spnToUnitId").html(), "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html(), $(this).closest("tr").find("#spnToUnitId").html(), 0, "");
                        mMsaterFwdTo($(this).closest("tr").find("#spnToUnitId").html(), "ddlfwdFwdTo", 8, 0, $(this).closest("tr").find("#spnToUnitId").html(), 0, "edit");
                    });

                 
                    $(document).on("click", ".cls-editCmt", function (e) {
                        e.preventDefault(); // Prevent default behavior (replaces return false)

                        debugger;

                        var psmid = $(this).data("psmid");


                        mMsater(0, "ddlStatus", 4, 0);


                        $('#EditComments').modal('show');
                        var words = projname.split(" ");
                        // Limit to 6 words and add "..." if needed
                        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projname;
                        var finalTitle = "Edit Comments for: " + shortProjName;

                        $('#Projname').text(finalTitle);



                        GetAllCommentsForEdit(psmid, 0);



                    });
                }
            }
        }
    }
    );

}


//$(document).ready(function () {
//    alert("hello from ProjectMovementjs");

//    var TeamDetailPostBackURL = '/Projects/AttDetails';
//    $(function () {
//        $(".anchorDetail").click(function () {

//            var $buttonClicked = $(this);
//            var id = $buttonClicked.attr('data-id');
//            var options = { "backdrop": "static", keyboard: true };
//            $.ajax({
//                type: "GET",
//                url: TeamDetailPostBackURL,
//                contentType: "application/json; charset=utf-8",
//                data: { "Id": id },
//                datatype: "json",
//                success: function (datadata) {

//                    $('#myModalPagehistoryAttechment').modal('show');
//                    $('#myModalContenthistoryAttechment').html(datadata);
//                    /* $('#myModal').modal(options);*/


//                },
//                error: function () {
//                    alert("Dynamic content load failed.");
//                }
//            });

//        });

//    });

//    $(document).on('click', '.pdf', function () {
//        $('#ViewRecordsHistory').modal('show');
//    });
//});

$(document).ready(function () {
    /*  alert("hello from ProjectMovementjs");*/

    var TeamDetailPostBackURL = '/Projects/AttDetails';
    $(document).on('click', '.anchorDetail', function () {
        var $buttonClicked = $(this);
        var id = $buttonClicked.attr('data-id');

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


var TeamDetailPostBackURL = '/Projects/AttDetails';
$(document).on('click', '.anchorDetail', function () {


    var $buttonClicked = $(this);
    var id = $buttonClicked.attr('data-id');
    //console.log('PsmId:', id); // Check if id is correct

    if (!id) {
        alert("No PsmId found.");
        return;
    }

    //var $buttonClicked = $(this);
    //var id = $buttonClicked.attr('data-id');
    var options = { "backdrop": "static", keyboard: true };
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
                    commentContainer += '<button class="btn btn-warning editComments" data-stkcommentid="' + data[i].stkCommentId + '">' +
                        '<i class="fas fa-edit"></i> Edit' +
                        '</button>';

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



$(document).on("click", ".editComments", function () {
    var stkcommentid = $(this).data("stkcommentid"); // get the id from button

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

    var stkcomment = {
        comments: $("#edtCmts").val(),
        ddlstatus: $("#ddlStatus").val(),
        CommentDateFwd: $("#CommentDateFwd").val(),
        stkcommentid: $("#StkcommentId").val()
    };

    $("#edtCmts, #ddlStatus, #CommentDateFwd, #StkcommentId").removeClass("is-invalid");

    let isValid = true;

    // Validate each field
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

    // Stop further action if invalid
    if (!isValid) {
        return false;
    }


    $.ajax({
        url: '/Projects/UpdateStkcomments',
        type: 'POST',
        data: stkcomment,
        success: function (response) {

            // Check if update was successful
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


