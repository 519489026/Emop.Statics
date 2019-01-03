var ip;
var address = "";
var postUrl = "http://localhost:9710/tj/save";

$.getScript('http://pv.sohu.com/cityjson?ie=utf-8', function () {

    ip = returnCitySN.cip;

    $.getScript('http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=js&ip=' + ip, function () {
        address += remote_ip_info.country + ";";
        address += remote_ip_info.province + ";";
        address += remote_ip_info.city;
        if (getCookie("guid") == null) {
            setCookie("guid", newGuid());
        }

        AddScranQrCode();
        ListenButton(getScriptArg("b"));

    });
    //AddScranQrCode();
    //ListenButton(getScriptArg("b"));
});

var browser = {
    versions: function () {
        var u = navigator.userAgent.toLowerCase(), app = navigator.appVersion;
        return {
            //移动终端浏览器版本信息
            trident: u.indexOf('trident') > -1, //IE内核
            presto: u.indexOf('presto') > -1, //opera内核
            webKit: u.indexOf('applewebkit') > -1, //苹果、谷歌内核
            gecko: u.indexOf('gecko') > -1 && u.indexOf('khtml') == -1, //火狐内核
            mobile: !!u.match(/applewebkit.*mobile.*/) || !!u.match(/applewebkit/), //是否为移动终端
            ios: !!u.match(/\(i[^;]+;( u;)? cpu.+mac os x/), //ios终端
            android: u.indexOf('android') > -1 || u.indexOf('linux') > -1, //android终端或者uc浏览器
            iPhone: u.indexOf('iphone') > -1 || u.indexOf('mac') > -1, //是否为iPhone或者QQHD浏览器
            iPad: u.indexOf('ipad') > -1, //是否iPad
            webApp: u.indexOf('safari') == -1, //是否web应该程序，没有头部与底部
            weixin: u.indexOf("micromessenger") > -1,
            weibo: u.indexOf("weibo") > -1
        };
    }(),
    language: (navigator.browserLanguage || navigator.language).toLowerCase()
}

var mobile;
if (browser.versions.ios) {
    mobile = 1;
} else if (browser.versions.android) {
    mobile = 0;
} else {
    mobile = 2;
}


function getScriptArg(key) {//获取单个参数
    var script = document.getElementById("tj"), src = script.src;

    return (src.match(new RegExp("(?:\\?|&)" + key + "=(.*?)(?=&|$)")) || ['', null])[1];
};

//记录扫码信息
function AddScranQrCode() {
    $.ajax({
        url: postUrl,
        type: "POST",
        data: {
            qrCode: getScriptArg("q"),
            phoneType: mobile,
            ip: ip,
            province: address,
            enterType: "1",
            CilentCookie: getCookie("guid")
        },
        success: function (result) {
            if (result === "ok") {
                console.log("扫码记录成功");
            } else {
                console.error("扫码记录失败");
            }
        }
    });
}

function ListenButton(btnId) {
    var btn = document.getElementById(btnId);
    if (btn == null) {
        return;
    }
    btn.addEventListener("click", saveClickNums);
}

//记录点击次数
function saveClickNums() {
    $.ajax({
        url: postUrl,
        type: "POST",
        data: {
            qrCode: getScriptArg("q"),
            phoneType: mobile,
            ip: ip,
            province: address,
            enterType: "3",
            CilentCookie: getCookie("guid")
        },
        success: function (result) {
            if (result === "ok") {
                console.log("点击下载记录成功");
            } else {
                console.error("点击下载记录失败");
            }
        }
    });
}

/*
    创建cookie 
*/
function setCookie(name, value) {
    var days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

//读取cookie
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}

function newGuid() {
    var guid = "";
    for (var i = 1; i <= 32; i++) {
        var n = Math.floor(Math.random() * 16.0).toString(16);
        guid += n;
        if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
            guid += "-";
    }
    return guid;
}