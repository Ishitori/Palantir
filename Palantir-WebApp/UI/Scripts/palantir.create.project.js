CreateProjectUiController = function (options, isAddGroupsDisabled) {
    $.extend(this.options, options);
    this.initialize(isAddGroupsDisabled);
};

CreateProjectUiController.prototype = {
    options: {
        createProjectDataButtonId: "#createProjectButton",
        createProjectDataButtonContainerId: "#createProjectButtonContainer",
        createProjectProcessingContainerId: "#createProjectProcessingContainer",
        tooltip: "#message"
    },

    createProjectDataButton: null,
    createProjectDataButtonContainer: null,
    createProjectProcessingContainer: null,
    tooltip: null,

    initialize: function (isAddGroupsDisabled) {
        this.createProjectDataButton = $(this.options.createProjectDataButtonId);
        this.createProjectDataButtonContainer = $(this.options.createProjectDataButtonContainerId);
        this.createProjectProcessingContainer = $(this.options.createProjectProcessingContainerId);
        this.tooltip = $(this.options.tooltip);

        this.createProjectProcessingContainer.hide();

        this.createProjectDataButton.click($.proxy(function() {
            var url = this.createProjectDataButtonContainer.data("source");
            var title = $("#Project_Title").val();
            var vkUrl = $("#Project_Url").val();
            if (vkUrl == '' || title == '') {
                alert('Нужно заполнить все поля!');
            } else {
                var temp = vkUrl.toLowerCase();
                if (temp.indexOf("vk.com") == 0 || temp.indexOf("http://vk.com") == 0) {
                    $.get(url, { Title: title, Url: vkUrl }, $.proxy(function(result) {
                        this.createProjectDataButtonContainer.hide();
                        this.createProjectProcessingContainer.show();
                        this.createProjectCheckTimeout = setTimeout($.proxy(function() { this.checkIfCreationDone(result.TicketId); }, this), 3000);
                    }, this));
                } else {
                    if (temp.indexOf("facebook.com") != -1) {
                        alert("В настоящее время Barberry.pro не поддерживает группы Facebook. Работы над поддержкой уже ведутся.");
                    } else {
                        alert('Данные адрес не является группой на ВКонтакте. Пожалуйста, укажите адрес группы ВКонтакте');
                    }
                }
            }
        }, this));
        
        if (isAddGroupsDisabled)
        {
            $("#Project_Title").attr("disabled", true);
            $("#Project_Url").attr("disabled", true);
            $("#createProjectButton").attr("disabled", true);
            $("#createProjectButton").css({ cursor: "default" });
            document.getElementById("message").style.display = 'block';
        }
    },

    checkIfCreationDone: function (ticketId) {
        var checkUrl = this.createProjectDataButtonContainer.data("check");

        $.get(checkUrl, { ticketId: ticketId }, $.proxy(function (result) {
            clearTimeout(this.createProjectCheckTimeout);

            if (result.IsFinished) {
                this.createProjectDataButtonContainer.hide();
                this.createProjectProcessingContainer.hide();

                if (result.IsSuccess) {
                    window.location = result.ProjectUrl;
                }
                else {
                    alert("Не удалось добавить группу. Попробуйте снова через некоторое время.");
                }
            }
            else {
                this.createProjectCheckTimeout = setTimeout($.proxy(function () { this.checkIfCreationDone(result.TicketId); }, this), 3000);
            }
        }, this));
    }
};