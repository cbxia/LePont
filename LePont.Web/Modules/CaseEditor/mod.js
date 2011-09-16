Application.Modules["CaseEditor"] = function (module) { // param module is the module object to augment
    function init() {
        __initForm();
        __bindEvents();
    }

    function show(caseObj) {
        $("#layer-CaseEditor").show();
        $(Application.PRIMARY_MODULE_SELECTOR).hide();
    }

    function hide() {
        $("#layer-CaseEditor").hide();
        $(Application.PRIMARY_MODULE_SELECTOR).show();
    }

    function __initForm() {
        //// Set up form validation message container.
        var errorContainer = $("#case-editor-form div.validation-tips");
        var errorLabelContainer = $("#case-editor-form div.validation-tips ol");
        $("#case-editor-form").validate({
            errorContainer: errorContainer,
            errorLabelContainer: errorLabelContainer,
            wrapper: "li",
            meta: "validate"
        });
        //// Bind selection lists
        Application.BindDropDownList(
            "#di-InternalCaseType",
            "GetCaseTypes",
            {
                domain: "IN"
            }
        );
        Application.BindDropDownList(
            "#di-ExternalCaseType",
            "GetCaseTypes",
            {
                domain: "EX"
            }
        );
        Application.BindDropDownList(
            "#di-PartiesRelationType",
            "GetRelationTypes"
        );
        $("#di-Registrar").html(Application.GetCurrentUser().Name);
        var d = new Date();
        $("#di-DateTime").html(formatChineseDate(d));
    }

    function __gatherForm() {
        var _case = {};
        _case.Title = $("#di-Title").val();
        _case.Locality = $("#di-Locality").val();
        _case.InternalCaseType = { ID: $("#di-InternalCaseType").val() };
        _case.ExternalCaseType = { ID: $("#di-ExternalCaseType").val() };
        _case.PartiesRelationType = { ID: $("#di-PartiesRelationType").val() };
        _case.Content = $("#di-Content").val();
        _case.MoneyInvolved = $("#di-MoneyInvolved").val();
        _case.PeopleInvolved = $("#di-PeopleInvolved").val();
        _case.Flag1 = $("#di-Flag1").is(":checked");
        _case.Flag2 = $("#di-Flag2").is(":checked");
        _case.Flag3 = $("#di-Flag3").is(":checked");
        _case.Flag4 = $("#di-Flag4").is(":checked");
        _case.Flag5 = $("#di-Flag5").is(":checked");
        _case.Flag6 = $("#di-Flag6").is(":checked");
        _case.Flag7 = $("#di-Flag7").is(":checked");
        _case.Flag8 = $("#di-Flag8").is(":checked");
        _case.Status = $("#case-editor-form input[name=Status]:checked").val();
        _case.MediatorAdvice = $("#di-MediatorAdvice").val();
        _case.Instructions = $("#di-Instructions").val();
        _case.Progress = $("#di-Progress").val();
        _case.Instructions = $("#di-Instructions").val();
        _case.Disposal = $("#di-Disposal").val();
        _case.Responsable = $("#di-Responsable").val();
        _case.ResponsablePhone = $("#di-ResponsablePhone").val();
        _case.IsConcluded = $("#di-IsConcluded-Y").is(":checked"); // Equivalent: $("#xxx").attr("checked")
        // To be provided at server side:
        // Department, Registrar, DateTime, Deactivated, LastModifyTime, ConcludeDate
        return _case;
    }

    function __addCase(caseObj) {
        var xhr =
        Application.InvokeService(
            "AddCase",
            {
                caseObj: caseObj
            },
            function (result) {
                alert("保存案件成功！");
            }
        );
        return xhr;
    }

    function __bindEvents() {
        $("#return-to-list").click(function () {
            hide();
        });

        $("#submit-entry").click(function () {
            // TODO: authorization control
            if ($("#case-editor-form").valid()) { // will trigger form validation
                var _case = __gatherForm();
                __addCase(_case).done(function () {
                    hide();
                });
            }
        });
    }
    //// exports
    var interface = module || {};
    interface.init = init;
    interface.show = show;
    interface.hide = hide;
    return interface;
} (Application.Modules["CaseEditor"]); 

