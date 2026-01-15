$(document).ready(function () {
    initializeDataTable('#SoftwareType');
    sessionStorage.setItem("spntabType", $("#spntabType").html());

    GetAllComments2();

    GetAllComments1();

    $("#btnAnalytics").click(function () {
        debugger;
        $('#ProjHoldHistory').modal('show');
        // alert($(this).closest("tr").find(".clsspnprojId").html())
        $(".lblProjHoldHistory").html($("#projectNameCell").html())
        $("#cardforProjHoldHistory").removeClass("d-none");
        GetProjHold($(".ProjectcommentprojId").html());
        ProjectWiseStatusByProjid($(".ProjectcommentprojId").html());
    });


});
function GetAllComments2() {
    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            "PsmId": 0,
            "stakeholderId": 1,
            "ProjId": $(".ProjectcommentprojId").html()
        },
        success: function (data) {
            var projectName = data?.[0]?.projectName || "";
            var adminap = data?.[0]?.adminApprovalStatus || "";

            var tableHTML = '<table class="table custom-table">';
            tableHTML += '<thead>';
            tableHTML += '<tr>';
            tableHTML += '<th class="table-header">Ser No</th>';
            tableHTML += '<th class="table-header">Stakeholder</th>';
            tableHTML += '<th class="table-header">Datetime</th>';
            tableHTML += '<th class="table-header">Comment</th>';
            tableHTML += '<th class="table-header">Status</th>';
            tableHTML += '<th class="table-header">PDF</th>';
            tableHTML += '<th class="d-none">DateType</th>';
            tableHTML += '</tr>';
            tableHTML += '</thead>';
            tableHTML += '<tbody>';

            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    tableHTML += '<tr>';
                    tableHTML += '<td class="table-cell">' + (i + 1) + '</td>';
                    tableHTML += '<td class="table-cell">' + data[i].stakeholder + ' (' + data[i].userDetails + ')</td>';
                    tableHTML += '<td class="table-cell">' + DateFormateddMMyyyyhhmmss(data[i].date) + '</td>';
                    tableHTML += '<td class="table-cell">' + data[i].comments + '</td>';

                    var statusClass = data[i].status === "Accepted" ? "badge-success" :
                        (data[i].status === "Obsn" ? "badge-warning" :
                            (data[i].status === "Info" ? "badge-info" : "badge-danger"));
                    tableHTML += '<td class="table-cell"><span class="badge ' + statusClass + '">' + data[i].status + '</span></td>';

                    tableHTML += '<td class="table-cell">';
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

            var table = $('#ChatBox .table').DataTable({
                paging: true,
                ordering: true,
                info: true,
                dom: '<"row"<"col-sm-2 col-md-1 add-comment-btn"><"col-sm-12 col-md-11"Bf>>rt<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                buttons: [
                    'copy',
                    'excel',
                    'csv',
                    {
                        text: 'PDF',
                        extend: 'pdfHtml5',
                        action: function (e, dt, node, config) {
                            PdfDiv();
                        }
                    }
                ]
            });

            // Insert the +Add Comment button inside the left container
            $('<button type="button" class="btn btn-primary">+Add Comment</button>')
                .appendTo('.add-comment-btn')
                .on('click', function () {
                    // Your custom +Add Comment button click logic here
                    alert('Add Comment clicked!');
                });

            function PdfDiv() {
                var popupWin = window.open('', '_blank', 'top=100,width=900,height=500,location=no');
                popupWin.document.open();

                var tableStyles = `
            <style type="text/css">
                table.custom-table {
                    width: 100%;
                    border-collapse: collapse;
                    margin-bottom: 20px;
                }
                thead {
                    vertical-align: bottom;
                    background-color: red;
                }
                th, td {
                    padding: 8px;
                    border: 1px solid #ddd;
                    text-align: center;
                }
                th {
                    background-color: #f2f2f2;
                    color: black;
                }
            </style>
        `;

                var filteredData = table.rows({ search: 'applied' }).data().toArray();

                var tableHTML = '<table>';

                tableHTML += '<thead><tr>';
                table.columns().header().each(function (header) {
                    tableHTML += '<th>' + header.innerHTML + '</th>';
                });
                tableHTML += '</tr></thead>';

                tableHTML += '<tbody>';
                for (var i = 0; i < filteredData.length; i++) {
                    tableHTML += '<tr>';
                    for (var j = 0; j < filteredData[i].length; j++) {
                        tableHTML += '<td>' + filteredData[i][j] + '</td>';
                    }
                    tableHTML += '</tr>';
                }
                tableHTML += '</tbody></table>';

                var ipadds = $("#ipAdd").html().replace(/\n/g, ' | ').trim();
                var watermarkText = ipadds;

                popupWin.document.write('<html><head>'
                    + tableStyles + '</head><body onload="window.print()">'
                    + tableHTML + `<div class="watermark">${watermarkText}</div></body></html>`);

                popupWin.document.close();
            }
        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}


function GetAllComments1() {

    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            "PsmId": 0,
            "stakeholderId": 1,
            "ProjId": $(".ProjectcommentprojId").html()
        },
        success: function (data) {
            //// Assuming the API returns PsmId as part of the response object
            //if (data && data.length > 0 && data[0].psmId !== undefined) {
            //    // Bind the PsmId to a hidden field
            //    $('#hiddenPsmId').val(data[0].psmId); // Ensure you have an input field with ID "hiddenPsmId"
            //}
            var tableHTML = '<table style="width:100%; border: 1px solid black; border-collapse:collapse;">';
            tableHTML += '<thead>';
            tableHTML += '<tr>';
            tableHTML += '<th style="width:5%;text-align:center;border:1px solid black;border-collapse:collapse">Ser No</th>';
            tableHTML += '<th style="text-align: center; border: 1px solid black;">Stakeholder</th>';
            tableHTML += '<th style="text-align: center; border: 1px solid black;">Datetime</th>';
            tableHTML += '<th style="text-align: center; border: 1px solid black;">Comment</th>';
            tableHTML += '<th style="text-align: center; border: 1px solid black;">Status</th>';
            tableHTML += '</tr>';
            tableHTML += '</thead>';
            tableHTML += '<tbody>';

            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    var date = new Date(data[i].date);
                    var formattedDate = ("0" + date.getDate()).slice(-2) + '-' +
                        ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                        date.getFullYear() + ' ' +
                        ("0" + date.getHours()).slice(-2) + ':' +
                        ("0" + date.getMinutes()).slice(-2) + ':' +
                        ("0" + date.getSeconds()).slice(-2);

                    tableHTML += '<tr>';
                    tableHTML += '<td style="border: 1px solid black;">' + (i + 1) + '</td>';
                    tableHTML += '<td style="border: 1px solid black;">' + data[i].stakeholder + '</td>';
                    tableHTML += '<td style="border: 1px solid black;">' + formattedDate + '</td>';
                    tableHTML += '<td style="border: 1px solid black;">' + data[i].comments + '</td>';
                    tableHTML += '<td style="border: 1px solid black;">' + (data[i].status == "Accepted" ? '<span class="badge badge-success text-white">' + data[i].status + '</span>' : '<span class="badge badge-danger text-white">' + data[i].status + '</span>') + '</td>';
                    tableHTML += '</tr>';
                }
            }

            tableHTML += '</tbody>';
            tableHTML += '</table>';

            $('#ChatBoxPDF').empty().html(tableHTML);

            $('#ChatBoxPDF .table').DataTable({
                "paging": false,
                "ordering": false,
                "info": false
            });
        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}


$(document).ready(function () {

    var TeamDetailPostBackURL = '/Projects/AttDetails';
    $(function () {
        /*$(".anchorDetail").click(function () {*/
        $(".anchorDetail").on("click", function () {
          
            var $buttonClicked = $(this);
            var id = $buttonClicked.attr('data-id');
            var options = { "backdrop": "static", keyboard: true };
            $.ajax({
                type: "GET",
                url: TeamDetailPostBackURL,
                contentType: "application/json; charset=utf-8",
                data: { "Id": id },
                datatype: "json",
                success: function (datadata) {

                    $('#myModalPagehistoryAttechment').modal('show');
                    $('#myModalContenthistoryAttechment').html(datadata);
                    /* $('#myModal').modal(options);*/


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


    function PrintDiv() {
        var projectName = document.getElementById('projectNameCell').innerText.trim(); // Get the project name from the table cell
        var divToPrint = document.getElementById('widget-content');
        var popupWin = window.open('', '_blank', 'width=800,height=800,location=Center,Center=20px');
        popupWin.document.open();
        var tableStyle = getComputedStyle(document.getElementById('htmlTopdf'));
        popupWin.document.write('<style type="text/css">');
        popupWin.document.write(tableStyle.cssText);
        popupWin.document.write('</style>');
        popupWin.document.write('<html><head><title>' + projectName + '</title></head><body onload="window.print()">' +
            '<div id="print-container">' +
            divToPrint.innerHTML +
            '</div>' +
            '<div style="transform: rotate(-45deg);z-index:10000;opacity: 0.3;color: BLACK; position:fixed;top: auto; left: 6%; top: 39%;color: #8e9191;font-size: 80px; font-weight: 500px;display: grid;justify-content: center;align-content: center;">' +
            '@(TempData["ipadd"])' + " " +
            '</div>' +
            '</body></html>');

        popupWin.document.close();
    }

    var loginData = @Html.Raw(Json.Serialize(ViewBag.logins));
    var projectid = $(".ProjectcommentprojId").html();
    ProjectWiseStatusByProjid(projectid);


