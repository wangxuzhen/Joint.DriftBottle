
var WXJsApi = {
    shareUrl: "",//分享地址

    //提交
    init: function (initUrl, storeID, postUrl) {

        if (postUrl == undefined || postUrl == "") {
            postUrl = "../Coupon/GetWxConfig";
        }

        $.post(postUrl, { shareUrl: initUrl, storeID: storeID }, function (result) {
            if (result) {
                shareUrl = result.shareUrl;
                wx.config({
                    debug: false,

                    appId: result.appID,

                    timestamp: result.timeStamp,

                    nonceStr: result.nonceStr,

                    signature: result.signature.toLowerCase(),

                    jsApiList: [

                      'checkJsApi',

                      'onMenuShareTimeline',

                      'onMenuShareAppMessage',

                      'onMenuShareQQ',

                      'onMenuShareWeibo',

                      'hideMenuItems',

                      'showMenuItems',

                      'hideAllNonBaseMenuItem',

                      'showAllNonBaseMenuItem',

                      'translateVoice',

                      'startRecord',

                      'stopRecord',

                      'onRecordEnd',

                      'playVoice',

                      'pauseVoice',

                      'stopVoice',

                      'uploadVoice',

                      'downloadVoice',

                      'chooseImage',

                      'previewImage',

                      'uploadImage',

                      'downloadImage',

                      'getNetworkType',

                      'openLocation',

                      'getLocation',

                      'hideOptionMenu',

                      'showOptionMenu',

                      'closeWindow',

                      'scanQRCode',

                      'chooseWXPay',

                      'openProductSpecificView',

                      'addCard',

                      'chooseCard',

                      'openCard',

                      'openAddress'

                    ]

                });

            } else {

            }
        }, 'json');
    },
    //微信分享动作
    share: function (title, desc, link, imgurl, sucess, cancel) {
        wx.ready(function () {

            //分享到朋友圈
            wx.onMenuShareTimeline({
                title: title, // 分享标题
                link: link, // 分享链接
                imgUrl: imgurl, // 分享图标
                success: function () {
                    // 用户确认分享后执行的回调函数
                    sucess();
                },
                cancel: function () {
                    // 用户取消分享后执行的回调函数
                    cancel();
                }
            });

            //分享给朋友
            wx.onMenuShareAppMessage({
                title: title, // 分享标题
                desc: desc, // 分享描述
                link: link, // 分享链接
                imgUrl: imgurl, // 分享图标
                type: '', // 分享类型,music、video或link，不填默认为link
                dataUrl: '', // 如果type是music或video，则要提供数据链接，默认为空
                success: function () {
                    // 用户确认分享后执行的回调函数

                    sucess();
                },
                cancel: function () {
                    // 用户取消分享后执行的回调函数

                    cancel();
                }
            });

            //分享到QQ
            wx.onMenuShareQQ({
                title: title, // 分享标题
                desc: desc, // 分享描述
                link: link, // 分享链接
                imgUrl: imgurl, // 分享图标
                success: function () {
                    // 用户确认分享后执行的回调函数
                    sucess();
                },
                cancel: function () {
                    // 用户取消分享后执行的回调函数
                    cancel();
                }
            });

            //分享到腾讯微博
            wx.onMenuShareWeibo({
                title: title, // 分享标题
                desc: desc, // 分享描述
                link: link, // 分享链接
                imgUrl: imgurl, // 分享图标
                success: function () {
                    // 用户确认分享后执行的回调函数
                    sucess();
                },
                cancel: function () {
                    // 用户取消分享后执行的回调函数
                    cancel();
                }
            });

            //分享到QQ空间
            wx.onMenuShareQZone({
                title: title, // 分享标题
                desc: desc, // 分享描述
                link: link, // 分享链接
                imgUrl: imgurl, // 分享图标
                success: function () {
                    // 用户确认分享后执行的回调函数
                    sucess();
                },
                cancel: function () {
                    // 用户取消分享后执行的回调函数
                    cancel();
                }
            });
        });

        wx.error(function (res) {
            //alert(res);
            //alert(res);
            // config信息验证失败会执行error函数，如签名过期导致验证失败，具体错误信息可以打开config的debug模式查看，也可以在返回的res参数中查看，对于SPA可以在这里更新签名。

        });
    }
}






