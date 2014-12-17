SocialUiController = function(options) {
    $.extend(this.options, options);
    this.initialize();
};

SocialUiController.prototype = {
    options: {
        socialChart: "#socialChart",
        menWomenChart: "#menWomenChart",
        ageChart: "#ageChart",
        educationChart: "#educationChart",
    },

    socialChart: null,
    menWomenChart: null,
    ageChart: null,
    educationChart: null,

    initialize: function () {
        this.socialChart = $(this.options.socialChart);
        this.menWomenChart = $(this.options.menWomenChart);
        this.menWomenChart = $(this.options.menWomenChart);
        this.ageChart = $(this.options.ageChart);
        this.educationChart = $(this.options.educationChart);

        var filter = new UiPeriodFilter({
            initialized: $.proxy(this.filterChanged, this),
            filterChanged: $.proxy(this.filterChanged, this)
        });

        LoadPieChart(this.menWomenChart, this.menWomenChart.data("source"));
        LoadPieChart(this.ageChart, this.ageChart.data("source"));
        LoadPieChart(this.educationChart, this.educationChart.data("source"));

        new UiTagCloud("#userInterests", this.userInterestSelected, $.proxy(this.restoreUserInterestsState, this));
    },

    restoreUserInterestsState: function() {
        setTimeout($.proxy(function () {
            var selectedTag = $.AnchorData("userInterests");
            if (selectedTag) {
                this.userInterestSelected($("#userInterests a[data-id='" + selectedTag + "']"));
            }
        }, this), 100);    
    },

    userInterestSelected: function (event) {
        if ($.isFunction(event.preventDefault)) event.preventDefault();
        var target = $(event.target || event);
        var selectedTag = target.data("id");
        if (!selectedTag) return;
        var interestName = target.text();
        $.get($("#userInterestsDetails").data("source"), { tagTitle: selectedTag }, function (data) {
            data = "<h3>Пользователи группы, интересующиеся «" + interestName + "»</h3>" + data;
            $("#userInterestsDetails").html(data);
            new UiTable($("#userInterestsDetails table"));
            $.AnchorData("userInterests", selectedTag);
        });
    },

    filterChanged: function (event, filterCriteria) {
        if (!filterCriteria || !filterCriteria.isValid)
        {
            return;
        }

        var url = this.socialChart.data("source");
        LoadChart(this.socialChart, url, filterCriteria);
    }
};