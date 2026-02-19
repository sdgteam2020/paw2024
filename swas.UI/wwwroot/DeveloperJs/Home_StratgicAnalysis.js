var myChart1;
$(document).on('ready', function () {
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

        },
        error: function (error) {

            console.error('Error fetching data:', error);
        }
    });

    $.ajax({
        url: '/Home/indexToBarChart',
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data.error) {
                console.error('Error fetching data:', data.error);
                return;
            }


            var AppDescNames = [...new Set(data.filter(item => item.AppDesc !== null).map(item => item.AppDesc))];

            var allMonths = getLastSixMonthNames();
            var AllMonthss = [...new Set(data.map(item => item.MonthName))];

            var datasets = AppDescNames.map(AppName => {
                var appCounts = AllMonthss.map(month => {
                    var monthData = data.find(item => item.MonthName === month && item.AppDesc === AppName);
                    return monthData ? monthData.AppTypeCount : 0;
                });



                return {

                    label: AppName,
                    data: appCounts,
                    backgroundColor: getRandomColor(),


                };
            });

            var ctx = document.getElementById('myChart').getContext('2d');
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: allMonths,
                    datasets: datasets,
                },
                options: {
                    scales: {
                        x: {
                            stacked: false,
                            title: {
                                display: true,
                                text: 'Month'
                            }
                        },
                        y: {
                            stacked: false,
                            title: {
                                display: true,
                                text: 'AppType Count'
                            }
                        }
                    }
                }
            });
        }
    });

})
function lightenColor(color, percent) {
    var num = parseInt(color.replace("#", ""), 16),
        amt = Math.round(2.55 * percent),
        R = (num >> 16) + amt,
        B = (num >> 8 & 0x00FF) + amt,
        G = (num & 0x0000FF) + amt;
    return "#" + (0x1000000 + (R < 255 ? R < 1 ? 0 : R : 255) * 0x10000 + (B < 255 ? B < 1 ? 0 : B : 255) * 0x100 + (G < 255 ? G < 1 ? 0 : G : 255)).toString(16).slice(1);
}
function getRandomColorss() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
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
        color = '#' + Math.floor(Math.random() * 16777215).toString(16); // Generate random color

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

function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}
function getLastSixMonthNames() {
    var today = new Date();
    var months = [];

    for (var i = 5; i >= 0; i--) {
        var date = new Date(today);
        date.setMonth(today.getMonth() - i);
        var monthName = date.toLocaleString('default', { month: 'long' }) + ' ' + date.getFullYear().toString().slice(-2);

        months.push(monthName);
    }

    return months;
}