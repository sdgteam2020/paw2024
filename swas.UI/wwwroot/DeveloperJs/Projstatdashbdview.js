


    $(function () {
        $(".processDetail").click(function () {

            var $buttonClicked = $(this);
            var ProjectId = $buttonClicked.attr('data-id');

            Swal.fire({
                title: 'Are you sure?',
                text: "Do you want to Process!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, Process it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/ProjStakeHolderMov/ProcessMail',
                        type: 'POST',
                        data: { "id": ProjectId, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                        success: function (response) {
                            console.log(response);
                            if (response && response === 1) {
                                Swal.fire({
                                    position: 'top-end',
                                    icon: 'success',
                                    title: 'Project Processed successfully',
                                    showConfirmButton: false,
                                    timer: 700
                                });
                            }

                            window.location.reload();
                        },
                        error: function (error) {
                            console.error('Error occurred:', error);
                            // Handle error if needed
                        }
                    });
                }
            });

        });
    });











    $(function () {
        $(".RetDuplicate").click(function () {

            var $buttonClicked = $(this);
            var ProjectId = $buttonClicked.attr('data-id');

            Swal.fire({
                title: 'Are you sure?',
                text: "Do you want to Return!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, Return it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/ProjStakeHolderMov/RetDuplicate',
                        type: 'POST',
                        data: { "id": ProjectId, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                        success: function (response) {
                            console.log(response);
                            if (response && response === 1) {
                                Swal.fire({
                                    position: 'top-end',
                                    icon: 'success',
                                    title: 'Project Returned successfully',
                                    showConfirmButton: false,
                                    timer: 700
                                });
                            }

                            window.location.reload();
                        },
                        error: function (error) {
                            console.error('Error occurred:', error);
                            // Handle error if needed
                        }
                    });
                }
            });

        });
    });


    $(function () {
        $(".RetObsn").click(function () {
            var $buttonClicked = $(this);
            var ProjectId = $buttonClicked.attr('data-id');

            Swal.fire({
                title: 'Are you sure?',
                text: "Do you want to Return with Obsn!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, Return it!',
                input: 'text', // Add an input field for comments
                inputPlaceholder: 'Enter your observation comments...',
                inputValidator: (value) => {
                    // Apply the regex to remove unwanted characters
                    var cleanedValue = value.replace(/[^a-zA-Z0-9/ ]/g, "");

                    // Update the input value with the cleaned value
                    Swal.getInput().value = cleanedValue;

                    // You can add additional validation if needed
                    if (!cleanedValue) {
                        return 'Please enter your observation comments!';
                    }
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    var observationComments = Swal.getInput().value;

                    $.ajax({
                        url: '/ProjStakeHolderMov/RetwithObsn',
                        type: 'POST',
                        data: {
                            "id": ProjectId,
                            "observationComments": observationComments,
                            "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (response) {
                            console.log(response);
                            if (response && response === 1) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Project Returned successfully',
                                    showConfirmButton: false,
                                    timer: 900
                                });
                            }

                            window.location.reload();
                        },
                        error: function (error) {
                            console.error('Error occurred:', error);
                            // Handle error if needed
                        }
                    });
                }
            });

            // Add event listener for keyup and input events
            Swal.getInput().addEventListener('keyup', function () {
                var cleanedValue = this.value.replace(/[^a-zA-Z0-9/ ]/g, "");
                this.value = cleanedValue;
            });

            Swal.getInput().addEventListener('input', function () {
                var cleanedValue = this.value.replace(/[^a-zA-Z0-9/ ]/g, "");
                this.value = cleanedValue;
            });
        });
    });










