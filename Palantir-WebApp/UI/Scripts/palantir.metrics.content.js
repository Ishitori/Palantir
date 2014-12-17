ContentUiController = function(options) {
    $.extend(this.options, options);
    this.initialize();
};

ContentUiController.prototype = {
    options: {
        photosChartContainer: "#photosChart",
        videosChartContainer: "#videosChart"
    },

    photosChartContainer: null,
    videosChartContainer: null,

    initialize: function () {
        this.photosChartContainer = $(this.options.photosChartContainer);
        this.videosChartContainer = $(this.options.videosChartContainer);

        var filter = new UiPeriodFilter({
            initialized: $.proxy(this.filterChanged, this),
            filterChanged: $.proxy(this.filterChanged, this)
        });
        
        $(this.photosChartContainer).bind('plotcreate', function () {
            var chartData = $('.ui-chart', this).data('plot');
            if (!chartData) return;
            var photosChart = chartData.getData()[2];
            var videosChart = chartData.getData()[5];

            $('#photosDifference')
                .setSum($.map(photosChart.data, function (val) { return val[1]; }))
                .css({color: photosChart.color});
            
            $('#videosDifference')
                .setSum($.map(videosChart.data, function (val) { return val[1]; }))
                .css({color: videosChart.color});

            $('#differenceViewers').toggle(photosChart && videosChart);
        });
    },

    filterChanged: function (event, filterCriteria) {
        if (!filterCriteria || !filterCriteria.isValid)
        {
            return;
        }

        LoadChart(this.photosChartContainer, this.photosChartContainer.data("source"), filterCriteria);

        var url = $("#mostPopularContentContainer").data("source");
        var dateFilter = Palantir.getDateFilter(filterCriteria);
        
        $.get(url, dateFilter, function (data) {
            $("#mostPopularContentContainer").html(data);
            new UiTable($("#mostPopularContent"));
        });
    }
};