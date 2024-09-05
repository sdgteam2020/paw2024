$(document).ready(function () {
    mMsaterfwdStage(0, "ddlfwdStage", 5, 0)

    $("#ddlfwdStage").change(function () {
        mMsaterStage(0, "ddlfwdSubStage", 6, $("#ddlfwdStage").val(), 0)
    });
    $("#ddlfwdSubStage").change(function () {

        mMsater(0, "ddlfwdAction", 7, $("#ddlfwdSubStage").val())
    });
    $("#ddlfwdAction").change(function () {

        mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html());
    });

   // GetProjectMovement();
    $("#txtProjectName").autocomplete({
        source: function (request, response) {

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


        var userdata =
        {
            "ProjId": $("#spanProjectId").html(),
            "PsmId": $("#spanEditPslmId").html(),
            /* "StatusId": $("#ddlfwdSubStage").val(),*/
            "StatusActionsMappingId": $("#ddlfwdAction").val(),
            "Remarks": $("#txtRemarksfwd").val(),
            "ToUnitId": $("#ddlfwdFwdTo").val(),

            "TimeStamp": $("#TimeStampToProjfwd").val()
        };
        $.ajax({
            url: '/Projects/ProjectMovementUpdate',
            type: 'POST',
            data: userdata,
            success: function (response) {
                console.log(response);
                if (response != null) {
                    /*$("#spanEditPslmId").html(response.psmId);*/
                    FwdProjConfirm(CurrentPslmId);
                    $(".Fwdtitle").html("Projects Attch Details");
                    $(".ProjectsFwd").addClass("d-none");
                    $(".Attmenthistory").removeClass("d-none");

                }

            }
        });
    }

   

    $("#btnAttchMultiforpsmid").click(function () {

        alert("Amit3");
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

        $('#ProjFwdEdit').modal('hide');
        
      

        
        GetProjectMovement($("#spanProjectId").html());
       
    });

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
          else  if (response == -2) {

                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Only Pdf File Upload!",
                    
                });
            }

        },
        error: function (error) {
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
            console.log(response);


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
function GetProjectMovement(ProjectId)
{

        var listItem = "";
     
    $.ajax({
        url: '/Projects/GetProjectMov',
        type: 'Post',
        data: {
            "Id": ProjectId
        },
        success: function (response) {
           
            console.log(response);
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
                        listItem += "<td class='d-none'><span id='spnpsmId' class='d-none'>" + response[i].psmIds + "</span>";
                        listItem += "<span id='spnStageId' class='d-none'>" + response[i].stageId + "</span>";
                        listItem += "<span id='spanProjId' class='d-none'>" + response[i].projId + "</span>";
                        listItem += "<span id='spnStatusId' class='d-none'>" + response[i].statusId + "</span>";
                        listItem += "<span id='spnActionId' class='d-none'>" + response[i].actionId + "</span>";
                        listItem += "<span id='spnToUnitId' class='d-none'>" + response[i].toUnitId + "</span>";
                       
                        listItem += "</td>";
                        listItem += "<td>" + count +"</td>";
                        listItem += "<td class=''><span id='spnDate' class='d-none'>" + response[i].dateTimeOfUpdate + "</span><span id=''>" + DateFormateddMMyyyyhhmmss(response[i].dateTimeOfUpdate) + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].fromUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].toUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].stage + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].status + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].action + "</span></td>";
                        listItem += "<td class='align-middle'><span id='spnremarks'>" + response[i].remarks + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'><span class='btn btn-primary cls-btnedit'>Edit</span></td>";


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
                        "order": [[0, "asc"]],
                        "ordering": true,
                        "paging": true,
                        dom: 'lBfrtip',
                        buttons: [
                            'copy',
                            'excel',
                            'csv',
                           
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

                        $(".ProjectsFwd").removeClass("d-none");
                        $(".Attmenthistory").addClass("d-none");

                        $("#spanProjectId").html($(this).closest("tr").find("#spanProjId").html());
                        $("#spanEditPslmId").html($(this).closest("tr").find("#spnpsmId").html());
                        $("#txtRemarksfwd").val($(this).closest("tr").find("#spnremarks").html());
                        $("#TimeStampToProjfwd").val($(this).closest("tr").find("#spnDate").html());



                        mMsaterfwdStage($(this).closest("tr").find("#spnStageId").html(), "ddlfwdStage", 5, 0, 1)
                        mMsaterStage($(this).closest("tr").find("#spnStatusId").html(), "ddlfwdSubStage", 6, $(this).closest("tr").find("#spnStageId").html(), 0)
                        mMsater($(this).closest("tr").find("#spnActionId").html(), "ddlfwdAction", 7, $(this).closest("tr").find("#spnStatusId").html())
                        mMsaterFwdTo($(this).closest("tr").find("#spnToUnitId").html(), "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html(), $(this).closest("tr").find("#spnToUnitId").html());

                    });

                }

            }

        }

    }
    );
    
}




