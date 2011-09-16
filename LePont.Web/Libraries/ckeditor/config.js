/*
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    config.skin = "kama";
    config.language = "zh-cn";
    config.uiColor = "#132C90";
    config.toolbar = [
        ['Bold', 'Italic', 'Underline', 'StrikeThrough', '-', 'Undo', 'Redo', '-', 'Cut', 'Copy', 'Paste', 'Find', 'Replace', '-', 'Outdent', 'Indent'],
        ['TextColor', 'BGColor'],
        ['NumberedList', 'BulletedList', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
        ['Table', 'Link', 'Smiley'], 
        ['Styles', 'Format', 'Font', 'FontSize'], 
        ['Source','Preview']
    ];
};
