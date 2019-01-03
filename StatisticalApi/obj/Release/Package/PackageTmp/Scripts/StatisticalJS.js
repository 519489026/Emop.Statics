AjaxLoadJquerylibrary();

function AjaxLoadJquerylibrary() {
    if (!window.jQuery) {
        console.log("插入jquery库");
        var hm = document.createElement("script");
        hm.src = "http://cdn.bootcss.com/jquery/1.11.1/jquery.min.js";
        var head = document.getElementsByTagName("script")[0];
        head.parentNode.insertBefore(hm, head);
    }
    var btnId = getScriptArg("b");
    var qrCode = getScriptArg("q");

    if (btnId === "" || qrCode === "" || btnId === undefined || qrCode === undefined) {
        return;
    }

    var hm2 = document.createElement("script");

    hm2.src = "http://localhost:9710/scripts/DoStatistical.js?b=" + btnId + "&q=" + qrCode;

    hm2.id = "tj2";
    var s = document.getElementsByTagName("script")[0];
    s.parentNode.insertBefore(hm2, s);
}

function getScriptArg(key) {//获取单个参数
    var script = document.getElementById("tj"), src = script.src;

    return (src.match(new RegExp("(?:\\?|&)" + key + "=(.*?)(?=&|$)")) || ['', null])[1];
};