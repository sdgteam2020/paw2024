
$(document).ready(function () {
    
    pdfsizevalidation();

    const requestStartdomainId = {
        method: "POST",
        redirect: "follow"
    };



    fetchServerDate().then(function (S) {
        applyDateLogic(S.today);
    });
 
function applyDateLogic(today) {

    let selectedMode = $('input[name="mcalender_dates"]:checked').val();

    if (selectedMode === "0") {

        $('input[type="date"]').attr('min', today);
        $('input[type="date"]').removeAttr('max');

        // Set default date only if empty (Add scenario)
        if (!$('#InitiatedDate').val()) {
            $('#InitiatedDate').val(today);
        }

        if (!$('#CompletionDate').val()) {
            $('#CompletionDate').val(today);
        }

        $("#RequestRemarks")
            .prop("disabled", true)
            .css({
                "pointer-events": "none",
                "background-color": "#e9ecef",
                "opacity": "1"
            });

    } else {

        $('input[type="date"]').attr('max', today);
        $('input[type="date"]').removeAttr('min');

        $("#RequestRemarks")
            .prop("disabled", false)
            .css({
                "pointer-events": "",
                "background-color": "",
                "opacity": ""
            });

        $('#CompletionDate').attr('min', $("#InitiatedDate").val());
        $('#CompletionDate').removeAttr('max');
    }
}

fetchServerDate().then(function (S) {
    applyDateLogic(S.today);
});

$('input[name="mcalender_dates"]').on('change', function () {
    fetchServerDate().then(function (S) {
        applyDateLogic(S.today);
    });
});

$('#InitiatedDate').on('change', function () {
    let initiatedDate = $(this).val();
    $('#CompletionDate').attr('min', initiatedDate);
});


    $('input[name="mcalender_dates"]').change(function () {

        let selectedValue = $('input[name="mcalender_dates"]:checked').val();
        $.ajax({
            url: '/Projects/SetCalendarModeInSession', // controller endpoint
            type: 'POST',
            data: { mode: selectedValue },
            success: function (response) {
                console.log("Session updated:", response.message);
            },
            error: function (xhr, status, error) {
                console.error('Error saving session:', error);
            }
        });
    });


    $('.form-control').keypress(function (e) {
        let keyCode = e.which;
       
        if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (keyCode >= 48 && keyCode <= 57) || (keyCode == 32)) {
            return true; // Allow the keypress
        } else {

            if (keyCode == 46 || keyCode == 44 || keyCode == 40 || keyCode == 41 || keyCode == 45 || keyCode == 58 || keyCode == 47 || keyCode == 13 || keyCode==38)
                return true; // Allow the keypress
            else {
                
                alert('Only Alphabets and Numbers allowed');
                return false; // Block the keypress
            }

        }
    });
});
function DateFormateyyy_mm_dd(date) {

    let todaysDate = new Date();
    let datef1 = new Date(date);
    let datef2 = new Date(date);
    let months = "" + `${(datef2.getMonth() + 1)}`;
    let days = "" + `${(datef2.getDate())}`;
    let pad = "00"
    let monthsans = pad.substring(0, pad.length - months.length) + months
    let dayans = pad.substring(0, pad.length - days.length) + days
    let year = `${datef2.getFullYear()}`;
    let hh = `${datef2.getHours()}`;
    let mm = `${datef2.getMinutes()}`;
    let ss = `${datef2.getSeconds()}`;
    if (hh < 10) hh = "0" + hh;
    if (mm < 10) mm = "0" + mm;
    if (ss < 10) ss = "0" + ss;
    if (year > 1902) {

        let datemmddyyyy = year + `-` + monthsans + `-` + dayans
        return datemmddyyyy;
    }
    else {
        return '';
    }
}
function DateFormateddMMyyyyhhmmss(date) {
  
    let todaysDate = new Date();
    let datef1 = new Date(date);
    let datef2 = date ? new Date(date) : new Date();
    let months = "" + `${(datef2.getMonth() + 1)}`;
    let days = "" + `${(datef2.getDate())}`;
    let pad = "00"
    let monthsans = pad.substring(0, pad.length - months.length) + months
    let dayans = pad.substring(0, pad.length - days.length) + days
    let year = `${datef2.getFullYear()}`;
    let hh = `${datef2.getHours()}`;
    let mm = `${datef2.getMinutes()}`;
    let ss = `${datef2.getSeconds()}`;
    if (hh < 10) hh = "0" + hh;
    if (mm < 10) mm = "0" + mm;
    if (ss < 10) ss = "0" + ss;
    if (year > 1902) {

        let datemmddyyyy = dayans + `-` + monthsans + `-` + year + ` ` + hh + `:` + mm + `:` + ss
        return datemmddyyyy;
    }
    else {
        return '';
    }
}



function DateFormated(date) {
    if (date == null) {
        return '-';
    }

    let todaysDate = new Date();
    let datef1 = new Date(date);
    let datef2 = new Date(date);
    let hhmmss;
    let months = "" + `${(datef2.getMonth() + 1)}`;
    let days = "" + `${(datef2.getDate())}`;
    let pad = "00"
    let monthsans = pad.substring(0, pad.length - months.length) + months
    let dayans = pad.substring(0, pad.length - days.length) + days
    let year = `${datef2.getFullYear()}`;
    let hh = `${datef2.getHours()}`;
    let mm = `${datef2.getMinutes()}`;
    let ss = `${datef2.getSeconds()}`;
    if (hh < 10) hh = "0" + hh;
    if (mm < 10) mm = "0" + mm;
    if (ss < 10) ss = "0" + ss;

    if (hh && mm && ss != 0) {
         hhmmss = ":"+ hh + `:` + mm + `:` + ss
    }
    else {
        hhmmss = "";
    }
  
    if (year > 1902) {

        let datemmddyyyy = dayans + `-` + monthsans + `-` + year +  hhmmss
        return datemmddyyyy;
    }
    else {
        return '';
    }
}

function DateCalculateago(fmDate, end_actual_time) {
    
   
    let ago = "";
    let start_actual_time = new Date(fmDate);  // Start time
     end_actual_time = end_actual_time ? new Date(end_actual_time) : new Date();  // End time
    let diff = end_actual_time - start_actual_time;
    let diffSeconds = diff / 1000;
    let diffMinutes = diffSeconds / 60;
    let diffHours = diffMinutes / 60;
    let diffDays = diffHours / 24;
    let diffMonths = diffDays / 30;
    let diffYears = diffDays / 365;
    let HH = Math.floor(diffHours);  // Total hours
    let MM = Math.floor(diffMinutes % 60);  // Remaining minutes after calculating hours
    let formatted = (HH < 10 ? "0" + HH : HH) + ":" + (MM < 10 ? "0" + MM : MM);
     const wholedays = Math.floor(diffDays);
     const Remainderhours = diffHours - (wholedays *24);
     let Rounddays = Remainderhours >=12 ? (wholedays +1): wholedays;
    if (diffHours < 24) {
        ago = formatted + ' Min';
    } else if (diffHours < 730) {  // Less than 730 hours (~30 days)
       
        ago = Math.floor(Rounddays) + ' Days';
    } else if (diffHours < 8760) {  // Less than 8760 hours (~365 days)
        ago = Math.floor(Rounddays) + ' Days';
    } else {
        ago = Math.floor(diffYears) + ' Years'+" "+"("+Math.floor(Rounddays)+' Days)';
    }

    return ago;
}


function DateCalculateagoForChart(fmDate, end_actual_time) {
    const diffMs = new Date(end_actual_time) - new Date(fmDate);
    const totalMinutes = diffMs / 60000;
    const totalHoursDecimal = (totalMinutes / 60).toFixed(1); // 1 decimal place
    return parseFloat(totalHoursDecimal);

}
function formatDateToDDMMYYYY(date) {
    let dateObj = new Date(date);
    if (isNaN(dateObj.getTime())) {
        return ''; // Return an empty string if the date is invalid
    }
    let day = dateObj.getDate().toString().padStart(2, '0'); // Ensure 2 digits
    let month = (dateObj.getMonth() + 1).toString().padStart(2, '0'); // Ensure 2 digits
    let year = dateObj.getFullYear();
    return `${day}-${month}-${year}`;
}



function bindLiveProjectSearch(inputSelector, dropdownSelector, endpointUrl, onItemSelect) {
    

    $(inputSelector).on("keyup", function () {
        
        let query = $(this).val();
        if (query.length > 200) {

            Swal.fire({
                icon: 'error',
                title: 'Only maximaum characters limit is 200',
            });
            return $(this).val("");
        }




        if (query.length < 2) {
            $(dropdownSelector).hide();
            return;
        }

        $.ajax({
            url: endpointUrl,
            method: 'GET',
            data: { searchQuery: query },
            success: function (data) {
                $(dropdownSelector).empty();
                if (data.length > 0) {
                    data.forEach(function (item) {
                        $(dropdownSelector).append(`
                            <li class="dropdown-item" data-id="${item.projId}" data-name="${item.projName}">${item.projName}</li>
                        `);
                    });
                    $(dropdownSelector).show();
                } else {
                    $(dropdownSelector).hide();
                }
            },
            error: function (err) {
                console.error("Error fetching project data:", err);
                $(dropdownSelector).hide();
            }
        });
    });

    $(document).on("click", `${dropdownSelector} li`, function () {
        
        const projId = $(this).data("id");

        const projName = $(this).data("name");

        Swal.fire({
            title: 'Enter Remarks',
            input: 'text',
            inputLabel: `Project Name: ${projName}`,
            inputPlaceholder: 'Type your Remarks here....',
            ShowCancelButton: true,
            confirmButtonText: 'Submit',
            preConfirm: (remarks) => {
                if (!remarks) {
                    
                    Swal.ShowValidationMessage('Remarks are Required');
                } if (remarks.length < 10) {
                    Swal.showValidationMessage('Remarks Must be Atleast 10 characters');
                }
                if (remarks.length > 200) {
                    Swal.showValidationMessage('Remarks Must not exceed 200 characters');
                }
                return remarks;
            }
        }).then((result) => {
            if (result.isConfirmed) {

                const remarks = result.value;

                if (typeof onItemSelect === "function") {
                    onItemSelect(projId, projName, remarks);
                }

                $(dropdownSelector).hide();
            }

        });


    });
    $(document).on("click", function (e) {
        if (!$(e.target).closest(inputSelector).length && !$(e.target).closest(dropdownSelector).length) {
            $(dropdownSelector).hide();
        }
    });
}



function trimByWords(text, wordLimit) {
    if (!text) return "";
    let words = text.split(" ");
    if (words.length > wordLimit) {
        return words.slice(0, wordLimit).join(" ") + ".....";
    }
    return text;
}
function trimByChars(text, charLimit) {
    if (!text) return "";
    if (text.length > charLimit) {
        return text.substring(0, charLimit) + ".....";
    }
    return text;
}
function breakLinesByWords(text, wordLimit) {
    if (!text) return "";
    let words = text.split(" ");
    let result = [];

    for (let i = 0; i < words.length; i += wordLimit) {
        result.push(words.slice(i, i + wordLimit).join(" "));
    }

    return result.join("<br>");
}

function fetchServerDate() {
    
    return $.ajax({
        type: "GET",
        url: "/Projects/GetDate",
        dataType: "json",
        cache: false
    }).then(function (data) {

        const ymd = data.dateYmd;                // yyyy-MM-dd
        const dt = data.dateTimeLocal;           // yyyy-MM-ddTHH:mm:ss
        const serverDate = new Date(dt);

        if (isNaN(serverDate.getTime())) {
            throw new Error("Invalid server date");
        }

        return {
            today: ymd,
            todayDateTime: dt,   // perfect for <input type="datetime-local">
            analy: data.analy
        };
    }).catch(function (err) {

        console.error("Server date error, using client date:", err);

        const d = new Date();
        const pad = n => String(n).padStart(2, "0");
        const ymd = `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
        const dt = `${ymd}T${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;

        return {
            today: ymd,
            todayDateTime: dt,
            analy: d.toTimeString()
        };
    });
}
$(document).ready(function () {
    let token = $('input[name="__RequestVerificationToken"]').val();

    if (token) {
        $.ajaxPrefilter(function (options, originalOptions, jqXHR) {
            if (!options.skipAntiForgery) {
                jqXHR.setRequestHeader('RequestVerificationToken', token);
            }
        });
    }
});

$(document).ready(function () {
    $('.char-limit').each(function () {
       
        let inputField = $(this);

        let maxLength = parseInt(inputField.data('maxlength'));

        let errorMsg = inputField.closest('.input-container').find('.charErrorMsg');

        let countSpan = inputField.siblings('label').find('.inputcount');

        inputField.on('input', function () {

            let value = $(this).val();

            let remaining = maxLength - value.length;

            countSpan.text(" (Remaining: " + remaining + ")");

            if (value.length > maxLength) {

                $(this).val(value.substring(0, maxLength));

                errorMsg.removeClass('d-none');

            } else {

                errorMsg.addClass('d-none');
            }

        });

    });

});

function encryptData(data) {
    const cryptoKey = $('#Logincrypto').val();
   
    // 🔥 Convert object → string
    const text = JSON.stringify(data);

    return CryptoJS.AES.encrypt(text, cryptoKey).toString();
}


function encryptPayloadData(plainText) {
    const secretKey = $("#hiddenSa").val();
    if (!secretKey) return "";

    // 🔥 convert int → string
    const text = plainText.toString();

    const key = CryptoJS.enc.Utf8.parse(secretKey);
    const iv = CryptoJS.enc.Utf8.parse(secretKey.padEnd(16, '0').substring(0, 16));

    const encrypted = CryptoJS.AES.encrypt(text, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    return encrypted.toString();
}



function animateCount($element) {
   
    if ($element.data('count-started') === true) {
        return;
    }

    $element.data('count-started', true);

    var target = Number(
        String($element.attr('data-count')).replace(/,/g, '')
    );

    if (!Number.isFinite(target)) {
        return;
    }

    var duration = 1100;
    var start = performance.now();

    function update(now) {

        var progress = Math.min((now - start) / duration, 1);

        var easedProgress =
            1 - Math.pow(1 - progress, 3);

        var currentValue =
            Math.round(target * easedProgress);

        $element.text(
            new Intl.NumberFormat().format(currentValue)
        );

        if (progress < 1) {
            requestAnimationFrame(update);
        }
    }

    $element.text('0');

    requestAnimationFrame(update);
}


function getUnitIdFromSession() {
    // This works if you have set UnitId in session via backend
    // and exposed it to frontend (common pattern)

    let unitId = null;

    // Method 1: From a hidden field (most common & safe)
    unitId = $("#hdnInboxUnitId").val();

    // Method 2: From a global JS variable set in layout
    if (!unitId && typeof currentUnitId !== "undefined") {
        unitId = currentUnitId;
    }

    // Method 3: From localStorage (if you saved it after login)
    if (!unitId) {
        unitId = localStorage.getItem("UnitId");
    }

    return unitId ? parseInt(unitId) : null;
}

/**
* InputGuard: client-side input cleaning + request blocking.
* Usability layer only. The server (SanitizeActionFilter) is the real protection.
*
* - Removes ONLY the unsafe part of a value (mode "strip"), or clears the whole field (mode "clear").
* - Never trims or changes text while the user is typing, except removing the unsafe part.
* - Blocks form submit, form.submit(), $.ajax/$.post, fetch(), XMLHttpRequest if a value is unsafe.
* - Opt out with data-ig-skip on a field, a wrapper, or a form (e.g. rich-text editors).
* - Per-field mode: data-ig-mode="clear"   Global mode: InputGuard.mode = "clear"
* - Skip one request with ignoreInputGuard: true in $.ajax / fetch options.
*
* Load order: jQuery -> inputguard.js -> your page scripts.
*/
/**
 * InputGuard: client-side input cleaning + request blocking.
 * Usability layer only. The server (SanitizeActionFilter) is the real protection.
 *
 * - XSS: removes ONLY the unsafe part (mode "strip"), or clears the whole field (mode "clear").
 * - SQL: any SQL match clears the whole field (a half-removed query is never left behind).
 * - Blocks form submit, form.submit(), $.ajax/$.post, fetch(), XMLHttpRequest if a value is unsafe.
 * - Opt out with data-ig-skip on a field, a wrapper, or a form (e.g. rich-text editors, code boxes).
 * - Per-field mode: data-ig-mode="clear"   Global mode: InputGuard.mode = "clear"
 * - Skip one request with ignoreInputGuard: true in $.ajax / fetch options.
 *
 * Load order: jQuery -> inputguard.js -> your page scripts.
 */
(function (window, document, $) {
    "use strict";

    var MAX_DEPTH = 8;
    var MAX_FINDINGS = 20;
    var NOTICE_MS = 3500;
    var SQL = "SQL injection pattern";
    var SKIP_NAMES = ["__requestverificationtoken"];

    var FIELD_SELECTOR = [
        "input:not([type])",
        "input[type='text']",
        "input[type='email']",
        "input[type='search']",
        "input[type='url']",
        "input[type='tel']",
        "textarea"
    ].join(",");

    var HANDLERS = "(?:error|load|click|dblclick|mouse\\w+|key\\w+|focus\\w*|blur|change|submit|toggle|input|pointer\\w+|animation\\w+|begin|end)";

    // ---------- detection rules (same as server): [reason, pattern] ----------
    var RULES = [
        // XSS
        ["script tag", /<\s*\/?\s*script\b/i],
        ["javascript:/vbscript: URL", /\b(?:java|vb)script\s*:/i],
        ["data:text/html URL", /data\s*:\s*text\/html/i],
        ["HTML tag with event handler", /<[^>]*\bon[a-z]+\s*=/i],
        ["event handler attribute", new RegExp("(?:^|[\"'\\s/])on" + HANDLERS + "\\s*=", "i")],
        ["dangerous HTML tag", /<\s*(?:iframe|object|embed|svg|math|meta|link|base|form|style)\b/i],
        ["CSS expression()", /expression\s*\(/i],

        // SQL
        [SQL, /\bunion\b\s+(?:all\s+)?select\b/i],
        [SQL, /;\s*(?:drop|truncate|delete|insert|update|alter|create|shutdown|exec|execute|waitfor|declare)\b/i],
        [SQL, /['"]\s*(?:or|and)\s+['"(]?\w+['"]?\s*(?:=|like|<|>)\s*['"(]?\w+/i],
        [SQL, /['"]\s*(?:--|#|\/\*)/],
        [SQL, /\b(?:or|and)\s+\d+\s*=\s*\d+/i],
        [SQL, /\bwaitfor\s+delay\b/i],
        [SQL, /\bexec(?:ute)?\s*\(|\bexec(?:ute)?\s+(?:sp_|xp_)/i],
        [SQL, /\b(?:xp_cmdshell|sp_executesql|sp_oacreate|openrowset|information_schema|sysobjects|syscolumns)\b|@@version/i],
        [SQL, /\b(?:drop|truncate)\s+(?:table|database|view|procedure|proc|index)\b|\balter\s+(?:table|database)\b/i],
        [SQL, /\bselect\s+\*\s+from\b/i],
        [SQL, /\bselect\s+(?:top\s+\d+\s+)?[\w.\[\]]+(?:\s*,\s*[\w.\[\]]+)*\s+from\s+[\w.\[\]]+\s+where\b/i],
        [SQL, /\binsert\s+into\b|\bdelete\s+from\b|\bupdate\s+[\w.\[\]]+\s+set\b/i],
        [SQL, /['"]\s*\+\s*\w+\s*\+\s*['"]/],
        [SQL, /\brequest\s*(?:\.|\()\s*(?:form|querystring|params)\b/i],
        [SQL, /--\s*['"]?\s*$/]
    ];

    // ---------- removal rules (XSS only): delete just the unsafe fragment ----------
    var STRIP_RULES = [
        /<\s*script\b[^>]*>[\s\S]*?<\s*\/\s*script\s*>/gi,                      // <script>...</script> with content
        /<\s*\/?\s*script\b[^>]*>?/gi,                                          // stray script tags
        /<\s*\/?\s*(?:iframe|object|embed|svg|math|meta|link|base|form|style)\b[^>]*>?/gi,
        new RegExp("<[^>]*\\bon" + HANDLERS + "\\s*=[^>]*>?", "gi"),            // whole tag with an event handler
        new RegExp("\\bon" + HANDLERS + "\\s*=\\s*(?:\"[^\"]*\"|'[^']*'|[^\\s>]*)", "gi"), // loose onerror=... attribute
        /\b(?:java|vb)script\s*:/gi,
        /data\s*:\s*text\/html/gi,
        /expression\s*\(/gi
    ];

    // ---------- detection ----------

    var decoder = document.createElement("textarea");

    function htmlDecode(s) {
        decoder.innerHTML = s;          // textarea content is inert, nothing executes
        return decoder.value;
    }

    function variants(input) {
        var list = [input];
        var current = input;
        for (var i = 0; i < 2; i++) {
            var next = current;
            try { next = decodeURIComponent(next); } catch (e) { /* keep as is */ }
            next = htmlDecode(next).replace(/\u0000/g, "");
            if (next === current) break;
            current = next;
            list.push(current);
        }
        return list;
    }

    // Returns the reason (string) or null
    function detect(value) {
        if (typeof value !== "string" || !value.trim()) return null;
        var list = variants(value);
        for (var v = 0; v < list.length; v++) {
            for (var r = 0; r < RULES.length; r++) {
                if (RULES[r][1].test(list[v])) return RULES[r][0];
            }
        }
        return null;
    }

    // Returns true if ANY variant matches an SQL rule (even when an XSS rule matched first)
    function hasSql(value) {
        var list = variants(value);
        for (var v = 0; v < list.length; v++) {
            for (var r = 0; r < RULES.length; r++) {
                if (RULES[r][0] === SQL && RULES[r][1].test(list[v])) return true;
            }
        }
        return false;
    }

    // XSS: removes only the unsafe fragments, repeating until stable (defeats <scr<script>ipt>).
    // SQL: clears everything. Anything still unsafe afterwards is cleared too.
    function strip(value) {
        if (typeof value !== "string" || !detect(value)) return value;
        if (hasSql(value)) return "";

        var current = value;
        for (var pass = 0; pass < 5; pass++) {
            var before = current;
            for (var i = 0; i < STRIP_RULES.length; i++) {
                current = current.replace(STRIP_RULES[i], "");
            }
            if (current === before) break;
        }
        return detect(current) ? "" : current;
    }

    // Control characters and edge whitespace. Used on blur/submit only, not while typing.
    function normalize(value, multiline) {
        value = value.replace(/[\u0000-\u0008\u000B\u000C\u000E-\u001F\u007F]/g, "");
        value = multiline ? value.replace(/\r\n?/g, "\n") : value.replace(/[\r\n]+/g, " ");
        return value.trim();
    }

    // ---------- field UI ----------

    function shouldSkip($f) {
        var name = ($f.attr("name") || "").toLowerCase();
        return $f.prop("disabled") ||
            SKIP_NAMES.indexOf(name) > -1 ||
            $f.closest("[data-ig-skip]").length > 0;
    }

    function showNotice($f, text) {
        var $msg = $f.data("igMsg");
        if (!$msg) {
            $msg = $("<div class='invalid-feedback d-block ig-msg' role='alert'></div>");
            var $anchor = $f.parent().hasClass("input-group") ? $f.parent() : $f;
            $anchor.after($msg);
            $f.data("igMsg", $msg);
        }
        $msg.text(text);                                                    // .text(), never .html()
        $f.addClass("is-invalid").attr("aria-invalid", "true").data("igFlag", true);

        clearTimeout($f.data("igTimer"));
        $f.data("igTimer", setTimeout(function () { clearNotice($f); }, NOTICE_MS));
    }

    function clearNotice($f) {
        if (!$f.data("igFlag")) return;      // only clear what this script set
        clearTimeout($f.data("igTimer"));
        var $msg = $f.data("igMsg");
        if ($msg) $msg.remove();
        $f.removeData("igMsg").removeData("igFlag").removeData("igTimer")
            .removeClass("is-invalid").removeAttr("aria-invalid");
    }

    // Cleans one field in place. Returns true if something was removed.
    function cleanField($f) {
        if (shouldSkip($f)) return false;

        var el = $f[0];
        var oldValue = el.value;
        var reason = detect(oldValue);
        if (!reason) return false;

        var mode = $f.attr("data-ig-mode") || InputGuard.mode;
        var newValue = mode === "clear" ? "" : strip(oldValue);

        var pos = null;
        try { pos = el.selectionStart; } catch (e) { /* e.g. type=email has no selection API */ }

        el.value = newValue;

        if (pos !== null) {
            var np = Math.max(0, Math.min(newValue.length, pos - (oldValue.length - newValue.length)));
            try { el.setSelectionRange(np, np); } catch (e) { }
        }

        showNotice($f, newValue === ""
            ? "Unsafe input (" + reason + ") was removed."
            : "Unsafe part (" + reason + ") was removed from this field.");
        return true;
    }

    // Returns true when the form is clean. If anything was removed, returns false once
    // so the user can see the cleaned values, then the next submit goes through.
    function validateForm(formOrSelector) {
        var $form = $(formOrSelector);
        if ($form.closest("[data-ig-skip]").length) return true;

        var firstChanged = null;
        $form.find(FIELD_SELECTOR).each(function () {
            var $f = $(this);
            if (shouldSkip($f)) return;
            var cleaned = normalize($f.val(), $f.is("textarea"));
            if (cleaned !== $f.val()) $f.val(cleaned);
            if (cleanField($f) && !firstChanged) firstChanged = $f;
        });

        if (firstChanged) firstChanged.trigger("focus");
        return !firstChanged;
    }

    // ---------- scanning plain data (AJAX payloads) ----------

    function check(name, value, out) {
        if (out.length >= MAX_FINDINGS) return;
        if (SKIP_NAMES.indexOf(String(name).toLowerCase()) > -1) return;
        var reason = detect(value);
        if (reason) out.push({ field: String(name), reason: reason });
    }

    function collect(value, path, out, depth) {
        if (value == null || depth > MAX_DEPTH || out.length >= MAX_FINDINGS) return;
        if (typeof value === "string") { check(path, value, out); return; }
        if (typeof value !== "object") return;
        if (typeof File !== "undefined" && value instanceof File) return;

        if (Array.isArray(value)) {
            value.forEach(function (v, i) { collect(v, path + "[" + i + "]", out, depth + 1); });
            return;
        }
        Object.keys(value).forEach(function (k) {
            check(path + "." + k + " (name)", k, out);
            collect(value[k], path + "." + k, out, depth + 1);
        });
    }

    function scan(data, root) {
        var out = [];
        collect(data, root || "payload", out, 0);
        return out;
    }

    // Scans any request body: FormData, URLSearchParams, JSON string,
    // query-string string, or a plain object.
    function scanBody(body, contentType) {
        var out = [];
        if (!body) return out;

        if (typeof FormData !== "undefined" && body instanceof FormData) {
            body.forEach(function (v, k) {
                check(k + " (name)", k, out);
                if (typeof v === "string") check(k, v, out);
            });
        } else if (typeof URLSearchParams !== "undefined" && body instanceof URLSearchParams) {
            body.forEach(function (v, k) {
                check(k + " (name)", k, out);
                check(k, v, out);
            });
        } else if (typeof body === "string") {
            var isJson = (contentType || "").toLowerCase().indexOf("json") > -1;
            var parsed = null;
            if (isJson || /^\s*[\[{]/.test(body)) {
                try { parsed = JSON.parse(body); } catch (e) { parsed = null; }
            }
            if (parsed !== null) {
                collect(parsed, "payload", out, 0);
            } else {
                new URLSearchParams(body).forEach(function (v, k) {
                    check(k + " (name)", k, out);
                    check(k, v, out);
                });
            }
        } else if (typeof body === "object") {
            collect(body, "payload", out, 0);
        }
        return out;
    }

    // ---------- public API ----------

    var InputGuard = {
        mode: "strip",                   // "strip" = remove only unsafe XSS part, "clear" = empty the field
        detect: detect,
        strip: strip,
        scan: scan,                      // InputGuard.scan(obj) -> [{field, reason}]
        cleanField: cleanField,
        validateForm: validateForm,
        // Override to show your own toast/modal
        onBlocked: function (problems) {
            var text = problems.map(function (p) {
                return "'" + p.field + "' contains " + p.reason;
            }).join("\n");
            console.warn("InputGuard blocked a request:\n" + text);
            window.alert("Invalid input detected:\n" + text);
        }
    };
    window.InputGuard = InputGuard;

    // ---------- wiring: fields ----------

    var composing = false;               // IME typing (Hindi, Chinese, ...): wait until it finishes

    $(document).on("compositionstart", FIELD_SELECTOR, function () { composing = true; });
    $(document).on("compositionend", FIELD_SELECTOR, function () {
        composing = false;
        cleanField($(this));
    });

    // Typing and pasting: clean immediately, keep the cursor position.
    $(document).on("input", FIELD_SELECTOR, function () {
        if (composing) return;
        var $f = $(this);
        if (!cleanField($f)) clearNotice($f);
    });

    // Leaving the field: tidy whitespace/control chars, then clean.
    $(document).on("change focusout", FIELD_SELECTOR, function () {
        var $f = $(this);
        if (shouldSkip($f)) return;
        var cleaned = normalize($f.val(), $f.is("textarea"));
        if (cleaned !== $f.val()) $f.val(cleaned);
        cleanField($f);
    });

    // ---------- wiring: blocking requests ----------

    // 1) Normal submit button / Enter key / requestSubmit().
    document.addEventListener("submit", function (e) {
        if (!e.target || e.target.tagName !== "FORM") return;
        if (!validateForm(e.target)) {
            e.preventDefault();
            e.stopImmediatePropagation();
        }
    }, true);

    // 2) Programmatic form.submit() and $('#form').submit()
    var nativeSubmit = HTMLFormElement.prototype.submit;
    HTMLFormElement.prototype.submit = function () {
        if (!validateForm(this)) return;
        return nativeSubmit.apply(this, arguments);
    };

    // 3) $.ajax / $.post with a body
    $.ajaxPrefilter(function (options, original, jqXHR) {
        var method = (options.type || "GET").toUpperCase();
        if (method === "GET" || method === "HEAD" || options.ignoreInputGuard) return;

        var problems = scanBody(options.data, options.contentType);
        if (problems.length) {
            InputGuard.onBlocked(problems);
            jqXHR.abort();
        }
    });

    // 4) fetch()
    var nativeFetch = window.fetch;
    if (nativeFetch) {
        window.fetch = function (input, init) {
            init = init || {};
            var method = String(init.method || (input && input.method) || "GET").toUpperCase();
            if (method !== "GET" && method !== "HEAD" && init.body && !init.ignoreInputGuard) {
                var headers = init.headers || {};
                var ct = (typeof headers.get === "function")
                    ? headers.get("Content-Type")
                    : (headers["Content-Type"] || headers["content-type"]);
                var problems = scanBody(init.body, ct);
                if (problems.length) {
                    InputGuard.onBlocked(problems);
                    return Promise.reject(new DOMException("Blocked by InputGuard", "AbortError"));
                }
            }
            return nativeFetch.apply(this, arguments);
        };
    }

    // 5) Raw XMLHttpRequest (axios, plugins)
    var nativeOpen = XMLHttpRequest.prototype.open;
    var nativeSend = XMLHttpRequest.prototype.send;
    var nativeSetHeader = XMLHttpRequest.prototype.setRequestHeader;

    XMLHttpRequest.prototype.open = function (method) {
        this._igMethod = String(method || "GET").toUpperCase();
        this._igContentType = "";
        return nativeOpen.apply(this, arguments);
    };
    XMLHttpRequest.prototype.setRequestHeader = function (name, value) {
        if (String(name).toLowerCase() === "content-type") this._igContentType = value;
        return nativeSetHeader.apply(this, arguments);
    };
    XMLHttpRequest.prototype.send = function (body) {
        if (this._igMethod !== "GET" && this._igMethod !== "HEAD" && body) {
            var problems = scanBody(body, this._igContentType);
            if (problems.length) {
                InputGuard.onBlocked(problems);
                this.abort();
                return;
            }
        }
        return nativeSend.apply(this, arguments);
    };

})(window, document, jQuery);


function pdfsizevalidation()
{
      const pdfFileInput = document.getElementById('pdfFileInput');

        pdfFileInput.addEventListener('change', function (event) {
            debugger;
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
}
 