Application.Modules["Forum"] = function (module) { // param module is the module object to augment
    var __activeBlockID;
    var __activeTopicID;
    var __pageSize = 30;
    var __currentPageIndex = 1;
    var __views = { LIST_VIEW: 0, POST_VIEW: 1 };

    function init() {
        __initForm();
        __bindEvents();
        __refreshBlockSummary();
        __switchView(__views.LIST_VIEW);
    }

    function __initForm() {
        //// Set up form validation message container.
        var errorContainer = $("#forum-editor-dlg div.validation-tips");
        var errorLabelContainer = $("#forum-editor-dlg div.validation-tips ol");
        $("#forum-editor-dlg .forum-editor-form").validate({
            errorContainer: errorContainer,
            errorLabelContainer: errorLabelContainer,
            wrapper: "li",
            meta: "validate"
        });
        // CKEditors
        if (CKEDITOR.instances["di-Content"])
            delete CKEDITOR.instances["di-Content"];
        $("#di-Content").ckeditor({ width: 600 });

        if (CKEDITOR.instances["di-Response"])
            delete CKEDITOR.instances["di-Response"];
        $("#di-Response").ckeditor({ width: 700, height: 120 });
        // Buttons
        $("#forum .command-bar a").button();
        $("#forum .button").button();
    }

    function __refreshBlockSummary() {
        var asyncOp = $.Deferred();
        Application.InvokeService(
            "GetForumBlockSummary", null,
            function (result) {
                if (result != null && result.length > 0) {
                    renderTemplatedItems(result, "tmpl-forum-block-item", "#forum .blocks tbody");
                    __switchBlock(result[0]);
                }
                asyncOp.resolve();
            }
        );
        return asyncOp;
    }

    function __refreshTopicList() {
        if (typeof __activeBlockID == "number") {
            Application.InvokeService(
                "GetForumTopics",
                {
                    blockID: __activeBlockID,
                    pageSize: __pageSize,
                    pageIndex: __currentPageIndex
                },
                function (result) {
                    if (result != null && result.Data != null && result.Data.length > 0) {
                        renderTemplatedItems(result.Data, "tmpl-forum-topic-item", "#forum ul.list");
                    }
                    else {
                        $("#forum ul.list").empty();
                    }
                }
            );
        }
    }

    function __refreshResponses(topicID) {
        var asynOp = $.Deferred();
        Application.InvokeService(
            "GetForumResponses",
            {
                topicID: topicID
            },
            function (result) {
                renderTemplatedItems(result, "tmpl-forum-followup-item", "#forum div.posts div.responses");
                $("#forum div.posts div.responses .button").button();
                asynOp.resolve();
            }
        );
        return asynOp;
    }

    function __switchBlock(block) {
        $("#forum .blocks tbody tr").removeClass("current");
        $("#forum .blocks tbody tr.block_id_" + block.BlockID).addClass("current");
        $("#forum .command-bar span.current-block").html("当前版块： " + block.BlockName);
        __activeBlockID = block.BlockID;
        __refreshTopicList();
    }

    function __switchView(view) {
        if (view == __views.LIST_VIEW) {
            $("#forum table.blocks thead tr.headings").show();
            $("#forum table.blocks tbody tr").show();
            $("#forum div.command-bar").show();
            $("#forum ul.list").show();
            $("#forum div.posts").hide();
        }
        else {
            $("#forum table.blocks thead tr.headings").hide();
            $("#forum table.blocks tbody tr").hide();
            $("#forum div.command-bar").hide();
            $("#forum ul.list").hide();
            $("#forum div.posts").show();
        }
    }

    function __gatherForm() {
        var topic = {};
        topic.Title = $("#di-Title").val();
        topic.Content = escape($("#di-Content").val());
        return topic;
    }

    function __addTopic(topic) {
        var xhr =
        Application.InvokeService(
            "AddForumTopic",
            {
                post: topic
            }
        );
        return xhr;
    }

    function __deleteTopic(topicId) {
        var xhr =
        Application.InvokeService(
            "DeleteForumTopic",
            {
                postId: topicId
            }
        );
        return xhr;
    }

    function __addResponse(response) {
        var xhr =
        Application.InvokeService(
            "AddForumResponse",
            {
                post: response
            }
        );
        return xhr;
    }

    function __deleteResponse(responseId) {
        var xhr =
        Application.InvokeService(
            "DeleteForumResponse",
            {
                postId: responseId
            }
        );
        return xhr;
    }
    function __showTopicEditor() {
        clearForm("#forum-editor-dlg .forum-editor-form");
        $("#forum-editor-dlg").dialog({
            title: "发帖",
            width: 720,
            modal: true,
            buttons: {
                "确定": function () {
                    // TODO: authorization control
                    if ($("#forum-editor-dlg .forum-editor-form").valid()) { // will trigger form validation
                        var button = $(this);
                        var topic = __gatherForm();
                        topic.Block = { ID: __activeBlockID };
                        __addTopic(topic).done(function () {
                            alert("发帖成功！");
                            __refreshTopicList();
                            clearForm("#forum-editor-dlg .forum-editor-form");
                            button.dialog("close");
                        });
                    }
                    //else // Prompting like this seems to be superfluous and annoying, given the on-site validation tips.
                    //    alert("输入数据未通过表单验证。");
                },
                "取消": function () {
                    $("#forum-editor-dlg .forum-editor-form").data().validator.resetForm(); // Note how we get the validator object associated with the form DOM 
                    $(this).dialog("close");
                }
            }
        });
    }

    function __bindEvents() {
        $("#forum").delegate(".blocks .switch", "click", function () {
            __switchBlock($(this).tmplItem().data);
        });
        $("#forum").delegate(".command-bar .new-topic", "click", function () {
            __showTopicEditor();
        });
        $("#forum").delegate(".posts .response-edit .return-to-list", "click", function () {
            __switchView(__views.LIST_VIEW);
        });
        $("#forum").delegate(".posts .response-edit .delete-topic", "click", function () {
            if (confirm("请确认是否删除当前主题？")) {
                __deleteTopic(__activeTopicID);
                __refreshTopicList();
                __switchView(__views.LIST_VIEW);
                alert("删除成功！");
            }
        });
        $("#forum").delegate(".posts .responses .delete-response", "click", function () {
            if (confirm("请确认是否删除当前回帖？")) {
                var responseId = $(this).tmplItem().data.ID;
                __deleteResponse(responseId);
                __refreshResponses(__activeTopicID);
                alert("删除成功！");
            }
        });
        $("#forum").delegate(".posts .response-edit .commit", "click", function () {
            if ($("#di-Response").val().length > 0) {
                var response = {};
                response.Block = { ID: __activeBlockID };
                response.Topic = { ID: __activeTopicID };
                response.Content = escape($("#di-Response").val());
                __addResponse(response).done(function () {
                    __refreshResponses(response.Topic.ID);
                    $("#di-Response").val("");
                    alert("回帖成功！");
                });
            }
            else
                alert("请输入回帖内容！");
        });
        $("#forum").delegate("li.topic a", "click", function () {
            var topic = $(this).tmplItem().data;
            __activeTopicID = topic.ID;
            renderTemplatedItems(topic, "tmpl-forum-topic-detail", "#forum div.posts div.topic");
            __refreshResponses(topic.ID).done(function () {
                __switchView(__views.POST_VIEW);
            })
        });
    }
    //// exports
    var interface = module || {};
    interface.init = init;
    return interface;
} (Application.Modules["Forum"]); 