Application.Modules["UserManager"] = function (module) {
    function init(context) {
        createDepartmentTree({
            elementSelector: "#deptree",
            onActivate: function (node) {
                createUserList(node.data.deparment);
                Application["UserManager.CurrentDepartment"] = node.data.deparment;
            }
        });
        initForm();
        bindEvents();
    }

    function setupValidation() {
        var errorContainer = $("#user-editor-form div.validation-tips");
        var errorLabelContainer = $("#user-editor-form div.validation-tips ol");
        $("#user-editor-form").validate({
            errorContainer: errorContainer,
            errorLabelContainer: errorLabelContainer,
            wrapper: "li",
            meta: "validate"
        });
    }

    function initForm() {
        setupValidation();
        Application.BindDropDownList(
            "[name=di-password-question]",
            "GetPasswordQuestions",
            null, // param-less
            "ID",
            "Content"
        );
    }

    function createUserList(depData) {
        //// Retrieve user for currently selected department and generate user list from template
        Application.InvokeService(
            "GetUsersByDepartment",
            {
                depId: depData.ID
            },
            function (result) {
                renderTemplatedItems(result, "tmpl-user-list-row", "#user-list tbody");
            }
        );
    }

    function bindEvents() {
        $("#user-manager").delegate("#add-user", "click", function () {
            clearForm("#user-editor-form");
            $("td.department-name").html(Application["UserManager.CurrentDepartment"].Name);
            $("#user-editor-form").dialog({
                title: "添加用户",
                // height: 380, // auto-stretching is preferred.
                width: 500,
                modal: true,
                buttons: {
                    "确定添加": function () {
                        if ($("#user-editor-form").valid()) { // will trigger form validation
                            var user = gatherForm();
                            addUser(user);
                            $(this).dialog("close");
                        }
                        //else // Prompting like this seems to be superfluous and annoying, given the on-site validation tips.
                        //    alert("输入数据未通过表单验证。");
                    },
                    "取消": function () {
                        $("#user-editor-form").data().validator.resetForm(); // Note how we get the validator object associated with the form DOM 
                        $(this).dialog("close");
                    }
                }
            });
        });

        $("#modify-user").click(function () {
            clearForm("#user-editor-form");
            var selectedRow = $("#user-list tr:has(input.row-select:checked)");
            if (selectedRow.length > 0) {
                var selectedUser = selectedRow.tmplItem().data;
                populateForm(selectedUser);
                $("td.department-name").html(Application["UserManager.CurrentDepartment"].Name);
                $("#user-editor-form").dialog({
                    title: "修改用户",
                    // height: 380, // auto-stretching is preferred.
                    width: 500,
                    modal: true,
                    buttons: {
                        "确定修改": function () {
                            if ($("#user-editor-form").valid()) { // will trigger form validation
                                var user = gatherForm();
                                user.ID = selectedUser.ID;
                                user.Deactivated = selectedUser.Deactivated;
                                user.ListOrder = selectedUser.ListOrder;
                                user.CreateTime = parseMSJsonDate(selectedUser.CreateTime); // TODO: Find a better solution than this!
                                modifyUser(user);
                                $(this).dialog("close");
                            }
                        },
                        "取消": function () {
                            $("#user-editor-form").data().validator.resetForm();
                            $(this).dialog("close");
                        }
                    }
                });
            }
            else
                alert("请先选择要修改的用户。");
        });

        $("#deactivate-user").click(function () {
            var selectedRow = $("#user-list tr:has(input.row-select:checked)");
            if (selectedRow.length > 0) {
                var selectedUser = selectedRow.tmplItem().data;
                deactivateUser(selectedUser);
            }
            else
                alert("请先选择要禁用的用户。");
        });

        $("#activate-user").click(function () {
            var selectedRow = $("#user-list tr:has(input.row-select:checked)");
            if (selectedRow.length > 0) {
                var selectedUser = selectedRow.tmplItem().data;
                activateUser(selectedUser);
            }
            else
                alert("请先选择要激活的用户。");
        });
    }

    function populateForm(user) {
        if (typeof (user) != "undefined" && user != null) {
            $("[name=di-login-id]").val(user.LoginId);
            $("[name=di-user-name]").val(user.Name);
            $("[name=di-password]").val(user.Password);
            $("[name=di-phone]").val(user.Phone);
            $("[name=di-email]").val(user.Email);
            if (user.PasswordQuestion != null)
                $("[name=di-password-question]").val(user.PasswordQuestion.ID);
            $("[name=di-password-answer]").val(user.PasswordAnswer);
            if (Application.IsUserInRole(user, 1))
                $("[name=di-permission-1]").attr("checked", true);
            if (Application.IsUserInRole(user, 2))
                $("[name=di-permission-2]").attr("checked", true);
            if (Application.IsUserInRole(user, 3))
                $("[name=di-permission-3]").attr("checked", true);
            if (Application.IsUserInRole(user, 4))
                $("[name=di-permission-4]").attr("checked", true);
            if (Application.IsUserInRole(user, 5))
                $("[name=di-permission-5]").attr("checked", true);
            if (Application.IsUserInRole(user, 6))
                $("[name=di-permission-6]").attr("checked", true);
        }
    }

    function gatherForm() {
        var user = {};
        user.LoginId = $("[name=di-login-id]").val();
        user.Name = $("[name=di-user-name]").val();
        user.Password = $("[name=di-password]").val();
        user.Phone = $("[name=di-phone]").val();
        user.Email = $("[name=di-email]").val();
        user.Department = { ID: Application["UserManager.CurrentDepartment"].ID };
        user.PasswordQuestion = { ID: $("[name=di-password-question]").val() };
        user.PasswordAnswer = $("[name=di-password-answer]").val();
        user.Roles = [];
        if ($("[name=di-permission-1]").is(":checked")) {
            user.Roles.push({ ID: 1 });
        }
        if ($("[name=di-permission-2]").is(":checked")) {
            user.Roles.push({ ID: 2 });
        }
        if ($("[name=di-permission-3]").is(":checked")) {
            user.Roles.push({ ID: 3 });
        }
        if ($("[name=di-permission-4]").is(":checked")) {
            user.Roles.push({ ID: 4 });
        }
        if ($("[name=di-permission-5]").is(":checked")) {
            user.Roles.push({ ID: 5 });
        }
        if ($("[name=di-permission-6]").is(":checked")) {
            user.Roles.push({ ID: 6 });
        }
        return user;
    }

    function addUser(user) {
        Application.InvokeService(
            "AddUser",
            {
                user: user
            },
            function (result) {
                createUserList(Application["UserManager.CurrentDepartment"]);
            }
        ).done(function () {
            alert("添加用户成功！");
        });
    }

    function modifyUser(user) {
        Application.InvokeService(
            "ModifyUser",
            {
                user: user
            },
            function (result) {
                createUserList(Application["UserManager.CurrentDepartment"]);
            }
        ).done(function () {
            alert("修改用户成功！");
        });
    }

    function deactivateUser(user) {
        if (user.Deactivated == true)
            alert("该用户已处于被禁用状态！");
        else
            if (confirm("请确认是否禁用用户[" + user.Name + "]？")) {
                Application.InvokeService(
                    "DeactivateUser",
                    {
                        userID: user.ID
                    },
                    function (result) {
                        createUserList(Application["UserManager.CurrentDepartment"]);
                    }
                ).done(function () {
                    alert("禁用用户成功！");
                });
            }
    }

    function activateUser(user) {
        if (user.Deactivated == false)
            alert("该用户已处于被激活状态！");
        else {
            if (confirm("请确认是否激活用户[" + user.Name + "]？")) {
                Application.InvokeService(
                    "ActivateUser",
                    {
                        userID: user.ID
                    },
                    function (result) {
                        createUserList(Application["UserManager.CurrentDepartment"]);
                    }
                ).done(function () {
                    alert("激活用户成功！");
                });
            }
        }
    }
    //// exports
    var interface = module || {};
    interface.init = init;
    return interface;
} (Application.Modules["UserManager"]);
