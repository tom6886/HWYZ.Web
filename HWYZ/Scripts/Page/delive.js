;
$(function () {

    var delive = {};

    delive.openDialog = function (modal, orderId, itemId) {
        if (!(orderId && itemId)) { alert("参数错误"); return false; }

        $.post("deliveedit/queryDialog", { orderId: orderId, itemId: itemId }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            delive.bindDialog(modal);
        });
    };

    delive.bindDialog = function (modal) {
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
                    $(".pay").val(r.pay);
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

        $("select[name=Discount]", _form).val(Number($("#Discount", _form).val()));

        $("select[name=Discount]", _form).change(function () {
            $("#pay", _form).val($("input[name=Price]").val() * $(this).val() * $("input[name=RealNumber]").val());
        });

        $("input[name=OrderNumber]", _form).on('input propertychange', function () {
            $("#pay", _form).val($("input[name=Price]").val() * $(this).val() * $("input[name=Discount]").val());
        });

        $(".save", modal).click(function () {
            _form.submit();
        });
    }

    delive.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            delive.openDialog($(this), $("input[name=orderId]").val(), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $(".send").click(function () {
            var _orderId = $("input[name=orderId]").val(),
                _pay = $("input[name=Pay]").val(),
                _expressCode = $("input[name=ExpressCode]").val(),
                _expressUrl = $("input[name=ExpressUrl]").val();

            if (!_pay) { alert("实际金额为必填项"); return false; }

            if (isNaN(_pay) || _pay <= 0) { alert("实际金额必须输入数字"); return false; }

            if (!_expressCode) { alert("快递单号为必填项"); return false; }

            if (!confirm("确认发货？发货后订单将不能再进行任何更改")) { return false; }

            $.post("deliveedit/sendProduct", { orderId: _orderId, pay: _pay, expressCode: _expressCode, expressUrl: _expressUrl }, function (r) {
                alert(r.msg);

                if (r.code > 0) {
                    window.location.href = "orderview?orderId=" + _orderId;
                }
            });
        });
    }();

});