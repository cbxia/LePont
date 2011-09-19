Application.Modules["RecycleBin"] = function (module) { // param module is the module object to augment
    var __pageSize = 50,
        __totalPages,
        __pageIndex = 1;

    function init() {
        __bindEvents();
        __fetchData();
    }

    function __fetchData() {
        Application.InvokeService(
            "GetDeactivatedCases",
            {
                pageSize: __pageSize,
                pageIndex: __pageIndex
            },
            function (result) {
                if (result != null && result.TotalRecords > 0) {
                    renderTemplatedItems(result.Data, "tmpl-case-list-items", "#recycle-bin .item-list tbody");
                    __totalPages = Math.ceil(result.TotalRecords / __pageSize);
                    $("#recycle-bin .total-records").html(result.TotalRecords);
                    $("#recycle-bin .total-pages").html(__totalPages);
                    $("#recycle-bin .current-page").html(__pageIndex);
                    $("#recycle-bin .page-selector option").remove();
                    for (var i = 1; i <= __totalPages; i++) {
                        if (i == __pageIndex)
                            $($.format("<option selected='selected' value='%s'>%s</option>", [i, i])).appendTo($("#recycle-bin .page-selector"));
                        else
                            $($.format("<option value='%s'>%s</option>", [i, i])).appendTo($("#recycle-bin .page-selector"));
                    }
                }
                else {
                    alert("回收站中没有发现被删除数据！");
                }
            }
        );
    }

    function __bindEvents() {
        $("#recycle-bin .pager-bar .prev").click(function () {
            if (__pageIndex > 1) {
                __pageIndex = __pageIndex - 1;
                __fetchData();
            }
            else {
                alert("现在已经是第一页！");
            }
        });

        $("#recycle-bin .pager-bar .next").click(function () {
            if (__pageIndex < __totalPages) {
                __pageIndex = __pageIndex + 1;
                __fetchData();
            }
            else {
                alert("现在已经是最后一页！");
            }
        });

        $("#recycle-bin .page-selector").change(function () {
            __pageIndex = parseInt($(this).val());
            __fetchData();
        });

        $("#recycle-bin .item-list").delegate("td a.recover-case", "click", function () {
            var caseObj = $(this).parent().tmplItem().data;
            if (confirm("请确认是否恢复被删除案件[" + caseObj.Title + "]？")) {
                Application.InvokeService(
                    "ActivateCase",
                    {
                        id: caseObj.ID
                    },
                    function () {
                        __fetchData();
                    }
                ).done(function () {
                    alert("恢复数据成功！");
                });
            }
        });
    }
    //// exports
    var interface = module || {};
    interface.init = init;
    return interface;
} (Application.Modules["RecycleBin"]); 