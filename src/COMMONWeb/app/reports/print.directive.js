define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var PrintDirective = /** @class */ (function () {
        function PrintDirective() {
            var _this = this;
            this.restrict = 'A';
            this.scope = {
                printElementId: '@'
            };
            this.link = function (scope, button, attrs) {
                var t = _this;
                button.on('click', function () {
                    var d = document.getElementById(scope.printElementId);
                    if (d)
                        t.printElement(d);
                });
                window.onafterprint = function () {
                    t.parentElement.appendChild(t.originalElement);
                };
            };
            // If there's a 'printSection' div defined in the document, use it.
            // Otherwise, create one and append it to the document.
            this.printSection = document.getElementById('printSection');
            if (!this.printSection) {
                this.printSection = document.createElement('div');
                this.printSection.id = 'printSection';
                document.body.appendChild(this.printSection);
            }
        }
        PrintDirective.prototype.printElement = function (element) {
            // Move the element to the printSection so the @media CSS things
            // will work right.
            // We can't clone the original element like the original code does
            // because the graphs don't show up right. Instead, let's just
            // move the element to the printSection element, then when the
            // printing has completed it will be moved back.
            // We just have to keep track of the original parent so it can be
            // moved back properly.
            this.originalElement = element;
            this.parentElement = element.parentElement;
            this.printSection.appendChild(element);
            window.print();
        };
        PrintDirective.Factory = function () {
            var factory = function () {
                return new PrintDirective();
            };
            return factory;
        };
        return PrintDirective;
    }());
    exports.PrintDirective = PrintDirective;
});
// Found at: http://blogs.microsoft.co.il/gilf/2014/08/09/building-a-simple-angularjs-print-directive/
//function printDirective() {
//    var printSection = document.getElementById('printSection');
//    // if there is no printing section, create one
//    if (!printSection) {
//        printSection = document.createElement('div');
//        printSection.id = 'printSection';
//        document.body.appendChild(printSection);
//    }
//    function link(scope, element, attrs) {
//        element.on('click', function () {
//            var elemToPrint = document.getElementById(attrs.printElementId);
//            if (elemToPrint) {
//                printElement(elemToPrint);
//            }
//        });
//        window.onafterprint = function () {
//            // clean the print section before adding new content
//            printSection.innerHTML = '';
//        }
//    }
//    function printElement(elem) {
//        // clones the element you want to print
//        var domClone = elem.cloneNode(true);
//        printSection.appendChild(domClone);
//        window.print();
//    }
//    return {
//        link: link,
//        restrict: 'A'
//    };
//}
//# sourceMappingURL=print.directive.js.map