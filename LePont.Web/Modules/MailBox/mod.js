Application.Modules["MailBox"] = function (module) { // param module is the module object to augment
    function init() {
        __initForm();
        __bindEvents();
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

    function __bindEvents() {
        $("#mailbox").delegate("#mailbox .command-bar .send", "click", function () {
            __sendMail();
        })
    }
    //// exports
    var interface = module || {};
    interface.init = init;
    return interface;
} (Application.Modules["MailBox"]); 