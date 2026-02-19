(function () {
    try {
        var isReload = false;

        if (performance && performance.navigation) {
            isReload = (performance.navigation.type === 1);
        } else {
            var navEntries = performance.getEntriesByType && performance.getEntriesByType("navigation");
            if (navEntries && navEntries.length) {
                isReload = (navEntries[0].type === "reload");
            }
        }

        if (isReload) localStorage.setItem("pageRefreshed", "true");
    } catch (e) {
    }
})();

/**
 * Your preventBack logic moved from inline.
 */
(function () {
    function preventBack() {
        window.history.forward();
    }
    window.setTimeout(preventBack, 0);
    window.onunload = function () { return null; };
})();

/**
 * Your ValInData(input) function moved from inline.
 * Keep global because you might be calling it from HTML: oninput="ValInData(this)"
 */
window.ValInData = function (input) {
    var regex = /[^a-zA-Z0-9/ ]/g;
    input.value = input.value.replace(regex, "");
};

/**
 * Your jQuery AJAX loader show/hide moved from inline.
 */
$(document).ready(function () {
    $("#loading").hide();

    $(document)
        .ajaxStart(function () {
            $("#loading").show();
        })
        .ajaxStop(function () {
            $("#loading").hide();
        })
        .ajaxError(function () {
            $("#loading").hide();
        });
});
