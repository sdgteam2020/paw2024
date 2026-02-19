

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
   

    var listitem = "";

    $.ajax({
        url: '/Projects/GetProjectLegacyHistory',
        type: 'POST',
        data: { ProjectId: ProjId },
        success: function (response) {
            console.log(response);
           
          

            if (response.length > 0) {
                let listitem = '<div class="timeline-month">';
                listitem += '<span>' + response.length + ' Entries</span>';
                listitem += '</div>';

                for (let i = 0; i < response.length; i++) {
                    if (response[0])
                    {
                        $("#lblHistory").text("Project Name: " + response[0].projectName + " History");
                    }
                    const item = response[i];

                    listitem += '<div class="timeline-section">';
                    listitem += '<div class="timeline-date">' + DateFormateddMMyyyyhhmmss(item.actionDate) + '</div>';
                    listitem += '<div class="row g-3">';
                    listitem += '<div class="col-md-6">';
                    listitem += '<div class="timeline-box">';
                    if (item.actionType === 1) {
                        listitem += '<div class="box-title bg-warning text-white"><i class="fa-solid fa-forward"></i> ' + item.actionTypeText + '</div>';
                    } else if (item.actionType === 2) {
                        listitem += '<div class="box-title bg-success text-white"><i class="fa-solid fa-circle-check"></i> ' + item.actionTypeText + '</div>';
                    } else if (item.actionType === 3 || item.actionType === 4) {
                        listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-rotate-left"></i> ' + item.actionTypeText + '</div>';
                    } else {
                        listitem += '<div class="box-title bg-secondary text-white">' + item.actionTypeText + '</div>';
                    }
                    listitem += '<div class="box-content">';
                    if (item.actionType === 1) {
                        listitem += '<div class="row mb-1"><div class="col-4"><strong> Request By</strong>:</div><div class="col-8"><span class="badge bg-secondary">' + (item.fromunitName || 'N/A') + '</span></div></div>';
                    }

                    if (item.actionType === 3 || item.actionType === 4 || item.actionType === 2) {
                        listitem += '<div class="row mb-1"><div class="col-4"><strong>' + item.actionTypeText + ' By</strong>:</div><div class="col-8"><span class="badge bg-secondary">' + (item.fromunitName || 'N/A') + '</span></div></div>';

                    }
                    listitem += '</div>'; // box-content

                    listitem += '<div class="box-footer">' + (item.userdetails || 'Unknown User') + '</div>';
                    listitem += '</div></div>'; // End Action Box
                    if (item.remarks) {
                        listitem += '<div class="col-md-6">';
                        listitem += '<div class="timeline-box">';
                        listitem += '<div class="box-title"><i class="fa fa-pencil text-info"></i> Remarks On ' + DateFormateddMMyyyyhhmmss(item.actionDate) + '</div>';
                        listitem += '<div class="box-content">';
                        listitem += '<div class="box-item">' + item.remarks + '</div>';
                        listitem += '</div>';
                        listitem += '<div class="box-footer">' + (item.userdetails || 'Unknown User') + '</div>';
                        listitem += '</div></div>';
                    }

                    listitem += '</div>'; // End row
                    listitem += '</div>'; // End timeline-section
                }
               
                $("#projectLegacyhistory").html(listitem);
            } else {
                $("#projectLegacyhistory").html('<div class="alert alert-info">No history available.</div>');
            }

        }
    });

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
        success: function (response) {

            
            if (response.success === false) {
                Swal.fire({
                    title: 'Error',
                    text: response.message,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                return;
            }

            let listItem = '';
            let count = 0;
            const badge = document.getElementById("IngestionReqforother");

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

                listItem += "<td class='align-middle'>" + (i + 1) + "</td>";

                listItem += "<td class='align-middle'>";
                listItem += "<a  href='/Projects/ProjHistory?EncyID=" + encodeURIComponent(item.encyID) + "'>";
                listItem += "<span id='projectName' class='projNameDetail' >" + shortProjName + "</span>";
                listItem += "</a>";
                listItem += "</td>";

                listItem += "<td class='align-middle'>" + item.user + "</td>";
                listItem += "<td class='align-middle'>" + item.unitName + "</td>";
                listItem += "<td class='align-middle'>" + DateFormateddMMyyyyhhmmss(item.request_Date) + "</td>";
                listItem += "<td class='da-td-mid-center'>" + (item.ddgiT_Approval_dat ? DateFormateddMMyyyyhhmmss(item.ddgiT_Approval_dat) : "-") + "</td>";

                listItem += "<td class='align-middle text-start'>" + formatRemarks(item.remarks) + "</td>";

                let isApproved = item.ddgiT_approval === true || item.ddgiT_approval === "true";

                if (isApproved) {
                    listItem += `<td class='align-middle d-flex'>
				<button class='btn btn-success btn-sm approve-btn'
					data-bold="${item.isRead}"
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="4" disabled>Approved</button>

				<button class='btn btn-warning btn-sm ml-2 approve-btn'
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="3" title="Request Reject">Reject</button>

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

            $('#DateApproval1').html(listItem);
            initializeDataTable('#TableType2');
            fetchProjectCommentsUnreadCount();
        },
        error: function (error) {
            console.error('Error fetching project details:', error);
        }
    });
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



