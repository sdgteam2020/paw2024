$(document).ready(function () {

    GetProjCommentsByUnitId();

    $("#StatusUpdate").click(function () {
        requiredFields = $('#StatusUpdateForm').find('.requiredField');
        var allFieldsComplete = true;
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
            SendMsg();
        }
    });

});
function GetProjCommentsByUnitId() {
    var listItem = "";
    $.ajax({
        url: '/Projects/GetProjCommentsByUnitId',
        type: 'POST',
        data: { "Id": 0 },
        success: function (response) {
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
                    // { attId: 8, psmId: 8, attPath: 'Swas_22ed1265-b2a0-4008-b7ff-b3eb5f704849.pdf', actionId: 0, timeStamp: '2024-05-02T16:17:45.6016413', … }
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='align-middle'><span id='divName'>" + count + "</span></td>";
                        listItem += "<td class='d-none'><span id='spnProjId'>" + response[i].projId + "</span><span id='spnpsmId'>" + response[i].psmId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='projectName'>" + response[i].projectName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='stakeholder'>" + response[i].stakeholder + "</span></td>";

                        if (response[i].stkStatusId != null && response[i].stkStatusId!=0) {
                            listItem += "<td class='align-middle'><span id='status'>Ok</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btncomment btn-icon btn-round btn-success mr-1'><i class='fas fa-comment'></i></button></td>";

                        }
                        else {
                            listItem += "<td class='align-middle'><span id='status'>Pending</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btncomment btn-icon btn-round btn-danger mr-1'><i class='fas fa-comment'></i></button></td>";
                        }


                        /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";
                        count++;

                    }

                    $("#DetailBody").html(listItem);
                    



                    var rows;





                    $("body").on("click", ".cls-btncomment", function () {
                        $("#ProjectcommentprojId").html($(this).closest("tr").find("#spnProjId").html());
                        $("#ProjectcommentPsmId").html($(this).closest("tr").find("#spnpsmId").html());
                        reset()
                        mMsater(0, "ddlStatus", 4, 0)
                        $("#ProjCommentModal").modal('show');
                        GetAllComments();
                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";
               
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
    var formData = new FormData();
    var totalFiles = document.getElementById("uploadfile").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("uploadfile").files[i];
        formData.append("uploadfile", file);
       
    }
    formData.append("Comments", $("#Comments").val());
    formData.append("StkStatusId", $("#ddlStatus").val());
    formData.append("ProjectId", $("#ProjectcommentprojId").html());
    formData.append("psmid", $("#ProjectcommentPsmId").html());
    $.ajax({
        type: "POST",
        url: '/Projects/SendCommentonProject',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response == 0) {

            }
            if (response == 1) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Comment Sent successfully',
                    showConfirmButton: false,
                    timer: 1500
                });

               
                GetAllComments($("#ProjectcommentprojId").html());
                GetProjCommentsByUnitId();
                if ($("#ddlStatus").val() == 1) {
                    FwdProjConfirm($("#ProjectcommentPsmId").html());
                }

                reset();
            }
            if (response == 6) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'Error',
                    title: 'comment Not sent allready Accepted !',
                    showConfirmButton: true,
                    
                });
            }
        },
        error: function (error) {
           

        }
    });
}
function GetAllComments()
{
    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            "PsmId": $("#ProjectcommentPsmId").html(),
            "stakeholderId": 1,
            "ProjId": $("#ProjectcommentprojId").html()
        },
        success: function (data) {

            var commentContainer = '';
            if (data != null) {

                for (var i = 0; i < data.length; i++) {
                    var date = new Date(data[i].date);
                    var formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

                    commentContainer += '<div class="comment-box" style="text-align: justify;">'; // Use text-align: justify for justified text
                    commentContainer += '<div class="comment-header">';
                    commentContainer += '<div>';
                    commentContainer += '<span style="font-family: Arial; font-weight: bold; color: #0793f7;">' + data[i].stakeholder + '</span>';
                    commentContainer += '<span style="margin-left: 10px;" class="comment-meta">' + formattedDate + '</span>';
                    commentContainer += '</div>';
                    commentContainer += '<div>';
                    if (data[i].status =="Accepted")
                        commentContainer += '<span class="comment-meta badge badge-success text-white">' + data[i].status + '</span>';
                    else
                        commentContainer += '<span class="comment-meta badge badge-danger text-white">' + data[i].status + '</span>';

                    commentContainer += '<span class="pdf-link">'; // Move the PDF link to the same line as status

                    if (data[i].state !== null) {

                        commentContainer += '<a href="/Home/WaterMark3?id=' + data[i].attpath + '" target="_blank">';
                        commentContainer += '&nbsp;&nbsp; &nbsp;&nbsp;<img src="/assets/images/icons/pdfimg.png" alt="PDF icon" style="width: 24px; height: 24px;">';
                        commentContainer += '</a>';
                    }


                    commentContainer += '</span>';
                    commentContainer += '</div>';
                    commentContainer += '</div>';
                    commentContainer += '<div class="comment-content">' + data[i].comments + '</div>';
                    commentContainer += '</div>';
                }

                commentContainer += '</div>'; // Close the container
                $('#ChatBox').empty().html(commentContainer);


            }

        },
        error: function () {
            alert('Error fetching comments.');
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