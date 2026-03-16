var current = 1;
$(document).ready(function () {
    $(function () {
        $(".datepicker1").datepicker({ dateFormat: 'dd-mm-yy' });
    });

    $("#sponsorNameInput").val($("#SponsorName").html());

    $("#spanProjectId").html($("#ProjId").val());
    mMsater($("#StakeHolderId").val(), "ddlStakeHolderId", 1, 0)
    mMsater($("#HostTypeID").val(), "ddlHostTypeID", 2, 0)
    mMsater($("#Apptype").val(), "ddlApptype", 3, 0)

    var current_fs, next_fs, previous_fs; //fieldsets
    var opacity;


    $("#uploadButton").click(function () {

        // Example logic — replace with your actual validation
      
        var requiredFields = $('#fwduploaditems').find('.requiredField');
        var allFieldsComplete = true;

        var maxlength = 200;
        var attRemarks = $("#Reamarks").val().trim(); // fixed ID



        if (attRemarks.length > maxlength) { // check string length
            $('#uploadLoader').hide();
            Swal.fire({
                title: 'Error!',
                text: 'Docu Desc cannot exceed more than 200 characters.',
                icon: 'error',
                confirmButtonText: 'OK' // showConfirmButton is true by default
            });

        }
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
            $('#uploadLoader').show();
            setTimeout(function () {
                UploadFiles();
            }, 1000)
           
        }
    });

    $("#finalupload").click(function () {
        ProjectSubmited($(this));

    });
    $("#draftUpload").click(function () {
        ProjectSaveAsDraft($(this));
    });
    $("#submitUpload").click(function () {

        var requiredFields = $('#tablebasic2').find('.requiredField');
        var allFieldsComplete = true;
        requiredFields.each(function (index) {
            if (this.value.length == 0) {
                $(this).addClass('is-invalid');
                allFieldsComplete = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        $('.char-limit').each(function () {
            var inputField = $(this);
            var maxLength = inputField.data('maxlength'); // Get max length from data-maxlength attribute
            var currentLength = inputField.val().length;
            var errorMsg = inputField.closest('td').find('.charErrorMsg');

            if (currentLength > maxLength) {
                inputField.addClass('is-invalid');
                errorMsg.removeClass('d-none');  // Show error message
                allFieldsComplete = false; // Mark form as invalid
            } else {
                inputField.removeClass('is-invalid');
                errorMsg.addClass('d-none');  // Hide error message if within limit
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
            AddProject(this);
        }


    });
    $(".previous").click(function () {
        
        const fieldset = $("#0");  // jQuery object
        fieldset.removeClass('d-none'); // hides the fieldset

        current_fs = $(this).parent();
        prev_fs = $(this).parent().prev();
        $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");
        $("#progressbar li").eq($("fieldset").index(prev_fs)).addClass("active");
        prev_fs.show();
        current_fs.animate({ opacity: 0 }, {
            step: function (now) {
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
        const predate = DateFormateyyy_mm_dd(new Date() - 1);
        const todaydate = DateFormateyyy_mm_dd(new Date());

        $("#ProjName").val("New Project" + number);
        $("#InitiatedDate").val(predate);
        $("#CompletionDate").val(todaydate);
        $("#IsWhitelisted").val("YES" + number);
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
       
        const required = [
            "#ProjName",
            "#InitiatedDate",
            "#CompletionDate",
            "#IsWhitelisted",
           
            "#AimScope",
            
            
           
           

        ];
        


        required.forEach(function (selector) {
            var inputField = $(selector);
            var maxLength = inputField.data('maxlength'); // Get max length from data-maxlength attribute
            var currentLength = inputField.val().length;
            var errorMsg = inputField.closest('td').find('.charErrorMsg');

            if (currentLength > maxLength) {
                inputField.addClass('is-invalid');
                errorMsg.removeClass('d-none');  // Show error message
                allFieldsComplete = false; // Mark form as invalid
            } else {
                inputField.removeClass('is-invalid');
                errorMsg.addClass('d-none');  // Hide error message if within limit
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
            
            const fieldset = $("#0");  // jQuery object
            fieldset.addClass('d-none'); // hides the fieldset


            current_fs = $(this).parent();

            next_fs = $(this).parent().next();
            $('#IngestionRemarksandToggle').addClass('d-none');

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


    function filterProjectNames(query) {

        $.ajax({
            url: '/Projects/GetRevettedProjects',
            method: 'GET',
            data: {
                searchQuery: query
            },
            success: function (data) {

                $("#projectNameDropdown").empty();
                if (data.length > 0) {
                    data.forEach(function (name) {
                        $("#projectNameDropdown").append(`<li class="dropdown-item" data-id="${name.projId}">${name.projName}</li>`);
                    });
                    $("#projectNameDropdown").show();
                } else {
                    $("#projectNameDropdown").hide();
                }
            },
            error: function (error) {
                console.error('Error fetching project names:', error);
            }
        });
    }


    $("#ProjName").on("keyup", function () {
        
        var query = $(this).val();
        if ($("#IsWhitelisted").val() === "Re-Vetted" && query.length>=3) {
            filterProjectNames(query);
        }
    });


    $(document).on("click", "#projectNameDropdown li", function () {
        var projectId = $(this).data("id");
        var projectName = $(this).data("name");


        $("#ProjName").val(projectName);


        $("#ProjId").val(projectId);


        getProjectDetails(projectId);

        $("#projectNameDropdown").hide();
    });


    function getProjectDetails(projId) {
        $.ajax({
            url: '/Projects/GetProjectDetails',
            method: 'GET',
            data: {
                projId: projId
            },
            success: function (project) {


                $("#spanOldPslmId").html(project.currentPslmId);
                $("#ProjId").val(project.ProjId);
                $("#ProjName").val(project.projName);


                $("#InitialRemark").val(project.initialRemark);
                $("#StakeHolderId").val(project.stakeHolderId);
                $("#AimScope").val(project.aimScope);
                $("#HQandITinfraReqd").val(project.hQandITinfraReqd);
                $("#HostTypeID").val(project.hostTypeID);
                $("#ContentofSWApp").val(project.contentofSWApp);
                $("#ReqmtJustification").val(project.reqmtJustification);
                $("#UsabilityofProposedAppln").val(project.usabilityofProposedAppln);
                $("#ProjCode").val(project.projCode);
                $("#Sponsor").val(project.sponsor);
                $("#Technology_dependencies").val(project.technology_dependencies);
                $("#Database_reqmts").val(project.database_reqmts);
                $("#DetlsofUserBase").val(project.detlsofUserBase);
                $("#EnvisagedCost").val(project.envisagedCost);
                $("#NWBandWidthReqmt").val(project.nwBandWidthReqmt);
                $("#MajTimeLines").val(project.majTimeLines);
                $("#TechStackProposed").val(project.techStackProposed);
                $("#DataSecurity_backup").val(project.dataSecurity_backup);
                $("#ProposedDB_Engine").val(project.proposedDB_Engine);
                $("#Detlsof_OS").val(project.detlsof_OS);
                $("#DetlsofSw_Architecture").val(project.detlsofSw_Architecture);
                $("#DetlsofProposed_Architecture").val(project.detlsofProposed_Architecture);
                $("#DetlsPki_IAM").val(project.detlsPki_IAM);
                $("#Enhancement_upgradation").val(project.enhancement_upgradation);
                $("#Details_licensing").val(project.details_licensing);
                $("#ddlApptype").val(project.apptype);
                $("#ddlHostTypeID").val(project.hostTypeID);
                $("#TypeofSW").val(project.typeofSW);
                $("#BeingDevpInhouse").val(project.beingDevpInhouse);
                $("#EndorsmentbyHeadof").val(project.endorsmentbyHeadof);



            },
            error: function (error) {
                console.error('Error fetching project details:', error);
            }
        });
    }


    $("#IsWhitelisted").change(function () {
        var selectedStatus = $(this).val();
        if (selectedStatus === "Re-Vetted") {
            Swal.fire({
                title: 'Re-Vetting Project will be added as per details of Appx C to SOP , Also before filling the form, ensure that you have all the docus(PDF) ready as per SOP on Whitelisting of Sw Appl in IA',
                html: `
      <div class=text-left><div><ol><li><h5 class=p-2>Upload 02 x PDF Documents Before Submitting</h5><ul class=text-left><div><strong>(a) ACG Clearance Certificate </strong></div><div><strong>(b) DDGIT Whitelisted Clearance Certificate</strong></div></ul></li><li><h5 class=p-2>Declaration</h5><p class=text-left>I declare that all the information which I will be providing is correct to the best of my knowledge.</p></li></ol>
    `,
                icon: 'warning',
                confirmButtonColor: '#072697',
                confirmButtonText: '<i class="fa fa-check-circle"></i> Proceed',
                customClass: {
                    popup: 'swal2-border-radius'
                }
            });


            $("#ProjName").prop("disabled", false);
            $("#projectNameDropdown").show();
            $("#reVettedTag").removeClass("d-none");
        } else {
            var currentval = $("#ProjName").val();
            var cleanval = currentval.replace(/\s*Re-Vetted\s*\d+/i, "").trim();
            $("#ProjName").val(cleanval)

            $("#projectNameDropdown").hide();
            $("#reVettedTag").addClass("d-none");
        }
    });


    $(document).click(function (e) {
        if (!$(e.target).closest('#projectNameDropdown').length) {
            $("#projectNameDropdown").hide();
        }
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
    
    fetchServerDate().then(function (S) {

        var token = $('input[name="__RequestVerificationToken"]').val();
        
        var initialDate = $('#InitiatedDate').val();
        var completionDate = $('#CompletionDate').val();
        var currentDate = new Date(S.todayDateTime);
        var currentTime = currentDate.toLocaleTimeString('en-US', { hour12: false });

       
       
        var InitiatedDate = initialDate + ' ' + currentTime;
        var CompletionDate = completionDate + ' ' + currentTime;
        console.log("Inititated date:", initialDate + ", Complition Date: ", InitiatedDate)
        $.ajax({
            url: '/Projects/AddProject',
            type: 'POST',
            data: {
                "ProjId": $("#ProjId").val(),
                "ProjName": $("#ProjName").val(),
                "InitiatedDate": InitiatedDate,
                "CompletionDate": CompletionDate,
                "IsWhitelisted": $("#IsWhitelisted").val(),
                "Security_Classification": $("#Security_Classification").val(),
                "AsconNo": $("#AsconNo").val(),
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
                "ProjCode": $("#ProjCode").val(),
                "Sponsor": $("#sponsorNameInput").val(),

                "Detlsof_OS": $("#Detlsof_OS").val(),
                "ProposedDB_Engine": $("#ProposedDB_Engine").val(),
                "DetlsofSw_Architecture": $("#DetlsofSw_Architecture").val(),
                "DetlsofProposed_Architecture": $("#DetlsofProposed_Architecture").val(),
                "DetlsPki_IAM": $("#DetlsPki_IAM").val(),
                "Technology_dependencies": $("#Technology_dependencies").val(),
                "Database_reqmts": $("#Database_reqmts").val(),
                "Enhancement_upgradation": $("#Enhancement_upgradation").val(),
                "Details_licensing": $("#Details_licensing").val(),
                "OldPsmid": parseInt($("#spanOldPslmId").html()) || 0,
                "Date_type": $('input[name="mcalender_dates"]:checked').val(),
                "RequestRemarks": $("#RequestRemarks").val(),
                 "MobileNo": $("#MobileNo").val(),
                "Devlopment_Language": $("#Devlopment_Language").val(),
                "operation_system_hosting_env": $("#operation_system_hosting_env").val()
            },
            headers: {
                'RequestVerificationToken': token
            },//get the search string
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
                        title: "Error!",
                        text: "Project Name Already Exists!",
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
                        title: "Error!",
                        text: "Incorrect Selection Click Again on Edit!",
                        icon: "Error"
                    });
                }
                else if (result != null) {

                    var projid = result.projId;
                    $("#spanProjectId").html(projid);
                    $("#spanCurrentPslmId").html(result.currentPslmId);
                    var creatid = "PROJECT ID :" + projid
                    $("#projectId").html(creatid)




                    AttechHistory();
                    current_fs = $(thistag).parent();
                    next_fs = $(thistag).parent().next();
                    $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");
                    next_fs.show();
                    current_fs.animate({ opacity: 0 }, {
                        step: function (now) {
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
    })
}
function FwdProjConfirm(thisdata) {
   
    $.ajax({
        url: '/Projects/FwdProjConfirm',
        type: 'POST',
        data: { "PslmId": $("#spanCurrentPslmId").html() },
        success: function (response) {


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
                $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");
                next_fs.show();
                current_fs.animate({ opacity: 0 }, {
                    step: function (now) {
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
    var Remarks = $("#RequestRemarks").val();
    $.ajax({
        url: '/Projects/ProjectSubmited',
        type: 'POST',
        data: { "projid": $("#spanProjectId").html(), "type": 1, "Remarks": Remarks },
        success: function (response) {
            console.log(response);
            if (response.type == 404) {
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: response.message,
                    showConfirmButton: true,

                });
            }
            if (response >= 1) {
                $("#RequestRemarks").val("");
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Project Submit Successfully",
                    showConfirmButton: false,
                    timer: 1500
                });

                current_fs = $(thisdata).parent();
                next_fs = $(thisdata).parent().next();
                $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");
                next_fs.show();
                current_fs.animate({ opacity: 0 }, {
                    step: function (now) {
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


function ProjectSaveAsDraft(thisdata) {
    $.ajax({
        url: '/Projects/ProjectSubmited',
        type: 'POST',
        data: { "projid": $("#spanProjectId").html(), "type": 2, "Remarks": "" },
        success: function (response) {
            console.log(response);
            if (response.type == 404) {
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: response.message,
                    showConfirmButton: true,
                   
                });
            }
            if (response >= 1) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Draft Saved Successfully in Initiated Projects",
                    showConfirmButton: false,
                    timer: 1500
                });
                $("#draftUpload").prop("disabled", true).removeClass("enabled-button").addClass("disabled-button");
                $("#finalupload").prop("disabled", false).removeClass("disabled-button").addClass("enabled-button");
            }
        }
    });
}
function validationIsSuccessful() {
    var inputVal = $("#ProjEdit_DetlsofUserBase").val();
    if (inputVal === "") {
        return false;
    }
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
}

function DeleteProject(ProjectId) {
    $.ajax({
        url: '/Projects/DeleteProjects',
        type: 'POST',
        data: { "ProjectId": ProjectId },
        success: function (response) {


            if (response == 1) {
                alert("Record Deleted successfully");

                location.reload();

            }

        }
    });
}


document.addEventListener('DOMContentLoaded', function () {
    var notificationDiv = document.getElementById('notificationData');
    var notificationTitle = notificationDiv.getAttribute('data-title');
    var notificationHtml = notificationDiv.getAttribute('data-html');

    notificationTitle = notificationTitle.replace(/'/g, "\\'");
    notificationHtml = notificationHtml.replace(/'/g, "\\'").replace(/\n/g, '\\n').replace(/\r/g, '');

   

    Swal.fire({
        title: notificationTitle,
        html: notificationHtml,
        icon: 'info',
        confirmButtonText: 'OK',
        width: '861px',
        padding: '20px',
        background: '#f9f9f9',
        customClass: {
            container: 'swal-container'
        }
    });
});
 
        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('btn-Initiation').addEventListener('click', function () {
                Swal.fire({
                    title: 'Initiation Date Details',
                    text: 'Date of Initiation of Project for Whitelisting (To DDGIT)',
                    icon: 'info',
                    confirmButtonText: 'OK',
                    customClass: {
                        title: 'swal2-title', 
                        htmlContainer: 'swal2-html-container' 
                    }
                });
            });
            document.getElementById('btn-complition').addEventListener('click', function () {
                Swal.fire({
                    title: 'Completion Date  Details',
                    text: 'The Completion date is likely to be assumed',
                    icon: 'info',
                    confirmButtonText: 'OK',
                    customClass: {
                        title: 'swal2-title', 
                        htmlContainer: 'swal2-html-container' 
                    }
                });
            });
            document.getElementById('btn-Sponsor').addEventListener('click', function () {
                Swal.fire({
                    title: 'Sponsor Details',
                    text: 'Contact Admin to Update Name',
                    icon: 'info',
                    confirmButtonText: 'OK',
                    customClass: {
                        title: 'swal2-title', 
                        htmlContainer: 'swal2-html-container' 
                    }
                });
            });
        });
  
 
        $(document).ready(function () {
            $('.char-limit').each(function () {

                var inputField = $(this);
                var maxLength = parseInt(inputField.data('maxlength'));
                var errorMsg = inputField.closest('div').find('.charErrorMsg');

                inputField.on('input', function () {

                    var value = inputField.val();

                    // Stop typing after max length
                    if (value.length > maxLength) {
                        inputField.val(value.substring(0, maxLength));
                        errorMsg.removeClass('d-none');
                    } else {
                        errorMsg.addClass('d-none');
                    }

                });

            });

        });

$('#RequestRemarks').on('input', function () {

    const maxLength = $(this).data('maxlength');
    const errorMsg = $(this).siblings('.charErrorMsg');

    if (this.value.length > maxLength) {
        this.value = this.value.slice(0, maxLength);
        errorMsg.removeClass('d-none');
        $(this).addClass('is-invalid');
    } else {
        errorMsg.addClass('d-none');
        $(this).removeClass('is-invalid');
    }
});
