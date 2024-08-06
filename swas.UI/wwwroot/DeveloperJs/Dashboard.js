$(document).ready(function () {
    GetAllDashbaordCount();
    var myChart1;
});




function GetAllDashbaordCount() {
    $.ajax({
        type: "POST",
        url: '/Home/GetDashboardCount',
        data: {
            "Id": 0,
           
        },
        success: function (data) {

            
            var dtoDashboardHeaderlst = data.dtoDashboardHeaderlst;

            var listitem = '';
            var stageId = 0;
            var tot = 0;
            var peding = 0;
            var sent = 0;
            if (data != null) {
                for (var i = 0; i < dtoDashboardHeaderlst.length; i++) {
                    if (stageId != dtoDashboardHeaderlst[i].stageId) {
                        if (stageId != 0) {
                            listitem += '</div>';
                        }
                        listitem += '<div class="newprojectheading text-center"> ' + dtoDashboardHeaderlst[i].stages + ' </div>';
                        listitem += '<div class="r-1" style="margin-top: 13px;">';
                    }
                    
                    
                    listitem += '<div class="cd-1 btn " style="background-color:white"><span class="d-none" id="spnstatusId">' + dtoDashboardHeaderlst[i].statusId +'</span>';
                  
                    tot = 0;
                    peding = 0;
                    sent = 0;
                    var icons = "";
                    var DTODashboardCount = data.dtoDashboardCountlst.filter(function (element) { return element.stagesId == dtoDashboardHeaderlst[i].stageId && element.statusId == dtoDashboardHeaderlst[i].statusId; });
                    for (var j = 0; j < DTODashboardCount.length; j++) {


                        tot += DTODashboardCount[j].tot;

                        if (DTODashboardCount[j].isComplete == false) {
                            peding += DTODashboardCount[j].tot;
                        }
                        if (DTODashboardCount[j].isComplete == true) {
                            sent += DTODashboardCount[j].tot;
                        }
                      
                    }
                   
                    listitem += '<div class="icon-container">';
                   /* listitem += '<img src="/assets/images/icons/' + dtoDashboardHeaderlst[i].icons +'" alt="Icon" style="height:25px">';*/
                    listitem += '<img src="/assets/images/icons/prog.png" alt="Icon" style="height:25px">';
                    listitem += '</div>';
                    listitem += '<h5 style="margin-top: 8px;" >' + tot + ' </h5>';
                    listitem += '<div class="t-1 statusprojsummry">' + dtoDashboardHeaderlst[i].status +'</div> ';
                    listitem += ' <div class="btnGetsummay">';
                  
                       

                     //   listitem += '<button type="button" class="btn btn-primary"> ' + DTODashboardActionlst[j].action +' <span class="badge badge-light">9</span>';
                       
                    listitem += '<span class="badge badge-light text-black" style="font-size:18px"><span class="badge bg-danger">' + peding + '</span> / <span class="badge bg-success">' + sent + '</span></span>';
                      
                        

                        //listitem += '<span class="badge badge-primary mr-2"></span>';

                  
                  
                   
                    listitem += ' </div>';
                    listitem += '';
                    listitem += ' <div class="mb-2">';
                    listitem += '<span class="badge badge-info mr-2" style="font-size:18px">85</span>';
                    listitem += ' </div>';
                    listitem += ' </div>';
                    
                    listitem += '';

                    stageId=dtoDashboardHeaderlst[i].stageId;
                }

               
               
                $("#carddashboardcount").html(listitem);
                $("body").on("click", ".btnGetsummay", function () {

                    var spnstatusId = $(this).closest("div").find("#spnstatusId").html();
                    $('#ProjGetsummay').modal('show');
                    $('#ProjectSummaryTittle').html($(this).closest("div").find(".statusprojsummry").html());
                    getProjGetsummay(spnstatusId);


                })
            }

        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}

$(document).ready(function () {
    // Fetching data for the first chart
    $.ajax({
        url: '/Home/indexToBarChartS',
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data.error) {
                console.error('Error fetching data:', data.error);
                return;
            }

            var monthNames = [...new Set(data.map(item => item.MonthNameYr))];
            var unitNames = [...new Set(data.map(item => item.unitname))];

            var datasets = unitNames.map(unitName => {
                var totalInData = [];
                var totalOutData = [];

                monthNames.forEach(month => {
                    var monthData = data.find(item => item.MonthNameYr === month && item.unitname === unitName);
                    if (monthData) {
                        totalInData.push(monthData.TotalIn);
                        totalOutData.push(monthData.TotalOut);
                    } else {
                        totalInData.push(0);
                        totalOutData.push(0);
                    }
                });

                var totalInColor = getRandomColor();
                var totalOutColor = getRandomColor();

                return [{
                    label: unitName + ' Proj In',
                    data: totalInData,
                    backgroundColor: totalInColor,
                    stack: unitName,
                }, {
                    label: unitName + ' Proj Out',
                    data: totalOutData,
                    backgroundColor: totalOutColor,
                    stack: unitName,
                }];

            }).flat();

            var ctx = document.getElementById('myChart').getContext('2d');
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: monthNames,
                    datasets: datasets
                },
                options: {
                    scales: {
                        x: {
                            stacked: true,
                            title: {
                                display: true,
                                text: 'Month Name'
                            }
                        },
                        y: {
                            stacked: true,
                            title: {
                                display: true,
                                text: 'Total In/Total Out'
                            }
                        }
                    }
                }
            });

            // Set the title for the first chart
            $('#chartTitle').text('Bar Chart Title');
        }
    });

    // Fetching data for the second chart
    $.ajax({
        url: '/Home/indexToPieChart',
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data.error) {
                console.error('Error fetching data:', data.error);
                return;
            }

            updatePieChart(data);

            // Set the title for the second chart
            $('#chart1Title').text('Pie Chart Title');
        },
        error: function (error) {
            console.error('Error fetching data:', error);
        }
    });
});


//function getProjGetsummay(spnstatusId) {
//    var listItem = "";
//    var userdata =
//    {
//        "StatusId": spnstatusId,

//    };
//    $.ajax({
//        url: '/Home/GetDashboardStatusDetails',
//        contentType: 'application/x-www-form-urlencoded',
//        data: userdata,
//        type: 'POST',

//        success: function (response) {
//            if (response != "null" && response != null) {

//                if (response == -1) {
//                    Swal.fire({
//                        text: ""
//                    });
//                }
//                else if (response == 0) {
//                    listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";

//                    $("#DetailBodysummary").html(listItem);
//                    $("#lblTotal").html(0);
//                }

//                else {

//                    var count = 1;
//                    for (var i = 0; i < response.length; i++) {

//                        listItem += "<tr>";

//                        listItem += "<td class='align-middle'><span id='ProjName'>" + count + "</span></td>";
//                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].projName + "</span></td>";
//                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
//                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].fromUnitName + "</span></td>";
//                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].toUnitName + "</span></td>";

//                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].action + "</span></td>";

//                        if (response[i].isComplete) {
//                            listItem += "<td ><span class='badge badge-success' id='divName'>Accepted</span></td>";
//                        } else {
//                            if (response[i].stkStatusId == 2) {
//                                listItem += "<td ><span class='badge badge-warning' id='divName'>Obsn</span></td>";
//                            }
//                            else if (response[i].stkStatusId == 3) {
//                                listItem += "<td ><span class='badge badge-danger' id='divName'>Rejected</span></td>";

//                            }
//                            else {
//                                listItem += "<td ><span class='badge badge-danger' id='divName'>Pending</span></td>";
//                            }
//                        }

//                        listItem += "</tr>";
//                        count++;
//                    }

//                    $("#DetailBodysummary").html(listItem);




//                }
//            }
//            else {
//                listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";

//                $("#DetailBodysummary").html(listItem);

//            }
//        },
//        error: function (result) {
//            Swal.fire({
//                text: ""
//            });
//        }
//    });

//}

function getProjGetsummay(spnstatusId) {
    var listItem = "";
    var userdata = {
        "StatusId": spnstatusId,
    };

    $.ajax({
        url: '/Home/GetDashboardStatusDetails',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == -1) {
                    Swal.fire({ text: "" });
                } else if (response == 0) {
                    listItem += "<tr><td class='text-center' colspan='6'>No Record Found</td></tr>";
                    $("#DetailBodysummary").html(listItem);
                    $("#lblTotal").html(0);
                } else {
                    var count = 1;
                    for (var i = 0; i < response.length; i++) {
                        listItem += "<tr>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + count + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].projName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].fromUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].toUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].action + "</span></td>";

                        if (response[i].isComplete) {
                            listItem += "<td ><span class='badge badge-success' id='divName'>Accepted</span></td>";
                        } else {
                            if (response[i].stkStatusId == 2) {
                                listItem += "<td ><span class='badge badge-warning' id='divName'>Obsn</span></td>";
                            } else if (response[i].stkStatusId == 3) {
                                listItem += "<td ><span class='badge badge-danger' id='divName'>Rejected</span></td>";
                            } else {
                                listItem += "<td ><span class='badge badge-danger' id='divName'>Pending</span></td>";
                            }
                        }

                        listItem += "</tr>";
                        count++;
                    }

                    $("#DetailBodysummary").html(listItem);

                    var table1 = $('#dashboard').DataTable({
                        destroy: true,
                        lengthChange: true,
                        dom: 'lBfrtip',
                        pageLength: -1, // Show all entries by default
                        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                        buttons: [
                            'copy',
                            'excel',
                            'csv'
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

                    function PdfDiv() {
                        var popupWin = window.open('', '_blank', 'top=100,width=900,height=500,location=no');
                        popupWin.document.open();

                        var tableStyles = `
            <style type="text/css">
                table {
                    width: 100%;
                    border-collapse: collapse;
                    margin-bottom: 20px;
                }
                .table > thead {
                    vertical-align: bottom;
                    background-color: red;
                }
                th, td {
                    padding: 8px;
                    border: 1px solid #ddd;
                    text-align: center;
                }
                th {
                    background-color: #f2f2f2;
                    color: black;
                }
            </style>
        `;

                        var table = $('#dashboard').DataTable();
                        var filteredData = table.rows({ search: 'applied' }).data().toArray();

                        var tableHTML = '<table>';

                        tableHTML += '<thead>';
                        tableHTML += '<tr>';
                        table.columns().header().each(function (header) {
                            tableHTML += '<th>' + header.innerHTML + '</th>';
                        });
                        tableHTML += '</tr>';
                        tableHTML += '</thead>';

                        tableHTML += '<tbody>';
                        for (var i = 0; i < filteredData.length; i++) {
                            tableHTML += '<tr>';
                            for (var j = 0; j < filteredData[i].length; j++) {
                                tableHTML += '<td>' + filteredData[i][j] + '</td>';
                            }
                            tableHTML += '</tr>';
                        }
                        tableHTML += '</tbody>';

                        tableHTML += '</table>';

                        var watermarkText = '@(TempData["ipadd"])';

                        popupWin.document.write('<html><head>'
                            + tableStyles + '</head><body onload="window.print()">'
                            + tableHTML + '<div style="transform: rotate(-45deg);z-index:10000;opacity: 0.3;color: BLACK; position:fixed;top: auto; left: 6%; top: 39%;color: #8e9191;font-size: 80px; font-weight: 500px;display: grid;justify-content: center;align-content: center;">'
                            + watermarkText + '</div></body></html>');

                        popupWin.document.close();
                    }
                }
            } else {
                listItem += "<tr><td class='text-center' colspan='6'>No Record Found</td></tr>";
                $("#DetailBodysummary").html(listItem);
            }
        },
        error: function (result) {
            Swal.fire({ text: "" });
        }
    });
}

function updatePieChart(data) {
    var titles = data.map(item => item.Status);
    var chartData = data.map(item => item.TotalProj);

    var canvas = document.getElementById('myChart1');
    if (!canvas) {
        console.error("Canvas element 'myChart1' not found.");
        return;
    }
    var backgroundColors = generateRandomColors(titles.length);

    var ctx = canvas.getContext('2d');

    var myChart1 = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: titles,
            datasets: [{
                data: chartData,
                backgroundColor: backgroundColors,
                borderColor: backgroundColors, // Border color same as background color for consistency
                borderWidth: 1,
            }],
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                },
            },
        },
    });
}

function getRandomColor() {
    const minBrightness = 130;
    let color;
    do {
        color = '#' + Math.floor(Math.random() * 16777215).toString(16);
        const rgb = parseInt(color.slice(1), 16);
        const r = (rgb >> 16) & 0xff;
        const g = (rgb >> 8) & 0xff;
        const b = (rgb >> 0) & 0xff;
        const brightness = (r + g + b) / 3;
        if (brightness < minBrightness) {
            color = null;
        }
    } while (color === null);
    return color;
}

function generateRandomColors(count) {
    const colors = [];
    for (let i = 0; i < count; i++) {
        let color;
        do {
            color = getRandomColor();
        } while (colors.includes(color));
        colors.push(color);
    }
    return colors;
}





document.addEventListener('DOMContentLoaded', function () {
    const modal = document.querySelector('.modal-content');
    const header = modal.querySelector('.modal-header');

    let isDragging = false;
    let startX, startY, initialX, initialY;

    header.addEventListener('mousedown', (e) => {
        isDragging = true;
        startX = e.clientX;
        startY = e.clientY;
        initialX = modal.offsetLeft;
        initialY = modal.offsetTop;
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });

    function onMouseMove(e) {
        if (isDragging) {
            const dx = e.clientX - startX;
            const dy = e.clientY - startY;
            modal.style.left = `${initialX + dx}px`;
            modal.style.top = `${initialY + dy}px`;
        }
    }

    function onMouseUp() {
        isDragging = false;
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
    }
});
