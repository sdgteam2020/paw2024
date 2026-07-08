$(document).ready(function () {
    $("#btnAuditlog").on("click", function () {
        OpenAuditHistory(this);
    });
});
function OpenAuditHistory(btn) {
    var ProjectId = btn.dataset.projid;
    $("#auditHistoryContainer").html(
        "<div class='text-center'>Loading...</div>"
    );
    
    $.ajax({
        url: '/AuditLog/Index',
        type: 'GET',
        data: { ProjId: ProjectId },
        success: function (response) {

            $("#auditHistoryContainer").html(response);

            var modal =
                new bootstrap.Modal(
                    document.getElementById('auditHistoryModal')
                );

            modal.show();
        },
        error: function () {

            $("#auditHistoryContainer").html(
                "<div class='alert alert-danger'>Unable to load audit history.</div>"
            );
        }
    });
}