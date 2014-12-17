(function ($)
{
    // Создать график
    // - argument - имя вызываемого метода либо данные графика
    $.fn.pieChart = function (argument)
    {
        $.each($(this), function (i, obj) {
            obj = $(obj);
            chart = obj.data('pieChart');
            if (chart) {
                if (chart.methods[argument]) {
                    chart.methods[argument].apply(chart, Array.prototype.slice.call(arguments, 1));
                    return;
                } else {
                    chart.destroy();
                }
            }
            obj.data('pieChart', new PieChart(obj, argument));
        });
        return this;
    };

    PieChart = function(container, options) {
        this.container = $(container);
        $.extend(this.options, options);
        this.init();
    };
    
    PieChart.prototype = {
        options: {
            data: null,
            pieHover: function (event, pos, obj) {
                if (!obj) return;
                percent = parseFloat(obj.series.percent).toFixed(2);
                var tooltip = $('body > .chart-tooltip');
                tooltip.css({ 'top': pos.pageY - tooltip.outerHeight() + 'px', 'left': pos.pageX + 'px' });
                tooltip.html('<span style="color: ' + obj.series.color + '">' + obj.series.label + ' - ' + obj.series.data[0][1] + ' (' + percent + '%)</span>').show();
            },
            pieMouseover: function () {
                $('body > .chart-tooltip').hide();
            },
            pieClick: function (event, pos, obj) { }
        },
        methods: {
            destroy: function () {
                this.destroy();
            }
        },
        container: null,
        init: function () {
            var values = this.options.data;
            var opt = this.getPlotOptions(values);

            // Initialize the container.
            this.container.height(this.container.height() || opt.height);

            // Create chart.
            $.plot(this.container, values.items, opt);

            $(this.container).resize($.proxy(function () {
                $(this.container).empty();
                $.plot(this.container, values.items, opt);
            }, this));

            this.container.bind("plothover", this.options.pieHover)
                .bind("plotclick", this.options.pieClick)
                .bind('mouseleave', this.options.pieMouseover);

            // Add tooltip container.
            if (!$("body > .chart-tooltip").length) $("<div class='chart-tooltip'></div>").appendTo("body");
        },
        getPlotOptions: function () {
            return {
                height: 340,
                series: {
                    pie: {
                            innerRadius: 0.5,
                            show: true,
                            //UI settings
                            pieStrokeLineWidth: 1,
                            pieStrokeColor: '#FFF',
                            showLabel: true,
                            labelOffset: 0,
                            labelOffsetFactor: 5 / 6,
                            labelBackgroundOpacity: 0.75,
                            //Show label and percents in chart sectors
                            labelFormatter: function (serie) {
                                return '<span style="color: white;font-weight:bold;background-color: black;">' + serie.label + ' - ' + serie.data[0][1] + ' (' + Math.round(serie.percent) + '%)' + '</span>';
                            },
                        }
                },
                grid: {
                        hoverable: true
                },
                legend: {
                        show: true,
                        position: "ne",
                        backgroundOpacity: 0
                }
            };
        },
        destroy: function () {
            //this.data('pieChart').remove();
            //this.unbind();
        }
    }
})(jQuery);