$(function () {
    var filters = new AudienceFilters(window.globalAudienceFilter);
    var updater = new AudienceDataUpdater();
    filters.init();
    if (window.location.href.indexOf('#') > 0 && window.location.href.indexOf('&') > 0)
        {
    var filterString = window.location.href.split('#')[1].split('&');
        if (filterString) {
            var filter = {
                male: filterString[0].split('=')[1] == 'true',
                female: filterString[1].split('=')[1] == 'true',
                code: filterString[2].split('=')[1],
                citiesId: filterString[3].split('=')[1].split('%2C'),
                mineducation: filterString[4].split('=')[1],
                maxeducation: filterString[5].split('=')[1],
                minage: filterString[6].split('=')[1],
                maxage: filterString[7].split('=')[1]
            };
            if (filter.citiesId[0] == "") {
                filter.citiesId.length = 0;
            }
            $("#age").slider({ values: [filter.minage, filter.maxage] });
            $("#ageLabel").html(filter.minage + " - " + filter.maxage);
            
            $("#education").slider({ values: [filter.mineducation, filter.maxeducation] });
            var educations = ['Не указано', 'Среднее', 'Неполное высшее', 'Высшее', 'Ученая степень']
            $("#educationLabel").html(educations[filter.mineducation] + " - " + educations[filter.maxeducation]);
            if (filter.male) {
                $('#male').attr('checked', true);
            }
            if (filter.female) {
                $('#female').attr('checked', true);
            }
            for (i = 0; i < filter.citiesId.length; i++) {
                var name = $('option[value = ' + filter.citiesId[i] + ']').text();
                addCityItem(filter.citiesId[i], name);
            }
        }
    }
    //updater.update();
    $('#update').click(function () {
        updater.update();
    });
    
    function addCityItem(cid, name) {
        if (!$('#cities li[cityid=' + cid + ']').length) {
            var li = $('<li></li>');
            li.attr('cityid', cid);
            li.html(name);
            li.append('&nbsp;').append("<a href='javascript:void();' onclick='$(this).parent().remove();'>X</a>");
            $('#cities').append(li);
            var str = "[value='" + cid + "']";
            $(str).prop("disabled", true);
            $(str).hide();
        }
    }
});

function AudienceFilters(filter) {

    this.filter = filter;
    var self = this;

    var initGenderFilter = function() {
        if (self.filter.Male) {
            $('#male').attr('checked', true);
        }
        if (self.filter.Female) {
            $('#female').attr('checked', true);
        }
    };


    var initAgeFilter = function () {
        function resolveUndefined(val) {
            var noValueLabel = 'Не указано';
            return val == 0 ? noValueLabel : val;
        }

        $('#age').slider({
            range: true,
            min:  0,
            max: 100,
            step: 5,
            values: [self.filter.MinAge || 0, self.filter.MaxAge || 100],
            slide: function (event, ui) {
                var leftLabel = resolveUndefined(ui.values[0]);
                var rightLabel = resolveUndefined(ui.values[1]);
                $("#ageLabel").html(leftLabel + " - " + rightLabel);
            }
        });

        $("#ageLabel").html(resolveUndefined($("#age").slider("values", 0)) + " - " + resolveUndefined($("#age").slider("values", 1)));
    };

    var initEducationFilter = function () {
        var educations = ['Не указано', 'Среднее', 'Неполное высшее', 'Высшее', 'Ученая степень'];
        $('#education').slider({
            range: true,
            min: 0,
            max: 4,
            step: 1,
            values: [self.filter.MinEducation || 0, self.filter.MaxEducation || 4],
            slide: function (event, ui) {
                $("#educationLabel").html(educations[ui.values[0]] + " - " + educations[ui.values[1]]);
            }
        });

        $("#educationLabel").html(educations[$("#education").slider("values", 0)] + " - " + educations[$("#education").slider("values", 1)]);
    };

    var initCityFilter = function () {
        $("#city").combobox({
            select: function (event, ui) {
                addCityItem(this.value, ui.item.innerHTML);
            }
        });

        if (self.filter.Cities != null) {
            var cities = self.filter.Cities.split(',');
            for (var i = 0; i < cities.length; i++) {
                var id = cities[i];
                addCityItem(id, $('#city option[value=' + id + ']').html());
            }
        }

        function addCityItem(cid, name) {
            if (!$('#cities li[cityid=' + cid + ']').length) {
                var li = $('<li></li>');
                li.attr('cityid', cid);
                li.html(name);
                li.append('&nbsp;').append("<a href='javascript:void();' onclick='$(this).parent().remove();'>X</a>");
                $('#cities').append(li);
                var str = "[value='" + cid + "']";
                $(str).prop("disabled", true);
                $(str).hide();
            }
        }
    };

    this.init = function () {
        initAgeFilter();
        initEducationFilter();
        initCityFilter();
        initGenderFilter();
    };
}

function AudienceDataUpdater() {
    var createFilterModel = function () {
        var audienceFilter = {
            "male": $('#male').is(':checked'),
            "female": $('#female').is(':checked'),
            "code": Math.floor(Math.random() * 100000000),
            "cities": (function () {
                var cities = [];
                $('#cities li').each(function () {
                    var cityId = $(this).attr('cityid');
                    cities.push(cityId);
                });
                return cities.join(',');
            })()
        };
        
        audienceFilter.mineducation = $("#education").slider("values", 0);
        audienceFilter.maxeducation = $("#education").slider("values", 1);
        audienceFilter.minage = $("#age").slider("values", 0);
        audienceFilter.maxage = $("#age").slider("values", 1);
        return audienceFilter;
    };

    this.update = function () {
        var filter = createFilterModel();
        $('.lbl, .ibl').block({
            message: '<img src="../../Content/images/ajax-loader.gif" />'
        });
        $("#searchResult").show();
        $.ajax({
            url: "audience/audiencedata",
            dataType: "json",
            success: function (model)
            {
                var ref = this.url.split('?')[1];
                    var tempref = location.href.split('#');
                    tempref[1] = ref;
                    location.href = tempref[0] + '#' + tempref[1];

                $('#count').html(model.Count);
                if (model.Count > 0)
                {
                    var params = { filterCode: model.FilterCode };
                    LoadPieChart($('#menWomenChart'), 'audience/genderchart', params);
                    LoadPieChart($('#ageChart'), 'audience/agechart', params);
                    LoadPieChart($('#educationChart'), 'audience/educationchart', params);
                    LoadPieChart($('#cLRChart'), 'audience/CLRChart', params);
                    LoadPieChart($('#divDivTypeOfContentChart'), 'audience/GetTypeOfContentChart', params);
                    
                    $.get('audience/MemberSubTable', params, function (data) {
                        $("#memberSub").html(data);
                    });
                    
                    $.get('audience/citiesmozaic', params, function (data) {
                        new TreeView('#locations', data, '#locations-hint', '#locations-legend');
                        $('.lbl').unblock();
                    });
                    
                    $.get('audience/interestsmozaic', params, function (data) {
                        var treeInit = function () {
                            new TreeView('#interests', data, '#interests-hint', '#interests-legend');
                            $('.ibl').unblock();
                        };
                        treeInit();
                        $(window).resize(function () {
                            $('#interests').html('');
                            treeInit();
                        });
                    });
                }
            },

            data: filter
        });
    };
}