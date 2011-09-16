Application.Modules["DepartmentManager"].init = function (context) {
    createDepartmentTree({
        elementSelector: "#deptree",
        onActivate: function (node) {
            createDepEditor(node.data.deparment);
        }
    });
}

function createDepEditor(depData) {
    //// Generate tabview from template for currently selected department node
    $("#edit-area").empty();
    $("#tmpl-dep-editor-tabview").tmpl(depData).appendTo($("#edit-area"));
    $("#dep-editor-tabview").tabs();
    if (depData.Level == 3)
        $("#dep-editor-tabview").tabs("remove", 2);

    //// Data modifications events
    $("#submit-modif").click(function () {
        var department = {
            ID: depData.ID,
            Code: $("#textbox-depcode-mod").val(),
            Name: $("#textbox-depname-mod").val(),
            ListOrder: $("#textbox-listorder-mod").val()
        };
        modifyDepartment(department);
    });
    $("#submit-add").click(function () {
        var department = {
            ParentID: depData.ID,
            Code: $("#textbox-depcode-add").val(),
            Name: $("#textbox-depname-add").val(),
            ListOrder: $("#textbox-listorder-add").val()
        };
        addDepartment(department);
    });
}

function modifyDepartment(department) {
    Application.InvokeService(
        "ModifyDepartment",
        {
            id: department.ID,
            name: department.Name,
            code: department.Code,
            listOrder: department.ListOrder
        }
    ).done(
    function (result, textStatus, jqXHR) {
        alert("数据修改成功！");
    });
}

function addDepartment(department) {
    Application.InvokeService(
            "AddDepartment",
            {
                parentId: department.ParentID,
                name: department.Name,
                code: department.Code,
                listOrder: department.ListOrder
            }
        ).done(
        function (result, textStatus, jqXHR) {
            alert("添加部门成功！");
        });
}
