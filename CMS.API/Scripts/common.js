//window resizing
var cntSize = function() {
//if (($(window).width()) > 1200) {
//	$('.container_e').width($(window).width()-40);
	$('.container_e').height($(window).height()-157);
//		$('.content_grid').width($('.container_e').width()-17);
	$('.content_grid').height($('.container_e').height());

	$('.container_wrap').height($(window).height()-($('.top').height()+$('.header').height()+$('.current').height()));
	$('.sub_cnt').height($('.content').height()-$('.bul_wrap').height()-$('.search_area').height()-$('.bul_arr2').height()-52);
	$('.tem_sub_cnt').height($('.content').height()-$('.bul_wrap').height()-$('.search_area').height()-$('.bul_arr2').height()-52);
	$('.con_sub_cnt').height($('.content').height()-$('.bul_wrap').height()-$('.search_area').height()-$('.bul_arr2').height()-102);
	$('.sub_cnt_g').height($('.content_grid').height()-$('.bul_wrap').height()-$('.search_area').height()-40);
		
	$('.calendarArea').height($('.content_grid').height()-10-$('.bul_wrap').height());
		
	$('.container_tem').height($(window).height()-($('.top').height()+$('.header').height()+$('.current').height()));
	$('.sub_cnt_tem').height($('.content').height()-$('.bul_wrap').height()-2);
	//코드관리
	$('.content_left').height($('.content_grid').height()-$('.bul_wrap').height()-$('.search_area').height()-40);
	$('.content_right').height($('.content_grid').height()-$('.bul_wrap').height()-$('.search_area').height()-40);

    
    // 20180903 정해성 추가
    $('.grid_ej table').each(function () {
        $(this).setGridWidth($(this).closest('.grid_ej').width());
        $(this).setGridHeight($(this).closest('.grid_ej').height() - 30);
    });    
}

$(window).resize(cntSize);

$(document).ready(function () {

    cntSize();

	// gnb
	$(".all_menu").click(function(){
		$(".gnb_m").toggleClass('block');
		$(this).toggleClass('close');
	});
	// �덈룄�곗슜
	$(function(){
		var $snb =  $('.gnb_w .snb_level1 > li');
		var $head = $('.header');
		var $level2 = $('.gnb_w .snb_level2');
		var $a = $('.gnb_w .snb_level1 a');
		var $aLast = $('.gnb_w .snb_level1 li:last-child ul li:last-child a');
		var $alogo = $('h1.logo a');
		var $other = $('body');
        var $topBn = $('.main_top_baner');

		$level2.hide();
		$snb.on('mouseenter',function(){
			$level2.stop().show();
			$(this).addClass('on');
			$(".nav_a").addClass('on');
		})
		$snb.on('mouseleave',function(){
			$(this).removeClass('on');
			//$(".nav").removeClass('on');
		})
		$head.on('mouseleave',function(){
			$level2.stop().hide();
			$(".nav_a").removeClass('on');
		})
		$a.on('focus',function(){
			$level2.stop().show();
			$(this).parent().addClass('on');
			$(".nav_a").addClass('on');
		})
		$a.on('blur',function(){
			$(this).parent().removeClass('on');
			$(".nav_a").removeClass('on');
		});
		$aLast.on('blur',function(){
			$level2.stop().hide();
		});
		$alogo.on('focus',function(){
			$level2.stop().hide();
		});
		$other.on('click',function(){
			$level2.stop().hide();
		});
        $topBn.on('click',function(){
            $level2.stop().hide();
        });
		$head.on('click',function(){
            $level2.stop().hide();
            $(".nav_a").removeClass('on');
		});

    });

    //페이징
    //$('.paging li').click(function () {
    //    $(this).css({ 'color': '#1078c3', 'font-weight': 'bold' }).siblings().css({ 'color': '#000', 'font-weight': 'normal' })
    //})

});