$(document).ready(function () {

    initializeDataTable('#SoftwareType7');
    initializeDataTable('#mapunit');

    var UnitId = $("#Spnmodalunitid").html();

    //$("#ddlStage").change(function () {
    //    mMsaterStage(0, "ddlSubStage1", 6, $("#ddlStage").val(), 100);
    //});
    //$("#ddlSubStage1").change(function () {
    //    mMsater($("#ddlSubStage1").val(), "ddlAction1", 7, $("#ddlSubStage1").val());
    //});

    $(document).on('click', '.btn-editmapping', function () {
        var closestRow = $(this).closest("tr");
        var hiddenData = closestRow.find(".hiddenData");
        $('#SpnmodalStageid').val(hiddenData.find(".spanMappingStageId").text());
        $('#spanmodalStatusActionsMappingId').text(hiddenData.find(".spanStatusActionsMappingId").text());
        mMsater(hiddenData.find(".spanMappingStages").text(), "ddlStage", 5, 0);
        mMsaterStage(hiddenData.find(".spanMappingSubStages").text(), "ddlSubStage1", 6, hiddenData.find(".spanMappingStages").text(), 100);
        mMsater(hiddenData.find(".spanMappingActions").text(), "ddlAction1", 7, hiddenData.find(".spanMappingSubStages").text());
    });


    function fetchAndUpdateTable(StageId, StatusId) {
        $.ajax({
            type: 'POST',
            url: '/UnitDtls/GetMappingByUnitId',
            data: { StageId: StageId, StatusId: StatusId },
            dataType: "json",
            success: function (data) {
                var tableRows = '';
                for (var i = 0; i < data.length; i++) {
                    tableRows += '<tr>';
                    tableRows += '<td>' + data[i].stagesName + '</td>';
                    tableRows += '<td>' + data[i].subStagesName + '</td>';
                    tableRows += '<td>' + data[i].actionsName + '</td>';
                    tableRows += '<td class="hiddenData">' +
                        '<span class="spanStatusActionsMappingId">' + data[i].statusActionsMappingId + '</span>' +
                        '<span class="spanMappingStageId">' + data[i].stagesId + '</span>' +
                        '<span class="spanMappingSubStages">' + data[i].subStagesId + '</span>' +
                        '<span class="spanMappingActions">' + data[i].actionsId + '</span>' +
                        '</td>';
                    tableRows += '<td>' +
                        '<button class="btn btn-primary btn-editmapping">Edit</button>' +
                        '<button class="btn btn-primary btn-deletemapping">Delete</button>' +
                        '</td>';
                    tableRows += '</tr>';
                }
                $('#mappingdetails').html(tableRows);
            },
            error: function (error) {
                alert("Error fetching data");
            }
        });
    }
  

    $("#btn-Mapping").on('click', function () {
       
        $("#SpnmodalStageid").html($(this).closest("tr").find("#CurrentStageId").html());
        $("#SpnmodalStatusId").html($(this).closest("tr").find("#CurrentStatusId").html());

        var StageId = $("#SpnmodalStageid").html();
        var StatusId = $("#SpnmodalStatusId").html();

        mMsater(0, "ddlStage", 5, 0);

        $('#unitMapping').modal('show');
        $(".Fwdtitle").html("Status Mapping");

        fetchAndUpdateTable(StageId, StatusId);

        mMsater($(this).closest("tr").find("#CurrentStageId").html(), "ddlStage", 5, 0);
        mMsater($(this).closest("tr").find("#CurrentStatusId").html(), "ddlSubStage1", 6, $(this).closest("tr").find("#CurrentStageId").html());
        mMsater(1, "ddlAction1", 7, $(this).closest("tr").find("#CurrentStatusId").html());

        $('#unitMapping').modal('show');
        $(".Fwdtitle").html("Status Mapping");
    });

    $('#ddlSubStage1').select2();
    $('#ddlSubStageedit').select2();

    $('#btnsave').click(function () {
        var StageId = $('#SpnmodalStageid').html();
        var StagesId = $('#ddlStage').val();
        var SubStagesId = $('#ddlSubStage1').val();
        var ActionsId = $('#ddlAction1').val();
        var StatusActionsMappingId = $("#spanmodalStatusActionsMappingId").html();

        var data = {
            StatusActionsMappingId: StatusActionsMappingId,
            StageId: StageId,
            StagesId: StagesId,
            SubStagesId: SubStagesId,
            ActionsId: ActionsId
        };

        $.ajax({
            type: 'POST',
            url: '/UnitDtls/AddMapping',
            data: data,
            success: function (response) {
                if (response.message === "StatusActionsMapping data is already in the table") {
                    Swal.fire({
                        position: "top-mid",
                        icon: "error",
                        title: "Data is already in the table!",
                        showConfirmButton: true
                    });
                } else {
                    Swal.fire({
                        position: "top-mid",
                        icon: "success",
                        title: "Data saved successfully",
                        showConfirmButton: true
                    });

                    var StageId = $('#ddlStage').val();
                    var StatusId = $('#ddlAction1').val();
                    fetchAndUpdateTable(StageId, StatusId); // Refresh table data
                }
            },
            error: function (error) {
                alert("Error saving data");
            }
        });
    });

    $(document).on('click', '.btn-deletemapping', function () {
        var closestRow = $(this).closest("tr");
        var hiddenData = closestRow.find(".hiddenData");
        var StatusActionsMappingId = hiddenData.find(".spanStatusActionsMappingId").text();

        var data = {
            StatusActionsMappingId: StatusActionsMappingId
        };

        $.ajax({
            type: 'POST',
            url: '/UnitDtls/DeleteMapping',
            data: data,
            success: function (response) {
                if (response == 1) {
                    Swal.fire({
                        position: "top-mid",
                        icon: "success",
                        title: "Data deleted successfully",
                        showConfirmButton: true
                    });

                    var StageId = $('#SpnmodalStageid').html();
                    var StatusId = $('#SpnmodalStatusId').html();
                    fetchAndUpdateTable(StageId, StatusId); // Refresh table data
                } else {
                    Swal.fire({
                        position: "top-mid",
                        icon: "error",
                        title: "Something went wrong!",
                        showConfirmButton: true
                    });
                }
            },
            error: function (error) {
                alert("Error deleting data");
            }
        });
    });





    // =========================
    // Edit Mapping
    // =========================
    $(document).on('click', '.btn-editmapping', function () {

        var closestRow = $(this).closest("tr");
        var hiddenData = closestRow.find(".hiddenData");

        var stageId = hiddenData.find(".spanMappingStageId").text();
        var subStageId = hiddenData.find(".spanMappingSubStages").text();
        var actionId = hiddenData.find(".spanMappingActions").text();
        var mappingId = hiddenData.find(".spanStatusActionsMappingId").text();

        $('#SpnmodalStageid').text(stageId);
        $('#spanmodalStatusActionsMappingId').text(mappingId);

        mMsater(stageId, "ddlStage", 5, 0);
        mMsaterStage(subStageId, "ddlSubStage1", 6, stageId, 100);
        mMsater(actionId, "ddlAction1", 7, subStageId);
    });

    // =========================
    // Select2 Initialization
    // =========================
    $('#ddlStage').select2();
    $('#ddlSubStage1').select2();
    $('#ddlAction1').select2();

    // =========================
    // Save Mapping
    // =========================
    $('#btnsave').on('click', function () {

        var data = {
            StatusActionsMappingId: $('#spanmodalStatusActionsMappingId').text(),
            StageId: $('#SpnmodalStageid').text(),
            StagesId: $('#ddlStage').val(),
            SubStagesId: $('#ddlSubStage1').val(),
            ActionsId: $('#ddlAction1').val()
        };

        $.ajax({
            type: 'POST',
            url: '/UnitDtls/AddMapping',
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            success: function (response) {

                if (response.message === "StatusActionsMapping data is already in the table") {
                    Swal.fire({
                        icon: "error",
                        title: "Data is already in the table!",
                        confirmButtonText: "OK"
                    });
                } else {
                    Swal.fire({
                        icon: "success",
                        title: "Data saved successfully",
                        confirmButtonText: "OK"
                    });

                    var StageId = $('#SpnmodalStageid').text();
                    var StatusId = $('#SpnmodalStatusId').text();

                    fetchAndUpdateTable(StageId, StatusId);
                }
            },
            error: function () {
                Swal.fire({
                    icon: "error",
                    title: "Error saving data",
                    confirmButtonText: "OK"
                });
            }
        });
    });

    // =========================
    // Delete Mapping
    // =========================
    $(document).on('click', '.btn-deletemapping', function () {

        var mappingId = $(this)
            .closest("tr")
            .find(".spanStatusActionsMappingId")
            .text();

        $.ajax({
            type: 'POST',
            url: '/UnitDtls/DeleteMapping',
            data: JSON.stringify({ StatusActionsMappingId: mappingId }),
            contentType: 'application/json; charset=utf-8',
            success: function (response) {

                if (response === 1) {
                    Swal.fire({
                        icon: "success",
                        title: "Data deleted successfully",
                        confirmButtonText: "OK"
                    });

                    var StageId = $('#SpnmodalStageid').text();
                    var StatusId = $('#SpnmodalStatusId').text();

                    fetchAndUpdateTable(StageId, StatusId);
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Something went wrong!",
                        confirmButtonText: "OK"
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: "error",
                    title: "Error deleting data",
                    confirmButtonText: "OK"
                });
            }
        });
    });

    // ================================
    // Show Add Unit Modal
    // ================================


    $("#AddButon").on('click', function () {
        $('#UnitAdd').modal('show');
    });

});










// ================================
// Sync ddlSubStage with hidden input
// ================================
document.addEventListener('DOMContentLoaded', function () {

    var selectElement = document.getElementById('ddlSubStage');
    var hiddenInput = document.getElementById('hiddenFwdoffrs');

    if (selectElement && hiddenInput) {

        // Set initial value
        hiddenInput.value = selectElement.value;

        // Update hidden input on change
        selectElement.addEventListener('change', function () {
            hiddenInput.value = this.value;
        });
    }
});





// ================================
// Filter Unit Mapping (SINGLE FUNCTION)
// ================================
function filterUnitMapping(unitId) {

    const rows = document.querySelectorAll('#mapunit tbody tr');

    rows.forEach(function (row) {

        const span = row.querySelector('#spanMappingUnitId');

        if (!span) return;

        const mappingUnitId = parseInt(span.textContent.trim(), 10);

        if (mappingUnitId === unitId) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });

    // Show modal
    $('#unitMapping').modal('show');
}
