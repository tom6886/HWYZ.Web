;
$(function () {

    var product = {};

    product.openDialog = function (modal, userId) {
        $.post("productset/queryDialog", { userId: userId }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            product.bindDialog(modal);
        });
    };

    product.bindDialog = function (modal) {

        product.webUpload();

        var _form = $("form", modal);

        _form.ajaxForm({
            beforeSubmit: function () {
                return _form.valid();
            },
            success: function (r) {
                alert(r.msg);

                if (r.code > 0) {
                    modal.modal('hide');
                    $("#form_query").submit();
                }
            }
        }).validate({
            rules: {
                Account: "required",
                Name: "required",
                CardNumber: { required: true, cardnum: true }
            },
            messages: {
                Account: "用户编号是必填项",
                Name: "用户姓名是必填项",
                CardNumber: { required: "身份证号是必填项" }
            }
        });

        $('select[name=Status]', _form).val($("#Status", _form).val());

        $(".save", modal).click(function () {
            _form.submit();
        });
    }

    product.webUpload = function () {
        // 初始化Web Uploader
        var uploader = WebUploader.create({

            // 选完文件后，是否自动上传。
            auto: true,

            // swf文件路径
            swf: '~/Script/webuploader/Uploader.swf',

            // 文件接收服务端。
            server: '/productset/savePicture',

            // 选择文件的按钮。可选。
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.
            pick: '#filePicker',

            // 只允许选择图片文件。
            accept: {
                title: 'Images',
                extensions: 'gif,jpg,jpeg,bmp,png',
                mimeTypes: 'image/*'
            }
        });

        uploader.on("uploadSuccess", function (file, r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            $("#viewpic").attr("src", r.src);
            $("#ImagePath").val(r.src);
        });
    }

    product.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            product.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#basic-addon2").click(function () {
            $("#form_query").submit();
        });

    }();

});