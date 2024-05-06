$(document).ready(function () {
    var table = $('#SoftwareXX').DataTable({
        lengthChange: true,
        dom: 'lBfrtip',
        buttons: [
            'copy',
            'excel',
            'csv',
            {
                text: 'PDF',
                extend: 'pdfHtml5',
                action: function (e, dt, node, config) {
                    PdfDiv();
                }
            },
        ],
        searchBuilder: {
            conditions: {
                num: {
                    'MultipleOf': {
                        conditionName: 'Multiple Of',
                        init: function (that, fn, preDefined = null) {
                            var el = $('<input/>').on('input', function () { fn(that, this) });

                            if (preDefined !== null) {
                                $(el).val(preDefined[0]);
                            }

                            return el;
                        },
                        inputValue: function (el) {
                            return $(el[0]).val();
                        },
                        isInputValid: function (el, that) {
                            return $(el[0]).val().length !== 0;
                        },
                        search: function (value, comparison) {
                            return value % comparison === 0;
                        }
                    }
                }
            }
        }
    });

    var buttonContainer = table.buttons().container();
    var searchBuilderContainer = table.searchBuilder.container();
    buttonContainer.insertBefore(table.table().container());
    searchBuilderContainer.insertBefore(table.table().container());

    function PdfDiv() {
        var popupWin = window.open('', '_blank', 'top=100,width=900,height=500,location=no');
        popupWin.document.open();

        var tableStyles = `
                                <style type="text/css">
                                    table {
                                        width: 100%;
                                        border-collapse: collapse;
                                        margin-bottom: 20px;
                                    }

                                    .table > thead {
                                        vertical-align: bottom;
                                        background-color: red;
                                    }

                                    th, td {
                                        padding: 8px;
                                        border: 1px solid #ddd;
                                        text-align: center;
                                    }

                                    th {
                                        background-color: #f2f2f2;
                                        color: black;
                                    }
                                </style>
                             `;

        var table = $('#SoftwareXX').DataTable();

        var filteredData = table.rows({ search: 'applied' }).data().toArray();

        var tableHTML = '<table>';

        tableHTML += '<thead>';
        tableHTML += '<tr>';
        table.columns().header().each(function (header) {
            tableHTML += '<th>' + header.innerHTML + '</th>';
        });
        tableHTML += '</tr>';
        tableHTML += '</thead>';

        tableHTML += '<tbody>';
        for (var i = 0; i < filteredData.length; i++) {
            tableHTML += '<tr>';
            for (var j = 0; j < filteredData[i].length; j++) {
                tableHTML += '<td>' + filteredData[i][j] + '</td>';
            }
            tableHTML += '</tr>';
        }
        tableHTML += '</tbody>';

        tableHTML += '</table>';

        var watermarkText = '@(TempData["ipadd"])';

        popupWin.document.write('<html><head>'
            + tableStyles + '</head><body onload="window.print()">'
            + tableHTML + '<div style="transform: rotate(-45deg);z-index:10000;opacity: 0.3;color: BLACK; position:fixed;top: auto; left: 6%; top: 39%;color: #8e9191;font-size: 80px; font-weight: 500px;display: grid;justify-content: center;align-content: center;">'
            + watermarkText + '</div></body></html>');

        popupWin.document.close();
    }
});


