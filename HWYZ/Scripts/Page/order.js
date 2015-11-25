;
$(function () {

    var order = {};

    order.openDialog = function (modal, storeId) {
        $.post("orderdetail/queryDialog", { storeId: storeId }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            order.bindDialog(modal);
        });
    };

    order.bindDialog = function (modal) {
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
                OrderNumber: {
                    required: true,
                    digits: true
                }
            },
            messages: {
                OrderNumber: {
                    required: "发货数量是必填项",
                    digits: "请输入整数"
                }
            }
        });

        var product = $("._select", _form).select_2();

        product.on('change', function (e) {

            var data = product.select2("data");

            if (data == null) { return false; }

            $("input[name=ProductId]", _form).val(data.id);

            $("input[name=ProductName]", _form).val(data.name);

            $("input[name=ProductCode]", _form).val(data.code);

            $("input[name=Price]", _form).val(data.price);
        });

        $("input[name=OrderNumber]", _form).on('input propertychange', function () {
            var data = product.select2("data");

            if (data == null) { return false; }

            $("#pay", _form).val(data.price * $(this).val());
        });

        $(".save", modal).click(function () {

            var isNull = false;

            $("input._select", _form).each(function (i, v) {
                if (!$(v).val()) {
                    var html = '<label generated="true" class="error">{0}是必选项</label>';

                    $(v).after(html.format($(v).siblings("span").text()));

                    isNull = true;
                }
            });

            if (isNull) { return false; }

            var country = $("input[name=Country]").select2("data");

            $("input[name=CityCode]").val(country.code);

            _form.submit();
        });
    }

    order.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            order.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#basic-addon2").click(function () {
            $("#form_query").submit();
        });
    }();

});