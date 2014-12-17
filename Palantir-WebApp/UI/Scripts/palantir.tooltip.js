ToolTipLoader = function (container, toolTipOptions) {
    var defaultOptions = {
        animation: 'fade',
        delay: 350,
        interactive: true,
        interactiveTolerance: 2000,
        onlyOne: true,
        position: 'top',
        speed: 350,
        touchDevices: true,
        trigger: 'hover',
    };
    var options = toolTipOptions ? $.extend({}, defaultOptions, toolTipOptions)
                                : defaultOptions;
    
    $(container).tooltipster(options);
}