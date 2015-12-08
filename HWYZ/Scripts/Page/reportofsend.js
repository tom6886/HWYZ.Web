;
$(function () {

    var reportofsend = {};

    reportofsend.detail = function () {
        $.post("reportofsend/ListOfDetail", function (r) {
            $("#container").html(r);
        });

    };

    reportofsend.initPage = function () {
        window.onhashchange = function () {
            var hashStr = location.hash.replace("#", "");

            $(".panel .nav li").removeClass("active");

            $("." + hashStr).addClass("active");

            var func = reportofsend[hashStr];

            if (!func) { return false; }

            func();
        }
    }();

});