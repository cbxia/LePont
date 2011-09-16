var pub_type_id; // TODO: remove to namespace
Application.Modules["PublicationManager"].init = function (context) {
    pub_type_id = context;
    initForm();
    bindEvents();
    refreshList();
}

function initForm() {
    //// Set up form validation message container.
    var errorContainer = $("#publication-editor-dlg div.validation-tips");
    var errorLabelContainer = $("#publication-editor-dlg div.validation-tips ol");
    $("#publication-editor-form").validate({
        errorContainer: errorContainer,
        errorLabelContainer: errorLabelContainer,
        wrapper: "li",
        meta: "validate"
    });
    // CKEditor
    if (CKEDITOR.instances["di-Content"])
        delete CKEDITOR.instances["di-Content"];
    $("#di-Content").ckeditor({ width: 800 });
    // Initial values
    $("#di-Registrar").html(Application.GetCurrentUser().Name);
    var d = new Date();
    $("#di-DateTime").html(formatChineseDate(d));
}

function gatherForm() {
    var _publication = {};
    _publication.Title = $("#di-Title").val();
    _publication.Content = escape($("#di-Content").val());
    return _publication;
}

function addPublication(publication) {
    var xhr =
        Application.InvokeService(
            "AddPublication",
            {
                publication: publication
            }
        );
    return xhr;
}

function showEditor() {
    clearForm("#publication-editor-form");
    $("#publication-editor-dlg").dialog({
        title: "发布文章",
        // height: 380, // auto-stretching is preferred.
        width: 920,
        modal: true,
        buttons: {
            "确定": function () {
                // TODO: authorization control
                if ($("#publication-editor-form").valid()) { // will trigger form validation
                    var button = $(this);
                    ajaxFileUpload("publication-attach-form .file-uploading-anim", "di-Attachment-Pub").then(function () {
                        var _publication = gatherForm();
                        _publication.Type = { ID: pub_type_id };
                        addPublication(_publication).done(function () {
                            alert("保存信息成功！");
                            refreshList();
                            clearForm("#publication-editor-form");
                            button.dialog("close");
                        });
                    }
                        );
                }
                //else // Prompting like this seems to be superfluous and annoying, given the on-site validation tips.
                //    alert("输入数据未通过表单验证。");
            },
            "取消": function () {
                $("#publication-editor-form").data().validator.resetForm(); // Note how we get the validator object associated with the form DOM 
                $(this).dialog("close");
            }
        }
    });
}

function bindEvents() {
    $("#pub-manager").delegate("td a", "click", function () {
        var publication = $(this).parent().tmplItem().data;
        Application.EnsureLoadLayer("PublicationViewer").done(function () {
            Application.Modules["PublicationViewer"].show(publication.ID);
        });
    });

    $("#pub-manager").delegate("#add-publication", "click", function () {
        showEditor();
    })
}

function refreshList() {
    Application.InvokeService(
            "BrowserPublications",
            {
                typeId: pub_type_id,
                pageSize: 30,
                pageIndex: 1
            },
            function (result) {
                renderTemplatedItems(result, "tmpl-pub-list-items", "#pub-list tbody");
            }
        );
}
