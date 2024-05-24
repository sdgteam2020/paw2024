$(document).ready(function () {

    $(".processDetail").click(function () {

       
        var ProjId = $(this).closest("tr").find("#SpnCurrentProjId").html();
        var psmId = $(this).closest("tr").find("#SpnCurrentpsmId").html();
       
      //  GetForCommentStakeHolder(ProjId, psmId);
        SentForComment(ProjId, psmId, 0)
        ProcessProjConfirm(ProjId)
    });
});
function GetForCommentStakeHolder(ProjId, psmId) {

    $.ajax({
        url: '/UnitDtls/GetAllStakeHolderComment',
        type: 'POST',
        data: { "Id": 0 },
        success: function (response) {
            console.log(response);


            if (response != null) {

                for (var i = 0; i < response.length; i++) {
                    SentForComment(ProjId, psmId, response[i].unitid)

                }
                //SentForComment(ProjId, psmId);

                ProcessProjConfirm(ProjId)
            }

        }
    });
}
function SentForComment(ProjId, psmId, unitid) {
    $.ajax({
        url: '/Projects/ProcessMail',
        type: 'POST',
        data: { "ProjId": ProjId, "unitid": unitid, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                         success: function (response) {
                             console.log(response);
                             if (response && response === 1) {
                                 Swal.fire({
                                     position: 'top-end',
                                     icon: 'success',
                                     title: 'Project Processed successfully',
                                     showConfirmButton: false,
                                     timer: 700
                                 });
                             }

                             window.location.reload();
                         },
                         error: function (error) {
                             console.error('Error occurred:', error);
                             // Handle error if needed
                         }
                     });
}
function ProcessProjConfirm(ProjId) {
    $.ajax({
        url: '/Projects/IsProcessProjConfirm',
        type: 'POST',
        data: { "ProjId": ProjId },
        success: function (response) {
            console.log(response);
            if (response >= 1) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Project Successfully Submitted..!",
                    showConfirmButton: false,
                    timer: 1500
                });

            }
        }
    });
}
function FwdProjConfirm(psmId) {
        $.ajax({
            url: '/Projects/FwdProjConfirm',
            type: 'POST',
            data: { "PslmId": psmId },
            success: function (response) {
                console.log(response);
                if (response >= 1) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Project Successfully Submitted..!",
                        showConfirmButton: false,
                        timer: 1500
                    });

                }
            }
        });
    }


function Reset() {
    $("#spanFwdProjectId").html(0);
    $("#ddlfwdStage").val("");
    $("#ddlfwdSubStage").val("");
    $("#ddlfwdAction").val("");
    $("#txtRemarksfwd").val("");
    $("#ddlfwdFwdTo").val(0);
    $("#Reamarks").val("");
    $("#pdfFileInput").val("");
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