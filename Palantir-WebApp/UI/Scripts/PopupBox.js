(function($)
{
    if (!window.PopupBox)
    {
        window.PopupBox = PopupBox;
    }
    function PopupBox(options) {
        var pThis = this,
            settings = $.extend({ align: 'right' }, options);

        pThis.settings = settings;

        function open(event) {
            var box = $('#' + pThis.settings.boxId);
            var evTarget = getTarget(event.target);
            var offset = getOffset(evTarget);
            box.css({
                'top': getTop(offset, evTarget, box[0]),
                'left': getLeft(offset, evTarget, box[0]),
                'visibility': "visible",
                'opacity': 0
            }).animate({ opacity: 1 }, 100);
            box[0].style.visibility = 'visible';
        }

        function close() {
            $('#' + pThis.settings.boxId)[0].style.visibility = 'hidden';
        }

            function resize() {
                var box = $('#' + pThis.settings.boxId);

                if (box[0].visibility == 'hidden') {
                    return;
                }

                var target = document.getElementById(pThis.settings.targetId);
                var offset = getOffset(target);
                box.css({ 'top': getTop(offset, target, box[0]), 'left': getLeft(offset, target, box[0]) });
            }

            function getTop(offset, target, box) {
                return offset.top + target.offsetHeight;
            }

            function getLeft(offset, target, box) {
                return offset.left + target.offsetWidth - box.offsetWidth;
            }

            function getTarget(target) {
                var link = document.getElementById(pThis.settings.targetId);
                var box = document.getElementById(pThis.settings.boxId);

                if (target == link) {
                    return target;
                }

                while (target != document) {
                    target = target.parentNode;

                    if (target == link || target == box) {
                        return target;
                    }
                }

                return target;
            }

            function initialize() {
                $(document).click(function(event) {
                    var link = document.getElementById(pThis.settings.targetId);
                    var box = document.getElementById(pThis.settings.boxId);
                    var evTarget = getTarget(event.target);

                    if (evTarget != link && evTarget != box) {
                        close();
                    }
                });

                $(document).bind('keyup', function(event) {
                    if (event.keyCode == 27) {
                        close();
                    }
                });

                $(window).resize('keyup', function(event) {
                    resize();
                });

                $('#' + pThis.settings.targetId).bind('click', open);
            }

            initialize();
        }

}(jQuery));