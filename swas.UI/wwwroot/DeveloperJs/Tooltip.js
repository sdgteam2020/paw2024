document.addEventListener("DOMContentLoaded", function () {

    const tooltipContainers = document.querySelectorAll('.tooltip-container');

    tooltipContainers.forEach(container => {

        container.addEventListener('mouseenter', () => {
            const tooltip = container.querySelector('.tooltip-text, .tooltip-text1');
            if (tooltip) {
                tooltip.classList.add('tooltip-visible');
            }
        });

        container.addEventListener('mouseleave', () => {
            const tooltip = container.querySelector('.tooltip-text, .tooltip-text1');
            if (tooltip) {
                tooltip.classList.remove('tooltip-visible');
            }
        });

    });

});