$(document).on('ready', function () {
    $('#Soft').on('click', '.MargeButton', function (e) {
        e.preventDefault();

        let projName = $(this).data('proj-name');
        let encyId = $(this).data('ency-id');

         let url = '/Documents/DocumentHistory?EncyID=' + encodeURIComponent(encyId) + 
             '&projId=' + encodeURIComponent(encyId) +
                   '&projName=' + encodeURIComponent(projName);

        window.location.href = url;
    });
});