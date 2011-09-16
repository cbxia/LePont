Application.Modules["TopNav"].init = function () {
    Application.InvokeService(
            "GetPublicationTypes",
            null, //no params
            function (result) {
                renderTemplatedItems(result, "tmpl-block-links", "#quick-links");
                $("#quick-links a:first").css("border-width", 0);
            }
        );
}
