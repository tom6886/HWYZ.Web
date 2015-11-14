;
$(function () {

    var users = {};

    users.openDialog = function (modal, roleId) {
        $.post("roles/queryDialog", { roleId: roleId }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            users.bindDialog(modal);
        });
    };

    users.bindDialog = function (modal) {
        var _form = $("form", modal);

        _form.ajaxForm({
            success: function (r) {
                alert(r.msg);

                if (r.code > 0) {
                    modal.modal('hide');
                    $("#form_query").submit();
                }
            }
        }).validate({
            rules: {
                RoleName: "required"
            },
            messages: {
                RoleName: "角色名称是必填项"
            }
        });

        $('select[name=Status]', _form).val($("#Status", _form).val());

        $(".save", modal).click(function () {
            _form.submit();
        });
    }

    users.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            users.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });
    }();

});