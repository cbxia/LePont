Application.Modules["CaseViewer"].init = function () {
    $("#case-detail").tabs();
}

Application.Modules["CaseViewer"].open = function (caseId) {
    this.bind(caseId);
    $("#case-detail").dialog({
        title: "案件详情",
        // height: 380, // auto-stretching is preferred.
        width: 800,
        modal: true,
        buttons: {
            "关闭": function () {
                $(this).dialog("close");
            }
        }
    });
}

Application.Modules["CaseViewer"].bind = function (caseId) {
    Application.InvokeService(
        "GetCase",
        {
            id: caseId
        },
        function (result) {
            if (result != null) {
                if (result.IsConcluded)
                    $("#case-detail .di-Title").html(result.Title + "[已结案]");
                else
                    $("#case-detail .di-Title").html(result.Title + "[未结案]");
                $("#case-detail .di-Locality").html(result.Locality);
                $("#case-detail .di-InternalCaseType").html(result.InternalCaseType.Name);
                $("#case-detail .di-ExternalCaseType").html(result.ExternalCaseType.Name);
                $("#case-detail .di-Content").html(result.Content);
                $("#case-detail .di-MoneyInvolved").html(result.MoneyInvolved);
                $("#case-detail .di-PeopleInvolved").html(result.PeopleInvolved);
                $("#case-detail .di-Flag1").attr("checked", result.Flag1);
                $("#case-detail .di-Flag2").attr("checked", result.Flag2);
                $("#case-detail .di-Flag3").attr("checked", result.Flag3);
                $("#case-detail .di-Flag4").attr("checked", result.Flag4);
                $("#case-detail .di-Flag5").attr("checked", result.Flag5);
                $("#case-detail .di-Flag6").attr("checked", result.Flag6);
                $("#case-detail .di-Flag7").attr("checked", result.Flag7);
                $("#case-detail .di-Flag8").attr("checked", result.Flag8);
                $("#case-detail .di-PartiesRelationType").html(result.PartiesRelationType.Name);
                $("#case-detail .di-MediatorAdvice").html(result.MediatorAdvice);
                $("#case-detail .di-Instructions").html(result.Instructions);
                $("#case-detail .di-Progress").html(result.Progress);
                $("#case-detail .di-Disposal").html(result.Disposal);
                $("#case-detail .di-Responsable").html(result.Responsable);
                $("#case-detail .di-ResponsablePhone").html(result.ResponsablePhone);
                $("#case-detail .di-Registrar").html(result.Registrar.Name);
                $("#case-detail .di-DateTime").html(result.PeopleInvolved);
            }
        }
    );
}

