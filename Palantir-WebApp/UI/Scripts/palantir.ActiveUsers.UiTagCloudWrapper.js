UiTagCloudWrapper = function (options) {
    $.extend(this.options, options);
    this.initialize();
};

UiTagCloudWrapper.prototype = {
    options: {userInterestestsId: '#userInterests'},
    initialize: function () {
        $(this.options.userInterestestsId).empty();
        new UiTagCloud(this.options.userInterestestsId, this.userInterestSelected, $.proxy(this.restoreUserInterestsState, this), this.options.data);
    },
    restoreUserInterestsState: function () {
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
    }
};