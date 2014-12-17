    TreeView = function (containerSelector, data, hintSelector, legendSelector) {
        this.containerSelector = containerSelector;
        this.data = data;
        this.hintSelector = hintSelector;
        this.legendSelector = legendSelector;
        this.init();
    };

    TreeView.prototype = {
        init: function () {
            var self = this;
            clear();
            var width = $('.mainContainer').width();
            var height = width * 0.45;
            $(this.containerSelector).css({ position: 'relative', height: height, width: width });
            
            
            var color = d3.scale.category10();
            var treemap = d3.layout.treemap()
                .size([width, height])
                .sticky(true)
                .value(function (d) { return d.size; });

            d3.select(this.containerSelector)
                .datum(this.data)
                .selectAll(".node")
                .data(treemap.nodes)
                .enter().append("div")
                .attr("class", "node")
                .call(position)
                .style("background", function (d) {
                    var clr = color(!d.children ? d.parent.name: d.name);
                    if (d.children && d.depth != 0) {
                        addLegend(d, clr);
                    }
                    return clr;
                })
                
                .html(function (d) {
                    if (!d.children) {
                        d.w = d.name.width();
                        var maxWidth = (d.dx - 8) + 'px';
                        var maxHeight = (d.dy - 8) + 'px';
                        var isLabelVisible = (d.dx * d.dy) > (d.w * 16)  ? 1 : 0;
                        var html = "<p class='treeview-node-label' style='max-width:" + maxWidth + ";max-height:" + maxHeight + (!isLabelVisible ? ";display:none;" : "") + "'>" + d.name + "<br/>" + d.size + "</p>";
                        html += "<p style='display:none;'>. " + d.groupPercent + " от '" + d.parent.name + "', " + d.totalPercent + " от всего.</p>";
                        return html;
                    }
                    return null;
                })
                .on('mouseover', function () {
                    var parentId = '#' + $(this)[0].offsetParent.id;
                    $(parentId + " .node" +
                        "").stop().fadeTo(300, 0.4);
                    $(this).stop().fadeTo(0, 1.0);
                    var hint = $(this).children(':first').html();
                    var percentage = $($(this).children()[1]).html();
                    
                    hint = hint.replace('<br>', ' - ');
                    hint += percentage;
                    
                    $(self.hintSelector).html(hint);
                })
                .on('mouseleave', function () {
                    $('.node').stop().fadeTo('fast', '1');
                    $(self.hintSelector).html("");
                });

            function position() {
                this.style("left", function (d) { return d.x + "px"; })
                    .style("top", function (d) { return d.y + "px"; })
                    .style("width", function (d) { return Math.max(0, d.dx - 1) + "px"; })
                    .style("height", function (d) { return Math.max(0, d.dy - 1) + "px"; });
            }

            function addLegend(e, clr) {
                var span = document.createElement('span');
                var img = document.createElement('img');
                $(img).css("background", clr);
                $(span).addClass("legendLabel");
                $(span).append(e.name);
                $(self.legendSelector).append(img);
                $(self.legendSelector).append(span);
            }

            function clear() {
                $(self.containerSelector).html('');
                $(self.legendSelector).html('');
            }
        }
    };


    String.prototype.width = function (font) {
        var f = font || 'normal normal normal 16px/normal trebuchet ms, arial, helvetica, verdana, sans-serif',
        o = $('<div>' + this + '</div>')
            .css({ 'position': 'absolute', 'float': 'left', 'white-space': 'nowrap', 'visibility': 'hidden', 'font': f })
            .appendTo($('body')),
        w = o.width();

    o.remove();

    return w;
};