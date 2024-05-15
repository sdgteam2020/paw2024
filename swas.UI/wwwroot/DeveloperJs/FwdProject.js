$(document).ready(function () {

    mMsater(0, "ddlfwdStage", 5, 0)
    
    $("#ddlfwdStage").change(function () {
       
        mMsaterStage(0, "ddlfwdSubStage", 6, $("#ddlfwdStage").val(), $("#SpnStakeHolderId").html())
    });
    $("#ddlfwdSubStage").change(function () {

        mMsater(0, "ddlfwdAction", 7, $("#ddlfwdSubStage").val())
    });
    $("#ddlfwdAction").change(function () {
     
        mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnStakeHolderId").html())
    });
    $(".btn-Undo").click(function () {
        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
        Swal.fire({
            title: "Enter Undo Remarks",
            input: "text",
            inputAttributes: {
                autocapitalize: "off"
            },
            showCancelButton: true,
            confirmButtonText: "Undo",
            showLoaderOnConfirm: true,
            preConfirm: async (login) => {
                if (login == "") {
                    Swal.showValidationMessage(`
               Please Enter Remarks: `);
                }
                },
            allowOutsideClick: () => !Swal.isLoading()
        }).then((result) => {
            if (result.isConfirmed) {
               
                Updateundo($(this).closest("tr").find("#SpnCurrentProjId").html(), $(this).closest("tr").find("#SpnCurrentpsmId").html(), result.value);

            }
        });
    });
    $(".btn-FwdHistory").click(function () {
        $('#ProjFwdHistory').modal('show');
        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
        GetProjectMovHistory($(this).closest("tr").find("#SpnCurrentProjId").html());
    });
        $(".btn-Fwd").click(function () {
       
        $("#spanFwdCurrentPslmId").html($(this).closest("tr").find("#SpnCurrentpsmId").html())
        $("#spanFwdProjectId").html($(this).closest("tr").find("#SpnCurrentProjId").html())
        $("#SpnFwdStakeHolderId").html($(this).closest("tr").find("#SpnStakeHolderId").html())
           
            IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
        $('#ProjFwd').modal('show');

        $(".Fwdtitle").html("Projects Move Details");
        $(".ProjectsFwd").removeClass("d-none");
        $(".Attmenthistory").addClass("d-none");

        if ($(this).closest("tr").find("#SpnprojectIsProcess").html() == 'False') {
            $("#ddlfwdStage option[value='2']").remove();
            $("#ddlfwdStage option[value='3']").remove();
        }
        else {
            $("#ddlfwdStage option[value='1']").remove();

        }
       // alert($(this).closest("tr").find("#SpnprojectStageId").html())
        //GetAllComments($(this).closest("tr").find("#SpnCurrentProjId").html());
    });
    $(".btn-Obsn").click(function () {

        $("#spanFwdCurrentPslmId").html($(this).closest("tr").find("#SpnCurrentpsmId").html())
        $("#spanFwdProjectId").html($(this).closest("tr").find("#SpnCurrentProjId").html())
        $("#SpnFwdStakeHolderId").html($(this).closest("tr").find("#SpnStakeHolderId").html())


        $('#ProjFwd').modal('show');

        $(".Fwdtitle").html("Projects Obsn To Unit");

        $(".ProjectsFwd").removeClass("d-none");
        $(".Attmenthistory").addClass("d-none");

        
        $("#ddlfwdStage option[value='2']").remove();
        $("#ddlfwdStage option[value='3']").remove();
        //GetAllComments($(this).closest("tr").find("#SpnCurrentProjId").html());
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
            
            CheckFwdCondition($("#spanFwdCurrentPslmId").html())
           
        }
    });

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
                    UploadFiles();
                }
            });
        }
    });

    $("#btnFwdConfirm").click(function () {
        location.reload();
        $('#ProjFwd').modal('hide');
    });
});
function CheckFwdCondition(CurrentPslmId,) {
   
    AttechHistory();
    SaveFwdTo(CurrentPslmId);
}
function SaveFwdTo(CurrentPslmId) {
    var userdata =
    {
        "ProjId": $("#spanFwdProjectId").html(),
        "StatusId": $("#ddlfwdSubStage").val(),
        "ActionId": $("#ddlfwdAction").val(),
        "Remarks": $("#txtRemarksfwd").val(),
        "ToUnitId": $("#ddlfwdFwdTo").val(),

    };
        $.ajax({
            url: '/Projects/FwdToProject',
            type: 'POST',
            data: userdata,
            success: function (response) {
                console.log(response);
                if (response != null) {
                    $("#spanCurrentPslmId").html(response.psmId);
                    FwdProjConfirm(CurrentPslmId);
                    $(".Fwdtitle").html("Projects Attch Details");
                    $(".ProjectsFwd").addClass("d-none");
                    $(".Attmenthistory").removeClass("d-none");
                   
                }

            }
        });
    
}
function Updateundo(ProjId, PslmId, UndoRemarks) {
    var userdata =
    {
        "ProjectId": ProjId,
        "PsmId": PslmId,
        "Remarks": UndoRemarks
        

    };
    $.ajax({
        url: '/Projects/UndoProject',
        type: 'POST',
        data: userdata,
        success: function (response) {
            console.log(response);
            if (response != null) {
                if (response == 2) {
                    alert("Project Successfully Undo");
                    location.reload();
                }

            }

        }
    });

}