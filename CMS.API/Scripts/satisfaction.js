

/* 메뉴 평가  */
function WelmenuModel() {
    this.AREA_ID = '';
    this.RESTAURANT_CODE = '';
    this.MENU_DT = '';
    this.MEAL_TYPE = '';
    this.HALL_NO = '';
    this.MENU_TYPE = '';
    this.COURSE_TYPE = '';
    this.MENU_CODE = '';
}


/**
 * 메뉴 평가 조회
 * @param {any} url
 * @param {any} params
 */
function satisfactionList(params) {
    var xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        if (xhttp.status === 200 || xhttp.status === 201) {

            var data = JSON.parse(xhttp.responseText).Table;
            var localRankCnt = 0;
            var drRankCnt = 0;
            var isChanged = false;

            console.log(data);
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    isChanged = false;
                    var score5cnt = $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score1"]').text() * 1;
                    var score4cnt = $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score2"]').text() * 1;
                    var score3cnt = $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score3"]').text() * 1;
                    var score2cnt = $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score4"]').text() * 1;
                    var score1cnt = $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score5"]').text() * 1;

                    var floor_no = data[i].FLOOR_NO
                    
                    localRankCnt = localRankCnt + score5cnt + score4cnt + score3cnt + score2cnt + score1cnt;
                    drRankCnt = drRankCnt + data[i].EVAL_SCORE5_CNT + data[i].EVAL_SCORE4_CNT + data[i].EVAL_SCORE3_CNT + data[i].EVAL_SCORE2_CNT + data[i].EVAL_SCORE1_CNT;

                    if (score5cnt < data[i].EVAL_SCORE5_CNT) {

                        $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score1"]').text(data[i].EVAL_SCORE5_CNT);
                        score5cnt = data[i].EVAL_SCORE5_CNT;
                        isChanged = true;
                    }
                    if (score4cnt < data[i].EVAL_SCORE4_CNT) {
                        $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score2"]').text(data[i].EVAL_SCORE4_CNT);
                        score4cnt = data[i].EVAL_SCORE4_CNT;
                        isChanged = true;
                    }
                    if (score3cnt < data[i].EVAL_SCORE3_CNT) {
                        $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score3"]').text(data[i].EVAL_SCORE3_CNT);
                        score3cnt = data[i].EVAL_SCORE3_CNT;
                        isChanged = true;
                    }
                    if (score2cnt < data[i].EVAL_SCORE2_CNT) {
                        $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score4"]').text(data[i].EVAL_SCORE2_CNT);
                        score2cnt = data[i].EVAL_SCORE2_CNT;
                        isChanged = true;
                    }
                    if (score1cnt < data[i].EVAL_SCORE1_CNT) {
                        $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="score5"]').text(data[i].EVAL_SCORE1_CNT);
                        score1cnt = data[i].EVAL_SCORE1_CNT;
                        isChanged = true;
                    }

                    if ($('.ranking').length > 0 && isChanged == true) {
                        var cnt = score1cnt + score2cnt + score3cnt + score4cnt + score5cnt;
                        var path = $('[data-contents-area-id="' + data[i].AREA_ID).parents('.menuList');

                        if (floor_no == null || floor_no == '' || !$("#layer").attr("class")) {
                            path.trigger('click');
                        } else {
                            if ($("#layer").attr("class") == floor_no) {
                                path.trigger('click');
                            }
                        }

                        //path.trigger('click');

                        path.find('.bar').eq(0).css('width', score5cnt / cnt * 100 + '%');
                        path.find('.bar').eq(1).css('width', score4cnt / cnt * 100 + '%');
                        path.find('.bar').eq(2).css('width', score3cnt / cnt * 100 + '%');
                        path.find('.bar').eq(3).css('width', score2cnt / cnt * 100 + '%');
                        path.find('.bar').eq(4).css('width', score1cnt / cnt * 100 + '%');
                    }
                }

                //// 2019-04-24 실시간 선호메뉴
                //if ($('.ranking').length > 0) {
                //    var rank = 1;

                //    for (var i = 0; i < data.length; i++) {

                //        if (data[i].EVAL_SCORE1_CNT != 0 || data[i].EVAL_SCORE2_CNT != 0 || data[i].EVAL_SCORE3_CNT != 0 || data[i].EVAL_SCORE4_CNT != 0 || data[i].EVAL_SCORE5_CNT != 0) {
                //            $('.ranking li span').eq(rank - 1).text(rank + ". ");
                //            $('.ranking li p').eq(rank - 1).text($('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="name"]').text());
                //            rank = rank + 1;
                //        }
                //    }
                //}

                // 2019-04-24 실시간 선호메뉴
                if ($('.ranking').length > 0) {
                    if (localRankCnt <= drRankCnt) {
                        var rank = 0;
                        for (var i = 0; i < data.length; i++) {
                            var totalCount = data[i].TOTALCOUNT;

                            if (totalCount > 0) {
                                if (i > 0) {
                                    if (totalCount != data[(i - 1)].TOTALCOUNT) {
                                        rank = rank + 1;
                                    }
                                }
                                else {
                                    rank = rank + 1;
                                }

                                $('.ranking_li').eq(i).text(rank + ". " + $('[data-contents-area-id="' + data[i].AREA_ID + '"][data-product-category="name"]').text());
                            }
                        }
                    }
                }
            }
            //else {
            //    $('[data-product-category*="score"]').each(function () {
            //        $(this).text(0);
            //    });
            //}
        }
        else {
            console.log('fail');
        }
    };

    xhttp.open('POST', location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + '/Welmenu/getSatisfactionlist', true);
    xhttp.setRequestHeader('Content-Type', 'application/json');
    xhttp.send(JSON.stringify({ welmenuModels: params }));
}

/**
 * 메뉴 평가 하기
 * @param {any} params
 */
var timer;
function setSatisfaction(params) {

    if (timer) clearInterval(timer);
    timer = setTimeout(function () {
        var xhttp = new XMLHttpRequest();

        xhttp.onload = function () {
            if (xhttp.status === 200 || xhttp.status === 201) {
                var data = JSON.parse(xhttp.responseText).Table;
                isSetsatisfaction = false;
            }
            else {
                console.log('fail');
            }
        };

        switch (params.EVAL_SCORE) {
            case '1':
                params.EVAL_SCORE = '5';
                break;
            case '2':
                params.EVAL_SCORE = '4';
                break;
            case '3':
                params.EVAL_SCORE = '3';
                break;
            case '4':
                params.EVAL_SCORE = '2';
                break;
            case '5':
                params.EVAL_SCORE = '1';
                break;
        }

        xhttp.open('POST', location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + '/Welmenu/setSatisfaction', true);
        xhttp.setRequestHeader('Content-Type', 'application/json');
        xhttp.send(JSON.stringify({ welmenuModel: params }));
    }, 1500);


}

function setSatisfaction_bak(params) {

    var xhttp = new XMLHttpRequest();

    xhttp.onload = function () {
        if (xhttp.status === 200 || xhttp.status === 201) {
            var data = JSON.parse(xhttp.responseText).Table;
            isSetsatisfaction = false;
        }
        else {
            console.log('fail');
        }
    };

    switch (params.EVAL_SCORE) {
        case '1':
            params.EVAL_SCORE = '5';
            break;
        case '2':
            params.EVAL_SCORE = '4';
            break;
        case '3':
            params.EVAL_SCORE = '3';
            break;
        case '4':
            params.EVAL_SCORE = '2';
            break;
        case '5':
            params.EVAL_SCORE = '1';
            break;
    }

    xhttp.open('POST', location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + '/Welmenu/setSatisfaction', true);
    xhttp.setRequestHeader('Content-Type', 'application/json');
    xhttp.send(JSON.stringify({ welmenuModel: params }));
}


var products = {};
var obj = {};

var isSetsatisfaction = false;

//
$(document).ready(function () {

    $('#layer').fadeIn(2000);

    $('.menuList').click(function () {
        var element = this;
        $(element).addClass('choice fadeIn2');
        setTimeout(function () {
            $(element).removeClass('choice fadeIn2');
        }, 4000);
    });
    $('.satisfaction>div').click(function () {
        var satisfaction = $(this);
        $(satisfaction).delay(4000).addClass('blink');
        setTimeout(function () {
            $(satisfaction).removeClass('blink');
        }, 4000);
    });
});


function checkLoad() {

    $('video').each(function () {

        if ($(this).get(0).readyState == 4) {
            console.log('동영상 데이터 로드 완료');
            $(this).get(0).play();
        }
        else {
            console.log('실패');
            setTimeout(checkLoad, 1000);
        }

    });
}

// 이미지까지 로드 됬을 경우 실행 되는 함수
window.onload = function () {
    var video = document.querySelector("video");
    if (video) {
        video.onloadeddata = function () {
            video.addEventListener('click', function () { video.play(); }, false);
            $('video').trigger('click');
        }
    }


    $('[data-contents-area-type="product"]').each(function () {
        products[$(this).data('contents-area-id')] = {};

        obj[$(this).data('contents-area-id')] = {
            RESTAURANT_CODE: $(this).data('restaurant-code'),
            MENU_DT: $(this).data('menu-dt'),
            MEAL_TYPE: $(this).data('meal-type'),
            MENU_CODE: $(this).data('menu-cd'),
            COURSE_TYPE: $(this).data('course'),
            HALL_NO: $(this).data('hall-no')
        };
    });

    for (var product in products) {

        $('[data-contents-area-id="' + product + '"][data-contents-area-type="product"][data-product-category*="score"]').each(function () {

            var that = this;

            $(this).parent().on('animationstart', function () {
                isSetsatisfaction = true; // 메뉴 평가 중..
            });

            $(this).parent().on('animationend', function () {

                $(this).removeClass('ExpandUp');

                setSatisfaction({
                    RESTAURANT_CODE: $(that).data('restaurant-code'),
                    MENU_DT: $(that).data('menu-dt'),
                    MEAL_TYPE: $(that).data('meal-type'),
                    MENU_CODE: $(that).data('menu-cd'),
                    COURSE_TYPE: $(that).data('course'),
                    HALL_NO: $(that).data('hall-no'),
                    EVAL_SCORE: $(that).data('product-category').substring(5, 6)
                });
            });

            $(this).parent().click(function () {

                // 메뉴 평가중이라면 Return
                if (isSetsatisfaction) {
                    return;
                }

                $(this).addClass('ExpandUp');
                var score = ($(that).text() * 1) + 1
                $(that).text(score);

                // 2019-04-24 메뉴선호도 그래프 추가
                var i = 0;
                var totalScore = 0;

                $(that).parents('.menuList').find('.bar').each(function () {
                    totalScore = totalScore + Number($(that).parents('.menuList').find('.score').eq(i).text());
                    i = i + 1;
                });

                i = 0;
                $(that).parents('.menuList').find('.bar').each(function () {
                    $(that).parents('.menuList').find('.bar').eq(i).css('width', (Number($(that).parents('.menuList').find('.score').eq(i).text() / totalScore * 100)) + '%');
                    i = i + 1;
                });
            });
        });
    }

    var welmenuModels = [];

    for (var id in obj) {

        var welmenuModel = new WelmenuModel();
        
        welmenuModel.AREA_ID = id;
        welmenuModel.RESTAURANT_CODE = obj[id].RESTAURANT_CODE;
        welmenuModel.MENU_DT = obj[id].MENU_DT;
        welmenuModel.MEAL_TYPE = obj[id].MEAL_TYPE;
        welmenuModel.MENU_CODE = obj[id].MENU_CODE;
        welmenuModel.COURSE_TYPE = obj[id].COURSE_TYPE;
        welmenuModel.HALL_NO = obj[id].HALL_NO;

        welmenuModels.push(welmenuModel);
    }

    if ($('[data-product-category*="score"]').length > 0) {

        $('[data-product-category*="score"]').each(function () {
            $(this).text('0');
        });


        var s = document.createElement("link");
        s.rel = 'stylesheet';
        s.type = 'text/css'
        s.href = '/Content/animations.css';

        $("head").append(s);

        // 최초 평가 조회
        satisfactionList(welmenuModels);

        // 1. 1초 마다 메뉴 평가 조회
        this.setInterval(function () {
            satisfactionList(welmenuModels);
            //if ($('.ranking').length > 0) {
            //    $('.menuList').removeClass('choice fadeIn2');
            //    $('.satisfactionType3').removeClass('blink');
            //}
        }, 1000);
    }

    // 날짜 컨텐츠 타입이 있을 경우

    $('[data-contents-area-type="date"]').each(function () {

        var format = ($(this).data('date-format') ? $(this).data('date-format') : '');
        var that = this;

        var request = $.ajax({
            url: location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + '/Welmenu/getDate',
            method: "POST",
            dataType: "json",
            async: false,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ dateFormat: format }),
        });

        request.done(function (data) {

            $(that).empty().append(data);
            console.log(data);
        });

        request.fail(function (jqXHR, textStatus) {
            console.log('날짜 요청 실패', textStatus);
        });
    });

};

/* 애니메이션 */
/* http://www.justinaguilar.com/animations/index.html */