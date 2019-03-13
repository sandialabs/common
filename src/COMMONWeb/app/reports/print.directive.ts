export interface IPrintScope extends ng.IScope {
    printElementId: string;
}

export class PrintDirective implements ng.IDirective {
    private printSection: HTMLElement;
    private originalElement: HTMLElement;
    private parentElement: HTMLElement;

    restrict: string = 'A';
    scope = {
        printElementId: '@'
    };
    link: ng.IDirectiveLinkFn = (scope: IPrintScope, button: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
        let t = this;
        button.on('click', () => {
            let d: HTMLElement = document.getElementById(scope.printElementId);
            if (d)
                t.printElement(d);
        });
        window.onafterprint = () => {
            t.parentElement.appendChild(t.originalElement);
        }
    };

    constructor() {
        // If there's a 'printSection' div defined in the document, use it.
        // Otherwise, create one and append it to the document.
        this.printSection = document.getElementById('printSection');
        if (!this.printSection) {
            this.printSection = document.createElement('div');
            this.printSection.id = 'printSection';
            document.body.appendChild(this.printSection);
        }
    }

    printElement(element: HTMLElement) {
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
    }

    public static Factory(): ng.IDirectiveFactory {
        let factory = (): ng.IDirective => {
            return new PrintDirective();
        }
        return factory;
    }
}

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
