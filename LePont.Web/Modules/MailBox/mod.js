Application.Modules["MailBox"] = function (module) { // param module is the module object to augment
    var __pageSize = 50,
        __totalPages_In,
        __totalPages_Out,
        __totalPages_Trash,
        __pageIndex_In = 1,
        __pageIndex_Out = 1,
        __pageIndex_Trash = 1;

    function init() {
        __initForm();
        __bindEvents();
        __refreshInbox();
    }

    function __initForm() {
        //// Set up form validation message container.
        var errorContainer = $("#mailbox-tabview-compose div.validation-tips");
        var errorLabelContainer = $("#mailbox-tabview-compose div.validation-tips ol");
        $("#mail-editor-form").validate({
            errorContainer: errorContainer,
            errorLabelContainer: errorLabelContainer,
            wrapper: "li",
            meta: "validate"
        });
        // CKEditor
        if (CKEDITOR.instances["di-Content"])
            delete CKEDITOR.instances["di-Content"];
        $("#di-Content").ckeditor({ width: 660 });
        // Tabview
        $("#mailbox-tabview").tabs();
        $("#mailbox-tabview").tabs("select", 0);
        // Buttons
        $("#mailbox .button").button();
    }

    function __sendMail() {
        if ($("#mail-editor-form").valid()) { // will trigger form validation
            ajaxFileUpload("#mail-attach-form .file-uploading-anim", "di-Attachment-Pub").then(function () {
                var mail = {};
                mail.Subject = $("#di-Subject").val();
                mail.Content = escape($("#di-Content").val());
                var recipient = $("#di-Recipient").val();
                Application.InvokeService(
                    "SendMessage",
                    {
                        message: mail,
                        recipient: recipient
                    }
                )
                .done(function () {
                    alert("邮件发送成功！");
                    //refreshList();
                    clearForm("#mail-editor-form");
                })
            });
        }
    }

    function __refreshInbox() {
        Application.InvokeService(
            "GetInbox",
             {
                 pageSize: __pageSize,
                 pageIndex: __pageIndex_In
             },
            function (result) {
                if (result != null && result.TotalRecords > 0) {
                    renderTemplatedItems(result.Data, "tmpl_inbox_items", "#mailbox-tabview-inbox tbody");
                }
            }
        );
    }

    function __refreshOutbox() {
        Application.InvokeService(
            "GetOutbox",
             {
                 pageSize: __pageSize,
                 pageIndex: __pageIndex_Out
             },
            function (result) {
                if (result != null && result.TotalRecords > 0) {
                    renderTemplatedItems(result.Data, "tmpl_outbox_items", "#mailbox-tabview-outbox tbody");
                }
            }
        );
    }

    function __refreshTrashcan() {
        Application.InvokeService(
            "GetTrashcan",
             {
                 pageSize: __pageSize,
                 pageIndex: __pageIndex_Trash
             },
            function (result) {
                if (result != null && result.TotalRecords > 0) {
                    renderTemplatedItems(result.Data, "tmpl_trashcan_items", "#mailbox-tabview-trashcan tbody");
                }
            }
        );
    }
    function __bindEvents() {
        //// Tab switching
        $("#mailbox-tabview").bind("tabsselect", function (event, ui) {
            if (ui.index == 0) {
                __refreshInbox();
            }
            if (ui.index == 2) {
                __refreshOutbox();
            }
            if (ui.index == 3) {
                //  __refreshTrashcan();
            }

        });

        $("#mailbox").delegate("#mailbox .command-bar .send", "click", function () {
            __sendMail();
        })
    }
    //// exports
    var interface = module || {};
    interface.init = init;
    return interface;
} (Application.Modules["MailBox"]); 