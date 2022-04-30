
$(function () {
    ProjectWiseStatus();
   
});

function ProjectWiseStatus() {

    const userdata = { "Id": 0 };

    $.ajax({
        url: '/Home/GetProjectWiseStatus',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {

            if (!isValidResponse(response)) return;

            const { statusProjectlst, movProjectlst } = response;

            const headerHtml = buildTableHeader(statusProjectlst);
            const bodyHtml = buildTableBody(statusProjectlst, movProjectlst);

            $("#tblProjectWiseStatus").html(headerHtml + bodyHtml);

            bindProjectClick();

            initializeDataTable('#tblProjectWiseStatus');
        },

        error: function () {
            Swal.fire({ text: "" });
        }
    });
}
function isValidResponse(response) {
    if (response === "null" || response === null) return false;

    if (response === -1) {
        Swal.fire({ text: "" });
        return false;
    }

    return response !== 0;
}
function buildTableHeader(StatusProjectlst) {

    let html = `
        <thead>
            <tr>
                <th class="d-none noExport"></th>
                <th class="text-center">Ser No</th>
                <th>Project Name</th>
    `;

    StatusProjectlst.forEach(item => {
        if (!isValidStatus(item.status)) return;
        html += `<th>${item.status}</th>`;
    });

    html += `
            </tr>
        </thead>
    `;

    return html;
}
function buildTableBody(StatusProjectlst, MovProjectlst) {

    let html = '<tbody id="bodyProjectWiseStatus">';
    let count = 1;
    let projIdTracker = 0;

    MovProjectlst.forEach(item => {

        if (projIdTracker === item.projId) return;

        projIdTracker = item.projId;

        html += buildProjectRow(item, StatusProjectlst, MovProjectlst, count);
        count++;
    });

    html += '</tbody>';
    return html;
}
function buildProjectRow(project, StatusProjectlst, MovProjectlst, count) {

    let html = `
        <tr>
            <td class="clsspnprojId d-none noExport">${project.projId}</td>
            <td class="align-middle text-center">${count}</td>
            <td class="RefLetter-container btn-clsprojName">
                <div class="tooltip-container noExport">
                    ${trimByWords(project.projName, 5)}
                </div>
                <div class="RefLetter projnameforlabel">
                    ${project.projName}
                </div>
            </td>
    `;

    StatusProjectlst.forEach(statusItem => {

        if (!isValidStatus(statusItem.status)) return;

        html += buildStatusCell(project, statusItem, MovProjectlst);
    });

    html += '</tr>';

    return html;
}
function buildStatusCell(project, statusItem, MovProjectlst) {

    const match = MovProjectlst.filter(function (element) {
        return element.statusId == statusItem.statusId &&
            element.projId == project.projId;
    });

    if (match.length != 0) {

        const time = DateFormateddMMyyyyhhmmss(match[0].timeStamp);

        return '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="' + time + '">' +
            '<div class="pws-ok-dot">✔</div>' +
            '<span class="d-none">' + time + '</span>' +
            '</td>';
    }
    else {

        return '<td class="align-middle text-center">' +
            '<img src="/assets/images/icons/Cross_red_circle.png" width="22" height="22" alt="Readed">' +
            '</td>';
    }
}
function buildProjectRow(project, StatusProjectlst, MovProjectlst, count) {

    let html = `
        <tr>
            <td class="clsspnprojId d-none noExport">${project.projId}</td>
            <td class="align-middle text-center">${count}</td>
            <td class="RefLetter-container btn-clsprojName">
                <div class="tooltip-container noExport">
                    ${trimByWords(project.projName, 5)}
                </div>
                <div class="RefLetter projnameforlabel">
                    ${project.projName}
                </div>
            </td>
    `;

    StatusProjectlst.forEach(statusItem => {

        if (!isValidStatus(statusItem.status)) return;

        html += buildStatusCell(project, statusItem, MovProjectlst);
    });

    html += '</tr>';

    return html;
}
function isValidStatus(status) {
    return status !== "BISAG-N" && status !== "Re-Vetting";
}
function bindProjectClick() {

    $(document)
        .off("click", ".btn-clsprojName")
        .on("click", ".btn-clsprojName", function () {

            const row = $(this).closest("tr");
            const projId = row.find(".clsspnprojId").html();
            const projName = row.find(".projnameforlabel").html();

            $('#ProjHoldHistory').modal('show');
            $(".lblProjHoldHistory").html(projName);
            $("#cardforProjHoldHistory").removeClass("d-none");

            GetProjHold(projId);
            ProjectWiseStatusByProjid(projId);
        });
}



function ProjectWiseStatusByProjid(projid) {

    const userdata = { "Projid": projid };

    $.ajax({
        url: '/Home/GetProjectWiseStatus',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {

            if (!validateProjResponse(response)) return;

            const { statusProjectlst, movProjectlst } = response;

            const header = buildProjHeader(statusProjectlst);
            const body = buildProjBody(statusProjectlst, movProjectlst);

            $(".tblProjectWiseStatusByprojid").html(header + body);
        },

        error: function (error) {
            console.log('Error:', error);
            Swal.fire({ text: 'An error occurred while fetching data.' });
        }
    });
}
function ProjectWiseStatusByProjid(projid) {

    const userdata = { "Projid": projid };

    $.ajax({
        url: '/Home/GetProjectWiseStatus',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {

            if (!validateProjResponse(response)) return;

            const { statusProjectlst, movProjectlst } = response;

            const header = buildProjHeader(statusProjectlst);
            const body = buildProjBody(statusProjectlst, movProjectlst);

            $(".tblProjectWiseStatusByprojid").html(header + body);
        },

        error: function (error) {
            console.log('Error:', error);
            Swal.fire({ text: 'An error occurred while fetching data.' });
        }
    });
}
function validateProjResponse(response) {

    if (response == "null" || response == null) {
        return false;
    }

    if (response == -1) {
        Swal.fire({ text: "Error fetching data!" });
        return false;
    }

    if (response == 0) {
        Swal.fire({ text: "No data found for the given project ID." });
        return false;
    }

    return true;
}
function buildProjHeader(StatusProjectlst) {

    let html = `
        <thead>
            <tr class="theadfontsize">
                <th class="d-none noExport"></th>
    `;

    StatusProjectlst.forEach(item => {
        if (!isValidStatus(item.status)) return;
        html += `<th class="text-white">${item.status}</th>`;
    });

    html += `
            </tr>
        </thead>
    `;

    return html;
}
function buildProjBody(StatusProjectlst, MovProjectlst) {

    let html = '<tbody id="bodyProjectWiseStatusByprojid table-responsive">';
    let projTracker = 0;

    MovProjectlst.forEach(item => {

        if (projTracker === item.projId) return;

        projTracker = item.projId;

        html += buildProjRow(item, StatusProjectlst, MovProjectlst);
    });

    html += '</tbody>';

    return html;
}
function buildProjRow(project, StatusProjectlst, MovProjectlst) {

    let html = `
        <tr>
            <td class="clsspnprojId d-none noExport">${project.projId}</td>
    `;

    StatusProjectlst.forEach(statusItem => {
        if (!isValidStatus(statusItem.status)) return;

        html += buildProjStatusCell(project, statusItem, MovProjectlst);
    });

    html += '</tr>';

    return html;
}

function buildProjStatusCell(project, statusItem, MovProjectlst) {

    const match = MovProjectlst.filter(el =>
        el.statusId === statusItem.statusId &&
        el.projId === project.projId
    );

    if (match.length) {
        const time = DateFormateddMMyyyyhhmmss(match[0].timeStamp);

        return `
            <td data-toggle="tooltip" data-placement="top" title="${time}">
                <div class="d-flex d-flex justify-content-between">
                    <span class="pws-ok-dot d-flex">✔</span>
                    <span class="nowrap">${time}</span>
                </div>
            </td>
        `;
    }

    return `
        <td class="align-middle text-center">
            <img src="/assets/images/icons/Cross_red_circle.png"
                 width="22" height="22" alt="Readed">
        </td>
    `;
}

function isValidStatus(status) {
    return status !== "BISAG-N" && status !== "Re-Vetting" && status !== "AI/ML";
}





                            

    const pdfFileInput = document.getElementById('pdfFileInput');
if (pdfFileInput != null) {
    pdfFileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];

        if (file) {


            const maxSizeInBytes = 10 * 1024 * 1024;
            if (file.size > maxSizeInBytes) {
                $('#uploadButton').hide();
                pdfFileInput.value = '';
                Swal.fire({
                    title: 'File Size Exceeded',
                    text: 'Please select a file smaller than 10MB.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                return;
            }


            const reader = new FileReader();
            reader.onloadend = function () {
                const bytes = new Uint8Array(reader.result);
                const pdfHeader = new Uint8Array([37, 80, 68, 70, 45]); // %PDF-
                const isPDF = compareArrays(bytes.slice(0, 5), pdfHeader);
                if (isPDF) {

                    console.log('PDF file is valid. Proceed with upload.');
                } else {

                    Swal.fire({
                        title: 'Invalid File ....!',
                        text: 'Invalid PDF file. Please select a valid PDF file.',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                    $('#uploadButton').hide();
                    pdfFileInput.value = '';
                }
            };


            reader.readAsArrayBuffer(file);
        }
    });


    pdfFileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];

        if (file) {
            const reader = new FileReader();
            reader.onloadend = function () {
                const bytes = new Uint8Array(reader.result);
                const pdfHeader = new Uint8Array([37, 80, 68, 70, 45]); // %PDF-
                const isPDF = compareArrays(bytes.slice(0, 5), pdfHeader);
                if (isPDF) {

                    console.log('PDF file is valid. Proceed with upload.');
                } else {
                    pdfFileInput.value = '';
                    Swal.fire({
                        title: 'Invalid File ....!',
                        text: 'Invalid PDF file. Please select a valid PDF file.',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            };


            reader.readAsArrayBuffer(file);
        }
    });

}
    

    function compareArrays(array1, array2) {
        if (array1.length !== array2.length) {
            return false;
        }
        for (let i = 0; i < array1.length; i++) {
            if (array1[i] !== array2[i]) {
                return false;
            }
        }
        return true;
    }





    

    


    function compareArrays(array1, array2) {
        const pdfFileInputs = document.getElementById('pdfFileInput');
        if (array1.length !== array2.length) {
            pdfFileInputs.value = '';
            return false;
        }
        for (let i = 0; i < array1.length; i++) {
            if (array1[i] !== array2[i]) {
                pdfFileInputs.value = '';
                return false;
            }
        }
        return true;
    }

    $(document).ready(function () {
        $('.dropdownsearch').select2();
    });





    $(document).ready(function () {

        function checkConditions() {
            var remarksLength = $('#AttHisAdd_Reamarks').val().length;
            var pdfFileInput = $('#pdfFileInput')[0].files.length;

            if (remarksLength > 1 && pdfFileInput > 0) {
                $('#uploadButton').prop('disabled', false);
            } else {
                $('#uploadButton').prop('disabled', true);
            }
        }

        $('#upload').click(function () {
            var documentDescription = $('#pdfFileInput').val();
            if (documentDescription.trim() === "") {
                Swal.fire({
                    title: 'Missing Upload File  ....!',
                    text: 'Please upload a file first.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });

                return false; // Prevent form submission
            }
        });

        $('#AttHisAdd_Reamarks, #pdfFileInput').on('input change', function () {
            checkConditions();
        });

        $('#uploadButton').prop('disabled', true);
    });

document.addEventListener("DOMContentLoaded", function () {
    const btn = document.getElementById("btnClose");
    if (btn) {
        btn.addEventListener("click", function () {
            history.back();
        });
    }
});
