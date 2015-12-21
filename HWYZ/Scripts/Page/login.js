;
$(function () {

    var _form = $("#form_login");

    _form.ajaxForm({
        dataType: 'json',
        success: function (r) {
            if (r.code < 0) {
                $('form').fadeIn(500);
                $('.wrapper').removeClass('form-success');
                alert(r.msg);
                return false;
            }

            location.href = r.url;
        }
    });

    $("#login-button").click(function () {
        if (!$("input[name=account]").val()) {
            alert("请输入用户编号");
            return false;
        }

        if (!$("input[name=pwd]").val()) {
            alert("请输入口令");
            return false;
        }

        event.preventDefault();
        $('form').fadeOut(500);
        $('.wrapper').addClass('form-success');

        _form.submit();
    });
});