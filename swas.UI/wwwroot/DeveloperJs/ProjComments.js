


    $(document).ready(function () {
        $('.table-button').on('click', function () {
            let $button = $(this);
            let stakeHolderId = $button.data('stakeholder-id');
            let projId = $button.data('proj-id');
            let psmId = $button.data('psm-id');

            $('#StakeholdertextId').val(stakeHolderId);
            $('#ProjtextId').val(projId);
            $('#PsmToProj').val(psmId);

            $.ajax({
                type: "POST",
                url: '@Url.Action("GetComments", "Home")',
                data: {
                    "PsmId": psmId,
                    "stakeholderId": stakeHolderId,
                    "ProjId": projId
                },
                success: function (data) {

                    let commentContainer = '';

                    for (let i = 0; i < data.length; i++) {
                        let date = new Date(data[i].date);
                        let formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

                        commentContainer += '<div class="comment-box">'; // Use text-align: justify for justified text
                        commentContainer += '<div class="comment-header">';
                        commentContainer += '<div>';
                        commentContainer += '<span class="comment-stakeholder">' + data[i].stakeholderName + '</span>';
                        commentContainer += '<span class="comment-meta">' + formattedDate + '</span>';
                        commentContainer += '</div>';
                        commentContainer += '<div>';
                        commentContainer += '<span class="comment-status">' + data[i].statusName + '</span>';
                        commentContainer += '<span class="pdf-link">'; // Move the PDF link to the same line as status

                        if (data[i].state !== null) {

                            commentContainer += '<a href="/Home/WaterMark3?id=' + data[i].state + '" target="_blank">';
                            commentContainer += '<img src="/assets/images/icons/pdfimg.png" alt="PDF icon" class="pdf-icon">';
                            commentContainer += '</a>';
                        }

                        commentContainer += '</span>';
                        commentContainer += '</div>';
                        commentContainer += '</div>';
                        commentContainer += '<div class="comment-content">' + data[i].comments + '</div>';
                        commentContainer += '</div>';
                    }

                    $('#ChatBox').empty().html(commentContainer);
                    $('#AddStatusDetails').show();
                    $('#IndexTableContainer').hide();
                },
                error: function () {
                    alert('Error fetching comments.2');
                }
            });
        });

        $('#AddStatusDetails').hide();

        $('#CancelUpdate').click(function () {
            $('#AddStatusDetails').hide();
            $('#IndexTableContainer').show();
        });
      

    });





$(document).ready(function () {
    $('.table-readonly').on('click', function () {
        let $button = $(this);
        let stakeHolderId = $button.data('stakeholder-id');
        let projId = $button.data('proj-id');
        let psmId = $button.data('psm-id');

        $('#StakeholdertextId').val(stakeHolderId);
        $('#ProjtextId').val(projId);
        $('#PsmToProj').val(psmId);

        $.ajax({
            type: "POST",
            url: '@Url.Action("GetComments", "Home")',
            data: {
                "PsmId": psmId,
                "stakeholderId": stakeHolderId,
                "ProjId": projId
            },
            success: function (data) {
                let commentContainer = '';

                for (let i = 0; i < data.length; i++) {
                    let date = new Date(data[i].date);
                    let formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

                    commentContainer += '<div class="comment-box">'; // Justified text is controlled in CSS
                    commentContainer += '<div class="comment-header">';
                    commentContainer += '<div>';
                    commentContainer += '<span class="comment-stakeholder">' + data[i].stakeholderName + '</span>';
                    commentContainer += '<span class="comment-meta">' + formattedDate + '</span>';
                    commentContainer += '</div>';
                    commentContainer += '<div>';
                    commentContainer += '<span class="comment-meta">' + data[i].statusName + '</span>';
                    commentContainer += '<span class="pdf-link">'; // Move the PDF link to the same line as status

                    if (data[i].state !== null) {
                        commentContainer += '<a href="/Home/WaterMark3?id=' + data[i].state + '" target="_blank">';
                        commentContainer += '<img src="/assets/images/icons/pdfimg.png" alt="PDF icon" class="pdf-icon">';
                        commentContainer += '</a>';
                    }

                    commentContainer += '</span>';
                    commentContainer += '</div>';
                    commentContainer += '</div>';
                    commentContainer += '<div class="comment-content">' + data[i].comments + '</div>';
                    commentContainer += '</div>';
                }

                $('#ChatBoxreadonly').empty().html(commentContainer);
                $('#AddStatusDetReadonly').show();
                $('#IndexTableContainer').hide();

            },
            error: function () {
                alert('Error fetching comments.3');
            }
        });
    });

    $('#AddStatusDetReadonly').hide();

    $('#CancelUpdate1').click(function () {
        $('#AddStatusDetReadonly').hide();
        $('#IndexTableContainer').show();
    });
});









document.addEventListener('DOMContentLoaded', function () {
    let projectDetailsBtns = document.querySelectorAll('.project-details-btn');

    projectDetailsBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {

            let projectDetailsDiv = document.getElementById('projectDetails');
            let projectDetailsDiv1 = document.getElementById('ProjDetails1');
            let projectDetailsDiv2 = document.getElementById('ProjDetails2');
            let projectreadDeatilsDiv = document.getElementById('projectreadDetails');
            let projectreadDetailsDiv1 = document.getElementById('ProjreadDetails1');
            let projectreadDetailsDiv2 = document.getElementById('ProjreadDetails2');

            // ✅ Using dataset instead of getAttribute
            let projName = btn.dataset.projNamess || '';
            let aimScope = btn.dataset.aimScope || '';
            let Initiateddate = btn.dataset.initiatedDate || '';
            let newbandwidth = btn.dataset.bandWith || '';
            let hostingtype = btn.dataset.hostingType || '';
            let reqjustification = btn.dataset.reqJusti || '';
            let conceptofsw = btn.dataset.conceptSw || '';
            let initiatedBy = btn.dataset.initiatedBy || '';
            let hosttype = btn.dataset.hosttype || '';

            projectDetailsDiv.innerHTML = `
                Proj Details
                <table class="new-proj-table">
                    <tr><td>Proj Name</td><td>${projName}</td></tr>
                    <tr><td>Aim & Scope</td><td class="long-text">${aimScope}</td></tr>
                    <tr><td>Initiated Date</td><td>${Initiateddate}</td></tr>
                </table>
            `;

            projectDetailsDiv1.innerHTML = `
                Tech Details
                <table class="new-proj-table">
                    <tr><td>New Band With</td><td>${newbandwidth}</td></tr>
                    <tr><td>Hosting Type</td><td>${hostingtype}</td></tr>
                    <tr><td>Request Justification</td><td class="long-text">${reqjustification}</td></tr>
                </table>
            `;

            projectDetailsDiv2.innerHTML = `
                Other Details
                <table class="new-proj-table">
                    <tr><td>Concept Of S/W</td><td>${conceptofsw}</td></tr>
                    <tr><td>Initiated By</td><td>${initiatedBy}</td></tr>
                    <tr><td>Host Type</td><td>${hosttype}</td></tr>
                </table>
            `;

            projectreadDeatilsDiv.innerHTML = `
                Proj Details
                <table class="new-proj-table">
                    <tr><td>Proj Name</td><td>${projName}</td></tr>
                    <tr><td>Aim & Scope</td><td class="long-text">${aimScope}</td></tr>
                    <tr><td>Initiated Date</td><td>${Initiateddate}</td></tr>
                </table>
            `;

            projectreadDetailsDiv1.innerHTML = `
                Tech Details
                <div id="testforscroll">
                    <table class="new-proj-table">
                        <tr><td>New Band With</td><td>${newbandwidth}</td></tr>
                        <tr><td>Hosting Type</td><td>${hostingtype}</td></tr>
                        <tr><td>Request Justification</td><td class="long-text">${reqjustification}</td></tr>
                    </table>
                </div>
            `;

            projectreadDetailsDiv2.innerHTML = `
                Other Details
                <table class="new-proj-table">
                    <tr><td>Concept Of S/W</td><td>${conceptofsw}</td></tr>
                    <tr><td>Initiated By</td><td>${initiatedBy}</td></tr>
                    <tr><td>Host Type</td><td>${hosttype}</td></tr>
                </table>
            `;
        });
    });
});











    function handleStatusChange() {

        let selectedStatus = document.getElementById("ddlStatus").value;


        let fileInput = document.getElementById("uploadfile");
        }
    

    

        const pdfFileInput = document.getElementById('uploadfile');

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
        const pdfHeader = new Uint8Array([37, 80, 68, 70, 45]);
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

        pdfFileInput.value = ''; 
                }
            };


        reader.readAsArrayBuffer(file);
        }
    });

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
    




    
     

