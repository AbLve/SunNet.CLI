/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here.
    // For complete reference see:
    // http://docs.ckeditor.com/#!/api/CKEDITOR.config
    config.defaultLanguage = 'en';
    config.language = 'en';
    config.enterMode = CKEDITOR.ENTER_BR;
    config.forcePasteAsPlainText = true;
    config.height = 100;
    config.image_previewText = ' ';
    config.filebrowserImageUploadUrl = window._staticDomain_ + "Uploader/CKUploader.ashx";
    config.ImageDlgHideAdvanced = true;
  
    // The toolbar groups arrangement, optimized for a single toolbar row.
    config.toolbarGroups = [
		{ name: 'document', groups: ['mode', 'document', 'doctools'] },
		{ name: 'clipboard', groups: ['clipboard', 'undo'] },
		{ name: 'editing', groups: ['find', 'selection', 'spellchecker'] },
		{ name: 'forms' },
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
		{ name: 'links' },
		{ name: 'insert' },
		{ name: 'styles' },
		{ name: 'colors' },
		{ name: 'tools' },
		{ name: 'others' },
		{ name: 'about' }
    ];

    config.toolbar_Cli = [
                    {
                        name: 'document', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Outdent', 'Indent',
                         'JustifyLeft', 'JustifyRight', 'FontSize',
                         'NewPage', 'Undo', 'Redo', '-', 'Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord',
                         'NumberedList', 'BulletedList',
                         'TextColor', 'BGColor']
                    }
    ];
    config.toolbar_COT = [
                    {
                        name: 'document', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Outdent', 'Indent',
                         'JustifyLeft', 'JustifyRight', 'FontSize',
                         'NewPage', 'Undo', 'Redo', '-', 'Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord',
                         'NumberedList', 'BulletedList']
                    }
    ];
    config.toolbar_Community = [
    ['Styles', 'Format', 'Font', 'FontSize'],
    ['TextColor'],
    ['Bold', 'Italic', 'Underline'],
    ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
    ['NumberedList', 'BulletedList'],
    ['Image']
    ];

    // The default plugins included in the basic setup define some buttons that
    // are not needed in a basic editor. They are removed here.
    //config.removeButtons = 'Cut,Copy,Paste,Undo,Redo,Anchor,Underline,Strike,Subscript,Superscript';

    // Dialog windows are also simplified.
    config.removeDialogTabs = 'image:advanced';
};
