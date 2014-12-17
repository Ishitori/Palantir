PostsUiController = function (options) {
    $.extend(this.options, options);
    this.initialize();
};

PostsUiController.prototype = {
    options: {
        postsTable: "#mostPopularPostsContainer"
    },

    postsTable: null,

    initialize: function () {       
        this.postsTable = $(this.options.postsTable);

        var filter = new UiPeriodFilter({
            initialized: $.proxy(this.filterChanged, this),
            filterChanged: $.proxy(this.filterChanged, this)
        });
    },

    filterChanged: function (event, filterCriteria) {
        if (!filterCriteria || !filterCriteria.isValid)
        {
            return;
        }

        var url = $("#mostPopularPostsContainer").data("source");
        var dateFilter = Palantir.getDateFilter(filterCriteria);

        $.get(url, dateFilter, function (data) {
            $("#mostPopularPostsContainer").html(data);
            new UiTable($("#mostPopularPosts"));
        });
    }
};