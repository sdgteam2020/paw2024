var memberTable = "";

$(document).ready(function () {

    GetProjCommentsByUnitId();
     
    $("#btnStatusUpdate").unbind().click(function () {
       
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

function IsUnReadInbox(psmId) {

    $.ajax({
        url: '/Projects/IsUnReadInbox',
        type: 'POST',
        data: { "PsmId": psmId },
        success: function (response) {
            console.log(response);

        }
    });
}
function GetProjCommentsByUnitId() {
    var listItem = "";
     
    /*let boldCount = 0;*/
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
                    console.log(response);
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
                            listItem += "<tr class='font-weight-bold'>";
                           /* boldCount++;*/
                        } else {
                            listItem += "<tr>";
                        }
                        listItem += "<td class='noExport d-none'><span class='noExport d-none' id='spnProjId'>" + response[i].projId + "</span><span class='noExport d-none' id='spnpsmId'>" + response[i].psmId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='divName'>" + count + "</span></td>";

                        listItem += "<td class='align-middle'>";
                        listItem += "<a  href='/Projects/ProjHistory?EncyID=" + encodeURIComponent(response[i].encyID) + "'>";
                        listItem += "<span id='projectName' class='projNameDetail' >" + response[i].projectName + "</span>";
                        listItem += "</a>";
                        listItem += "</td>";
                        listItem += "<td class='align-middle'><span id='stakeholder'>" + response[i].stakeholder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='TimeStamp'>" + TimeStamp + "</span></td>";
                        if (response[i].stkStatusId == 1) {
                            listItem += "<td class='align-middle'><span id='status'>Accepted</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btncomment btn-icon btn-round btn-success mr-1'><i class='fas fa-comment'></i></button></td>";

                        }
                        else if (response[i].stkStatusId == 2) {
                            listItem += "<td class='align-middle'><span id='status'>Obsn</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btncomment btn-icon btn-round  btn-warning mr-1'><i class='fas fa-comment'></i></button></td>";

                        }
                        else if (response[i].stkStatusId == 3) {
                            listItem += "<td class='align-middle'><span id='status'>Rejected</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btncomment btn-icon btn-round btn-danger mr-1'><i class='fas fa-comment'></i></button></td>";
                        }
                        else {
                            listItem += "<td class='align-middle'><span id='status'>Pending</span></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btncomment btn-icon btn-round btn-danger mr-1'><i class='fas fa-comment'></i></button></td>";
                        }
                        listItem += "</tr>";
                        count++;

                    }

                    $("#DetailBody").html(listItem);
                    
                   /* $('#ProjectCommentCount').html(boldCount);*/
                    
                    $("body").on("click", ".cls-btncomment", function () {
                       
                        $("#ProjectcommentprojId").html($(this).closest("tr").find("#spnProjId").html());
                        $("#ProjectcommentPsmId").html($(this).closest("tr").find("#spnpsmId").html());

                        
                        //08/08/24
                        //var $row = $(this).closest("tr");
                        //var psmId = $row.find("#spnpsmId").html().trim();
                        //globalPsmId = psmId;
                        reset()
                        mMsater(0, "ddlStatus", 4, 0)
                        $("#ProjCommentModal").modal('show');
                        GetAllComments();
                    });


                    $("body").on("click", ".projNameDetail", function () {
                        IsReadInbox($(this).closest("tr").find("#spnpsmId").html());
                        IsReadNotification($(this).closest("tr").find("#spnProjId").html());
                      /*  IsReadInbox($(this).closest("tr").find("#spnpsmId").html());*/
                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";
               
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
    formData.append("CommentDate", $("#CommentDateFwd").val());
    
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

              
                if ($("#ddlStatus").val() == 1) {
                    FwdProjConfirm($("#ProjectcommentPsmId").html());
                }
                

                GetAllComments($("#ProjectcommentprojId").html());
                GetProjCommentsByUnitId();
              //IsUnReadComment($("#ProjectcommentprojId").html());
                //GetNotification($("#ProjectcommentprojId").html());

                reset();
            }
            if (response == 6) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'Error',
                    title: 'No Amdts Allowed as the Project is Already Accepted By You !',
                    showConfirmButton: true,
                    
                });
            }

            IsUnReadComment($("#ProjectcommentprojId").html());
            GetNotification($("#ProjectcommentprojId").html());
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
                    var formattedDate =
                        ("0" + date.getDate()).slice(-2) + '-' +
                        ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                        date.getFullYear() + ' ' +
                        ("0" + date.getHours()).slice(-2) + ':' +
                        ("0" + date.getMinutes()).slice(-2) + ':' +
                        ("0" + date.getSeconds()).slice(-2); 

                    commentContainer += '<div class="comment-box" style="text-align: justify;">'; // Use text-align: justify for justified text
                    commentContainer += '<div class="comment-header">';
                    commentContainer += '<div>';
                    commentContainer += '<span style="font-family: Arial; font-weight: bold; color: #0793f7;">' + data[i].stakeholder + '</span>';
                    commentContainer += '<span style="margin-left: 10px;" class="comment-meta">' + formattedDate + '</span>';
                    commentContainer += '</div>';
                    commentContainer += '<div>';
                    if (data[i].status == "Accepted")
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
                $('#ChatBox').empty().html(commentContainer);

                //for (var i = 0; i < data.length; i++) {
                //    var date = new Date(data[i].date);
                //    var formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

                //    commentContainer += '<div class="comment-box" style="text-align: justify;">'; // Use text-align: justify for justified text
                //    commentContainer += '<div class="comment-header">';
                //    commentContainer += '<div>';
                //    commentContainer += '<span style="font-family: Arial; font-weight: bold; color: #0793f7;">' + data[i].stakeholder + '</span>';
                //    commentContainer += '<span style="margin-left: 10px;" class="comment-meta">' + formattedDate + '</span>';
                //    commentContainer += '</div>';
                //    commentContainer += '<div>';
                //    if (data[i].status =="Accepted")
                //        commentContainer += '<span class="comment-meta badge badge-success text-white">' + data[i].status + '</span>';
                //    else
                //        commentContainer += '<span class="comment-meta badge badge-danger text-white">' + data[i].status + '</span>';

                //    commentContainer += '<span class="pdf-link">'; // Move the PDF link to the same line as status

                //    if (data[i].state !== null) {

                //        commentContainer += '<a href="/Home/WaterMark3?id=' + data[i].attpath + '" target="_blank">';
                //        commentContainer += '&nbsp;&nbsp; &nbsp;&nbsp;<img src="/assets/images/icons/pdfimg.png" alt="PDF icon" style="width: 24px; height: 24px;">';
                //        commentContainer += '</a>';
                //    }


                //    commentContainer += '</span>';
                //    commentContainer += '</div>';
                //    commentContainer += '</div>';
                //    commentContainer += '<div class="comment-content">' + data[i].comments + '</div>';
                //    commentContainer += '</div>';
                //}

                //commentContainer += '</div>'; // Close the container
                //$('#ChatBox').empty().html(commentContainer);


            }

        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}



function IsReadComment(ProjId) {
    $.ajax({
        url: '/Projects/IsReadComment',
        type: 'POST',
        data: { "ProjId": ProjId },
        success: function (response) {
            console.log(response);
        }
    })
}

function GetNotification(ProjId) {
 
    $.ajax({
        url: '/Home/GetNotification',
        type: 'POST',
        data: { "ProjId": ProjId },
        success: function (response) {

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

function IsUnReadComment(ProjId) {
    $.ajax({
        url:'/Projects/IsUnReadComment',
        type: 'POST',
        data: { "ProjId": ProjId },
        success: function (response) {
            console.log(response);
            window.location.reload();
        }
    })
}

function IsReadInbox(psmId) {

    $.ajax({
        url: '/Projects/IsReadInbox',
        type: 'POST',
        data: { "PsmId": psmId },
        success: function (response) {
            console.log(response);

        }
    });
}



function IsReadNotification(ProjId) {

    $.ajax({
        url: '/Projects/IsReadNotification',
        type: 'POST',
        data: { "ProjId": ProjId },
        success: function (response) {
            console.log(response);

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