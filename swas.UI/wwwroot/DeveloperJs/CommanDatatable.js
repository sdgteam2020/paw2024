function initializeDataTable(tableSelector) {
    debugger;
    return $(tableSelector).DataTable({
        lengthChange: true,
        dom: 'lBfrtip',
        retrieve: true,
        destroy: true,
        pageLength: -1,
        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [
            {
                extend: 'excel',
                text: 'Excel',
                exportOptions: {
                    columns: ':visible:not(:last-child)',
                    format: {
                        body: function (data, row, column, node) {
                            var text = typeof data === 'string' && data.indexOf('<') >= 0 ? $(data).text().trim() : data;
                            return column === 0 ? row + 1 : text;
                        }
                    }
                }
            },
            {
                extend: 'csv',
                exportOptions: {
                    columns: ':visible:not(:last-child)',
                    format: {
                        body: function (data, row, column, node) {
                            var text = typeof data === 'string' && data.indexOf('<') >= 0 ? $(data).text().trim() : data;
                            return column === 0 ? row + 1 : text;
                        }
                    }
                }
            },
            {
                extend: 'pdfHtml5',
                text: 'PDF',
                exportOptions: {
                    columns: ':visible:not(:last-child)'
                },
                action: function (e, dt, node, config) {
                    PdfDiv(tableSelector); // dynamically use the table passed
                }
            }
        ],
        searchBuilder: {
            conditions: {
                num: {
                    'MultipleOf': {
                        conditionName: 'Multiple Of',
                        init: function (that, fn, preDefined = null) {
                            var el = $('<input>').on('input', function () { fn(that, this); });
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
}
function PdfDiv(tableSelector, watermarkSelector = "#IpAddress") {
    debugger;
    const table = $(tableSelector).DataTable();
    const filteredData = table.rows({ search: 'applied' }).data().toArray();

    let headers = [];
    table.columns(':visible').header().each(function (header, index) {
        if (index !== table.columns().count() - 1) {
            headers.push($(header).text().trim());
        }
    });

    let data = [];
    for (let i = 0; i < filteredData.length; i++) {
        let rowData = [];
        for (let j = 0; j < filteredData[i].length - 1; j++) {
            let cellData = filteredData[i][j];
            let cleanText = (typeof cellData === 'string' && cellData.indexOf('<') >= 0)
                ? cellData.replace(/<br\s*\/?>/gi, '\n').replace(/<\/?[^>]+(>|$)/g, "").trim()
                : cellData;
            rowData.push(j === 0 ? i + 1 : cleanText);
        }
        data.push(rowData);
    }

    let tableHTML = '<table><thead><tr>';
    headers.forEach(header => {
        tableHTML += `<th>${header}</th>`;
    });
    tableHTML += '</tr></thead><tbody>';
    data.forEach(row => {
        tableHTML += '<tr>';
        row.forEach(cell => {
            tableHTML += `<td>${cell}</td>`;
        });
        tableHTML += '</tr>';
    });
    tableHTML += '</tbody></table>';

    const watermarkText = $(watermarkSelector).html() || '';
    const popupWin = window.open('', '_blank', 'top=100,width=900,height=500,location=no');
    popupWin.document.open();

    const tableStyles = `
        <style>
            table {
                width: 100%;
                border-collapse: collapse;
                margin-bottom: 20px;
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
        </style>`;

    popupWin.document.write(`
        <html>
        <head>${tableStyles}</head>
        <body onload="window.print()">
            ${tableHTML}
            <div style="transform: rotate(-45deg);z-index:10000;opacity: 0.3;
                position:fixed;left: 6%; top: 39%;font-size: 80px;
                display: grid;justify-content: center;align-content: center;">
                ${watermarkText}
            </div>
        </body>
        </html>`);
    popupWin.document.close();
}
