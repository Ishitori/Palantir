LoadCharts = function (options) {
    $.extend(this.options, options);
    this.initialize();
};
LoadCharts.prototype = {
    options: { ageChart: '#ageChart', genderChart: '#genderChart', educationChart: '#educationChart' },
    ageChart: null, genderChart: null, educationChart: null, countryChart: null, cityChart: null,
    initialize: function() {
        this.ageChart = $(this.options.ageChart);
        this.genderChart = $(this.options.genderChart);
        this.educationChart = $(this.options.educationChart);
        this.countryChart = $(this.options.countryChart);
        this.cityChart = $(this.options.cityChart);
        LoadPieChart(this.ageChart, this.options.data.AgeData);
        LoadPieChart(this.genderChart, this.options.data.GenderData);
        LoadPieChart(this.educationChart, this.options.data.EducationData);
    }
}