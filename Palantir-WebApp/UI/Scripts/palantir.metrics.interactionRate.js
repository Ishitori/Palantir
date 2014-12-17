InteractionRateUiController = function (options) {
    $.extend(this.options, options);
    this.initialize();
};

InteractionRateUiController.prototype = {
    options: {
        chartContainer: "#interactionRateChart",
        frequencyChartContainer: '#interactionFrequencyChart',
        averageCountChart: '#averageCountChart'
    },

    chartContainer: null, frequencyChartContainer: null, averageCountChart: null,

    initialize: function () {
        this.chartContainer = $(this.options.chartContainer);
        this.frequencyChartContainer = $(this.options.frequencyChartContainer);
        this.averageCountChart = $(this.options.averageCountChart);

        var filter = new UiPeriodFilter({
            initialized: $.proxy(this.filterChanged, this),
            filterChanged: $.proxy(this.filterChanged, this)
        });

        var url = this.frequencyChartContainer.data("source");
        LoadChart(this.frequencyChartContainer, url, null, this.frequencyChart_BuildTooltip);
    },

    filterChanged: function (event, filterCriteria) {
        if (!filterCriteria || !filterCriteria.isValid) {
            return;
        }
        var chartUrl = this.chartContainer.data("source");
        var averageCountChartUrl = this.averageCountChart.data("source");
        LoadChart(this.chartContainer, chartUrl, filterCriteria);
        LoadChart(this.averageCountChart, averageCountChartUrl, filterCriteria);
    },

    frequencyChart_BuildTooltip: function (item, xlabel, ylabel) {
        var tip;
        var data_forTooltip = item.series.data_forTooltip;
        var color = item.series.color;
        if (data_forTooltip.length > 0 && data_forTooltip[0].length > 3) {
            var perc_fromActiveUsers = data_forTooltip[item.dataIndex][2];
            var perc_fromAllUsers = data_forTooltip[item.dataIndex][3];

            // F+, users made >= xlabel actions
            if (item.seriesIndex != 0) {
                tip = '<div class="chart-tooltip-header">Только ' + xlabel + ' ' + getActionText_Extended(xlabel, 0) + '</div>'
                + '<div class="chart-tooltip-body" style="color: ' + color + '">' + ylabel + ' ' + getActionText_Extended(ylabel, 1) + '</div>';
            } else {
                // F, users made = xlabel actions
                tip = '<div class="chart-tooltip-header">Более ' + xlabel + ' ' + getActionText(xlabel) + '</div>'
                + '<div class="chart-tooltip-body" style="color: ' + color + '">' + ylabel + ' ' + getActionText_Extended(ylabel, 1) + '</div>';
            }

            tip += '<div class="chart-tooltip-info">' + '<em style="color: ' + color + '">' + perc_fromAllUsers + '</em>' + ' от всех в группе</div>'
                + '<div class="chart-tooltip-info">' + '<em style="color: ' + color + '">' + perc_fromActiveUsers + '</em>' + ' от активных</div>';
        }

        return tip;
    }
};

function getActionText(num_actions) {
    var s_num_actions = num_actions + '';

    // 1, 21, 31, 101, 121, 101392783721
    if (num_actions == 1 || ((num_actions - 1) % 10 == 0 && (s_num_actions.slice(s_num_actions.length - 2) != 11))) {
        return 'действия';
    } else {
        return 'действий';
    }
}

function getActionText_Extended(num_actions, version) {
    var s_num_actions = num_actions + '';

    // 1, 21, 31, 101, 121 действие
    if (num_actions == 1 || ((num_actions - 1) % 10 == 0 && (s_num_actions.slice(s_num_actions.length - 2) != 11))) {
        return version == 0 ? 'действие' : 'участник';
    }

    // 2, 3, 4, 22, 23, 24, 102, 103, 104 действия
    var num_action_lastChar = s_num_actions[s_num_actions.length - 1];
    var num_action_prevLastChar = s_num_actions[s_num_actions.length - 2];
    if (num_action_lastChar >= 2 && num_action_lastChar <= 4 && num_action_prevLastChar != 1) {
        return version == 0 ? 'действия' : 'участника';
    }

    // 5 - 20, 25 - 30, 95 - 100, 105 - 120
    return version == 0 ? 'действий' : 'участников';
}