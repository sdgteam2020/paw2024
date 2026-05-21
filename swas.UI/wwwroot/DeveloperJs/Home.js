
$(document).on('ready', function () {
    $('.table-button').on('click', function () {
        let $button = $(this);
        let stakeHolderId = $button.data('stakeholder-id');
        let projId = $button.data('proj-id');
        let psmId = $button.data('psm-id');
        aler("Sanal");
        $('#StakeholdertextId').val(stakeHolderId);
        $('#ProjtextId').val(projId);
        $('#PsmToProj').val(psmId);
        let status = $button.closest('td').attr('class');
        $button.removeClass('green red yellow'); // Remove existing color classes
        $button.addClass(status); // Add the new color class
    });
    $('.dropdownsearch').select2();
});




document.onreadystatechange = function () {
    if (document.readyState === 'interactive') {
        let popupTriggers = document.getElementById("preDev");
        let popupOverlay = document.getElementById("popupOverlay");
        let closeButton = document.getElementById("closeButton");

        popupTriggers.onclick = function () {
            popupOverlay.style.display = "block";
        };

        closeButton.onclick = function () {
            popupOverlay.style.display = "none";
        };

        popupOverlay.onclick = function (event) {
            if (event.target === popupOverlay) {
                popupOverlay.style.display = "none";
            }
        };
    }
};






document.onreadystatechange = function () {
    if (document.readyState === 'interactive') {
        let popupTrigger = document.getElementById("popupTrigger");
        let popupContent = document.getElementById("popupContent");

        popupTrigger.onclick = function () {
            if (popupContent.style.display === "none" || popupContent.style.display === "") {
                popupContent.style.display = "block";
            } else {
                popupContent.style.display = "none";
            }
        };
    }
};




let TeamDetailPostBackURL = '/Projects/AttDetails';
$(function () {
    $(".anchorDetail").click(function () {

        let $buttonClicked = $(this);
        let id = $buttonClicked.attr('data-id');
        let options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: TeamDetailPostBackURL,
            contentType: "application/json; charset=utf-8",
            data: { "Id": id },
            datatype: "json",
            success: function (data) {

                $('#myModalContent').html(data);
                $('#myModal').modal(options);
                $('#myModal').modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });

    });

});






let myChart1;

$(document).ready(function () {

    $.ajax({
        url: '/Home/indexToBarChartS',
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data.error) {
                console.error('Error fetching data:', data.error);
                return;
            }

            let monthNames = [...new Set(data.map(item => item.MonthNameYr))];
            let unitNames = [...new Set(data.map(item => item.unitname))];

            let datasets = unitNames.map(unitName => {
                let totalInData = [];
                let totalOutData = [];

                monthNames.forEach(month => {
                    let monthData = data.find(item => item.MonthNameYr === month && item.unitname === unitName);
                    if (monthData) {
                        totalInData.push(monthData.TotalIn);
                        totalOutData.push(monthData.TotalOut);
                    } else {
                        totalInData.push(0);
                        totalOutData.push(0);
                    }
                });

                let unitNames = [...new Set(data.map(item => item.unitname))];

                let colors = []; // Store unique colors for each unit

                let totalInColor = getRandomColorss(); // Get a random color for TotalIn bars
                let totalOutColor = getRandomColorss(); // Get a random color for TotalOut bars


                let colors = []; // Store unique colors for each unit
                unitNames.forEach(unitName => {
                    colors.push(getRandomColorss()); // One color for TotalIn
                    colors.push(getRandomColorss()); // Another color for TotalOut
                });

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

            }).flat(); // Use flat() to flatten the array of arrays into a single array


            let ctx = document.getElementById('myChart').getContext('2d');
            let myChart = new Chart(ctx, {
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
        }
    });
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





})



const containerTabs = document.getElementById("tabs");

const getInfoTabs = container => {
    return [...container.querySelectorAll(".tabs__content__item")];
};

const getLinksTab = container => {
    return [...container.querySelectorAll("a[data-tab]")];
};

const activateTab = (tabId) => {
    const tabsInfo = getInfoTabs(containerTabs);

    tabsInfo.forEach(tab => {
        const isActive = tab.getAttribute("id") === tabId;
        tab.classList.toggle("active-tab", isActive);
    });
};

const activateLink = (link) => {
    const linksTab = getLinksTab(containerTabs);
    linksTab.forEach(tabLink => {
        tabLink.classList.toggle("active-link", tabLink === link);
    });
};

const handleTabClick = event => {
    const clickedElement = event.target;
    if (clickedElement.tagName === "A" && clickedElement.hasAttribute("data-tab")) {
        event.preventDefault();
        const tabId = clickedElement.getAttribute("data-tab");
        activateTab(tabId);


        activateLink(clickedElement);
    }
};

containerTabs.onclick = handleTabClick;
const firstTabLink = getLinksTab(containerTabs)[0];
if (firstTabLink) {
    firstTabLink.click();
}



$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

function openPopup(id, status) {
    let newUrl = "/Projects/ProjStatDashBdView?id=" + encodeURIComponent(id) + "&status=" + encodeURIComponent(status);

    window.location.href = newUrl;

}




function openPopup(id) {
    document.getElementById('modalContent').innerText = id;
    $('#myModal').modal('show');
    window.location.href = '/ControllerName/ActionName?id=' + id;
}

$(document).ready(function () {
    $("#ddlUnitId").change(function () {
        let selectedMode = $(this).val();
    });
});

function ValInData(input) {
    let regex = /[^a-zA-Z0-9/ ]/g;
    input.value = input.value.replace(regex, "");
}

$(document).ready(function () {
    $(document).on('click', '.pluscircless', function () {
        $('#RegisterX').modal('show');

    });

});

$(document).ready(function () {
    $(document).on('click', '.pluscircle', function () {
        $('#UnitAdd').modal('show');

    });

});

function processButtonClick() {
    let buttonColor = "@(ViewBag.ProcessButtonColor)";

    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to proceed?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: buttonColor === 'green' ? '#28a745' : (buttonColor === 'red' ? '#dc3545' : '#007bff'), // Green for "OK", Red for "Cancel"
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancel'

    }).then((result) => {
        if (result.isConfirmed) {
            if (buttonColor === 'green') {
                let signInUrl = '/Home/Index';
                window.location.href = signInUrl;
            } else if (buttonColor === 'red') {
                let signUpUrl = '/Identity/Account/Register';
                window.open(signUpUrl, '_blank');
            }
        }
    });
};

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

function openPopup(id, status) {
    let newUrl = "/Projects/ProjStatDashBdView?id=" + encodeURIComponent(id) + "&status=" + encodeURIComponent(status);

    window.location.href = newUrl;

}