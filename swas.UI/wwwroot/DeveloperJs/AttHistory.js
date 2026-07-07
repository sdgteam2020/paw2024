let allAttachments = []; // Array to hold all attachments and remarks
async function getGeneratedPdfLogSignFromPreview() {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: "/Certificate/SignCertificate",
            type: "POST",
            xhrFields: {
                responseType: 'blob'
            },
            success: function (pdfBlob) {




                const blobUrl = URL.createObjectURL(pdfBlob);
                window._currentPdfBlobUrl = blobUrl; // store for download

                // Convert Blob → ArrayBuffer → pass to renderPdfToCanvases
                const reader = new FileReader();
                reader.onload = function (e) {
                    renderPdfToCanvases(e.target.result); // e.target.result is ArrayBuffer
                };
                reader.readAsArrayBuffer(pdfBlob);

                $('#btnDigitalsign').prop('disabled', true);
                const file = new File([pdfBlob], "GeneratedCertificate.pdf", {
                    type: "application/pdf"
                });
               
                resolve(file);
            },
            error: function () {
                alert("Signing failed");
                reject("Signing failed");
            }
        });
    });
}



async function getGeneratedPdfFromPreview() {
    if (!generatedPdfBlob) {
        return null;
    }

    return new File(
        [generatedPdfBlob],
        "GeneratedCertificate.pdf",
        { type: "application/pdf" }
    );
}

function AttOnFWD() {
    $('.uploadLoader').addClass('d-none')
    
    let listItem = "";
    if ($.trim($("#AttBody").text()) === "No Record Found") {
        $("#AttBody").empty(); // remove placeholder row
    }



    const input = document.getElementById("pdfFileInput");
    const files = input?.files || [];
    const remarksVal = $("#Remarks").val() ?? $("#Reamarks").val() ?? "";

    if (!files.length) {
        alert("Please choose at least one PDF.");
        return;
    }
    const fd = new FormData();
    fd.append("remarks", remarksVal); // Add remarks to FormData
    attachments = [];
    for (let i = 0; i < files.length; i++) {
        fd.append("uploadfile[]", files[i]);  // Append files to FormData
        attachments.push({ file: files[i], remarks: remarksVal }); // Track the files and remarks
    }
    allAttachments.push(...attachments);
    for (let i = 0; i < files.length; i++) {
        const f = files[i];
        const tempUrl = URL.createObjectURL(f);

        listItem += "<tr>";
        listItem += "<td class='align-middle'>" +
            "<button type='button' class='att-btnDelete btn-icon btn-round btn-danger mr-1'>" +
            "<i class='fas fa-trash-alt'></i>" +
            "</button>" +
            "</td>";

        listItem += "<td class='align-middle RefLetter-container'>" +
            "<span>" + trimByWords(remarksVal, 4) + "</span>" +
            "<div class='RefLetter'>" + remarksVal + "</div>" +
            "</td>";

        listItem += "<td class='align-middle RefLetter-container'>" +
            "<span>" +
            "<a class='link-success' href='" + tempUrl + "' target='_blank'>" + trimByWords(f.name, 4) + "</a>" +
            "</span>" +
            "<div class='RefLetter'>" + f.name + "</div>" +
            "</td>";

        listItem += "<td class='align-middle'><span>" +
            new Date().toLocaleString() +
            "</span></td>";

        listItem += "</tr>";
    }
    $("#Reamarks").val("");
    $("#pdfFileInput").val("");
    $("#AttBody").append(listItem);
    $(".btnFwdConfirm").off().on("click", async function () {
        
       
        const urlParams = new URLSearchParams(window.location.search);
        let psmid;

        if (urlParams.get('Type') === 'XRDC') {
            psmid = urlParams.get('psmid');
        } else {
            psmid = $("#spanFwdCurrentPslmId").html();
        }
        let ddlaction = $("#ddlfwdAction option:selected").text();
        let generatedPdf = null;
        if (ddlaction === "Approved / Completed" && $('#ddlfwdStage').val() == 3) {
            generatedPdf = await getGeneratedPdfLogSignFromPreview();
        }
        SaveFwdTo(psmid, generatedPdf, allAttachments);
    });


}

//function sendPDFToServer(pdfpath, thumbprint) {
//    debugger;

//    $.ajax({
//        url: 'https://dgisapp.army.mil:55102/Temporary_Listen_Addresses/ByteDigitalSignAsync',
//        type: 'POST',
//        contentType: 'application/json',
//        dataType: 'json',
//        data: JSON.stringify([{
//            Thumbprint: thumbprint,
//            pdfpath: pdfpath,
//            XCoordinate: "470",
//            YCoordinate: "740",
//            Page: "1",
//            CustomText: "Digital Signature"
//        }]),
//        skipAntiForgery: true,
//        success: function (response) {
//            debugger;
//            $('.uploadLoader').addClass('d-none')
//            if (response.Message != '') {
//                Swal.fire({
//                    title: "Application Approved",
//                    text: "Application has been digitally signed successfully.",
//                    icon: "success",
//                    confirmButtonText: "OK",
//                    customClass: {
//                        popup: 'swal-success-theme',
//                        confirmButton: 'swal-confirm-green'
//                    },
//                    buttonsStyling: false
//                }).then(async () => {  // <-- async here 

//                    if (response.Message == "Token Expired !") {
//                        Swal.fire({
//                            title: "Application Not Approved",
//                            text: response.Message,
//                            icon: "warning",
//                            confirmButtonText: "OK",
//                            customClass: {
//                                popup: 'swal-danger-theme',
//                                confirmButton: 'swal-confirm-danger'
//                            },

//                        });
//                    }
               
//                    if (!response) {
//                        Swal.fire({ icon: "error", title: "Oops...", text: "Certificate not Generated" });
//                        return;
//                    }

//                    try {


//                        const base64String = response.Message.replace(/\s/g, '').replace(/-/g, '+').replace(/_/g, '/');
                      



//                        const byteCharacters = atob(base64String);
//                        const byteNumbers = new Uint8Array(byteCharacters.length);

//                        for (let i = 0; i < byteCharacters.length; i++) {
//                            byteNumbers[i] = byteCharacters.charCodeAt(i);
//                        }

//                        //const pdfBlob = new Blob([byteNumbers], { type: "application/pdf" });
//                        generatedPdfBlob = new Blob([byteNumbers], { type: "application/pdf" });
//                        window._currentPdfBlobUrl = URL.createObjectURL(generatedPdfBlob);

//                        // ✅ Ensure container exists before rendering
//                        if (!$('#Certificatepreview').length) {
//                            console.error("Container not found");
//                            return;
//                        }

//                        renderPdfToCanvases(byteNumbers.buffer);

//                    } catch (err) {
//                        console.error("PDF render error:", err);
//                        Swal.fire({ icon: "error", title: "Error", text: "Failed to render PDF" });
//                    }

//                    $('#btnLogSign').attr('disabled', true);
//                    $('#btnDigitalsign').attr('disabled', true);
                 
//                    const urlParams = new URLSearchParams(window.location.search);
//                    let psmid;
                   
//                    if (urlParams.get('Type') === 'XRDC') {
//                        psmid = urlParams.get('psmid');
//                    } else {
//                        psmid = $("#spanFwdCurrentPslmId").html();
//                    }

//                    let ddlaction = $("#ddlfwdAction option:selected").text();
//                    let generatedPdf = null;

//                    if (ddlaction === "Approved / Completed" && $('#ddlfwdStage').val() == 3) {
//                        generatedPdf = await getGeneratedPdfFromPreview(); // now works
//                    }
                 
//                    SaveFwdTo(psmid, generatedPdf, allAttachments);
//                });
//            } else {
//                Swal.fire({
//                    title: "Error!",
//                    text: "Failed to sign PDF.",
//                    icon: "error"
//                });
//            }
//        },
//        error: function (error) {
//            console.error('Error sending PDF:', error);
//        }
//    });

//}
function sendPDFToServer(pdfpath, thumbprint) {

    debugger;
    $.ajax({
        url: 'https://dgisapp.army.mil:55102/Temporary_Listen_Addresses/ByteDigitalSignAsync',
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify([{
            Thumbprint: thumbprint,
            pdfpath: pdfpath,
            XCoordinate: "470",
            YCoordinate: "740",
            Page: "1",
            CustomText: "Digital Signature"
        }]),
        skipAntiForgery: true,
        success: function (response) {
            debugger;
            $('.uploadLoader').addClass('d-none')
            if (!response) {
                Swal.fire({ icon: "error", title: "Oops...", text: "Certificate not Generated" });
                return;
            }
            if (response.Valid == true) {
                Swal.fire({
                    title: "Application Approved",
                    text: "Application has been digitally signed successfully.",
                    icon: "success",
                    confirmButtonText: "OK",
                    customClass: {
                        popup: 'swal-success-theme',
                        confirmButton: 'swal-confirm-green'
                    },
                    buttonsStyling: false
                }).then(async () => {  
                    try {

                        const base64String = response.Message.replace(/\s/g, '').replace(/-/g, '+').replace(/_/g, '/');

                        const byteCharacters = atob(base64String);
                        const byteNumbers = new Uint8Array(byteCharacters.length);

                        for (let i = 0; i < byteCharacters.length; i++) {
                            byteNumbers[i] = byteCharacters.charCodeAt(i);
                        }

                        //const pdfBlob = new Blob([byteNumbers], { type: "application/pdf" });
                        generatedPdfBlob = new Blob([byteNumbers], { type: "application/pdf" });
                        window._currentPdfBlobUrl = URL.createObjectURL(generatedPdfBlob);

                        // ✅ Ensure container exists before rendering
                        if (!$('#Certificatepreview').length) {
                            console.error("Container not found");
                            return;
                        }

                        renderPdfToCanvases(byteNumbers.buffer);

                    } catch (err) {
                        console.error("PDF render error:", err);
                        Swal.fire({ icon: "error", title: "Error", text: "Failed to render PDF" });
                    }

                    $('#btnLogSign').attr('disabled', true);
                    $('#btnDigitalsign').attr('disabled', true);

                    const urlParams = new URLSearchParams(window.location.search);
                    let psmid;

                    if (urlParams.get('Type') === 'XRDC') {
                        psmid = urlParams.get('psmid');
                    } else {
                        psmid = $("#spanFwdCurrentPslmId").html();
                    }

                    let ddlaction = $("#ddlfwdAction option:selected").text();
                    let generatedPdf = null;

                    if (ddlaction === "Approved / Completed" && $('#ddlfwdStage').val() == 3) {
                        generatedPdf = await getGeneratedPdfFromPreview(); // now works
                    }

                    SaveFwdTo(psmid, generatedPdf, allAttachments);
                });
            } else if (response.Message == "Token Expired !" || response.Valid == false) {
                Swal.fire({
                    title: "Application Not Approved",
                    text: response.Message,
                    icon: "warning",
                    confirmButtonText: "OK",
                    customClass: {
                        popup: 'swal-danger-theme',
                        confirmButton: 'swal-confirm-danger'
                    },

                });
            }
           
        },
        error: function (error) {
            console.error('Error sending PDF:', error);
        }
    });

}

$(document).on("click", ".att-btnDelete", function () {

    Swal.fire({
        title: 'Are you sure?',
        text: "You want to Delete ",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#072697',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Delete It!'
    }).then((result) => {
        if (result.value) {
            let rowIndex = $(this).closest("tr").index();
            allAttachments.splice(rowIndex, 1);
            $(this).closest("tr").remove();

        }
    });
   
});

function UploadFiles() {
    let formData = new FormData();
    let totalFiles = document.getElementById("pdfFileInput").files.length;
    // 🔥 Get DocumentTypeId from active tracker step
    let documentTypeId = $(".stepsforatt .step.active").data("document-id");

    if (!documentTypeId) {
        alert("Invalid document step.");
        return;
    }
    for (let i = 0; i < totalFiles; i++) {
        let file = document.getElementById("pdfFileInput").files[i];
        formData.append("uploadfile", file);
        formData.append("Reamarks", $("#Reamarks").val());
        formData.append("PsmId", $("#spanCurrentPslmId").html());
       
    }
    // ✅ Send Foreign Key instead of Remarks
    formData.append("DocumentTypeId", documentTypeId);

    $.ajax({
        type: "POST",
        url: '/Projects/UploadMultiFile',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $('.uploadLoader').addClass('d-none');
            $('#uploadLoader').hide();

            if (response == 1) {
              
                AttechHistory();
                //setTimeout(function(){
                //    trackerIndex();
                   

                //}, 1000)

                $("#Reamarks").val("");
                $("#pdfFileInput").val("");
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Upload success",
                    showConfirmButton: false,
                    timer: 1500
                });
            } else if (response == -2) {

                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Only Pdf File Upload!",

                });
            }
            else if (response == -5) {

                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Pdf File LessThen 10 MB !",

                });
            }

        },
        error: function (error) {
            $('#uploadLoader').hide();
            $(".error-msg").removeClass("d-none")
            $("#error-msg").html("Somthing is wrong");;

        }
    });
}

function AttechHistory() {
    let listItem = "";
    let userdata =
    {
        "PslmId": $("#spanCurrentPslmId").html(),

    };
    $.ajax({
        url: '/Projects/GetAtthHistoryByProjectId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {

            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({
                        text: ""
                    });
                }
                else if (response == 0) {

                    listItem += "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";
                    syncTrackerWithResponse([]);
                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }

                else {
                    for (let i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'>" +
                            "<span id='spnattId'>" + response[i].attId + "</span>" +
                            "<span id='spnpsmId'>" + response[i].psmId + "</span>" +
                            "</td>";
                        listItem += "<td class='align-middle'>" +
                            "<span id='btnedit'>" +
                            "<button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'>" +
                            "<i class='fas fa-trash-alt'></i>" +
                            "</button>" +
                            "</span>" +
                            "</td>";
                        let breakRemarks = response[i].reamarks || "";
                        let formatedName = trimByWords(breakRemarks, 4);

                        listItem += "<td class='align-middle RefLetter-container'>" +
                            "<span id='comdName'>" + formatedName + "</span>" +
                            "<div class='RefLetter'>" + breakRemarks + "</div>" +
                            "</td>";
                        listItem += "<td class='align-middle RefLetter-container'>" +
                            "<span id='corpsName'>" +
                            "<a class='link-success' target='_blank' href='/uploads/" + response[i].attPath + "'>" +
                            trimByWords(response[i].actFileName, 4) +
                            "</a>" +
                            "</span>" +
                            "<div class='RefLetter'>" + response[i].actFileName + "</div>" +
                            "</td>";
                        listItem += "<td class='align-middle'><span id='divName'>" + DateFormateddMMyyyyhhmmss(response[i].timeStamp) + "</span></td>";

                        listItem += "</tr>";
                    }

                    $("#DetailBody").html(listItem);
                    syncTrackerWithResponse(response);
                    $("#lblTotal").html(response.length);
                  





                    $("body").on("click", ".cls-btnDelete", function () {

                        Swal.fire({
                            title: 'Are you sure?',
                            text: "You want to Delete ",
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonColor: '#072697',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Yes, Delete It!'
                        }).then((result) => {
                            if (result.value) {

                                Deleteattechment($(this).closest("tr").find("#spnattId").html());

                            }
                        });
                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";
                $("#SoftwareTypes").DataTable().destroy();
                $("#DetailBody").html(listItem);
                $("#lblTotal").html(0);
            }
        },
        error: function (result) {
            Swal.fire({
                text: ""
            });
        }
    });
}
function Deleteattechment(AttechId) {
    $.ajax({
        url: '/Projects/DeleteAttech',
        type: 'POST',
        data: { "AttechId": AttechId },
        success: function (response) {


            if (response == 1) {
             
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Record Deleted successfully',
                    showConfirmButton: false,
                    timer: 1500
                });

                AttechHistory();

            }

        }
    });
}