$(document).on('ready', function () {
    getAppConfig();

    $('#PolicyWhiteList').on('click', function () {
        $('#ProjectPolicyModel').modal('show');
    });

    $('#whitelistCard').on('click', function () {
        $('#ProjectUnderProcess').modal('show');
    });

    $('#WhiteListedProject').on('click', function () {
        $('#WhiteListedProjectDetail').modal('show');
    });
});
function openForm() {
    document.getElementById("myForm").style.display = "block";
}

function closeForm() {
    document.getElementById("myForm").style.display = "none";
}
function getAppConfig() {
    var configElement = document.getElementById('app-config');

    if (!configElement) {
        console.warn('App config element not found');
        return {
            flag: '',
            ButtonText: '',
            username: '',
            ip: ''
        };
    }

    return {
        flag: configElement.dataset.flag || '',
        ButtonText: configElement.dataset.buttonText || '',
        username: configElement.dataset.username || '',
        ip: configElement.dataset.ip || ''
    };
}