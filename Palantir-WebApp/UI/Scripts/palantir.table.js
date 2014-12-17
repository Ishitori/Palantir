UiTable = function (table, userOptions) {
    table = $(table);
    var tableId = table.prop('id');
    var autonumeric = $('td.ui-table-autonumeric', table) ? $('td.ui-table-autonumeric', table).prevAll('th').length : -1;
    var defaultOptions = {
        bPaginate: false,
        bLengthChange: false,
        bFilter: false,
        bInfo: false,
        aaSorting: [],
        aoColumns: []
    };

    var options = userOptions ? $.extend({}, defaultOptions, userOptions)
                                : defaultOptions;
    // Настройки колонок таблицы
    table.find('thead th').each(function (i, th) {
        th = $(th);
        var sortable = [];
        if (th.hasClass('ui-table-sortable-d')) sortable.push("desc");
        else if (th.hasClass('ui-table-sortable-a')) sortable.push("asc");
        else if (th.hasClass('ui-table-sortable')) sortable.push("desc", "asc");
        options.aoColumns.push({ 
            bSortable: sortable.length > 0,
            asSorting: sortable
        });
    });
    // Разбор настроек сортировки таблиц, сохраненных в якоре
    var anchor = $.AnchorData(tableId);
    if (anchor) {
        var parts = anchor.split('-');
        options.aaSorting.push([Number(parts[0]), parts[1]]);
    }
    // Если для таблицы не сохранены настройки в якоре, устанавливаются настройки по-умолчанию
    if (!options.aaSorting.length) {
        table.find('th.ui-table-def-sorted-d, th.ui-table-def-sorted-a, th.ui-table-def-sorted').each(function (i, th) {
            th = $(th);
            var pos = th.prevAll('th').length;
            options.aaSorting.push([pos, th.hasClass('ui-table-def-sorted-a') ? 'asc' : 'desc']);
        });
    }
    table.dataTable(options);
    table.find('th').click(tableSorted);
    tableSorted();
    function tableSorted(event) {
        // Восстановление порядка следования классов
        $('tbody tr', table).removeClass('last').removeClass('first').removeClass('even-row')
            .first().addClass('first').end()
            .last().addClass('last');

        // Сохранение сортировки в якоре
        if (!event) return;
        var th = $('thead .sorting_desc, thead .sorting_asc', table);
        var param = '';
        param += th.prevAll('th').length + '-';
        param += th.hasClass('sorting_desc') ? 'desc' : 'asc';
        $.AnchorData(tableId, param);
    }
}

$(function () {
    // Инициализация таблиц
    $('.ui-table').each(function (i, table) {
        // 
        if (table.className.indexOf('preventAutoInit') < 0) {
            new UiTable(table);
        }
    });
});