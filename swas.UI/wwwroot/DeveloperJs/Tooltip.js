

document.addEventListener("DOMContentLoaded", function (event) {
    var tooltipContainers = document.querySelectorAll('.tooltip-container');
    tooltipContainers.forEach(container => {
        container.addEventListener('mouseenter', () => {
            const tooltip = container.querySelector('.tooltip-text');
            if (tooltip) {
                tooltip.style.visibility = 'visible';
                tooltip.style.opacity = '1';
            }
        });

        container.addEventListener('mouseleave', () => {
            const tooltip = container.querySelector('.tooltip-text');
            if (tooltip) {
                tooltip.style.visibility = 'hidden';
                tooltip.style.opacity = '0';
            }
        });
    });
});


document.addEventListener("DOMContentLoaded", function (event) {
    var tooltipContainers = document.querySelectorAll('.tooltip-container');
    tooltipContainers.forEach(container => {
        container.addEventListener('mouseenter', () => {
            const tooltip = container.querySelector('.tooltip-text1');
            if (tooltip) {
                tooltip.style.visibility = 'visible';
                tooltip.style.opacity = '1';
            }
        });

        container.addEventListener('mouseleave', () => {
            const tooltip = container.querySelector('.tooltip-text1');
            if (tooltip) {
                tooltip.style.visibility = 'hidden';
                tooltip.style.opacity = '0';
            }
        });
    });
}); 