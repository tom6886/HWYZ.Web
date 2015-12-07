;
$(function () {

    var number = {};

    number.openDialog = function (modal, id) {
        $.post("productnumber/queryDialog", { id: id }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            number.bindDialog(modal);
        });
    };

    number.bindDialog = function (modal) {
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
                ProductNumber: {
                    required: true,
                    digits: true
                }
            },
            messages: {
                ProductNumber: {
                    required: "发货数量是必填项",
                    digits: "请输入整数"
                }
            }
        });

        var product = $("._select", _form).select_2();

        if (product) {
            product.on('change', function (e) {

                var data = product.select2("data");

                if (data == null) { return false; }

                $("input[name=ProductId]", _form).val(data.id);

                $("input[name=ProductName]", _form).val(data.name);

                $("input[name=ProductCode]", _form).val(data.code);

                $("input[name=Price]", _form).val(data.price);
            });
        }

        $(".save", modal).click(function () {
            _form.submit();
        });
    }

    number.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            number.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#basic-addon2").click(function () {
            $("#form_query").submit();
        });
    }();

});