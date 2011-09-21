/* TODO: 
 * [1] Prompting for user confirmation before module switching, if there's unsaved data.
 *     The active module should be able to observe module loading events.  
 * [2]
 */

//////// Application context
var Application = function () {
    var Modules = {};
    var Layers = {};
    var PRIMARY_MODULE_SELECTOR = "#work-area .module";
    var LAYER_CONTAINER_SELECTOR = "#work-area .layers";
    var __allRoles, __currentUser;

    function GetCurrentUser() {
        return __currentUser;
    }

    function GetAllRoles() {
        return __allRoles;
    }

    function __loadCss(module_path) {
        var css_path = module_path + "/mod.css?no-cache";
        var cssElem = document.createElement('link');
        cssElem.setAttribute('rel', 'stylesheet');
        cssElem.setAttribute('type', 'text/css');
        cssElem.setAttribute('href', css_path);
        document.getElementsByTagName('head')[0].appendChild(cssElem);
        // Note: The following code won't work for IE 
        //$("head").append($.format("<link rel='stylesheet' type='text/css' href='%s'/>", [css_path]));
    }

    function LoadScript(module_path) {
        var script_path = module_path + "/mod.js?no-cache";
        var jsElem = document.createElement('script');
        jsElem.setAttribute('type', 'text/javascript');
        jsElem.setAttribute('src', script_path);
        document.getElementsByTagName('head')[0].appendChild(jsElem);
        // Note: Neither of the following works with firebug debuggger, though both functions correctly.
        //$("head").append($.format("<script type='text/javascript' src='%s'/>", [script_path]));
        //$.getScript(script_path);
    }

    function __clearDialogLeftovers() {
        $("body>div.ui-dialog:has(.clearable-dlg)").remove();
        $("body>.clearable-dlg").remove();
    }

    /********************** IMPORTANT **************************************************
    * Any code in the module script files (mod.js) that is dependant on module HMTL
    * must be executed after module HTML is loaded completely. An example is the
    * event handler definitions. Code that is called, directly or indirectly, by 
    * the init method can naturally meet this requirement.
    ***********************************************************************************/

    ///////// Module loader
    function LoadModule(moduleName, context, containerSelector) {
        var successful = false;
        if (moduleName != null) {
            if (containerSelector == null)
                containerSelector = PRIMARY_MODULE_SELECTOR;
            if (typeof $(containerSelector).data("moduleName") == "undefined" || $(containerSelector).data("moduleName") != moduleName) {
                var module_path = Modules[moduleName].path;
                // Unload css of existing module.
                var current_css_path = $(containerSelector).data("cssPath");
                if (typeof current_css_path != "undefined") {
                    $($.format("head link[href='%s']", [current_css_path])).remove();
                }
                // Load Css
                __loadCss(module_path);
                $(containerSelector).data("cssPath", module_path + "/mod.css?no-cache");
                // Load Html
                var html_path = module_path + "/mod.htm?no-cache";
                $(containerSelector).load(html_path, function (responseText, textStatus) {
                    if (textStatus == "success") {
                        __clearDialogLeftovers();
                        if (typeof Modules[moduleName].init != "undefined") {
                            Modules[moduleName].init(context);
                        }
                    }
                    else
                        alert("加载模块[" + moduleName + "]失败，请联系系统管理员。");
                });
                $(containerSelector).data("moduleName", moduleName);
                successful = true;
            }
        }
        return successful;
    }

    function EnsureLoadLayer(moduleName, context) {
        var asyncResult = $.Deferred();
        if (moduleName != null) {
            if (typeof Modules[moduleName].loaded == "undefined" || !Modules[moduleName].loaded) {
                var module_path = Modules[moduleName].path;
                __loadCss(module_path);
                $(LAYER_CONTAINER_SELECTOR).append("<div id='layer-" + moduleName + "' class='layer'></div>");
                var layer = $("#layer-" + moduleName);
                var html_path = module_path + "/mod.htm?no-cache";
                layer.load(html_path, function (responseText, textStatus) {
                    if (textStatus == "success") {
                        Modules[moduleName].loaded = true;
                        if (typeof Modules[moduleName].init != "undefined")
                            Modules[moduleName].init(context);
                        asyncResult.resolve();
                    }
                    else {
                        alert("加载模块[" + moduleName + "]失败，请联系系统管理员。");
                        asyncResult.reject();
                    }
                });
            }
            else
                asyncResult.resolve();
        }
        else
            asyncResult.reject();
        return asyncResult.promise();
    }

    //////// Service-related
    function GetServiceUrl(method, params) {
        var serviceUrl = $(location).attr('pathname') + "?invokemode=service&method=" + method;
        if (params != null) {
            $.each(params, function (index, value) {
                serviceUrl += "&" + index + "=" + JSON.stringify(value);
            });
        }
        return serviceUrl;
    }

    function InvokeService(method, params, onSuccess, onFailure) {
        if (params != null) {
            $.each(params, function (index, value) {
                params[index] = JSON.stringify(value);

            });
        }
        if (typeof onFailure == "undefined" || onFailure == null)
            onFailure = __promptGenericAJAXError;
        var xhr = $.ajax({
            url: GetServiceUrl(method),
            data: params,
            type: "POST",
            dataType: "json",
            success: onSuccess,
            error: onFailure
        });
        return xhr;
    }

    function __promptGenericAJAXError(jqXHR, textStatus, errorThrown) {
        window.alert("后台服务调用失败，请联系系统管理员。错误消息： " + errorThrown);
    }

    function BindDropDownList(elementSelector, method, params, idField, nameField, blankItem) {
        var idField = idField || "ID";
        var nameField = nameField || "Name";
        if (typeof blankItem != "undefined" && blankItem != null)
            $($.format("<option value='%s'>%s</option>", [blankItem[idField], blankItem[nameField]])).appendTo(elementSelector);
        InvokeService(
            method,
            params,
            function (items) {
                $.each(items, function (i, v) {
                    $($.format("<option value='%s'>%s</option>", [v[idField], v[nameField]])).appendTo(elementSelector);
                })
            }
        );
    }

    function IsUserInRole(user, role) {
        var result = false;
        for (var i = 0; i < user.Roles.length; i++) {
            var r = user.Roles[i];
            if (typeof (role) == "number") {
                if (r.ID == role) {
                    result = true;
                    break;
                }
            }
            else {// (typeof (role) == "string") 
                if (r.Name == role) {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }

    function Load() {
        var xhr = InvokeService(
            "GetAppCtx",
            null, //param-less
            function (result, textStatus, jqXHR) {
                __allRoles = result.AllRoles;
                __currentUser = result.CurrentUser;
                __currentUser.IsInRole = function (role) {
                    return IsUserInRole(this, role);
                };
            },
            function (xhr, ajaxOptions, thrownError) {
                alert("程序初始化错误。[" + thrownError + "]");
            }
        );
        return xhr;
    }

    ///// exports /////
    return {
        //// properties
        Modules: Modules,
        Layers: Layers,
        GetAllRoles: GetAllRoles,
        GetCurrentUser: GetCurrentUser,
        //// constants
        PRIMARY_MODULE_SELECTOR: PRIMARY_MODULE_SELECTOR,
        //// methods
        Load: Load,
        LoadModule: LoadModule,
        LoadScript: LoadScript,
        EnsureLoadLayer: EnsureLoadLayer,
        GetServiceUrl: GetServiceUrl,
        InvokeService: InvokeService,
        IsUserInRole: IsUserInRole,
        BindDropDownList: BindDropDownList
    }
} ();
