/* Manages Controls and UI*/
var appControlsFtn = function () {
    var MENU_TO_ACTIVATE = null;
    var SERVER_DATE_TODAY = null;

    var watchedCollection = watchedCollectionManagerFtn();

    var migrateOldBootstrapPagination = function () {
        $("ul.pagination li").addClass("page-item");
        $("ul.pagination li>a").addClass("page-link");
        var span = $("ul.pagination li.active>span");
        var pageNo = span.text();
        span.remove();
        $("ul.pagination li.active").append('<a class="page-link" href="#">' + pageNo + ' <span class="sr-only">(current)</span></a>');
    };

    var formatControls = function () {
        $('select').selectpicker('destroy');

        $("input[type='text']:not(.form-control),input[type='password']:not(.form-control),input[type='email']:not(.form-control)").addClass("form-control");

        $("ol.breadcrumb>li:not(.breadcrumb-item)").addClass("breadcrumb-item");

        $(".datepicker").each(function (i, d) {
            var dateBox = $(d);
            var configParams = {
                format: "YYYY-MM-DD",
                showClear: true,
                showTodayButton: true,
                showClose: true,
                widgetPositioning: { vertical: "bottom" }
            };

            if (dateBox.hasClass("present")) {
                configParams.maxDate = app.controls.SERVER_DATE_TODAY;
            }
            else if (dateBox.hasClass("no-past")) {
                configParams.minDate = app.controls.SERVER_DATE_TODAY;
            }

            if (dateBox.hasClass("show-time")) {
                configParams.format = "YYYY-MM-DD HH:mm";
                configParams.sideBySide = true;
            }
            else if (dateBox.hasClass("time-only")) {
                configParams.format = "HH:mm";
            }

            var currentValue = dateBox.val();

            dateBox.datetimepicker(configParams);

            dateBox.val(currentValue);
        });

        watchedCollection.update();

        //enable searchable combos
        $("select").selectpicker({
            liveSearch: true,
            container: "body"
        });

        //enable fixed headers
        $(".table-header-fixed").each(function (i, d) {
            var table = $(d);

            if (!table.hasClass("configured")) {
                table.floatThead({
                    scrollContainer: function (table) {
                        return table.closest('.table-responsive');
                    }
                });
                table.addClass("configured");
            }
        });

        //rebind form validation
        $('form').removeData('validator');
        $('form').removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse('form');

        $("input.require-commas").change();

        $(window).trigger("resize");
    };

    var isFormValid = function (f) {
        var form = $(f);

        if (form.data("validator") != null) form.data("validator").settings.ignore = ":disabled";//enable validation of non-active tabs
        var validator = form.validate();
        var isValid = validator.form();

        if (!isValid) {//open offending tab if any
            var invalidControlSelector = "";
            
            $([".input-validation-error"]).each(function (i, selector) {
                $(["select", "input", "textarea"]).each(function (j, control) {
                    invalidControlSelector += (invalidControlSelector.length > 0 ? "," : "") + control + selector;
                });
            });

            var firstInvalidControl = form.find(invalidControlSelector);
            //console.log(firstInvalidControl.attr("id"));

            if (firstInvalidControl.length > 0) {
                var tabpane = firstInvalidControl.eq(0).closest(".tab-pane");

                if (tabpane.length > 0) {
                    var tab = form.find(".nav-tabs");
                    tab.find("a[href='#" + tabpane.attr("id") + "']").tab("show");
                }
            }
        }

        return isValid;
    };

    var disableFormSubmission = function (controlId) {
        $("#" + controlId).closest("form").on("submit", function () { return false; });
    };

    var updateRefreshComboOptions = function (combo) {
        $(combo).selectpicker('refresh');
    };

    var ensureFormValidBeforeSubmission = function (childControlSelector) {
        var form = $(childControlSelector).closest("form");

        form.on("submit", function () {
            var form = $(childControlSelector).closest("form");
            if (!isFormValid(form)) {
                alert('Please fill all required fields!');
                return false;
            }
            return true;
        });
    };

    var addCommasToInput = function () {
        var input = this;
        var val = $(input).val();

        val = val.replace(/,/g, "");
        val = val.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");

        $(input).val(val);
    };

    var removeCommas = function (val) {
        return val.replace(/,/g, "");
    };

    var getNumericValue = function (text) {
        var result = 0;

        try {
            result = removeCommas(text) * 1;

            if (isNaN(result)) {
                result = 0;
            }
        }
        catch (e) { }

        return result;
    };

    var addCommas = function (val) {
        return val.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
    };

    var repositionDropdowns = function () {
        $(this).find(".dropdown-multi-div").each(function (i, d) {
            var div = $(d);
            var width = div.width();
            var parent = div.parent();
            var parentLeft = parent.offset().left - 20;
            var parentWidth = parent.width();

            if (width > 150) {
                var correction = ((width / 2) - (parentWidth/2));
                if (correction > parentLeft) {
                    correction = parentLeft;
                }
                div.css("margin-left", (correction * -1) + "px");
            }
        });
    };

    function refreshMasterCheck() {
        $(".master-checkbox").prop("checked", $(".item-checkbox:not(:checked)").length == 0);
    }

    function implementMasterCheck() {
        if ($(".master-checkbox").is(":checked")) updateChildBoxes(".item-checkbox:not(:checked)", true);
        else updateChildBoxes(".item-checkbox:checked", false);
    }

    function updateChildBoxes(selector, value) {
        $(selector).each(function (i, d) {
            $(d).prop("checked", value);
        });
    }

    /**
     * Process Checked Records
     * @param {Function} callbackFtn A function that receives an array of values
     * @param {Boolean} ignoreAtleast1Validation If set to true, ignores the validation for selecting atleast 1 record
     */
    function processCheckedRecords(callbackFtn,ignoreAtleast1Validation) {
        var toolIds = [];

        $(".item-check:checked").each(function (i, d) {
            toolIds.push($(d).val());
        });

        if (!ignoreAtleast1Validation) {
            if (toolIds.length == 0) {
                alert("Select atleast 1 record!");
                return;
            }
        }

        callbackFtn(toolIds);
    }

    return {
        MENU_TO_ACTIVATE: MENU_TO_ACTIVATE,
        SERVER_DATE_TODAY: SERVER_DATE_TODAY,
        initialize: function () {
            $("#" + this.MENU_TO_ACTIVATE).addClass("active");

            migrateOldBootstrapPagination();

            //bind remove row
            $("body").on("click", ".remove-row", function (event) {
                $(event.target).closest("tr").remove();
                formatControls();
            });

            $("body").on("keyup change", "input.require-commas", addCommasToInput);
            $("body").on("change", "select.change-triggers-postback", function (event) {
                $(event.target).closest("form").submit();
            });
            $("body").on("click", "item-checkbox", refreshMasterCheck);
            $("body").on("click", "master-checkbox", implementMasterCheck);

            formatControls();

            $(".dropdown").on("shown.bs.dropdown", repositionDropdowns);
            repositionDropdowns();
        },

        formatControls: formatControls,
        isFormValid: isFormValid,
        disableFormSubmission: disableFormSubmission,
        updateRefreshComboOptions: updateRefreshComboOptions,
        ensureFormValidBeforeSubmission: ensureFormValidBeforeSubmission,
        getNumericValue: getNumericValue,
        addCommas: addCommas,
        watchedCollection: watchedCollection,
        processCheckedRecords:processCheckedRecords
    };
};