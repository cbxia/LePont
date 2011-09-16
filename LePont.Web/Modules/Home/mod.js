Application.Modules["Home"].init = function (context) {
    $("#mb-1").delegate(".more", "click", function () {
        Application.LoadModule("CaseManager");
    })
    $("#mb-2").delegate(".more", "click", function () {
        Application.LoadModule("PublicationManager", 5);
    })
    $("#mb-3").delegate(".more", "click", function () {
        Application.LoadModule("PublicationManager", 1);
    })
    $("#mb-4").delegate(".more", "click", function () {
        Application.LoadModule("PublicationManager", 6);
    })

    reloadCaseList();
    reloadPublicationList(1, "#mb-3 tbody");
    reloadPublicationList(5, "#mb-2 tbody");
    reloadPublicationList(6, "#mb-4 tbody");
    //// Get Cases
    function reloadCaseList() {
        Application.InvokeService(
            "BrowseCases",
            {
                pageSize: 5,
                pageIndex: 1
            },
            function (result) {
                if (result != null)
                    renderTemplatedItems(result, "tmpl-case-list-items", "#mb-1 tbody");
            }
        );
    }

    //// Get Lists
    function reloadPublicationList(typeId, containerSelector) {
        Application.InvokeService(
                "BrowserPublications",
                {
                    typeId: typeId,
                    pageSize: 5,
                    pageIndex: 1
                },
                function (result) {
                    if (result != null)
                        renderTemplatedItems(result, "tmpl-pub-list-items", containerSelector);
                }
            );
    }

    //// Events
    $("#mb-1").delegate("td a.open", "click", function () {
        var caseObj = $(this).parent().tmplItem().data;
        Application.EnsureLoadLayer("CaseViewer").done(function () {
            Application.Modules["CaseViewer"].show(caseObj.ID);
        });
    });

    $("#mb-1").delegate("td a.instruct", "click", function () {
        var caseObj = $(this).parent().tmplItem().data;
        Application.EnsureLoadLayer("InstructionViewer").done(function () {
            Application.Modules["InstructionViewer"].show(caseObj);
        });
    });

    $(".pub-block").delegate("td a.open", "click", function () {
        var publication = $(this).parent().tmplItem().data;
        Application.EnsureLoadLayer("PublicationViewer").done(function () {
            Application.Modules["PublicationViewer"].show(publication.ID);
        });
    });

}
