

$(document).ready(function () {
    
    $('.tabs__head a').click(function (e) {
        e.preventDefault();
        $('.tabs__head a').removeClass('active-link');
        $(this).addClass('active-link');
        $('.tab-content').hide();
        var tabId = $(this).data('tab');
        $('#' + tabId).show();
    });
});


$(document).on('click', '.approve-btn', function () {
    
    let id = Number($(this).data('id')) || 0;
    let projid = $(this).data('ids')

    let projectName = $(this).data('project-name');
    let isApproved = $(this).text().trim() === 'Approved';
   
    let actiontype = $(this).data('actiontype');
    let message= " ";
    if (actiontype === 3) {
        message='Please Enter Remarks:' 
    }
    else {
        message = 'Please Enter Remarks:'
}
   
         

    $.ajax({
        url: '/Notification/GetUnreadProjectCommentsCount',
        type: 'POST',
        data: { id: id },
        success: function (res) {
            if (res.success) {
                loadDateApprovalTable();
                getProjectDetails();
                fetchProjectCommentsUnreadCount();

                Swal.fire({
                    title: (actiontype === 3) ? 'Confirm Rejection' : 'Confirm Approval',
                    html: message,
                    input: 'textarea',
                    inputPlaceholder: 'Enter your remarks here...',
                    inputAttributes: {
                        'aria-label': 'Remarks'
                    },
                    showCancelButton: true,
                    confirmButtonColor: (actiontype === 3) ? '#ffc107' : '#28a745',
                    cancelButtonColor: '#d33',
                    confirmButtonText: (actiontype === 3) ? 'Yes, Reject' : 'Yes, Approve',
                    preConfirm: (remarks) => {
                        if (!remarks) {
                            Swal.showValidationMessage('Remarks are required.');
                        }
                        if (remarks.length<10) {
                            Swal.showValidationMessage('Remarks Must be Atleast 10 characters');
                        }
                        if (remarks.length>200) {
                            Swal.showValidationMessage('Remarks Must not exceed 200 characters');
                        }
                        return remarks;
                    }
                }).then((result) => {
                    if (result.isConfirmed && result.value) {
                        let remarks = result.value;

                        $.ajax({
                            url: '/Projects/ApproveDateRequest',
                            type: 'POST',
                            data: {
                                id: id,
                                projid: projid,
                                remarks: remarks,
                                actiontype: actiontype // send flag to server to know if it's unapprove
                            },

                            success: function (res) {
                                if (res.success) {
                                    Swal.fire({
                                        title: (actiontype === 3) ? 'Rejected!' : 'Approved!',
                                        text: res.message,
                                        icon: 'success',
                                        timer: 2000,
                                        showConfirmButton: false
                                    });

                                    loadDateApprovalTable();
                                    getProjectDetails();
                                    fetchProjectCommentsUnreadCount();
                                } else {
                                    Swal.fire('Error!', res.message, 'error');
                                }
                            },
                            error: function () {
                                Swal.fire('Server Error', 'Could not process the request.', 'error');
                            }
                        });
                    }
                });

            } else {
                Swal.fire('Error!', res.message, 'error');
            }
        },
        error: function () {
            Swal.fire('Server Error', 'Could not update read status.', 'error');
        }
    });
});


$(document).on('click', '.LegacyHistory', function () {
    
    $('#ProjFwdHistory').modal('show');
    
    var ProjId = parseInt($(this).data("ids"));
    
    GetProjectLegacyHistory(ProjId); // <-- fixed this line
});




function GetProjectLegacyHistory(ProjId) {

    $.ajax({
        url: '/Projects/GetProjectLegacyHistory',
        type: 'POST',
        data: { ProjectId: ProjId },
        success: handleLegacySuccess
    });
}

// 🔹 SUCCESS HANDLER
function handleLegacySuccess(response) {

    console.log(response);

    if (!response || response.length === 0) {
        $("#projectLegacyhistory").html('<div class="alert alert-info">No history available.</div>');
        return;
    }

    $(".lblHistory").text("Project Name: " + response[0].projectName + " History");

    let html = buildTimelineHeader(response.length);

    html += response.map(buildTimelineSection).join('');

    $("#projectLegacyhistory").html(html);
}

// 🔹 HEADER
function buildTimelineHeader(count) {
    return `
        <div class="timeline-month">
            <span>${count} Entries</span>
        </div>`;
}

// 🔹 SECTION
function buildTimelineSection(item) {

    return `
    <div class="timeline-section">
        <div class="timeline-date">${DateFormateddMMyyyyhhmmss(item.actionDate)}</div>
        <div class="row g-3">
            ${buildActionBox(item)}
            ${buildRemarksBox(item)}
        </div>
    </div>`;
}

// 🔹 ACTION BOX
function buildActionBox(item) {

    return `
    <div class="col-md-6">
        <div class="timeline-box">
            ${getActionTitle(item)}
            <div class="box-content">
                ${getActionContent(item)}
            </div>
            <div class="box-footer">${item.userdetails || 'Unknown User'}</div>
        </div>
    </div>`;
}

// 🔹 ACTION TITLE
function getActionTitle(item) {

    if (item.actionType === 1) {
        return `<div class="box-title bg-warning text-white">
                    <i class="fa-solid fa-forward"></i> ${item.actionTypeText}
                </div>`;
    }

    if (item.actionType === 2) {
        return `<div class="box-title bg-success text-white">
                    <i class="fa-solid fa-circle-check"></i> ${item.actionTypeText}
                </div>`;
    }

    if (item.actionType === 3 || item.actionType === 4) {
        return `<div class="box-title bg-danger text-white">
                    <i class="fa-solid fa-rotate-left"></i> ${item.actionTypeText}
                </div>`;
    }

    return `<div class="box-title bg-secondary text-white">${item.actionTypeText}</div>`;
}

// 🔹 ACTION CONTENT
function getActionContent(item) {

    if (item.actionType === 1) {
        return buildRow("Request By", item.fromunitName);
    }

    if ([2, 3, 4].includes(item.actionType)) {
        return buildRow(item.actionTypeText + " By", item.fromunitName);
    }

    return '';
}

// 🔹 REMARKS BOX
function buildRemarksBox(item) {

    if (!item.remarks) return '';

    return `
    <div class="col-md-6">
        <div class="timeline-box">
            <div class="box-title">
                <i class="fa fa-pencil text-info"></i>
                Remarks On ${DateFormateddMMyyyyhhmmss(item.actionDate)}
            </div>
            <div class="box-content">
                <div class="box-item">${item.remarks}</div>
            </div>
            <div class="box-footer">${item.userdetails || 'Unknown User'}</div>
        </div>
    </div>`;
}

// 🔹 COMMON ROW
function buildRow(label, value) {
    return `
    <div class="row mb-1">
        <div class="col-4"><strong>${label}</strong>:</div>
        <div class="col-8">
            <span class="badge bg-secondary">${value || 'N/A'}</span>
        </div>
    </div>`;
}

function loadDateApprovalTable() {
    $.ajax({
        url: '/Home/GetDateApprovalList',
        method: 'GET',

        success: function (response) {

            let listItem = '';
            let count = 0;
            const badge = document.getElementById("IngestionReq");

            for (let i = 0; i < response.length; i++) {

                let item = response[i];

                if (response[i].isRead == false) {
                    count++;
                    listItem += "<tr class='bold-text'>";
                } else {
                    listItem += "<tr>";
                }

                var projName = item.projName;
                var words = projName.split(" ");
                var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;

                listItem += "<td class='align-middle '>" + (i + 1) + "</td>";

                listItem += "<td class='align-middle'>";
                listItem += "<a  href='/Projects/ProjHistory?EncyID=" + encodeURIComponent(item.encyID) + "'>";
                listItem += "<span id='projectName' class='projNameDetail' >" + shortProjName + "</span>";
                listItem += "</a>";
                listItem += "</td>";

                listItem += "<td class='align-middle'>" + item.user + "</td>";
                listItem += "<td class='align-middle'>" + item.unitName + "</td>";
                listItem += "<td class='align-middle'>" + DateFormateddMMyyyyhhmmss(item.request_Date) + "</td>";
                listItem += "<td class='da-td-mid-center'>" + (item.ddgiT_Approval_dat ? DateFormateddMMyyyyhhmmss(item.ddgiT_Approval_dat) : "-") + "</td>";

                let isApproved = item.ddgiT_approval === true || item.ddgiT_approval === "true";
                listItem += "<td class='align-middle text-start'>" + formatRemarks(item.remarks) + "</td>";

                if (isApproved) {
                    listItem += `<td class='align-middle d-flex'>
				<button class='btn btn-success btn-sm approve-btn'
					data-bold="${item.isRead}"
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="4"  disabled>Approved</button>

				<button class='btn btn-warning btn-sm ml-2 approve-btn'
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="3"  title="Request Reject">Reject</button>

				<a href="#" class="ml-2 LegacyHistory" data-action="LegacyHistory" data-ids="${item.projId}" title="History of the Legacy">
					<img src="/assets/images/icons/Legacyhistory.png" alt="Icon" class="da-ico-27">
				</a>
			</td>`;
                } else {
                    listItem += `<td class='align-middle d-flex'>
				<button class='btn btn-danger btn-sm approve-btn'
					data-bold="${item.isRead}"
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="2">Approve</button>

				<button class='btn btn-warning btn-sm ml-2 approve-btn'
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="3" title="Request Reject">Reject</button>

				<a href="#" class="ml-2 LegacyHistory" data-action="LegacyHistory" data-ids="${item.projId}" title="History of the Legacy">
					<img src="/assets/images/icons/Legacyhistory.png" alt="Icon" class="da-ico-27">
				</a>
			</td>`;
                }

                listItem += "</tr>";
            }

            if (badge) {
                if (count > 0) {
                    badge.textContent = count;
                    badge.classList.remove("d-none");
                } else {
                    badge.textContent = '';
                    badge.classList.add("d-none");
                }
            }

            fetchProjectCommentsUnreadCount();
            $('#DateApproval').html(listItem);
            initializeDataTable('#TableType1');
        },
        error: function () {
            console.error('Error fetching data');
        }
    });
}


$(document).ready(function () {
    getProjectDetails();
    bindLiveProjectSearch(
        "#Find_forApproval",                  // Input field
        "#projectNameDropdown",
        "/Projects/GetProjectByKeyup",// Dropdown UL
        function (projId, projName, remarks) {        // On select
            $("#ProjName").val(projName);
            $("#ProjId").val(projId);
            getProjectDetails(projId,remarks);       // Your custom logic
        }
    );
});





function getProjectDetails(projId, remarks) {
    $.ajax({
        url: '/Home/GetDateApprovalList',
        method: 'GET',
        data: {
            projId: projId,
            requestType: 2,
            remarks: remarks
        },
        success: handleProjectDetailsSuccess,
        error: handleProjectDetailsError
    });
}

// 🔹 SUCCESS HANDLER
function handleProjectDetailsSuccess(response) {

    if (response.success === false) {
        showError(response.message);
        return;
    }

    let count = 0;

    const rows = response.map((item, i) => {
        if (!item.isRead) count++;
        return buildProjectRow(item, i);
    }).join('');

    updateBadge(count);

    $('#DateApproval1').html(rows);
    initializeDataTable('#TableType2');
    fetchProjectCommentsUnreadCount();
}

// 🔹 ROW BUILDER
function buildProjectRow(item, index) {

    const rowClass = item.isRead ? "" : "bold-text";
    const shortProjName = getShortName(item.projName);
    const isApproved = item.ddgiT_approval === true || item.ddgiT_approval === "true";

    return `
    <tr class="${rowClass}">
        <td class='align-middle'>${index + 1}</td>

        <td class='align-middle'>
            <a href='/Projects/ProjHistory?EncyID=${encodeURIComponent(item.encyID)}'>
                <span id='projectName' class='projNameDetail'>${shortProjName}</span>
            </a>
        </td>

        <td class='align-middle'>${item.user}</td>
        <td class='align-middle'>${item.unitName}</td>
        <td class='align-middle'>${DateFormateddMMyyyyhhmmss(item.request_Date)}</td>
        <td class='da-td-mid-center'>
            ${item.ddgiT_Approval_dat ? DateFormateddMMyyyyhhmmss(item.ddgiT_Approval_dat) : "-"}
        </td>

        <td class='align-middle text-start'>${formatRemarks(item.remarks)}</td>

        ${buildActionButtons(item, isApproved)}
    </tr>`;
}

// 🔹 SHORT NAME
function getShortName(name) {
    const words = name.split(" ");
    return words.length > 6 ? words.slice(0, 6).join(" ") + "..." : name;
}

// 🔹 ACTION BUTTONS
function buildActionButtons(item, isApproved) {

    if (isApproved) {
        return `
        <td class='align-middle d-flex'>
            <button class='btn btn-success btn-sm approve-btn'
                data-bold="${item.isRead}"
                data-id="${item.id}"
                data-project-name="${item.projName}"
                data-actiontype="4" disabled>Approved</button>

            <button class='btn btn-warning btn-sm ml-2 approve-btn'
                data-id="${item.id}"
                data-project-name="${item.projName}"
                data-actiontype="3" title="Request Reject">Reject</button>

            ${buildHistoryIcon(item.projId)}
        </td>`;
    }

    return `
    <td class='align-middle d-flex'>
        <button class='btn btn-danger btn-sm approve-btn'
            data-bold="${item.isRead}"
            data-id="${item.id}"
            data-project-name="${item.projName}"
            data-actiontype="2">Approve</button>

        <button class='btn btn-warning btn-sm ml-2 approve-btn'
            data-id="${item.id}"
            data-project-name="${item.projName}"
            data-actiontype="3" title="Request Reject">Reject</button>

        ${buildHistoryIcon(item.projId)}
    </td>`;
}

// 🔹 HISTORY ICON
function buildHistoryIcon(projId) {
    return `
    <a href="#" class="ml-2 LegacyHistory" data-action="LegacyHistory" data-ids="${projId}" title="History of the Legacy">
        <img src="/assets/images/icons/Legacyhistory.png" alt="Icon" class="da-ico-27">
    </a>`;
}

// 🔹 BADGE UPDATE
function updateBadge(count) {
    const badge = document.getElementById("IngestionReqforother");

    if (!badge) return;

    if (count > 0) {
        badge.textContent = count;
        badge.classList.remove("d-none");
    } else {
        badge.textContent = '';
        badge.classList.add("d-none");
    }
}

// 🔹 ERROR HANDLER
function showError(message) {
    Swal.fire({
        title: 'Error',
        text: message,
        icon: 'error',
        confirmButtonText: 'OK'
    });
}

function handleProjectDetailsError(error) {
    console.error('Error fetching project details:', error);
}


    function formatRemarks(remarks) {
			if (!remarks) return "<span class='text-muted'>-</span>";

    let words = remarks.split(" ");
    let formatted = "";
    for (let i = 0; i < words.length; i++) {
        formatted += words[i] + " ";
    if ((i + 1) % 5 === 0) {
        formatted += "<br>";
				}
			}
    return formatted.trim();
		}
    $(document).ready(function () {
        loadDateApprovalTable();
		});



    function formatDate(dateStr) {
        let d = new Date(dateStr);
    return d.toLocaleDateString() + " " + d.toLocaleTimeString();
		}



