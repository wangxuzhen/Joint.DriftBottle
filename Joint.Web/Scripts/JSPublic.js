var JSPublic = JSPublic || {};
(function () {

    JSPublic.Forms = {
        showModalDialog: function (title, showId, callback) {
            var divHead = ' <div class="modal fade" id="showModalDialog"><div class="modal-dialog"><div class="modal-content"><div class="modal-header">' +
                        '<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>' +
                        '<h4 class="modal-title blue lighter"><i></i> <label id="modalTitle"> ' + title + '</label></h4></div>' +
                        '<div class="modal-body">';
            var divFoot = ' </div><div class="modal-footer"><button type="button" class="btn btn-default" data-dismiss="modal"><i class="icon-remove"></i>关闭</button>' +
                        '<button type="button" id="btnShowModalDialogSubmit" class="btn btn-primary"><i class="icon-save"></i>保存</button></div></div></div></div>';
            var divBody = $('#' + showId).prop("outerHTML");

            var html = divHead + divBody + divFoot;
            var obj = $(html).appendTo('.main-content');

            $('#showModalDialog').modal();
            $("#showModalDialog").find("input").val("");
            $("#btnShowModalDialogSubmit").click(callback);

        }
    }

    // 对Date的扩展，将 Date 转化为指定格式的String
    // 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
    // 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
    // 例子： 
    // (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
    // (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
    Date.prototype.Format = function (fmt) {
        var o = {
            "M+": this.getMonth() + 1, //月份 
            "d+": this.getDate(), //日 
            "h+": this.getHours(), //小时 
            "m+": this.getMinutes(), //分 
            "s+": this.getSeconds(), //秒 
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
            "S": this.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    }


    JSPublic.Date = {
        /**
        * 传入时间相加减
        */
        DateOperator: function (date, days) {

            date = date.replace(/-/g, "/"); //更改日期格式  
            var nd = new Date(date);
            nd = nd.valueOf();
            nd = nd + days * 24 * 60 * 60 * 1000;
            nd = new Date(nd);
            var y = nd.getFullYear();
            var m = nd.getMonth() + 1;
            var d = nd.getDate();
            if (m <= 9) m = "0" + m;
            if (d <= 9) d = "0" + d;
            var cdate = y + "-" + m + "-" + d;
            return cdate;
        },

        //js获取日期：前天、昨天、今天、明天、后天 AddDayCount整数传参
        GetDateStr: function (AddDayCount) {
            var dd = new Date();
            dd.setDate(dd.getDate() + AddDayCount);//获取AddDayCount天后的日期
            var y = dd.getFullYear();
            var m = dd.getMonth() + 1;//获取当前月份的日期
            var d = dd.getDate();
            return dd.Format("yyyy-MM-dd");
            // return y + "-" + m + "-" + d;
        },

        /**
         * 获取当前月的第一天
         */
        getCurrentMonthFirst: function () {
            var date = new Date();
            date.setDate(1);
            return date;
        },
        /**
         * 获取当前月的最后一天
         */
        getCurrentMonthLast: function () {
            var date = new Date();
            var currentMonth = date.getMonth();
            var nextMonth = ++currentMonth;
            var nextMonthFirstDay = new Date(date.getFullYear(), nextMonth, 1);
            var oneDay = 1000 * 60 * 60 * 24;
            return new Date(nextMonthFirstDay - oneDay);
        }

    }


    JSPublic.Utils = {
        toInt: function (val) {
            var i = parseInt(val);
            if (isNaN(i)) {
                return 0;
            }
            else {
                return i;
            }
        },
        toFloat: function (val) {
            var i = parseFloat(val);
            if (isNaN(i)) {
                return 0;
            }
            else {
                return i;
            }
        }
    }



})();