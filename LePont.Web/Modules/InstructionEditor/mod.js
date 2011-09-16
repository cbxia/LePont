Application.Modules["InstructionEditor"].init = function () {
    //// Set up form validation message container.
    var errorContainer = $("#instruction-editor-dlg div.validation-tips");
    var errorLabelContainer = $("#instruction-editor-dlg div.validation-tips ol");
    $("#instruction-editor-form").validate({
        errorContainer: errorContainer,
        errorLabelContainer: errorLabelContainer,
        wrapper: "li",
        meta: "validate"
    });
    // CKEditor
    if (CKEDITOR.instances["di-Content-InstrEd"])
        delete CKEDITOR.instances["di-Content-InstrEd"];
    $("#di-Content-InstrEd").ckeditor({ width: 650, height: 100 });
}

Application.Modules["InstructionEditor"].popupAdd = function (_case, onCompletion) {
    this.populateForm(_case);
    $("#instruction-editor-dlg").dialog({
        title: "添加批示",
        width: 800,
        modal: true,
        buttons: {
            "确定": function () {
                if ($("#instruction-editor-form").valid()) { // will trigger form validation
                    var button = $(this);
                    ajaxFileUpload("instruction-attach-form .file-uploading-anim", "di-Attachment-Instr").then(function () {
                        var instruction = Application.Modules["InstructionEditor"].gatherForm();
                        instruction.TargetCase = { ID: _case.ID };
                        Application.Modules["InstructionEditor"].add(instruction).done(function () {
                            alert("保存信息成功！");
                            if (typeof onCompletion == "function")
                                onCompletion();
                            clearForm("#instruction-editor-form");
                            button.dialog("close");
                        });
                    });
                }
            },
            "取消": function () {
                //$("#instruction-form").data().validator.resetForm(); // Note how we get the validator object associated with the form DOM 
                $(this).dialog("close");
            }
        }

    });
}

Application.Modules["InstructionEditor"].popupModify = function (instruction, onCompletion) {
    this.populateForm(instruction.TargetCase, instruction);
    $("#instruction-editor-dlg").dialog({
        title: "修改批示",
        width: 800,
        modal: true,
        buttons: {
            "确定": function () {
            },
            "取消": function () {
                //$("#instruction-form").data().validator.resetForm(); // Note how we get the validator object associated with the form DOM 
                $(this).dialog("close");
            }
        }

    });
}

Application.Modules["InstructionEditor"].populateForm = function (_case, instruction) {
    if (_case != null) {
        $("#instruction-editor-dlg .case-title").html(_case.Title);
        $("#instruction-editor-dlg .case-content").html(_case.Content);
    }
    if (instruction != null) {
        $("#instruction-editor-dlg .di-Title").val(instruction.Title);
        $("#di-Content-InstrEd").val(instruction.Content);
        $("#instruction-editor-dlg .di-Issuer").html(instruction.Issuer.Name);
        $("#instruction-editor-dlg .di-DateTime").html(instruction.DateTime);
    }
    else {
        $("#instruction-editor-dlg .di-Issuer").html(Application.GetCurrentUser().Name);
        var d = new Date();
        $("#instruction-editor-dlg .di-DateTime").html(formatChineseDate(d));
    }
}

Application.Modules["InstructionEditor"].gatherForm = function () {
    var _instruction = {};
    _instruction.Title = $("#instruction-editor-dlg .di-Title").val();
    _instruction.Content = escape($("#di-Content-InstrEd").val());
    return _instruction;
}

Application.Modules["InstructionEditor"].add = function (instruction) {
    var xhr =
            Application.InvokeService(
                "AddInstruction",
                {
                    instruction: instruction
                }
            );
    return xhr;
}
