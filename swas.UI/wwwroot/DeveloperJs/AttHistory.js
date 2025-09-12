
let allAttachments = []; // Array to hold all attachments and remarks

function AttOnFWD() {
    $('.uploadLoader').addClass('d-none')
    debugger;
    var listItem = "";

    // Check for any previous rows and remove placeholder if needed
    if ($.trim($("#DetailBody").text()) === "No Record Found") {
        $("#DetailBody").empty(); // remove placeholder row
    }

   

    const input = document.getElementById("pdfFileInput");
    const files = input?.files || [];
    const remarksVal = $("#Remarks").val() ?? $("#Reamarks").val() ?? "";

    if (!files.length) {
        alert("Please choose at least one PDF.");
        return;
    }

    // Build FormData (though we won’t send it yet)
    const fd = new FormData();
    fd.append("remarks", remarksVal); // Add remarks to FormData

    // Add files to FormData and store them in the allAttachments array
    let attachments = [];
    for (let i = 0; i < files.length; i++) {
        fd.append("uploadfile[]", files[i]);  // Append files to FormData
        attachments.push({ file: files[i], remarks: remarksVal }); // Track the files and remarks
    }

    // Store the current attachments and remarks in allAttachments
    allAttachments.push(...attachments);

    // Build table rows for preview (displaying files and remarks in the table)
    for (let i = 0; i < files.length; i++) {
        const f = files[i];
        const tempUrl = URL.createObjectURL(f);

        listItem += "<tr>";
        listItem += "<td class='align-middle'>" +
            "<button type='button' class='att-btnDelete btn-icon btn-round btn-danger mr-1'>" +
            "<i class='fas fa-trash-alt'></i>" +
            "</button>" +
            "</td>";

        listItem += "<td class='align-middle RefLetter-container'>" +
            "<span>" + trimByWords(remarksVal, 4) + "</span>" +
            "<div class='RefLetter'>" + remarksVal + "</div>" +
            "</td>";

        listItem += "<td class='align-middle RefLetter-container'>" +
            "<span>" +
            "<a class='link-success' href='" + tempUrl + "' target='_blank'>" + trimByWords(f.name, 4) + "</a>" +
            "</span>" +
            "<div class='RefLetter'>" + f.name + "</div>" +
            "</td>";

        listItem += "<td class='align-middle'><span>" +
            new Date().toLocaleString() +
            "</span></td>";

        listItem += "</tr>";
    }

    // Clear the form fields
    $("#Reamarks").val("");
    $("#pdfFileInput").val("");

    // Bind to the table
    $("#DetailBody").append(listItem);

    // When the confirm button is clicked, send all attachments and remarks
    $("#btnFwdConfirm").off("click").on("click", function () {
        // Send the allAttachments array along with form data
        SaveFwdTo($("#spanFwdCurrentPslmId").html(), fd, allAttachments);
        
    });

}



$(document).on("click", ".att-btnDelete", function () {
    // Get the index of the row that contains the delete button
    var rowIndex = $(this).closest("tr").index();

    // Remove the attachment from the allAttachments array based on the row index
    allAttachments.splice(rowIndex, 1);

    // Remove the row from the table
    $(this).closest("tr").remove();
});

function UploadFiles() {
   

    var formData = new FormData();
    debugger;
    
        var totalFiles = document.getElementById("pdfFileInput").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("pdfFileInput").files[i];
            formData.append("uploadfile", file);
          
            formData.append("Reamarks", $("#Reamarks").val());
            formData.append("PsmId", $("#spanCurrentPslmId").html());
        }
 

    $.ajax({
        type: "POST",
        url: '/Projects/UploadMultiFile',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {

            $('#uploadLoader').hide()
        
            if (response == 1) {
                $('#uploadLoader').hide();
                AttechHistory();
                $("#Reamarks").val("");
                $("#pdfFileInput").val("");
             
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Upload success",
                    showConfirmButton: false,
                    timer: 1500
                });
            } else if (response == -2) {

                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Only Pdf File Upload!",

                });
            }
            else if (response == -5) {

                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Pdf File LessThen 10 MB !",

                });
            }

        },
        error: function (error) {
            $('#uploadLoader').hide();
            $(".error-msg").removeClass("d-none")
            $("#error-msg").html("Somthing is wrong");;

        }
    });
}

function AttechHistory() {
    debugger;
    var listItem = "";
    var userdata =
    {
        "PslmId": $("#spanCurrentPslmId").html(),

    };
    $.ajax({
        url: '/Projects/GetAtthHistoryByProjectId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({
                        text: ""
                    });
                }
                else if (response == 0) {

                    listItem += "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";
                   
                       

                            $("#DetailBody").html(listItem);
                     
                    
                    $("#lblTotal").html(0);
                }

                else {


                    // { attId: 8, psmId: 8, attPath: 'Swas_22ed1265-b2a0-4008-b7ff-b3eb5f704849.pdf', actionId: 0, timeStamp: '2024-05-02T16:17:45.6016413', … }
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";

                        // Hidden IDs
                        listItem += "<td class='d-none'>" +
                            "<span id='spnattId'>" + response[i].attId + "</span>" +
                            "<span id='spnpsmId'>" + response[i].psmId + "</span>" +
                            "</td>";

                        // Delete Button
                        listItem += "<td class='align-middle'>" +
                            "<span id='btnedit'>" +
                            "<button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'>" +
                            "<i class='fas fa-trash-alt'></i>" +
                            "</button>" +
                            "</span>" +
                            "</td>";

                        // Remarks (trimmed to 6 words)
                        var breakRemarks = response[i].reamarks || "";
                        var formatedName = trimByWords(breakRemarks, 4);

                        listItem += "<td class='align-middle RefLetter-container'>" +
                            "<span id='comdName'>" + formatedName + "</span>" +
                            "<div class='RefLetter'>" + breakRemarks + "</div>" +
                            "</td>";

                        // FileName (trimmed to 4 words, full shown inside hidden div/tooltip)
                        listItem += "<td class='align-middle RefLetter-container'>" +
                            "<span id='corpsName'>" +
                            "<a class='link-success' target='_blank' href='/uploads/" + response[i].attPath + "'>" +
                            trimByWords(response[i].actFileName, 4) +
                            "</a>" +
                            "</span>" +
                            "<div class='RefLetter'>" + response[i].actFileName + "</div>" +
                            "</td>";

                        // Timestamp
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].timeStamp + "</span></td>";

                        listItem += "</tr>";
                    }
                   
                            // If the modal is not visible, update #DetailBody
                            $("#DetailBody").html(listItem);
                       
                   
                    $("#lblTotal").html(response.length);



                    var rows;





                    $("body").on("click", ".cls-btnDelete", function () {

                        Swal.fire({
                            title: 'Are you sure?',
                            text: "You want to Delete ",
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonColor: '#072697',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Yes, Delete It!'
                        }).then((result) => {
                            if (result.value) {

                                Deleteattechment($(this).closest("tr").find("#spnattId").html());

                            }
                        });
                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";
                $("#SoftwareTypes").DataTable().destroy();
                $("#DetailBody").html(listItem);
                $("#lblTotal").html(0);
            }
        },
        error: function (result) {
            Swal.fire({
                text: ""
            });
        }
    });
}
function Deleteattechment(AttechId) {
    $.ajax({
        url: '/Projects/DeleteAttech',
        type: 'POST',
        data: { "AttechId": AttechId },
        success: function (response) {
            //console.log(response);


            if (response == 1) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Record Deleted successfully',
                    showConfirmButton: false,
                    timer: 1500
                });

                AttechHistory();

            }

        }
    });
}