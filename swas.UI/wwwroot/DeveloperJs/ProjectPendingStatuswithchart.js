function GetProjHold(ProjId) {

    var listItem = "";
    var listItemc = "";

    var userdata = {
        "ProjId": ProjId
    };

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

                    const labels = [];
                    const totals = [];
                    const totalsForlabel = [];
                    const labelscmd = [];
                    const totalscmd = [];
                    const totalsForlabelcmd = [];
                    let colorscmd = [];

                    var cmtunit = [1, 3, 4, 5];

                    let responseforchart = response
                        .filter(function (elements) {
                            return elements.isComment == false;
                        })
                        .sort(function (a, b) {
                            if (a.statusId === b.statusId) {
                                return a.tounitId - b.tounitId;
                            }
                            return a.statusId - b.statusId;
                        });

                    let totalTimeSpentData = calculateTotalTime(responseforchart);

                    totalTimeSpentData.forEach(function (item) {
                        labels.push(item.tounit);
                        totals.push(item.totalTimeSpent);
                        totalsForlabel.push(convertMinutesToAgo(item.totalTimeSpent));
                    });

                    for (var j = response.length - 1; j >= 0; j--) {
                        if (cmtunit.includes(response[j].tounitId)) {
                            if (response[j].isComment == true) {

                                if (response[j].firstStkStatus == "Accepted" || response[j].approvedStatusId == 1) {

                                    colorscmd.push("#008000");
                                    totalsForlabelcmd.push("Approved\n" + DateCalculateago(response[j].timeStampfrom, response[j].firstActionDate));
                                    totalscmd.push(DateCalculateagoForChart(response[j].timeStampfrom, response[j].firstActionDate));
                                }
                                else if (response[j].firstStkStatus == "Rejected") {
                                    colorscmd.push("#f96161");
                                    totalscmd.push(DateCalculateagoForChart(response[j].timeStampfrom, response[j].firstActionDate));
                                    totalsForlabelcmd.push(response[j].firstStkStatus + "\n" + DateCalculateago(response[j].timeStampfrom, response[j].firstActionDate));
                                }
                                else if (response[j].firstStkStatus == "Obsn") {
                                    colorscmd.push("#fbbb4b");
                                    totalscmd.push(DateCalculateagoForChart(response[j].timeStampfrom, response[j].firstActionDate));
                                    totalsForlabelcmd.push(response[j].firstStkStatus + "\n" + DateCalculateago(response[j].timeStampfrom, response[j].firstActionDate));
                                }
                                else if (response[j].firstStkStatus == "Info") {
                                    colorscmd.push("#73a3f9");
                                    totalsForlabelcmd.push(response[j].firstStkStatus + "\n" + DateCalculateago(response[j].timeStampfrom, response[j].firstActionDate));
                                    totalscmd.push(DateCalculateagoForChart(response[j].timeStampfrom, response[j].firstActionDate));
                                }
                                else {
                                    colorscmd.push("#FF0000");
                                    totalsForlabelcmd.push("Pending\n" + DateCalculateago(response[j].timeStampfrom, response[j].timeStampTo));
                                    totalscmd.push(DateCalculateagoForChart(response[j].timeStampfrom, response[j].timeStampTo));
                                }

                                labelscmd.push(response[j].status + '(' + response[j].tounit + ')');

                            }
                            else {
                                $("#Recddt").html(" ");
                            }
                        }
                    }

                    for (var j = 0; j < response.length; j++) {
                        if (response[j].isComment == false) {

                            if (response[j].isComplete == false) {
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
                            else if (response[j].isComment == true)
                                listItem += '<td class="align-middle">' + DateFormateddMMyyyyhhmmss(response[j].timeStampTo) + '</td>';

                            listItem += '<td class="align-middle">' + DateCalculateago(response[j].timeStampfrom, response[j].timeStampTo) + '</td>';

                            listItem += '</tr>';
                            count++;

                        }
                        else {
                            var commentRecdt = DateFormateddMMyyyyhhmmss(response[j].timeStampfrom);
                            $("#Recddt").html(" (Project is FWD by DDGIT On " + commentRecdt + ")");

                            if (response[j].stkStauts === "Accepted") {
                                listItemc += '<tr class="table-success">';
                            }
                            else if (response[j].stkStauts === "Info") {
                                listItemc += '<tr class="table-primary">';
                            }
                            else if (response[j].stkStauts === "Obsn") {
                                listItemc += '<tr class="table-warning">';
                            }
                            else {
                                listItemc += '<tr class="table-danger">';
                            }

                            listItemc += '<td class="align-middle text-center">' + countc + '</td>';

                            if (response[j].tounit != null)
                                listItemc += '<td class="align-middle">' + response[j].tounit + '</td>';
                            else
                                listItemc += '<td class="align-middle">--</td>';

                            if (response[j].status != null) {
                                if (response[j].isComment == false)
                                    listItemc += '<td class="align-middle">' + response[j].status + '</td>';
                                else
                                    listItemc += '<td class="align-middle">' + response[j].tounit + ' For Parallel Comments</td>';
                            }
                            else
                                listItemc += '<td class="align-middle">--</td>';

                            if (response[j].stkStauts != null) {
                                listItemc += '<td class="align-middle">' + response[j].stkStauts + '</td>';
                            }
                            else {
                                listItemc += '<td class="align-middle">Pending</td>';
                            }

                            if (response[j].isComment == false)
                                listItemc += '<td class="align-middle">' + DateFormateddMMyyyyhhmmss(response[j].timeStampTo) + '</td>';
                            else if (response[j].isComment == true)
                                listItemc += '<td class="align-middle">' + DateFormateddMMyyyyhhmmss(response[j].timeStampTo) + '</td>';

                            if (response[j].isComplete == true) {
                                listItemc += '<td class="align-middle">' + DateCalculateago(response[j].timeStampfrom, response[j].timeStampTo) + '</td>';
                            }
                            else {
                                listItemc += '<td class="align-middle">' + DateCalculateago(response[j].timeStampfrom, response[j].timeStampTo) + '</td>';
                            }

                            if (response[j].approvedStatusId == 1) {
                                listItemc += '<td class="align-middle">' +
                                    "(" + DateFormateddMMyyyyhhmmss(response[j].approveddate) + ")   " +
                                    '<div class="ph-ok-dot">✔</div>' +
                                    '</td>';
                            } else if (response[j].rejectedDt != null) {
                                listItemc += '<td class="align-middle ">' + "(" + DateFormateddMMyyyyhhmmss(response[j].rejectedDt) + ")   " + '<img src="/assets/images/icons/Cross_red_circle.png" width="22" height="22" alt="Readed"></td>';
                            }
                            else {
                                listItemc += '<td class="align-middle">--</td>';
                            }

                            listItemc += '</tr>';
                            countc++;
                        }
                    }

                    $("#DetailBodyholdComments").html(listItemc);
                    $("#DetailBodyhold").html(listItem);

                    initializeDataTable('#tblprojComments');
                    initializeDataTable('#tblprojhold');

                    const colors = [
                        "#73a3f9", "#fbbb4b", "#c3cad4", "#48d0ad", "#a88bfa",
                        "#4ee17e", "#fee47d", "#f76e6e", "#8f88f9", "#3edfd0",
                        "#f9cb48", "#d7aaff", "#4fd4ff", "#faa4a4", "#a6eb48",
                        "#faa6d6", "#c6b4ff", "#40dff4", "#fa4d78", "#a285ff"
                    ];

                    bindProjHoldChart(labels, totals, totalsForlabel, colors);
                    bindProjHoldCommentsChart(labelscmd, totalscmd, totalsForlabelcmd, colorscmd);
                }
            }
        },
        error: function (result) {
            Swal.fire({ text: "" });
        }
    });
}
let projHoldChart;
function bindProjHoldChart(labels, totals, totalsForlabel, colors) {
    if (projHoldChart) {
        projHoldChart.destroy();
    }
    projHoldChart = new Chart(document.getElementById('ProjHoldHistoryChart'), {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Projects',
                backgroundColor: colors,
                data: totals,
                barThickness: 50,
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    
                },
                legend: { display: false },
                datalabels: {
                    anchor: 'end',
                    align: 'top',
                    color: '#000',
                    innerWidth: 0,
                    font: {
                        weight: 'bold'
                    }, formatter: function (value, context) {
                        return totalsForlabel[context.dataIndex];
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}
let projHoldChartcomment;
function bindProjHoldCommentsChart(labels, totals, totalsForlabel, colors) {
    if (projHoldChartcomment) {
        projHoldChartcomment.destroy();
    }
    projHoldChartcomment = new Chart(document.getElementById('ProjHoldHistoryCommentChart'), {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Projects',
                backgroundColor: colors,
                data: totals,
                barThickness: 50,
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    
                },
                legend: { display: false },
                datalabels: {
                    anchor: 'end',
                    align: 'top',
                    color: '#000',
                    innerWidth: 0,
                    font: {
                        weight: 'bold'
                    },
                    textAlign: "center",
                    formatter: function (value, context) {
                        return totalsForlabel[context.dataIndex];
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}
function calculateTotalTime(response) {
    let result = {};
    response.forEach(entry => {
        const { tounitId, tounit, timeStampfrom, timeStampTo } = entry;
        const from = new Date(timeStampfrom);
        const to = new Date(timeStampTo);
        const timeSpent = (to - from) / 1000 / 60; // Difference in minutes
        const key = `${tounitId}_${tounit}`;
        if (!result[key]) {
            result[key] = 0;
        }
        result[key] += timeSpent;
    });
    const groupedResult = Object.keys(result).map(key => {
        const [tounitId, tounit] = key.split('_');
        return {
            tounitId: parseInt(tounitId),
            tounit: tounit,

            totalTimeSpent: result[key]
        };
    });

    return groupedResult;
}
function convertMinutesToAgo(minutes) {
    if (minutes == null || isNaN(minutes) || minutes < 0) {
        minutes = 0;
    }
    const totalHours = minutes / 60;
    const remainingMinutes = minutes % 60;

    let ago = "";
    let formatted;
    if (totalHours >= 8760) {
        const days = Math.round(totalHours / 24);
        const hours = Math.round(totalHours % 24);  // Remaining hours after extracting days
        formatted = `${hours.toString().padStart(2, '0')}:${Math.round(remainingMinutes).toString().padStart(2, '0')}`;
        const daysforyear = `${days} Days`;



        const years = Math.round(totalHours / 8760);  // 8760 hours in a year
        formatted = `${Math.round(totalHours).toString().padStart(3, '0')}:${Math.round(remainingMinutes).toString().padStart(2, '0')}`;
        ago = `${years} Years (${daysforyear})`;
    }
    else if (totalHours >= 730) {
        const days = Math.round(totalHours / 24);
        const hours = Math.round(totalHours % 24);  // Remaining hours after extracting days
        formatted = `${hours.toString().padStart(2, '0')}:${Math.round(remainingMinutes).toString().padStart(2, '0')}`;
        ago = `${days} Days`;
    }
    else if (totalHours >= 24) {
        const days = Math.round(totalHours / 24);
        const hours = Math.round(totalHours % 24);  // Remaining hours after extracting days
        formatted = `${hours.toString().padStart(2, '0')}:${Math.round(remainingMinutes).toString().padStart(2, '0')}`;
        ago = `${days} Days`;
    }
    else {
        if (minutes == 0) {
            ago = "Till Now"
        }
        else {
            formatted = `${Math.floor(totalHours).toString().padStart(2, '0')}:${Math.floor(remainingMinutes).toString().padStart(2, '0')}`;
            ago = `${formatted} Min`;
        }
    }


    return ago;
}




