;
$(function () {

    var users = {};

    users.loadTable = function () {
        $.post("roles/getTable", function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            users.bindTable(r);
        });
    };

    users.bindTable = function (table) {
        $("#container").html(table);

        $(".delRole").click(function () {
            if (!confirm("您确定要删除此角色？")) { return false; }

            var _this = $(this);

            $.post("roles/deleteRole", { roleId: _this.parent().data('id') }, function (r) {
                alert(r.msg);

                if (r.code < 0) {
                    return false;
                }

                users.loadTable();
            });
        });
    }

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
            beforeSubmit: function () {
                return _form.valid();
            },
            success: function (r) {
                alert(r.msg);

                if (r.code > 0) {
                    modal.modal('hide');
                    users.loadTable();
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

        users.initTree(_form);

        $(".save", modal).click(function () {
            var zTree = $.fn.zTree.getZTreeObj("roleTree");

            var authVal = 0;

            if (zTree) {
                var checkedNodes = zTree.getCheckedNodes();

                for (var i = 0, length = checkedNodes.length; i < length; i++) {
                    authVal += Number(checkedNodes[i].authVal);
                }
            }

            $("input[name=RoleVal]", _form).val(authVal);

            _form.submit();
        });
    };

    users.initTree = function (_form) {

        var setting = {
            check: {
                enable: true
            },
            data: {
                simpleData: {
                    enable: true
                }
            }
        };

        $.post("roles/queryTree", { roleId: $("input[name=ID]", _form).val() }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            $.fn.zTree.init($("#roleTree", _form), setting, JSON.parse(r.tree));
        }, "json");
    };

    users.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            users.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        users.loadTable();
    }();

});