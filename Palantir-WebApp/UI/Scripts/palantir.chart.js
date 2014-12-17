(function ($) {
    var colors = ["green", "red", "#4C6F96", "#008FB2"];

    $.fn.chart = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        }

        return methods.init.apply(this, arguments);
    };

    var DATA_KEY = {
        tooltip: "chart.tooltip",
        initialized: "chart.initialized"
    };

    var methods = {
        init: function (charts, buildTooltip_Function) {
            /// <summary>Initialize the chart.</summary>

            if (this.data(DATA_KEY.initialized)) {
                this.chart("destroy");
            }

            if (!$.isArray(charts)) charts = [charts];
            var series = [];
            $.each(charts, function (i, chart) {
                var d = getDataSeries(chart.values);

                if (chart.ShowPoints) {
                    series.push({
                        data: d,
                        data_forTooltip: d,
                        fake: true,
                        hoverable: false,
                        color: colors[i],
                        lines: { show: true, fill: chart.FillLines, lineWidth: 3 },
                        points: { show: false },
                        curvedLines: d.length > 1 ? { apply: true, fit: true } : { apply: false, fit: false },
                        yaxis: chart.YaxisOrder,
                    });
                }

                series.push({
                    data: d,
                    data_forTooltip: d,
                    bars: { show: chart.ShowBars, barWidth: chart.BarWidth, align: chart.BarAlignString, order: chart.BarOrder },
                    //if more than 100 points - do not show them
                    points: {
                        show: chart.ShowPoints,
                        radius: chart.values != null && chart.values.length < 100 ? 3.5 : 0.01
                    },
                    color: colors[i],
                    lines: { show: false },
                    label: chart.Name,
                    yaxis: chart.YaxisOrder,
                    stack: chart.Stacking,
                    shlabintip: chart.ShowLabelInTip
                });
                
                if (chart.Limited) {
                    if (chart.values) {
                        $("#chartMessage").html("Данные для этого графика доступны с " + chart.MinTimeLimit + " по " + chart.MaxTimeLimit).show();
                    } else {
                        $("#chartMessage").html("Данных для этого периода нет").show();
                    }
                }
            });

            // Initialize the container.      
            var opt = getOptions(charts, this.prevObject[0].children[0]);
            var dataForTooltips = getXvaluesForTooltips(charts[0].values);
            this.height(this.height() || opt.height);

            // Create chart.
            $.plot(this, series, opt);
           
            $('.chart-container').resize(function () {
                var relation = 0.42;
                $('.ui-chart').each(function () {
                    $(this).height($(this).width() * relation);
                });
            });

            // Add tooltip container.
            var tooltip = $("<div class='chart-tooltip'></div>").appendTo("body");
            this.data(DATA_KEY.tooltip, tooltip);

            // Bind events.
            this.bind("plothover", getPointHoverHandler(tooltip, dataForTooltips, buildTooltip_Function));

            // Mark object as initialized.
            this.data(DATA_KEY.initialized, true);
            this.trigger('plotcreate');
        },
        destroy: function () {
            this.data(DATA_KEY.tooltip).remove();
            this.unbind();
            this.data(DATA_KEY.initialized, false);
        }
    };

    function getOptions(charts, legendContainer) {
        return {
            legend: {
                show: true,
                container: legendContainer,
                noColumns: charts.length 
            },
            grid: {
                hoverable: true,
                autoHighlight: true,
                mouseActiveRadius: 5
            },
            height: 400,
            series: {
                curvedLines: { active: true },
                points: { show: true },
                lines: { show: true }
            },
            xaxis: {
                ticks: getChartTicks(charts[0].values),
                font: { size: 10, color: "#545454" },
                color: "#BBB"
            },
            yaxes: [
                { position: "left", color: "#545454", font: { color: "#545454" } },
                { position: "right", font: { color: colors[0] } },
                { position: "right", font: { color: colors[1] } },
                { position: "right", font: { color: colors[2] } },
                { position: "right", font: { color: colors[3] } }
            ]
        };
    }

    function getChartTicks(values) {
        var result = [];

        if (values != null && values.length) {
            //show every {interval} point, we can show not more 32 points on chart for a good displaying
            var interval = Math.ceil(values.length / 32);

            for (var i = 0; i < values.length; ++i) {
                if (interval == 1) {
                    result.push([i, values[i].X]);
                } else {
                    if (i % interval == 0) {
                        result.push([i, values[i].X]);
                    } else {
                        result.push([i, ""]);
                    }
                }

            }
        }
        return result;
    }

    function getXvaluesForTooltips(values) {
        var array = [];

        if (values != null && values.length) {
            for (var i = 0; i < values.length; ++i) {
                if (values[i].hasOwnProperty("perc_activeUsers") && values[i].hasOwnProperty("perc_allUsers")) {
                    array.push([i, values[i].X, values[i].perc_activeUsers, values[i].perc_allUsers]);
                } else {
                    array.push([i, values[i].X]);
                }
            }
        }
        return array;
    }

    function getPointHoverHandler(tooltip, dataForTooltips, buildTooltip_Function) {
        var lastPoint = null;

        return function (e, pos, item) {
            if (!item) {
                tooltip.hide();
                lastPoint = null;
                return;
            }

            // fake series
            if (item.series.curvedLines.apply) {
                return;
            }

            if (lastPoint == item.dataIndex) {
                return;
            }

            if (item.series.xaxis.ticks[item.dataIndex] == undefined) {
                return;
            }

            lastPoint = item.dataIndex;
            var xlabel = dataForTooltips[item.dataIndex][1];

            var coord = _.find(item.series.data, function(it) {
                return it[0] == item.datapoint[0];
            });
            
            var ylabel = !item.series.stack ? item.datapoint[1] : coord[1];

            var color = item.series.color;
            var tip;

            if(buildTooltip_Function && typeof(buildTooltip_Function) == "function") {
                tip = buildTooltip_Function(item, xlabel, ylabel);
            } else {
                if (item.series.shlabintip) {
                    tip = '<div class="chart-tooltip-header">' + xlabel + '</div><div class="chart-tooltip-body" style="color: ' + color + '">' + item.series.label + ': ' + ylabel + '</div>';
                } else {
                    tip = '<div class="chart-tooltip-header">' + xlabel + '</div><div class="chart-tooltip-body" style="color: ' + color + '">' + ylabel + '</div>';
                }
            }

            tooltip.html(tip);
            tooltip.show();
            tooltip.css({ left: item.pageX - (tooltip.outerWidth(true) / 2), top: item.pageY - (tooltip.outerHeight(true) + 5) });
        };
    }

    function getDataSeries(values) {
        var result = [];

        if (values != null && values.length) {
            for (var i = 0; i < values.length; i++) {
                if (values[i].hasOwnProperty("perc_activeUsers") && values[i].hasOwnProperty("perc_allUsers")) {
                    result.push([i, values[i].Y, values[i].perc_activeUsers, values[i].perc_allUsers]);
                } else {
                    result.push([i, values[i].Y]);
                }
            }
        }

        return result;
    }

})(jQuery);