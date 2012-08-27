/*
This module presents another, more succint style of module definition, 
taking advantage of the in-place invoking syntax of the nameless function.
It treats the passed-in module as a reference parameter, and doesn't 
bother to return it:

!function(module) {}(module_to_augment);

Another equivalent syntax (slightly more verbose):
(function(module) {})(module_to_augment);

*/

!function (module) {
    //// exports
    module = module || {};
    module.init = init;

    function init() {
        generateLinks();
        bindEvents();
    }

    function generateLinks() {
        Application.InvokeService(
            "GetPublicationTypes",
            null, //no params
            function (result) {
                renderTemplatedItems(result, "tmpl-block-links", "#quick-links");
                $("#quick-links span:first").css("border-width", 0);
            }
        );
    }

    function bindEvents() {
        $("#quick-links").delegate("span", "click", function () {
            var pubType = $(this).tmplItem().data;
            console.log(pubType);
            if (!Application.IsModuleLoaded("PublicationManager"))
                Application.LoadModule("PublicationManager", pubType.ID);
            else
                Application.Modules["PublicationManager"].setPublicationType(pubType.ID);
        });
    }

} (Application.Modules["TopNav"]);

