;
$(function () {

    var product = {};

    product.openViewDialog = function (modal, productId) {
        $.post("productset/queryViewDialog", { productId: productId }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);
        });
    };

    product.openDialog = function (modal, productId) {
        $.post("productset/queryDialog", { productId: productId }, function (r) {
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
                ProductCode: "required",
                ProductName: "required",
                Price: { required: true, number: true }
            },
            messages: {
                ProductCode: "商品编号是必填项",
                ProductName: "商品名称是必填项",
                Price: { required: "价格是必填项" }
            }
        });

        $('select[name=AllowReturn]', _form).val($("#AllowReturn", _form).val());
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
            server: '/image/savePicture',

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

            $("#viewpic").attr("src", "/Image/getInitial?docId={0}".format(r.docId));
            $("#DocId").val(r.docId);
        });
    }

    product.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            product.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#dlg_view").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            product.openViewDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#basic-addon2").click(function () {
            $("#form_query").submit();
        });

    }();

});