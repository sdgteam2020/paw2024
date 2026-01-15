
    if (TempData.ContainsKey("FailureMessage"))
    {
        <text>
            Swal.fire({
                title: 'Error',
                text: '@TempData["FailureMessage"]',
                icon: 'failure',
                confirmButtonText: 'OK'
            });
        </text>
                TempData.Remove("FailureMessage");
    }




    if (TempData.ContainsKey("SuccessMessage"))
    {
        <text>
            Swal.fire({
                title: 'Success',
                text: '@TempData["SuccessMessage"]',
                icon: 'success',
                confirmButtonText: 'OK'
            });
                    
        </text>
        TempData.Remove("SuccessMessage");
    }



    
        
         function populateCorpsDropdown(selectElement) {
        var selectedCommandId = $(selectElement).val();
       
            $.ajax({
                url: "/Ddl/ddlCorps",
                data: { Command: selectedCommandId },
                type: "GET",
                dataType: "json",
                success: function (result) {

                    if (result.length > 0) {
                        var list = "";


                        list = '<option value="0">---- Select  ----</option>';

                        for (var j = 0; j < result.length; j++) {


                            list += '<option value=' + result[j].corpsid + '>' + result[j].corpsname + '</option>';
                        }



                        $('#CorpsId').html(list);

                        $('#CorpsId').val($('#addNewForm').html());
                        $("#CorpsId").val("0");

                    }
                    else {
                        var list = "";
                        list = '<option value="0" selected="true">---- Select ----</option>';

                        $('#CorpsId').html(list).selectedCommandId;

                        $('#CorpsId').val($('#addNewForm').html());

                        $("#CorpsId").val("0");

                    }

                }
            });
        };

    


    function ValInData(input) {
        var regex = /[^a-zA-Z0-9/ ]/g;
        input.value = input.value.replace(regex, "");
    }

    $(document).ready(function () {
        $('.dropdownsearch').select2();
    });
