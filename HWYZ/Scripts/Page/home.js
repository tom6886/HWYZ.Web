;
$(function () {

    var home = {};

    home.openDialog = function (modal, userId) {
        $.post("home/queryDialog", { userId: userId }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            home.bindDialog(modal);
        });
    };

    home.bindDialog = function (modal) {
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
                Title: "required"
            },
            messages: {
                Title: "标题是必填项"
            }
        });

        var um = UM.getEditor('myEditor');

        $(".save", modal).click(function () {
            _form.submit();
        });
    }

    home.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            home.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });
    }();

});