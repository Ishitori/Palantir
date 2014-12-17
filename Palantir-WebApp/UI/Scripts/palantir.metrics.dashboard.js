DashboardUiController = function(options) {
    $.extend(this.options, options);
    this.initialize();
};

DashboardUiController.prototype = {
    options: {
        metricsContainer: "#dashboardMetrics",
        metricsEmptyContainer: "#dashboardMetricsEmpty",
        chartContainer: "#dashboardChart",
    },

    metricsContainer: null,
    metricsEmptyContainer: null,
    chartContainer: null,

    initialize: function () {
        this.metricsContainer = $(this.options.metricsContainer);
        this.metricsEmptyContainer = $(this.options.metricsEmptyContainer);
        this.chartContainer = $(this.options.chartContainer);

        var filter = new UiPeriodFilter({
            initialized: $.proxy(this.filterChanged, this),
            filterChanged: $.proxy(this.filterChanged, this)
        });
    },

    filterChanged: function(event, filterCriteria) {

        if (!filterCriteria || !filterCriteria.isValid) {
            return;
        }

        var dashboardUrl = this.metricsContainer.data("source");
        var projectId = this.metricsContainer.data("id");

        var tooltip1 = {
            tooltipContainer: ".ir-tooltip", //Id or class
            options: {
                content: "Interaction Rate = (количество лайков+количество комментариев+количество «рассказать друзьям»)/количество постов/количество пользователей. Показатель отражает уровень взаимодействия пользователей группы с публикуемым контентом. Значение Interaction Rate зависит от выбираемого периода.",
            }
        };
        
        var tooltip2 = {
            tooltipContainer: ".rr-tooltip", //Id or class
            options: {
                content: "Response rate = (количество постов пользователей с ответами администраторов группы) / (общее количество постов пользователей). <br />Показатель отражает уровень взаимодействия администраторов группы с постами пользователей. <br />Значение Response rate зависит от выбираемого периода.",
            }
        };
        
        var tooltip3 = {
            tooltipContainer: ".rt-tooltip", //Id or class
            options: {
                content: "Response time = среднее количество времени, прошедшее с момента публикации поста пользователя до первого комментария администратора группы к этому посту. <br />Показатель учитывает только те посты пользователей, к которым были комментарии администраторов. <br />Значение Response time зависит от выбираемого периода.",
            }
        };
        
        var tooltip4 = {
            tooltipContainer: ".du-tooltip", //Id or class
            options: {
                content: "Количество участников группы с заблокированными и удаленными аккаунтами на последний день выбранного периода.",
            }
        };
        
        var tooltip5 = {
            tooltipContainer: ".ma-tooltip", //Id or class
            options: {
                content: "Отражает день недели и временной интервал, в течение которого было опубликовано наибольшее количество постов. <br />Значение показателя усредняется в зависимости от выбираемого периода. Таким образом, при выборе анализируемого периода «за день», показатель отражает часы наибольшего количества постов за последние 24 часа. <br />При выборе анализируемого периода «за неделю», «за месяц», «за год», «за другой период» показатель отражает день недели и часы наибольшего количества постов за этот период.",
            }
        };

        var tooltip6 = {
            tooltipContainer: ".post-tooltip", //Id or class
            options: {
                content: "Общее количество постов за выбранный период.",
            }
        };

        var tooltip7 = {
            tooltipContainer: ".tm-tooltip", //Id or class
            options: {
                content: "Общее количество тем, созданных в течение выбранного периода и общее количество сообщений, оставленных в течение выбранного периода во всех темах группы.",
            }
        };

        var tooltip8 = {
            tooltipContainer: ".v-tooltip", //Id or class
            options: {
                content: "Общее количество видеороликов, опубликованных в группе в течение выбранного периода.",
            }
        };
        
        var tooltip9 = {
            tooltipContainer: ".ph-tooltip", //Id or class
            options: {
                content: "Общее количество фотографий, опубликованных в течение выбранного периода.",
            }
        };
        
        var tooltip10 = {
            tooltipContainer: ".au-tooltip", //Id or class
            options: {
                content: "Количество пользователей, оставивших хотя бы один пост, комментарий, «мне нравится» или «рассказать друзьям» в течение выбранного периода.",
            }
        };
        
        var tooltip11 = {
            tooltipContainer: ".nau-tooltip", //Id or class
            options: {
                content: "Количество пользователей, не оставивших ни одного поста, и ни одного комментария, и ни одного нажатия кнопки «мне нравится», и ни одного нажатия кнопки «рассказать друзьям» в течение выбранного периода.",
            }
        };
        
        var tooltip12 = {
            tooltipContainer: ".t-tooltip", //Id or class
            options: {
                content: "Общее количество пользователей на последний день выбранного периода.",
            }
        };
        
        var tooltip13 = {
            tooltipContainer: ".cp-tooltip", //Id or class
            options: {
                content: "Среднее количество комментариев на пост = (общее количество комментариев) / (общее количество постов). Значение показателя зависит от выбираемого периода.",
            }
        };
        
        var tooltip14 = {
            tooltipContainer: ".lp-tooltip", //Id or class
            options: {
                content: "Среднее количество «мне нравится» на пост = (общее количество нажатий «мне нравится») / (общее количество постов). Значение показателя зависит от выбираемого периода.",
            }
        };
        
        var tooltip15 = {
            tooltipContainer: ".up-tooltip", //Id or class
            options: {
                content: "Среднее количество постов на пользователя = (общее количество постов, публикуемых пользователями) / (количество пользователей в группе). Значение показателя зависит от выбираемого периода.",
            }
        };
        
        var tooltip16 = {
            tooltipContainer: ".sp-tooltip", //Id or class
            options: {
                content: "Среднее количество «Рассказать друзьям» на пост = (общее количество нажатий «Рассказать друзьям»)/(общее количество постов). Значение показателя зависит от выбираемого периода.",
            }
        };
        var optionsList = [tooltip1, tooltip2, tooltip3, tooltip4, tooltip5, tooltip6, tooltip7, tooltip8, tooltip9, tooltip10, tooltip11, tooltip12, tooltip13, tooltip14, tooltip15, tooltip16];

        this.metricsContainer.empty().append(this.metricsEmptyContainer.html());
        LoadDashboard(this.metricsContainer, dashboardUrl, filterCriteria, optionsList);

        var chartUrl = this.chartContainer.data("source");
        LoadChart(this.chartContainer, chartUrl, filterCriteria);
    }
};