$(document).ready(function () {

    

    function formatDate(dateStr) {
        if (!dateStr) return '';
        let d = new Date(dateStr);
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
        let table = $(tableId).DataTable();
        let data = table.rows({ search: 'applied' }).data().toArray();

        let html = '<table><thead><tr>';
        table.columns().header().each(h => html += `<th>${h.innerHTML}</th>`);
        html += '</tr></thead><tbody>';

        data.forEach(r => {
            html += '<tr>';
            r.forEach(c => html += `<td>${c}</td>`);
            html += '</tr>';
        });

        html += '</tbody></table>';

        let win = window.open('', '_blank', 'width=900,height=500');
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

    

    function genericSearch(btn, url, tableId, bodyId, fromId, toId) {
        let table;

        $(btn).on('click', async function () {
            const selected = getSelectedValues(btn);
            const from = $(fromId).val();
            const to = $(toId).val();

            if (!validateDates(from, to, toId)) return;

            try {
                const data = await fetchSearchData(url, selected, from, to);

                destroyTable(tableId);

                const html = data.map(createProjectRow).join('');
                $(bodyId).html(html);

                table = initializeDataTable(tableId);

            } catch (error) {
                console.error('Search failed:', error);
                alert('Something went wrong. Please try again.');
            }
        });
    }

    // 🔹 Helpers

    function getSelectedValues(btn) {
        const selected = [];
        $(`${btn}Data:checked`).each(function () {
            selected.push($(this).val());
        });
        return selected;
    }

    async function fetchSearchData(url, selected, from, to) {
        return $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            data: JSON.stringify({
                searchStakename: selected,
                TimeStampFrom: from,
                TimeStampTo: to
            })
        });
    }

    function escapeHtml(text) {
        return $('<div>').text(text ?? '').html();
    }

    function createProjectRow(d) {
        return `
    <tr>
        <td>${escapeHtml(d.projectId)}</td>
        <td>
            <a class="editButton"
               href="/Search/SearchProjHistory?dataProjId=${encodeURIComponent(d.projectId)}">
               ${escapeHtml(d.projectName)}
            </a>
        </td>
        <td>${escapeHtml(formatDate(d.initiatedDate))}</td>
        <td>${escapeHtml(d.stage)}</td>
        <td>${escapeHtml(d.heldWith || d.stakeholderName)}</td>
        <td>${escapeHtml(d.status)}</td>
        <td>${escapeHtml(d.action)}</td>
        <td>${escapeHtml(d.comment)}</td>
    </tr>`;
    }

    function initializeDataTable(tableId) {
        return $(tableId).DataTable({
            dom: 'lBfrtip',
            buttons: [
                'copy',
                'excel',
                'csv',
                {
                    text: 'PDF',
                    action: () => printPdf(tableId)
                }
            ]
        });
    }
    genericSearch('#searchstackButton',
        '@Url.Action("SearchstackResult","Search")',
        '#SearchStackHolderTable',
        '#searchStackHolderResults',
        '#TimeStampFromheld',
        '#TimeStampToheld');

});
