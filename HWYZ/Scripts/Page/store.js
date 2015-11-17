;
$(function () {

    var store = {};

    store.openDialog = function (modal, storeId) {
        $.post("store/queryDialog", { storeId: storeId }, function (r) {
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
                Account: "required",
                Name: "required",
                CardNumber: "required"
            },
            messages: {
                Account: "用户编号是必填项",
                Name: "用户姓名是必填项",
                CardNumber: "身份证号是必填项"
            }
        });

        $('select[name=RoleId]', _form).val($("#RoleId", _form).val());
        $('select[name=Sex]', _form).val($("#Sex", _form).val());
        $('select[name=Status]', _form).val($("#Status", _form).val());

        $(".save", modal).click(function () {
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