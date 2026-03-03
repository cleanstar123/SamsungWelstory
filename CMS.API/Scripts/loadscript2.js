var result = null;
var tmp = [];

location.search
    .substr(1)
    .split("&")
    .forEach(function (item) {
        tmp = item.split("=");
        if (tmp[0] === 'type') result = decodeURIComponent(tmp[1]);
    });

// GET 방식으로 TYPE 파라메터가 없을 경우 디스플레이에서 서비스중이라는 의미
if (!result) {

    var s  = document.createElement("script");
    s.type = "text/javascript";
    s.src  = '/Scripts/satisfaction2.js?' + Date.now();

    $("body").append(s);
}
else {
    $('#layer').css('display', 'block');
}