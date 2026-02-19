$(document).ready(function () {

    $(document)
        .ajaxStart(function () {
            $("#loading").show();
        })
        .ajaxStop(function () {
            $("#loading").hide();
        })
        .ajaxError(function () {
            $("#loading").hide();
        });
    GetAllUsers();

 

    $("#chatProfileSearch").keyup(function () {
        GetAllUsers();
    });

    $("#type_msg").keypress(function () {
        $("#type_msg").removeClass('is-invalid');
    });

    $('#type_msg').keypress(function (e) {
        if (e.which === 13 && !e.shiftKey) {
            e.preventDefault();

            if ($("textarea#type_msg").val().trim() === "") {
                $("textarea#type_msg").addClass('is-invalid');
            } else {
                SaveChat($("textarea#type_msg").val());
            }
        }
    });

    $("#send_btn").click(function () {
        if ($("textarea#type_msg").val().trim() === "") {
            $("textarea#type_msg").addClass('is-invalid');
        } else {
            SaveChat($("textarea#type_msg").val());
        }
    });
});

function SaveChat(Msg) {
    $.ajax({
        url: '/Chat/SaveChat',
        type: 'POST',
        data: {
            "UserMapChatId": $("#spnUserMapChatId").html(),
            "Msg": Msg
        },
        success: function (response) {
            if (response != null) {
                $("textarea#type_msg").val("");
                UserChat(
                    $("#spnUserMapChatId").html(),
                    $("#spnToUserIdMapChatId").html(),
                    $("#profsortname").html()
                );
                GetAllUsers();
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

            if (!response || !response.length) return;

            var listitem = "";

            for (var i = 0; i < response.length; i++) {
                const letter = response[i].offr_Name.trim().charAt(0).toUpperCase();
                const colorClass = displayFixedColorAlphabet(letter);

                if (
                    $("#chatProfileSearch").val() === "" ||
                    response[i].offr_Name.toLowerCase().includes($("#chatProfileSearch").val().toLowerCase()) ||
                    response[i].userName.toLowerCase().includes($("#chatProfileSearch").val().toLowerCase())
                ) {
                    listitem += '<li>';
                    listitem += '<div class="d-flex bd-highlight chatrequest">';
                    listitem += '<div class="img_cont">';
                  listitem +=
    '<div class="circleimg ' + colorClass + '">' +
    letter +
    
                       
                        '</div>';

                    if (parseInt(response[i].total) > 0)
                        listitem += '<span class="online_icon"></span>';

                    listitem += '</div>';
                    listitem += '<div class="user_info">';
                    listitem += '<span class="d-none" id="chatprofileId">' + response[i].id + '</span>';
                    listitem += '<span id="profName">' + response[i].rankName + ' ' + response[i].offr_Name + '</span>';
                    listitem += '<p>' + response[i].userName + '</p>';
                    listitem += '</div></div></li>';
                }
            }

            $(".contacts").html(listitem);

            $("body")
                .off("click", ".chatrequest")
                .on("click", ".chatrequest", function () {

                    $(".chatrequest").removeClass('active');
                    $(this).addClass("active");

                    $(".btnforchat").removeClass('d-none');
                    $(".bd-highlight").removeClass('d-none');

                    $("#profsortname").html($(this).find(".circleimg").html());

                    UserMapChat(
                        $(this).find("#chatprofileId").html(),
                        $(this).find("#profName").html(),
                        $(this).find(".circleimg").html()
                    );
                });
        }
    });
}

function UserMapChat(ToUserId, profName, sortname) {
    $.ajax({
        url: '/Chat/SaveUserMapChat',
        type: 'POST',
        data: { "UserMapChatId": 0, "ToUserId": ToUserId },
        success: function (response) {
            if (!response) return;

            $("#profnamechat").html(profName);
            $("#profsortname").html(sortname);
            $("#spnUserMapChatId").html(response.userMapChatId);
            $("#spnToUserIdMapChatId").html(ToUserId);
            const letter = sortname.trim().charAt(0).toUpperCase();
            const colorClass = displayFixedColorAlphabet(letter);

            $("#img_contuser").html(
                '<div class="circleimg ' + colorClass + '">' +
                sortname.toUpperCase() +
                '</div>'
            );
            UserChat(response.userMapChatId, ToUserId, sortname);
        }
    });
}

function UserChat(userMapChatId, FromUserId, sortname) {
    $.ajax({
        url: '/Chat/GetUserMapChat',
        type: 'POST',
        data: { "UserMapChatId": userMapChatId, "FromUserId": FromUserId },
        success: function (response) {

            var listitem = "";

            if (response && response.length > 0) {
                for (var i = 0; i < response.length; i++) {
                    const name = $(".spnOffr_Name").text().trim();
                    const letter = name.charAt(0).toUpperCase();
                    const colorClass = displayFixedColorAlphabet(letter);
                    if (response[i].type == 1) {
                        listitem += '<div class="d-flex justify-content-start mb-4">';
                        listitem += '<div class="img_cont_msg">';
                        listitem +=
                            '<div class="circleimgchat ' + colorClass + '">' +
                            name.substr(0, 2).toUpperCase() +
                            '</div></div>';
                        listitem += '<div class="msg_cotainer">';
                        listitem += response[i].msg;
                        listitem += '<span class="msg_time">' +
                            DateFormateddMMyyyyhhmmss(response[i].createdOn) +
                            '</span></div></div>';
                    } else {
                        listitem += '<div class="d-flex justify-content-end mb-4">';
                        listitem += '<div class="msg_cotainer_send">';
                        listitem += response[i].msg;
                        listitem += '<span class="msg_time_send">' +
                            DateFormateddMMyyyyhhmmss(response[i].createdOn) +
                            '</span></div>';
                        listitem += '<div class="img_cont_msg">';
                        listitem += '<div class="circleimgchat" style="background-color:' +
                            displayFixedColorAlphabet(sortname.trim().substr(0, 1).toUpperCase()) +
                            ';color:#fff">' + sortname.toUpperCase() +
                            '</div></div></div>';
                    }
                }
            }

            $(".msg_card_body").html(listitem);
            $(".msg_card_body").scrollTop($('.msg_card_body')[0].scrollHeight);
        }
    });
}


function displayFixedColorAlphabet(letter) {
    const alphabet = 'abcdefghijklmnopqrstuvwxyz';
    const index = alphabet.indexOf(letter.toLowerCase());
    return index >= 0 ? 'circle-color-' + index : 'circle-color-0';
}

