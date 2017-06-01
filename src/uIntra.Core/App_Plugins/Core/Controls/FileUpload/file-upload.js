﻿let Dropzone = require("dropzone");
Dropzone.autoDiscover = false;

require('dropzone/dist/min/dropzone.min.css');
require("./file-upload.css");

let fileUploader = (function () {
    let separator = ';';

    let getUploadedFiles = function (container) {
        let val = container.val();
        if (!val) {
            return [];
        }
        return val.split(separator);
    }

    let addFile = function (container, fileId) {
        let files = getUploadedFiles(container);
        files.push(fileId);
        container.val(files.join(separator));
    }

    let removeFile = function (container, fileId) {
        container.val(container.val().replace(fileId + separator));
    }

    return {
        init: function (holder, options) {
            holder = $(holder);
            let dropzoneElem = holder.find('.js-dropzone');
            let hiddenInput = holder.find('input[type=hidden].js-new-media');

            if (!holder || !hiddenInput || !dropzoneElem) {
                throw new Error("FileUpload: Can't find elements to work with");
            }

            let allowedExtentions = dropzoneElem.data('allowed').replace(/\s/g, '') || "";
            let maxCount = dropzoneElem.data('maxCount');

            let defaultOptions = {
                url: "/Umbraco/Api/File/UploadSingle",
                maxFiles: maxCount,
                addRemoveLinks: true,
                maxFilesize: 50,
                acceptedFiles: allowedExtentions,
                dictDefaultMessage: dropzoneElem.data('defaultText'),
                dictRemoveFile: dropzoneElem.data('removeText')
            };

            let dropzoneOptions = $.extend(defaultOptions, options);
            let dropzone = new Dropzone(dropzoneElem[0], dropzoneOptions);

            //disable upload file
            let editHolder = holder.find('.js-file-edit');
            let filesElem = editHolder.find('input[type="hidden"]');
            if (filesElem.length) {
                let existedFiles = filesElem.val().replace(/\s/g, '').split(',').filter(function (s){ return s != null && s != ''});   
                if (dropzone.options.maxFiles <= existedFiles.length) {
                    dropzone.removeEventListeners();
                }    
            }            

            dropzone.on('success', function (file, fileId) {
                file.uuid = fileId;
                addFile(hiddenInput, fileId);
            });

            dropzone.on('maxfilesreached',function() {
                dropzone.removeEventListeners();
            });
            
            dropzone.on('removedfile', function (file) {
                removeFile(hiddenInput, file.uuid);
                if (dropzone.options.maxFiles > this.files.length) {
                    dropzone.setupEventListeners();
                }
            });

            return dropzone;
        }
    }
})();

let fileEditor = (function () {
    let modelInput;
    let controlHolder;

    let removeFileView = function (targetId) {
        let fileHolder = controlHolder.find('li[data-id="' + targetId + '"]');
        if (!fileHolder) {
            throw new Error("FileEdit: Can't file holder with target id: " + targetId);
        }

        fileHolder.css('display', 'none');
        fileHolder.hidden = true;
    }

    let bindRemove = function () {
        let btn = $(this);
        btn.on("click", function () {
            let targetId = btn.data('targetId');
            if (!targetId) {
                throw new Error("FileEdit: Can't find target id for element");
            }

            let modelValue = modelInput.val().replace(/\s/g, '').split(',').filter(function (s){ return s != null && s != ''});
            let newModelValue = modelValue.filter(s => s != targetId);
            modelInput.val(newModelValue.join(',') || "");
            removeFileView(targetId);

            // allow upload 
            let dropzoneElem = controlHolder.siblings('div').find('.dropzone');            
            if (dropzoneElem[0].dropzone.options.maxFiles <= modelValue.length) {
                dropzoneElem[0].dropzone.setupEventListeners();
            }  
        });
    }

    return {
        init: function (holder) {
            holder = $(holder);
            controlHolder = holder.find('.js-file-edit');
            let removeBtns = controlHolder.find('.js-remove-file-btn');
            modelInput = controlHolder.find('input[type="hidden"]');

            if (!modelInput) {
                throw new Error("FileEdit: Can't find model input");
            }

            removeBtns.each(bindRemove);
        }
    }
})();

let FileUploadController = {
    init: function (holder, fileUploaderOptions) {
        let dropzone = fileUploader.init(holder, fileUploaderOptions);
        fileEditor.init(holder);

        return dropzone;
    }
}

export default FileUploadController;