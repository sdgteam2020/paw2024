$(document).on("click", ".open-doc", function () {

    let documentTypeId = $(this).data("document-id");
    let projid = $(this).data("projid");

   
    $.ajax({
        type: "GET",
        url: "/Projects/GetUploadedDocument",
        data: {
            projid: projid,
            DocumentTypeId: documentTypeId
        },
        success: function (response) {

            if (response.success) {
                // 🔥 Open PDF in new tab
                window.open("/uploads/" + response.filePath, "_blank");
            }
            else {
                alert("Document not uploaded yet.");
            }
        }
    });

});
$(document).ready(function () {

    initializeDataTable('#SoftwareType');

    GetAllComments2();

    GetAllComments1();
})

