function createDepartmentTree(params) {
    $(params.elementSelector).dynatree({
        minExpandLevel: 1,
        autoCollapse: true,
        clickFolderMode: 1,
        noLink: false,
        //strings: { loading: "正在加载数据...", loaderror: "加载数据出错。" },
        onActivate: params.onActivate,
        onClick: params.onClick,
        onSelect: params.onSelect,
        onLazyRead: loadChildNodes
    });
    loadRootNode(params.elementSelector).done(function () {
        var rootNode = $(params.elementSelector).dynatree("getRoot");
        var firstLevelNode = rootNode.getChildren()[0];
        firstLevelNode.expand(true);
        firstLevelNode.activate();
    });
}

function loadRootNode(elementSelector) {
    if (Application != null && Application.GetCurrentUser().IsInRole("系统管理"))
        rootDepId = 1;
    else
        rootDepId = Application.GetCurrentUser().Department.ID;
    ////
    var xhr = Application.InvokeService(
        "GetDepartment",
        {
            id: rootDepId
        },
        function (department) {
            var rootNode = $(elementSelector).dynatree("getRoot"); // Root department is the child node of the root node! (Dynatree is actually a forest!)
            rootNode.addChild({
                key: department.ID,
                title: department.Name,
                toolTip: department.Name + "[" + department.Code + "]",
                isFolder: department.Level < 3,
                isLazy: department.Level < 3,
                deparment: {
                    ID: department.ID,
                    Code: department.Code,
                    Name: department.Name,
                    Level: department.Level,
                    ListOrder: department.ListOrder
                }
            });
        }
    );
    return xhr;
}

function loadChildNodes(node) {
    Application.InvokeService(
        "GetSubDepartments",
        {
            parentId: node.data.deparment.ID
        },
        function (departments) {
            for (var i = 0; i < departments.length; i++) {
                var department = departments[i];
                node.addChild({
                    key: department.ID,
                    title: department.Name,
                    toolTip: department.Name + "[" + department.Code + "]",
                    isFolder: department.Level < 3,
                    isLazy: department.Level < 3,
                    deparment: {
                        ID: department.ID,
                        Code: department.Code,
                        Name: department.Name,
                        Level: department.Level,
                        ListOrder: department.ListOrder
                    }
                });
            }
        }
     );
}

//function createDepTreeNode(parentNode, department) {
//    var thisNode = parentNode.addChild({
//        key: department.ID,
//        title: department.Name,
//        toolTip: department.Name + "[" + department.Code + "]",
//        isFolder: department.Level < 3 || department.Subordinates.length > 0,
//        deparment: {
//            ID: department.ID,
//            Code: department.Code,
//            Name: department.Name,
//            Level: department.Level,
//            ListOrder: department.ListOrder
//        }
//    });
//    for (var i = 0; i < department.Subordinates.length; i++) {
//        createDepTreeNode(thisNode, department.Subordinates[i]);
//    }
//}