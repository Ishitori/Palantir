CompareGroupsUiController = function (options) {
    $.extend(this.options, options);
    this.initialize();
};

CompareGroupsUiController.prototype = {
    options: {
        compareButtonId: "#concurrentButton",
        compareButtonContainerId: "#concurrentButtonContainer",
        comparisonProcessingContainerId: "#comparisonProcessingContainer",
        comparisonFailedMessageId: "#comparisonFailedMessage",
        comparisonResultContainerId: "#comparisonResultContainer",
        concurrentsOptionsId : "#concurrentsOptions"
    },

    compareButton: null,
    compareButtonContainer: null,
    comparisonProcessingContainer: null,
    comparisonResultContainer: null,
    comparisonFailedMessage: null,

    initialize: function () {
        this.comparisonResultContainer = $(this.options.comparisonResultContainerId);
        this.compareButton = $(this.options.compareButtonId);
        this.compareButtonContainer = $(this.options.compareButtonContainerId);
        this.comparisonFailedMessage = $(this.options.comparisonFailedMessageId);
        this.comparisonProcessingContainer = $(this.options.comparisonProcessingContainerId);
        this.concurrentOptionsContainer = $(this.options.concurrentsOptionsId);

        this.comparisonProcessingContainer.hide();
        this.comparisonResultContainer.hide();

        this.compareButton.click($.proxy(this.compareGroups, this));

        if (this.getConcurrentSelection().length > 0)
        {
            $.proxy(this.compareGroups, this)();
        }
    },

    getConcurrentSelection: function () {
        var item,
            result = [],
            checkboxes = this.concurrentOptionsContainer.find("input[type='checkbox']");

        for(var i = 0, len = checkboxes.length; i < len; i++)
        {
           item = checkboxes[i];

           if (item.checked)
           {
               result.push(item.value);
           }
        }

        return result;
    },

    compareGroups: function() {
        var filter = new UiPeriodFilter();
        var filterCriteria = filter.getFilterCriteria();
        var parsedCriteria = Palantir.getDateFilter(filterCriteria);
        var url = this.compareButtonContainer.data("source");
        var councurrents = this.getConcurrentSelection();
        parsedCriteria.items = councurrents.join(',');
        this.compareButtonContainer.hide();
        this.comparisonResultContainer.hide();
        this.comparisonProcessingContainer.show();

        $.get(url, parsedCriteria, $.proxy(function (result) {
                this.comparisonProcessingContainer.hide();
                this.compareButtonContainer.show();
                this.comparisonResultContainer.html(result);
                this.comparisonResultContainer.show();
            }, this))
                .fail($.proxy(function () {
                    this.showComparisonFailed();
                }, this));
    },
    
    showComparisonFailed: function()
    {
        this.compareButtonContainer.hide();
        this.comparisonProcessingContainer.hide();
        this.comparisonFailedMessage.show();
        this.comparisonResultContainer.show();
    }
};