//$(document).on('click', '.approve-btn', function () {
//    let id = $(this).data('id');
//    let projectName = $(this).data('project-name');


//    let message = '';

//    if ($(this).text().trim() === 'Approved') {
//        message = `Do you want to Unapprove this date request for This Project: ${projectName} ?`;
//    } else {
//        message = `Do you want to approve this date request for This Project: ${projectName} ?`;
//    }


//    $.ajax({
//        url: '/Notification/GetUnreadProjectCommentsCount',
//        type: 'POST',
//        data: { id: id },
//        success: function (res) {
//            if (res.success) {

//                loadDateApprovalTable();
//                fetchProjectCommentsUnreadCount();
//                Swal.fire({
//                    title: 'Confirm Approval',
//                    text: message,
//                    icon: 'question',
//                    showCancelButton: true,
//                    confirmButtonColor: '#28a745',
//                    cancelButtonColor: '#d33',
//                    confirmButtonText: 'Yes, Approve',
//                    cancelButtonText: 'Cancel'
//                }).then((result) => {
//                    if (result.isConfirmed) {

//                        $.ajax({
//                            url: '/Home/ApproveDateRequest',
//                            type: 'POST',
//                            data: { id: id },
//                            success: function (res) {
//                                if (res.success) {
//                                    Swal.fire({
//                                        title: 'Approved!',
//                                        text: res.message,
//                                        icon: 'success',
//                                        timer: 2000,
//                                        showConfirmButton: false
//                                    });

//                                    loadDateApprovalTable();
//                                    fetchProjectCommentsUnreadCount();
//                                } else {
//                                    Swal.fire('Error!', res.message, 'error');
//                                }
//                            },
//                            error: function () {
//                                Swal.fire('Server Error', 'Could not approve the request.', 'error');
//                            }
//                        });
//                    }
//                });
//            } else {
//                Swal.fire('Error!', res.message, 'error');
//            }
//        },
//        error: function () {
//            Swal.fire('Server Error', 'Could not update read status.', 'error');
//        }
//    });
//});


$(document).on('click', '.approve-btn', function () {
    debugger;
    let id = $(this).data('id');
    let projectName = $(this).data('project-name');
    let isApproved = $(this).text().trim() === 'Approved';
   
    let actiontype = $(this).data('actiontype');
    let message= " ";
    if (actiontype === 3) {
       
       message = `Do you want to Reject this date request for this project: ${projectName}? Please enter remarks:`
    }
    else {
        message = `Do you want to Approve this date request for this project: ${projectName}? Please enter remarks:`;
}
   
         

    $.ajax({
        url: '/Notification/GetUnreadProjectCommentsCount',
        type: 'POST',
        data: { id: id },
        success: function (res) {
            if (res.success) {
                loadDateApprovalTable();
                fetchProjectCommentsUnreadCount();

                Swal.fire({
                    title: (actiontype === 3) ? 'Confirm Rejection' : 'Confirm Approval',
                    text: message,
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
    debugger;
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
           
           
            //if (response.length > 0) {
            //    debugger;
            //    let listitem = '<div class="timeline-month">';

            //    // Display current date and number of entries
            //    listitem += '<span>' + response.length + ' Entries</span>';
            //    listitem += '</div>';

            //    // Loop through history items
            //    for (let i = 0; i < response.length; i++) {
            //        debugger;

            //        // Start timeline section
            //        listitem += '<div class="timeline-section">';
            //        listitem += '<div class="timeline-date"> ' + response[i].actionDate + '</div>';

            //        // Start row for action details
            //        listitem += '<div class="row"><div class="col-sm-4">';
            //        listitem += '<div class="timeline-box">';

            //        // Display action title based on action type
            //        if (response[i].actionType === 1) {
            //            listitem += '<div class="box-title bg-warning text-white"><i class="fa-solid fa-forward"></i> ' + response[i].actionTypeText + '</div>';
            //        } else if (response[i].actionType === 3) {
            //            listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-rotate-left"></i> ' + response[i].actionTypeText + '</div>';
            //            //  listitem += '<div class="box-title bg-warning text-white"><i class="fa-solid fa-sent-left"></i> ' + response[i].actionTypeText + '</div>';
            //        } else if (response[i].actionType == 2) {
            //            listitem += '<div class="box-title bg-success text-white"><i class="fa-solid fa-circle-check"></i> ' + response[i].actionTypeText + '</div>';
            //        } else {
            //            listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-rotate-left"></i> ' + response[i].actionTypeText + '</div>';
            //        }

            //        listitem += '<div class="box-content">';

            //        listitem += '<div class="row"><div class="col-md-4"><div class="box-item"><strong>From</strong>: </div></div>';
            //        listitem += '<div class="col-md-8"><div class="box-item"><span class="rounded-pill bg-secondary" style="color: white;">' + (response[i].unitName || 'N/A') + '</span></div></div></div>';

            //        listitem += '<div class="row"><div class="col-md-4"><div class="box-item"><strong>To</strong>: </div></div>';
            //        listitem += '<div class="col-md-8"><div class="box-item"><span class="badge rounded-pill bg-secondary">' + (response[i].actionType || 'N/A') + '</span></div></div></div>';

            //        listitem += '</div>'; // End box-content
            //        listitem += '<div class="box-footer">' + (response[i].userdetails || 'Unknown User') + '</div>';
            //        listitem += '</div></div>';

            //        // Display remarks if available
            //        if (response[i].remarks) {
            //            listitem += '<div class="col-sm-4">';
            //            listitem += '<div class="timeline-box">';
            //            listitem += '<div class="box-title"><i class="fa fa-pencil text-info" aria-hidden="true"></i> Remarks On ' +response[i].actionDate + '</div>';
            //            listitem += '<div class="box-content">';
            //            listitem += '<div class="box-item">' + (response[i].actions && response[i].actions === null ? '<strong>Pulled Back Remarks</strong> - ' + response[i].remarks : response[i].remarks) + '</div>';
            //            listitem += '</div>';
            //            listitem += '<div class="box-footer">' + (response[i].actions === 2 ? '<div class="bg-warning">' : '') + (response[i].userdetails || 'Unknown User') + '</div>';
            //            listitem += '</div></div>';
            //        }

            //        listitem += '</div>'; // End timeline-section
            //    }

            //    // Display the history in the page
            //    $("#projectLegacyhistory").html(listitem);
            //}

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
                    listitem += '<div class="timeline-date">' + item.actionDate + '</div>';

                    // Start row for boxes
                    listitem += '<div class="row g-3">';

                    // Action Type Box
                    listitem += '<div class="col-md-6">';
                    listitem += '<div class="timeline-box">';

                    // Action Type Colors
                    if (item.actionType === 1) {
                        listitem += '<div class="box-title bg-warning text-white"><i class="fa-solid fa-forward"></i> ' + item.actionTypeText + '</div>';
                    } else if (item.actionType === 2) {
                        listitem += '<div class="box-title bg-success text-white"><i class="fa-solid fa-circle-check"></i> ' + item.actionTypeText + '</div>';
                    } else if (item.actionType === 3 || item.actionType === 4) {
                        listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-rotate-left"></i> ' + item.actionTypeText + '</div>';
                    } else {
                        listitem += '<div class="box-title bg-secondary text-white">' + item.actionTypeText + '</div>';
                    }

                    // Action details
                    listitem += '<div class="box-content">';
                    if (item.actionType === 1) {
                        listitem += '<div class="row mb-1"><div class="col-4"><strong> Request By</strong>:</div><div class="col-8"><span class="badge bg-secondary">' + (item.fromunitName || 'N/A') + '</span></div></div>';
                    }
                        // listitem += '<div class="row"><div class="col-4"><strong>To</strong>:</div><div class="col-8"><span class="badge bg-secondary">' + (item.actionType || 'N/A') + '</span></div></div>';

                    if (item.actionType === 3 || item.actionType === 4 || item.actionType === 2) {
                        listitem += '<div class="row mb-1"><div class="col-4"><strong>' + item.actionTypeText + ' By</strong>:</div><div class="col-8"><span class="badge bg-secondary">' + (item.fromunitName || 'N/A') + '</span></div></div>';
                      //  listitem += '<div class="row"><div class="col-4"><strong>To</strong>:</div><div class="col-8"><span class="badge bg-secondary">' + (item.actionType || 'N/A') + '</span></div></div>';

                    }
                    listitem += '</div>'; // box-content

                    listitem += '<div class="box-footer">' + (item.userdetails || 'Unknown User') + '</div>';
                    listitem += '</div></div>'; // End Action Box

                    // Remarks Box (if any)
                    if (item.remarks) {
                        listitem += '<div class="col-md-6">';
                        listitem += '<div class="timeline-box">';
                        listitem += '<div class="box-title"><i class="fa fa-pencil text-info"></i> Remarks On ' + item.actionDate + '</div>';
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