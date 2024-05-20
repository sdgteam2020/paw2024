var current = 1;
$(document).ready(function () {
    //$("#1").hide();
    //$("#3").show();
    
    $("#spanProjectId").html($("#ProjId").val());
    mMsater($("#StakeHolderId").val(),"ddlStakeHolderId",1,0)
    mMsater($("#HostTypeID").val(),"ddlHostTypeID",2,0)
    mMsater($("#Apptype").val(), "ddlApptype", 3, 0)
   
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

        ProjectSubmited($(this));
       

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
        const predate = DateFormateyyy_mm_dd(new Date()-1);
        const todaydate = DateFormateyyy_mm_dd(new Date());
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
        data: { "PslmId": $("#spanCurrentPslmId").html() },
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
function ProjectSubmited(thisdata) {
    $.ajax({
        url: '/Projects/ProjectSubmited',
        type: 'POST',
        data: { "projid": $("#spanProjectId").html() },
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
function DateFormateyyy_mm_dd(date) {

   
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