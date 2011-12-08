Application.Modules["LogonStat"] = function (module) { // param module is the module object to augment
    function init() {
        __initForm();
        __bindEvents();
    }

    function __initForm() {
        createDepartmentTree({
            elementSelector: "#logon-stat .dep-picker ul",
            onActivate: function (node) {
                $("#ls-Department").val(node.data.deparment.Name);
                $("#ls-Department").data("depid", node.data.deparment.ID);
                $("#logon-stat .dep-picker").hide();
            },
            onClick: function (node, event) {
                if (node.getEventTargetType(event) == "title") {
                    $("#logon-stat .dep-picker").hide();
                    return true;
                }
            }
        });
        $("#logon-stat .dep-picker").hide();

        $("#ls-DateFrom").datepicker({
            changeYear: true,
            onSelect: function () {
                $("#ls-search-form").validate().element(this); // forces validation, otherwise, no immediate validation
            }
        });

        $("#ls-DateTo").datepicker({
            changeYear: true,
            onSelect: function () {
                $("#ls-search-form").validate().element(this); // forces validation, otherwise, no immediate validation
            }
        });

        var maintenant = new Date();
        var oneMonthAgo = new Date();
        oneMonthAgo.setDate(oneMonthAgo.getDate() - 90);

        $("#ls-DateFrom").datepicker('setDate', oneMonthAgo);
        $("#ls-DateTo").datepicker('setDate', maintenant);

        //// Set up form validation message container.
        var errorContainer = $("#logon-stat .validation-tips");
        var errorLabelContainer = $("#logon-stat .validation-tips ol");
        $("#ls-search-form").validate({
            errorContainer: errorContainer,
            errorLabelContainer: errorLabelContainer,
            wrapper: "li",
            meta: "validate",
            rules:
            {
            }
        });
        //// Turn anchors into jQuery-UI buttons
        $("#logon-stat .condition-bar .command-pane a").button();
    }

    function __doSearch(__depId, __dateFrom, __dateTo) {
        __dateTo.setDate(__dateTo.getDate() + 1);
        Application.InvokeService(
            "GetLogonStat",
            {
                depId: __depId,
                dateFrom: __dateFrom,
                dateTo: __dateTo
            },
            function (result) {
                if (result != null && result.length > 0) {
                    //// Show data
                    renderTemplatedItems(result, "tmpl-logon-stat-items", "#logon-stat .result tbody");
                }
                else {
                    alert("没有查到符合条件的数据！");
                    renderTemplatedItems(result, "tmpl-logon-stat-items", "#logon-stat .result tbody");
                }
            }
        );
    }

    function __bindEvents() {
        $("#ls-Department, #logon-stat .dep-pick-trig").click(function () {
            $("#logon-stat .dep-picker").show();
        });
        $("#ls-DoSearch").click(function () {
            if ($("#ls-search-form").valid()) { // will trigger form validation
                var depId = $("#ls-Department").data("depid");
                var dateFrom = $("#ls-DateFrom").datepicker('getDate');
                var dateTo = $("#ls-DateTo").datepicker('getDate');
                __doSearch(depId, dateFrom, dateTo);
            }
        });
    }

    //// exports
    var interface = module || {};
    interface.init = init;
    return interface;
} (Application.Modules["LogonStat"]); 