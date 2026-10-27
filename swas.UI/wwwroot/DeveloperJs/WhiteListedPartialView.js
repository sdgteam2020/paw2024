


function GetwhilteListProject(TypeId) {
    const encryptedPayload = encryptData({ TypeId: TypeId });

    $.ajax({
        type: "POST",
        url: "/Home/GetWhiteListedActionProj",
        dataType: "json",
        data: { encrypted_payload: encryptedPayload },

        success: function (data) {
            const items = Array.isArray(data)
                ? data
                : Array.isArray(data?.data)
                    ? data.data
                    : Object.values(data || {});

            const escapeHtml = function (value) {
                return $("<div>").text(value == null ? "" : String(value)).html();
            };

            const truncate = function (value, length) {
                value = value == null ? "" : String(value);
                return value.length > length ? value.slice(0, length) + "...." : value;
            };

            const formatDate = function (value) {
                if (!value) return "";

                const date = new Date(value);
                return Number.isNaN(date.getTime()) ? "" : escapeHtml(DateFormated(date));
            };

            let rows = "";

            items.forEach(function (item, index) {
                const projectName = item.projName || "";
                const remarks = item.remarks || "";

                let validUpto = "";
                if (item.clearence) {
                    const clearanceDate = new Date(item.clearence);

                    if (!Number.isNaN(clearanceDate.getTime())) {
                        clearanceDate.setFullYear(clearanceDate.getFullYear() + 3);
                        validUpto = formatDate(clearanceDate);
                    }
                }

                rows += `
                    <tr>
                        <td class="s-no-column">${index + 1}</td>
                        <td class="RefLetter-container WhitelistedTable-width8">
                            <div class="noExport">${escapeHtml(truncate(projectName, 10))}</div>
                            <div class="RefLetter">${escapeHtml(projectName)}</div>
                        </td>
                        <td class="WhitelistedTable-width8">${escapeHtml(item.hostedOn)}</td>
                        <td>${escapeHtml(item.appt)}</td>
                        <td>${escapeHtml(item.fmn)}</td>
                        <td class="col-width6">${escapeHtml(item.contactNo)}</td>
                        <td class="WhitelistedTable-width6">${formatDate(item.clearence)}</td>
                        <td class="WhitelistedTable-width15">${escapeHtml(item.certNo)}</td>
                        <td class="col-width6">${validUpto}</td>
                        <td class="RefLetter-container">
                            ${escapeHtml(truncate(remarks, 10))}
                            ${remarks ? `<div class="RefLetter">${escapeHtml(remarks)}</div>` : ""}
                        </td>
                    </tr>`;
            });

            if ($.fn.DataTable.isDataTable("#WhitelistedTable")) {
                $("#WhitelistedTable").DataTable().clear().destroy();
            }

            $("#WhitelistedTableData").html(rows);
            initializeDataTable("#WhitelistedTable");
        },

        error: function (xhr, status, error) {
            console.error("Error loading white list projects:", error);
        }
    });
}
