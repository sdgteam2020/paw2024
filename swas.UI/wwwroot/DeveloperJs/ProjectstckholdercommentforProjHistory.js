$(document).ready(function () {
   
   
    initializeDataTable('#SoftwareType');
    sessionStorage.setItem("spntabType", $("#spntabType").html());

    GetAllComments2();
    getProjWiseStatus();
    GetAllComments1();

    $("#btnAnalytics").click(function () {
       
        $('#ProjHoldHistory').modal('show');
        $(".lblProjHoldHistory").html($("#projectNameCell").html())
        $("#cardforProjHoldHistory").removeClass("d-none");
        GetProjHold($(".ProjectcommentprojId").html());
        ProjectWiseStatusByProjid($(".ProjectcommentprojId").html());
    });


       


});

function getProjWiseStatus() {
    var projectId = $(".ProjectcommentprojId").html();
    if (projectId != null) {

    ProjectWiseStatusByProjid(projectId);
    }
}

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
            tableHTML += '<th class="table-header noExport">PDF</th>';
            tableHTML += '<th class="d-none noExport">DateType</th>';
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

                       
                        var approval = $(this).attr("addminaproval");

                        fetchServerDate().then(function (S) {
                            debugger;
                            var projId = $(".ProjectcommentprojId").html().trim();
                            $("#ProjectcommentForStackHolderprojId").html($(".ProjectcommentprojId").html())
                            $("#ProjectcommentForStackHolderPsmId").html($("#IsCommentPsmiId").html())
                            mMsater(0, "ddlStatus", 4, 0)
                            $("#ProjCommentModal").modal('show');
                            GetAllComments($("#IsCommentPsmiId").html(), $(".ProjectcommentprojId").html());
                            var words = projectName.split(" ");
                            var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projectName;
                            var finalTitle = "Project Name: " + shortProjName;
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

    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            "PsmId": 0,
            "stakeholderId": 1,
            "ProjId": $(".ProjectcommentprojId").html()
        },
        success: function (data) {

            var tableHTML = '<table class="comments-table">';
            tableHTML += '<thead>';
            tableHTML += '<tr>';
            tableHTML += '<th class="ser-no">Ser No</th>';
            tableHTML += '<th>Stakeholder</th>';
            tableHTML += '<th>Datetime</th>';
            tableHTML += '<th>Comment</th>';
            tableHTML += '<th>Status</th>';
            tableHTML += '</tr>';
            tableHTML += '</thead>';
            tableHTML += '<tbody>';

            if (data != null) {
                for (var i = 0; i < data.length; i++) {

                    var date = new Date(data[i].date);
                    var formattedDate =
                        ("0" + date.getDate()).slice(-2) + '-' +
                        ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                        date.getFullYear() + ' ' +
                        ("0" + date.getHours()).slice(-2) + ':' +
                        ("0" + date.getMinutes()).slice(-2) + ':' +
                        ("0" + date.getSeconds()).slice(-2);

                    tableHTML += '<tr>';
                    tableHTML += '<td>' + (i + 1) + '</td>';
                    tableHTML += '<td>' + data[i].stakeholder + '</td>';
                    tableHTML += '<td>' + formattedDate + '</td>';
                    tableHTML += '<td>' + data[i].comments + '</td>';

                    if (data[i].status == "Accepted")
                        tableHTML += '<td><span class="badge badge-success text-white">' + data[i].status + '</span></td>';
                    else
                        tableHTML += '<td><span class="badge badge-danger text-white">' + data[i].status + '</span></td>';

                    tableHTML += '</tr>';
                }
            }

            tableHTML += '</tbody>';
            tableHTML += '</table>';

            $('#ChatBoxPDF').empty().html(tableHTML);

            $('#ChatBoxPDF .comments-table').DataTable({
                paging: false,
                ordering: false,
                info: false
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

$('.PrintDiv').on('click', function () {
 
    PrintDiv();
})

function PrintDiv() {
    const projectName =
        document.getElementById('projectNameCell')?.innerText.trim() || 'Print';

    const divToPrint = document.getElementById('widget-content');
    const ipText = document.getElementById('IpAddress')?.innerText || '';

    const popupWin = window.open('', '_blank', 'width=800,height=800');
    popupWin.document.open();

    popupWin.document.write(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>${projectName}</title>

            <!-- Load SAME CSS file -->
            <link rel="stylesheet" href="/css/projhistory.css">
   
        </head>
        <body>
            <div id="print-container">
                ${divToPrint.innerHTML}
            </div>

            <div class="print-watermark">
                ${ipText}
            </div>

            <script src="/js/print.js"></script>
        </body>
        </html>
    `);

    popupWin.document.close();
}


    var projectid = $(".ProjectcommentprojId").html();
  

