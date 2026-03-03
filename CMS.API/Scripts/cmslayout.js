// 컨텐츠 영역의 메뉴 정보
function ProductContentsArea() {
    this.contentsAreaId = '';
    this.image          = false;
    this.name           = false;
    this.ename          = false;
    this.price          = false;
    this.sidedish       = false;
    this.kcal           = false;
    this.score1         = false;
    this.score2         = false;
    this.score3         = false;
    this.score4         = false;
    this.score5         = false;
}

// 컨텐츠 영역 정보
function ContentsArea() {
    this.screenW      = 0;
    this.screenH      = 0;
    this.layoutHByV   = '';
    this.layoutHVType = '';
    this.evalUseYn    = 'N';
    this.contentsCnt  = 0;
    this.product      = [];
    this.badge        = [];
    this.head         = [];
    this.image        = [];
    this.text         = [];
    this.movie        = [];
    this.url          = [];
    this.date         = [];   /* 2018.09.06 추가 */

    this.CONTENTSTYPES = {
        product: [],
        badge  : [],
        head   : [],
        image  : [],
        text   : [],
        movie  : [],
        url    : [],
        date   : []
    }
}

var contentsAreaInfo = {};

/**
 * 브라우저 상에서 레이아웃(.html)파일을 읽어 text 형식으로 리턴 
 */
function readLayoutFile(eId, targetId, callback) {
    var files = document.getElementById(eId).files;

    if (!files.length) {
        return;
    }

    var file = files[0];
    var reader = new FileReader(); // IE10 이상

    reader.onloadend = function (e) {
        if (e.target.readyState == FileReader.DONE) {
            $('#' + targetId).html(e.target.result);
            contentsAreaInfo = getContentsArea();

            callback();
        }
    }

    reader.readAsText(file);
}

/**
 * 컨텐츠 영역의 아이디와 타입 목록을 MAP형식(아이디=타입)으로 리턴
 */
function getContentsAreaIdAndTyps() {
    var arrContentsArea = $('[data-contents-area-id]');    // data-contents-area-id 속성을 가지고있는 모든 요소를 찾음
    var map = {};

    for (var i = 0; i < arrContentsArea.length; i++) {
        map[$(arrContentsArea[i]).data("contents-area-id")] = $(arrContentsArea[i]).data("contents-area-type");
    }

    return map;
}

/**
 * 컨텐츠 영역 정보를 리턴
 */
function getContentsArea() {

    var contentsArea = new ContentsArea();
    var map = getContentsAreaIdAndTyps();

    contentsArea.contentsCnt = Object.size(map);

    for (var key in map) {

        if (map[key].toLowerCase() == 'product') {

            var productContentsArea = new ProductContentsArea();
            productContentsArea.contentsAreaId = key;

            $('[data-contents-area-id="' + key + '"][data-product-category]').each(function () {

                productContentsArea[$(this).data('product-category')] = true;
            });

            contentsArea.CONTENTSTYPES[map[key].toLowerCase()].push(productContentsArea);
        }
        else {
            contentsArea.CONTENTSTYPES[map[key].toLowerCase()].push(key);
        }
    }

    contentsArea.layoutHByV   = $('#LAYOUT_H_BY_V').val();
    contentsArea.layoutHVType = $('#LAYOUT_HV_TYPE').val();
    contentsArea.screenW      = $('#SCREEN_X').val();
    contentsArea.screenH      = $('#SCREEN_Y').val();
    contentsArea.evalUseYn    = ($('[data-product-category*="score"]').length > 0 ? 'Y' : 'N');

    console.log(contentsArea);

    return contentsArea;
}