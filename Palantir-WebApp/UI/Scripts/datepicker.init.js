$(document).ready(function () {
    $('#calendarSelector').daterangepicker(
    	{
    	    ranges: {
    	        'За день': [Date.today(), Date.today()],
    	        'За неделю': [Date.today().add({ days: -7 }), Date.today()],
    	        'За месяц': [Date.today().add({ months: -1 }), Date.today()],
    	        'За год': [Date.today().add({ years: -1 }), Date.today()]
    	    },
    	    periods: ['day', 'week', 'month', 'year'],
    	    locale: {
    	        customRangeLabel: 'За другой период',
    	        applyLabel: 'Применить',
    	        clearLabel: "Очистить",
    	        fromLabel: 'С',
    	        toLabel: 'По',
    	        firstDay: 1,
    	        daysOfWeek: ["Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб"],
    	        monthNames: ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"],
    	    },
    	    maxDate: Date.now(),
    	},
	function (start, end, period) {
	    var startStr = moment(start).format("DD.MM.YYYY");
	    var endStr = moment(end).format("DD.MM.YYYY");
	    var value = startStr != endStr ? startStr + ' - ' + endStr : startStr;
	    $('#calendarText').val(value);
	    $('#calendarSelector').trigger('periodChanged', [period, startStr, endStr]);
	    /*console.log(period, startStr, endStr);*/
	}
    );
});