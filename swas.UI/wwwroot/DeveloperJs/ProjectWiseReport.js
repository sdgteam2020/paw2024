$(document).ready(function () {

    ProjectWiseStatus()
});
function ProjectWiseStatus() {
    var listItem = "";

    //table.destroy();
    /* alert(spnstatusActionsMappingId)*/
    var userdata = {
        "Id": 0
    };

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
                    listItem += '<th class="d-none"></th>';
                    listItem += '<th class="text-center">Ser No</th>';
                    listItem += '<th>Project Name</th>';
                    for (var i = 0; i < StatusProjectlst.length; i++) {
                        listItem += '<th>' + StatusProjectlst[i].status + '</th>';

                    }
                    listItem += '</tr>';
                    listItem += '</thead>';
                    listItem += '<tbody id="bodyProjectWiseStatus">';
                    var count = 1;
                    var ProjId = 0;
                    for (var j = 0; j < MovProjectlst.length; j++) {
                        if (ProjId != MovProjectlst[j].projId) {
                            ProjId = MovProjectlst[j].projId;
                            if (j != 0) {

                            }
                            listItem += '<tr>';
                            listItem += '<td class="align-middle text-center">' + count + '</td>';
                            listItem += '<td class="clsspnprojId d-none">' + MovProjectlst[j].projId + '</td>';
                            //listItem += '<td class="clsspnprojId d-none">' + MovProjectlst[j].projId + '</td>';
                            //listItem += '<td class="align-middle text-center">' + count + '</td>';
                            listItem += '<td class="btn-clsprojName">' + MovProjectlst[j].projName + '</td>';

                            for (var i = 0; i < StatusProjectlst.length; i++) {
                                var isstatus = MovProjectlst.filter(function (element) { return element.statusId == StatusProjectlst[i].statusId && element.projId == MovProjectlst[j].projId; });

                                //if (isstatus.length != 0) {
                                //    listItem += '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="' + DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp) + '"><i class="fa fa-check-circle" style="color: #28a745;font-size: 20px;" aria-hidden="true"></i></td>';

                                //}
                                //else {
                                //    listItem += '<td class="align-middle text-center"><i class="fa fa-minus-circle" style="color: #ffc107;font-size: 20px;" aria-hidden="true"></i></td>';
                                //}
                                //if (isstatus.length != 0) {

                                //    listItem += '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="'
                                //        + DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp)
                                //        + '"><div style="width: 25px; height: 25px; border-radius: 50%; background-color: #28a745; color: #fff; display: inline-flex; align-items: center; justify-content: center; font-weight: bold;">A</div></td>';
                                //}
                                //else {
                                //    // Pending (P) - yellow circle with P
                                //    listItem += '<td class="align-middle text-center"><div style="width: 25px; height: 25px; border-radius: 50%; background-color: #ffc107; color: #000; display: inline-flex; align-items: center; justify-content: center; font-weight: bold;">P</div></td>';
                                //}
                                if (isstatus.length != 0) {
                                    // Green tick instead of "A"
                                    listItem += '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="'
                                        + DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp)
                                        + '"><div style="width: 25px; height: 25px; border-radius: 50%; background-color: #28a745; color: #fff; display: inline-flex; align-items: center; justify-content: center; font-weight: bold; font-size: 18px;">✔</div></td>';
                                } else {
                                    listItem += '<td class="align-middle text-center"><img src="/assets/images/icons/Cross_red_circle.png" width="22" height="22" alt="Readed"></td>';
                                }
                            }
                            listItem += '</tr>';
                            count++;
                        }
                        //if (MovProjectlst[j].projName == "AmitSingha") {
                        //    alert(1)
                        //}


                    }
                    listItem += '</tbody>';

                    $("#tblProjectWiseStatus").html(listItem);

                    $("body").unbind().on("click", ".btn-clsprojName", function () {
                       
                        $('#ProjHoldHistory').modal('show');
                        // alert($(this).closest("tr").find(".clsspnprojId").html())
                        $(".lblProjHoldHistory").html($(this).html())
                        $("#cardforProjHoldHistory").removeClass("d-none");
                        GetProjHold($(this).closest("tr").find(".clsspnprojId").html())
                    });
                    
                    var table = $('#tblProjectWiseStatus').DataTable({
                        lengthChange: true,
                        retrieve: true,
                        bDestroy: true,

                        searching: true,
                        stateSave: true,
                        order: [[0, 'asc']],
                        /*"order": [[0, "asc"]],*/
                        /*order: [[1, 'desc']],*/
                        "ordering": true,
                        "paging": true,
                        dom: 'lBfrtip',
                        buttons: [
                            'copy',
                            'excel',
                            'csv',
                            //{
                            //    text: 'PDF',
                            //    extend: 'pdfHtml5',
                            //    action: function (e, dt, node, config) {
                            //        PdfDiv();
                            //    }
                            //},
                        ],
                        searchBuilder: {
                            conditions: {
                                num: {
                                    'MultipleOf': {
                                        conditionName: 'Multiple Of',
                                        init: function (that, fn, preDefined = null) {
                                            var el = $('<input/>').on('input', function () { fn(that, this) });

                                            if (preDefined !== null) {
                                                $(el).val(preDefined[0]);
                                            }

                                            return el;
                                        },
                                        inputValue: function (el) {
                                            return $(el[0]).val();
                                        },
                                        isInputValid: function (el, that) {
                                            return $(el[0]).val().length !== 0;
                                        },
                                        search: function (value, comparison) {
                                            return value % comparison === 0;
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
            }
            else {


            }
        },
        error: function (result) {
            Swal.fire({ text: "" });
        }
    });
}
function GetProjHold(ProjId) {
    var listItem = "";
    var listItemc = "";

    //table.destroy();
    /* alert(spnstatusActionsMappingId)*/
    var userdata = {
        "ProjId": ProjId
    };
    var currentdate = new Date();
    $.ajax({
        url: '/Home/GetProjHoldStatus',
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
                    var countc = 1;

                    $('#tblprojhold').dataTable().fnClearTable();
                    $('#tblprojhold').dataTable().fnDestroy();
                    const labels = []; // label array
                    const totals = []; // total array
                    const totalsForlabel = []; // total array
                    for (var j = response.length-1; j >= 0; j--) {
                        if (response[j].isComment == false) {

                            labels.push(response[j].status + '(' + response[j].fromunit + ')');
                            totals.push(DateCalculateagoForChart(response[j].timeStampfrom, response[j].timeStampTo))
                            totalsForlabel.push(DateCalculateago(response[j].timeStampfrom, response[j].timeStampTo).replace("</h6>",""));
                        }
                    }
                    for (var j = 0; j < response.length; j++) {
                        if (response[j].isComment == false) {
                           // labels.push(response[j].status + '(' + response[j].fromunit+')');

                            if (response[j].isComplete == false )
                            {
                                if (response[j].undoRemarks != null)
                                    listItem += '<tr class="table-success">';
                                    else
                                listItem += '<tr class="table-danger">';
                            }
                            else
                                listItem += '<tr class="table-success">';

                            listItem += '<td class="align-middle text-center">' + count + '</td>';
                            listItem += '<td class="align-middle">' + response[j].fromunit + '</td>';
                            if (response[j].tounit != null)
                                listItem += '<td class="align-middle">' + response[j].tounit + '</td>';
                            else
                                listItem += '<td class="align-middle">--</td>';
                            if (response[j].status != null) {
                                if (response[j].isComment == false)
                                    listItem += '<td class="align-middle">' + response[j].status + '</td>';
                                else
                                    listItem += '<td class="align-middle">' + response[j].tounit + ' For Comments</td>';
                            }

                            else
                                listItem += '<td class="align-middle">--</td>';
                            if (response[j].action != null)
                                listItem += '<td class="align-middle">' + response[j].action + '</td>';
                            else
                                listItem += '<td class="align-middle">--</td>';

                            listItem += '<td class="align-middle">' + DateFormateddMMyyyyhhmmss(response[j].timeStampfrom) + '</td>';
                            if (response[j].isComment == false)
                                listItem += '<td class="align-middle">' + DateFormateddMMyyyyhhmmss(response[j].timeStampTo) + '</td>';
                            else if (response[j].isComment == true) {

                               
                                listItem += '<td class="align-middle">' + DateFormateddMMyyyyhhmmss(response[j].timeStampTo) + '</td>';

                            }
                           // totals.push(DateCalculateagoForChart(response[j].timeStampfrom, response[j].timeStampTo))
                            listItem += '<td class="align-middle">' + DateCalculateago(response[j].timeStampfrom, response[j].timeStampTo) + '</td>';


                            listItem += '</tr>';
                            count++;
                        }
                        else {
                           
                                if (response[j].isComplete == false)
                                    listItemc += '<tr class="table-danger">';
                                else
                                    listItemc += '<tr class="table-success">';

                            listItemc += '<td class="align-middle text-center">' + countc + '</td>';
                                listItemc += '<td class="align-middle">' + response[j].fromunit + '</td>';
                                if (response[j].tounit != null)
                                    listItemc += '<td class="align-middle">' + response[j].tounit + '</td>';
                                else
                                    listItemc += '<td class="align-middle">--</td>';
                                if (response[j].status != null) {
                                    if (response[j].isComment == false)
                                        listItemc += '<td class="align-middle">' + response[j].status + '</td>';
                                    else
                                        listItemc += '<td class="align-middle">' + response[j].tounit + ' For Comments</td>';
                                }

                                else
                                    listItemc += '<td class="align-middle">--</td>';
                                if (response[j].action != null)
                                    listItemc += '<td class="align-middle">' + response[j].action + '</td>';
                                else
                                    listItemc += '<td class="align-middle">--</td>';

                                listItemc += '<td class="align-middle">' + DateFormateddMMyyyyhhmmss(response[j].timeStampfrom) + '</td>';
                                if (response[j].isComment == false)
                                    listItemc += '<td class="align-middle">' + DateFormateddMMyyyyhhmmss(response[j].timeStampTo) + '</td>';
                                else if (response[j].isComment == true) {

                                    listItemc += '<td class="align-middle">' + DateFormateddMMyyyyhhmmss(response[j].timeStampTo) + '</td>';

                                }
                            if (response[j].isComplete == true) {
                               
                                listItemc += '<td class="align-middle">' + DateCalculateago(response[j].timeStampfrom, response[j].timeStampTo) + '</td>';
                            }
                            else {
                                listItemc += '<td class="align-middle">' + DateCalculateago(response[j].timeStampfrom, currentdate) + '</td>';
                                
                            }
                              
                                listItemc += '</tr>';
                                countc++;
                            
                        }
                    }
                        //if (MovProjectlst[j].projName == "AmitSingha") {
                        //    alert(1)
                        //}


                    
                   

                $("#DetailBodyholdComments").html(listItemc);
                $("#DetailBodyhold").html(listItem);

                   
                    var table11 = $('#tblprojComments').DataTable({
                        lengthChange: true,
                        retrieve: true,
                        Destroy: true,

                        searching: true,
                        stateSave: true,
                        "order": [[0, "asc"]],
                        "ordering": true,
                        "paging": true,
                        dom: 'lBfrtip',
                        buttons: [
                            'copy',
                            'excel',
                            'csv',
                            //{
                            //    text: 'PDF',
                            //    extend: 'pdfHtml5',
                            //    action: function (e, dt, node, config) {
                            //        PdfDiv();
                            //    }
                            //},
                        ],
                        searchBuilder: {
                            conditions: {
                                num: {
                                    'MultipleOf': {
                                        conditionName: 'Multiple Of',
                                        init: function (that, fn, preDefined = null) {
                                            var el = $('<input/>').on('input', function () { fn(that, this) });

                                            if (preDefined !== null) {
                                                $(el).val(preDefined[0]);
                                            }

                                            return el;
                                        },
                                        inputValue: function (el) {
                                            return $(el[0]).val();
                                        },
                                        isInputValid: function (el, that) {
                                            return $(el[0]).val().length !== 0;
                                        },
                                        search: function (value, comparison) {
                                            return value % comparison === 0;
                                        }
                                    }
                                }
                            }
                        }
                    });
                   var table = $('#tblprojhold').DataTable({
                        lengthChange: true,
                        retrieve: true,
                        bDestroy: true,

                        searching: true,
                        stateSave: true,
                    "order": [[0, "asc"]],
                    "ordering": true,
                        "paging": true,
                        dom: 'lBfrtip',
                        buttons: [
                            'copy',
                            'excel',
                            'csv',
                            //{
                            //    text: 'PDF',
                            //    extend: 'pdfHtml5',
                            //    action: function (e, dt, node, config) {
                            //        PdfDiv();
                            //    }
                            //},
                        ],
                        searchBuilder: {
                            conditions: {
                                num: {
                                    'MultipleOf': {
                                        conditionName: 'Multiple Of',
                                        init: function (that, fn, preDefined = null) {
                                            var el = $('<input/>').on('input', function () { fn(that, this) });

                                            if (preDefined !== null) {
                                                $(el).val(preDefined[0]);
                                            }

                                            return el;
                                        },
                                        inputValue: function (el) {
                                            return $(el[0]).val();
                                        },
                                        isInputValid: function (el, that) {
                                            return $(el[0]).val().length !== 0;
                                        },
                                        search: function (value, comparison) {
                                            return value % comparison === 0;
                                        }
                                    }
                                }
                            }
                        }
                });


                 
                    const colors = [
                        "#73a3f9", "#fbbb4b", "#c3cad4", "#48d0ad", "#a88bfa",
                        "#4ee17e", "#fee47d", "#f76e6e", "#8f88f9", "#3edfd0",
                        "#f9cb48", "#d7aaff", "#4fd4ff", "#faa4a4", "#a6eb48",
                        "#faa6d6", "#c6b4ff", "#40dff4", "#fa4d78", "#a285ff"
                    ];

                    // Map names and totals
                    //const labels = ProjectStatus.map(x => x.status);
                   // const totals = ProjectStatus.map(x => x.timeStampfrom);
                    // Reset and bind chart with new data
                    bindProjHoldChart(labels, totals, totalsForlabel,colors,);

                  


                }
            }
            else {


            }
        },
        error: function (result) {
            Swal.fire({ text: "" });
        }
    });
}
// Declare chart globally so you can destroy it later
let projHoldChart;

// Function to create/rebind the chart
function bindProjHoldChart(labels, totals, totalsForlabel, colors) {
    // If chart already exists, destroy it before creating new
    if (projHoldChart) {
        projHoldChart.destroy();
    }

    // Create new chart
    projHoldChart = new Chart(document.getElementById('ProjHoldHistoryChart'), {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Projects',
                backgroundColor: colors,
                data: totals
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Project Hold History (in Hours.Minute)'
                },
                legend: { display: false },
                datalabels: {
                    anchor: 'end',
                    align: 'top',
                    color: '#000',
                    font: {
                        weight: 'bold'
                    }, formatter: function (value, context) {
                        // Use value from holdLabels array by index
                        return totalsForlabel[context.dataIndex];
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}
