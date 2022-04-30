let myChart1;
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


            let AppDescNames = [...new Set(data.filter(item => item.AppDesc !== null).map(item => item.AppDesc))];

            let allMonths = getLastSixMonthNames();
            let AllMonthss = [...new Set(data.map(item => item.MonthName))];

            let datasets = AppDescNames.map(AppName => {
                let appCounts = AllMonthss.map(month => {
                    let monthData = data.find(item => item.MonthName === month && item.AppDesc === AppName);
                    return monthData ? monthData.AppTypeCount : 0;
                });



                return {

                    label: AppName,
                    data: appCounts,
                    backgroundColor: getRandomColor(),


                };
            });

            let ctx = document.getElementById('myChart').getContext('2d');
            let myChart = new Chart(ctx, {
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
    let num = parseInt(color.replace("#", ""), 16),
        amt = Math.round(2.55 * percent),
        R = (num >> 16) + amt,
        B = (num >> 8 & 0x00FF) + amt,
        G = (num & 0x0000FF) + amt;
    return "#" + (0x1000000 + (R < 255 ? R < 1 ? 0 : R : 255) * 0x10000 + (B < 255 ? B < 1 ? 0 : B : 255) * 0x100 + (G < 255 ? G < 1 ? 0 : G : 255)).toString(16).slice(1);
}
function getRandomColorss() {
    let letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

function updatePieChart(data) {

    let titles = data.map(item => item.Status);
    let chartData = data.map(item => item.TotalProj);


    let canvas = document.getElementById('myChart1');
    if (!canvas) {
        console.error("Canvas element 'myChart1' not found.");
        return;
    }
    let backgroundColors = generateRandomColors(titles.length);

    let ctx = canvas.getContext('2d');

    let myChart1 = new Chart(ctx, {
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
} function getRandomColor() {
    const minBrightness = 130;
    let color;

    do {
        color = '#' + Math.floor(Math.random() * 16777215).toString(16).padStart(6, '0');

        const rgb = parseInt(color.slice(1), 16);

        const r = Math.trunc(rgb / (256 * 256)) % 256;
        const g = Math.trunc(rgb / 256) % 256;
        const b = rgb % 256;

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
    let letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}
function getLastSixMonthNames() {
    let today = new Date();
    let months = [];

    for (let i = 5; i >= 0; i--) {
        let date = new Date(today);
        date.setMonth(today.getMonth() - i);
        let monthName = date.toLocaleString('default', { month: 'long' }) + ' ' + date.getFullYear().toString().slice(-2);

        months.push(monthName);
    }

    return months;
}