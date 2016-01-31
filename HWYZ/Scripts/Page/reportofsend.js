;
$(function () {

    var reportofsend = {};

    reportofsend.detail = function (storeId, StartDate, EndDate, storeType, pi) {
        $.post("reportofsend/ListOfDetail", { storeId: storeId, StartDate: StartDate, EndDate: EndDate, storeType: storeType, pi: pi }, function (r) {
            $("#container").html(r);

            $(".pagination a").click(function () {
                reportofsend.detail($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("select[name=StoreType]").val(), $(this).data("pageindex"));
                return false;
            });

            $("#goToBtn").click(function () {
                if (Number($("#pageIndexBox").val()) > Number($("#pageCount").val())) { alert("页索引超出范围"); return false; }

                reportofsend.detail($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("select[name=StoreType]").val(), $("#pageIndexBox").val());
                return false;
            });
        });
    };

    reportofsend.store = function (storeId, StartDate, EndDate, storeType, pi) {
        $.post("reportofsend/ListOfStore", { storeId: storeId, StartDate: StartDate, EndDate: EndDate, storeType: storeType, pi: pi }, function (r) {
            $("#container").html(r);

            reportofsend.setChart();

            $(".pagination a").click(function () {
                reportofsend.store($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("select[name=StoreType]").val(), $(this).data("pageindex"));
                return false;
            });

            $("#goToBtn").click(function () {
                if (Number($("#pageIndexBox").val()) > Number($("#pageCount").val())) { alert("页索引超出范围"); return false; }

                reportofsend.store($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("select[name=StoreType]").val(), $("#pageIndexBox").val());
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
            lang: {
                printChart: "打印图表",
                downloadJPEG: "下载JPEG 图片",
                downloadPDF: "下载PDF文档",
                downloadPNG: "下载PNG 图片",
                downloadSVG: "下载SVG 矢量图",
                exportButtonTitle: "导出图片"
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
        var end = date.getFullYear() + "/" + (date.getMonth() + 1) + "/" + date.getDate();

        $("input[name=StartDate]").val(start);
        $("input[name=EndDate]").val(end);

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