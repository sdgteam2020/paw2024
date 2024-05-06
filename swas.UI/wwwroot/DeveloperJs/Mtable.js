function mMsater(sectid = '', ddl, TableId, ParentId) {


    var userdata =
    {
        "id": TableId,
        "ParentId": ParentId,

    };
    $.ajax({
        url: '/Projects/GetAllMasterTableforddl',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == 0) {
                    Swal.fire({
                        text: "Error"
                    });
                }

                else {

                    var listItemddl = "";
                   
                        listItemddl += '<option value="">Please Select</option>';
                    

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].id + '">' + response[i].name + '</option>';
                    }
                    $("#" + ddl + "").html(listItemddl);

                    //if (TableId == 5 || TableId == 7 || TableId == 8) {

                    //    if (sectid != '') {
                    //        $("#" + ddl + " option").filter(function () {
                    //            return this.text == sectid;
                    //        }).attr('selected', true);

                    //    }
                    //}
                    //else
                    //{
                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }

                    //}


                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: "Error"
            });
        }
    });
}
