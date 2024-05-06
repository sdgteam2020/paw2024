


function ValInData(input) {
    var regex = /[^a-zA-Z0-9/ ]/g;
    input.value = input.value.replace(regex, "");
}

$(document).ready(function () {
    $("#ddlUnitId").change(function () {
        var selectedMode = $(this).val();
    });
});
