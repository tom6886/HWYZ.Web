;
$(function () {

    var dictionary = {};

    dictionary.openDialog = function (modal, id) {
        $.post("dictionary/queryDialog", { id: id }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            dictionary.bindDialog(modal);
        });
    };

    dictionary.bindDialog = function (modal) {
        var _form = $("form", modal);

        _form.ajaxForm({
            success: function (r) {
                alert(r.msg);

                if (r.code > 0) {
                    modal.modal('hide');
                    $("#form_query").submit();
                }
            }
        });

        $(".save", modal).click(function () {
            _form.submit();
        });
    }

    dictionary.openNodes = function (modal, code) {
        $.post("dictionary/queryNodes", { code: code }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            $("#addNode", modal).click(function () {
                dictionary.openNodeEdit($("#parent").val());
            });

            dictionary.getNodes(code);
        });
    };

    dictionary.getNodes = function (code) {
        $.post("dictionary/getNodes", { code: code }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            dictionary.bindNodes(r);
        });
    }

    dictionary.bindNodes = function (table) {
        $("#container_nodes").html(table);

        $(".editDict", $("#container_nodes")).click(function () {
            dictionary.openNodeEdit($("#parent").val(), $(this).parent().data("id"));
        });
    }

    dictionary.openNodeEdit = function (parentCode, id) {
        $.post("dictionary/queryNodeEdit", { parentCode: parentCode, id: id }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            $("#dlg_node_edit").modal("show").html(r);

            dictionary.bindNodeEdit($("#dlg_node_edit"));
        });
    }

    dictionary.bindNodeEdit = function (modal) {
        var _form = $("form", modal);

        _form.ajaxForm({
            success: function (r) {
                alert(r.msg);

                if (r.code > 0) {
                    modal.modal('hide');
                    dictionary.getNodes($("#parent").val());
                }
            }
        });

        $(".save", modal).click(function () {
            _form.submit();
        });
    }

    dictionary.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            dictionary.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#dlg_nodes").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            dictionary.openNodes($(this), button.parent().data('code'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#dlg_node_edit").on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#basic-addon2").click(function () {
            $("#form_query").submit();
        });
    }();

});