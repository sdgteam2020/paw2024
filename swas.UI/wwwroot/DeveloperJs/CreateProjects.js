
$(document).ready(function () {
    $(document).on('click', '.plus', function () {
        $('#UnitAdd').modal('show');
    });


    var initiatedDateInput = $("#ProjEdit_InitiatedDate");
    var completionDateInput = $("#ProjEdit_CompletionDate");

    var today = new Date();


    var formattedDate = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');


    $("#ProjEdit_InitiatedDate").val(formattedDate);

    var completionDateInput = document.getElementById("ProjEdit_CompletionDate");

    @if (Model.ProjEdit != null && Model.ProjEdit.CompletionDate.HasValue) {

        var completionDateValue = Model.ProjEdit.CompletionDate.Value.ToString("yyyy-MM-dd");
        @Html.Raw("ProjEdit_CompletionDate.value = '" + completionDateValue + "';")
    }

});

document.addEventListener('DOMContentLoaded', function () {

    var form = document.getElementById('msform');
    var submitButton = document.getElementById('submitUpload');
    submitButton.addEventListener('click', function (event) {
        event.preventDefault();
        form.submit();
    });
});


$(document).ready(function () {

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
            var fdsetn = "fieldset#" + "8";
            $(fdsetn).hide();




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


editAnchors.forEach((editAnchor) => {
    editAnchor.addEventListener('click', (event) => {
        event.preventDefault();
        const projName = editAnchor.getAttribute('data-proj-name');


        editFormContainer.style.display = 'block';
        addFormContainer.style.display = 'none';
    });
});


$(document).ready(function () {
    $("#envisagedCostInputa").on("input", function () {

        var inputValue = $(this).val();


        var numericValue = parseFloat(inputValue);


        if (numericValue < 0) {

            $(this).val('');

            Swal.fire({
                title: 'Negative Value Not Allowed',
                text: 'Please Enter Possitive Values...',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
});


$(document).ready(function () {
    // Check if it's a postback
    var isPostBack = '<%= Page.IsPostBack %>';

    if (isPostBack.toLowerCase() === 'true') {
        // If it's a postback, hide the table
        $('#SoftwareType1').hide();
    }
});


