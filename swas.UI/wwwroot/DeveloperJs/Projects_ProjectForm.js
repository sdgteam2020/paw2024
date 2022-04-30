$(document).on('ready', function () {

    populateddlStakeHolder();
    $('#CurrentPslmId').val('0');
    $('#IsActive').val('True');
    let today = new Date();
    let year = today.getFullYear();
    let month = String(today.getMonth() + 1).padStart(2, '0');
    let day = String(today.getDate()).padStart(2, '0');

    let defaultCompletionDate = year + '-' + month + '-' + day;


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