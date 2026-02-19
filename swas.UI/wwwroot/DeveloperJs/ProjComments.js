


    $(document).ready(function () {
        $('.table-button').on('click', function () {
            var $button = $(this);
            var stakeHolderId = $button.data('stakeholder-id');
            var projId = $button.data('proj-id');
            var psmId = $button.data('psm-id');

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

                    var commentContainer = '';

                    for (var i = 0; i < data.length; i++) {
                        var date = new Date(data[i].date);
                        var formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

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
                    alert('Error fetching comments.');
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
        var $button = $(this);
        var stakeHolderId = $button.data('stakeholder-id');
        var projId = $button.data('proj-id');
        var psmId = $button.data('psm-id');

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
                var commentContainer = '';

                for (var i = 0; i < data.length; i++) {
                    var date = new Date(data[i].date);
                    var formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

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
                alert('Error fetching comments.');
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
    var projectDetailsBtns = document.querySelectorAll('.project-details-btn');

    projectDetailsBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var projectDetailsDiv = document.getElementById('projectDetails');
            var projectDetailsDiv1 = document.getElementById('ProjDetails1');
            var projectDetailsDiv2 = document.getElementById('ProjDetails2');
            var projectreadDeatilsDiv = document.getElementById('projectreadDetails');
            var projectreadDetailsDiv1 = document.getElementById('ProjreadDetails1');
            var projectreadDetailsDiv2 = document.getElementById('ProjreadDetails2');
            var projName = btn.getAttribute('data-proj-namess') || '';
            var aimScope = btn.getAttribute('data-aim-scope') || '';
            var Initiateddate = btn.getAttribute('data-initiated-date') || '';
            var newbandwidth = btn.getAttribute('data-band-with') || '';
            var hostingtype = btn.getAttribute('data-hosting-type') || '';
            var reqjustification = btn.getAttribute('data-req-justi') || '';
            var conceptofsw = btn.getAttribute('data-concept-sw') || '';
            var initiatedBy = btn.getAttribute('data-initiated-by') || '';
            var hosttype = btn.getAttribute('data-hosttype') || '';
            projectDetailsDiv.innerHTML = `
                Proj Details
                <table class="new-proj-table">
                    <tr>
                        <td>Proj Name</td>
                        <td>${projName}</td>
                    </tr>
                    <tr>
                        <td>Aim & Scope</td>
                        <td class="long-text">${aimScope}</td>
                    </tr>
                    <tr>
                        <td>Initiated Date</td>
                        <td>${Initiateddate}</td>
                    </tr>
                </table>
            `;

            projectDetailsDiv1.innerHTML = `
                Tech Details
                <table class="new-proj-table">
                    <tr>
                        <td>New Band With</td>
                        <td>${newbandwidth}</td>
                    </tr>
                    <tr>
                        <td>Hosting Type</td>
                        <td>${hostingtype}</td>
                    </tr>
                    <tr>
                        <td>Request Justification</td>
                        <td class="long-text">${reqjustification}</td>
                    </tr>
                </table>
            `;

            projectDetailsDiv2.innerHTML = `
                Other Details
                <table class="new-proj-table">
                    <tr>
                        <td>Concept Of S/W</td>
                        <td>${conceptofsw}</td>
                    </tr>
                    <tr>
                        <td>Initiated By</td>
                        <td>${initiatedBy}</td>
                    </tr>
                    <tr>
                        <td>Host Type</td>
                        <td>${hosttype}</td>
                    </tr>
                </table>
            `;

            projectreadDeatilsDiv.innerHTML = `
                Proj Details
                <table class="new-proj-table">
                    <tr>
                        <td>Proj Name</td>
                        <td>${projName}</td>
                    </tr>
                    <tr>
                        <td>Aim & Scope</td>
                        <td class="long-text">${aimScope}</td>
                    </tr>
                    <tr>
                        <td>Initiated Date</td>
                        <td>${Initiateddate}</td>
                    </tr>
                </table>
            `;

            projectreadDetailsDiv1.innerHTML = `
                Tech Details
                <div id="testforscroll">
                    <table class="new-proj-table">
                        <tr>
                            <td>New Band With</td>
                            <td>${newbandwidth}</td>
                        </tr>
                        <tr>
                            <td>Hosting Type</td>
                            <td>${hostingtype}</td>
                        </tr>
                        <tr>
                            <td>Request Justification</td>
                            <td class="long-text">${reqjustification}</td>
                        </tr>
                    </table>
                </div>
            `;

            projectreadDetailsDiv2.innerHTML = `
                Other Details
                <table class="new-proj-table">
                    <tr>
                        <td>Concept Of S/W</td>
                        <td>${conceptofsw}</td>
                    </tr>
                    <tr>
                        <td>Initiated By</td>
                        <td>${initiatedBy}</td>
                    </tr>
                    <tr>
                        <td>Host Type</td>
                        <td>${hosttype}</td>
                    </tr>
                </table>
            `;
        });
    });
});











    function handleStatusChange() {

        var selectedStatus = document.getElementById("ddlStatus").value;


        var fileInput = document.getElementById("uploadfile");
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
    




    
     

