function mMsater(sectid = '', ddl, TableId, ParentId) {
    
    let userdata =
        {
            "id": TableId,
            "ParentId": ParentId,
        
       

    };
    if (ddl == 'ddlAction1' || ddl =='ddlSubStage1') {
        userdata.unitId = 1;
    }

    let encrypte_data = encryptData(userdata)
  
    $.ajax({
        url: '/Master/GetAllMasterTableforddl',
        contentType: 'application/x-www-form-urlencoded',
        data: {
            encrypte_data: encrypte_data
        },
        type: 'POST',

        success: function (response) {
           
            if (response != "null" && response != null) {
                if (response == 0) {
                    listItemddl += '<option  Value = "", Disabled = true, Selected = true>--Select--</option>';
                    
                    $("#" + ddl + "").html(listItemddl);
                }

                else {

                    let listItemddl = "";

                    
                    listItemddl += '<option  Value = "", Disabled = true, Selected = true>--Select--</option>';


                    for (let i = 0; i < response.length; i++) {
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
   
    const userdata = {
        "id": TableId,
        "ParentId": ParentId,
    };
    let encrypte_data = encryptData(userdata)
    
    $.ajax({
        url: '/Master/GetAllMasterTableforddl',
        contentType: 'application/x-www-form-urlencoded',
        data: {encrypte_data: encrypte_data
    },
        type: 'POST',
        success: function (response) {
            handleDropdownResponse(response, ddl, sectid, type, projecttype);
        },
        error: function () {
            Swal.fire({ text: "Error" });
        }
    });
}

// 🔹 MAIN HANDLER
function handleDropdownResponse(response, ddl, sectid, type, projecttype) {

    if (!isValidResponse(response)) return;

    let listItemddl = buildDefaultOption();

    if (response == 0) {
        renderDropdown(ddl, listItemddl);
        return;
    }

    listItemddl += buildOptions(response, projecttype);

    renderDropdown(ddl, listItemddl);

    setSelectedValue(ddl, sectid);

    handleStageRemoval(type);
}

// 🔹 VALIDATION
function isValidResponse(response) {
    return response !== "null" && response !== null;
}

// 🔹 DEFAULT OPTION
function buildDefaultOption() {
    return '<option value="">Please Select</option>';
}

// 🔹 BUILD OPTIONS
function buildOptions(response, projecttype) {
  
    let html = '';
   
    response.forEach(item => {
        if (projecttype === "Re-Vetted") {
            if (item.id == 3) {
                html += '<option value="' + item.id + '"selected>' + item.name + '</option>';

            }
        }
        else if (projecttype === 1) {
            if (item.id == 1) {
                html += '<option value="' + item.id + '" selected>' + item.name + '</option>';

            }
        } else {

            html += '<option value="' + item.id + '">' + item.name + '</option>';
        }
    });

    return html;
}


// 🔹 RENDER
function renderDropdown(ddl, html) {
    $("#" + ddl).html(html);
}

// 🔹 SET VALUE
function setSelectedValue(ddl, sectid) {
    if (sectid != '') {
        $("#" + ddl).val(sectid);
    }
}

// 🔹 STAGE LOGIC (UNCHANGED)
function handleStageRemoval(type) {

    if (type == 2) {
        removeStageOptions();
        return;
    }

    if (type == 1) {
        const isProcess = $(".SpnprojectIsProcess").first().html();

        if (isProcess == 'False') {
            removeStageOptions();
        }
    }
}

// 🔹 REMOVE OPTIONS
function removeStageOptions() {
    $("#ddlfwdStage option[value='2']").remove();
    $("#ddlfwdStage option[value='3']").remove();
}
function mMsaterStage(sectid = '', ddl, TableId, ParentId, StakeHolderId) {

    const userdata = {
        "id": TableId,
        "ParentId": ParentId,
        "StakeHolderId": StakeHolderId,
    };
    let encrypted_Payload = encryptData(userdata)

    $.ajax({
        url: '/Master/GetStagebyStakeHolderId',
        contentType: 'application/x-www-form-urlencoded',
        data: {
            encrypted_Payload: encrypted_Payload
    },
        type: 'POST',
        success: function (response) {
            handleStageResponse(response, ddl, sectid);
        },
        error: function () {
            Swal.fire({ text: "Error" });
        }
    });
}

// 🔹 MAIN HANDLER
function handleStageResponse(response, ddl, sectid) {

    if (!isValidResponse(response)) return;

    let listItemddl = buildDefaultOption();

    if (response == 0) {
        renderDropdown(ddl, listItemddl);
        return;
    }

    listItemddl += buildStageOptions(response);

    renderDropdown(ddl, listItemddl);

    setSelectedValue(ddl, sectid);
}

// 🔹 VALIDATION (SAME LOGIC)
function isValidResponse(response) {
    return response !== "null" && response !== null;
}

// 🔹 DEFAULT OPTION
function buildDefaultOption() {
    return '<option value="">Please Select</option>';
}

// 🔹 OPTIONS BUILDER (UNCHANGED LOGIC)
function buildStageOptions(response) {
    let html = '';

    response.forEach(item => {
        html += `<option value="${item.id}">${item.name}</option>`;
    });

    return html;
}

// 🔹 RENDER
function renderDropdown(ddl, html) {
    $("#" + ddl).html(html);
}

// 🔹 SET VALUE
function setSelectedValue(ddl, sectid) {
    if (sectid != '') {
        $("#" + ddl).val(sectid);
    }
}

function mMsaterFwdTo(sectid = '', ddl, TableId, ParentId, StakeHolderId, type, value) { 


    let userdata =
    {
        "id": TableId,
        "ParentId": ParentId,
        "StakeHolderId": StakeHolderId,
        "Type": type,
        "Value": value

    };

    let encrypted_payload = encryptData(userdata)
    $.ajax({
        url: '/Master/GetFwdTo',
        contentType: 'application/x-www-form-urlencoded',
        data: {encrypted_payload: encrypted_payload
    },
        type: 'POST',

        success: function (response) {
          
            if (response != "null" && response != null) {
                if (response == 0) {
                    listItemddl += '<option value="">Please Select</option>';
                    $("#" + ddl + "").html(listItemddl);
                }

                else {
                    if (type == 1) {

                        let listItemddl = "";
                        if (ddl != "ddlfwdCCTo")
                        listItemddl += '<option value="">Please Select</option>';


                        for (let i = 0; i < response.length; i++) {
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
                        let listItemddl = "";

                        listItemddl += '<option value="">Please Select</option>';


                        for (let i = 0; i < response.length; i++) {
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
    let user_ids =
    {
        "PsmId": 0,

        "ProjId": projectid
    }

    let encrypted_ids = encryptData(user_ids)
    $.ajax({
        type: "POST",
        url: '/Projects/GetAllCommentBypsmId_UnitId',
        data: {
            encrypted_ids: encrypted_ids
        },
        success: function (data) {

            let commentContainer = '';
            if (data != null) {

                for (let i = 0; i < data.length; i++) {
                    let date = new Date(data[i].date);
                    let formattedDate = ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();

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


    let datef2 = new Date(date);
    let months = "" + `${(datef2.getMonth() + 1)}`;
    let days = "" + `${(datef2.getDate())}`;
    let pad = "00"
    let monthsans = pad.substring(0, pad.length - months.length) + months
    let dayans = pad.substring(0, pad.length - days.length) + days
    let year = `${datef2.getFullYear()}`;
    if (year > 1902) {

        let datemmddyyyy = dayans + `/` + monthsans + `/` + year
        return datemmddyyyy;
    }
    else {
        return '';
    }
}


function DateTimeFormatedd_mm_yyyy(date) {
    let dateObj = new Date(date); // Convert input date to Date object
    if (Object.prototype.toString.call(dateObj) !== "[object Date]" || isNaN(dateObj.getTime())) {
        return ''; // Return empty string if date is invalid
    }
    let day = dateObj.getDate().toString().padStart(2, '0');
    let month = (dateObj.getMonth() + 1).toString().padStart(2, '0');
    let year = dateObj.getFullYear();
    let hours = dateObj.getHours().toString().padStart(2, '0');
    let minutes = dateObj.getMinutes().toString().padStart(2, '0');
    let seconds = dateObj.getSeconds().toString().padStart(2, '0');
    let formattedDateTime = day + '/' + month + '/' + year + ' ' + hours + ':' + minutes + ':' + seconds;

    return formattedDateTime;
}
function DateTimeFormatedd_dd_mm_yyyy(dateString) {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');  // Add leading zero if single digit
    const month = String(date.getMonth() + 1).padStart(2, '0');  // Get month (0-based)
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

