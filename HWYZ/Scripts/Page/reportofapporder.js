;
$(function () {

    var reportofapporder = {};

    reportofapporder.list = function (StoreId, StartDate, EndDate, pi) {
        $.post("reportofapporder/List", { StoreId: StoreId, StartDate: StartDate, EndDate: EndDate, pi: pi }, function (r) {
            $("#container").html(r);

            reportofapporder.setChart();

            $(".pagination a").click(function () {
                reportofapporder.list($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $(this).data("pageindex"));
                return false;
            });

            $("#goToBtn").click(function () {
                if (Number($("#pageIndexBox").val()) > Number($("#pageCount").val())) { alert("页索引超出范围"); return false; }

                reportofapporder.list($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("#pageIndexBox").val());
                return false;
            });
        });
    }

    reportofapporder.setChart = function () {
        $('#chart').highcharts({
            data: {
                table: 'datatable'
            },
            chart: {
                type: 'column'
            },
            title: {
                text: 'App营业结算排名'
            },
            yAxis: {
                allowDecimals: false,
                title: {
                    text: '销售额'
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

    reportofapporder.initPage = function () {

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
            $(this).attr('href', "reportofapporder/Export?StoreId={0}&StartDate={1}&EndDate={2}".format($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val()));
        });

        $(".query").click(function () {
            reportofapporder.list($("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("#pageIndexBox").val());
            return false;
        });

        reportofapporder.list();
    }();

});