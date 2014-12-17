(function ()
{
    if (!window.Palantir)
    {
        window.Palantir = {};
    }

    var palantir = window.Palantir;
    $.ajaxSetup({ cache: false });

    palantir.namespace = function (name)
    {
        /// <summary>Creates namespace.</summary>
        /// <param name="name" type="string">Namespace name, for example "Palantir.Events".</param>

        var allSpaces = name.split(".");
        var current = window;

        for (var i = 0; i < allSpaces.length; i++)
        {
            var space = allSpaces[i];

            if (!current[space])
            {
                current[space] = {};
            }

            current = current[space];
        }

        return current;
    };

    palantir.getDate = function(datePickerId)
    {
        var datePicker = $("#" + datePickerId);
        var day = datePicker.datepicker('getDate').getDate();                 
        var month = datePicker.datepicker('getDate').getMonth() + 1;             
        var year = datePicker.datepicker('getDate').getFullYear();
        var fullDate = year + "-" + month + "-" + day;

        return fullDate;
    };

    palantir.getDateFilter = function (filterCriteria) {
        var date = { period: filterCriteria.Periodicity };
        if (filterCriteria.Periodicity == "other")
        {
            if (filterCriteria.DateRange.From && filterCriteria.DateRange.To)
            {
                date["dateFrom"] = filterCriteria.DateRange.From;
                date["dateTo"] = filterCriteria.DateRange.To;
            }
        }

        return date;
    };
})();

LoadDashboard = function (dashboardContainer, url, filterCriteria,tooltips) {
    var data = Palantir.getDateFilter(filterCriteria);

    if (url && data) {
        $.get(url, data, function(html) {
            dashboardContainer.empty().append(html);

            for (var i in tooltips) {
                new ToolTipLoader(tooltips[i].tooltipContainer, tooltips[i].options);
            }
        });
    }
};

// BEGIN: LoadChart
LoadChart = function (chartContainer, url, filterCriteria, buildTooltip_Function) {
    if (filterCriteria) {
        var data = Palantir.getDateFilter(filterCriteria);
        $("#chartMessage").hide();
        if (url && data) {
            $(chartContainer).block({
                message: '<img src="../../Content/images/ajax-loader.gif" />'
            });
            $.get(url, data, function (chart) {
                chartContainer.find(".ui-chart").chart(chart, buildTooltip_Function);
                $(chartContainer).unblock();
            });   
        }
    } else {
        if (url) {
            $(chartContainer).block({
                message: '<img src="../../Content/images/ajax-loader.gif" />'
            });
            $.get(url, function (chart) {
                chartContainer.find(".ui-chart").chart(chart, buildTooltip_Function);
                $(chartContainer).unblock();
            });  
        }
    }
};

LoadPieChart = function(chartContainer, url, params) {
    if (url) {
        $(chartContainer).block({
            message: '<img src="../../Content/images/ajax-loader.gif" />'
        });

        data = params || {};
        $.get(url, params, function (chart) {
            chartContainer.find(".ui-graph").pieChart({ data: chart });
            $(chartContainer).unblock();
        });
    }
};
// END: LoadChart

// BEGIN: UiPeriodFilter
UiPeriodFilter = function(options) {
    $.extend(this.options, options);
    this.init();
};

UiPeriodFilter.prototype = {
    options: {
        selector: '#calendarSelector',
        textInput: '#calendarText',
        period: false,
        start: false,
        end: false,
        saveOptions: true,
        dateFormatRegex: /^(0?[1-9]|[12][0-9]|3[01])[\/\-\.](0?[1-9]|1[012])[\/\-\.]\d{4}$/,
        initialized: function (event, filterCriteria) { },
        filterChanged: function (event, filterCriteria) { },
        periodChanged: function (event, filterCriteria) { },
        dateChanged: function (event, filterCriteria) { }
    },

    init: function () {
        this.saveOptions(true);
        this.loadOptions();
        $(this.options.selector).on('periodChanged', $.proxy(this.periodChanged, this));
        var filterCriteria = this.getFilterCriteria();
        this.options.initialized({}, filterCriteria);
    },

    periodChanged: function (e, period, start, end) {
        this.options.period = period;
        this.options.start = start;
        this.options.end = end;
        var filterCriteria = this.getFilterCriteria();
        this.saveOptions();
        this.options.filterChanged(e, filterCriteria);
    },

    saveOptions: function (init) {
        if (init) {
            var anchorString = $.AnchorData('period') || localStorage.getItem("period");

            if (!anchorString) {
                this.options.period = 'day';
            } else {
                this.parseAnchorString(anchorString);
            }
        }

        var anchor = this.options.period;

        if (anchor) {
            if (this.options.period == 'other') {
                anchor += '-range-' + this.options.start + '-' + this.options.end;
            }
        }
        
        $.AnchorData('period', anchor);
        try {
            localStorage.setItem("period", anchor);
        } catch (e) { if (window.console) console.error(e); }
    },

    loadOptions: function () {
        var anchor = $.AnchorData('period');
        if (!anchor) anchor = localStorage.getItem("period");
        if (anchor) {
            this.parseAnchorString(anchor);
        } 
    },
    
    parseAnchorString: function (anchor) {
        var parts = anchor.split('-');
        if (parts.length >= 1) {
            this.options.period = parts[0];
            $(this.options.textInput).val(this.loadDateResolver(anchor));
        }
        if (parts.length >= 4) {
            this.options.start = parts[2];
            this.options.end = parts[3];
            $(this.options.textInput).val(parts[2] + " - " + parts[3]);
        }
    },
    
    loadDateResolver: function (loadValue) {
        var range = [];
        switch (loadValue) {
            case "day":
                range = [Date.today(), Date.today()];
                break;
            case "week":
                range = [Date.today().add({ days: -7 }), Date.today()];
                break;
            case "month":
                range = [Date.today().add({ months: -1 }), Date.today()];
                break;
            case "year":
                range = [Date.today().add({ years: -1 }), Date.today()];
                break;
        }
        var startStr = moment(range[0]).format("DD.MM.YYYY");
        var endStr = moment(range[1]).format("DD.MM.YYYY");
        var value = startStr != endStr ? startStr + ' - ' + endStr : startStr;
        return value;
    },

    getFilterCriteria: function () {
        var filterCriteria =
        {
            Periodicity: this.options.period,
            DateRange: {
                From: this.options.start,
                To: this.options.end
            }
        };

        filterCriteria.isValid = (filterCriteria.Periodicity != 'Other') ||
                                 (filterCriteria.DateRange.From && filterCriteria.DateRange.To &&
                                  filterCriteria.DateRange.From.match(this.options.dateFormatRegex) &&
                                  filterCriteria.DateRange.To.match(this.options.dateFormatRegex));

        return filterCriteria;
    }
};
// END: UiPeriodFilter

// BEGIN: UiTagCloud
UiTagCloud = function(containerId, onTagClicked, onLoadComlpete) {
    this.container = containerId;
    this.onTagClicked = onTagClicked || function() {};
    this.onLoadComlpete = onLoadComlpete || function() {};
    this.init();
};

UiTagCloud.prototype = {
    container: null,
    onTagClicked: null,

    init: function () {
        var sourceUrl = $(this.container).data("source");
        $.getJSON(sourceUrl, {}, $.proxy(function (data) {
            var tagList = $.map(data, $.proxy(function (el) {
                return  {
                            text: el.Text,
                            weight: el.Weight,
                            link: {href: el.Href, "data-id": el.DataId },
                            handlers: {
                                click: this.onTagClicked
                            }
                        };
            }, this));
            $(this.container).jQCloud(tagList);
            this.onLoadComlpete();
        }, this));
    }
}
// END: UiTagCloud

$.AnchorData = function(name, value) {
    // Если не требуется присваивать нового значения - ищем и возвращаем текущее
    var anchor = window.location.href.split('#');
    if (value == null) {
        var currentValue = null;

        if (anchor.length == 2) {
            $.each(anchor[1].split(';'), function(i, sortParam) {
                var pos = sortParam.indexOf('-');
                if (pos < 0 || sortParam.substring(0, pos) != name) return;
                currentValue = sortParam.substring(pos + 1);
            });
        }
        return currentValue || false;
    }
    var newAnchor = [];
    if (anchor.length == 2) {
        $.each(anchor[1].split(';'), function(i, sortParam) {
            var pos = sortParam.indexOf('-');
            if (pos >= 0 && sortParam.substring(0, pos) == name) return;
            newAnchor.push(sortParam);
        });
    }
    newAnchor.push(name + "-" + value);
    location.href = anchor[0] + "#" + newAnchor.join(";");
};

$.fn.setDifference = function(array) {
    $(this).each(function(i, obj) {
        var el = $(obj);
        el.addClass('ui-defferent-viewer');
        var val = array[array.length - 1] - array[0];
        var octothorpe = val >= 0 ? "+" : "-";
        el.html('<span class="octothorpe">' + octothorpe + '</span><span class="value">' + Math.abs(val) + '</span>');
    });
    return this;
};

$.fn.setSum = function(array) {
    $(this).each(function(i, obj) {
        var el = $(obj);
        el.addClass('ui-defferent-viewer');
        var val = 0;

        for (var j = 0; j < array.length; j++)
        {
            if (array[j])
            {
                val += array[j];
            }
        }
        
        var octothorpe = val >= 0 ? "+" : "-";
        el.html('<span class="octothorpe">' + octothorpe + '</span><span class="value">' + Math.abs(val) + '</span>');
    });
    return this;
};