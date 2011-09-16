Application.Modules["InstructionViewer"].init = function () {
}

Application.Modules["InstructionViewer"].show = function (_case) {
    this.bind(_case);
    $("#instruction-detail").dialog({
        title: "批示查看",
        width: 700,
        modal: true,
        buttons: {
            "关闭": function () {
                $(this).dialog("close");
            }
        }
    });
}

Application.Modules["InstructionViewer"].bind = function (_case) {
    Application.InvokeService(
        "GetInstructionByCase",
        {
            caseId: _case.ID
        },
        function (instruction) {
            if (instruction != null) {
                $("#instruction-detail .title").html(instruction.Title);
                $("#instruction-detail .content").html(unescape(instruction.Content));
                $("#instruction-detail .issuer").html("【批示人：" + instruction.Issuer.Name);
                $("#instruction-detail .datetime").html("批示时间：" + formatJsonDate(instruction.DateTime) + "】");
                if (instruction.AttachmentFileName != null && instruction.AttachmentFileName.length > 0) {
                    $("#instruction-detail .attachment a").attr("href", Application.GetServiceUrl("GetInstructionAttachment") + "&pubId=" + instruction.ID);
                    $("#instruction-detail .attachment .filename").html(instruction.AttachmentFileName);
                    $("#instruction-detail .attachment").show();
                }
                else
                    $("#instruction-detail .attachment").hide();
            }
            else {
                $("#instruction-detail .title").html("本案件尚未批示...");
                $("#instruction-detail .content").html("");
                $("#instruction-detail .issuer").html("");
                $("#instruction-detail .datetime").html("");
                $("#instruction-detail .attachment").hide();
            }
        }
    );
}