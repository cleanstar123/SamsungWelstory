/* 파티셜뷰 스케줄 등록 수정 화면 */
var manageSchedule = (function () {

    return {

        models: {
            
            ScheduleModel: function () {
                this.SCHEDULE_ID        = '';
                this.ROLLING_YN         = '';
                this.URL                = '';
                this.SCHEDULE_TYPE      = '';
                this.SCHEDULE_NM        = '';
                this.SCHEDULE_DESC      = '';
                this.TEMPLATE_ID        = '';
                this.START_DATE         = '';
                this.END_DATE           = '';
                this.START_TIME         = '';
                this.END_TIME           = '';
                this.PLAY_TIME          = '';
                this.MON_YN             = '';
                this.TUE_YN             = '';
                this.WED_YN             = '';
                this.THU_YN             = '';
                this.FRI_YN             = '';
                this.SAT_YN             = '';
                this.SUN_YN             = '';
            },
            ScheduleTemplateMapModel: function () {
                this.SEQ                = '';
                this.TEMPLATE_ID        = '';
                this.TEMPLATE_URL       = '';
                this.PLAY_TIME          = '';
            },
            DisplayGroupModel: function () {
                this.RESTAURANT_CODE    = '';
                this.DISPLAY_GROUP_ID   = '';
                this.DISPLAY_GROUP_NM   = '';
                this.DISPLAY_GROUP_DESC = '';
            },
            DisplayMapModel: function () {
                this.DISPLAY_GROUP_ID   = '';
                this.DISPLAY_ID         = '';
            },
            DisplayModel: function () {
                this.DISPLAY_ID         = '';
            }
        },
        elements: {
            $jgGroup                : $('#jgGroup'),
            $jgDisplayMap           : $('#jgDisplayMap'),
            $jgDisplay              : $('#jgDisplay'),
            $txtDisplayGroupNm      : $('#txtDisplayGroupNm'),
            $SCHEDULE_ID            : $('#SCHEDULE_ID'),
            $ROLLING_YN             : $('#ROLLING_YN'),
            $URL                    : $('#URL'),
            $SCHEDULE_TYPE          : $('#SCHEDULE_TYPE'),
            $SCHEDULE_NM            : $('#SCHEDULE_NM'),
            $SCHEDULE_DESC          : $('#SCHEDULE_DESC'),
            $TEMPLATE_ID            : $('#TEMPLATE_ID'),
            $TEMPLATE_NM            : $('#TEMPLATE_NM'),
            $START_DATE             : $('#START_DATE'),
            $END_DATE               : $('#END_DATE'),
            $START_TIME             : $('#START_TIME'),
            $END_TIME               : $('#END_TIME'),
            $MON_YN                 : $('#MON_YN'),
            $TUE_YN                 : $('#TUE_YN'),
            $WED_YN                 : $('#WED_YN'),
            $THU_YN                 : $('#THU_YN'),
            $FRI_YN                 : $('#FRI_YN'),
            $SAT_YN                 : $('#SAT_YN'),
            $SUN_YN                 : $('#SUN_YN'),
            $btnSearchTemplate      : $('#btnSearchTemplate'),
            $btnUpdateTemplate      : $('#btnUpdateTemplate')
        },
        popups: {
            jbSchedule: new jBox('Modal', {
                draggable          : 'title',
                dragOver           : false,
                closeOnEsc         : true,
                isolateScroll      : false,
                attach             : '#myModal',
                title              : '스케줄 관리',
                content            : $('#jbSchedule'),
                closeButton        : 'title',
                width              : "47%",
                height             : "64%",
                minWidth           : 760,
                minHeight          : 550,
                responsiveMinWidth : 760,
                responsiveMinHeight: 550,
                responsiveWidth    : false,
                responsiveHeight   : false,
                footer             : '<div style="text-align:center;"><a class="a_btn btn_style3" id="btnManageScheduleSave">저장</a><a class="a_btn btn_style3" id="btnManageScheduleUpdate">수정</a><a class="a_btn btn_style3" id="btnManageScheduleDelete">삭제</a><a class="a_btn btn_style3" id="btnManageScheduleCancel">취소</a></div>',
                onClose            : function () {

                    $('#btnManageScheduleSave').unbind('click');
                    $('#btnManageScheduleUpdate').unbind('click');
                    $('#btnManageScheduleDelete').unbind('click');
                    $('#btnManageScheduleCancel').unbind('click');
                }
            }),
            showJBSchedule: function (actionType, id) {

                manageSchedule.init();                       // 스케줄 팝업 초기화
                manageSchedule.popups.jbSchedule.open();     // 스케줄 팝업 열기

                // 디스플레이 그룹 조회
                searchDisplayGroup({}, function (data) {

                    manageSchedule.elements.$jgGroup.jqGrid('clearGridData');
                    manageSchedule.elements.$jgGroup[0].addJSONData({ rows: data.Table });
                    manageSchedule.elements.$jgGroup.jqGrid("addRowData", jqgridNewRowId(manageSchedule.elements.$jgGroup), { DISPLAY_GROUP_NM: "전체" }, 'first');
                });

                $('#selectedTemplateArea').empty();

                // 취소 버튼 클릭 이벤트 추가
                $('#btnManageScheduleCancel').bind('click', function () {
                    manageSchedule.popups.jbSchedule.close();
                });

                if (actionType.toUpperCase() == "U") {

                    // 수정하는 스케줄 검색
                    searchSchedule(id);

                    // 팝업 하단 버튼 이벤트 추가
                    $('#btnManageScheduleSave').hide();
                    $('#btnManageScheduleUpdate').bind('click', function () { manageSchedule.saveSchedule('U'); }).show();
                    $('#btnManageScheduleDelete').bind('click', function () { manageSchedule.deleteSchedule();  }).show();
                }
                else {

                    // 팝업 하단 버튼 이벤트 추가
                    $('#btnManageScheduleSave').bind('click', function () { manageSchedule.saveSchedule("I"); }).show();
                    $('#btnManageScheduleUpdate').hide();
                    $('#btnManageScheduleDelete').hide();
                }

                /* 스케줄 팝업의 그리드 크기 조정 */
                $(manageSchedule.elements.$jgGroup).setGridWidth(manageSchedule.elements.$jgGroup.parents('.grid_ej').width());
                $(manageSchedule.elements.$jgGroup).setGridHeight(manageSchedule.elements.$jgGroup.parents('.grid_ej').height() - 30);

                $(manageSchedule.elements.$jgDisplayMap).setGridWidth(manageSchedule.elements.$jgDisplayMap.parents('.grid_ej').width());
                $(manageSchedule.elements.$jgDisplayMap).setGridHeight(manageSchedule.elements.$jgDisplayMap.parents('.grid_ej').height() - 30);

                $(manageSchedule.elements.$jgDisplay).setGridWidth(manageSchedule.elements.$jgDisplay.parents('.grid_ej').width());
                $(manageSchedule.elements.$jgDisplay).setGridHeight(manageSchedule.elements.$jgDisplay.parents('.grid_ej').height() - 30);
            }
        },
        init: function () {
            manageSchedule.elements.$jgGroup.jqGrid("resetSelection");
            manageSchedule.elements.$jgDisplayMap.jqGrid('clearGridData');
            manageSchedule.elements.$jgDisplay.jqGrid('clearGridData');
            manageSchedule.elements.$SCHEDULE_TYPE.val('1');
            manageSchedule.elements.$SCHEDULE_NM.val('');
            manageSchedule.elements.$SCHEDULE_DESC.val('');
            manageSchedule.elements.$TEMPLATE_ID.val('');
            manageSchedule.elements.$TEMPLATE_NM.val('');
            manageSchedule.elements.$START_DATE.val(nowDate());
            manageSchedule.elements.$END_DATE.val('');
            manageSchedule.elements.$START_TIME.val('');
            manageSchedule.elements.$END_TIME.val('');
            manageSchedule.elements.$MON_YN.prop("checked", true);
            manageSchedule.elements.$TUE_YN.prop("checked", true);
            manageSchedule.elements.$WED_YN.prop("checked", true);
            manageSchedule.elements.$THU_YN.prop("checked", true);
            manageSchedule.elements.$FRI_YN.prop("checked", true);
            manageSchedule.elements.$SAT_YN.prop("checked", true);
            manageSchedule.elements.$SUN_YN.prop("checked", true);
        },
        options: {
            jgGroupOption: {
                regional: 'en',
                datatype: "json",
                autowidth: true,
                shrinkToFit: true,
                colModel: [
                    { label: '그룹 아이디', name: 'DISPLAY_GROUP_ID',   width: 50, align: 'center', hidden: true, sortable: false },
                    { label: '그룹 이름',   name: 'DISPLAY_GROUP_NM',   width: 50, align: 'center', sortable: false },
                    { label: '그룹 설명',   name: 'DISPLAY_GROUP_DESC', width: 90, sortable: false },
                ],
                viewrecords: true, // show the current page, data rang and total records on the toolbar
                width: 250,
                height: 200,
                rowNum: 'all',
                //pager: "#jgpGroup",
                onSelectRow: function (rowid, status, e) {
                    searchDisplayMap(manageSchedule.elements.$jgGroup.jqGrid('getRowData', rowid).DISPLAY_GROUP_ID, function (data) {

                        manageSchedule.elements.$jgDisplayMap.jqGrid('clearGridData');
                        manageSchedule.elements.$jgDisplayMap[0].addJSONData({ rows: data.Table });
                    });
                },
                afterInsertRow: function (rowid, rowdata, rawdata) {
                    manageSchedule.elements.$jgGroup.jqGrid("setSelection", rowid);
                }
            },
            jgDisplayMapOption: {
                regional: 'en',
                datatype: "json",
                autowidth: true,
                shrinkToFit: true,
                colModel: [
                    { label: '아이디',      name: 'DISPLAY_ID',   width: 160, align: 'center', hidden: true, sortable: false },
                    { label: '이름',        name: 'DISPLAY_NM',   width: 150, sortable: false },
                    { label: '설명',        name: 'DISPLAY_DESC', width: 150, sortable: false },
                    { label: '해상도 넓이', name: 'SCREEN_W',     width: 110, hidden: true, sortable: false },
                    { label: '해상도 높이', name: 'SCREEN_H',     width: 110, hidden: true, sortable: false },
                    { label: '운영체제',    name: 'DISPLAY_OS',   width: 100, hidden: true, sortable: false },
                    { label: 'IP',          name: 'DISPLAY_IP',   width: 100, align: 'center', hidden: true, sortable: false },
                    { label: 'MAC',         name: 'DISPLAY_MAC',  width: 100, align: 'center', hidden: true, sortable: false },
                    { label: '사용유무',    name: 'USE_YN',       width: 100, align: 'center', hidden: true, sortable: false },
                    { label: '생성자',      name: 'REG_ID',       width: 80, hidden: true, sortable: false },
                    { label: '생성일',      name: 'REG_DTM',      width: 80,  align: 'center', hidden: true, sortable: false },
                    { label: '수정자',      name: 'MOD_ID',       width: 80, hidden: true, sortable: false },
                    { label: '수정일',      name: 'MOD_DTM',      width: 80,  align: 'center', hidden: true, sortable: false }
                ],
                viewrecords: true, // show the current page, data rang and total records on the toolbar
                height: 200,
                width: 250,
                rowNum: 'all',
                //pager: "#jgpDisplayMap",
                multiselect: true,
                beforeSelectRow: checkboxSelect
            },
            jgDisplayOption: {
                regional: 'en',
                datatype: "json",
                autowidth: true,
                shrinkToFit: true,
                colModel: [
                    { label: '아이디',      name: 'DISPLAY_ID',   width: 160, sortable: false, align: 'center', hidden: true  },
                    { label: '이름',        name: 'DISPLAY_NM',   width: 150, sortable: false, align: 'left',   hidden: false },
                    { label: '설명',        name: 'DISPLAY_DESC', width: 150, sortable: false, align: 'left',   hidden: false },
                    { label: '해상도 넓이', name: 'SCREEN_W',     width: 110, sortable: false, align: 'left'  , hidden: true  },
                    { label: '해상도 높이', name: 'SCREEN_H',     width: 110, sortable: false, align: 'left'  , hidden: true  },
                    { label: '운영체제',    name: 'DISPLAY_OS',   width: 100, sortable: false, align: 'left'  , hidden: true  },
                    { label: 'IP',          name: 'DISPLAY_IP',   width: 100, sortable: false, align: 'center', hidden: true  },
                    { label: 'MAC',         name: 'DISPLAY_MAC',  width: 100, sortable: false, align: 'center', hidden: true  },
                    { label: '사용유무',    name: 'USE_YN',       width: 100, sortable: false, align: 'center', hidden: true  },
                    { label: '생성자',      name: 'REG_ID',       width: 80,  sortable: false, align: 'left'  , hidden: true  },
                    { label: '생성일',      name: 'REG_DTM',      width: 80,  sortable: false, align: 'center', hidden: true  },
                    { label: '수정자',      name: 'MOD_ID',       width: 80,  sortable: false, align: 'left'  , hidden: true  },
                    { label: '수정일',      name: 'MOD_DTM',      width: 80,  sortable: false, align: 'center', hidden: true  }
                ],
                viewrecords: true, // show the current page, data rang and total records on the toolbar
                width:250,
                height: 200,
                rowNum: 'all',
                //pager: "#jgpDisplay",
                multiselect: true,
                beforeSelectRow: checkboxSelect
            }
        },
        addSelectedTemplate: function (data, scheduleId) {

            $('#selectedTemplateArea').empty();

            var selectedTemplate = '';

            for (var templateSelectedId in data) {

                selectedTemplate += '<li>';
                selectedTemplate += '<i class="fa fa-arrows-alt" style="cursor:move;"></i>';
                selectedTemplate += '<input type="text" class="tem_input_1 template-id" value="' + data[templateSelectedId].TEMPLATE_ID + '"  disabled />';
                selectedTemplate += '<input type="text" class="tem_input_2" value="' + data[templateSelectedId].TEMPLATE_NM + '" disabled />';
                selectedTemplate += '<input type="text" class="tem_input_1 play-time" value="' + (data[templateSelectedId].PLAY_TIME ? data[templateSelectedId].PLAY_TIME : '5')  + '" />초 ';
                selectedTemplate += '<i class="fa fa-window-close" style="cursor:pointer;" onclick="manageSchedule.removeSelectedTemplate(this);"></i>';
                selectedTemplate += '<input type="hidden" class="template-url" value="' + data[templateSelectedId].TEMPLATE_URL + '" />';
                selectedTemplate += '<a class="a_btn btn_style3 f_r" href="javascript:manageSchedule.moveUpdateTemplate(\'' + data[templateSelectedId].TEMPLATE_ID + '\', \'' + scheduleId + '\');" >템플릿 수정</a>'
                selectedTemplate += '</li>';
            }

            $('#selectedTemplateArea').append($(selectedTemplate));
            $("#selectedTemplateArea").sortable();
            $("#selectedTemplateArea").disableSelection();

            new Cleave($('.play-time'), {
                delimiter: '',
                numericOnly: true,
                blocks: [5]
            });
        },
        removeSelectedTemplate: function (ele) {
            if (confirm('스케줄에서 제거하시겠습니까?')) {
                $(ele).parent('li').remove();
            }
        },
        moveUpdateTemplate: function (templateId, scheduleId) {

            if (localStorage) {
                localStorage.setItem('scheduleId', scheduleId);
            }

            location.href = 'template/managetemplate?type=U&id=' + templateId;
        },
        saveSchedule: function(actionType) {

            var date = new Date();
            var nowDate = date.getFullYear() + '' + pad((date.getMonth() + 1), 2) + '' + date.getDate();

            // 정합성 체크

            // 제어 스케줄일 경우 템플릿을 선택하지 않아도 저장이 가능
            if (manageSchedule.elements.$SCHEDULE_TYPE.val() != '3') {

                if ($('#selectedTemplateArea li').length <= 0) {
                    alert('템플릿을 선택 해주세요.');
                    manageSchedule.elements.$btnSearchTemplate.focus();
                    return false;
                }
            }

            if (!manageSchedule.elements.$SCHEDULE_NM.val()) {
                alert('스케줄 이름을 입력하세요.');
                manageSchedule.elements.$SCHEDULE_NM.focus();
                return false;
            }

            // 이후 박찬진 차장님이 프로시저 체크 수정하시면 주석 처리
            var startTime = manageSchedule.elements.$START_TIME.val() ? manageSchedule.elements.$START_TIME.val().replace(':', '') * 1 : 0;
            var endTime   = manageSchedule.elements.$END_TIME.val()   ? manageSchedule.elements.$END_TIME.val().replace(':', '') * 1   : 2359;

            if (startTime > endTime) {

                alert('종료시간이 시작시간보다 작을 수 없습니다.');
                manageSchedule.elements.$START_TIME.focus();
                return false;
            }
            // 이후 박찬진 차장님이 프로시저 체크 수정하시면 주석 처리

            if (!manageSchedule.elements.$START_DATE.val()) {
                alert('시작일을 입력하세요.');
                manageSchedule.elements.$START_DATE.focus();
                return false;
            }
            else {

                // 시작일을 입력했을 경우 현재일 체크
                if ((nowDate * 1) > (manageSchedule.elements.$START_DATE.val().replace(/-/gi, "") * 1)) {
                    alert('시작일은 현재일 부터 입력 가능합니다.');
                    manageSchedule.elements.$START_DATE.focus();
                    return false;
                }
            }

            // 종료일을 입력했을 경우
            if (manageSchedule.elements.$END_DATE.val()) {

                // 시작일과 종료일 체크
                if ((manageSchedule.elements.$START_DATE.val().replace(/-/gi, "") * 1) > (manageSchedule.elements.$END_DATE.val().replace(/-/gi, "") * 1)) {
                    alert('종료일이 시작일 보다 작을 수 없습니다.');
                    manageSchedule.elements.$START_DATE.focus();
                    return false;
                }

                // 시작일과 종료일이 같을 경우
                if (manageSchedule.elements.$START_DATE.val() == manageSchedule.elements.$END_DATE.val()) {

                    // 시작시간과 종료시간이 입력 됬을 경우
                    if (startTime >= endTime) {

                        alert('종료시간이 시작시간과 같거나 작을 수 없습니다.');
                        manageSchedule.elements.$START_TIME.focus();
                        return false;
                    }
                }
            }

            if (manageSchedule.elements.$END_TIME.val()) {

                if (manageSchedule.elements.$END_TIME.val() == '00:00') {
                    alert('종료시간은 00:01 부터 입력 가능합니다.');
                    manageSchedule.elements.$END_TIME.focus();
                    return false;
                }
            }


            if (!manageSchedule.elements.$MON_YN.is(":checked") && 
                !manageSchedule.elements.$TUE_YN.is(":checked") &&
                !manageSchedule.elements.$WED_YN.is(":checked") &&
                !manageSchedule.elements.$THU_YN.is(":checked") &&
                !manageSchedule.elements.$FRI_YN.is(":checked") &&
                !manageSchedule.elements.$SAT_YN.is(":checked") &&
                !manageSchedule.elements.$SUN_YN.is(":checked")) {

                alert('요일은 하나 이상 선택해야 합니다.');
                manageSchedule.elements.$MON_YN.focus();
                return false;
            }
            
            var params = {
                TYPE: actionType,
                scheduleModel: {},
                scheduleTemplateMapModels: [],
                displayModels: []
            };

            var scheduleModel = new manageSchedule.models.ScheduleModel();

            scheduleModel.SCHEDULE_ID   = manageSchedule.elements.$SCHEDULE_ID.val();
            scheduleModel.ROLLING_YN    = manageSchedule.elements.$ROLLING_YN.val();
            scheduleModel.URL           = manageSchedule.elements.$URL.val();
            scheduleModel.SCHEDULE_TYPE = manageSchedule.elements.$SCHEDULE_TYPE.val();
            scheduleModel.SCHEDULE_NM   = manageSchedule.elements.$SCHEDULE_NM.val();
            scheduleModel.SCHEDULE_DESC = manageSchedule.elements.$SCHEDULE_DESC.val();
            scheduleModel.TEMPLATE_ID   = manageSchedule.elements.$TEMPLATE_ID.val();
            scheduleModel.START_DATE    = manageSchedule.elements.$START_DATE.val();
            scheduleModel.END_DATE      = manageSchedule.elements.$END_DATE.val();
            scheduleModel.START_TIME    = manageSchedule.elements.$START_TIME.val();
            scheduleModel.END_TIME      = manageSchedule.elements.$END_TIME.val();
            scheduleModel.MON_YN        = manageSchedule.elements.$MON_YN.is(":checked") ? 'Y' : 'N';
            scheduleModel.TUE_YN        = manageSchedule.elements.$TUE_YN.is(":checked") ? 'Y' : 'N';
            scheduleModel.WED_YN        = manageSchedule.elements.$WED_YN.is(":checked") ? 'Y' : 'N';
            scheduleModel.THU_YN        = manageSchedule.elements.$THU_YN.is(":checked") ? 'Y' : 'N';
            scheduleModel.FRI_YN        = manageSchedule.elements.$FRI_YN.is(":checked") ? 'Y' : 'N';
            scheduleModel.SAT_YN        = manageSchedule.elements.$SAT_YN.is(":checked") ? 'Y' : 'N';
            scheduleModel.SUN_YN        = manageSchedule.elements.$SUN_YN.is(":checked") ? 'Y' : 'N';

            params.scheduleModel = scheduleModel;

            var scheduleTemplateMapModels = [];

            $('#selectedTemplateArea li').each(function (index, value) {
                var scheduleTemplateMapModel = new manageSchedule.models.ScheduleTemplateMapModel();

                scheduleTemplateMapModel.SEQ          = index;
                scheduleTemplateMapModel.TEMPLATE_ID  = $(this).children('input.template-id').val();
                scheduleTemplateMapModel.TEMPLATE_URL = $(this).children('input.template-url').val();
                scheduleTemplateMapModel.PLAY_TIME    = $(this).children('input.play-time').val();

                scheduleTemplateMapModels.push(scheduleTemplateMapModel);
            });

            params.scheduleTemplateMapModels = scheduleTemplateMapModels;

            var displays = manageSchedule.elements.$jgDisplay.getRowData();

            if (displays.length <= 0) {
                alert('디스플레이는 하나 이상 등록해야합니다.')
                return false;
            }

            for (var i = 0; i < displays.length; i++) {

                var displayModel = new manageSchedule.models.DisplayModel();
                displayModel.DISPLAY_ID = displays[i].DISPLAY_ID;

                params.displayModels.push(displayModel);
            }

            simpleAjax(urlManageSchedule, params, false).done(function (data) {
                if (data.ERR_CODE == '0') {
                    resultMessage(actionType);
                    manageSchedule.popups.jbSchedule.close();
                    if (searchScheduleCal) {
                        isFirstSearch = true;
                        searchScheduleCal();
                    }
                }
                else {
                    alert(data.ERROR_MSG);

                    if (data.ERR_CODE == '9998') {
                        location.href = 'Account/Logout';
                        return;
                    }
                    else
                        return false;
                }
            });

            //simpleAjax(urlCheckScheduleOverlap, params, false).done(function (data) {

            //    var isSave = false;

            //    if (data.ERR_CODE == '0') {
            //        isSave = true;
            //        console.log('등록 가능한 스케줄');
            //    }
            //    else if (data.ERR_CODE == '3001') {

            //        // 중복 체크
            //        if (confirm(data.ERROR_MSG)) {
            //            isSave = true;
            //        }
            //    }
            //    else {
            //        isSave = false;
            //        alert('스케줄 체크중 오류가 발생하였습니다.');
            //        return false;
            //    }

            //    if (isSave) {
            //        // 스케줄 저장
            //        simpleAjax(urlManageSchedule, params, false).done(function (data) {

            //            if (data.ERR_CODE == '0') {
            //                resultMessage(actionType);
            //                jbSchedule.close();
            //            }
            //            else {
            //                alert('스케줄 등록중 오류가 발생하였습니다.\n' + data.ERROR_MSG);
            //                return false;
            //            }
            //        });
            //    }
            //});
        },
        deleteSchedule: function () {

            if (!confirm(deleteConfirmMessage))
                return;

            var params = {
                TYPE: 'D',
                scheduleModel: {
                    SCHEDULE_ID: manageSchedule.elements.$SCHEDULE_ID.val()
                }
            };

            simpleAjax(urlManageSchedule, params, false).done(function (data) {

                if (data.ERR_CODE == '0') {
                    resultMessage("D");
                    manageSchedule.popups.jbSchedule.close();
                }
                else {
                    if (data.ERR_CODE == "9998") {
                        alert(data.ERROR_MSG);
                        location.href = 'Account/Logout';
                        return;
                    }

                    alert('스케줄 삭제중 오류가 발생하였습니다.\n' + data.ERROR_MSG);
                    return false;
                }

                location.reload();
            });

        },
        addRow: function() {
            jqgridMoveRows(manageSchedule.elements.$jgDisplayMap, manageSchedule.elements.$jgDisplay, false);
        },
        removeRow: function () {
            var selarrrow = manageSchedule.elements.$jgDisplay.jqGrid('getGridParam', 'selarrrow');

            for (var i = selarrrow.length; i >= 0; i--) {
                manageSchedule.elements.$jgDisplay.delRowData(selarrrow[i]);
            }
        }
    }
})();

manageSchedule.elements.$jgGroup.jqGrid(manageSchedule.options.jgGroupOption);
manageSchedule.elements.$jgDisplayMap.jqGrid(manageSchedule.options.jgDisplayMapOption);
manageSchedule.elements.$jgDisplay.jqGrid(manageSchedule.options.jgDisplayOption);

manageSchedule.init();

$('#btnAddDisplay').click(function () {
    manageSchedule.addRow();
});

$('#btnRemoveDisplay').click(function () {
    manageSchedule.removeRow();
});


var cleave = new Cleave(manageSchedule.elements.$START_TIME, {
    time: true,
    timePattern: ['h', 'm']
});

var cleave = new Cleave(manageSchedule.elements.$END_TIME, {
    time: true,
    timePattern: ['h', 'm']
});

var cleave = new Cleave(manageSchedule.elements.$START_DATE, {
    date: true,
    datePattern: ['Y', 'm', 'd'],
    delimiter: '-',
    blocks: [4, 2, 2],
    uppercase: true
});

var cleave = new Cleave(manageSchedule.elements.$END_DATE, {
    date: true,
    datePattern: ['Y', 'm', 'd'],
    delimiter: '-',
    blocks: [4, 2, 2],
    uppercase: true
});