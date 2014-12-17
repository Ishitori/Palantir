UiColumnTable = function (table) {
    table = $(table);
    var tableId = table.prop('id');

    var ths = table.find('thead tr th'),
        maxRank = ths.length - 1,
        bgColor = [],
        color = new Palantir.Color(122, 255, 122, 0.5),
        step = Math.ceil(122 / maxRank),
        i = 0,
        len;

    bgColor[0] = color.toHexString();

    for (i = 1; i < maxRank; i++)
    {
        color.addRed(step);
        color.addBlue(step);
        bgColor[i] = color.toHexString();
    }

    var tds = table.find('td');

    for (i = 0, len = tds.length; i < len; i++)
    {
        var td = $(tds[i]);
        var rank = td.data('rank');

        if (!rank)
        {
            continue;
        }

        color = bgColor[rank - 1];
        td.css('background-color', color);
        td.attr('title', rank + ' место');
    }
};

$(function () {
    // Инициализация таблиц
    $('.ui-column-table').each(function (i, table) {
        new UiColumnTable(table);
    });
});