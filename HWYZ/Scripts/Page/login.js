;
$(function () {

    var _form = $("#form_login");

    _form.ajaxForm({
        dataType: 'json',
        beforeSubmit: function () {
            return _form.valid();
        },
        success: function (r) {
            alert(r.msg);

            if (r.code > 0) {
                location.href = r.url;
            }
        }
    }).validate({
        rules: {
            account: "required",
            pwd: "required"
        },
        messages: {
            account: "请输入用户编号",
            pwd: "请输入口令"
        }
    });


});