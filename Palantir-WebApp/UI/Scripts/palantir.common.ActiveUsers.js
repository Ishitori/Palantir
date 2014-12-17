LoadPieChart = function (chartContainer, dataChart) {
    new PieChart(chartContainer, { 'data': dataChart });
};

UiTagCloud = function(containerId, onTagClicked, onLoadComlpete, datalist) {
    this.container = containerId;
    this.onTagClicked = onTagClicked || function() {};
    this.onLoadComlpete = onLoadComlpete || function () { };
    this.interestData = datalist.InterestsData;
    this.init();
};

UiTagCloud.prototype = {
    container: null,
    onTagClicked: null,
    interestData:null,
    init: function () {
            var tagList = $.map(this.interestData, $.proxy(function (el) {
                return  {
                            text: el.Text,
                            weight: el.Weight,
                            link: {href: el.Href, "data-id": el.DataId },
                            handlers: {
                                click: this.onTagClicked
                            }
                        };
            }, this));
            $(this.container).jQCloud(tagList);
            this.onLoadComlpete();
    }
}