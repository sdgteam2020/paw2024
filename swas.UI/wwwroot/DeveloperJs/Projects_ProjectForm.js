$(document).on('ready', function () {

    populateddlStakeHolder();
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

    $('#MyFormSubmit').DataTable();

    $('#cancelButton').on('click', function () {
        showAddNewForms();
    });
});
function showAddNewForms() {
    $('#addNewFormContainer').addClass('hidden');
    $('#addNewFormContainer').hide();

}