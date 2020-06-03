(function ($) {
    $.fn.extend({
        //myAlert: function () { alert("实例才能调用我"); },
        //将表单对象序列化成json对象
        serializeJson: function () {
            var serializeObj = {};
            var array = this.serializeArray();
            var str = this.serialize();
            $(array).each(function () {
                if (serializeObj[this.name]) {
                    if ($.isArray(serializeObj[this.name])) {
                        serializeObj[this.name].push(this.value);
                    } else {
                        serializeObj[this.name] = [serializeObj[this.name], this.value];
                    }
                } else {
                    serializeObj[this.name] = this.value;
                }
            });

            //针对ace的switch进行封装
            $(this).find("input.ace-switch").each(function () {
                serializeObj[this.name] = $(this).prop("checked");
            });

            return serializeObj;
        },
        //下拉表格
        YBdropTable: function (o) {
            //
            if (!this.dropdown) {
                if (!layer) {
                    layer.msg("请引用jquery.dropdown.js");
                }
                else {
                    alert("请引用jquery.dropdown.js");
                }
                return;
            }
            var tableoption = { qtitletext: "请输入关键字", pagesize: 10, showSearch: true, showTableTH: true, showPager: true, autoload: true, textindex: 1, valueindex: 0, qtextWidth: 300, autoPost: false, multipleChoices: false };
            if (!o.dataformat) {
                o.dataformat = function (data) {
                    var jsonData = {};

                    for (var i = 0; i < o.tableoptions.colmodel.length; i++) {
                        jsonData[o.tableoptions.colmodel[i].name] = data[i];
                    }
                    return jsonData;

                    //return {
                    //    text: data[1], value: data[0]
                    //};
                };
            }

            if (!o || !o.tableoptions) {
                if (!layer) {
                    layer.msg("请引用jquery.dropdown.js");
                }
                else {
                    alert("tableoptions必须被设置");
                }
                return;
            }
            else {
                if (o.tableoptions.url && o.tableoptions.colmodel && o.tableoptions.colmodel.length > 0) {
                    $.extend(tableoption, o.tableoptions);
                }
                else {
                    alert("tableoptions中的url,colmodel参数必须被配置");
                    return;
                }
            }
            var qtparse = {
                name: "qtable",
                render: function (parent) {
                    var target = this.target;
                    var qpanel = $($.format("<div class='input-group' style='{0}{1}'/>", "width:" + tableoption.qtextWidth + "px;", (tableoption.showSearch ? "" : "display: none;"))); //querypanel

                    var qtext = $("<input type='text' class='form-control search-query' name='SearchStr' placeholder='" + tableoption.qtitletext + "'>");
                    //$(qtext).data("watermark", tableoption.qtitletext);
                    var qbtn = $("<span class='input-group-btn'><button type='button' class='btn btn-success btn-sm'>查询<i class='icon-search icon-on-right bigger-110'></i></button></span>");
                    qtext.focus(function () {
                        //var v = $(this).val();
                        //var mark = $(this).data("watermark");
                        //if (v == mark) {
                        //    $(this).val("").removeClass("watermark");;
                        //}
                    })
                    .blur(function () {
                        //var v = $(this).val();
                        //var mark = $(this).data("watermark");
                        //if (v == "") {
                        //    $(this).val(mark).addClass("watermark");
                        //}
                    })
                    .keypress(function (e) {
                        if (e.keyCode == 13) {
                            query(this.value);
                            return false;
                        }
                    })
                    .keyup(function () {
                        //自动查询
                        if (tableoption.autoPost) {
                            query(this.value);
                        }
                        return false;
                    });
                    qbtn.click(function () {

                        var v = $(this).prev().val();
                        query(v);
                        return false;
                    });
                    qpanel.append(qtext).append(qbtn);
                    var tbpanel = $("<div class='tablecontaienr' style='background-color: #FFFFFF;'/>");  //tablepanel
                    var thtml = [];
                    thtml.push($.format("<table class='table table-striped table-bordered table-hover' cellpadding='2' cellspacing='0'><thead><tr style='{0}'>", tableoption.showTableTH ? "" : "display: none;"));
                    for (var i = 0, l = tableoption.colmodel.length; i < l; i++) {
                        thtml.push("<th style='" + (tableoption.colmodel[i].visible ? "" : "display: none;") + "'><div style='width:", tableoption.colmodel[i].width, "'>", tableoption.colmodel[i].displayname, "</div></th>");
                    }
                    thtml.push("</tr></thead>");
                    thtml.push("<tbody></tbody></table>");
                    tbpanel.html(thtml.join(""));
                    var ppanel = $($.format("<div class='pagecontainer' style='margin-top: -21px;{0}'/>", tableoption.showPager ? "" : "display: none;"));  //pagepanel
                    //<a class='pagenext' href="javascript:void(0);">下一页</a><a class="pageprev" href="javascript:void(0);">上一页</a><span>1/10</span>
                    var pnexta = $("<li><a class='pagenext' href=\"javascript:void(0);\">下一页 →</a></li>").click(function () {
                        var v = qtext.val();
                        query(v, 1);
                    });
                    var ppreva = $("<li><a class='pageprev' href='javascript:void(0);'>← 上一页</a></li>").click(function () {
                        var v = qtext.val();
                        query(v, -1);
                    });

                    var pagination = $("<ul class='pagination' style='margin-top: 0px;'/>");


                    pagination.append(ppreva).append("<li><span>0/0</span></li>").append(pnexta);
                    ppanel.append(pagination);

                    parent.append(qpanel).append(tbpanel).append(ppanel);
                    if (tableoption.autoload) {
                        query("", 0);
                    }
                    function query(v, ptype) {

                        if (v == tableoption.qtitletext) {
                            v = "";
                        }
                        if (ptype == 0) {
                            tableoption.pageindex = 0;
                        }
                        else if (ptype == 1) {
                            if (tableoption.pageindex + 1 < tableoption.pageTotal) {
                                tableoption.pageindex++;
                            }
                        }
                        else {
                            tableoption.pageindex = 0;
                            //if (tableoption.pageindex - 1 >= 0) {
                            //    tableoption.pageindex--;
                            //}
                        }
                        if (tableoption.cols != "") {
                            var cols = [];
                            for (var cindex = 0, clength = tableoption.colmodel.length; cindex < clength; cindex++) {
                                cols.push(tableoption.colmodel[cindex].name);
                            }
                            tableoption.cols = cols.join(",");
                        }
                        var p = { "pageindex": tableoption.pageindex, "pagesize": tableoption.pagesize, "cols": tableoption.cols, "qtext": v };
                        var purl = tableoption.url + (tableoption.url.indexOf('?') > -1 ? '&' : '?') + '_=' + (new Date()).valueOf();
                        $.ajax({
                            type: "POST",
                            url: purl,
                            data: p,
                            dataType: "json",
                            success: function (data) { adddata(data); },
                            error: function (data) { alert("抱歉，发生错误了"); }
                        });

                    }
                    function adddata(data) {
                        //如果服务器返回的是字符串，而不是json对象的时候，转换一下
                        if (typeof (data) != "object") {
                            data = $.parseJSON(data);
                        }

                        if (data.total >= 0) {
                            tableoption.total = data.total;
                            tableoption.pageTotal = Math.ceil(tableoption.total / tableoption.pagesize);
                        }
                        ppanel.find("ul>li>span").text((tableoption.pageindex + 1) + "/" + tableoption.pageTotal);
                        var thtml = [];
                        for (var i = 0, l = data.rows.length; i < l; i++) {
                            thtml.push("<tr>");
                            for (var j = 0, k = tableoption.colmodel.length; j < k; j++) {
                                thtml.push("<td style='" + (tableoption.colmodel[j].visible ? "" : "display: none;") + "'><div>", data.rows[i][tableoption.colmodel[j].name] || "&nbsp;", "</div></td>");
                            }
                            thtml.push("</tr>");
                        }
                        var tbody = tbpanel.find(">table tbody");
                        tbody.html(thtml.join(""));
                        tbody.find("tr").each(function () {
                            $(this).DhoverClass("hover").click(select);
                        });

                    }
                    function select() {
                        
                        var cell = [];
                        $(this).find("div").each(function () {
                            var t = $(this).text();
                            cell.push(t);
                        })
                        var ret = o.dataformat(cell);
                        o.tableoptions.singleSelectFunc(ret);
                        target.SelectedChanged(ret);
                    }
                },
                items: [],
                setValue: function (item) { },
                onshow: function (parent) {
                    var input = parent.find("input.search-query").val("");
                    if (tableoption.autoload) {
                        parent.find("span.input-group-btn>button").click();
                    }
                },
                target: null
            };
            $.extend(o, { parse: qtparse, containerCssClass: "qtableContainer", autoheight: true });
            return $(this).dropdown(o);
        }
    });

    $.extend({
        //hello: function () { alert('全局方法，直接$来调用'); }
        //jsonStr：要填充的json，fillID：填充对象的ID
        loadData: function (jsonStr, fillID) {
            var obj;
            if ((typeof jsonStr).toLowerCase() == "string") {
                obj = eval("(" + jsonStr + ")");
            }
            else {
                obj = jsonStr;
            }

            var key, value, tagName, type, arr;
            for (x in obj) {
                key = x;
                value = obj[x];
                $(fillID + " [name='" + key + "'],[name='" + key + "[]']").each(function () {
                    tagName = $(this)[0].tagName;
                    type = $(this).attr('type');
                    if (tagName == 'INPUT') {
                        if (type == 'radio') {
                            $(this).attr('checked', $(this).val() == value);
                        } else if ($(this).hasClass("ace-switch")) {
                            //如果是ace的switch开关
                            if (value == true) {
                                $(this).prop("checked", true)
                            }
                            else {
                                $(this).prop("checked", false)
                            }

                        } else if (type == 'checkbox') {

                            if (typeof value == "string") {
                                arr = value.split(',');
                                for (var i = 0; i < arr.length; i++) {
                                    if ($(this).val() == arr[i]) {
                                        $(this).attr('checked', true);
                                        break;
                                    }
                                }
                            }
                            else if (typeof value == "boolean") {
                                $(this).attr('checked', value);
                            }
                           
                        } else {
                            $(this).val(value);
                        }
                    } else if (tagName == 'SELECT' || tagName == 'TEXTAREA') {
                        $(this).val(value);
                    }

                });
            }
        },

        //格式化字符串
        format: function (source, params) {
            if (arguments.length == 1)
                return function () {
                    var args = $.makeArray(arguments);
                    args.unshift(source);
                    return $.format.apply(this, args);
                };
            if (arguments.length > 2 && params.constructor != Array) {
                params = $.makeArray(arguments).slice(1);
            }
            if (params.constructor != Array) {
                params = [params];
            }
            $.each(params, function (i, n) {
                source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
            });
            return source;
        },
        //截取字符串
        cutstr2: function (str, len, replaceStr) {
            var str_length = 0;
            var str_len = 0;
            str_cut = new String();
            str_len = str.length;
            for (var i = 0; i < str_len; i++) {
                a = str.charAt(i);
                str_length++;
                if (escape(a).length > 4) {
                    //中文字符的长度经编码之后大于4
                    str_length++;
                }
                str_cut = str_cut.concat(a);
                if (str_length >= len) {
                    str_cut = str_cut.concat(replaceStr);
                    return str_cut;
                }
            }
            //如果给定字符串小于指定长度，则返回源字符串；
            if (str_length < len) {
                return str;
            }
        },
        //截取字符串
        cutstr: function (str, len) {
            return $.cutstr2(str, len, "...");
        },
        //获取Url参数
        getUrlParam: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null)
                return decodeURIComponent(r[2]);
            return null;
        },
        //判断字符串是否为空
        isNullOrEmpty: function (send) {
            if (send == '' || send == null || send == undefined) {
                return true;
            }
            return false;
        },
        //判断数组内是否包含某个值
        arrIsInclude: function (arrObj, singleEL) {
            //
            var flage = false;
            $(arrObj).each(function (index, el) {
                if (el == singleEL)
                {
                    flage = true;
                }
            });
            return flage;
        },
        selfAdaptionButton: function (buttonHtml)
        {
            if ($.isNullOrEmpty(buttonHtml)) {
                return "";
            }

            //大屏显示html
            var bigHtml = "<div class=\"visible-md visible-lg hidden-sm hidden-xs btn-group\">{0}</div>";

            //小屏显示html
            var smallHtml = "<div style=\"position: absolute;margin-top: -10px;\" class=\"visible-xs visible-sm hidden-md hidden-lg\">" +
                                    "<div class=\"inline position-relative\">" +
                                        "<button class=\"btn btn-minier btn-primary dropdown-toggle\" data-toggle=\"dropdown\">" +
                                            "<i class=\"icon-cog icon-only bigger-110\"></i>" +
                                        "</button>" +
                                        "<ul style=\"padding: 5px;\" class=\"dropdown-menu dropdown-only-icon dropdown-yellow pull-right dropdown-caret dropdown-close\">" +
                                        "{0}" +
                                        "</ul>" +
                                    "</div>" +
                            " </div>";

            var endButtonHtml = $.format(bigHtml, buttonHtml) + $.format(smallHtml, buttonHtml);
            return endButtonHtml;
        },
        //日期格式化函数
        formatDate: function(pattern,date){
            //如果不设置，默认为当前时间
            if(!date) date = new Date();
            if(typeof(date) ==="string"){
                if(date=="")  date = new Date();
                else  date = new Date(date.replace(/-/g,"/"));
            }	
            /*补00*/
            var toFixedWidth = function(value){
                var result = 100+value;
                return result.toString().substring(1);
            };
    
            /*配置*/
            var options = {
                regeExp:/(yyyy|M+|d+|h+|m+|s+|ee+|ws?|p)/g,
                months: ['January','February','March','April','May',
                         'June','July', 'August','September',
                      'October','November','December'],
                weeks: ['Sunday','Monday','Tuesday',
                        'Wednesday','Thursday','Friday',
                      'Saturday']
            };
    
            /*时间切换*/
            var swithHours = function(hours){
                return hours<12?"AM":"PM";
            };
    
            /*配置值*/
            var pattrnValue = {
                "yyyy":date.getFullYear(),                      //年份
                "MM":toFixedWidth(date.getMonth()+1),           //月份
                "dd":toFixedWidth(date.getDate()),              //日期
                "hh":toFixedWidth(date.getHours()),             //小时
                "mm":toFixedWidth(date.getMinutes()),           //分钟
                "ss":toFixedWidth(date.getSeconds()),           //秒
                "ee":options.months[date.getMonth()],           //月份名称
                "ws":options.weeks[date.getDay()],              //星期名称
                "M":date.getMonth()+1,
                "d":date.getDate(),
                "h":date.getHours(),
                "m":date.getMinutes(),
                "s":date.getSeconds(),
                "p":swithHours(date.getHours())
            };
    
            return pattern.replace(options.regeExp,function(){
                return  pattrnValue[arguments[0]];
            });
        }
    });

})(jQuery);