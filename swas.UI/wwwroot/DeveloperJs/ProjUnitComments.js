
    $(document).ready(function () {
        // Handle click event on elements with id starting with "commentDiv_"
        $('[id^="commentDiv_"]').click(function () {
            // Extract the index from the id attribute
            alert("Hy buddy");
            var index = $(this).attr('id').replace('commentDiv_', '');
            handleDivClick(data[index].comments);
        });
    });

    function handleDivClick(comments) {
        // Your handling logic here
        console.log('Div clicked with comments:', comments);
        // You can perform additional actions here
    }

    $(document).ready(function () {
        $('#StatusUpdate').click(function () {
            var status = $('#ddlStatus').val();
            var comments = $('#Comments').val();
            var stakeholderId = $('#StakeholdertextId').val();
            var projId = $('#ProjtextId').val();
            var psmId = $('#PsmToProj').val();
            //var pdfFile = $('#pdfFile')[0].files[0];

            $.ajax({
                type: "POST",
                url: '@Url.Action("UpdateUnitStatus", "Home")',
                data: {
                    StakeholderId: stakeholderId,
                    ProjId: projId,
                    PsmID: psmId,
                    StatusId: status,
                    Comment: comments,
                    //PsmID: pdfFile
                },
                success: function (data) {
                    // Handle success response from the server
                    $('#IndexTable').html(response);
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText); // Log the detailed error message
                    alert('Error fetching comments. See console for details.')
                }
            });
        });
    });
document.addEventListener('DOMContentLoaded', function () {
    var projectDetailsBtns = document.querySelectorAll('.project-details-btn');

    projectDetailsBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var projectDetailsDiv = document.getElementById('projectDetails');
            var projectDetailsDiv1 = document.getElementById('ProjDetails1');
            var projectDetailsDiv2 = document.getElementById('ProjDetails2');

            // Get data from the clicked button
            var projName = btn.getAttribute('data-proj-namess') || '';
            var aimScope = btn.getAttribute('data-aim-scope') || '';
            var Initiateddate = btn.getAttribute('data-initiated-date') || '';
            var newbandwidth = btn.getAttribute('data-band-with') || '';
            var hostingtype = btn.getAttribute('data-hosting-type') || '';
            var reqjustification = btn.getAttribute('data-req-justi') || '';
            var conceptofsw = btn.getAttribute('data-concept-sw') || '';
            var initiatedBy = btn.getAttribute('data-initiated-by') || '';
            var hosttype = btn.getAttribute('data-hosttype') || '';

            // Update the content of the projectheading div
            projectDetailsDiv.innerHTML = `
                <h3>Proj Details</h3>
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
                <h3>Tech Details</h3>
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
                <h3>Other Details</h3>
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



    function handlePdfClick(pdfFileName) {
        alert(pdfFileName);
        $.ajax({
            type: "POST",
            url: "/Home/WaterMark3",
            data: { id: pdfFileName },
            success: function (result) {

                console.log("PDF click handled successfully");
            },
            error: function (error) {

                console.error("Error handling PDF click", error);
            }
        });
    }


    const pdfFileInput = document.getElementById('pdfFile');

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


<script>
    function redirectToNewPage() {
        // You can implement redirection logic here if needed
    }

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
                url: '@Url.Action("GetUnitComments", "Home")',
                data: {
                    stakeholderId: stakeHolderId,
                    psmId: psmId,
                    projId: projId
                },
                success: function (data) {
                    var commentContainer = '<div class="comment-container">';

                    for (var i = 0; i < data.length; i++) {
                        var date = new Date(data[i].date);
                        var formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

                        commentContainer += '<div class="comment-box">';
                        commentContainer += '<div class="comment-header">';
                        commentContainer += '<div>';
                        commentContainer += '<span class="stakeholder-name">' + data[i].stakeholderName + '</span>';
                        commentContainer += '<span class="comment-meta">' + formattedDate + '</span>';
                        commentContainer += '</div>';
                        commentContainer += '<div>';
                        commentContainer += '<span class="comment-status">' + data[i].statusName + '</span>';
                        commentContainer += '<span class="pdf-link">';

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

                    commentContainer += '</div>'; // Close the container
                    $('#ChatBox').empty().html(commentContainer);
                    $('#AddStatusDetails').show();

                    // Hide IndexTable container
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

    // Show IndexTable container
    $('#IndexTableContainer').show();
        });
    });
</script>
