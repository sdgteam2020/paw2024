var table1 = $('#HeldTable').DataTable({
    lengthChange: true,
    dom: 'lBfrtip',
    pageLength: -1, // Show all entries by default
    lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
    buttons: [
        { extend: "excel", className: "buttonsToHide" },
        { extend: "pdf", className: "buttonsToHide" },
        { extend: "print", className: "buttonsToHide" }
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

var table2 = $('#WhitelistedTable').DataTable({
    lengthChange: false,
    dom: 'lBfrtip',
    buttons: [
        { extend: "excel", className: "buttonsToHide" },
        { extend: "pdf", className: "buttonsToHide" },
        { extend: "print", className: "buttonsToHide" }
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
