$(document).ready(function () {

    /* ================= COMMON HELPERS ================= */

    function formatDate(dateStr) {
        if (!dateStr) return '';
        var d = new Date(dateStr);
        if (isNaN(d)) return '';
        return ('0' + d.getDate()).slice(-2) + '/'
            + ('0' + (d.getMonth() + 1)).slice(-2) + '/'
            + d.getFullYear();
    }

    function validateDates(from, to, toSelector) {
        if (from && to && from > to) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'To Date must be greater than From Date',
                confirmButtonColor: '#d33'
            });
            $(toSelector).val('');
            return false;
        }
        return true;
    }

    function destroyTable(dt) {
        if ($.fn.DataTable.isDataTable(dt)) {
            dt.DataTable().clear().destroy();
        }
    }

    function printPdf(tableId) {
        var table = $(tableId).DataTable();
        var data = table.rows({ search: 'applied' }).data().toArray();

        var html = '<table><thead><tr>';
        table.columns().header().each(h => html += `<th>${h.innerHTML}</th>`);
        html += '</tr></thead><tbody>';

        data.forEach(r => {
            html += '<tr>';
            r.forEach(c => html += `<td>${c}</td>`);
            html += '</tr>';
        });

        html += '</tbody></table>';

        var win = window.open('', '_blank', 'width=900,height=500');
        win.document.write(`
            <html><head>
            <style>
                table{width:100%;border-collapse:collapse}
                th,td{border:1px solid #ccc;padding:6px;text-align:center}
                th{background:#f2f2f2}
            </style>
            </head>
            <body onload="window.print()">
            ${html}
            <div>
                @(TempData["ipadd"])
            </div>
            </body></html>
        `);
        win.document.close();
    }

    /* ================= PROJECT NAME SEARCH ================= */

    let projTable;

    $('#SearchProjName').click(function () {

        let from = $('#TimeStampFromProj').val();
        let to = $('#TimeStampToProj').val();
        if (!validateDates(from, to, '#TimeStampToProj')) return;

        $.post('@Url.Action("SearchResults","Search")', {
            SearchText: $('#searchText').val(),
            TimeStampFrom: from,
            TimeStampTo: to,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        }, function (data) {

            destroyTable('#SearchProjTable');

            if (!data || data.length === 0) {
                Swal.fire('Data Not Found', '', 'error');
                return;
            }

            let rows = '';
            data.forEach(d => {
                rows += `
                <tr>
                    <td>${d.projectId}</td>
                    <td>
                        <a class="editButton"
                           href="/Search/SearchProjHistory?dataProjId=${d.projectId}">
                           ${d.projectName}
                        </a>
                    </td>
                    <td>${formatDate(d.initiatedDate)}</td>
                    <td>${d.stage}</td>
                    <td>${d.heldWith}</td>
                    <td>${d.status}</td>
                    <td>${d.action}</td>
                    <td>${d.comment}</td>
                </tr>`;
            });

            $('#searchtableProj').html(rows);

            projTable = $('#SearchProjTable').DataTable({
                dom: 'lBfrtip',
                buttons: ['copy', 'excel', 'csv', {
                    text: 'PDF',
                    action: () => printPdf('#SearchProjTable')
                }]
            });
        });
    });

    /* ================= STACK / STAGE / STATUS / ACTION / INITIATED ================= */

    function genericSearch(btn, url, tableId, bodyId, fromId, toId) {
        let table;

        $(btn).click(function () {

            let selected = [];
            $(`${btn}Data:checked`).each(function () {
                selected.push($(this).val());
            });

            let from = $(fromId).val();
            let to = $(toId).val();
            if (!validateDates(from, to, toId)) return;

            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json",
                data: JSON.stringify({
                    searchStakename: selected,
                    TimeStampFrom: from,
                    TimeStampTo: to
                }),
                success: function (data) {

                    destroyTable(tableId);

                    let html = '';
                    data.forEach(d => {
                        html += `
                        <tr>
                            <td>${d.projectId}</td>
                            <td>
                              <a class="editButton"
                                 href="/Search/SearchProjHistory?dataProjId=${d.projectId}">
                                 ${d.projectName}
                              </a>
                            </td>
                            <td>${formatDate(d.initiatedDate)}</td>
                            <td>${d.stage}</td>
                            <td>${d.heldWith || d.stakeholderName}</td>
                            <td>${d.status}</td>
                            <td>${d.action}</td>
                            <td>${d.comment}</td>
                        </tr>`;
                    });

                    $(bodyId).html(html);

                    table = $(tableId).DataTable({
                        dom: 'lBfrtip',
                        buttons: ['copy', 'excel', 'csv', {
                            text: 'PDF',
                            action: () => printPdf(tableId)
                        }]
                    });
                }
            });
        });
    }

    /* Calls (IDs SAME) */
    genericSearch('#searchstackButton',
        '@Url.Action("SearchstackResult","Search")',
        '#SearchStackHolderTable',
        '#searchStackHolderResults',
        '#TimeStampFromheld',
        '#TimeStampToheld');

});
