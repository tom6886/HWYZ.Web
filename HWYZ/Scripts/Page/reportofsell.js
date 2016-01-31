;
$(function () {

    var reportofsell = {};

    reportofsell.number = function (ProductName, StartDate, EndDate, pi) {
        $.post("reportofsell/ListOfNumber", { ProductName: ProductName, StartDate: StartDate, EndDate: EndDate, pi: pi }, function (r) {
            $("#container").html(r);

            $(".pagination a").click(function () {
                reportofsell.number($("input[name=ProductName]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $(this).data("pageindex"));
                return false;
            });

            $("#goToBtn").click(function () {
                if (Number($("#pageIndexBox").val()) > Number($("#pageCount").val())) { alert("页索引超出范围"); return false; }

                reportofsell.number($("input[name=ProductName]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("#pageIndexBox").val());
                return false;
            });
        });
    };

    reportofsell.pay = function (ProductName, StartDate, EndDate, pi) {
        $.post("reportofsell/ListOfPay", { ProductName: ProductName, StartDate: StartDate, EndDate: EndDate, pi: pi }, function (r) {
            $("#container").html(r);

            reportofsell.setChart();

            $(".pagination a").click(function () {
                reportofsell.pay($("input[name=ProductName]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $(this).data("pageindex"));
                return false;
            });

            $("#goToBtn").click(function () {
                if (Number($("#pageIndexBox").val()) > Number($("#pageCount").val())) { alert("页索引超出范围"); return false; }

                reportofsell.pay($("input[name=ProductName]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("#pageIndexBox").val());
                return false;
            });
        });
    };

    reportofsell.setChart = function () {
        $('#chart').highcharts({
            data: {
                table: 'datatable'
            },
            chart: {
                type: 'column'
            },
            title: {
                text: '畅销商品排名'
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

    reportofsell.initPage = function () {

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

            if (!hashStr) { hashStr = "number"; }

            $(this).attr('href', "reportofsell/Export{0}?ProductName={1}&StartDate={2}&EndDate={3}".format(hashStr, $("input[name=ProductName]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val()));
        });

        $(".query").click(function () {
            onhashchange();
            return false;
        });

        window.onhashchange = function () {
            var hashStr = location.hash.replace("#", "");

            if (!hashStr) { hashStr = "number"; }

            $(".panel .nav li").removeClass("active");

            $("." + hashStr).addClass("active");

            var func = reportofsell[hashStr];

            if (!func) { return false; }

            func($("input[name=ProductName]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val());
        };

        onhashchange();
    }();

});