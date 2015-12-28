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
                ProductNumber: { required: true, digits: true },
                OnlinePrice: { required: true, number: true },
                OfflinePrice: { required: true, number: true },
            },
            messages: {
                ProductNumber: { required: "发货数量是必填项", digits: "请输入整数" },
                OnlinePrice: { required: "线上售价是必填项" },
                OfflinePrice: { required: "线下售价是必填项" },
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

        $("input[name=OnlinePrice]", _form).on('input propertychange', function () {
            $("input[name=OfflinePrice]", _form).val($(this).val());
        });

        $(".save", modal).click(function () {
            var _price = Number($("input[name=Price]", _form).val()),
                _online = Number($("input[name=OnlinePrice]", _form).val()),
                _offline = Number($("input[name=OfflinePrice]", _form).val());

            if ((_online < _price || _offline < _price) && !confirm("您设定的销售价格小于进货价格，确定保存？")) { return false; }

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