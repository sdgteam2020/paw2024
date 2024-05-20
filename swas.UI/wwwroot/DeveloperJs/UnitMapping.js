$(document).ready(function () {

    $("#ddlStage").change(function () {

        mMsaterStage(0, "ddlSubStage", 6, $("#ddlStage").val(), 0)
    });
    $("#ddlSubStage").change(function () {

        mMsater($("#ddlSubStage").val(), "ddlAction", 7, $("#ddlSubStage").val())
    });

    $(document).on('click', '.btn-editmapping', function () {
        var closestRow = $(this).closest("tr");
        var hiddenData = closestRow.find(".hiddenData");

        alert("test");

        alert(hiddenData.find(".spanStatusActionsMappingId").text());
        alert(hiddenData.find(".spanUnitStatusMappingId").text());

        alert(hiddenData.find(".spanMappingUnitId").text());

        $('#UnitName').val(closestRow.find("#mapingUnit").text());
        $('#Spnmodalunitid').val(hiddenData.find(".spanMappingUnitId").text());
        $('#spanmodalStatusActionsMappingId').text(hiddenData.find(".spanStatusActionsMappingId").text());
        $('#spanmodalUnitStatusMappingId').text(hiddenData.find(".spanUnitStatusMappingId").text());

        mMsater(hiddenData.find(".spanMappingStages").text(), "ddlStage", 5, 0);
        alert(hiddenData.find(".spanMappingSubStages").text());
        mMsaterStage(hiddenData.find(".spanMappingSubStages").text(), "ddlSubStage", 6, hiddenData.find(".spanMappingStages").text(), 100);

        mMsater(hiddenData.find(".spanMappingActions").text(), "ddlAction", 7, hiddenData.find(".spanMappingSubStages").text());
    });

    $(".btn-Mapping").click(function () {
        $("#Spnmodalunitid").html($(this).closest("tr").find("#Spnunitid").html());
        var UnitId = $("#Spnmodalunitid").html();
        var table;
        alert(UnitId)

        $.ajax({
            type: 'POST',
            url: '/UnitDtls/GetMappingByUnitId',
            data: { UnitId: UnitId },
            dataType: "json",
            success: function (data) {
                if (table) {
                    table.destroy();
                }
                if (data === 0) {
                    location.reload();
                }
                var tableRows = '';
                for (var i = 0; i < data.length; i++) {
                    tableRows += '<tr>';
                    tableRows += '<td id="mapingUnit">' + data[i].unit + '</td>';
                    tableRows += '<td>' + data[i].stagesName + '</td>';
                    tableRows += '<td>' + data[i].subStagesName + '</td>';
                    tableRows += '<td>' + data[i].actionsName + '</td>';
                    tableRows += '<td class="hiddenData">' +
                        '<span class="spanStatusActionsMappingId">' + data[i].statusActionsMappingId + '</span>' +
                        '<span class="spanUnitStatusMappingId">' + data[i].unitStatusMappingId + '</span>' +
                        '<span class="spanMappingUnitId">' + data[i].unitId + '</span>' +
                        '<span class="spanMappingStages">' + data[i].stages + '</span>' +
                        '<span class="spanMappingSubStages">' + data[i].subStages + '</span>' +
                        '<span class="spanMappingActions">' + data[i].actions + '</span>' +
                        '</td>';
                    tableRows += '<td>' +
                        '<button class="btn btn-primary btn-editmapping">Edit</button>' +
                        '<button class="btn btn-primary btn-deletemapping">Delete</button>' +
                        '</td>';
                    tableRows += '</tr>';
                }
                $('#mappingdetails').empty().html(tableRows);
            },
            error: function (error) {
                // Handle error
                alert("Error saving data");
            }
        });





    $("#Spnmodalunitid").html($(this).closest("tr").find("#Spnunitid").html());
    $("#UnitName").val($(this).closest("tr").find("#SpnUnitName").html());
    mMsater(0, "ddlStage", 5, 0)

    $('#unitMapping').modal('show');
    $(".Fwdtitle").html("Unit & Status Mapping");

});


    // Apply Select2 to the dropdown with ID "ddlSubStage"
    $('#ddlSubStage').select2();
    $('#ddlSubStageedit').select2();

    $('#btnsave').click(function () {

        

        var UnitId = $('#Spnmodalunitid').html();
        var stage = $('#ddlStage').val();
        var subStage = $('#ddlSubStage').val();
        var action = $('#ddlAction').val();




        var data = {
            StatusActionsMappingId: $("#spanmodalStatusActionsMappingId").html(),
            UnitStatusMappingId: $("#spanmodalUnitStatusMappingId").html(),
            UnitId: UnitId,
            Stages: stage,
            SubStages: subStage,
            Actions: action
        };


        $.ajax({
            type: 'POST',
            url: '/UnitDtls/AddMapping',
            data: data,

            success: function (response) {
                if (response.message === "StatusActionsMapping data is already in the table") {
                    alert("Error", "data is already in the table", "error");
                } else {
                    alert("Success", "Data saved successfully", "success");
                }
            },
            error: function (error) {
                // Handle error
                alert("Error", "Error saving data", "error");
            }
        });
    });

    $(document).on('click', '.btn-deletemapping', function () {
        debugger;

        $('#Spnmodalunitid').val($(this).closest("tr").find("#spanMappingUnitId").html());
        $('#spanmodalStatusActionsMappingId').html($(this).closest("tr").find("#spanStatusActionsMappingId").html());
        $('#spanmodalUnitStatusMappingId').html($(this).closest("tr").find("#spanUnitStatusMappingId").html());

        var data = {
            StatusActionsMappingId: $("#spanmodalStatusActionsMappingId").html(),
            UnitStatusMappingId: $("#spanmodalUnitStatusMappingId").html(),
            
        };

         $.ajax({
            type: 'Post',
             url: '/UnitDtls/DeleteMapping',
             data: data,

             success: function (response) {
                 if (response == 1)
                     alert('Data Delete successfully');
                 else {

                 }
             },
            error: function (error) {
                alert(error);
            }
        });



    });
});

