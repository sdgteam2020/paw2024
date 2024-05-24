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

                    if (i == 0) {
                        listitem += '<div class="cd-1 btn btnGetsummaytotal" style="background-color:white"><span class="d-none" id="spnstatusId">10000</span>';


                        listitem += '<div class="icon-container">';
                        listitem += '<img src="/assets/images/icons/process.png" alt="Icon" style="height:25px">';
                        listitem += '</div>';
                        listitem += '<h5 style="margin-top: 8px;" class="spntotalProject">' + data.totalProjectCount.total +' </h5>';
                        listitem += '<div class="t-1 statusprojsummry">Total Project</div> ';
                        listitem += ' <div class="mb-2">';

                        listitem += '<span class="badge bg-success spntotalProject">' + data.totalProjectCount.total +'</span></span>';

                        listitem += ' </div>';
                        listitem += ' </div>';

                    }
                    
                    listitem += '<div class="cd-1 btn btnGetsummay" style="background-color:white"><span class="d-none" id="spnstatusId">' + dtoDashboardHeaderlst[i].statusId +'</span>';
                  
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
                    listitem += '<img src="/assets/images/icons/' + dtoDashboardHeaderlst[i].icons +'" alt="Icon" style="height:25px">';
                    listitem += '</div>';
                    listitem += '<h5 style="margin-top: 8px;" >' + tot + ' </h5>';
                    listitem += '<div class="t-1 statusprojsummry">' + dtoDashboardHeaderlst[i].status +'</div> ';
                    listitem += ' <div class="mb-2">';
                         
                    listitem += '<span class="badge badge-light text-black" style="font-size:18px"><span class="badge bg-danger">' + peding + '</span> / <span class="badge bg-success">' + sent + '</span></span>';
                   
                    listitem += ' </div>';
                    listitem += ' </div>';
                    
                    listitem += '';

                    stageId=dtoDashboardHeaderlst[i].stageId;
                }

               
               
                $("#carddashboardcount").html(listitem);
                $("body").on("click", ".btnGetsummaytotal", function () {
                    
                    $('#ProjGetsummayTotal').modal('show');
                    getProjGetsummayTotal();
                });

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

function getProjGetsummayTotal() {
    var listItem = "";
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Projects/GetMyProjectsDetails',
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
                    listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";

                    $("#DetailBodysummaryTotal").html(listItem);
                   
                }

                else {

                    var count = 1;
                    // { attId: 8, psmId: 8, attPath: 'Swas_22ed1265-b2a0-4008-b7ff-b3eb5f704849.pdf', actionId: 0, timeStamp: '2024-05-02T16:17:45.6016413', … }
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spntotalprojId'>" + response[i].projId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + count + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].projName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + DateFormatedd_mm_yyyy(response[i].initiatedDate) + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].hostedon + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].apptype + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].remark + "</span></td>";
                        listItem += "<td class='align-middle'> <button type='button' class='btn btn-success  btn-History ml-3'><i class='fa-solid fa-timeline'></i></button></td>";
                      
                          listItem += "</tr>";
                        count++;
                    }

                    $("#DetailBodysummaryTotal").html(listItem);


                    $("body").on("click", ".btn-History", function () {

                        $('#ProjFwdHistory').modal('show');
                        GetProjectMovHistory($(this).closest("tr").find("#spntotalprojId").html());

                    });

                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";

                $("#DetailBodysummaryTotal").html(listItem);

            }
        },
        error: function (result) {
            Swal.fire({
                text: ""
            });
        }
    });






}
function getProjGetsummay(spnstatusId) {
    var listItem = "";
    var userdata =
    {
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
                    Swal.fire({
                        text: ""
                    });
                }
                else if (response == 0) {
                    listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";

                    $("#DetailBodysummary").html(listItem);
                    $("#lblTotal").html(0);
                }

                else {

                    var count = 1;
                    // { attId: 8, psmId: 8, attPath: 'Swas_22ed1265-b2a0-4008-b7ff-b3eb5f704849.pdf', actionId: 0, timeStamp: '2024-05-02T16:17:45.6016413', … }
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";

                        listItem += "<td class='align-middle'><span id='ProjName'>" + count + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].projName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].fromUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].toUnitName + "</span></td>";
                        
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].action + "</span></td>";

                        if (response[i].isComplete) {
                            listItem += "<td ><span class='badge badge-success' id='divName'>Done</span></td>";
                        } else {
                            listItem += "<td ><span class='badge badge-danger' id='divName'>Pending</span></td>";
                        }
                        /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";
                        count++;
                    }

                    $("#DetailBodysummary").html(listItem);
                   



                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";
               
                $("#DetailBodysummary").html(listItem);
               
            }
        },
        error: function (result) {
            Swal.fire({
                text: ""
            });
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