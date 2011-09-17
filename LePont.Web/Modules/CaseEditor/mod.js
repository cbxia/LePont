Application.Modules["CaseEditor"] = function (module) { // param module is the module object to augment
    var __method;
    var __case;

    function init() {
        __initForm();
        __bindEvents();
    }

    function open(caseId) {
        __case = null;
        __resetForm();
        if (typeof caseId != "undefined" && caseId != null) {
            __getCase(caseId);
            __method = "ModifyCase";
        }
        else
            __method = "AddCase";
        $("#layer-CaseEditor").show();
        $(Application.PRIMARY_MODULE_SELECTOR).hide();
    }

    function __getCase(caseId) {
        Application.InvokeService(
            "GetCase",
            {
                id: caseId
            },
            function (caseObj) {
                if (caseObj != null) {
                    __case = caseObj;
                    __populateForm();
                }
            }
        );
    }

    function __hide() {
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
            },
            null,
            null,
            { ID: 0, Name: "---请选择---" }

        );
        Application.BindDropDownList(
            "#di-ExternalCaseType",
            "GetCaseTypes",
            {
                domain: "EX"
            },
            null,
            null,
            { ID: 0, Name: "---请选择---" }
        );
        Application.BindDropDownList(
            "#di-PartiesRelationType",
            "GetRelationTypes",
            null,
            null,
            null,
            { ID: 0, Name: "---请选择---" }
        );
        $("#di-Registrar").html(Application.GetCurrentUser().Name);
        var d = new Date();
        $("#di-DateTime").html(formatChineseDate(d));
    }

    function __resetForm() {
        //// Reset fields.
        $("#di-Title").val(null);
        $("#di-Locality").val(null);
        $("#di-InternalCaseType").val(0);
        $("#di-ExternalCaseType").val(0);
        $("#di-PartiesRelationType").val(0);
        $("#di-Content").val(null);
        $("#di-MoneyInvolved").val(null);
        $("#di-PeopleInvolved").val(null);
        $("#di-Flag1").attr("checked", false);
        $("#di-Flag2").attr("checked", false);
        $("#di-Flag3").attr("checked", false);
        $("#di-Flag4").attr("checked", false);
        $("#di-Flag5").attr("checked", false);
        $("#di-Flag6").attr("checked", false);
        $("#di-Flag7").attr("checked", false);
        $("#di-Flag8").attr("checked", false);
        $("#case-editor-form :radio[name=Status][value=0]").attr("checked", true);
        $("#di-MediatorAdvice").val(null);
        $("#di-Instructions").val(null);
        $("#di-Progress").val(null);
        $("#di-Instructions").val(null);
        $("#di-Disposal").val(null);
        $("#di-Responsable").val(null);
        $("#di-ResponsablePhone").val(null);
        $("#case-editor-form :radio[name=IsConcluded][value=false]").attr("checked", true);
        //// reset validator
        $("#case-editor-form").data().validator.resetForm();
    }

    function __populateForm() {
        $("#di-Title").val(__case.Title);
        $("#di-Locality").val(__case.Locality);
        if (__case.InternalCaseType != null)
            $("#di-InternalCaseType").val(__case.InternalCaseType.ID);
        if (__case.ExternalCaseType != null)
            $("#di-ExternalCaseType").val(__case.ExternalCaseType.ID);
        if (__case.PartiesRelationType != null)
            $("#di-PartiesRelationType").val(__case.PartiesRelationType.ID);
        $("#di-Content").val(__case.Content);
        $("#di-MoneyInvolved").val(__case.MoneyInvolved);
        $("#di-PeopleInvolved").val(__case.PeopleInvolved);
        if (__case.Flag1 == 1)
            $("#di-Flag1").attr("checked", true);
        if (__case.Flag2 == 1)
            $("#di-Flag2").attr("checked", true);
        if (__case.Flag3 == 1)
            $("#di-Flag3").attr("checked", true);
        if (__case.Flag4 == 1)
            $("#di-Flag4").attr("checked", true);
        if (__case.Flag5 == 1)
            $("#di-Flag5").attr("checked", true);
        if (__case.Flag6 == 1)
            $("#di-Flag6").attr("checked", true);
        if (__case.Flag7 == 1)
            $("#di-Flag7").attr("checked", true);
        if (__case.Flag8 == 1)
            $("#di-Flag8").attr("checked", true);
        $("#case-editor-form :radio[name=Status][value=" + __case.Status + "]").attr("checked", true);
        $("#di-MediatorAdvice").val(__case.MediatorAdvice);
        $("#di-Instructions").val(__case.Instructions);
        $("#di-Progress").val(__case.Progress);
        $("#di-Instructions").val(__case.Instructions);
        $("#di-Disposal").val(__case.Disposal);
        $("#di-Responsable").val(__case.Responsable);
        $("#di-ResponsablePhone").val(__case.ResponsablePhone);
        $("#case-editor-form :radio[name=IsConcluded][value=" + __case.IsConcluded + "]").attr("checked", true);
    }

    function __gatherForm() {
        __case = __case || {};
        __case.Title = $("#di-Title").val();
        __case.Locality = $("#di-Locality").val();
        __case.InternalCaseType = { ID: $("#di-InternalCaseType").val() };
        __case.ExternalCaseType = { ID: $("#di-ExternalCaseType").val() };
        __case.PartiesRelationType = { ID: $("#di-PartiesRelationType").val() };
        __case.Content = $("#di-Content").val();
        __case.MoneyInvolved = $("#di-MoneyInvolved").val();
        if (typeof __case.MoneyInvolved != "number")
            __case.MoneyInvolved = null;
        __case.PeopleInvolved = $("#di-PeopleInvolved").val();
        if (typeof __case.PeopleInvolved != "number")
            __case.PeopleInvolved = null;
        __case.Flag1 = $("#di-Flag1").is(":checked");
        __case.Flag2 = $("#di-Flag2").is(":checked");
        __case.Flag3 = $("#di-Flag3").is(":checked");
        __case.Flag4 = $("#di-Flag4").is(":checked");
        __case.Flag5 = $("#di-Flag5").is(":checked");
        __case.Flag6 = $("#di-Flag6").is(":checked");
        __case.Flag7 = $("#di-Flag7").is(":checked");
        __case.Flag8 = $("#di-Flag8").is(":checked");
        __case.Status = $("#case-editor-form :radio[name=Status]:checked").val();
        __case.MediatorAdvice = $("#di-MediatorAdvice").val();
        __case.Instructions = $("#di-Instructions").val();
        __case.Progress = $("#di-Progress").val();
        __case.Instructions = $("#di-Instructions").val();
        __case.Disposal = $("#di-Disposal").val();
        __case.Responsable = $("#di-Responsable").val();
        __case.ResponsablePhone = $("#di-ResponsablePhone").val();
        __case.IsConcluded = $("#case-editor-form :radio[name=IsConcluded]:checked").val();
        // Ref: $("xxx").is(":checked"); $("xxx").attr("checked");
        // To be provided at server side:
        // Department, Registrar, DateTime, Deactivated, LastModifyTime, ConcludeDate
    }

    function __saveCase() {
        if (typeof __case.Registrar != "undefined" && __case.Registrar != null)
            __case.Registrar = { ID: __case.Registrar.ID }; // Eliminating all other fields, which are unnecessary, to ease json conversion
        if (typeof __case.Department != "undefined" && __case.Department != null)
            __case.Department = { ID: __case.Department.ID };
        __case.DateTime = parseMSJsonDate(__case.DateTime);
        __case.LastModifyTime = parseMSJsonDate(__case.LastModifyTime);
        __case.ConcludeDate = parseMSJsonDate(__case.ConcludeDate);
        var xhr =
        Application.InvokeService(
            __method,
            {
                caseObj: __case
            },
            function (result) {
                alert("保存案件成功！");
            }
        );
        return xhr;
    }

    function __bindEvents() {
        $("#return-to-list").click(function () {
            __hide();
        });

        $("#submit-entry").click(function () {
            // TODO: authorization control
            if ($("#case-editor-form").valid()) { // will trigger form validation
                __gatherForm();
                __saveCase().done(function () {
                    __hide();
                });
            }
        });
    }
    //// exports
    var interface = module || {};
    interface.init = init;
    interface.open = open;
    return interface;
} (Application.Modules["CaseEditor"]); 

