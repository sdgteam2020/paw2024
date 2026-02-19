function mMsater(sectid = '', ddl, TableId, ParentId) {
    
    var userdata =
        {
            "id": TableId,
            "ParentId": ParentId,
        
       

    };
    if (ddl == 'ddlAction1' || ddl =='ddlSubStage1') {
        userdata.unitId = 1;
    }
    $.ajax({
        url: '/Master/GetAllMasterTableforddl',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
           
            if (response != "null" && response != null) {
                if (response == 0) {
                    listItemddl += '<option  Value = "", Disabled = true, Selected = true>--Select--</option>';
                    
                    $("#" + ddl + "").html(listItemddl);
                }

                else {

                    var listItemddl = "";

                    
                    listItemddl += '<option  Value = "", Disabled = true, Selected = true>--Select--</option>';


                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].id + '">' + response[i].name + '</option>';
                    }
                    $("#" + ddl + "").html(listItemddl);
                  
                        if (sectid != '') {
                            $("#" + ddl + "").val(sectid);

                        }


                }
            }
            else {
            }
        },
        error: function (result) {
            Swal.fire({
                text: "Error"
            });
        }
    });
}

function mMsaterfwdStage(sectid = '', ddl, TableId, ParentId, type, projecttype) {
    var userdata =
    {
        "id": TableId,
        "ParentId": ParentId,

    };
    $.ajax({
        url: '/Master/GetAllMasterTableforddl',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
           
            if (response != "null" && response != null) {
                if (response == 0) {
                    listItemddl += '<option value="">Please Select</option>';
                    $("#" + ddl + "").html(listItemddl);
                }

                else {

                    var listItemddl = "";

                    listItemddl += '<option value="">Please Select</option>';


                    for (var i = 0; i < response.length; i++) {
                        if (projecttype === "Re-Vetted") {
                            if (response[i].id == 3) {
                                listItemddl += '<option value="' + response[i].id + '" selected>' + response[i].name + '</option>';

                            }
                        }
                      else if (projecttype === 1) {
                            if (response[i].id == 1) {
                                listItemddl += '<option value="' + response[i].id + '" selected>' + response[i].name + '</option>';

                            }
                        } else {

                            listItemddl += '<option value="' + response[i].id + '">' + response[i].name + '</option>';
                        }
                    }
                    $("#" + ddl + "").html(listItemddl);
                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }
                    if (type == 1) {
                        if ($(this).closest("tr").find("#SpnprojectIsProcess").html() == 'False') {
                            $("#ddlfwdStage option[value='2']").remove();
                            $("#ddlfwdStage option[value='3']").remove();
                        }
                        else {
                               

                        }
                    }
                    else if (type == 2) {
                        $("#ddlfwdStage option[value='2']").remove();
                        $("#ddlfwdStage option[value='3']").remove();
                    }


                }
            }
            else {
            }
        },
        error: function (result) {
            Swal.fire({
                text: "Error"
            });
        }
    });
}
function mMsaterStage(sectid = '', ddl, TableId, ParentId, StakeHolderId) {
   
    var userdata =
    {
        "id": TableId,
        "ParentId": ParentId,
        "StakeHolderId": StakeHolderId,

    };
    $.ajax({
        url: '/Master/GetStagebyStakeHolderId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
           
            if (response != "null" && response != null) {
                if (response == 0) {
                    listItemddl += '<option value="">Please Select</option>';
                    $("#" + ddl + "").html(listItemddl);
                }

                else {

                    var listItemddl = "";

                    listItemddl += '<option value="">Please Select</option>';


                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].id + '">' + response[i].name + '</option>';
                    }
                    $("#" + ddl + "").html(listItemddl);
                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }


                }
            }
            else {
            }
        },
        error: function (result) {
            Swal.fire({
                text: "Error"
            });
        }
    });
}

function mMsaterFwdTo(sectid = '', ddl, TableId, ParentId, StakeHolderId, type, value) { 


    var userdata =
    {
        "id": TableId,
        "ParentId": ParentId,
        "StakeHolderId": StakeHolderId,
        "Type": type,
        "Value": value

    };
    $.ajax({
        url: '/Master/GetFwdTo',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
          
            if (response != "null" && response != null) {
                if (response == 0) {
                    listItemddl += '<option value="">Please Select</option>';
                    $("#" + ddl + "").html(listItemddl);
                }

                else {
                    if (type == 1) {

                        var listItemddl = "";
                        if (ddl != "ddlfwdCCTo")
                        listItemddl += '<option value="">Please Select</option>';


                        for (var i = 0; i < response.length; i++) {
                            listItemddl += '<option value="' + response[i].id + '">' + response[i].name + '</option>';
                        }
                        $("#" + ddl + "").html(listItemddl);
                        if (response.length > 0) {
                            $("#searchBox").hide();
                            $("select[name='fwdoffrs']").show();
                        }

                        if (sectid != '') {
                            $("#" + ddl + "").val(sectid);

                        }

                    }
                    else {
                        var listItemddl = "";

                        listItemddl += '<option value="">Please Select</option>';


                        for (var i = 0; i < response.length; i++) {
                            listItemddl += '<option value="' + response[i].id + '">' + response[i].name + '</option>';
                        }

                        if (TableId == 8 && ddl == "ddlfwdFwdTo" && value !="edit") {

                            listItemddl += '<option value="More">More</option>';
                        }

                        $("#" + ddl + "").html(listItemddl);
                        if (sectid != '') {
                            $("#" + ddl + "").val(sectid);

                        }
                    }
                }
            }
            else {
            }
        },
        error: function (result) {
            Swal.fire({
                text: "Error"
            });
        }
    });
}
function GetAllComments(projectid) {
    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            "PsmId": 0,
            "stakeholderId": 1,
            "ProjId": projectid
        },
        success: function (data) {

            var commentContainer = '';
            if (data != null) {

                for (var i = 0; i < data.length; i++) {
                    var date = new Date(data[i].date);
                    var formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

                    commentContainer += '<div class="comment-box">'; // Use class for text alignment
                    commentContainer += '<div class="comment-header">';
                    commentContainer += '<div>';
                    commentContainer += '<span class="comment-stakeholder">' + data[i].stakeholder + '</span>';
                    commentContainer += '<span class="comment-meta">' + formattedDate + '</span>';
                    commentContainer += '</div>';
                    commentContainer += '<div>';
                    if (data[i].status == "Accepted")
                        commentContainer += '<span class="comment-status accepted">' + data[i].status + '</span>';
                    else
                        commentContainer += '<span class="comment-status rejected">' + data[i].status + '</span>';

                    commentContainer += '<span class="pdf-link">'; // Move the PDF link to the same line as status

                    if (data[i].state !== null) {

                        commentContainer += '<a href="/Home/WaterMark3?id=' + data[i].attpath + '" target="_blank">';
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
            }
        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}


function DateFormatedd_mm_yyyy(date) {


    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    if (year > 1902) {

        var datemmddyyyy = dayans + `/` + monthsans + `/` + year
        return datemmddyyyy;
    }
    else {
        return '';
    }
}


function DateTimeFormatedd_mm_yyyy(date) {
    var dateObj = new Date(date); // Convert input date to Date object
    if (Object.prototype.toString.call(dateObj) !== "[object Date]" || isNaN(dateObj.getTime())) {
        return ''; // Return empty string if date is invalid
    }
    var day = dateObj.getDate().toString().padStart(2, '0');
    var month = (dateObj.getMonth() + 1).toString().padStart(2, '0');
    var year = dateObj.getFullYear();
    var hours = dateObj.getHours().toString().padStart(2, '0');
    var minutes = dateObj.getMinutes().toString().padStart(2, '0');
    var seconds = dateObj.getSeconds().toString().padStart(2, '0');
    var formattedDateTime = day + '/' + month + '/' + year + ' ' + hours + ':' + minutes + ':' + seconds;

    return formattedDateTime;
}
function DateTimeFormatedd_dd_mm_yyyy(dateString) {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');  // Add leading zero if single digit
    const month = String(date.getMonth() + 1).padStart(2, '0');  // Get month (0-based)
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

