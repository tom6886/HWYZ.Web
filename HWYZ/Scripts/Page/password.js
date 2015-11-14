;
$(function () {
    var _form = $("#form_change");

    _form.ajaxForm({
        dataType: 'json',
        beforeSubmit: function () {
            return _form.valid();
        },
        success: function (r) {
            alert(r.msg);

            if (r.code > 0) {
                setTimeout("location.href = '{0}';".format(r.url), 3000);
            }
        }
    }).validate({
        rules: {
            oldPass: "required",
            newPass: "required",
            newPassAgain: { required: true, equalTo: "#newPass" }
        },
        messages: {
            oldPass: "请输入原密码",
            newPass: "请输入新密码",
            newPassAgain: { required: "请再次输入新密码", equalTo: "两次输入密码不一致" }
        }
    });

});