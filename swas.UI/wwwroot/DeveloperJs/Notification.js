

$(document).ready(function () {
   
    updateNotificationCountForChat(3, 'InterUserMsgCount'); // For Inter-User Msg

   
  
    fetchProjectCommentsUnreadCount(); // For DateApproval
});

function AddNotification(ProjId, type, unitid) {
  
    
    $.ajax({
        url: '/Notification/AddNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type,
            "unitid": unitid, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response && response === 1) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Project Submit Successfully',
                    showConfirmButton: false,
                    timer: 700
                });
            }
            updateNotificationCountForChat(3, 'InterUserMsgCount'); // For Inter-User Msg

        },
        error: function (error) {
            console.error('Error occurred:', error);
        }
    });
}

function IsReadNotification(ProjId, type) {
    $.ajax({
        url: '/Notification/IsReadNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type
        },
        success: function (response) {
            updateNotificationCountForChat(3, 'InterUserMsgCount'); // For Inter-User Msg
        }
    });
}


function UndoNotification(ProjId, type, ToUnitId) {

    $.ajax({
        url: '/Notification/UndoNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type,
            "ToUnitId": ToUnitId
        },
        success: function (response) {

        }
    });
}




function UnReadNotification(ProjId, type) {
    $.ajax({
        url: '/Notification/UnReadNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type
        },
        success: function (response) {

        }
    });
}
function updateNotificationCount(type, elementId) {
    $.ajax({
        url: `/Notification/GetNotificationCount?type=${type}`, // Pass the notification type as a query parameter
        type: 'GET',
        success: function (count) {
            count = count || 0;
            $('.' + elementId).html(count);
            if (count > 0) {
                
                $("." + elementId).removeClass("d-none")
            } else {
                
                $("." + elementId).addClass("d-none")
            }
        },
        error: function () {
            console.error(`Could not retrieve ${elementId} count.`);
            $(`#${elementId}`).text(0).hide();
        }
    });
}

function updateNotificationCountForChat(type, elementId) {
 
    $.ajax({
        url: `/Notification/GetNotificationCountForChat`,  // Pass the notification type as a query parameter
        type: 'GET',
        success: function (count) {
            count = count || 0;
            if (count > 0) {
                $("." + elementId).removeClass("d-none")
                
            } else {
                $("." + elementId).addClass("d-none").empty();
            }
        },
        error: function () {
            console.error(`Could not retrieve ${elementId} count.`);
            $(`#${elementId}`).text(0).hide();
        }
    });
}


function fetchProjectCommentsUnreadCount() {

    $.ajax({
        url: '/Notification/GetUnreadProjectCommentsCount',
        type: 'get',

        success: function (response) {

            const count = response.count;
            const badge = document.getElementById("ProjectlegacyCount");

            if (badge) {
                if (count > 0) {
                    badge.textContent = count;
                    badge.classList.remove("d-none");
                } else {
                    badge.classList.add("d-none");
                }
            }
        },
        error: function (err) {
            console.error('Error fetching unread project comment count:', err);
        }
    });
}