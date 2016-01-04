;
(function ($) {
    var area = { province: null, city: null, country: null };

    var priv = {
        set: function ($this) {
            $this.each(function (i, v) {
                var func = fc[$(v).data('select')];

                if (!func) { return true; }

                func($(v));
            });
        },
        select: function (obj) {
            var org = obj.elem.select2({
                allowClear: true,
                placeholder: obj.placeholder,
                ajax: {
                    url: "common/" + obj.url,
                    dataType: 'json',
                    quietMillis: 100,
                    data: function (term, page) {
                        var query = { key: term, page: page };

                        if (typeof (obj.addQuery) == "function") {
                            obj.addQuery(query);
                        };

                        return query;
                    },
                    results: function (data, page) {
                        return { results: data.results, more: (page * data.pageSize) < data.total };
                    }
                }, id: function (data) {
                    return data[obj.dataParam];
                }, formatResult: function (data) {
                    return '<span class="select2-match" class="selector_subject_options" ></span>' + data.name;
                }, formatSelection: function (data) {
                    return data.name;
                }, initSelection: function (e, callback) {
                    callback({ id: e.data('id'), name: e.val() });
                }
            });
            if (obj.elem.data('value')) { obj.elem.select2('val', [obj.elem.data('value')]); }

            return org;
        }
    };

    var fc = {
        p: function (e) {
            area.province = priv.select({ elem: e, placeholder: "选择省", url: "getArea?levelType=1", dataParam: "name" });

            area.province.on('change', function (e) {
                if (area.city) {
                    area.city.select2('val', '');
                }

                if (area.country) {
                    area.country.select2('val', '');
                }
            });
        },
        ci: function (e) {
            area.city = priv.select({
                elem: e, placeholder: "选择市", url: "getArea?levelType=2", dataParam: "name", addQuery: function (query) {
                    if (area.province) {
                        var i = area.province.select2('data');
                        if (i) { query.pId = i.id; };
                    }
                }
            });

            area.city.on('change', function (e) {
                if (area.country) {
                    area.country.select2('val', '');
                }
            });
        },
        co: function (e) {
            area.country = priv.select({
                elem: e, placeholder: "选择县", url: "getArea?levelType=3", dataParam: "name", addQuery: function (query) {
                    if (area.city) {
                        var i = area.city.select2('data');
                        if (i) { query.pId = i.id; };
                    }
                }
            });
        },
        s: function (e) {
            priv.select({ elem: e, placeholder: "选择门店", url: "getStore", dataParam: "id" });
        },
        pr: function (e) {
            priv.select({ elem: e, placeholder: "选择商品", url: "getProduct", dataParam: "name" });
        },
        spr: function (e) {
            priv.select({ elem: e, placeholder: "选择商品", url: "getStoreProduct", dataParam: "name" });
        },
        u: function (e) {
            priv.select({ elem: e, placeholder: "选择登录账号", url: "getUser", dataParam: "id" });
        }
    }

    $.fn.select_2 = function () {

        var $this = this;

        if ($this.length == 0) { return; }

        priv.set($this);

        return $this;
    };
})(jQuery);