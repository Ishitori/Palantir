ExportUiController = function (options) {
    $.extend(this.options, options);
    this.initialize();
};

ExportUiController.prototype = {
    options: {
        exportDataButtonId: "#exportDataButton",
        exportDataButtonContainerId: "#exportButtonContainer",
        exportProcessingContainerId: "#exportProcessingContainer",
        exportResultLinkId: "#exportResultLink",
        exportFailedMessageId: "#exportFailedMessage",
        exportResultContainerId: "#exportResultContainer"
    },

    exportDataButton: null,
    exportDataButtonContainer: null,
    exportProcessingContainer: null,
    exportResultContainer: null,
    exportResultLink: null,
    exportFailedMessage: null,
    exportCheckTimeout: null,

    initialize: function () {
        this.exportResultContainer = $(this.options.exportResultContainerId);
        this.exportDataButton = $(this.options.exportDataButtonId);
        this.exportDataButtonContainer = $(this.options.exportDataButtonContainerId);
        this.exportResultLink = $(this.options.exportResultLinkId);
        this.exportFailedMessage = $(this.options.exportFailedMessageId);
        this.exportProcessingContainer = $(this.options.exportProcessingContainerId);

        this.exportProcessingContainer.hide();
        this.exportResultContainer.hide();
        var filter = new UiPeriodFilter();

        this.exportDataButton.click($.proxy(function() {
            this.exportDataButtonContainer.hide();
            this.exportProcessingContainer.show();
            var filterCriteria = filter.getFilterCriteria();
            var parsedCriteria = Palantir.getDateFilter(filterCriteria);
            var url = this.exportDataButtonContainer.data("source");
            
            $.get(url, parsedCriteria, $.proxy(function (result) {
              this.exportDataButtonContainer.hide();
              this.exportResultContainer.hide();
              this.exportProcessingContainer.show();
              this.exportCheckTimeout = setTimeout($.proxy(function() { this.checkIfExportDone(result.TicketId); }, this), 5000);
            }, this))
                .fail($.proxy(function () {
                    this.showExportFailed();
                }, this));
          }, this));
    },

    checkIfExportDone: function(ticketId)
    {
      var checkUrl = this.exportDataButtonContainer.data("check");

      $.get(checkUrl, { id: this.exportDataButtonContainer.data("id"), ticketId: ticketId }, $.proxy(function (result) {
        clearTimeout(this.exportCheckTimeout);

        if (result.IsFinished)
        {
          this.exportDataButtonContainer.hide();
          this.exportProcessingContainer.hide();

          if (result.IsSuccess)
          {
              this.exportFailedMessage.hide();
              this.exportResultLink.attr('href', result.FileUrl);
              this.exportResultLink.show();
          }
          else
          {
              this.exportResultLink.hide();
              this.exportFailedMessage.show();
          }

          this.exportResultContainer.show();
        }
        else
        {
          this.exportCheckTimeout = setTimeout($.proxy(function() { this.checkIfExportDone(result.TicketId); }, this), 5000);
        }
      }, this));
    },
    
    showExportFailed: function()
    {
        this.exportDataButtonContainer.hide();
        this.exportProcessingContainer.hide();
        this.exportResultLink.hide();
        this.exportFailedMessage.show();
        this.exportResultContainer.show();
    }
};