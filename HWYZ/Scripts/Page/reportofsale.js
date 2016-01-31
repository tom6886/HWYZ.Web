;
$(function () {

    var reportofsale = {};

    reportofsale.list = function (Province, City, Country, StoreId, StartDate, EndDate, pi) {
        $.post("reportofsale/List", { Province: Province, City: City, Country: Country, StoreId: StoreId, StartDate: StartDate, EndDate: EndDate, pi: pi }, function (r) {
            $("#container").html(r);

            reportofsale.setChart();

            $(".pagination a").click(function () {
                reportofsale.list($("input[name=Province]").val(), $("input[name=City]").val(), $("input[name=Country]").val(), $("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $(this).data("pageindex"));
                return false;
            });

            $("#goToBtn").click(function () {
                if (Number($("#pageIndexBox").val()) > Number($("#pageCount").val())) { alert("页索引超出范围"); return false; }

                reportofsale.list($("input[name=Province]").val(), $("input[name=City]").val(), $("input[name=Country]").val(), $("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("#pageIndexBox").val());
                return false;
            });
        });
    }

    reportofsale.setChart = function () {
        $('#chart').highcharts({
            data: {
                table: 'datatable'
            },
            chart: {
                type: 'column'
            },
            title: {
                text: '营业经营排名'
            },
            yAxis: {
                allowDecimals: false,
                title: {
                    text: '营业额'
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

    reportofsale.initPage = function () {

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
            $(this).attr('href', "reportofsale/Export?Province={0}&City={1}&Country={2}&StoreId={3}&StartDate={4}&EndDate={5}".format($("input[name=Province]").val(), $("input[name=City]").val(), $("input[name=Country]").val(), $("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val()));
        });

        $(".query").click(function () {
            reportofsale.list($("input[name=Province]").val(), $("input[name=City]").val(), $("input[name=Country]").val(), $("input[name=StoreId]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $("#pageIndexBox").val());
            return false;
        });

        reportofsale.list();
    }();

});