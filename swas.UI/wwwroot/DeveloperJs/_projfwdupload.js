$(document).on("click", ".open-doc", function () {

    var documentTypeId = $(this).data("document-id");
    var projid = $(this).data("projid");

   
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


