$(document).ready(function () {

    let  stakeholderId = parseInt($('#spnStakeholderid').text()) || 0;
    let  unitId = parseInt($('#hdnUnitId').val()) || 0;

  
    if ([1, 3, 4, 5,2].includes(unitId) || unitId === stakeholderId) {
        $('#commentDiv').show();
    }

        
       

   
   
    sessionStorage.setItem("spntabType", $("#spntabType").html());

    //GetAllComments2();
    getProjWiseStatus();
    //GetAllComments1();

    $("#btnAnalytics").click(function () {
       
        $('#ProjHoldHistory').modal('show');
        $(".lblProjHoldHistory").html($("#projectNameCell").html())
        $("#cardforProjHoldHistory").removeClass("d-none");
        GetProjHold($(".ProjectcommentprojId").html());
        ProjectWiseStatusByProjid($(".ProjectcommentprojId").html());
    });


       


});


$('#uploadfile').on('change', function () {

    const file = this.files[0];
    if (!file) return;
    if (file.type !== "application/pdf") {
        Swal.fire({
            title: 'Invalid File!',
            text: 'Only PDF files are allowed.',
            icon: 'error',
            confirmButtonText: 'OK'
        });

        $(this).val(''); // reset file input
    }
});

function getProjWiseStatus() {
    let projectId = $(".ProjectcommentprojId").html();
    if (projectId != null) {

    ProjectWiseStatusByProjid(projectId);
    }
}

function GetAllComments2() {
    let user_ids =
    {
        "PsmId": 0,

        "ProjId": $(".ProjectcommentprojId").html()
    }
   
    let encrypted_ids = encryptData(user_ids)
    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            encrypted_ids: encrypted_ids
        },
 
        success: function (data) {
            let projectName = data?.[0]?.projectName || "";
            let adminap = data?.[0]?.adminApprovalStatus || "";

            let tableHTML = '<table class="table custom-table">';
            tableHTML += '<thead>';
            tableHTML += '<tr>';
            tableHTML += '<th class="table-header">Ser No</th>';
            tableHTML += '<th class="table-header">Stakeholder</th>';
            tableHTML += '<th class="table-header">Datetime</th>';
            tableHTML += '<th class="table-header">Comment</th>';
            tableHTML += '<th class="table-header">Status</th>';
            tableHTML += '<th class="table-header noExport">PDF</th>';
            tableHTML += '<th class="d-none noExport">DateType</th>';
            tableHTML += '</tr>';
            tableHTML += '</thead>';
            tableHTML += '<tbody>';

            if (data != null) {
                for (let i = 0; i < data.length; i++) {
                    tableHTML += '<tr>';
                    tableHTML += '<td class="table-cell">' + (i + 1) + '</td>';
                    tableHTML += '<td class="table-cell">' + data[i].stakeholder + ' (' + data[i].userDetails + ')</td>';
                    tableHTML += '<td class="table-cell">' + DateFormateddMMyyyyhhmmss(data[i].date) + '</td>';
                    tableHTML += '<td class="table-cell">' + data[i].comments + '</td>';

                    let statusClass = data[i].status === "Accepted" ? "badge-success" :
                        (data[i].status === "Obsn" ? "badge-warning" :
                            (data[i].status === "Info" ? "badge-info" : "badge-danger"));
                    tableHTML += '<td class="table-cell"><span class="badge ' + statusClass + '">' + data[i].status + '</span></td>';

                    tableHTML += '<td class="table-cell noExport">';
                    if (data[i].state !== null && data[i].attpath !== null && data[i].attpath !== '') {
                        tableHTML += '<a href="/Home/WaterMark3?id=' + data[i].attpath + '" target="_blank">';
                        tableHTML += '<img src="/assets/images/icons/pdfimg.png" alt="PDF icon" class="pdf-icon">';
                        tableHTML += '</a>';
                    }
                    tableHTML += '<td class="d-none noExport"><span class="DateType">' + data[i].adminApprovalStatus + '</span></td>';
                    tableHTML += '</td>';
                    tableHTML += '</tr>';
                }
            }

            tableHTML += '</tbody>';
            tableHTML += '</table>';

            $('#ChatBox').empty().html(tableHTML);

            initializeDataTable("#ChatBox .table");
            if ($('.add-comment-btn button').length === 0) {
                $('<button type="button" class="btn btn-primary">+Add Comment</button>')
                    .appendTo('.add-comment-btn')
                    .on('click', function () {
                        $(this).attr("addminaproval", adminap);

                       
                        let approval = $(this).attr("addminaproval");

                        fetchServerDate().then(function (S) {
                            
                            let projId = $(".ProjectcommentprojId").html().trim();
                            $("#ProjectcommentForStackHolderprojId").html($(".ProjectcommentprojId").html())
                            $("#ProjectcommentForStackHolderPsmId").html($("#IsCommentPsmiId").html())
                            mMsater(0, "ddlStatus", 4, 0)
                            $("#ProjCommentModal").modal('show');
                            GetAllComments($("#IsCommentPsmiId").html(), $(".ProjectcommentprojId").html());
                            let words = projectName.split(" ");
                            let shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projectName;
                            let finalTitle = "Project Name: " + shortProjName;
                            $('#addComment').text(finalTitle);



                            if (approval === "true") {

                                $('#CommentDateFwd').attr('type', 'datetime-local');
                                $('#CommentDateFwd').attr('max', S.todayDateTime);
                                $('#CommentDateFwd').prop('disabled', false); // Allow user input
                                $('#CommentDateFwd').val(S.todayDateTime); // Allow user input
                            } else {

                                $('#CommentDateFwd').attr('type', 'datetime-local');
                                $('#CommentDateFwd').val(S.todayDateTime); // Set today's date
                                $('#CommentDateFwd').prop('disabled', true); // Freeze input
                            }

                        });

                    });
            }

           
        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}

function GetAllComments1() {
    let user_ids = {
        "PsmId": 0,
        "ProjId": $(".ProjectcommentprojId").html()
    };

    let encrypted_ids = encryptData(user_ids);

    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            encrypted_ids: encrypted_ids
        },
        success: function(data) {

            let tableHTML = `
                <div class="table-responsive">
                    <table class="table table-bordered comments-table">
                        <thead>
                            <tr>
                                <th class="col-ser-no">Ser No</th>
                                <th>Stakeholder</th>
                                <th class="col-datetime">Datetime</th>
                                <th>Comment</th>
                                <th class="col-status">Status</th>
                            </tr>
                        </thead>
                        <tbody>
            `;

            if (data && data.length > 0) {
                for (let i = 0; i < data.length; i++) {
                    let date = new Date(data[i].date);
                    let formattedDate =
                        ("0" + date.getDate()).slice(-2) + '-' +
                        ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                        date.getFullYear() + ' ' +
                        ("0" + date.getHours()).slice(-2) + ':' +
                        ("0" + date.getMinutes()).slice(-2) + ':' +
                        ("0" + date.getSeconds()).slice(-2);

                    tableHTML += `
                        <tr>
                            <td class="text-center">${i + 1}</td>
                            <td>${data[i].stakeholder || '-'}</td>
                            <td>${formattedDate}</td>
                            <td>${data[i].comments || '-'}</td>
                            <td class="text-center">${data[i].status || '-'}</td>
                        </tr>
                    `;
                }
            } else {
                tableHTML += `
                    <tr>
                        <td colspan="5" class="text-center text-muted">No comments found</td>
                    </tr>
                `;
            }

            tableHTML += `
                        </tbody>
                    </table>
                </div>
            `;

            $('#ChatBoxPDF').empty().html(tableHTML);

            // Optional: keep DataTable only for screen view
            if (data && data.length > 0) {
                $('#ChatBoxPDF .comments-table').DataTable({
                    paging: false,
                    ordering: false,
                    info: false,
                    searching: false,
                    autoWidth: false
                });
            }
        },
        error: function() {
            alert('Error fetching comments.');
        }
    });
}


$(document).ready(function () {

    let TeamDetailPostBackURL = '/Projects/AttDetails';
    $(function () {
        
        $(".anchorDetail").on("click", function () {
          
            let $buttonClicked = $(this);
            let id = $buttonClicked.attr('data-id');
            let options = { "backdrop": "static", keyboard: true };
            $.ajax({
                type: "GET",
                url: TeamDetailPostBackURL,
                contentType: "application/json; charset=utf-8",
                data: { "Id": id },
                datatype: "json",
                success: function (datadata) {

                    $('#myModalPagehistoryAttechment').modal('show');
                    $('#myModalContenthistoryAttechment').html(datadata);
                    


                },
                error: function () {
                    alert("Dynamic content load failed.");
                }
            });

        });

    });

    $(document).on('click', '.pdf', function () {
        $('#ViewRecordsHistory').modal('show');
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const pdfInput = document.getElementById("pdfInput");
    const pdfList = document.getElementById("pdfList");
    const selectedFileList = document.getElementById("selected-file-list");
    const remarksInput = document.getElementById("remarksInput");
    const uploadedFiles = [];

   
    $("#pdfInput").on("change", function (e) {
        handleFileUpload(e);
    });


    function handleFileUpload(event) {

        const files = event.target.files;
        for (const file of files) {
            const fileType = file.type;

            if (fileType.includes("image") || fileType === "application/pdf") {
                const pdfItem = createPDFItem(file);
                pdfList.appendChild(pdfItem);
                uploadedFiles.push({
                    file: file,
                    pdfItem: pdfItem,
                    remarks: ""
                });

                pdfInput.value = "";
                updateSelectedFileList();
            }
        }
    }

    function createPDFItem(file) {
        const pdfItem = document.createElement("div");
        pdfItem.className = "pdf-item";
        pdfItem.setAttribute("data-filename", file.name);

        const pdfPreview = document.createElement("div");
        pdfPreview.className = "pdf-preview";

        const reader = new FileReader();
        reader.onload = function () {
            const fileType = file.type;

            if (fileType.includes("image")) {
                pdfPreview.innerHTML = `<img src="${reader.result}" alt="Image Preview" class="image-preview">`;
            } else if (fileType === "application/pdf") {
                pdfPreview.innerHTML = `<embed src="${reader.result}" type="application/pdf" width="100%" height="100%" />`;
            }
        };
        reader.readAsDataURL(file);

        const deleteButton = document.createElement("button");
        deleteButton.innerHTML = "&times;";
        deleteButton.className = "delete-button";
        deleteButton.addEventListener("click", () => {
            pdfList.removeChild(pdfItem);
            const index = uploadedFiles.findIndex(item => item.pdfItem === pdfItem);
            if (index !== -1) {
                uploadedFiles.splice(index, 1);
            }
            pdfInput.value = "";

            updateSelectedFileList();
        });

        pdfItem.appendChild(pdfPreview);
        pdfItem.appendChild(deleteButton);

        return pdfItem;
    }

    function formatFileSize(size) {
        const units = ['B', 'KB', 'MB', 'GB'];
        let index = 0;
        while (size >= 1024 && index < units.length - 1) {
            size /= 1024;
            index++;
        }
        return `${size.toFixed(2)} ${units[index]}`;
    }

    function updateSelectedFileList() {
        const selectedFileList = document.getElementById("selectedFileList");
       
        if (selectedFileList) {
            selectedFileList.innerHTML = "";
        }
        for (const uploadedFile of uploadedFiles) {
            const listItem = document.createElement("li");
            const remarks = uploadedFile.remarks !== "" ? uploadedFile.remarks : "No Remarks";
            const fileSize = formatFileSize(uploadedFile.file.size);
            listItem.innerHTML = `<a href="#" class="popup-link">${uploadedFile.file.name} (${fileSize}) - Remarks: ${remarks}</a>`;
            selectedFileList.appendChild(listItem);
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        const popup = document.getElementById("popup");
        const closePopupButton = document.getElementById("closePopup");

        const popupLinks = document.querySelectorAll(".popup-link");
        popupLinks.forEach(link => {
            link.addEventListener("click", function (event) {
                event.preventDefault();
                popup.style.display = "block";
            });
        });

        closePopupButton.addEventListener("click", function () {
            popup.style.display = "none";
        });
    });



    $("#remarksInput").on("input", function () {

        if (uploadedFiles.length === 0) return; // safety

        const remarks = $(this).val();
        const lastFileIndex = uploadedFiles.length - 1;

        uploadedFiles[lastFileIndex].remarks = remarks;

        updateSelectedFileList();
    });

    updateSelectedFileList();
});

$(".btn-FwdFromMOv").on('click', function () {
  
    openForwardModal(this, true);
});


 
    $('.btncopy').on('click', function () {
        
        let text = $(this).siblings('.copyitem').text().trim();
        navigator.clipboard.writeText(text);
        alert("Text Copied");
    })




// function PrintDiv() {
  
//     const projectName =
//         document.getElementById('projectNameCell')?.innerText.trim() || 'Print';

//     const divToPrint = document.getElementById('widget-content');
//     const ipText = $('.IpAddress').text() || '';
   
//     const popupWin = window.open('', '_blank', 'width=800,height=800');
//     popupWin.document.open();

//     popupWin.document.write(`
//         <!DOCTYPE html>
//         <html>
//         <head>
//             <title>${projectName}</title>

//             <!-- Load SAME CSS file -->
//             <link rel="stylesheet" href="/css/projhistory.css">
   
//         </head>
//         <body>
//             <div id="print-container">
//                 ${divToPrint.innerHTML}
//             </div>

//             <div class="print-watermark">
//                 ${ipText}
//             </div>

//             <script src="/js/print.js"></script>
//         </body>
//         </html>
//     `);

//     popupWin.document.close();
// }


    //let projectid = $(".ProjectcommentprojId").html();
  

$('#btnGenerateAndOpenAppxC').on('click', function() {

    var $btn = $(this);
    var documentTypeId = $btn.data('document-id');
    var projid = $btn.data('projid');

    const projectName = document.getElementById('projectNameCell')?.innerText.trim() || 'Print';
    const divToPrint = document.getElementById('widget-content');
    const ipText = $('.IpAddress').text() || '';

    // Open one popup
    const popupWin = window.open('', '_blank', 'width=1100,height=900');
    popupWin.document.open();

    popupWin.document.write(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>${projectName}</title>
            <link rel="stylesheet" href="/css/projhistory.css">
            <style>
                body { margin: 20px; font-family: Arial, sans-serif; }
                .section-title { 
                    background: #072697; 
                    color: white; 
                    padding: 10px; 
                    margin-top: 30px;
                    font-size: 16px;
                    border-radius: 4px;
                }
                #generatedSection, #uploadedSection {
                    margin-bottom: 40px;
                }
                #Certificatepreview, #UploadedPdfPreview {
                    margin-top: 15px;
                }
            </style>
        </head>
        <body>
            <!-- ==================== GENERATED CHECKLIST ==================== -->
            <div id="generatedSection">
                <div class="section-title">Generated Checklist</div>
                <div id="print-container">
                    ${divToPrint.innerHTML}
                </div>
                <div class="print-watermark">${ipText}</div>
            </div>

            <!-- ==================== UPLOADED PDF ==================== -->
            <div id="uploadedSection">
                <div class="section-title">Uploaded Appx C</div>
                <div id="UploadedPdfPreview">
                    <p>Loading uploaded document...</p>
                </div>
            </div>

            <script src="/js/print.js"></script>
            <!-- Make sure pdf.js is available if renderPdfToCanvases needs it -->
        </body>
        </html>
    `);

    popupWin.document.close();

    // ========== Fetch Uploaded PDF and Render like Certificate ==========
    $.ajax({
        type: "GET",
        url: "/Projects/GetUploadedDocument",
        data: {
            projid: projid,
            DocumentTypeId: documentTypeId
        },
        success: function(response) {

            if (response.success && response.filePath) {

                // Fetch the actual PDF file as ArrayBuffer
                fetch("/uploads/" + response.filePath)
                    .then(res => res.arrayBuffer())
                    .then(arrayBuffer => {

                        // Convert to Uint8Array (same as your certificate code)
                        const byteNumbers = new Uint8Array(arrayBuffer);

                        // Clear loading text
                        const previewDiv = popupWin.document.getElementById('UploadedPdfPreview');
                        previewDiv.innerHTML = ''; // clear "Loading..."

                        // Create a container inside popup for canvases
                        const canvasContainer = popupWin.document.createElement('div');
                        canvasContainer.id = 'UploadedPdfCanvasContainer';
                        previewDiv.appendChild(canvasContainer);

                        // ========== Render using same method ==========
                        // Option 1: If renderPdfToCanvases can accept a target container
                        renderPdfToCanvasesInPopup(byteNumbers.buffer, canvasContainer, popupWin);

                    })
                    .catch(err => {
                        console.error(err);
                        popupWin.document.getElementById('UploadedPdfPreview').innerHTML =
                            `<p style="color:red;">Failed to load PDF file.</p>`;
                    });

            } else {
                popupWin.document.getElementById('UploadedPdfPreview').innerHTML =
                    `<p style="color:red;">Uploaded document not available.</p>`;
            }
        },
        error: function() {
            popupWin.document.getElementById('UploadedPdfPreview').innerHTML =
                `<p style="color:red;">Error while fetching document.</p>`;
        }
    });
});
function renderPdfToCanvasesInPopup(arrayBuffer, container, popupWindow) {

    // You need to adapt your existing renderPdfToCanvases logic here
    // Example structure (adjust according to your actual function):

    pdfjsLib.getDocument({ data: arrayBuffer }).promise.then(function(pdf) {

        container.innerHTML = ''; // clear

        for (let pageNum = 1; pageNum <= pdf.numPages; pageNum++) {

            pdf.getPage(pageNum).then(function(page) {

                const scale = 1.5;
                const viewport = page.getViewport({ scale: scale });

                const canvas = popupWindow.document.createElement('canvas');
                const context = canvas.getContext('2d');
                canvas.height = viewport.height;
                canvas.width = viewport.width;
                canvas.style.display = 'block';
                canvas.style.marginBottom = '20px';
                canvas.style.border = '1px solid #ccc';

                container.appendChild(canvas);

                page.render({
                    canvasContext: context,
                    viewport: viewport
                });
            });
        }
    });
}