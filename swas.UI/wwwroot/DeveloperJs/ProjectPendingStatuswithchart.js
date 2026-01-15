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
            // console.log(response);
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
                data: totals,
                barThickness: 50,
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    /*text: 'Project Hold History (in Hours.Minute)'*/
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
                        // Use value from holdLabels array by index
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
    debugger;
    // If chart already exists, destroy it before creating new
    if (projHoldChartcomment) {
        projHoldChartcomment.destroy();
    }

    // Create new chart
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
                    /*text: 'Project Hold History (in Hours.Minute)'*/
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
                        // Use value from holdLabels array by index
                        return totalsForlabel[context.dataIndex];
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
}
function calculateTotalTime(response) {
    // Result object to hold the grouped data
    let result = {};

    // Iterate over the response and process each entry
    response.forEach(entry => {
        const { tounitId, tounit, timeStampfrom, timeStampTo } = entry;

        // Parse timestamps into Date objects
        const from = new Date(timeStampfrom);
        const to = new Date(timeStampTo);

        // Calculate time spent in minutes
        const timeSpent = (to - from) / 1000 / 60; // Difference in minutes

        // Create a unique key for each (fromunitId, statusId, fromunit, status) combination
        const key = `${tounitId}_${tounit}`;

        // If the group doesn't exist, initialize it
        if (!result[key]) {
            result[key] = 0;
        }

        // Add the time spent to the group
        result[key] += timeSpent;
    });

    // Transform the result into a more readable array
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
// Assuming totalTimeSpent is in minutes
function convertMinutesToAgo(minutes) {
    if (minutes == null || isNaN(minutes) || minutes < 0) {
        minutes = 0;
    }
    // Calculate total hours from minutes
    const totalHours = minutes / 60;

    // Calculate remaining minutes after extracting hours
    const remainingMinutes = minutes % 60;

    let ago = "";
    let formatted;

    // Calculate years first
    if (totalHours >= 8760) {
        const days = Math.round(totalHours / 24);
        const hours = Math.round(totalHours % 24);  // Remaining hours after extracting days
        formatted = `${hours.toString().padStart(2, '0')}:${Math.round(remainingMinutes).toString().padStart(2, '0')}`;
        //ago = `${days} Days (${formatted})`;
        const daysforyear = `${days} Days`;



        const years = Math.round(totalHours / 8760);  // 8760 hours in a year
        // const remainingHoursInYear = totalHours % 8760;
        formatted = `${Math.round(totalHours).toString().padStart(3, '0')}:${Math.round(remainingMinutes).toString().padStart(2, '0')}`;
        //ago = `${years} Years (${formatted})`;
        ago = `${years} Years (${daysforyear})`;
    }
    // Calculate months if total hours are less than a year but greater than or equal to 730 hours (~30 days)
    else if (totalHours >= 730) {
        // const months = Math.round(totalHours / 730);  // 730 hours in a month (approx)
        // // const remainingHoursInMonth = totalHours % 730;
        // formatted = `${Math.round(totalHours).toString().padStart(3, '0')}:${Math.round(remainingMinutes).toString().padStart(2, '0')}`;
        // //ago = `${months} Months (${formatted})`;
        // ago = `${months} Months`;
        const days = Math.round(totalHours / 24);
        const hours = Math.round(totalHours % 24);  // Remaining hours after extracting days
        formatted = `${hours.toString().padStart(2, '0')}:${Math.round(remainingMinutes).toString().padStart(2, '0')}`;
        //ago = `${days} Days (${formatted})`;
        ago = `${days} Days`;
    }
    // Calculate days if total hours are less than 730 but greater than 24 hours
    else if (totalHours >= 24) {
        const days = Math.round(totalHours / 24);
        const hours = Math.round(totalHours % 24);  // Remaining hours after extracting days
        formatted = `${hours.toString().padStart(2, '0')}:${Math.round(remainingMinutes).toString().padStart(2, '0')}`;
        //ago = `${days} Days (${formatted})`;
        ago = `${days} Days`;
    }
    // For less than 24 hours, simply show the minutes
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




