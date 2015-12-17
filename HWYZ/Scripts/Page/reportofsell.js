﻿;
$(function () {

    var reportofsell = {};

    reportofsell.number = function (ProductName, StartDate, EndDate, pi) {
        $.post("reportofsell/ListOfNumber", { ProductName: ProductName, StartDate: StartDate, EndDate: EndDate, pi: pi }, function (r) {
            $("#container").html(r);

            $(".table-page a").click(function () {
                reportofsell.number($("input[name=ProductName]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $(this).data("pageindex"));
                return false;
            });
        });
    };

    reportofsell.pay = function (ProductName, StartDate, EndDate, pi) {
        $.post("reportofsell/ListOfPay", { ProductName: ProductName, StartDate: StartDate, EndDate: EndDate, pi: pi }, function (r) {
            $("#container").html(r);

            reportofsell.setChart();

            $(".table-page a").click(function () {
                reportofsell.pay($("input[name=ProductName]").val(), $("input[name=StartDate]").val(), $("input[name=EndDate]").val(), $(this).data("pageindex"));
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
            language: 'fr',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            minView: 2,
            forceParse: 0
        });

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