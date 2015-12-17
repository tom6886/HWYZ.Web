;
$(function () {

    var store = {};

    store.openDialog = function (modal, storeId) {
        $.post("storegrant/queryDialog", { storeId: storeId }, function (r) {
            if (r.code < 0) {
                alert(r.msg);
                return false;
            }

            modal.html(r);

            store.bindDialog(modal);
        });
    };

    store.bindDialog = function (modal) {
        var _form = $("form", modal);

        _form.ajaxForm({
            beforeSubmit: function () {
                return _form.valid();
            },
            success: function (r) {
                alert(r.msg);

                if (r.code > 0) {
                    modal.modal('hide');
                    $("#form_query").submit();
                }
            }
        }).validate({
            rules: {
                StoreCode: "required",
                StoreName: "required",
                Address: "required",
                Presider: "required",
                Tel: "required",
                Lng: {
                    required: true,
                    number: true,
                    lng: true
                },
                Lat: {
                    required: true,
                    number: true,
                    lat: true
                }
            },
            messages: {
                StoreCode: "门店编号是必填项",
                StoreName: "门店名是必填项",
                Address: "地址是必填项",
                Presider: "负责人是必填项",
                Tel: "联系电话是必填项",
                Lng: {
                    required: "经度是必填项",
                    number: "经度只能输入数字"
                },
                Lat: {
                    required: "纬度是必填项",
                    number: "纬度只能输入数字"
                }
            }
        });

        $("._select", _form).select_2();

        $('select[name=StoreType]', _form).val($("#StoreType", _form).val());
        $('select[name=Discount]', _form).val(Number($("#Discount", _form).val()));
        $('select[name=Status]', _form).val($("#Status", _form).val());

        $(".save", modal).click(function () {

            var isNull = false;

            $("input._select", _form).each(function (i, v) {
                if (!$(v).val()) {
                    var html = '<label generated="true" class="error">{0}是必选项</label>';

                    $(v).after(html.format($(v).siblings("span").text()));

                    isNull = true;
                }
            });

            if (isNull) { return false; }

            var country = $("input[name=Country]").select2("data");

            $("input[name=CityCode]").val(country.code);

            _form.submit();
        });

        $(".map", modal).click(function () {
            var city = $("input[name=City]").val(), country = $("input[name=Country]").val();

            if (!country) { alert("请先选择城市"); return false; }

            store.resetMap(city);
        });
    }

    store.resetMap = function (city) {

        $("#dlg_map").modal("show");

        $.post("storegrant/getMap", { city: city }, function (r) {

            var list = JSON.parse(r),
                opts = {
                    width: 250,     // 信息窗口宽度
                    height: 80,     // 信息窗口高度
                    title: "分店地址" // 信息窗口标题
                };;

            for (var i = 0, length = list.length; i < length; i++) {

                var marker = new BMap.Marker(new BMap.Point(list[i].Lng, list[i].Lat));  // 创建标注

                var content = list[i].Address;

                map.addOverlay(marker);               // 将标注添加到地图中

                store.addClickHandler(content, marker, opts);
            }

            setTimeout(function () {
                map.centerAndZoom(city, 13);
            }, 100);
        });
    }

    store.addClickHandler = function (content, marker, opts) {
        marker.addEventListener("mouseover", function (e) {
            var p = e.target;
            var point = new BMap.Point(p.getPosition().lng, p.getPosition().lat);
            var infoWindow = new BMap.InfoWindow(content, opts);  // 创建信息窗口对象 
            map.openInfoWindow(infoWindow, point); //开启信息窗口
        });
    }

    store.initPage = function () {
        $("#dlg_edit").on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            store.openDialog($(this), button.parent().data('id'));
        }).on('hidden.bs.modal', function () {
            $(".modal-dialog", $(this)).remove();
        });

        $("#basic-addon2").click(function () {
            $("#form_query").submit();
        });
    }();

});