$(document).on('ready', function () {
    $(document).on('click', '.pluscircle', function () {
        $('#UnitAdd').modal('show');
    });


    var initiatedDateInput = $("#ProjEdit_InitiatedDate");
    var completionDateInput = $("#ProjEdit_CompletionDate");

    var today = new Date();


    var formattedDate = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');


    $("#ProjEdit_InitiatedDate").val(formattedDate);

    var completionDateInput = document.getElementById("ProjEdit_CompletionDate");

    document.addEventListener('DOMContentLoaded', function () {

        var form = document.getElementById('msform');
        var submitButton = document.getElementById('submitUpload');
        submitButton.addEventListener('click', function (event) {
            event.preventDefault();
            form.submit();
        });
    });

    var initiatedDateInput = $("#ProjEdit_InitiatedDate");
    var completionDateInput = $("#ProjEdit_CompletionDate");

    completionDateInput.on("change", function () {

        var initiatedDateVal = initiatedDateInput.val();
        var completionDateVal = completionDateInput.val();

        if (!initiatedDateVal) {

            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Initiation Date is required',
                confirmButtonColor: '#d33',
                confirmButtonText: 'OK'
            });

            completionDateInput.val('');
            return;
        }

        var initiatedDate = new Date(initiatedDateVal);
        var completionDate = new Date(completionDateVal);


        if (completionDate <= initiatedDate) {

            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Completion Date must be greater than Initiation Date',
                confirmButtonColor: '#d33',
                confirmButtonText: 'OK'
            });

            completionDateInput.val('');
        }
    });

    const addFormContainer = document.getElementById('addForm');

    addFormContainer.style.display = 'block';

    document.addEventListener('DOMContentLoaded', () => {
        const editButtons = document.querySelectorAll('.editButton');
        editButtons.forEach(editButton => {
            editButton.addEventListener('click', async () => {




                var fdset = "fieldset#" + "uploaded";
                $(fdset).hide();
                var fdset = "fieldset#" + "5";

                editFormContainer.style.display = 'block';
                addFormContainer.style.display = 'none';

                $("#fieldset#5").addClass("active");
                $(fdset).show();



                const projId = editButton.getAttribute('data-proj-id');
                const response = await fetch(`/Projects/Details?id=${projId}`);
                const data = await response.json();

                if (data.success) {

                    const project = data.project;



                    const editFormContainer = document.getElementById('editFormContainer');
                    const ProjNameInput = editFormContainer.querySelector('#ProjEdit_ProjName');
                    const InitiatedDateInput = editFormContainer.querySelector('#ProjEdit_InitiatedDate');
                    const CompletionDateInput = editFormContainer.querySelector('#ProjEdit_CompletionDate');
                    const IsWhitelistedInput = editFormContainer.querySelector('#ProjEdit_IsWhitelisted');
                    const InitialRemarkInput = editFormContainer.querySelector('#ProjEdit_InitialRemark');
                    const StakeHolderIdSelect = editFormContainer.querySelector('#ddlUnitIdedit');
                    const AppTypeidSelect = editFormContainer.querySelector('#ddlAppTypeEdit');
                    const AimScopeTextarea = editFormContainer.querySelector('#ProjEdit_AimScope');
                    const HQandITinfraReqdInput = editFormContainer.querySelector('#ProjEdit_HQandITinfraReqd');
                    const HostedonInput = editFormContainer.querySelector('#Hostedtype');
                    const ContentofSWAppTextarea = editFormContainer.querySelector('#ProjEdit_ContentofSWApp');
                    const ReqmtJustificationTextarea = editFormContainer.querySelector('#ProjEdit_ReqmtJustification');
                    const UsabilityofProposedApplnInput = editFormContainer.querySelector('#ProjEdit_UsabilityofProposedAppln');
                    const DetlsofUserBaseInput = editFormContainer.querySelector('#ProjEdit_DetlsofUserBase');

                    const EnvisagedCostTextarea = editFormContainer.querySelector('#envisagedCostInput');

                    const NWBandWidthReqmtInput = editFormContainer.querySelector('#ProjEdit_NWBandWidthReqmt');


                    const MajTimeLinesInput = editFormContainer.querySelector('#ProjEdit_MajTimeLines');
                    const TechStackProposedInput = editFormContainer.querySelector('#ProjEdit_TechStackProposed');
                    const DataSecurity_backupInput = editFormContainer.querySelector('#ProjEdit_DataSecurity_backup');
                    const TypeofSWInput = editFormContainer.querySelector('#ProjEdit_TypeofSW');
                    const BeingDevpInhouseInput = editFormContainer.querySelector('#ProjEdit_BeingDevpInhouse');
                    const EndorsmentbyHeadofInput = editFormContainer.querySelector('#ProjEdit_EndorsmentbyHeadof');

                    const ProjEditProjId = editFormContainer.querySelector('#ProjEdit_ProjId');
                    const CurrentPslmId = editFormContainer.querySelector('#ProjEdit_CurrentPslmId');



                    CurrentPslmId.value = project.currentPslmId;
                    ProjEditProjId.value = project.projId;
                    ProjNameInput.value = project.projName;

                    InitiatedDateInput.value = formatDate(project.initiatedDate);
                    CompletionDateInput.value = formatDate(project.completionDate);

                    setSelectedValue(StakeHolderIdSelect, project.stakeHolderId);
                    setSelectedValue(AppTypeidSelect, project.apptype);
                    setSelectedValue(HostedonInput, project.hostTypeID);




                    IsWhitelistedInput.value = project.isWhitelisted;
                    InitialRemarkInput.value = project.initialRemark;
                    AimScopeTextarea.value = project.aimScope;
                    HQandITinfraReqdInput.value = project.hQandITinfraReqd;

                    ContentofSWAppTextarea.value = project.contentofSWApp;

                    ReqmtJustificationTextarea.value = project.reqmtJustification;
                    UsabilityofProposedApplnInput.value = project.usabilityofProposedAppln;
                    DetlsofUserBaseInput.value = project.detlsofUserBase;
                    EnvisagedCostTextarea.value = project.envisagedCost;

                    NWBandWidthReqmtInput.value = project.nwBandWidthReqmt;

                    MajTimeLinesInput.value = project.majTimeLines;
                    TechStackProposedInput.value = project.techStackProposed;
                    DataSecurity_backupInput.value = project.dataSecurity_backup;
                    TypeofSWInput.value = project.typeofSW;
                    BeingDevpInhouseInput.value = project.beingDevpInhouse;
                    EndorsmentbyHeadofInput.value = project.endorsmentbyHeadof;



                    editFormContainer.style.display = 'block';
                    addFormContainer.style.display = 'none';
                }
            });
        });
    });

    editAnchors.forEach((editAnchor) => {
        editAnchor.addEventListener('click', (event) => {
            event.preventDefault();
            const projName = editAnchor.getAttribute('data-proj-name');


            editFormContainer.style.display = 'block';
            addFormContainer.style.display = 'none';
        });
    });

    document.addEventListener("DOMContentLoaded", function () {
        const pdfInput = document.getElementById("pdfInput");
        const pdfList = document.getElementById("pdfList");
        const selectedFileList = document.getElementById("selected-file-list");
        const remarksInput = document.getElementById("remarksInput");
        const uploadedFiles = [];


        function createPDFItem(file) {
            const pdfItem = document.createElement("div");
            pdfItem.className = "pdf-item";
            pdfItem.setAttribute("data-filename", file.name);

            const pdfPreview = document.createElement("div");
            pdfPreview.className = "pdf-preview";

            const reader = new FileReader();
            reader.onload = function () {
                const fileType = file.type;

                if (fileType.includes("image")) {
                    pdfPreview.innerHTML = `<img src="${reader.result}" alt="Image Preview" class="image-preview">`;
                } else if (fileType === "application/pdf") {
                    pdfPreview.innerHTML = `<embed src="${reader.result}" type="application/pdf" width="100%" height="400px" />`;
                }
            };
            reader.readAsDataURL(file);

            const deleteButton = document.createElement("button");
            deleteButton.innerHTML = "&times;";
            deleteButton.className = "delete-button";
            deleteButton.addEventListener("click", () => {
                pdfList.removeChild(pdfItem);
                const index = uploadedFiles.findIndex(item => item.pdfItem === pdfItem);
                if (index !== -1) {
                    uploadedFiles.splice(index, 1);
                }
                pdfInput.value = "";

                updateSelectedFileList();
            });

            pdfItem.appendChild(pdfPreview);
            pdfItem.appendChild(deleteButton);

            return pdfItem;
        }

        function formatFileSize(size) {
            const units = ['B', 'KB', 'MB', 'GB'];
            let index = 0;
            while (size >= 1024 && index < units.length - 1) {
                size /= 1024;
                index++;
            }
            return `${size.toFixed(2)} ${units[index]}`;
        }

        function updateSelectedFileList() {
            selectedFileList.innerHTML = "";

            for (const uploadedFile of uploadedFiles) {
                const listItem = document.createElement("li");
                const remarks = uploadedFile.remarks !== "" ? uploadedFile.remarks : "No Remarks";
                const fileSize = formatFileSize(uploadedFile.file.size);
                listItem.innerHTML = `${uploadedFile.file.name} (${fileSize}) - Remarks: ${remarks}`;
                
                selectedFileList.appendChild(listItem);
            }
        }

        var fdsetid = '@tabshift';
        var current_fs, next_fs, previous_fs;
        var opacity;
        var steps = $("fieldset").length;
        if (fdsetid == 3) {
            var fdset = "fieldset#" + "upload";
            editFormContainer.style.display = 'none';
            addFormContainer.style.display = 'block';
            $("#1").hide();
            $("#AddlDetails").addClass("active");
            $(fdset).show();

        }
        else if (fdsetid == 12) {
            var fdset = "fieldset#" + "uploaded";
            editFormContainer.style.display = 'block';
            addFormContainer.style.display = 'none';
            $("#5").hide();
            $("#AddlDetailsed").addClass("active");
            $(fdset).show();
        }
        else {
            var fdset = "fieldset#" + fdsetid + "";

        }

        $("#1").hide();
        $(fdset).show();

        $(".editButton").click(function () {
            $("#1").toggle();
        });

        current = $(fdset).index() + 1;
        setProgressBar(current);

        if (fdsetid == 1) {
            $("#BasicDetails").addClass("active");
        }
        else if (fdsetid == 2) {
            $("#BasicDetails").addClass("active");
            $("#AddlDetails").addClass("active");
        } else if (fdsetid == 3) {
            $("#BasicDetails").addClass("active");
            $("#AddlDetails").addClass("active");
            $("#Upload").addClass("active");

        } else if (fdsetid == 4) {
            $("#BasicDetails").addClass("active");
            $("#AddlDetails").addClass("active");
            $("#Upload").addClass("active");
            $("#confirm").addClass("active");
        }



        $(".next").click(function () {
            current_fs = $(this).parent();
            next_fs = $(this).parent().next();
            if (fdsetid == 5) {
                $("#BasicDetailsed").addClass("active");
            }
            else if (fdsetid == 6) {
                $("#AddlDetailsed").addClass("active");
            } else if (fdsetid == 7) {
                $("#Uploaded").addClass("active");
            } else if (fdsetid == 8) {
                $("#confirmed").addClass("active");
            }
            var isValid = true;
            current_fs.find("input[required]").each(function () {
                if ($(this).val() === "") {
                    isValid = false;
                    $(this).addClass("missing");
                    $(this).next(".error-message").text("This field is required.").show();
                } else {
                    $(this).removeClass("missing");
                    $(this).next(".error-message").hide();
                }
            });



            var selectedValueX = $("#ProjEdit_Apptype").val();
            var ProjEdit_HostTypeX = $("#ProjEdit_HostTypeID").val();

            var ProjEdit_HostTypeXE = $("#Hostedtype").val();
            var ddlAppTypeEditXE = $("#ddlAppTypeEdit").val();



            if ((selectedValueX !== "0" && ProjEdit_HostTypeX !== "0") || (ProjEdit_HostTypeXE !== "0" && ddlAppTypeEditXE !== "0")) {
                isValid = true;
            }
            else {
                isValid = false;

                Swal.fire({
                    title: 'Something Went Wrong....!',
                    text: 'App type or Hosting Type Not Selected .',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });

                e.preventDefault();


            }


            if (!isValid) {
                return;
            }

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
                duration: 500,
                complete: function () {
                    current_fs.hide();
                    next_fs.show();
                }
            });


            setProgressBar(++current);
        });

        $(".previous").click(function () {
            current_fs = $(this).parent();
            previous_fs = $(this).parent().prev();

            $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

            previous_fs.show();

            current_fs.animate({ opacity: 0 }, {
                step: function (now) {
                    opacity = 1 - now;

                    current_fs.css({
                        'display': 'none',
                        'position': 'relative'
                    });
                    previous_fs.css({ 'opacity': opacity });
                },
                duration: 300,
                complete: function () {
                    current_fs.hide();
                    previous_fs.show();
                }
            });

            setProgressBar(--current);
        });
        function animateProgressBar() {
            $("#upload-progress-bar").animate({ width: "100%" }, {
                duration: 50,
                complete: function () {

                }
            });
        }

        $("#submitUpload").click(function () {
            current_fs = $(this).parent();
            next_fs = $(this).parent().next();

            current_fs.animate({ opacity: 0 }, {
                step: function (now) {
                    opacity = 1 - now;
                    current_fs.css({
                        'display': 'none',
                        'position': 'relative'
                    });
                    next_fs.css({ 'opacity': opacity });
                },
                duration: 500,
                complete: function () {
                    current_fs.hide();
                    next_fs.show();
                }
            });

            $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");
            setProgressBar(++current);
        });



        $("#finaluploaded").click(function () {

            var fdset = "fieldset#" + "9";
            editFormContainer.style.display = 'block';
            addFormContainer.style.display = 'none';
            $("fieldset#6").hide();
            $("fieldset#7").hide();
            $("fieldset#uploaded").hide();

            $(fdset).show();

            animateProgressBar();


        });

        function setProgressBar(curStep) {
            var percent = parseFloat(100 / steps) * curStep;
            percent = percent.toFixed();
            $(".progress-bar")
                .css("width", percent + "%")
        }


        $(".submit").click(function () {
            return false;
        });

        $('#CurrentPslmId').val('0');
        $('#IsActive').val('True');
        var today = new Date();
        var year = today.getFullYear();
        var month = String(today.getMonth() + 1).padStart(2, '0');
        var day = String(today.getDate()).padStart(2, '0');
        var defaultCompletionDate = year + '-' + month + '-' + day;
        $('#DateTimeOfUpdate').val(defaultCompletionDate);
        $('#InitialRemark').val('New Project');
        $('#InitiatedDate').val(defaultCompletionDate);
        $('#IsWhitelisted').val('No');

        $("#next").click(function (e) {
            var selectedValuess = $("#ProjEdit_Apptype").val();
            var ProjEdit_HostTypess = $("#ProjEdit_HostTypeID").val();


            if (selectedValuess === "0" || ProjEdit_HostTypess === "0") {
                Swal.fire({
                    title: 'Something Went Wrong....!',
                    text: 'App type or Hosting Type Not Selected .',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });

                e.preventDefault(); // Prevent form submission
            }
        });

    });
   

            function submitFormnew() {


                var curPSMid = 0;

                if (@(Model.ProjEdit != null ? "true" : "false")) {
                    curPSMid = @(Model.ProjEdit?.CurrentPslmId ?? 0);

                }



                $.ajax({

                    type: 'POST',
                    url: 'FwdProjConfirm',
                    data: { "projid": curPSMid },
                    datatype: "json",

                    success: function (response) {
                        console.log('Request successful', response);
                    },
                    error: function (error) {
                        console.error('Error occurred:', error);
                    }
                });
            }

        $("#finalupload").click(function () {
            submitFormnew();
        var fdset = "fieldset#" + "4";
        editFormContainer.style.display = 'none';
        addFormContainer.style.display = 'block';
        $(fdset).show();
        $("fieldset#upload").hide();
        $("fieldset#2").hide();
        $("fieldset#1").hide();
        animateProgressBar();
        });

       
   
});
function formatDate(dateString) {
    const dateParts = dateString.split('T')[0].split('-');
    return `${dateParts[0]}-${dateParts[1]}-${dateParts[2]}`;
}

function setSelectedValue(selectElement, value) {
    for (let option of selectElement.options) {
        if (option.value == value) {
            option.selected = true;

            break;

        }

    }
}