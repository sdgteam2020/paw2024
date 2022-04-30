
$('.controlvalidation').on('keyup', function () {
    ValInData(this);
})


    function ValInData(input) {
        let regex = /[^a-zA-Z0-9/ ]/g;
        input.value = input.value.replace(regex, "");
    }
