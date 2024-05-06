
$(document).ready(function () {

    populateStages();

    $('#ddlStages').on('change', function () {

        var stageIds = $(this).val();

        if (stageIds > 0) {
            $('#ddlStatus').empty();

            getStatusByStage(stageIds);

        } else {
            $('#ddlStatus').empty();

        }
    });

});


$(document).ready(function () {
    $("#LimitimeInputa").on("input", function () {

        var inputValue = $(this).val();

        var numericValue = parseFloat(inputValue);

        if (numericValue < 0) {

            $(this).val('');

            Swal.fire({
                title: 'Negative Value Not Allowed',
                text: 'Please Enter Possitive Values...',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
});


const container = document.getElementById('container');
const boxes = document.querySelectorAll('.box');
const linkLines = document.querySelectorAll('.link-line');

const totalItems = boxes.length;
const scrollThreshold = Math.ceil(totalItems / 2); // Scroll when reaching 50% of items

function animateBoxes() {
    boxes.forEach((box, index) => {
        setTimeout(() => {
            box.style.opacity = 1;
            box.style.transform = 'translateY(0)';

            // Animate link lines alongside the box animations
            if (index < linkLines.length) {
                setTimeout(() => {
                    linkLines[index].style.height = `${box.scrollHeight}px`;
                }, 500); // Adjust the timing as needed
            }

            if (index >= scrollThreshold) {
                // Smoothly scroll the container as new boxes appear
                box.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }

            // Scroll to the end of the container after the last box is revealed
            if (index === totalItems - 1) {
                setTimeout(() => {
                    container.scrollIntoView({ behavior: 'smooth', block: 'end' });
                }, 500 * (index + 1)); // Adjust the timing as needed
            }
        }, 500 * (index + 1)); // Adjust the timing as needed
    });
}

animateBoxes();




function functionConfirm1(ActionsId) {
    console.log('functionConfirm1 called with id:', ActionsId);

    Swal.fire({
        title: 'Are you sure?',
        text: "Do you want to delete!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, Delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Actions/Delete',
                type: 'POST',
                data: { "id": ActionsId, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {
                    console.log(response);
                    if (response) {
                        if (response == 1) {
                            Swal.fire({
                                position: 'top-end',
                                icon: 'success',
                                title: 'Record Deleted successfully',
                                showConfirmButton: false,
                                timer: 1500
                            });
                        }
                    }

                    window.location.href = '@Url.Action("Index", "Actions")';
                }
            });
        }
    });
}
