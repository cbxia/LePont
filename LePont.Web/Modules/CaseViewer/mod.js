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

            }
        }
    );
}

