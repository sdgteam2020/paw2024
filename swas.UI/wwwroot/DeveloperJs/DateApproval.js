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

$(document).ready(function () {
    debugger;
    $('.tabs__head a').click(function (e) {
        e.preventDefault();

        // Remove active class from all tabs
        $('.tabs__head a').removeClass('active-link');

        // Add active class to clicked tab
        $(this).addClass('active-link');

        // Hide all tab contents
        $('.tab-content').hide();

        // Show the one with the matching data-tab id
        var tabId = $(this).data('tab');
        $('#' + tabId).show();
    });
});


$(document).on('click', '.approve-btn', function () {
    debugger;
    let id = Number($(this).data('id')) || 0;
    let projid = $(this).data('ids')

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
                getProjectDetails();
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
                    listitem += '<div class="timeline-date">' + DateTimeFormatedd_mm_yyyy(item.actionDate) + '</div>';

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
                        listitem += '<div class="box-title"><i class="fa fa-pencil text-info"></i> Remarks On ' + DateTimeFormatedd_mm_yyyy(item.actionDate) + '</div>';
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

//function initializeDataTable(tableSelector) {
//    return $(tableSelector).DataTable({
//        lengthChange: true,
//        dom: 'lBfrtip',
//        retrieve: true,
//        destroy: true, // Use `destroy` instead of `bDestroy` (deprecated)
//        pageLength: -1,
//        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
//        buttons: [
//            {
//                extend: 'excel',
//                text: 'Excel',
//                exportOptions: {
//                    columns: ':visible:not(:last-child)',
//                    format: {
//                        body: function (data, row, column, node) {
//                            var text = typeof data === 'string' && data.indexOf('<') >= 0 ? $(data).text().trim() : data;
//                            return column === 0 ? row + 1 : text;
//                        }
//                    }
//                }
//            },
//            {
//                extend: 'csv',
//                exportOptions: {
//                    columns: ':visible:not(:last-child)',
//                    format: {
//                        body: function (data, row, column, node) {
//                            var text = typeof data === 'string' && data.indexOf('<') >= 0 ? $(data).text().trim() : data;
//                            return column === 0 ? row + 1 : text;
//                        }
//                    }
//                }
//            },
//            {
//                extend: 'pdfHtml5',
//                text: 'PDF',
//                exportOptions: {
//                    columns: ':visible:not(:last-child)'
//                },
//                action: function (e, dt, node, config) {
//                    PdfDiv(); // You can pass tableSelector if needed
//                }
//            }
//        ],
//        searchBuilder: {
//            conditions: {
//                num: {
//                    'MultipleOf': {
//                        conditionName: 'Multiple Of',
//                        init: function (that, fn, preDefined = null) {
//                            var el = $('<input>').on('input', function () { fn(that, this); });
//                            if (preDefined !== null) {
//                                $(el).val(preDefined[0]);
//                            }
//                            return el;
//                        },
//                        inputValue: function (el) {
//                            return $(el[0]).val();
//                        },
//                        isInputValid: function (el, that) {
//                            return $(el[0]).val().length !== 0;
//                        },
//                        search: function (value, comparison) {
//                            return value % comparison === 0;
//                        }
//                    }
//                }
//            }
//        }
//    });
//}
function loadDateApprovalTable() {
    $.ajax({
        url: '/Home/GetDateApprovalList',
        method: 'GET',
        
        success: function (response) {
            


            let listItem = '';
            let count = 0; // 👈 Move outside the loop
            const badge = document.getElementById("IngestionReq");
            for (let i = 0; i < response.length; i++) {




                let item = response[i];
                if (response[i].isRead == false) {
                    count++;

                    listItem += "<tr class='bold-text'>";
                    /* boldCount++;*/
                } else {
                    listItem += "<tr>";
                }
                var projName = item.projName;
                var words = projName.split(" ");
                var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;



                listItem += "<td class='align-middle '>" + (i + 1) + "</td>";
                /* listItem += "<td id='dateApprovalProjName' class='align-middle'>" + shortProjName + "</td>";*/
                listItem += "<td class='align-middle'>";
                listItem += "<a  href='/Projects/ProjHistory?EncyID=" + encodeURIComponent(item.encyID) + "'>";
                listItem += "<span id='projectName' class='projNameDetail' >" + shortProjName + "</span>";
                listItem += "</a>";
                listItem += "</td>";
                listItem += "<td class='align-middle'>" + item.user + "</td>";
                listItem += "<td class='align-middle'>" + item.unitName + "</td>";
                listItem += "<td class='align-middle'>" + formatDate(item.request_Date) + "</td>";
                //listItem += "<td class='align-middle'>" + formatDate(item.ddgiT_Approval_dat) + "</td>";
                listItem += "<td style='Vertical-align:middle; text-align:center'>" + (item.ddgiT_Approval_dat ? formatDate(item.ddgiT_Approval_dat) : "-") + "</td>";

                // Fixed approval status check
                let isApproved = item.ddgiT_approval === true || item.ddgiT_approval === "true";
                listItem += "<td class='align-middle text-start'>" + formatRemarks(item.remarks) + "</td>";



                if (isApproved) {
                    listItem += `<td class='align-middle d-flex'>
				<button class='btn btn-success btn-sm approve-btn'
					data-bold="${item.isRead}"
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="4"  disabled>Approved</button> <!-- 2 for Unapprove -->

						<button class='btn btn-warning btn-sm ml-2  approve-btn'
					data-id="${item.id}"
					data-project-name="${item.projName}"
							data-actiontype="3"  title="Request Reject">Reject</button>

				<a href="#" class="ml-2 LegacyHistory" data-action="LegacyHistory" data-ids="${item.projId}" title="History of the Legacy">
					<img src="/assets/images/icons/Legacyhistory.png" alt="Icon" style="height: 27px;">
				</a>
			</td>`;
                } else {
                    listItem += `<td class='align-middle d-flex'>
				<button class='btn btn-danger  btn-sm approve-btn'
					data-bold="${item.isRead}"
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="2">Approve</button> <!-- 1 for Approve -->

						<button class='btn btn-warning btn-sm ml-2  approve-btn'
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="3" title="Request Reject" >Reject</button>

			 <!-- 3 for Reject -->

				<a href="#" class="ml-2  LegacyHistory" data-action="LegacyHistory"  data-ids="${item.projId}" title="History of the Legacy">
					<img src="/assets/images/icons/Legacyhistory.png" alt="Icon" style="height: 27px;">
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

            // $("#DetailBody").html(listItem);
            // var table = $('#TableType1').DataTable({
            // 	lengthChange: true,
            // 	dom: 'lBfrtip',
            // 	retrieve: true,
            // 	bDestroy: true,
            // 	pageLength: -1,
            // 	lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
            // 	buttons: [
            // 		{
            // 			extend: 'excel',
            // 			text: 'Excel',
            // 			exportOptions: {
            // 				columns: ':visible:not(:last-child)',
            // 				format: {
            // 					// body: function (data, row, column, node) {
            // 					//     var excelRowData = $(data).text().trim();
            // 					//     return column === 0 ? row + 1 : excelRowData;
            // 					// }
            // 							body: function (data, row, column, node) {
            // 							var text = typeof data === 'string' && data.indexOf('<') >= 0 ? $(data).text().trim() : data;
            // 							return column === 0 ? row + 1 : text;
            // 							}

            // 				}
            // 			}
            // 		},
            // 		{
            // 			extend: 'csv',
            // 			exportoptions: {
            // 				columns: ':visible:not(:last-child)',
            // 				format: {
            // 					body: function (data, row, column, node) {
            // 						var csvrowdata = $(data).text().trim();
            // 						return column === 0 ? row + 1 : csvrowdata; // fix ser no in export
            // 					}
            // 				}
            // 			}
            // 		},
            // 		{
            // 			extend: 'pdfHtml5',
            // 			text: 'PDF',
            // 			exportOptions: {
            // 				columns: ':visible:not(:last-child)'
            // 			},
            // 			action: function (e, dt, node, config) {
            // 				PdfDiv();
            // 			}
            // 		}
            // 	],
            // 	searchBuilder: {
            // 		conditions: {
            // 			num: {
            // 				'MultipleOf': {
            // 					conditionName: 'Multiple Of',
            // 					init: function (that, fn, preDefined = null) {
            // 						var el = $('<input>').on('input', function () { fn(that, this); });
            // 						if (preDefined !== null) {
            // 							$(el).val(preDefined[0]);
            // 						}
            // 						return el;
            // 					},
            // 					inputValue: function (el) {
            // 						return $(el[0]).val();
            // 					},
            // 					isInputValid: function (el, that) {
            // 						return $(el[0]).val().length !== 0;
            // 					},
            // 					search: function (value, comparison) {
            // 						return value % comparison === 0;
            // 					}
            // 				}
            // 			}
            // 		}
            // 	}
            // });
        },
        error: function () {
            console.error('Error fetching data');
        }
    });
}




//$("#Find_forApproval").on("keyup", function () {

//    var query = $(this).val();

//        filterProjectNames(query);

//});
//function filterProjectNames(query) {

//    $.ajax({
//        url: '/Projects/GetRevettedProjects',
//        method: 'GET',
//        data: {
//            searchQuery: query
//        },
//        success: function (data) {

//            $("#projectNameDropdown").empty();
//            if (data.length > 0) {
//                data.forEach(function (name) {
//                    $("#projectNameDropdown").append(`<li class="dropdown-item" data-id="${name.projId}">${name.projName}</li>`);
//                });
//                $("#projectNameDropdown").show();
//            } else {
//                $("#projectNameDropdown").hide();
//            }
//        },
//        error: function (error) {
//            console.error('Error fetching project names:', error);
//        }
//    });
//}
//$(document).on("click", "#projectNameDropdown li", function () {
//    var projectId = $(this).data("id");
//    var projectName = $(this).data("name");


//    $("#ProjName").val(projectName);


//    $("#ProjId").val(projectId);


//    getProjectDetails(projectId);

//    $("#projectNameDropdown").hide();
//});


$(document).ready(function () {
    getProjectDetails();
    bindLiveProjectSearch(
        "#Find_forApproval",                  // Input field
        "#projectNameDropdown",
        "/Projects/GetProjectByKeyup",// Dropdown UL
             // API endpoint
        function (projId, projName, remarks) {        // On select
            $("#ProjName").val(projName);
            $("#ProjId").val(projId);
            getProjectDetails(projId,remarks);       // Your custom logic
        }
    );
});






//function bindLiveProjectSearch(inputSelector, dropdownSelector, onItemSelect) {
//    debugger;
 
//    $(inputSelector).on("keyup", function () {
        
//        debugger;
//        let query = $(this).val();
//        const validpattern = /^[a-zA-Z0-9]*$/;

//        if (!validpattern.test(query)) {
//            Swal.fire({
//                icon: 'error',
//                title: 'Invalid Input',
//                text: 'Special Characters are not allowed',
//            });
//            $(this).val(query.Replace(/[a-zA-Z0-9]/g, ' '));
//            return;
//        }

//        if (query.length < 2) {
//            $(dropdownSelector).hide();
//            return;
//        }

       
           
//        $.ajax({
//            url: '/Projects/GetProjectByKeyup',
//            type: 'GET',
//            data: { searchQuery: query },

//            success: function (data) {
//                $(dropdownSelector).empty();
//                if (data.length > 0) {
//                    data.forEach(function (item) {
//                        $(dropdownSelector).append(`
//                            <li class="dropdown-item" data-id="${item.projId}" data-name="${item.projName}">${item.projName}</li>
//                        `);
//                    });
//                    $(dropdownSelector).show();
//                } else {
//                    $(dropdownSelector).hide();
//                }
//            },
//            error: function (err) {
//                console.error("Error fetching project data:", err);
//                $(dropdownSelector).hide();
//            }
//        });
//    });

//    $(document).on("click", `${dropdownSelector} li`, function () {
//        const projId = $(this).data("id");

//        const projName = $(this).data("name");

//        if (typeof onItemSelect === "function") {
//            onItemSelect(projId, projName);
//        }

//        $(dropdownSelector).hide();
//    });

//    // Optional: Hide dropdown when clicking outside
//    $(document).on("click", function (e) {
//        if (!$(e.target).closest(inputSelector).length && !$(e.target).closest(dropdownSelector).length) {
//            $(dropdownSelector).hide();
//        }
//    });
//}






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

            debugger;
            if (response.success === false) {
                Swal.fire({
                    title: 'Error',
                    text: response.message,  // Show the message from the server (like "Record already exists")
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                return;  // Stop further execution if error
            }
          
           

            let listItem = '';
            let count = 0; // 👈 Move outside the loop
            const badge = document.getElementById("IngestionReqforother");
            for (let i = 0; i < response.length; i++) {

                


                let item = response[i];
                if (response[i].isRead == false) {

                    
                    count++;
                






                    listItem += "<tr class='bold-text'>";
                    /* boldCount++;*/
                } else {
                    listItem += "<tr>";
                }
                var projName = item.projName;
                var words = projName.split(" ");
                var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;



                listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                /*listItem += "<td id='dateApprovalProjName' class='align-middle'>" + shortProjName + "</td>";*/
                listItem += "<td class='align-middle'>";
                listItem += "<a  href='/Projects/ProjHistory?EncyID=" + encodeURIComponent(item.encyID) + "'>";
                listItem += "<span id='projectName' class='projNameDetail' >" + shortProjName + "</span>";
                listItem += "</a>";
                listItem += "</td>";
                listItem += "<td class='align-middle'>" + item.user + "</td>";
                listItem += "<td class='align-middle'>" + item.unitName + "</td>";
                listItem += "<td class='align-middle'>" + formatDate(item.request_Date) + "</td>";
                //listItem += "<td class='align-middle'>" + formatDate(item.ddgiT_Approval_dat) + "</td>";
                listItem += "<td style='Vertical-align:middle; text-align:center'>" + (item.ddgiT_Approval_dat ? formatDate(item.ddgiT_Approval_dat) : "-") + "</td>";

               
                listItem += "<td class='align-middle text-start'>" + formatRemarks(item.remarks) + "</td>";
                // Fixed approval status check
                let isApproved = item.ddgiT_approval === true || item.ddgiT_approval === "true";


                if (isApproved) {
                    listItem += `<td class='align-middle d-flex'>
				<button class='btn btn-success btn-sm approve-btn'
					data-bold="${item.isRead}"
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="4"  disabled>Approved</button> <!-- 2 for Unapprove -->

						<button class='btn btn-warning btn-sm ml-2  approve-btn'
					data-id="${item.id}"
					data-project-name="${item.projName}"
							data-actiontype="3"  title="Request Reject">Reject</button>

				<a href="#" class="ml-2 LegacyHistory" data-action="LegacyHistory" data-ids="${item.projId}" title="History of the Legacy">
					<img src="/assets/images/icons/Legacyhistory.png" alt="Icon" style="height: 27px;">
				</a>
			</td>`;
                } else {
                    listItem += `<td class='align-middle d-flex'>
				<button class='btn btn-danger  btn-sm approve-btn'
					data-bold="${item.isRead}"
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="2">Approve</button> <!-- 1 for Approve -->

						<button class='btn btn-warning btn-sm ml-2  approve-btn'
					data-id="${item.id}"
					data-project-name="${item.projName}"
					data-actiontype="3" title="Request Reject" >Reject</button>

			 <!-- 3 for Reject -->

				<a href="#" class="ml-2  LegacyHistory" data-action="LegacyHistory"  data-ids="${item.projId}" title="History of the Legacy">
					<img src="/assets/images/icons/Legacyhistory.png" alt="Icon" style="height: 27px;">
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
