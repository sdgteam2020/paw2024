


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
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",
                success: function (data) {

                    var commentContainer = '';

                    for (var i = 0; i < data.length; i++) {
                        var date = new Date(data[i].date);
                        var formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

                        commentContainer += '<div class="comment-box" style="text-align: justify;">'; // Use text-align: justify for justified text
                        commentContainer += '<div class="comment-header">';
                        commentContainer += '<div>';
                        commentContainer += '<span style="font-family: Arial; font-weight: bold; color: #0793f7;">' + data[i].stakeholderName + '</span>';
                        commentContainer += '<span style="margin-left: 10px;" class="comment-meta">' + formattedDate + '</span>';
                        commentContainer += '</div>';
                        commentContainer += '<div>';
                        commentContainer += '<span class="comment-meta">' + data[i].statusName + '</span>';
                        commentContainer += '<span class="pdf-link">'; // Move the PDF link to the same line as status

                        if (data[i].state !== null) {

                            commentContainer += '<a href="/Home/WaterMark3?id=' + data[i].state + '" target="_blank">';
                            commentContainer += '&nbsp;&nbsp; &nbsp;&nbsp;<img src="/assets/images/icons/pdfimg.png" alt="PDF icon" style="width: 24px; height: 24px;">';
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

                        commentContainer += '<div class="comment-box" style="text-align: justify;">'; // Use text-align: justify for justified text
                        commentContainer += '<div class="comment-header">';
                        commentContainer += '<div>';
                        commentContainer += '<span style="font-family: Arial; font-weight: bold; color: #0793f7;">' + data[i].stakeholderName + '</span>';
                        commentContainer += '<span style="margin-left: 10px;" class="comment-meta">' + formattedDate + '</span>';
                        commentContainer += '</div>';
                        commentContainer += '<div>';
                        commentContainer += '<span class="comment-meta">' + data[i].statusName + '</span>';
                        commentContainer += '<span class="pdf-link">'; // Move the PDF link to the same line as status

                        if (data[i].state !== null) {
                            commentContainer += '<a href="/Home/WaterMark3?id=' + data[i].state + '" target="_blank">';
                            commentContainer += '&nbsp;&nbsp; &nbsp;&nbsp;<img src="/assets/images/icons/pdfimg.png" alt="PDF icon" style="width: 24px; height: 24px;">';
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
                                        Proj Details
                                        <table class="new-proj-table" style=width: 100%;height: 150px;>
                                            <tr>
                                                <td style="color: black;">Proj Name</td>
                                                <td style="color: black;">${projName}</td>
                                            </tr>
                                            <tr>
                                                <td style="color: black;">Aim & Scope</td>
                                                        <td class="long-text" style="color: black;">${aimScope}</td>
                                            </tr>
                                            <tr>
                                                <td style="color: black;">Initiated Date</td>
                                                <td style="color: black;">${Initiateddate}</td>
                                            </tr>
                                            <!-- Add more rows as needed -->
                                        </table>
                                    `;

                projectDetailsDiv1.innerHTML = `
                                        Tech Details
                                        <table class="new-proj-table">
                                            <tr>
                                                <td style="color: black;">New Band With</td>
                                                <td style="color: black;">${newbandwidth}</td>
                                            </tr>
                                            <tr>
                                                <td style="color: black;">Hosting Type</td>
                                                <td style="color: black;">${hostingtype}</td>
                                            </tr>
                                            <tr>
                                                <td style="color: black;">Request Justification</td>
                                                        <td class="long-text" style="color: black;">${reqjustification}</td>
                                            </tr>
                                            <!-- Add more rows as needed -->
                                        </table>
                                    `;

                projectDetailsDiv2.innerHTML = `
                                        Other Details
                                        <table class="new-proj-table" style=width: 100%;height: 150px;>
                                            <tr>
                                                <td style="color: black;">Concept Of S/W</td>
                                                <td style="color: black;">${conceptofsw}</td>
                                            </tr>
                                            <tr>
                                                <td style="color: black;">Initiated By</td>
                                                <td style="color: black;">${initiatedBy}</td>
                                            </tr>
                                            <tr>
                                                <td style="color: black;">Host Type</td>
                                                <td style="color: black;">${hosttype}</td>
                                            </tr>
                                            <!-- Add more rows as needed -->
                                        </table>
                                    `;
                projectreadDeatilsDiv.innerHTML = `
                                                Proj Details
                                                <table class="new-proj-table" style=width: 100%;height: 150px;>
                                                    <tr>
                                                        <td style="color: black;">Proj Name</td>
                                                        <td style="color: black;">${projName}</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="color: black;">Aim & Scope</td>
                                                        <td class="long-text" style="color: black;">${aimScope}</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="color: black;">Initiated Date</td>
                                                        <td style="color: black;">${Initiateddate}</td>
                                                    </tr>
                                                    <!-- Add more rows as needed -->
                                                </table>
                                            `;

                projectreadDetailsDiv1.innerHTML = `
                                                Tech Details
                                                <div id="testforscroll">
                                                   <table class="new-proj-table">
                                                <tr>
                                                    <td style="color: black;">New Band With</td>
                                                    <td style="color: black;">${newbandwidth}</td>
                                                </tr>
                                                <tr>
                                                    <td style="color: black;">Hosting Type</td>
                                                    <td style="color: black;">${hostingtype}</td>
                                                </tr>
                                                <tr>
                                                    <td style="color: black;">Request Justification</td>
                                                            <td class="long-text" style="color: black;">${reqjustification}</td>
                                                </tr>
                                                <!-- Add more rows as needed -->
                                            </table>
                                                </div>
                                            `;

                projectreadDetailsDiv2.innerHTML = `
                                                Other Details
                                                <table class="new-proj-table" style=width: 100%;height: 150px;>
                                                    <tr>
                                                        <td style="color: black;">Concept Of S/W</td>
                                                        <td style="color: black;">${conceptofsw}</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="color: black;">Initiated By</td>
                                                        <td style="color: black;">${initiatedBy}</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="color: black;">Host Type</td>
                                                        <td style="color: black;">${hosttype}</td>
                                                    </tr>
                                                    <!-- Add more rows as needed -->
                                                </table>
                                            `;
            });
        });
    });











    function handleStatusChange() {

        var selectedStatus = document.getElementById("ddlStatus").value;


        var fileInput = document.getElementById("uploadfile");

        //if (selectedStatus === "1") {
            //    fileInput.removeAttribute("disabled");
            //} else {
            //    fileInput.value = "";
            //    fileInput.setAttribute("disabled", "disabled");
            //}
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
    




    
        @if (TempData.ContainsKey("SuccessMessage"))
        {
            <text>
                Swal.fire({
                    title: 'Saved & Ready for Next Step',
                text: '@TempData["SuccessMessage"]',
                icon: 'success',
                confirmButtonText: 'OK'
            });

            </text>
        TempData.Remove("SuccessMessage");
    }
    



    
        @if (TempData.ContainsKey("FailureMessage"))
        {
            <text>
                Swal.fire({
                    title: 'Something Went Wrong....!',
                text: '@TempData["FailureMessage"]',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            </text>
        TempData.Remove("FailureMessage");

    }
    

