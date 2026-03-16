

  
$(document).ready(function () {
   
    
    loadTracker()


        //setProgress(1);
    


  
    const pdfFileInput = document.getElementById('pdfFileInput');

    pdfFileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];

        if (file) {


            const maxSizeInBytes = 10 * 1024 * 1024;
            if (file.size > maxSizeInBytes) {

                pdfFileInput.value = '';
                Swal.fire({
                    title: 'File Size Exceeded',
                    text: 'Please select a file smaller than 10MB.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                return;
            }


            const reader = new FileReader();
            reader.onloadend = function () {
                const bytes = new Uint8Array(reader.result);
                const pdfHeader = new Uint8Array([37, 80, 68, 70, 45]); // %PDF-
                const isPDF = compareArrays(bytes.slice(0, 5), pdfHeader);
                if (isPDF) {

                    console.log('PDF file is valid. Proceed with upload.');
                } else {

                    Swal.fire({
                        title: 'Invalid File ....!',
                        text: 'Invalid PDF file. Please select a valid PDF file.',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });

                    pdfFileInput.value = '';
                }
            };


            reader.readAsArrayBuffer(file);
        }
    });


    function compareArrays(array1, array2) {
        if (array1.length !== array2.length) {
            return false;
        }
        for (let i = 0; i < array1.length; i++) {
            if (array1[i] !== array2[i]) {
                return false;
            }
        }
        return true;
    }
});


function trackerIndex() {
   
    var $activeStep = $(".stepsforatt .step.active");

    if ($activeStep.length === 0) {
        setProgress(1);
        return;
    }

    var currentStep = $activeStep.index() + 1;
    setProgress(currentStep + 1);
}



function setProgress(stepNumber) {
   
    var $steps = $(".stepsforatt .step");
    var totalSteps = $steps.length;

    // ✅ Safety boundary
    stepNumber = Math.max(1, Math.min(stepNumber, totalSteps));

    // Reset
    $steps.removeClass("active completed");

    $steps.each(function (index) {

        var stepIndex = index + 1;
        var $circle = $(this).find(".step-circle");

        if (stepIndex < stepNumber) {
            $(this).addClass("completed");
            $circle.html("✓");
        }
        else if (stepIndex === stepNumber) {
            $(this).addClass("active");
            $circle.html(stepIndex);
        }
        else {
            $circle.html(stepIndex);
        }

    });

    // ✅ Update Progress Bar
    var progressPercent = ((stepNumber - 1) / (totalSteps - 1)) * 100;
    $(".progress").css("width", progressPercent + "%");

    // ✅ Update Textbox (ONLY ONCE)
    var currentLabel = $steps
        .eq(stepNumber - 1)
        .find(".tracker-label")
        .text()
        .trim();

    $("#Reamarks").val(currentLabel);
}


function syncTrackerWithResponse(response) {

    var $steps = $(".stepsforatt .step");
    var totalSteps = $steps.length;

    var uploadedDocs = [];

    if (response && response.length > 0) {
        for (var i = 0; i < response.length; i++) {
            if (response[i].reamarks) {
                uploadedDocs.push(response[i].reamarks.trim());
            }
        }
    }

    $steps.removeClass("active completed");

    var firstIncompleteIndex = -1;
    var completedCount = 0;

    $steps.each(function (index) {

        var $step = $(this);
        var $circle = $step.find(".step-circle");
        var label = $step.find(".tracker-label").text().trim();

        if (uploadedDocs.includes(label)) {
            $step.addClass("completed");
            $circle.html("✓");
            completedCount++;
        }
        else {
            $circle.html(index + 1);

            if (firstIncompleteIndex === -1) {
                firstIncompleteIndex = index;   // ✅ FIRST missing step
            }
        }
    });

    // ✅ Activate first missing step
    if (firstIncompleteIndex !== -1) {
        $steps.eq(firstIncompleteIndex).addClass("active");
    }
    else {
        // All completed
        $steps.last().addClass("active");
    }

    // ✅ Progress calculation (based on completed steps)
    var progressPercent = totalSteps > 1
        ? (completedCount / (totalSteps - 1)) * 100
        : 0;

    $(".progress").css("width", progressPercent + "%");

    // ✅ Update textbox with active step label
    var activeLabel = $(".stepsforatt .step.active .tracker-label")
        .text()
        .trim();

    $("#Reamarks").val(activeLabel);
}

function loadTracker() {

    $.get("/Projects/GetDocumentTypes", function (data) {

        var html = "";

        for (var i = 0; i < data.length; i++) {

            html += `
                <div class="step" data-document-id="${data[i].id}">
                    <div class="step-circle">${i + 1}</div>
                    <div class="step-label">
                        <span class="tracker-label">${data[i].name}</span>
                        ${data[i].isRequired ? '<span class="paw-red-font">*</span>' : ''}
                    </div>
                </div>
            `;
        }

        $(".stepsforatt").html(html);

        AttechHistory()
    });
}