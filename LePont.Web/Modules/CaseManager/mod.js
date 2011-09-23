Application.Modules["CaseManager"] = function (module) { // param module is the module object to augment
    var __depId,
        __caseTypeId,
        __statuses,
        __dateFrom,
        __dateTo,
        __pageSize = 10,
        __totalPages,
        __pageIndex;

    function init() {
        __initForm();
        __bindEvents();
    }

    function __initForm() {
        createDepartmentTree({
            elementSelector: "#case-manager .dep-picker ul",
            onActivate: function (node) {
                $("#cm-Department").val(node.data.deparment.Name);
                $("#cm-Department").data("depid", node.data.deparment.ID);
                $("#case-manager .dep-picker").hide();
            },
            onClick: function (node, event) {
                if (node.getEventTargetType(event) == "title") {
                    $("#case-manager .dep-picker").hide();
                    return true;
                }
            }
        });
        $("#case-manager .dep-picker").hide();

        $("#cm-DateFrom").datepicker({
            changeYear: true,
            onSelect: function () {
                $("#cm-search-form").validate().element(this); // forces validation, otherwise, no immediate validation
            }
        });

        $("#cm-DateTo").datepicker({
            changeYear: true,
            onSelect: function () {
                $("#cm-search-form").validate().element(this); // forces validation, otherwise, no immediate validation
            }
        });

        var maintenant = new Date();
        var oneMonthAgo = new Date();
        oneMonthAgo.setDate(oneMonthAgo.getDate() - 90);

        $("#cm-DateFrom").datepicker('setDate', oneMonthAgo);
        $("#cm-DateTo").datepicker('setDate', maintenant);

        Application.BindDropDownList(
            "#cm-InternalCaseType",
            "GetCaseTypes",
            {
                domain: "IN"
            },
            null,
            null,
            { ID: 0, Name: "全部类别" }
        );
        //// Set up form validation message container.
        var errorContainer = $("#case-manager .validation-tips");
        var errorLabelContainer = $("#case-manager .validation-tips ol");
        $("#cm-search-form").validate({
            errorContainer: errorContainer,
            errorLabelContainer: errorLabelContainer,
            wrapper: "li",
            meta: "validate",
            rules:
            {
            }
        });
        //// Turn anchors into jQuery-UI buttons
        $("#case-manager .search-bar .command-pane a").button();
        $("#cm-Export").button({ disabled: true });
    }

    function __doSearch() {
        __dateTo.setDate(__dateTo.getDate() + 1);
        Application.InvokeService(
            "SearchCases",
            {
                depId: __depId,
                caseTypeId: __caseTypeId,
                statuses: __Statuses,
                dateFrom: __dateFrom,
                dateTo: __dateTo,
                pageSize: __pageSize,
                pageIndex: __pageIndex
            },
            function (result) {
                if (result != null && result.TotalRecords > 0) {
                    //// Show data
                    renderTemplatedItems(result.Data, "tmpl-case-list-items", "#cm-searchresults tbody");
                    //// Setup pager
                    __totalPages = Math.ceil(result.TotalRecords / __pageSize);
                    $("#case-manager .total-records").html(result.TotalRecords);
                    $("#case-manager .total-pages").html(__totalPages);
                    $("#case-manager .current-page").html(__pageIndex);
                    $("#case-manager .page-selector option").remove();
                    for (var i = 1; i <= __totalPages; i++) {
                        if (i == __pageIndex)
                            $($.format("<option selected='selected' value='%s'>%s</option>", [i, i])).appendTo($("#case-manager .page-selector"));
                        else
                            $($.format("<option value='%s'>%s</option>", [i, i])).appendTo($("#case-manager .page-selector"));
                    }
                    //// Setup export
                    $("#cm-Export").attr("href",
                        Application.GetServiceUrl(
                        "ExportCases",
                        {
                            depId: __depId,
                            caseTypeId: __caseTypeId,
                            statuses: __Statuses,
                            dateFrom: __dateFrom,
                            dateTo: __dateTo
                        })
                    );
                    $("#cm-Export").button({ disabled: false });
                }
                else {
                    alert("没有查到符合条件的数据！");
                    renderTemplatedItems(result.Data, "tmpl-case-list-items", "#cm-searchresults tbody");
                    __totalPages = 0;
                    $("#case-manager .total-records").html("0");
                    $("#case-manager .total-pages").html("0");
                    $("#case-manager .current-page").html("");
                    $("#case-manager .page-selector option").remove();
                    $("#cm-Export").attr("href", "javascript:void(0);");
                    $("#cm-Export").button({ disabled: true });
                }
            }
        );
    }

    function __bindEvents() {
        $("#cm-Department, #case-manager .dep-pick-trig").click(function () {
            $("#case-manager .dep-picker").show();
        });
        $("#cm-DoSearch").click(function () {
            if ($("#cm-search-form").valid()) { // will trigger form validation
                __depId = $("#cm-Department").data("depid");
                __Statuses = [];
                if ($("#cm-casestatus-0").is(":checked"))
                    __Statuses.push(0);
                if ($("#cm-casestatus-1").is(":checked"))
                    __Statuses.push(1);
                if ($("#cm-casestatus-2").is(":checked"))
                    __Statuses.push(2);
                if ($("#cm-casestatus-3").is(":checked"))
                    __Statuses.push(3);
                __caseTypeId = $("#cm-InternalCaseType").val();
                __dateFrom = $("#cm-DateFrom").datepicker('getDate');
                __dateTo = $("#cm-DateTo").datepicker('getDate');
                __pageIndex = 1;
                __doSearch();
            }
        });

        $("#cm-Export").click(function () {

        });

        $("#case-manager .pager-bar .prev").click(function () {
            if (__pageIndex > 1) {
                __pageIndex = __pageIndex - 1;
                __doSearch();
            }
            else {
                alert("现在已经是第一页！");
            }
        });

        $("#case-manager .pager-bar .next").click(function () {
            if (__pageIndex < __totalPages) {
                __pageIndex = __pageIndex + 1;
                __doSearch();
            }
            else {
                alert("现在已经是最后一页！");
            }
        });

        $("#case-manager .page-selector").change(function () {
            __pageIndex = parseInt($(this).val());
            __doSearch();
        });

        $("#cm-AddCase").click(function () {
            //Application.LoadModule("CaseEditor", "CaseManager");
            Application.EnsureLoadLayer("CaseEditor").done(function () {
                Application.Modules["CaseEditor"].open(null, __doSearch);
            });
        });

        //// Buttons on grid
        $("#cm-searchresults").delegate("td a.view-case", "click", function () {
            var caseObj = $(this).parent().tmplItem().data;
            Application.EnsureLoadLayer("CaseViewer").done(function () {
                Application.Modules["CaseViewer"].open(caseObj.ID);
            });
        });

        $("#cm-searchresults").delegate("td a.modi-case", "click", function () {
            var caseObj = $(this).parent().tmplItem().data;
            Application.EnsureLoadLayer("CaseEditor").done(function () {
                Application.Modules["CaseEditor"].open(caseObj.ID, __doSearch);
            });
        });

        $("#cm-searchresults").delegate("td a.dele-case", "click", function () {
            var caseObj = $(this).parent().tmplItem().data;
            if (confirm("请确认是否删除案件[" + caseObj.Title + "]？")) {
                Application.InvokeService(
                    "DeactivateCase",
                    {
                        id: caseObj.ID
                    },
                    function () {
                        __doSearch();
                    }
                ).done(function () {
                    alert("删除案件成功！");
                });
            }
        });

        $("#cm-searchresults").delegate("td a.add-inst", "click", function () {
            var caseObj = $(this).parent().tmplItem().data;
            Application.EnsureLoadLayer("InstructionEditor").done(function () {
                Application.Modules["InstructionEditor"].popupAdd(caseObj);
            });
        });


        $("#cm-searchresults").delegate("td a.view-inst", "click", function () {
            var caseObj = $(this).parent().tmplItem().data;
            Application.EnsureLoadLayer("InstructionViewer").done(function () {
                Application.Modules["InstructionViewer"].show(caseObj);
            });
        });
    }
    //// exports
    var interface = module || {};
    interface.init = init;
    interface.refresh = __doSearch;
    return interface;
} (Application.Modules["CaseManager"]); 