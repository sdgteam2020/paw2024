
$(function () {
    ProjectWiseStatus();
   
});

function ProjectWiseStatus() {
 
    var listItem = "";

    var userdata = { "Id": 0 };

    $.ajax({
        url: '/Home/GetProjectWiseStatus',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({ text: "" });
                } else if (response == 0) {

                } else {
                    var count = 1;

                    var StatusProjectlst = response.statusProjectlst;
                    var MovProjectlst = response.movProjectlst;

                    listItem += '<thead>';
                    listItem += '<tr>';
                    listItem += '<th class="d-none noExport"></th>';
                    listItem += '<th class="text-center">Ser No</th>';
                    listItem += '<th>Project Name</th>';

                    for (var i = 0; i < StatusProjectlst.length; i++) {
                        if (StatusProjectlst[i].status !== "BISAG-N" && StatusProjectlst[i].status !== "Re-Vetting") {
                            listItem += '<th>' + StatusProjectlst[i].status + '</th>';
                        }
                    }

                    listItem += '</tr>';
                    listItem += '</thead>';
                    listItem += '<tbody id="bodyProjectWiseStatus">';

                    count = 1;
                    var ProjId = 0;

                    for (var j = 0; j < MovProjectlst.length; j++) {
                        if (ProjId != MovProjectlst[j].projId) {

                            ProjId = MovProjectlst[j].projId;

                            listItem += '<tr>';
                            listItem += '<td class="clsspnprojId d-none noExport">' + MovProjectlst[j].projId + '</td>';
                            listItem += '<td class="align-middle text-center">' + count + '</td>';
                            listItem += '<td class="RefLetter-container btn-clsprojName"><div class="tooltip-container noExport">' +
                                trimByWords(MovProjectlst[j].projName, 5) +
                                '</div><div class="RefLetter projnameforlabel">' +
                                MovProjectlst[j].projName +
                                '</div></td>';

                            for (var i = 0; i < StatusProjectlst.length; i++) {
                                if (StatusProjectlst[i].status !== "BISAG-N" && StatusProjectlst[i].status !== "Re-Vetting") {

                                    var isstatus = MovProjectlst.filter(function (element) {
                                        return element.statusId == StatusProjectlst[i].statusId && element.projId == MovProjectlst[j].projId;
                                    });

                                    if (isstatus.length != 0) {
                                        listItem += '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="' +
                                            DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp) +
                                            '"><div class="pws-ok-dot">✔</div><span class="d-none">' + DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp) +'</span></td>';
                                    } else {
                                        listItem += '<td class="align-middle text-center"><img src="/assets/images/icons/Cross_red_circle.png" width="22" height="22" alt="Readed"></td>';
                                    }
                                }
                            }

                            listItem += '</tr>';
                            count++;
                        }
                    }

                    listItem += '</tbody>';

                    $("#tblProjectWiseStatus").html(listItem);
                    $(document).off("click", ".btn-clsprojName").on("click", ".btn-clsprojName", function () {
                     
                        $('#ProjHoldHistory').modal('show');
                        $(".lblProjHoldHistory").html($(this).closest("tr").find(".projnameforlabel").html());
                        $("#cardforProjHoldHistory").removeClass("d-none");
                        GetProjHold($(this).closest("tr").find(".clsspnprojId").html());
                        ProjectWiseStatusByProjid($(this).closest("tr").find(".clsspnprojId").html());
                    });

                    initializeDataTable('#tblProjectWiseStatus');
                }
            }
        },
        error: function (result) {
            Swal.fire({ text: "" });
        }
    });
}



function ProjectWiseStatusByProjid(projid) {
  
    
    var listItem = "";

    var userdata = {
        "Projid": projid
    };

    $.ajax({
        url: '/Home/GetProjectWiseStatus',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({ text: "Error fetching data!" });
                } else if (response == 0) {
                    Swal.fire({ text: "No data found for the given project ID." });
                } else {
                    var StatusProjectlst = response.statusProjectlst;
                    var MovProjectlst = response.movProjectlst;

                    listItem += '<thead>';
                    listItem += '<tr>';
                    listItem += '<th class="d-none noExport"></th>';

                    for (var i = 0; i < StatusProjectlst.length; i++) {
                        if (StatusProjectlst[i].status !== "BISAG-N" && StatusProjectlst[i].status !== "Re-Vetting") {
                            listItem += '<th class="text-white">' + StatusProjectlst[i].status + '</th>';
                        }
                    }

                    listItem += '</tr>';
                    listItem += '</thead>';
                    listItem += '<tbody id="bodyProjectWiseStatusByprojid">';

                    var count = 1;
                    var ProjId = 0;

                    for (var j = 0; j < MovProjectlst.length; j++) {

                        if (ProjId != MovProjectlst[j].projId) {
                            ProjId = MovProjectlst[j].projId;
                            listItem += '<tr>';
                            listItem += '<td class="clsspnprojId d-none noExport">' + MovProjectlst[j].projId + '</td>';

                            for (var i = 0; i < StatusProjectlst.length; i++) {
                                if (StatusProjectlst[i].status !== "BISAG-N" && StatusProjectlst[i].status !== "Re-Vetting") {

                                    var isstatus = MovProjectlst.filter(function (element) {
                                        return element.statusId == StatusProjectlst[i].statusId && element.projId == MovProjectlst[j].projId;
                                    });

                                    if (isstatus.length != 0) {
                                        listItem += '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="'
                                            + DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp)
                                            + '"><div class="pws-ok-dot">✔</div></td>';
                                    } else {
                                        listItem += '<td class="align-middle text-center"><img src="/assets/images/icons/Cross_red_circle.png" width="22" height="22" alt="Readed"></td>';
                                    }
                                }
                            }

                            listItem += '</tr>';
                            count++;
                        }
                    }

                    listItem += '</tbody>';

                    $(".tblProjectWiseStatusByprojid").html(listItem);
                }
            }
        },
        error: function (error) {
            console.log('Error:', error);
            Swal.fire({ text: 'An error occurred while fetching data.' });
        }
    });
}





                            

    const pdfFileInput = document.getElementById('pdfFileInput');
if (pdfFileInput != null) {
    pdfFileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];

        if (file) {


            const maxSizeInBytes = 10 * 1024 * 1024;
            if (file.size > maxSizeInBytes) {
                $('#uploadButton').hide();
                pdfFileInput.value = '';
                Swal.fire({
                    title: 'File Size Exceeded',
                    text: 'Please select a file smaller than 10MB.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                return;
            }


            const reader = new FileReader();
            reader.onloadend = function () {
                const bytes = new Uint8Array(reader.result);
                const pdfHeader = new Uint8Array([37, 80, 68, 70, 45]); // %PDF-
                const isPDF = compareArrays(bytes.slice(0, 5), pdfHeader);
                if (isPDF) {

                    console.log('PDF file is valid. Proceed with upload.');
                } else {

                    Swal.fire({
                        title: 'Invalid File ....!',
                        text: 'Invalid PDF file. Please select a valid PDF file.',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                    $('#uploadButton').hide();
                    pdfFileInput.value = '';
                }
            };


            reader.readAsArrayBuffer(file);
        }
    });


    pdfFileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];

        if (file) {
            const reader = new FileReader();
            reader.onloadend = function () {
                const bytes = new Uint8Array(reader.result);
                const pdfHeader = new Uint8Array([37, 80, 68, 70, 45]); // %PDF-
                const isPDF = compareArrays(bytes.slice(0, 5), pdfHeader);
                if (isPDF) {

                    console.log('PDF file is valid. Proceed with upload.');
                } else {
                    pdfFileInput.value = '';
                    Swal.fire({
                        title: 'Invalid File ....!',
                        text: 'Invalid PDF file. Please select a valid PDF file.',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            };


            reader.readAsArrayBuffer(file);
        }
    });

}
    

    function compareArrays(array1, array2) {
        if (array1.length !== array2.length) {
            return false;
        }
        for (let i = 0; i < array1.length; i++) {
            if (array1[i] !== array2[i]) {
                return false;
            }
        }
        return true;
    }





    

    


    function compareArrays(array1, array2) {
        const pdfFileInputs = document.getElementById('pdfFileInput');
        if (array1.length !== array2.length) {
            pdfFileInputs.value = '';
            return false;
        }
        for (let i = 0; i < array1.length; i++) {
            if (array1[i] !== array2[i]) {
                pdfFileInputs.value = '';
                return false;
            }
        }
        return true;
    }

    $(document).ready(function () {
        $('.dropdownsearch').select2();
    });





    $(document).ready(function () {

        function checkConditions() {
            var remarksLength = $('#AttHisAdd_Reamarks').val().length;
            var pdfFileInput = $('#pdfFileInput')[0].files.length;

            if (remarksLength > 1 && pdfFileInput > 0) {
                $('#uploadButton').prop('disabled', false);
            } else {
                $('#uploadButton').prop('disabled', true);
            }
        }

        $('#upload').click(function () {
            var documentDescription = $('#pdfFileInput').val();
            if (documentDescription.trim() === "") {
                Swal.fire({
                    title: 'Missing Upload File  ....!',
                    text: 'Please upload a file first.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });

                return false; // Prevent form submission
            }
        });

        $('#AttHisAdd_Reamarks, #pdfFileInput').on('input change', function () {
            checkConditions();
        });

        $('#uploadButton').prop('disabled', true);
    });

document.addEventListener("DOMContentLoaded", function () {
    const btn = document.getElementById("btnClose");
    if (btn) {
        btn.addEventListener("click", function () {
            history.back();
        });
    }
});
