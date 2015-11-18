;
$(function () {

    var store = {};

    store.openDialog = function (modal, storeId) {
        $.post("storegrant/queryDialog", { storeId: storeId }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            store.bindDialog(modal);
        });
    };

    store.bindDialog = function (modal) {
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
                StoreCode: "required",
                StoreName: "required",
                Address: "required",
                Lng: {
                    required: true,
                    number: true,
                    lng: true
                },
                Lat: {
                    required: true,
                    number: true,
                    lat: true
                }
            },
            messages: {
                StoreCode: "门店编号是必填项",
                StoreName: "门店名是必填项",
                Address: "地址是必填项",
                Lng: {
                    required: "经度是必填项",
                    number: "经度只能输入数字"
                },
                Lat: {
                    required: "纬度是必填项",
                    number: "纬度只能输入数字"
                }
            }
        });

        $("._select", _form).select_2();

        $('select[name=StoreType]', _form).val($("#StoreType", _form).val());
        $('select[name=Discount]', _form).val(Number($("#Discount", _form).val()));
        $('select[name=Status]', _form).val($("#Status", _form).val());

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

            _form.submit();
        });
    }

    store.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            store.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#basic-addon2").click(function () {
            $("#form_query").submit();
        });
    }();

});