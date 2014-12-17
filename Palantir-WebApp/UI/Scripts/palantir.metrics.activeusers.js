ActiveUsersUiController = function (options) {
    $.extend(this.options, options);
    this.initialize();
};
ActiveUsersUiController.prototype = {
    options: {postsTable: "#mostActiveUsersContainer",},
    initialize: function() {
        var filter = new UiPeriodFilter({
            initialized: $.proxy(this.filterChanged, this),
            filterChanged: $.proxy(this.filterChanged, this)
        });
    },
    filterChanged: function(event, filterCriteria) {
        if (!filterCriteria || !filterCriteria.isValid) {
            return;
        }
        document.getElementById("message").style.display = 'none';
        document.getElementById("load").style.display = 'block';
        document.getElementById("mostActiveUser").style.display = 'none';
        var url = $("#mostActiveUsersContainer").data("source");
        var dateFilter = Palantir.getDateFilter(filterCriteria);
        $.get(url, dateFilter, function(data) {
            document.getElementById("load").style.display = 'none';
            if ((data.Table) && (data.InterestsData) && (data.AgeData) && (data.GenderData) && (data.EducationData) && (data.CountryAndCityData)) {
                document.getElementById("mostActiveUser").style.display = 'block';
                $("#mostActiveUsersContainer").html(data.Table);
                var tableObject = new UiTable($("#mostActiveUsers"));
                var treeInit = function () {
                    new TreeView('#interests', JSON.parse(data.InterestsData), '#interests-hint', '#interests-legend');
                };
                treeInit();
                
                var treeCCInit = function () {
                    new TreeView('#countryAndCityData', JSON.parse(data.CountryAndCityData), '#countryAndCity-hint', '#countryAndCity-legend');
                };
                treeCCInit();
                $(window).resize(function () {
                    $('#countryAndCityData').html('');
                    treeCCInit();
                    $('#interests').html('');
                    treeInit();
                });
                new LoadCharts({ 'data': data });
            } else {
                document.getElementById("message").style.display = 'block';
            }
        });
    }
};