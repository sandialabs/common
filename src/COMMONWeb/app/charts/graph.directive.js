define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var GraphDirective = /** @class */ (function () {
        function GraphDirective($window, $rootScope, chartBridgeFactoryService) {
            var _this = this;
            this.$window = $window;
            this.$rootScope = $rootScope;
            this.chartBridgeFactoryService = chartBridgeFactoryService;
            this.restrict = 'E';
            this.scope = {
                data: '=',
                settings: '='
            };
            this.template = '<canvas></canvas>';
            this.link = function (scope, element, attrs) {
                var context = {
                    element: element,
                    rootScope: _this.$rootScope,
                    scope: scope,
                    window: _this.$window
                };
                var f = _this.chartBridgeFactoryService.$get();
                var cb = f.createChartBridge(scope.data, scope.settings);
                if (cb)
                    cb.makeChart(context);
            };
        }
        GraphDirective.Factory = function () {
            var factory = function ($window, $rootScope, chartBridgeService) {
                return new GraphDirective($window, $rootScope, chartBridgeService);
            };
            factory.$inject = ['$window', '$rootScope', 'chartBridgeFactoryService'];
            return factory;
        };
        return GraphDirective;
    }());
    exports.GraphDirective = GraphDirective;
});
//# sourceMappingURL=graph.directive.js.map