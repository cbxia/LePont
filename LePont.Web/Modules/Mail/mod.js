Application.Modules["Mail"] = function (module) { // param module is the module object to augment
    function init() {
        __initForm();
        __bindEvents();
    }

    function __initForm() {
    }

    function __bindEvents() {
    }
    //// exports
    var interface = module || {};
    interface.init = init;
    return interface;
} (Application.Modules["Mail"]); 