Application.Modules["Home"].init = function (context) {
    $("#mb-1").delegate(".more", "click", function () {
        Application.LoadModule("CaseManager");
    })
    $("#mb-2").delegate(".more", "click", function () {
        Application.LoadModule("PublicationManager", 4);
    })
    $("#mb-3").delegate(".more", "click", function () {
        Application.LoadModule("PublicationManager", 1);
    })
    $("#mb-4").delegate(".more", "click", function () {
        Application.LoadModule("PublicationManager", 5);
    })

    loadCaseList();
    loadPublicationList(1, "#mb-3 tbody");
    loadPublicationList(4, "#mb-2 tbody");
    loadPublicationList(5, "#mb-4 tbody");
    loadLatestTopics();
    loadMonthlyStats();
    loadFriendlyLinks();
    //// Get Cases
    function loadCaseList() {
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
    function loadPublicationList(typeId, containerSelector) {
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

    function loadLatestTopics() {
        Application.InvokeService(
            "GetLatestPublications",
            {
                totalResults: 5
            },
            function (result) {
                if (result != null)
                    renderTemplatedItems({ Items: result }, "tmpl-latest-topics-items", "#latest-topics ol");
            }
        );
    }

    function loadMonthlyStats() {
        Application.InvokeService(
            "GetMonthlyUsageStats",null,
            function (result) {
                if (result != null)
                    renderTemplatedItems(result, "tmpl-monthly-stats-items", "#monthly-stats tbody");
            }
        );
    }

    function loadFriendlyLinks() {
        Application.InvokeService(
            "GetFriendlyLinks", null,
            function (result) {
                if (result != null)
                    renderTemplatedItems(result, "tmpl-friendly-links-items", "#friendly-links ul");
            }
        );
    }

    //// Events
    $("#mb-1").delegate("td a.open", "click", function () {
        var caseObj = $(this).parent().tmplItem().data;
        Application.EnsureLoadLayer("CaseViewer").done(function () {
            Application.Modules["CaseViewer"].open(caseObj.ID);
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

    $("#latest-topics").delegate("li a", "click", function () {
        var publication = $(this).parent().tmplItem().data;
        Application.EnsureLoadLayer("PublicationViewer").done(function () {
            Application.Modules["PublicationViewer"].show(publication.ID);
        });
    });

}
