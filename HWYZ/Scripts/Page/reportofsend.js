;
$(function () {

    var reportofsend = {};

    reportofsend.detail = function (storeId, StartDate, EndDate, storeType, pi) {
        $.post("reportofsend/ListOfDetail", { storeId: storeId, StartDate: StartDate, EndDate: EndDate, storeType: storeType, pi: pi }, function (r) {
            $("#container").html(r);

            $(".table-page a").click(function () {
                reportofsend.detail($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("select[name=StoreType]").val(), $(this).data("pageindex"));
                return false;
            });
        });
    };

    reportofsend.store = function (storeId, StartDate, EndDate, storeType, pi) {
        $.post("reportofsend/ListOfStore", { storeId: storeId, StartDate: StartDate, EndDate: EndDate, storeType: storeType, pi: pi }, function (r) {
            $("#container").html(r);

            reportofsend.setChart();

            $(".table-page a").click(function () {
                reportofsend.store($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("select[name=StoreType]").val(), $(this).data("pageindex"));
                return false;
            });
        });
    };

    reportofsend.setChart = function () {
        $('#chart').highcharts({
            data: {
                table: 'datatable'
            },
            chart: {
                type: 'column'
            },
            title: {
                text: '配送金额排名'
            },
            yAxis: {
                allowDecimals: false,
                title: {
                    text: '配送金额'
                }
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.series.name + '</b><br/>' +
                        this.point.y + ' ' + this.point.name.toLowerCase();
                }
            }
        });
    }

    reportofsend.initPage = function () {

        $("._select").select_2();

        $('.form_date').datetimepicker({
            language: 'zh-CN',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            minView: 2,
            forceParse: 0
        });

        var date = new Date();
        var start = date.getFullYear() + "/" + (date.getMonth() + 1) + "/01";

        $("input[name=StartDate]").val(start);
        $("input[name=EndDate]").val(date.toLocaleDateString());

        $(".export").click(function () {
            var hashStr = location.hash.replace("#", "");

            if (!hashStr) { hashStr = "detail"; }

            $(this).attr('href', "reportofsend/Export{0}?storeId={1}&StartDate={2}&EndDate={3}&storeType={4}".format(hashStr, $("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("select[name=StoreType]").val()));
        });

        $(".query").click(function () {
            onhashchange();
            return false;
        });

        window.onhashchange = function () {
            var hashStr = location.hash.replace("#", "");

            if (!hashStr) { hashStr = "detail"; }

            $(".panel .nav li").removeClass("active");

            $("." + hashStr).addClass("active");

            var func = reportofsend[hashStr];

            if (!func) { return false; }

            func($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("select[name=StoreType]").val());
        };

        onhashchange();
    }();

});