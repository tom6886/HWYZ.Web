﻿;
$(function () {

    var order = {};

    order.openDialog = function (modal, orderId, itemId) {
        if (!orderId) { alert("参数错误"); return false; }

        $.post("orderedit/queryDialog", { orderId: orderId, itemId: itemId }, function (r) {
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

        $("input[name=OrderNumber]", _form).on('input propertychange', function () {
            $("#pay", _form).val($("input[name=Price]").val() * $(this).val() * $("input[name=Discount]").val());
        });

        $(".save", modal).click(function () {
            _form.submit();
        });
    }

    order.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            order.openDialog($(this), $("input[name=orderId]").val(), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $(".submit").click(function () {
            var _orderId = $("input[name=orderId]").val(),
                _remark = $("input[name=remark]").val();

            if (!confirm("确认提交？提交后您将不能再对订单进行任何更改")) { return false; }

            $.post("orderedit/submitOrder", { orderId: _orderId, remark: _remark }, function (r) {
                alert(r.msg);

                if (r.code > 0) {
                    window.location.href = "orderview?orderId=" + _orderId;
                }
            });
        });
    }();

});