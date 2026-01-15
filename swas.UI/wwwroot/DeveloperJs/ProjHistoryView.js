
    $(document).ready(function () {

        var TeamDetailPostBackURL = '/Projects/AttDetails';
        $(function () {
            $(".anchorDetail").click(function () {

                var $buttonClicked = $(this);
                var id = $buttonClicked.attr('data-id');
                var options = { "backdrop": "static", keyboard: true };
                $.ajax({
                    type: "GET",
                    url: TeamDetailPostBackURL,
                    contentType: "application/json; charset=utf-8",
                    data: { "Id": id },
                    datatype: "json",
                    success: function (data) {

                        $('#myModalContent').html(data);
                        $('#myModal').modal(options);
                        $('#myModal').modal('show');

                    },
                    error: function () {
                        alert("Dynamic content load failed.");
                    }
                });

            });

        });





        $(document).on('click', '.pdf', function () {
            $('#ViewRecordsHistory').modal('show');
        });
    });































    document.addEventListener("DOMContentLoaded", function () {
        const pdfInput = document.getElementById("pdfInput");
        const pdfList = document.getElementById("pdfList");
        const selectedFileList = document.getElementById("selected-file-list");
        const remarksInput = document.getElementById("remarksInput");
        const uploadedFiles = [];

         $("#pdfInput").on("change", function (e) {
        handleFileUpload(e);
    });
    })
        function handleFileUpload(event) {

            const files = event.target.files;
            for (const file of files) {
                const fileType = file.type;

                if (fileType.includes("image") || fileType === "application/pdf") {
                    const pdfItem = createPDFItem(file);
                    pdfList.appendChild(pdfItem);

                    uploadedFiles.push({
                        file: file,
                        pdfItem: pdfItem,
                        remarks: ""
                    });

                    pdfInput.value = "";
                    updateSelectedFileList();
                }
            }
        }

        function createPDFItem(file) {
            const pdfItem = document.createElement("div");
            pdfItem.className = "pdf-item";
            pdfItem.setAttribute("data-filename", file.name);

            const pdfPreview = document.createElement("div");
            pdfPreview.className = "pdf-preview";

            const reader = new FileReader();
            reader.onload = function () {
                const fileType = file.type;

                if (fileType.includes("image")) {
                    pdfPreview.innerHTML = `<img src="${reader.result}" alt="Image Preview" class="image-preview">`;
                } else if (fileType === "application/pdf") {
                    pdfPreview.innerHTML = `<embed src="${reader.result}" type="application/pdf" width="100%" height="400px" />`;
                }
            };
            reader.readAsDataURL(file);

            const deleteButton = document.createElement("button");
            deleteButton.innerHTML = "&times;";
            deleteButton.className = "delete-button";
            deleteButton.addEventListener("click", () => {
                pdfList.removeChild(pdfItem);
                const index = uploadedFiles.findIndex(item => item.pdfItem === pdfItem);
                if (index !== -1) {
                    uploadedFiles.splice(index, 1);
                }
                pdfInput.value = "";

                updateSelectedFileList();
            });

            pdfItem.appendChild(pdfPreview);
            pdfItem.appendChild(deleteButton);

            return pdfItem;
        }

        function formatFileSize(size) {
            const units = ['B', 'KB', 'MB', 'GB'];
            let index = 0;
            while (size >= 1024 && index < units.length - 1) {
                size /= 1024;
                index++;
            }
            return `${size.toFixed(2)} ${units[index]}`;
        }

        function updateSelectedFileList() {
            const selectedFileList = document.getElementById("selectedFileList");
            selectedFileList.innerHTML = "";

            for (const uploadedFile of uploadedFiles) {
                const listItem = document.createElement("li");
                const remarks = uploadedFile.remarks !== "" ? uploadedFile.remarks : "No Remarks";
                const fileSize = formatFileSize(uploadedFile.file.size);
                listItem.innerHTML = `<a href="#" class="popup-link">${uploadedFile.file.name} (${fileSize}) - Remarks: ${remarks}</a>`;
                selectedFileList.appendChild(listItem);
            }
        }

        document.addEventListener("DOMContentLoaded", function () {
            const popup = document.getElementById("popup");
            const closePopupButton = document.getElementById("closePopup");

            const popupLinks = document.querySelectorAll(".popup-link");
            popupLinks.forEach(link => {
                link.addEventListener("click", function (event) {
                    event.preventDefault();
                    popup.style.display = "block";
                });
            });

            closePopupButton.addEventListener("click", function () {
                popup.style.display = "none";
            });
        });



        remarksInput.addEventListener("input", function () {
            const remarks = remarksInput.value;
            const lastFileIndex = uploadedFiles.length - 1;
            uploadedFiles[lastFileIndex].remarks = remarks;
            updateSelectedFileList();
        });
        updateSelectedFileList();
    });


    function PrintDiv() {
        var divToPrint = document.getElementById('widget-content');
    var popupWin = window.open('', '_blank', 'width=800,height=800,location=Center,Center=20px');
    popupWin.document.open();

    var table_style = document.getElementById("table_style").innerHTML;
    popupWin.document.write('<style type="text/css">');
        popupWin.document.write(table_style);
        popupWin.document.write('</style>');

    popupWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '<div class="watermark">' + '@(TempData["ipadd"])' + " " + '</div></html>');

    popupWin.document.close();
    }


