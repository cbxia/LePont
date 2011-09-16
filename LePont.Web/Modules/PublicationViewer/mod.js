Application.Modules["PublicationViewer"] = function (module) { // param module is the module object to augment
    function init() {
    }

    function bind(pubId) {
        Application.InvokeService(
            "GetPublication",
            {
                id: pubId
            },
            function (publication) {
                if (publication != null) {
                    $("#publication-detail .title").html(publication.Title);
                    $("#publication-detail .content").html(unescape(publication.Content));
                    $("#publication-detail .department").html("部门：" + publication.Department.Name);
                    $("#publication-detail .publisher").html("发布人：" + publication.Publisher.Name);
                    $("#publication-detail .publish-time").html("发布时间：" + formatJsonDate(publication.DateTime));
                    if (publication.AttachmentFileName != null && publication.AttachmentFileName.length > 0) {
                        $("#publication-detail .attachment a").attr("href", Application.GetServiceUrl("GetPublicationAttachment") + "&pubId=" + publication.ID);
                        $("#publication-detail .attachment .filename").html(publication.AttachmentFileName);
                        $("#publication-detail .attachment").show();
                    }
                    else
                        $("#publication-detail .attachment").hide();
                }
            }
        );
    }

    function show(pubId) {
        bind(pubId);
        $("#publication-detail").dialog({
            title: "文章查看",
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

    //// exports
    var interface = module || {};
    interface.init = init;
    interface.show = show;
    return interface;
}(Application.Modules["PublicationViewer"]); 

