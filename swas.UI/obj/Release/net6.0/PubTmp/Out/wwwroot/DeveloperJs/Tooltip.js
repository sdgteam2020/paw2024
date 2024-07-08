
//document.addEventListener("DOMContentLoaded", function (event) {
//    var tooltipContainers = document.querySelectorAll('.tooltip-container');
//    tooltipContainers.forEach(container => {
//        container.addEventListener('mouseenter', () => {
//            const tooltip = container.querySelector('.tooltip');
//            tooltip.style.visibility = 'visible';
//            tooltip.style.opacity = '1';
//        });

//        container.addEventListener('mouseleave', () => {
//            const tooltip = container.querySelector('.tooltip');
//            tooltip.style.visibility = 'hidden';
//            tooltip.style.opacity = '0';
//        });

//        var tooltipText = container.dataset.tooltip;
//        var words = tooltipText.split(' ');
//        var shortText = words.slice(0, 6).join(' ');
//        var longText = words.slice(0).join(' ');
//        container.querySelector('.short-text').textContent = shortText;
//        container.querySelector('.tooltip').textContent = longText;
//    });
//});

//document.addEventListener("DOMContentLoaded", function (event) {
//    var tooltipContainers = document.querySelectorAll('.tooltip-containerRemark');
//    tooltipContainers.forEach(container => {
//        container.addEventListener('mouseenter', () => {
//            const tooltip = container.querySelector('.tooltip');
//            tooltip.style.visibility = 'visible';
//            tooltip.style.opacity = '1';
//        });

//        container.addEventListener('mouseleave', () => {
//            const tooltip = container.querySelector('.tooltip');
//            tooltip.style.visibility = 'hidden';
//            tooltip.style.opacity = '0';
//        });

//        var tooltipText = container.dataset.tooltip;
//        var words = tooltipText.split(' ');
//        var shortText = words.slice(0, 2).join(' ');
//        var longText = words.slice(0).join(' ');
//        container.querySelector('.short-textRemark').textContent = shortText;
//        container.querySelector('.tooltip').textContent = longText;
//    });
//});

document.addEventListener("DOMContentLoaded", function (event) {
    var tooltipContainers = document.querySelectorAll('.tooltip-container');
    tooltipContainers.forEach(container => {
        container.addEventListener('mouseenter', () => {
            const tooltip = container.querySelector('.tooltip-text');
            tooltip.style.visibility = 'visible';
            tooltip.style.opacity = '1';
        });

        container.addEventListener('mouseleave', () => {
            const tooltip = container.querySelector('.tooltip-text');
            tooltip.style.visibility = 'hidden';
            tooltip.style.opacity = '0';
        });
    });
});


document.addEventListener("DOMContentLoaded", function (event) {
    var tooltipContainers = document.querySelectorAll('.tooltip-container');
    tooltipContainers.forEach(container => {
        container.addEventListener('mouseenter', () => {
            const tooltip = container.querySelector('.tooltip-text1');
            tooltip.style.visibility = 'visible';
            tooltip.style.opacity = '1';
        });

        container.addEventListener('mouseleave', () => {
            const tooltip = container.querySelector('.tooltip-text1');
            tooltip.style.visibility = 'hidden';
            tooltip.style.opacity = '0';
        });
    });
}); 