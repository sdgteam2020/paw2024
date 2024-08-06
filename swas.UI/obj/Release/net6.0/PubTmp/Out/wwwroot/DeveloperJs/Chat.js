$(document).ready(function () {
    $(document).ajaxStart(function () {
        $("#loading").hide();
    }).ajaxStop(function () {
        $("#loading").hide();
    }).ajaxError(function () {
        $("#loading").hide();
    });
    
    setInterval(function () {
        GetAllUsers()
        if ($("#spnUserMapChatId").html() != 0)
           
            UserChat($("#spnUserMapChatId").html(), $("#spnToUserIdMapChatId").html());

    }, 3000);
    
   
    $(".img_cont_msg").click(function () {
        alert(1)
    });
    $("#chatProfileSearch").keyup(function () {

        var armyno = $("#chatProfileSearch").val()
       
        GetAllUsers()

    });
    GetAllUsers()

    $("#type_msg").keypress(function () {
      
        $("#type_msg").removeClass('is-invalid');
    });
        $("#send_btn").click(function () {
            if ($("textarea#type_msg").val() == "") {
                $("textarea#type_msg").addClass('is-invalid');

            }
            else {
               
                SaveChat($("textarea#type_msg").val());
            }
    });
});
function SaveChat(Msg) {
    $.ajax({
        url: '/Chat/SaveChat',
        type: 'POST',
        data: { "UserMapChatId": $("#spnUserMapChatId").html(), "Msg": Msg, },
        success: function (response) {
            var listitem = "";
            if (response != null) {
                $("#type_msg").html(""); 
                
                UserChat($("#spnUserMapChatId").html(), $("#spnToUserIdMapChatId").html(), $("#profsortname").html());
                $("textarea#type_msg").val("");
                //Swal.fire({
                //    position: "top-end",
                //    icon: "success",
                //    title: "Chat Sent success",
                //    showConfirmButton: false,
                //    timer: 1500
                //});
            }
        }
    });
}
function GetAllUsers() {
    $.ajax({
        url: '/Chat/GetAllUsers',
        type: 'POST',
        data: { "Id": 0 },
        success: function (response) {
            console.log(response);
            if (response.length) {
                var listitem = "";
                for (var i = 0; i < response.length; i++) {
                   
                    if ($("#chatProfileSearch").val() == "" || response[i].offr_Name.toLowerCase().indexOf($("#chatProfileSearch").val().toLowerCase()) !== -1) {

                        listitem += '<li class="">';
                        listitem += '<div class="d-flex bd-highlight chatrequest">';
                        listitem += ' <div class="img_cont">';
                        //listitem += '<img src="/assets/images/icons/profilechat.png" class="rounded-circle user_img">';
                        listitem += '<div class="circleimg" style="background-color:' + displayFixedColorAlphabet(response[i].offr_Name.replace(/^\s+|\s+$/gm, '').substr(0, 1).toUpperCase()) + ';color:#ffff">' + response[i].offr_Name.replace(/^\s+|\s+$/gm, '').substr(0, 2).toUpperCase() +'</div>';
                        if (parseInt(response[i].total) > 0)
                            listitem += ' <span class="online_icon"></span>';
                        listitem += '</div>';
                        listitem += '<div class="user_info">';
                        listitem += '<span class="d-none" id="chatprofileId">' + response[i].id + '</span>';
                        listitem += '<span id="profName">' + response[i].rank + ' ' + response[i].offr_Name + '</span>';
                        listitem += ' <p>' + response[i].userName + '</p>';
                        listitem += '</div>';
                        listitem += '</div>';
                        listitem += ' </li>';

                    }


                }


                $(".contacts").html(listitem);

                $("body").unbind().on("click", ".chatrequest", function () {
                    $(".btnforchat").removeClass('d-none');
                    $(".bd-highlight").removeClass('d-none');
                    $("#profsortname").html($(this).closest("div").find(".circleimg").html())
                    UserMapChat($(this).closest("div").find("#chatprofileId").html(), $(this).closest("div").find("#profName").html(), $(this).closest("div").find(".circleimg").html());
                });
            }
        }
        });
}
function UserMapChat(ToUserId, profName,sortname) {
    $.ajax({
        url: '/Chat/SaveUserMapChat',
        type: 'POST',
        data: { "UserMapChatId": 0, "ToUserId": ToUserId, },
        success: function (response) {
            var listitem = "";
            if (response != null) {

                $("#profnamechat").html(profName);
                $("#profsortname").html(sortname);
               
                $("#spnUserMapChatId").html(response.userMapChatId);
                $("#spnToUserIdMapChatId").html(ToUserId);
                $(".img_cont").html('<div class="circleimg" style="background-color:' + displayFixedColorAlphabet(sortname.replace(/^\s+|\s+$/gm, '').substr(0, 1).toUpperCase()) + ';color:#ffff" >' + sortname.toUpperCase() +'</div>');

                UserChat(response.userMapChatId, ToUserId, sortname);
               
            }
        }
    });
}
function UserChat(userMapChatId, FromUserId, sortname) {
    $("#loading").hide();
    $.ajax({
        url: '/Chat/GetUserMapChat',
        type: 'POST',
        data: { "UserMapChatId": userMapChatId, "FromUserId": FromUserId },
        success: function (response) {
            var listitem = "";
            if (response != null) {
                GetAllUsers();
                if (response.length > 0) {
                    for (var i = 0; i < response.length; i++) {
                        if (response[i].type == 1) {
                            listitem += '<div class="d-flex justify-content-start mb-4">';
                            listitem += '<div class="img_cont_msg">';
                            /*listitem += '<img src="/assets/images/icons/profilechat.png" class="rounded-circle user_img_msg">';*/
                            listitem += '<div class="circleimgchat" style="background-color:' + displayFixedColorAlphabet($(".spnOffr_Name").html().replace(/^\s+|\s+$/gm, '').substr(0, 1).toUpperCase()) + ';color:#ffff">' + $(".spnOffr_Name").html().substr(0, 2).toUpperCase() + '</div>';
                            listitem += '</div>';
                            listitem += '<div class="msg_cotainer"><span class="spnmsgchatId">' + response[i].chatId + '</span>';
                            if (response[i].isRead == false) {
                                listitem += '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-check" viewBox="0 0 16 16">';
                                listitem += '<path d="M10.97 4.97a.75.75 0 0 1 1.07 1.05l-3.99 4.99a.75.75 0 0 1-1.08.02L4.324 8.384a.75.75 0 1 1 1.06-1.06l2.094 2.093 3.473-4.425z"/>';
                                listitem += '</svg>';
                            }
                            else {
                                listitem += '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" width="16" height="16" fill="currentColor"> <path d="M342.6 86.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L160 178.7l-57.4-57.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l80 80c12.5 12.5 32.8 12.5 45.3 0l160-160zm96 128c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L160 402.7 54.6 297.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l128 128c12.5 12.5 32.8 12.5 45.3 0l256-256z"/></svg>';
                            }
                            listitem += '' + response[i].msg + '';
                            listitem += '<span class="msg_time">' + DateFormateddMMyyyyhhmmss(response[i].createdOn) + '</span>';
                            listitem += '</div>'; //<span class="msg_del"><i class="fas fa-trash"></i></span>
                            listitem += '</div>';
                        } else if (response[i].type == 2) {
                            listitem += '<div class="d-flex justify-content-end mb-4">';
                            listitem += '<div class="msg_cotainer_send">';
                            if (response[i].isRead == false) {
                                listitem += '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-check" viewBox="0 0 16 16">';
                                listitem += '<path d="M10.97 4.97a.75.75 0 0 1 1.07 1.05l-3.99 4.99a.75.75 0 0 1-1.08.02L4.324 8.384a.75.75 0 1 1 1.06-1.06l2.094 2.093 3.473-4.425z"/>';
                                listitem += '</svg>';
                            }
                            else {
                                listitem += '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" width="16" height="16" fill="currentColor"> <path d="M342.6 86.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L160 178.7l-57.4-57.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l80 80c12.5 12.5 32.8 12.5 45.3 0l160-160zm96 128c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L160 402.7 54.6 297.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l128 128c12.5 12.5 32.8 12.5 45.3 0l256-256z"/></svg>';
                            }
                            listitem += '' + response[i].msg + '';
                            listitem += ' <span class="msg_time_send">' + DateFormateddMMyyyyhhmmss(response[i].createdOn) + '</span>';
                            listitem += ' </div>';
                            listitem += ' <div class="img_cont_msg">';
                            listitem += '<div class="circleimgchat" style="background-color:' + displayFixedColorAlphabet(sortname.replace(/^\s+|\s+$/gm, '').substr(0, 1).toUpperCase()) + ';color:#ffff">' + sortname.toUpperCase() + '</div>';
                           /* listitem += '	<img src="/assets/images/icons/profilechat.png" class="rounded-circle user_img_msg">';*/
                            listitem += '</div>';
                            listitem += '</div>';
                        }
                    }
                    $(".msg_card_body").html(listitem);
                    $(".msg_card_body").animate({ scrollTop: $('.msg_card_body').prop("scrollHeight") }, 1000);

                    $("body").on("click", ".msg_del", function () {
                        // alert($(this).closest("div").find(".spnmsgchatId").html())
                        alert(1)
                    });
                }
                else {
                    $(".msg_card_body").html(listitem);
                }

                
            }
            
        }
    });
}
function DateFormateddMMyyyyhhmmss(date) {

    var todaysDate = new Date();
    var datef1 = new Date(date);
    //if (datef1.setHours(0, 0, 0, 0) == todaysDate.setHours(0, 0, 0, 0)) {
    //    // Date equals today's date

    //    return 'Today';
    //}
    //else {
        var datef2 = new Date(date);
        var months = "" + `${(datef2.getMonth() + 1)}`;
        var days = "" + `${(datef2.getDate())}`;
        var pad = "00"
        var monthsans = pad.substring(0, pad.length - months.length) + months
        var dayans = pad.substring(0, pad.length - days.length) + days
        var year = `${datef2.getFullYear()}`;
        var hh = `${datef2.getHours()}`;
        var mm = `${datef2.getMinutes()}`;
        var ss = `${datef2.getSeconds()}`;
        if (year > 1902) {

            var datemmddyyyy = dayans + `/` + monthsans + `/` + year + ` ` + hh + `:` + mm + `:` + ss
            return datemmddyyyy;
        }
        else {
            return '';
        }
   // }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}
function DateCalculateago(fmDate) {
    ////////ago///////////
    var ago = "";
    var start_actual_time = fmDate;
    var end_actual_time = new Date();

    start_actual_time = new Date(start_actual_time);
    end_actual_time = new Date(end_actual_time);

    var diff = end_actual_time - start_actual_time;

    var diffSeconds = diff / 1000;
    var HH = Math.floor(diffSeconds / 3600);
    var MM = Math.floor(diffSeconds % 3600) / 60;

    var formatted = ((HH < 10) ? ("0" + HH) : HH) + ":" + ((MM < 10) ? ("0" + MM) : MM)

    var futureDate = new Date();
    var todayDate = new Date(fmDate);
    var milliseconds = futureDate.getTime() - todayDate.getTime();
    var hours = Math.floor(milliseconds / (60 * 60 * 1000));
    var formatted1 = formatted.substring(0, 2);
    if (parseInt(formatted1) == 00) {
        ago = formatted.substring(0, 5) + ' Minutes ago</h6>';;
    }
    else if (hours <= 24) {
        ago = hours + ' Hours ago</h6>';
    }
    else if (hours <= 730) {
        ago = Math.floor(hours / 24) + ' Days ago</h6>';;
    }
    else if (hours <= 8766) {
        ago = Math.floor(Math.floor(hours / 24) / 30) + ' Months ago</h6>';;
    }
    else {
        ago = Math.floor(Math.floor(Math.floor(hours / 24) / 30) / 12) + ' Years ago</h6>';;
    }
    return ago;
}
const colors = [
    '#FF0000', '#FF7F00', '#FFFF00', '#7FFF00', '#00FF00', '#00FF7F', '#00FFFF', '#007FFF',
    '#0000FF', '#7F00FF', '#FF00FF', '#FF007F', '#FF1493', '#FF4500', '#2E8B57', '#32CD32',
    '#4682B4', '#8A2BE2', '#D2691E', '#DC143C', '#FF8C00', '#B22222', '#ADFF2F', '#4B0082',
    '#FFD700', '#20B2AA'
];

// Function to display a to z with fixed colors
function displayFixedColorAlphabet(latter) {
   
    const alphabet = 'abcdefghijklmnopqrstuvwxyz';
    
    for (let i = 0; i < alphabet.length; i++) {
        if (latter.toUpperCase() == alphabet[i].toUpperCase()) {

            return colors[i]
        }
       
        
    }
}

