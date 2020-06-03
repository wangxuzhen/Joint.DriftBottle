var ValidatorExpand = ValidatorExpand || {};

(function () {

    /*输入框不同状态，显示图片的样式*/
    ValidatorExpand.FeedbackIcons = {
        valid: 'glyphicon glyphicon-ok',
        invalid: 'glyphicon glyphicon-remove',
        validating: 'glyphicon glyphicon-refresh'

    };

    /*不允许为空*/
    ValidatorExpand.NotEmpty = function (min, max, message) {
        var json;
        var msg = message != undefined ? message : '不能为空';
        if (max > min) {
            json = {
                validators: {
                    notEmpty: {
                        message: msg
                    },
                    stringLength: {/*长度提示*/
                        min: min,
                        max: max,
                        message: '长度必须在' + min + '到' + max + '字符之间'
                    }
                }
            };
        }
        else {
            json = {
                validators: {
                    notEmpty: {
                        message: msg
                    }
                }
            };
        }

        return json;
    };

    /*验证字符长度*/
    ValidatorExpand.StringLength = function (min, max) {
        var msg;
        if (max > min) {
            msg = '长度必须在' + min + '到' + max + '字符之间';
        }
        else {
            msg = '系统验证参数错误';
        }
        var json = {
            validators: {
                stringLength: {
                    min: min,
                    max: max,
                    message: msg
                }
            }
        };
        return json;
    }


    /*密码不允许为空*/
    ValidatorExpand.Password = {
        validators: {
            notEmpty: {
                message: '密码不能为空'
            },
            stringLength: {/*长度提示*/
                min: 6,
                max: 32,
                message: '长度必须在6到32字符之间'
            }/*最后一个没有逗号*/
        }
    };

    /*验证邮箱*/
    ValidatorExpand.Email = {
        validators: {
            notEmpty: {
                message: '邮箱不能为空'
            },
            emailAddress: {
                message: '邮箱地址格式有误'
            }
        }
    };

    /*验证手机*/
    ValidatorExpand.Phone = {
        validators: {
            notEmpty: {
                message: '手机不能为空'
            },
            regexp: {
                regexp: /^(0|86|17951)?(13[0-9]|15[012356789]|17[0678]|18[0-9]|14[57])[0-9]{8}$/,
                message: '手机格式有误'
            }
        }
    };

    /*验证电话*/
    ValidatorExpand.TEL = {
        validators: {
            regexp: {
                regexp: /^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$|(^(1[0-9][0-9]|15[0|3|6|7|8|9]|18[8|9])\d{8}$)/,
                message: '电话格式有误'
            }
        }
    };

    /*验证数值*/
    ValidatorExpand.NumericNotEmpty = {
        validators: {
            notEmpty: {
                message: '请输入数字'
            },
            numeric: {
                message: '输入数字格式有误'
            }
        }
    };

    /*验证数值*/
    ValidatorExpand.Numeric = {
        validators: {
            numeric: {
                message: '输入数字格式有误'
            }
        }
    };

    /*验证整数*/
    ValidatorExpand.Integer = {
        validators: {
            integer: {
                message: '整数格式有误'
            }
        }
    };

    /*验证整数*/
    ValidatorExpand.IntegerNotEmpty = {
        validators: {
            notEmpty: {
                message: '请输入数字'
            },
            integer: {
                message: '整数格式有误'
            }
        }
    };

    /*验证URL*/
    ValidatorExpand.URL = {
        validators: {
            uri: {
                message: '网址格式有误'
            }
        }
    };

    /*验证身份证号码*/
    ValidatorExpand.Idcard = {
        validators: {
            regexp: {
                regexp: /^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$/,
                message: '身份证号码有误'
            }
        }
    };

    /*验证邮编*/
    ValidatorExpand.ZipCode = {
        validators: {
            zipCode: {
                message: '邮编格式有误'
            }
        }
    };




})();
