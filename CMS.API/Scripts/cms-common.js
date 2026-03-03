/**
 * 공통 메세지 정의
 */

function resultMessage(type, callback) {

    // console.log(type, callback);

    var comment = '실행';

    if (type.toUpperCase() == 'I')
        comment = '생성';
    else if (type.toUpperCase() == 'U')
        comment = '수정';
    else if (type.toUpperCase() == 'D')
        comment = '삭제';

    alert('정상적으로 ' + comment + ' 되었습니다.');

    if (callback)
        callback();
}

var deleteConfirmMessage = '삭제 하시겠습니까?';
var noteSelectedRowMessage = '행이 선택되지 않았습니다.';


/*
 * jquery ajax
 */

function simpleAjax(url, data, asnyc, disableLoading) {

    var request = $.ajax({
        url: url,
        method: "POST",
        dataType: "json",
        async: asnyc === 'undefinde' ? true : asnyc,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data),
        beforeSend: function () {

            if (!disableLoading) {
                $('#loadingbar').show();
            }
            
        }
    });

    request.fail(function(jqXHR, textStatus) {
        alert("요청 실패: " + textStatus);
        return false;
    });

    request.always(function () {
        $('#loadingbar').hide();
    });

    return request
}

function simplefileUploadAjax(url, formData) {

    var request = $.ajax({
        url: url,
        method: "POST",
        contentType: false,
        processData: false,
        data: formData,
        beforeSend: function () {
            $('#loadingbar').show();
        }
    });

    request.fail(function (jqXHR, textStatus) {
        alert("요청 실패: " + textStatus);
        return false;
    });

    request.always(function () {
        $('#loadingbar').hide();
    });

    return request
}

function isEmpty(obj) {
    for (var prop in obj) {
        if (obj.hasOwnProperty(prop))
            return false;
    }

    return JSON.stringify(obj) === JSON.stringify({});
}


/*
 * jqgrid 관련 공통 함수 
 */

// jqgrid row 선택전 이벤트에 삽입하여 체크박스 cell 선택시 true, 아니면 false를 리턴
function checkboxSelect(rowid, e) {
    var $myGrid = $(this);
    var i = $.jgrid.getCellIndex($(e.target).closest('td')[0]);
    var cm = $myGrid.jqGrid('getGridParam', 'colModel');

    return (cm[i].name === 'cb');
}

function jqgridNewRowId(jqgrid) {

    var ids = jqgrid.jqGrid('getDataIDs');

    return (ids.length <= 0 ? 0 : findMaxValue(jqgrid.jqGrid('getDataIDs'))) * 1 + 1;
}

function findMaxValue(arr) {

    var max = arr.reduce(function (a, b) {
        return Math.max(a, b);
    });

    return max;
}

/**
 * JQGRID 의 행을 옴겨주는 함수
 * @param {any} jqgridSource  행을 제공하는 그리드
 * @param {any} jqgridTarget  행을 제공받는 그리드
 * @param {any} isRemove      행을 제공하는 그리드의 행을 삭제할지 여부 (true : 삭제, false : 삭제안함)
 */
function jqgridMoveRows(jqgridSource, jqgridTarget, isRemove) {

    var selarrrow = jqgridSource.jqGrid('getGridParam', 'selarrrow');

    for (var i = 0; i < selarrrow.length; i++) {
        var rowdata = jqgridSource.jqGrid('getRowData', selarrrow[i]);

        var isAdd = true;

        if (true) {
            var targetData = jqgridTarget.getRowData();

            for (var j = 0; j < targetData.length; j++) {

                if (rowdata.DISPLAY_ID == targetData[j].DISPLAY_ID) {
                    isAdd = false;
                    break;
                }
            }
        }

        if (isAdd) {
            jqgridTarget.jqGrid("addRowData", jqgridNewRowId(jqgridTarget), rowdata, 'last');
        }
    }
    

    if (isRemove) {
        for (var i = selarrrow.length; i >= 0 ; i--) {
            jqgridSource.delRowData(selarrrow[i]);
        }
    }
}



/**
 * Video에서 현재 플레이 되고 있는 화면을 Canvas에 캡처하는 함수
 * @param {any} video HTML5 Video 요소
 * @param {any} thumbnailCanvas Canvas 요소
 * @param {any} scaleFactor 비율
 */
function capture(videoId, thumbnailCanvasId, scaleFactor) {

    if (scaleFactor == null) {
        scaleFactor = 1;
    }

    var video  = document.getElementById(videoId);            // 비디오 요소
    var canvas = document.getElementById(thumbnailCanvasId);  // 캔버스 요소

    var per = 400 / video.videoWidth;

    // console.log(video.videoWidth, per);

    var w = video.videoWidth  * per;
    var h = video.videoHeight * per;

    console.log(video.videoWidth, video.videoHeight, w, h)
    
    canvas.width  = w;
    canvas.height = h;
    var ctx = canvas.getContext('2d');
    ctx.drawImage(video, 0, 0, w, h);

    return canvas;
}


function resample_single(canvas, width, height, resize_canvas) {
    var width_source  = canvas.width;
    var height_source = canvas.height;

    width  = Math.round(width);
    height = Math.round(height);

    var ratio_w = width_source / width;
    var ratio_h = height_source / height;
    var ratio_w_half = Math.ceil(ratio_w / 2);
    var ratio_h_half = Math.ceil(ratio_h / 2);

    var ctx   = canvas.getContext("2d");
    var img   = ctx.getImageData(0, 0, width_source, height_source);
    var img2  = ctx.createImageData(width, height);
    var data  = img.data;
    var data2 = img2.data;

    for (var j = 0; j < height; j++) {
        for (var i = 0; i < width; i++) {
            var x2 = (i + j * width) * 4;
            var weight = 0;
            var weights = 0;
            var weights_alpha = 0;
            var gx_r = 0;
            var gx_g = 0;
            var gx_b = 0;
            var gx_a = 0;
            var center_y = (j + 0.5) * ratio_h;
            var yy_start = Math.floor(j * ratio_h);
            var yy_stop = Math.ceil((j + 1) * ratio_h);
            for (var yy = yy_start; yy < yy_stop; yy++) {
                var dy = Math.abs(center_y - (yy + 0.5)) / ratio_h_half;
                var center_x = (i + 0.5) * ratio_w;
                var w0 = dy * dy; //pre-calc part of w
                var xx_start = Math.floor(i * ratio_w);
                var xx_stop = Math.ceil((i + 1) * ratio_w);
                for (var xx = xx_start; xx < xx_stop; xx++) {
                    var dx = Math.abs(center_x - (xx + 0.5)) / ratio_w_half;
                    var w = Math.sqrt(w0 + dx * dx);
                    if (w >= 1) {
                        //pixel too far
                        continue;
                    }
                    //hermite filter
                    weight = 2 * w * w * w - 3 * w * w + 1;
                    var pos_x = 4 * (xx + yy * width_source);
                    //alpha
                    gx_a += weight * data[pos_x + 3];
                    weights_alpha += weight;
                    //colors
                    if (data[pos_x + 3] < 255)
                        weight = weight * data[pos_x + 3] / 250;
                    gx_r += weight * data[pos_x];
                    gx_g += weight * data[pos_x + 1];
                    gx_b += weight * data[pos_x + 2];
                    weights += weight;
                }
            }
            data2[x2] = gx_r / weights;
            data2[x2 + 1] = gx_g / weights;
            data2[x2 + 2] = gx_b / weights;
            data2[x2 + 3] = gx_a / weights_alpha;
        }
    }
    //clear and resize canvas
    if (resize_canvas === true) {
        canvas.width = width;
        canvas.height = height;
    } else {
        ctx.clearRect(0, 0, width_source, height_source);
    }

    //draw
    ctx.putImageData(img2, 0, 0);

    return canvas;
}



// 현재 브라우저가 IE 인지 체크
function isIE() {
    var agent = navigator.userAgent.toLowerCase();

    if ((navigator.appName == 'Netscape' && agent.indexOf('trident') != -1) || (agent.indexOf("msie") != -1)) 
        return true;
    else 
        return false;
}

// Object 프로퍼티 갯수 구하는 함수
Object.size = function (obj) {
    var size = 0, key;
    for (key in obj) {
        if (obj.hasOwnProperty(key)) size++;
    }
    return size;
};

// 앞에 0 채우기
function pad(n, width) {
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join('0') + n;
}

// Sleep
function sleep(foo, millisec) {
    
    var startTime = new Date().getTime();
    while (new Date().getTime() < startTime + millisec);
    foo();
}

function findGetParameter(parameterName) {
    var result = null,
        tmp = [];
    location.search
        .substr(1)
        .split("&")
        .forEach(function (item) {
            tmp = item.split("=");
            if (tmp[0] === parameterName) result = decodeURIComponent(tmp[1]);
        });
    return result;
}

function nowDate() {
    var date = new Date();
    var nowDate = date.getFullYear() + '-' + pad(date.getMonth() + 1, 2) + '-' + pad(date.getDate(), 2);

    console.log(nowDate);

    return nowDate;
}

function contentsTypeConvert(contetnsType) {

    var rtnContentsType = '';

    switch (contetnsType.toUpperCase()) {
        case 'I':
            rtnContentsType = 'image';
            break;
        case 'B':
            rtnContentsType = 'badge';
            break;
        case 'H':
            rtnContentsType = 'head';
            break;
        case 'P':
            rtnContentsType = 'product';
            break;
        case 'M':
            rtnContentsType = 'movie';
            break;
        case 'U':
            rtnContentsType = 'url';
            break;
        case 'T':
            rtnContentsType = 'text';
            break;
    }

    return rtnContentsType;
}

/************************************************************************************************************************************************/
/************************************************************ CMS 공통 기능 *********************************************************************/
/************************************************************************************************************************************************/


/*********************************/
/* 변수 선언 및 클래스 정의 시작 */

//// 컨텐츠 모델
//function ContentsModel() {
//    this.RESTAURANT_CODE = '';
//    this.CONTENT_ID      = '';
//    this.CONTENT_NM      = '';
//    this.CONTENT_TYPE    = '';
//    this.TEXT_HTML       = '';
//    this.FILE_URL        = '';
//    this.ORGIN_FILE_URL  = '';
//    this.THUMBNAIL_URL   = '';    // Movie 타입의 경우 Base64 이미지 URL 할당
//    this.VIDEO_DURATION  = '';
//    this.PAGE_CNT        = '15';
//    this.PAGE_NO         = '';
//    this.FILE_PATH       = '';
//    this.FILE_NM         = '';
//    this.THUMBNAIL_PATH  = '';
//    this.THUMBNAIL_NM    = '';
//    this.IS_FILE_UPDATE  = false;
//};



///* 변수 선언 및 클래스 정의 끝   */
///*********************************/


///*********************************/
///****** CRUD 함수 정의 시작 ******/

///**
// * 컨텐츠 조회
// * @param {ContentsModel} contentsModel 조회할 컨텐츠명과 타입
// * @param {function} successCallback    성공시 실행할 콜백 함수
// */
//function searchContents(contentsModel, successCallback) {

//    simpleAjax(urlContentsList, { contentsModel: contentsModel }).done(function (data) {
//        if (successCallback) 
//            successCallback(data);
//    });
//}

///**
// * 컨텐츠 저장 (파일 저장 포함)
// * @param {FormData} params          ContentsModel을 포함한 컨텐츠 정보
// * @param {function} successCallback 성공시 실행할 콜백 함수
// */
//function saveContents(params, successCallback) {

//    if (params.contentsModel.CONTENT_TYPE == 'T' || params.contentsModel.CONTENT_TYPE == 'U') {

//        var requestUrl = '';

//        if (params.contentsModel.CONTENT_TYPE)
//            requestUrl = urlContentsUploadText;
//        else
//            requestUrl = urlContentsUploadUrl;

//        simpleAjax(requestUrl, { contentsModel: params }).done(function (data) {
//            if (successCallback) 
//                successCallback(data);
//        });
//    }
//    else { /* FILE (이미지(헤드라인, 뱃지), 동영상) */
//        simplefileUploadAjax(urlContentsUploadFile, params).done(function (data) {
//            if (successCallback) 
//                successCallback(data);
//        });
//    }
//}

// 디스플레이 그룹 조회
function searchDisplayGroup(params, successCallback) {

    simpleAjax(urlDisplayGroup, { displayGroupModel: params }).done(function (data) {
        if (successCallback) 
            successCallback(data);
    });
}

// 디스플레이 그룹
function searchDisplayMap(displayGroupId, successCallback) {

    var request = simpleAjax(urlDisplayMap, { displayGroupModel: { DISPLAY_GROUP_ID: displayGroupId } }, true, true).done(function (data) {
        if (successCallback) 
            successCallback(data);
    });
}


/******* CRUD 함수 정의 끝 *******/
/*********************************/


var Paging = function (totalCnt, dataSize, pageSize, pageNo, token) {

    totalCnt = parseInt(totalCnt); // 전체레코드수 
    dataSize = parseInt(dataSize); // 페이지당 보여줄 데이타수 
    pageSize = parseInt(pageSize); // 페이지 그룹 범위 1 2 3 5 6 7 8 9 10 
    pageNo   = parseInt(pageNo);   // 현재페이지

    var totalPageCnt = totalCnt / dataSize + totalCnt % dataSize;
    totalPageCnt += '';

    var lastPageNo = totalPageCnt.substring(0, totalPageCnt.length - 1) + '1';

    var html = new Array();
    if (totalCnt == 0) { return ""; } // 페이지 카운트 

    var pageCnt = totalCnt % dataSize;
    if (pageCnt == 0) {
        pageCnt = parseInt(totalCnt / dataSize);
    }
    else {
        pageCnt = parseInt(totalCnt / dataSize) + 1;
    }

    var pRCnt = parseInt(pageNo / pageSize);

    if (pageNo % pageSize == 0) {
        pRCnt = parseInt(pageNo / pageSize) - 1;
    } //이전 화살표 

    if (pageNo > pageSize) {
        var s2;
        if (pageNo % pageSize == 0) {
            s2 = pageNo - pageSize;
        }
        else {
            s2 = pageNo - pageNo % pageSize;
        }

        html.push('<li class="paging_btn"><img src="../images/sub/prev_btn_2.png" alt="" class="ico_btn" onclick="' + token + '(1)"></li>');
        html.push('<li class="paging_btn"><img src="../images/sub/prev_btn_1.png" alt="" class="ico_btn" onclick="' + token + '(' + s2 + ')"></li>');
        
        //html.push('<a href=javascript:' + token + '("');
        //html.push(s2); html.push('");>');
        //html.push('◀');
        //html.push("</a>");
    }
    else {
        html.push('<li class="paging_btn"><img src="../images/sub/prev_btn_2.png" alt="" class="ico_btn" onclick="' + token + '(1)"></li>');
        html.push('<li class="paging_btn"><img src="../images/sub/prev_btn_1.png" alt="" class="ico_btn"></li>');
        //html.push('<a href="#">\n'); html.push('◀'); html.push('</a>');
    } //paging Bar 

    for (var index = pRCnt * pageSize + 1; index < (pRCnt + 1) * pageSize + 1; index++) {

        // 현재 페이지 효과
        if (index == pageNo) {
            //html.push('<strong>');
            //html.push(index);
            //html.push('</strong>');
            html.push('<li style="color: #1078c3; font-weight: bold;">' + index + '</li>');
        }   //'color': '#1078c3', 'font-weight': 'bold'
        else {

            html.push('<li onclick="' + token + '(' + "'" + index + "'" + ')">' + index + '</li>');

        //html.push(index);
        //html.push("'" + '"' + ")>");
        //html.push(index);
        //html.push('</li>');
        }

        if (index == pageCnt) {
            break;
        }
        //else
            //html.push('|');

    } //다음 화살표 

    if (pageCnt > (pRCnt + 1) * pageSize) {

        html.push('<li class="paging_btn"><img src="../images/sub/next_btn_1.png" alt="" class="ico_btn" onclick="' + token + '(' + ((pRCnt + 1) * pageSize + 1) +')"></li>');
        html.push('<li class="paging_btn"><img src="../images/sub/next_btn_2.png" alt="" class="ico_btn" onclick="' + token + '(' + lastPageNo + ')"></li>');

        //html.push('<a href=javascript:' + token + '("');
        //html.push((pRCnt + 1) * pageSize + 1);
        //html.push('");>');
        //html.push('▶');
        //html.push('</a>');
    }
    else {

        html.push('<li class="paging_btn"><img src="../images/sub/next_btn_1.png" alt="" class="ico_btn"></li>');
        html.push('<li class="paging_btn"><img src="../images/sub/next_btn_2.png" alt="" class="ico_btn"></li>');

        //html.push('<a href="#">');
        //html.push('▶');
        //html.push('</a>');
    }

    return html.join("");
}
    