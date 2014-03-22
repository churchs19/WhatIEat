/*!
 * angular-winjs
 *
 * Copyright 2013 Josh Williams and other contributors
 * Released under the MIT license
 */
(function (global) {
    "use strict;"

    var module = angular.module("winjs", []);

    module.run(function ($rootScope) {
        var Scope = Object.getPrototypeOf($rootScope);
        var Scope$eval = Scope.$eval;
        Scope.$eval = function (expr, locals) {
            var that = this;
            return MSApp.execUnsafeLocalFunction(function () {
                return Scope$eval.call(that, expr, locals);
            });
        };
    })

    function getTemplate(elementType, transclude) {
        var template = "";
        if (elementType) {
            if (transclude) {
                template = "<" + elementType + " ng-transclude='true'></" + elementType + ">";
            } else {
                template = "<" + elementType + "></" + elementType + ">";
            }
        }
        return template;
    }

    function getBindableApi(ctor) {
        var properties = [];
        var events = [];
        var prototype = ctor.prototype;
        do {
            for (var key in prototype) {
                var pd = Object.getOwnPropertyDescriptor(prototype, key);
                if (pd && pd.get && pd.set) {
                    if (key[0] === "o" && key[1] === "n") {
                        events.push(key);
                    } else if (key[0] !== "_") {
                        properties.push(key);
                    }
                }
            }
        } while (prototype = Object.getPrototypeOf(prototype));

        var api = {};
        properties.forEach(function (key) {
            api[key] = "=?";
        });
        events.forEach(function (key) {
            api[key] = "&";
        });
        return api;
    }

    function createOptions(processors, $scope, keys, getControl, element) {
        processors = processors || {};
        function getValue(key) {
            return getControl()[key]
        }
        function update(key, $new, $old) {
            if ($new !== $old) {
                getControl()[key] = (processors[key] || angular.identity)($new, $old, getControl, element);
            }
        }
        var cleanup = [];
        var options = keys.reduce(function (options, key) {
            var value = $scope[key];
            if (value) {
                options[key] = (processors[key] || angular.identity)(value, null, getControl, element);
            }
            var cleanupFunction;
            if ((processors[key] || angular.identity).watch) {
                cleanupFunction = processors[key].watch($scope, key, getValue, options[key]);
            }
            cleanupFunction = cleanupFunction || $scope.$watch(key, function ($new, $old) {
                update(key, $new, $old);
            });
            cleanup.push(cleanupFunction);
            return options;
        }, {});
        return {
            options: options,
            cleanup: function () {
                cleanup.forEach(function (worker) {
                    worker();
                });
            },
        }
    }

    function controlDirectiveModel(ctor, createOptions, preLink, postLink, elementType, transclude) {
        var scopeSpec = getBindableApi(ctor);
        var scopeSpecKeys = Object.keys(scopeSpec);
        var template = getTemplate(elementType, transclude);
        return {
            restrict: "E", // @TODO, consider AE
            transclude: !!transclude,
            template: template,
            replace: !!elementType,
            scope: scopeSpec,
            link: function ($scope, elements, attrs) {
                var element = elements[0];
                var control;
                var options = createOptions($scope, scopeSpecKeys, function () { return control; }, element);
                preLink.forEach(function (f) { f($scope, options.options); });
                control = new ctor(element, options.options);
                postLink.forEach(function (f) { f($scope, control); });
                var cleanup = options.cleanup;
                $scope.$on("$destroy", function () {
                    if (control.dispose) {
                        control.dispose();
                    }
                    if (cleanup) {
                        cleanup();
                    }
                });
                return control;
            },
        };
    }

    // spec is an object which has: 
    // {
    //   elementType?: string, /* default is DIV */
    //   transclude?: boolean, /* default is false */
    //   optionsProcessors?: { name: (value) => value }, /* record of funcs by name */
    //   model?: [(model) => model], /* default is [] */
    //   preLink?: [($scope, options) => void], /* default is [] */
    //   postLink?: [($scope, control) => void], /* default is [] */
    // }
    function directive(modelGenerator, name, spec) {
        var shortName = "win" + name.substr(name.lastIndexOf(".") + 1);
        module.directive(shortName, function () {
            var ctor = WinJS.Utilities.getMember(name, global);
            var model = modelGenerator(
                ctor,
                createOptions.bind(null, spec.optionsProcessors || {}),
                spec.preLink || [],
                spec.postLink || [],
                spec.elementType || "DIV",
                spec.transclude || false
            );
            (spec.model || []).forEach(function (transform) {
                model = transform(model);
            });
            return model;
        })
    }

    function eventAction(eventName, action) {
        return function ($scope, control) {
            control.addEventListener(eventName, function () {
                $scope.$apply(function () {
                    action($scope, control);
                });
            });
        };
    }

    function eventPropertySet(eventName, property) {
        return eventAction(eventName, function ($scope, control) {
            $scope[property] = control[property];
        });
    }

    function root(element) {
        return element.parentNode ? root(element.parentNode) : element;
    }

    function select(selector, element) {
        return document.querySelector(selector) || root(element).querySelector(selector);
    }

    function anchorUpdate($new, $old, getControl, element) {
        $new = typeof $new === "string" ? select($new, element) : $new;
        $old = typeof $old === "string" ? select($old, element) : $old;
        if ($old && $old._anchorClick) {
            $old.removeEventListener("click", $old._anchorClick);
            $old._anchorClick = null;
        }
        if ($new && !$new._anchorClick) {
            $new._anchorClick = function () { getControl().show(); };
            $new.addEventListener("click", $new._anchorClick);
        }
        return $new;
    }

    function anchorWireup($scope, control) {
        if (control.anchor && control.anchor instanceof HTMLElement && !control.anchor._anchorClick) {
            control.anchor._anchorClick = function () { control.show(); };
            control.anchor.addEventListener("click", control.anchor._anchorClick);
        }
    }

    var WrapperList = WinJS.Class.derive(WinJS.Binding.List, function (array) {
        WinJS.Binding.List.call(this, array);
    });

    function bindingList($new, $old, getControl) {
        if ($new && Array.isArray($new)) {
            $new = new WrapperList($new);
            $new._proxy = true;
        }
        return $new;
    }
    bindingList.watch = function ($scope, key, getValue, initialValue) {
        if (!(initialValue instanceof WrapperList)) {
            // use normal watch, not $watchCollection diffing.
            return;
        }
        return $scope.$watchCollection(key, function (array) {
            var list = getValue(key);
            if (!list) {
                return;
            }
            if (!array) {
                list.length = 0;
                return;
            }
            var targetIndicies = new Map();
            for (var i = 0, len = array.length; i < len; i++) {
                targetIndicies.set(array[i], i);
            }
            var arrayIndex = 0, listIndex = 0;
            while (arrayIndex < array.length) {
                var arrayData = array[arrayIndex];
                if (listIndex >= list.length) {
                    list.push(arrayData);
                } else {
                    while (listIndex < list.length) {
                        var listData = list.getAt(listIndex);
                        if (listData === arrayData) {
                            listIndex++;
                            arrayIndex++;
                            break;
                        } else {
                            if (targetIndicies.has(listData)) {
                                var targetIndex = targetIndicies.get(listData);
                                if (targetIndex < arrayIndex) {
                                    // already in list, remove the duplicate
                                    list.splice(listIndex, 1);
                                } else {
                                    list.splice(listIndex, 0, arrayData);
                                    arrayIndex++;
                                    listIndex++;
                                    break;
                                }
                            } else {
                                // deleted, remove from list
                                list.splice(listIndex, 1);
                            }
                        }
                    }
                }
            }
            // clip any items which are left over in the tail.
            list.length = array.length;
        });
    }

    function dataSource($new, $old, getControl) {
        $new = bindingList($new);
        $new = $new && $new.dataSource ? $new.dataSource : $new;
        return $new;
    }
    dataSource.watch = function ($scope, key, getValue, initialValue) {
        return bindingList.watch($scope, key, function (key) {
            var value = getValue(key);
            return value ? value.list : null;
        }, initialValue ? initialValue.list : null);
    };

    function controller(contentProperties) {
        return function (model) {
            contentProperties.forEach(function (property) {
                model.scope[property] = "=?";
            });
            model.controller = function ($scope) {
                var that = this;
                contentProperties.forEach(function (property) {
                    var _value;
                    Object.defineProperty(that, property, {
                        get: function () { return $scope[property]; },
                        set: function (value) { $scope[property] = value; }
                    });
                })
            };
            return model;
        };
    }

    var controls = {
        "WinJS.UI.AppBar": {
            transclude: true
        },
        "WinJS.UI.AppBarCommand": {
            elementType: "BUTTON",
            transclude: true,
        },
        "WinJS.UI.BackButton": {
            elementType: "BUTTON",
        },
        "WinJS.UI.DatePicker": {
            postLink: [eventPropertySet("change", "current")],
        },
        "WinJS.UI.FlipView": {
            model: [controller(["itemTemplate"])],
            optionsProcessors: {
                itemDataSource: dataSource,
            },
            transclude: true,
        },
        "WinJS.UI.Flyout": {
            optionsProcessors: { anchor: anchorUpdate, },
            postLink: [anchorWireup],
            transclude: true,
        },
        "WinJS.UI.Hub": {
            model: [controller(["headerTemplate"])],
            postLink: [eventPropertySet("loadingstatechanged", "loadingState")],
            transclude: true,
        },
        "WinJS.UI.HubSection": {
            transclude: true
        },
        "WinJS.UI.ItemContainer": {
            postLink: [eventPropertySet("selectionchanged", "selection")],
            transclude: true,
        },
        "WinJS.UI.ListView": {
            // @TODO, can we get things like selection bound?
            model: [controller(["itemTemplate", "groupHeaderTemplate", "layout"])],
            optionsProcessors: {
                itemDataSource: dataSource,
                groupDataSource: dataSource,
            },
            transclude: true,
        },
        "WinJS.UI.Menu": {
            optionsProcessors: { anchor: anchorUpdate, },
            postLink: [anchorWireup],
            transclude: true,
        },
        "WinJS.UI.MenuCommand": {
            elementType: "BUTTON",
        },
        "WinJS.UI.NavBar": {
            transclude: true
        },
        "WinJS.UI.NavBarContainer": {
            model: [controller(["template"])],
            optionsProcessors: {
                data: bindingList,
            },
            transclude: true,
        },
        "WinJS.UI.NavBarCommand": {
            transclude: true
        },
        "WinJS.UI.Rating": {
            postLink: [eventPropertySet("change", "userRating")],
        },
        "WinJS.UI.SearchBox": {
            postLink: [eventPropertySet("querychanged", "queryText")],
        },
        "WinJS.UI.SemanticZoom": {
            transclude: true,
        },
        "WinJS.UI.TimePicker": {
            postLink: [eventPropertySet("change", "current")],
        },
        "WinJS.UI.ToggleSwitch": {
            postLink: [eventPropertySet("change", "checked")],
        },
        "WinJS.UI.Tooltip": {
            model: [controller(["contentElement"])],
            transclude: true,
        },
    };
    Object.keys(controls).forEach(function (key) {
        directive(controlDirectiveModel, key, controls[key]);
    });

    // Tooltop is a little odd because you have to be able to specify both the element
    // which has a tooltip (the content) and the tooltip's content itself. We specify
    // a special directive <win-tooltip-content /> which represents the latter.
    function tooltipContentModel() {
        return {
            require: "^winTooltip",
            restrict: "E",
            transclude: true,
            link: function ($scope, elements, attrs, tooltip) {
                tooltip.contentElement = elements[0].firstElementChild;
            },
            template: "\
<div style='display:none'>\
  <div ng-transclude='true'></div>\
</div>",
            replace: true,
        };
    }
    module.directive("winTooltipContent", tooltipContentModel);

    // Templates are needed for the ListView and FlipView, this makes directives which
    // can be specified within a ListView or FlipView as item or group header templates
    // which themselves simply contain angular bindings.
    function templateDirective(name, parents) {
        module.directive("win" + name[0].toUpperCase() + name.substr(1), function () {
            return {
                require: parents.map(function (item) { return "^?" + item; }),
                restrict: "E",
                compile: function (tElement, tAttrs, transclude) {
                    var rootElement = document.createElement("div");
                    Object.keys(tAttrs).forEach(function (key) {
                        if (key[0] !== '$') {
                            rootElement.setAttribute(key, tAttrs[key]);
                        }
                    });
                    var immediateToken;
                    return function ($scope, elements, attrs, parents) {
                        var parent = parents.reduce(function (found, item) { return found || item; });
                        parent[name] = function (itemPromise) {
                            return itemPromise.then(function (item) {
                                var itemScope = $scope.$new();
                                itemScope.item = item;
                                var result = rootElement.cloneNode(false);
                                transclude(itemScope, function (clonedElement) {
                                    for (var i = 0, len = clonedElement.length; i < len; i++) {
                                        result.appendChild(clonedElement[i]);
                                    }
                                });
                                WinJS.Utilities.markDisposable(result, function () {
                                    itemScope.$destroy();
                                });
                                immediateToken = immediateToken || setImmediate(function () {
                                    immediateToken = null;
                                    itemScope.$apply();
                                });
                                return result;
                            })
                        };
                    };
                },
                replace: true,
                transclude: true,
            };
        });
    }
    templateDirective("itemTemplate", ["winListView", "winFlipView"]);
    templateDirective("groupHeaderTemplate", ["winListView"]);
    // @TODO, make these work
    //templateDirective("headerTemplate", ["winHub"]);
    //templateDirective("template", ["winNavBarContainer"]);

    function layoutDirectiveModel(ctor, createOptions, preLink, postLink) {
        var scopeSpec = getBindableApi(ctor);
        var scopeSpecKeys = Object.keys(scopeSpec);
        return {
            require: "^winListView",
            restrict: "E",
            transclude: false,
            replace: true,
            template: "",
            scope: scopeSpec,
            link: function ($scope, elements, attrs, listView) {
                var layout;
                var options = createOptions($scope, scopeSpecKeys, function () { return layout; }, elements[0]);
                preLink.forEach(function (f) { f($scope, options.options); });
                layout = new ctor(options.options);
                postLink.forEach(function (f) { f($scope, layout); });
                listView.layout = layout;
                var cleanup = options.cleanup;
                $scope.$on("$destroy", function () {
                    if (cleanup) {
                        cleanup();
                    }
                });
                return layout;
            }
        }
    }

    var layouts = {
        "WinJS.UI.CellSpanningLayout": {},
        "WinJS.UI.GridLayout": {},
        "WinJS.UI.ListLayout": {}
    };
    Object.keys(layouts).forEach(function (key) {
        directive(layoutDirectiveModel, key, layouts[key]);
    });

    // This guy is a real odd-ball, you really need to coordinate with the settings 
    // event which fires, I need to think more about this.
    //WinJS.UI.SettingsFlyout;

    // Do not support explicitly, use ng-repeat
    //WinJS.UI.Repeater;

}(this));

