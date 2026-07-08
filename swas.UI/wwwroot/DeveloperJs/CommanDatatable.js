function cleanDataTableInput(value) {
    // Allow only letters, numbers, space, underscore, comma and dot
    return value.replace(/[^a-zA-Z0-9 _.,]/g, '');
}

function escapeHtml(value) {
    if (value === null || value === undefined) return '';

    return String(value)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function initializeDataTable(tableSelector) {

    const Initialorder = (tableSelector === "#Comment") ? [[1, 'asc']] : [];

    return $(tableSelector).DataTable({
        lengthChange: true,
        dom: "<'dt-toolbar'<'dt-left'lB><'dt-right'f>>rtip",
        retrieve: true,
        destroy: true,
        pageLength: 25,
        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],

        order: Initialorder,

        rowCallback: function (row, data, displayIndex) {
            $(row).find('.ser-no')
                .html(displayIndex + 1);
        },

        initComplete: function () {
            const table = this.api();

            const wrapper = $(table.table().container());

            wrapper.find('input[type="search"]').off('input.DT_SAFE').on('input.DT_SAFE', function () {
                const cleanValue = cleanDataTableInput(this.value);

                if (this.value !== cleanValue) {
                    this.value = cleanValue;
                }

                table.search(cleanValue).draw();
            });

            wrapper.find('input[type="search"]').off('keypress.DT_SAFE').on('keypress.DT_SAFE', function (e) {
                //const char = String.fromCharCode(e.which);

                const char = String.fromCharCode(e.which || e.keyCode);

                // Allow only letters, numbers, and space
                if (!/[a-zA-Z0-9 ]/.test(char)) {
                    e.preventDefault();
                    return;
                }

                // Prevent consecutive spaces
                if (char === ' ' && e.target.value.endsWith(' ')) {
                    e.preventDefault();
                }
            });

            wrapper.find('input[type="search"]').off('paste.DT_SAFE').on('paste.DT_SAFE', function (e) {
                e.preventDefault();

                const pastedText = (e.originalEvent || e).clipboardData.getData('text');
                const cleanValue = cleanDataTableInput(pastedText);

                document.execCommand('insertText', false, cleanValue);
            });
        },

        buttons: [
            {
                extend: 'excel',
                text: 'Excel',
                exportOptions: {
                    columns: ':visible:not(:last-child)',
                    format: {
                        body: function (data, row, column, node) {
                            if (typeof data === 'string' && data.indexOf('<') >= 0) {
                                let el = $('<div>' + data + '</div>');
                                el.find('.noExport').remove();

                                let cleanText = el.text().trim();
                                cleanText = cleanDataTableInput(cleanText);

                                return column === 0 ? row + 1 : cleanText;
                            }

                            return column === 0 ? row + 1 : cleanDataTableInput(String(data ?? ''));
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
                            if (typeof data === 'string' && data.indexOf('<') >= 0) {
                                let el = $('<div>' + data + '</div>');
                                el.find('.noExport').remove();

                                let cleanText = el.text().trim();
                                cleanText = cleanDataTableInput(cleanText);

                                return column === 0 ? row + 1 : cleanText;
                            }

                            return column === 0 ? row + 1 : cleanDataTableInput(String(data ?? ''));
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
                    PdfDiv(tableSelector);
                }
            }
        ],

        searchBuilder: {
            conditions: {
                num: {
                    'MultipleOf': {
                        conditionName: 'Multiple Of',
                        init: function (that, fn, preDefined = null) {
                            let el = $('<input>').on('input', function () {
                                this.value = cleanDataTableInput(this.value);
                                fn(that, this);
                            });

                            if (preDefined !== null) {
                                $(el).val(cleanDataTableInput(preDefined[0]));
                            }

                            return el;
                        },
                        inputValue: function (el) {
                            return cleanDataTableInput($(el[0]).val());
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

function PdfDiv(tableSelector, watermarkSelector = ".IpAddress") {

    const table = $(tableSelector).DataTable();

    (tableSelector === "#SoftwareType")
        ? table.order([1, 'asc']).draw()
        : null;

    const filteredData = table.rows({ search: 'applied', order: 'applied' }).data().toArray();

    let headers = [];

    table.columns(':visible').header().each(function (header, index) {
        if (!$(header).hasClass('noExport') && index !== table.columns().count()) {
            headers.push(cleanDataTableInput($(header).text().trim()));
        }
    });

    let data = [];

    for (let i = 0; i < filteredData.length; i++) {
        let rowData = [];

        for (let j = 0; j <= filteredData[i].length - 1; j++) {

            if ($(table.column(j).header()).hasClass('noExport')) {
                continue;
            }

            let cellData = filteredData[i][j];

            if (typeof cellData === 'string' && cellData.indexOf('<') >= 0) {
                let $html = $('<div>' + cellData + '</div>');
                $html.find('.noExport').remove();

                let cleanText = $html.html()
                    .replace(/<br\s*\/?>/gi, '\n')
                    .replace(/<\/?[^>]+(>|$)/g, "")
                    .trim();

                cleanText = cleanDataTableInput(cleanText);

                rowData.push(j === 0 ? i + 1 : cleanText);
            } else {
                rowData.push(j === 0 ? i + 1 : cleanDataTableInput(String(cellData ?? '')));
            }
        }

        data.push(rowData);
    }

    let tableHTML = '<table><thead><tr>';

    headers.forEach(header => {
        tableHTML += `<th>${escapeHtml(header)}</th>`;
    });

    tableHTML += '</tr></thead><tbody>';

    data.forEach(row => {
        tableHTML += '<tr>';

        row.forEach(cell => {
            tableHTML += `<td>${escapeHtml(cell)}</td>`;
        });

        tableHTML += '</tr>';
    });

    tableHTML += '</tbody></table>';

    const watermarkText = escapeHtml($(watermarkSelector).text() || '');
  
    const popupWin = window.open('', '_blank', 'top=100,width=900,height=500,location=no');

    popupWin.document.open();

    popupWin.document.write(`
        <html>
            <head>
                <link rel="stylesheet" href="/css/print-table.css">
            </head>
            <body onload="window.print()">
                ${tableHTML}
                <div class="datatblwatermark">
                    ${watermarkText}
                </div>
            </body>
        </html>
    `);

    popupWin.document.close();
}

function refreshDataTable(tableId) {
    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).DataTable().clear().destroy();
    }
}