require(['app.module'], function (app) {
    'use strict';
    angular.element(document).ready(function () {
        let a = new app.App();
        angular.bootstrap(document, ['app']);
    })
});
