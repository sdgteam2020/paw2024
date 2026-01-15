
    $(document).ready(function () {
        var fdsetid = '@tabshift';

    var current_fs, next_fs, previous_fs;
    var opacity;
    var steps = $("fieldset").length;
    if (fdsetid == 3) {
            var fdset = "fieldset#" + "upload";
    editFormContainer.style.display = 'none';
    addFormContainer.style.display = 'block';
    $("#1").hide();
    $("#AddlDetails").addClass("active");
    $(fdset).show();

        }
    else if (fdsetid == 12) {
            var fdset = "fieldset#" + "uploaded";
    editFormContainer.style.display = 'block';

    $("#5").hide();
    $("#AddlDetailsed").addClass("active");
    $(fdset).show();
        }
    else {
            var fdset = "fieldset#" + fdsetid + "";

        }



    $("#1").hide();
    $(fdset).show();

    $(".editButton").click(function () {
        $("#1").toggle();
        });

    current = $(fdset).index() + 1;
    setProgressBar(current);





    $(".next").click(function () {
        current_fs = $(this).parent();
    next_fs = $(this).parent().next();
    if (fdsetid == 5) {
        $("#BasicDetailsed").addClass("active");
            }
    else if (fdsetid == 6) {
        $("#AddlDetailsed").addClass("active");
            } else if (fdsetid == 7) {
        $("#Uploaded").addClass("active");
            } else if (fdsetid == 8) {
        $("#confirmed").addClass("active");
            }


    // Validation
    var isValid = true;
    current_fs.find("input[required]").each(function () {
                if ($(this).val() === "") {
        isValid = false;
    $(this).addClass("missing");
    $(this).next(".error-message").text("This field is required.").show();
                } else {
        $(this).removeClass("missing");
    $(this).next(".error-message").hide();
                }
            });



    var selectedValueX = $("#ProjEdit_Apptype").val();
    var ProjEdit_HostTypeX = $("#ProjEdit_HostTypeID").val();

    var ProjEdit_HostTypeXE = $("#Hostedtype").val();
    var ddlAppTypeEditXE = $("#ddlAppTypeEdit").val();



    if ((selectedValueX !== "0" && ProjEdit_HostTypeX !== "0") || (ProjEdit_HostTypeXE !== "0" && ddlAppTypeEditXE !== "0")) {
        isValid = true;
            }
    else {
        isValid = false;

    Swal.fire({
        title: 'Something Went Wrong....!',
    text: 'App type or Hosting Type Not Selected .',
    icon: 'error',
    confirmButtonText: 'OK'
                });

    e.preventDefault();


            }


    if (!isValid) {
                return;
            }

    $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

    next_fs.show();

    current_fs.animate({opacity: 0 }, {
        step: function (now) {
        opacity = 1 - now;
    current_fs.css({
        'display': 'none',
    'position': 'relative'
                    });
    next_fs.css({'opacity': opacity });
                },
    duration: 500,
    complete: function () {
        current_fs.hide();
    next_fs.show();
                }
            });


    setProgressBar(++current);
        });

    $(".previous").click(function () {
        current_fs = $(this).parent();
    previous_fs = $(this).parent().prev();

    $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

    previous_fs.show();

    current_fs.animate({opacity: 0 }, {
        step: function (now) {
        opacity = 1 - now;

    current_fs.css({
        'display': 'none',
    'position': 'relative'
                    });
    previous_fs.css({'opacity': opacity });
                },
    duration: 300,
    complete: function () {
        current_fs.hide();
    previous_fs.show();
                }
            });

    setProgressBar(--current);
        });
    function animateProgressBar() {
        $("#upload-progress-bar").animate({ width: "100%" }, {
            duration: 50,
            complete: function () {

            }
        });
        }

    $("#submitUpload").click(function () {
        current_fs = $(this).parent();
    next_fs = $(this).parent().next();

    current_fs.animate({opacity: 0 }, {
        step: function (now) {
        opacity = 1 - now;
    current_fs.css({
        'display': 'none',
    'position': 'relative'
                    });
    next_fs.css({'opacity': opacity });
                },
    duration: 500,
    complete: function () {
        current_fs.hide();
    next_fs.show();
                }
            });

    $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");
    setProgressBar(++current);
        });



    $("#finaluploaded").click(function () {

            var fdset = "fieldset#" + "9";
    editFormContainer.style.display = 'block';
    addFormContainer.style.display = 'none';
    $("fieldset#6").hide();
    $("fieldset#7").hide();
    $("fieldset#uploaded").hide();

    $(fdset).show();

    animateProgressBar();


        });

    function setProgressBar(curStep) {
            var percent = parseFloat(100 / steps) * curStep;
    percent = percent.toFixed();
    $(".progress-bar")
    .css("width", percent + "%")
        }


    $(".submit").click(function () {
            return false;
        });
    });



    $(document).ready(function () {
        // Wait for the document to be ready

        function submitFormnew() {


            var curPSMid = 0;

            if ((Model.ProjMov != null ? "true" : "false")) {
                curPSMid = Model.ProjMov.PsmId ?? 0;

            }
                


            $.ajax({

                type: 'POST',
                url: 'FwdProjConfirm',
                data: { "projid": curPSMid },
                datatype: "json",

                success: function (response) {
                    window.location.href = '/Projects/ProjDetails';
                    console.log('Request successful', response);
                },
                error: function (error) {
                    console.error('Error occurred:', error);
                }
            });
        }



        // Any other code that you need to run after the document is ready
    });



    $(document).ready(function () {
        $("#next").click(function (e) {
            var selectedValuess = $("#ProjEdit_Apptype").val();
            var ProjEdit_HostTypess = $("#ProjEdit_HostTypeID").val();


            if (selectedValuess === "0" || ProjEdit_HostTypess === "0") {
                Swal.fire({
                    title: 'Something Went Wrong....!',
                    text: 'App type or Hosting Type Not Selected .',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });

                e.preventDefault(); // Prevent form submission
            }
        });
    });

