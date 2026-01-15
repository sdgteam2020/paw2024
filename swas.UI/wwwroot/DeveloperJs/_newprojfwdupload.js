

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
