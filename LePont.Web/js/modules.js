Application.Modules["Home"] = { path: "Modules/Home" };
Application.Modules["TopNav"] = { path: "Modules/TopNav" };
Application.Modules["UserManager"] = { path: "Modules/UserManager" };
Application.Modules["DepartmentManager"] = { path: "Modules/DepartmentManager" };
Application.Modules["DataDictionaryManager"] = { path: "Modules/ManageDataDictionary" };
Application.Modules["CaseEditor"] = { path: "Modules/CaseEditor", loaded: false };
Application.Modules["CaseViewer"] = { path: "Modules/CaseViewer", loaded: false };
Application.Modules["CaseManager"] = { path: "Modules/CaseManager" };
Application.Modules["InstructionEditor"] = { path: "Modules/InstructionEditor", loaded: false };
Application.Modules["InstructionViewer"] = { path: "Modules/InstructionViewer", loaded: false };
Application.Modules["PublicationManager"] = { path: "Modules/PublicationManager" };
Application.Modules["PublicationViewer"] = { path: "Modules/PublicationViewer", loaded: false };
Application.Modules["WorkNetwork"] = { path: "Modules/WorkNetwork" };
Application.Modules["BBS"] = { path: "Modules/BBS" };
Application.Modules["RecycleBin"] = { path: "Modules/RecycleBin" };

// Load all module script.
$(document).ready(function () {
    $.each(Application.Modules, function (index, value) {
        if (value != null && typeof value.path != "undefined")
            Application.LoadScript(value.path);
    });
});