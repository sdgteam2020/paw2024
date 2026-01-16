$(document).on('ready', function () {
    $('#Soft').on('click', '.MargeButton', function (e) {
        e.preventDefault();

        var projName = $(this).data('proj-name');
        var encyId = $(this).data('ency-id');

         var url = '/Documents/DocumentHistory?EncyID=' + encodeURIComponent(encyId) + 
             '&projId=' + encodeURIComponent(encyId) +
                   '&projName=' + encodeURIComponent(projName);

        window.location.href = url;
    });
});