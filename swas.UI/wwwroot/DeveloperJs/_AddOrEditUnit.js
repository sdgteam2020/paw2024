function populateCorpsDropdown(selectElement) {
    let selectedCommandId = $(selectElement).val();

    $.ajax({
        url: "/Ddl/ddlCorps",
        data: { Command: selectedCommandId },
        type: "GET",
        dataType: "json",
        success: function (result) {

            if (result.length > 0) {
                let list = "";


                list = '<option value="0">---- Select  ----</option>';

                for (let j = 0; j < result.length; j++) {


                    list += '<option value=' + result[j].corpsid + '>' + result[j].corpsname + '</option>';
                }



                $('#CorpsId').html(list);

                $('#CorpsId').val($('#addNewForm').html());
                $("#CorpsId").val("0");

            }
            else {
                let list = "";
                list = '<option value="0" selected="true">---- Select ----</option>';

                $('#CorpsId').html(list).selectedCommandId;

                $('#CorpsId').val($('#addNewForm').html());

                $("#CorpsId").val("0");

            }

        }
    });
};


function ValInData(input) {
    let regex = /[^a-zA-Z0-9/ ]/g;
    input.value = input.value.replace(regex, "");
}

$(document).ready(function () {
    $('.dropdownsearch').select2();
});