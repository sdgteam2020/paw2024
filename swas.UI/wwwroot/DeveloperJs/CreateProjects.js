var current = 1;
$(document).ready(function () {
    //$("#1").hide();
    //$("#3").show();
    
    $("#spanProjectId").html($("#ProjId").val());
    mMsater($("#StakeHolderId").val(),"ddlStakeHolderId",1,0)
    mMsater($("#HostTypeID").val(),"ddlHostTypeID",2,0)
    mMsater($("#Apptype").val(), "ddlApptype", 3, 0)
    AttechHistory();
    var current_fs, next_fs, previous_fs; //fieldsets
    var opacity;

   

    $("#uploadButton").click(function () {

       

        requiredFields = $('#fwduploaditems').find('.requiredField');
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

            Swal.fire({
                title: 'Error!',
                text: 'Please complete all required fields',
                icon: 'error',
                showConfirmButton: false,
                timer: 1000
            })
        }
        else {
            UploadFiles();
        }
    });
    $("#finalupload").click(function () {

        FwdProjConfirm($(this));
       

    });
     $("#submitUpload").click(function () {


         AddProject(this);
       

    });
    $(".previous").click(function () {

        current_fs = $(this).parent();
        prev_fs = $(this).parent().prev();

        //Add Class Active
        $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");
        $("#progressbar li").eq($("fieldset").index(prev_fs)).addClass("active");

        //show the next fieldset
        prev_fs.show();
        //hide the current fieldset with style
        current_fs.animate({ opacity: 0 }, {
            step: function (now) {
                // for making fielset appear animation
                opacity = 1 - now;

                current_fs.css({
                    'display': 'none',
                    'position': 'relative'
                });
                prev_fs.css({ 'opacity': opacity });
            },
            duration: 600
        });


    });

    $("#tempBasicDetails").click(function () {
        var number = 1 + Math.floor(Math.random() * 15000);
        const predate = DateFormatedd_mm_yyyy(new Date()-1);
        const todaydate = DateFormatedd_mm_yyyy(new Date());
        $("#ProjName").val("New Project" + number);
        $("#InitiatedDate").val(predate);
        $("#CompletionDate").val(todaydate);;
        $("#IsWhitelisted").val("YES" + number);;
        $("#InitialRemark").val("Remarks" + number);
        $("#AimScope").val("AimScope" + number);
        $("#HQandITinfraReqd").val("HQandITinfraReqd" + number);
        $("#ContentofSWApp").val("Content of SWApp" + number);
        $("#ReqmtJustification").val("Reqmt Justification" + number);
        $("#UsabilityofProposedAppln").val("Usability of Proposed" + number);

        $("#ddlStakeHolderId").val(1);
        $("#ddlHostTypeID").val(1);
        $("#ddlApptype").val(1);


        $("#DetlsofUserBase").val("DetlsofUserBase" + number);
        $("#EnvisagedCost").val(number);
        $("#NWBandWidthReqmt").val("NWBa" + number);
        $("#MajTimeLines").val("MajTimeLines" + number);
        $("#TechStackProposed").val("TechStackProposed" + number);
        $("#DataSecurity_backup").val("DataSecurity_backup" + number);
        $("#TypeofSW").val("TypeofSW" + number);
        $("#BeingDevpInhouse").val("BeingDevpInhouse" + number);
        $("#EndorsmentbyHeadof").val("EndorsmentbyHeadof" + number);
    });
    $(".requiredField").change(function () {


        if (this.value.length == 0) {
            $(this).addClass('is-invalid');

        } else {
            $(this).removeClass('is-invalid');
            $(this).addClass('is-valid');
        }
    });
    $(".requiredField").blur(function () {

        
        if (this.value.length == 0) {
            $(this).addClass('is-invalid');
            
        } else {
            $(this).removeClass('is-invalid');
            $(this).addClass('is-valid');
        }
    });
    $("#btnbasic").click(function () {

       
        requiredFields = $('#tablebasic').find('.requiredField');
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
           
            Swal.fire({
                title: 'Error!',
                text: 'Please complete all required fields',
                icon: 'error',
                showConfirmButton: false,
                timer: 1000
            })
        }
        else {
            current_fs = $(this).parent();
            next_fs = $(this).parent().next();


            //Add Class Active
            $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

            //show the next fieldset
            next_fs.show();
            //hide the current fieldset with style
            current_fs.animate({ opacity: 0 }, {
                step: function (now) {
                    // for making fielset appear animation
                    opacity = 1 - now;

                    current_fs.css({
                        'display': 'none',
                        'position': 'relative'
                    });
                    next_fs.css({ 'opacity': opacity });
                },
                duration: 600
            });

        }
        return allFieldsComplete;
    });

   
    $("body").on("click", ".btndeleteProject", function () {

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
                
                
                DeleteProject($(this).closest("tr").find(".tblspnprojectid").html());

            }
        });
    });
});
function AddProject(thistag) {

    $.ajax({
        url: '/Projects/AddProject',
        type: 'POST',
        data: {
            "ProjId": $("#ProjId").val(),
            "ProjName": $("#ProjName").val(),
            "InitiatedDate": $("#InitiatedDate").val(),
            "CompletionDate": $("#CompletionDate").val(),
            "IsWhitelisted": $("#IsWhitelisted").val(),
            "InitialRemark": $("#InitialRemark").val(),
            "StakeHolderId": $("#ddlStakeHolderId").val(),
            "AimScope": $("#AimScope").val(),
            "HQandITinfraReqd": $("#HQandITinfraReqd").val(),
            "HostTypeID": $("#ddlHostTypeID").val(),
            "ContentofSWApp": $("#ContentofSWApp").val(),
            "ReqmtJustification": $("#ReqmtJustification").val(),
            "UsabilityofProposedAppln": $("#UsabilityofProposedAppln").val(),
            "Apptype": $("#ddlApptype").val(),
            "DetlsofUserBase": $("#DetlsofUserBase").val(),
            "EnvisagedCost": $("#EnvisagedCost").val(),
            "NWBandWidthReqmt": $("#NWBandWidthReqmt").val(),
            "MajTimeLines": $("#MajTimeLines").val(),
            "TechStackProposed": $("#TechStackProposed").val(),
            "DataSecurity_backup": $("#DataSecurity_backup").val(),
            "TypeofSW": $("#TypeofSW").val(),
            "BeingDevpInhouse": $("#BeingDevpInhouse").val(),
            "EndorsmentbyHeadof": $("#EndorsmentbyHeadof").val(),
            "CurrentPslmId": $("#CurrentPslmId").val(),
            "ProjCode": $("#ProjCode").val()
            
           

        }, //get the search string
        success: function (result) {


           
            if (result == -2) {
                

                Swal.fire({
                    title: "success!",
                    text: "User has been Updated!",
                    icon: "success"
                });

            }
            else if (result == -3) {

                Swal.fire({
                    title: "success!",
                    text: "Project Name Allredy Exit!",
                    icon: "Error"
                });

            }
            else if (result == -4) {
                Swal.fire({
                    title: "success!",
                    text: "Incorrect Data!",
                    icon: "Error"
                });
                

            }
            else if (result == -5) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })

            } else if (result != null) {
               

                    $("#spanProjectId").html(result.projId);
                    $("#spanCurrentPslmId").html(result.currentPslmId);


                    AttechHistory();
                    current_fs = $(thistag).parent();
                    next_fs = $(thistag).parent().next();


                    //Add Class Active
                    $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

                    //show the next fieldset
                    next_fs.show();
                    //hide the current fieldset with style
                    current_fs.animate({ opacity: 0 }, {
                        step: function (now) {
                            // for making fielset appear animation
                            opacity = 1 - now;

                            current_fs.css({
                                'display': 'none',
                                'position': 'relative'
                            });
                            next_fs.css({ 'opacity': opacity });
                        },
                        duration: 600
                    });


                


            }
        }
    });
}
function FwdProjConfirm(thisdata) {
    $.ajax({
        url: '/Projects/FwdProjConfirm',
        type: 'POST',
        data: { "projid": $("#spanCurrentPslmId").html() },
        success: function (response) {
            console.log(response);


            if (response >=1) {

                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Project Successfully Submitted..!",
                    showConfirmButton: false,
                    timer: 1500
                });

                current_fs = $(thisdata).parent();
                next_fs = $(thisdata).parent().next();


                //Add Class Active
                $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

                //show the next fieldset
                next_fs.show();
                //hide the current fieldset with style
                current_fs.animate({ opacity: 0 }, {
                    step: function (now) {
                        // for making fielset appear animation
                        opacity = 1 - now;

                        current_fs.css({
                            'display': 'none',
                            'position': 'relative'
                        });
                        next_fs.css({ 'opacity': opacity });
                    },
                    duration: 600
                });
               

            }

        }
    });
}
function UploadFiles() {
    var formData = new FormData();
    var totalFiles = document.getElementById("pdfFileInput").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("pdfFileInput").files[i];
        formData.append("uploadfile", file);
        formData.append("Reamarks", $("#Reamarks").val());
        formData.append("ProjectId", $("#spanProjectId").html());
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
               
            }
            
        },
        error: function (error) {
            $(".error-msg").removeClass("d-none")
            $("#error-msg").html("Somthing is wrong");;

        }
    });
}

function AttechHistory() {
    var listItem = "";
    var userdata =
    {
        "ProjectId": $("#spanProjectId").html(),

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
                    listItem += "<tr><td class='text-center' colspan=4>No Record Found</td></tr>";
                  
                    $("#DetailBody").html(listItem);
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

                    $("#DetailBody").html(listItem);
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
                $("#DetailBody").html(listItem);
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
function validationIsSuccessful() {
    // Your validation logic goes here
    // Return true if validation passes, false otherwise
    // Example:
    var inputVal = $("#ProjEdit_DetlsofUserBase").val();
    if (inputVal === "") {
        // Validation fails
        return false;
    }
    // Validation passes
    return true;
}
function DateFormatedd_mm_yyyy(date) {

   
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    if (year > 1902) {

        var datemmddyyyy = year + `-` + monthsans + `-` + dayans
        return datemmddyyyy;
    }
    else {
        return '';
    }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}

function checkFileSize(input) {
    const maxFileSizeInBytes = 500000;
    if (input.files.length > 0) {
        const fileSize = input.files[0].size;
        if (fileSize > maxFileSizeInBytes) {
            alert("File size exceeds the maximum limit of 5MB.");
            input.value = '';
        }
    }
}

function DeleteProject(ProjectId) {
    $.ajax({
        url: '/Projects/DeleteProjects',
        type: 'POST',
        data: { "ProjectId": ProjectId },
        success: function (response) {
            console.log(response);


            if (response == 1) {
                alert("Record Deleted successfully");

                location.reload();

            }

        }
    });
}